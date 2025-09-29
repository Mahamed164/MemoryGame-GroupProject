using Npgsql;
using SUP.Models;
using SUP.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace SUP.Services;

public class GameHubDbServices
{
    private readonly NpgsqlDataSource _dataSource;

    public StartViewModel StartVM { get; set; }

    public GameHubDbServices(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }
    /// <summary>
    /// skapar eller hämtar spelare utifrån nickname :) kopierat från YT
    /// </summary>
    /// <param name="nickname"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public async Task<Player> GetOrCreatePlayerAsync(string nickname)
    {
        


        if (string.IsNullOrWhiteSpace(nickname)) throw new ArgumentNullException(nameof(nickname)); //guard

        //statement 1 = om det inte redan finns en nickname som är det man skriver in
        const string trySqlStmt = @"insert into player(nickname) 
                                 select @nickname where not exists
                                (select 1 from player where lower(nickname) = lower(@nickname))
                                returning player_id, nickname"; 

        //statement 2 = hämtar id och nickname på spelare där nickname i databasen är det samma som man skriver in i spelet (@nickname) -- istället för att insert
        const string selectSqlStmt = @"select player_id, nickname
                                    from player
                                    where LOWER(nickname) = LOWER(@nickname)";


        try
        {
            await using var connection = await _dataSource.OpenConnectionAsync();
            using var tryCommand = new NpgsqlCommand(trySqlStmt, connection);
            {
                tryCommand.Parameters.AddWithValue("@nickname", nickname);

                await using var reader = await tryCommand.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    return new Player() { Id = reader.GetInt32(0), Nickname = reader.GetString(1) };
                }
            }

            var selectCommand = new NpgsqlCommand(selectSqlStmt, connection);
            {
                selectCommand.Parameters.AddWithValue("@nickname", nickname);

                await using var selectReader = await selectCommand.ExecuteReaderAsync();
                if (await selectReader.ReadAsync())
                {
                    return new Player() { Id = selectReader.GetInt32(0), Nickname = selectReader.GetString(1) };
                }
            }
            throw new InvalidOperationException("Något gick fel. Kunde inte skapa spelare");
        }

        catch (PostgresException ex)
        {

            throw new InvalidOperationException("Kunde inte skapa/hämta en spelare", ex);
        }
    }

    public async Task<int> GetNewSessionId(Result currentResult)
    {
        int sessionId = await SaveAndGetSessionAsync(currentResult);
        return sessionId;
    }

    public async Task<bool> SaveFullGameSession(IdForPlayerAndSession idForPlayerAndSession, Result currentResult)
    {
        bool sessionSaved;
        bool isSessionNew = await IsNewSession(idForPlayerAndSession);

        if (isSessionNew == true)
        {
            SaveSessionParticipantAsync(idForPlayerAndSession);
            SaveSessionScoreTime(idForPlayerAndSession, currentResult);
            SaveSessionScoreMoves(idForPlayerAndSession, currentResult);
            SaveSessionScoreMisses(idForPlayerAndSession, currentResult);
            SaveSessionScoreLevel(idForPlayerAndSession, currentResult);
            sessionSaved = true;
        }
        else
        {
            sessionSaved = false;
        }
            return sessionSaved;
    }

    public async Task<bool> IsNewSession(IdForPlayerAndSession idForPlayerAndSession)
    {

        //https://stackoverflow.com/questions/17903092/check-if-record-in-a-table-exist-in-a-database-through-executenonquery
        bool isSessionNew = true;
        int existingSession = 0;
        try
        {
            await using var connection = await _dataSource.OpenConnectionAsync();
            await using var command = new NpgsqlCommand("select session.session_id, count(*) from public.session join  session_score on session_score.session_id = session.session_id where session.session_id = @SESSION_ID group by session.session_id", connection);

            command.Parameters.AddWithValue("SESSION_ID", idForPlayerAndSession.SessionId);


            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    existingSession = reader.GetInt32(0);
                    isSessionNew = false;
                }
                
            }

            return isSessionNew;
        }
        catch (Exception ex)
        {
            throw;
        }

        return isSessionNew;
    }

    public async Task<int> SaveAndGetSessionAsync(Result currentResult)
    {
        int sessionId = 0;
        try
        {

            await using var connection = await _dataSource.OpenConnectionAsync();
            await using var command = new NpgsqlCommand("INSERT INTO PUBLIC.SESSION " +
                "(GAME_ID, STARTED_AT, ENDED_AT) " +
                "VALUES " +
                "((SELECT GAME_ID FROM GAME G WHERE NAME = 'G2Memory'), " +
                "@STARTDATE, " +
                "@ENDDATE) " +
                "RETURNING SESSION_ID", connection);

            command.Parameters.AddWithValue("STARTDATE", currentResult.StartTime);
            command.Parameters.AddWithValue("ENDDATE", currentResult.EndTime);

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    sessionId = (int)reader["session_id"];
                }
            }
            if (sessionId == 0)
            {
                MessageBox.Show("Session_Id fel");
            }
            return sessionId;
        }
        catch (Exception ex)
        {
            return 0;
        }
    }

    public async void SaveSessionParticipantAsync(IdForPlayerAndSession idForPlayerAndSession)
    {
        try
        {
            await using var connection = await _dataSource.OpenConnectionAsync();
            await using var command = new NpgsqlCommand("INSERT INTO SESSION_PARTICIPANT(SESSION_ID, PLAYER_ID) VALUES(@SESSION_ID, @PLAYER_ID)", connection);

            command.Parameters.AddWithValue("SESSION_ID", idForPlayerAndSession.SessionId);
            command.Parameters.AddWithValue("PLAYER_ID", idForPlayerAndSession.PlayerId);

            var result = await command.ExecuteNonQueryAsync();
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    public async void SaveSessionScoreTime(IdForPlayerAndSession idForPlayerAndSession, Result currentResult)
    {
        //https://learn.microsoft.com/en-us/dotnet/api/system.timespan.tryparseexact?view=net-9.0
        int timeAsInt = 0;
        if (TimeSpan.TryParseExact(currentResult.TimerText, @"mm\:ss", null, out var span))
        {
            timeAsInt = (int)span.TotalSeconds;
        }


        try
        {
            await using var connection = await _dataSource.OpenConnectionAsync();
            await using var command = new NpgsqlCommand("INSERT INTO SESSION_SCORE (SESSION_ID, PLAYER_ID, SCORE_TYPE_ID, VALUE) " +
                "VALUES (@SESSION_ID, @PLAYER_ID, (SELECT SCORE_TYPE_ID FROM SCORE_TYPE WHERE CODE = 'Time'), @TIME_AS_INT)", connection);

            command.Parameters.AddWithValue("SESSION_ID", idForPlayerAndSession.SessionId);
            command.Parameters.AddWithValue("PLAYER_ID", idForPlayerAndSession.PlayerId);
            command.Parameters.AddWithValue("TIME_AS_INT", timeAsInt);

            var result = await command.ExecuteNonQueryAsync();
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    public async void SaveSessionScoreMoves(IdForPlayerAndSession idForPlayerAndSession, Result currentResult)
    {
        try
        {
            await using var connection = await _dataSource.OpenConnectionAsync();
            await using var command = new NpgsqlCommand("INSERT INTO SESSION_SCORE (SESSION_ID, PLAYER_ID, SCORE_TYPE_ID, VALUE) " +
                "VALUES (@SESSION_ID, @PLAYER_ID, (SELECT SCORE_TYPE_ID FROM SCORE_TYPE WHERE CODE = 'Moves'), @NUMOFMOVES)", connection);

            command.Parameters.AddWithValue("SESSION_ID", idForPlayerAndSession.SessionId);
            command.Parameters.AddWithValue("PLAYER_ID", idForPlayerAndSession.PlayerId);
            command.Parameters.AddWithValue("NUMOFMOVES", currentResult.Guesses);

            var result = await command.ExecuteNonQueryAsync();
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    public async void SaveSessionScoreMisses(IdForPlayerAndSession idForPlayerAndSession, Result currentResult)
    {
        try
        {
            await using var connection = await _dataSource.OpenConnectionAsync();
            await using var command = new NpgsqlCommand("INSERT INTO SESSION_SCORE (SESSION_ID, PLAYER_ID, SCORE_TYPE_ID, VALUE) " +
                "VALUES (@SESSION_ID, @PLAYER_ID, (SELECT SCORE_TYPE_ID FROM SCORE_TYPE WHERE CODE = 'Misses'), @NUMOFMISSES)", connection);

            command.Parameters.AddWithValue("SESSION_ID", idForPlayerAndSession.SessionId);
            command.Parameters.AddWithValue("PLAYER_ID", idForPlayerAndSession.PlayerId);
            command.Parameters.AddWithValue("NUMOFMISSES", currentResult.Misses);

            var result = await command.ExecuteNonQueryAsync();
        }
        catch (Exception ex)
        {
            throw;
        }
    }
    public async void SaveSessionScoreLevel(IdForPlayerAndSession idForPlayerAndSession, Result currentResult)
    {
        try
        {
            await using var connection = await _dataSource.OpenConnectionAsync();
            await using var command = new NpgsqlCommand("INSERT INTO SESSION_SCORE (SESSION_ID, PLAYER_ID, SCORE_TYPE_ID, VALUE) " +
                "VALUES (@SESSION_ID, @PLAYER_ID, (SELECT SCORE_TYPE_ID FROM SCORE_TYPE WHERE CODE = 'Level'), @Selected_Level) ", connection);

            command.Parameters.AddWithValue("SESSION_ID", idForPlayerAndSession.SessionId);
            command.Parameters.AddWithValue("PLAYER_ID", idForPlayerAndSession.PlayerId);
            command.Parameters.AddWithValue("Selected_Level", currentResult.Level);

            var result = await command.ExecuteNonQueryAsync();
        }
        catch (Exception ex)
        {
            throw;
        }

    }



    public async Task<ObservableCollection<SessionScores>> GetHighScoreList(int level)
    {
        try
        {
            ObservableCollection<SessionScores> _sessionScores = new ObservableCollection<SessionScores>();
            SessionScores sessionScore = null;

            await using var connection = await _dataSource.OpenConnectionAsync();


            using var command = new NpgsqlCommand(
                "SELECT SS.SESSION_ID, P.NICKNAME AS NICKNAMES, S.STARTED_AT AS TIME_OF_PLAY, " +
                "SUM(CASE WHEN SS.SCORE_TYPE_ID = 4 THEN SS.VALUE END) AS MOVES, " +
                "SUM(CASE WHEN SS.SCORE_TYPE_ID = 5 THEN SS.VALUE END) AS MISSES, " +
                "SUM(CASE WHEN SS.SCORE_TYPE_ID = 3 THEN SS.VALUE END) AS TIME, " +
                "SUM(CASE WHEN SS.SCORE_TYPE_ID = 6 THEN SS.VALUE END) AS LEVEL " +
                "FROM SESSION_SCORE SS " +
                "JOIN PLAYER P ON SS.PLAYER_ID = P.PLAYER_ID " +
                "JOIN PUBLIC.SESSION S ON S.SESSION_ID = SS.SESSION_ID " +
                "GROUP BY SS.SESSION_ID, P.NICKNAME, S.STARTED_AT " +
                "HAVING SUM(CASE WHEN SS.SCORE_TYPE_ID = 6 THEN SS.VALUE END) = @Level " +
                "ORDER BY MOVES ASC, MISSES ASC, TIME ASC LIMIT 100", connection);

            command.Parameters.AddWithValue("Level", level);


            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    int seconds = Convert.ToInt32(reader["time"]);
                    string timer = TimeSpan.FromSeconds(seconds).ToString(@"mm\:ss");
                    sessionScore = new SessionScores
                    {
                        SessionId = Convert.ToInt32(reader["session_id"]),
                        Nickname = reader["nicknames"].ToString(),
                        TimeOfPlay = (DateTime)reader["time_of_play"],
                        Moves = Convert.ToInt32(reader["moves"]),
                        Misses = Convert.ToInt32(reader["misses"]),
                        Level = Convert.ToInt32(reader["level"]),
                        TimerText = timer,
                    }
                ;
                    _sessionScores.Add(sessionScore);

                }
            }

            return _sessionScores;
        }
        catch (Exception ex)
        {
            throw;
        }
    }
}