using Npgsql;
using SUP.Models;
using SUP.ViewModels;
using System;
using System.Collections.Generic;
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
    int sessionId = 0;

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
        //condition för singleplayer // multiplayer


        if (string.IsNullOrWhiteSpace(nickname)) throw new ArgumentNullException(nameof(nickname)); //guard

        //statement 1 = om det inte redan finns en nickname som är det man skriver in
        const string trySqlStmt = @"insert into player(nickname) 
                                 select @nickname where not exists
                                (select 1 from player where lower(nickname) = lower(@nickname))
                                returning player_id, nickname"; //försöker insert, men det funkar bara om nickname inte redan finns

        //statement 2 = hämtar id och nickname på spelare där nickname i databasen är det samma som man skriver in i spelet (@nickname) -- istället för att insert
        const string selectSqlStmt = @"select player_id, nickname
                                    from player
                                    where LOWER(nickname) = LOWER(@nickname)";

        /* https://stackoverflow.com/questions/1952922/how-to-insert-a-record-or-update-if-it-already-exists
         * https://www.sqltutorial.net/not-exists.html -- where not exists: "The subquery must return no result for the NOT EXISTS operator to be true. 
         *                                                                   If the subquery returns any result, the NOT EXISTS operator is false, and the outer query will not return any rows."
                                                                            
        Gamla statement (har även använts som källa/inspiration): 
        @"insert into player(nickname)
        values (@nickname)
        on duplicate key(nickname)
        do update set nickname = excluded.nickname
        returing player_id, nickname";
          */

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

    //public async Task<List<Player>> GetPlayersForHighScoreAsync()
    //{
    //    const string sqlStmt = "select player_id, nickname from player order by nickname";
    //    var players = new List<Player>();

    //    await using var connection = await _dataSource.OpenConnectionAsync();
    //    await using var command = new NpgsqlCommand(sqlStmt, connection);
    //    using var reader = await command.ExecuteReaderAsync();

    //    while (await reader.ReadAsync())
    //    {
    //        players.Add(new Player()
    //        {
    //            Id = reader.GetInt32(0),
    //            Nickname = reader.GetString(1)
    //        });
    //    }
    //    return players;
    //}

    public async Task<int> GetNewSessionId(DateTime startTime, DateTime endTime)
    {
        int sessionId = await SaveAndGetSessionAsync(startTime, endTime);
        return sessionId;
    }

    public async Task<bool> SaveFullGameSession(int sessionId, DateTime startTime, DateTime endTime, int playerId, int timeAsInt, int numOfMoves, int numOfMisses, int selectedLevel)
    {
        bool sessionSaved;
        bool isSessionNew = await IsNewSession(sessionId);

        if (isSessionNew == true)
        {
            SaveSessionParticipantAsync(sessionId, playerId);
            SaveSessionScoreTime(sessionId, playerId, timeAsInt);
            SaveSessionScoreMoves(sessionId, playerId, numOfMoves);
            SaveSessionScoreMisses(sessionId, playerId, numOfMisses);
            SaveSessionScoreLevel(sessionId, playerId, selectedLevel);
            sessionSaved = true;
        }
        else
        {
            sessionSaved = false;
        }
            return sessionSaved;
    }

    public async Task<bool> IsNewSession(int sessionId)
    {

        //https://stackoverflow.com/questions/17903092/check-if-record-in-a-table-exist-in-a-database-through-executenonquery
        bool isSessionNew = true;
        int existingSession = 0;
        try
        {
            await using var connection = await _dataSource.OpenConnectionAsync();
            await using var command = new NpgsqlCommand("select session.session_id, count(*) from public.session join  session_score on session_score.session_id = session.session_id where session.session_id = @SESSION_ID group by session.session_id", connection);

            command.Parameters.AddWithValue("SESSION_ID", sessionId);


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

    public async Task<int> SaveAndGetSessionAsync(DateTime startTime, DateTime endTime)
    {
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

            command.Parameters.AddWithValue("STARTDATE", startTime);
            command.Parameters.AddWithValue("ENDDATE", endTime);

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

    public async void SaveSessionParticipantAsync(int sessionId, int playerId)
    {
        try
        {
            await using var connection = await _dataSource.OpenConnectionAsync();
            await using var command = new NpgsqlCommand("INSERT INTO SESSION_PARTICIPANT(SESSION_ID, PLAYER_ID) VALUES(@SESSION_ID, @PLAYER_ID)", connection);

            command.Parameters.AddWithValue("SESSION_ID", sessionId);
            command.Parameters.AddWithValue("PLAYER_ID", playerId);

            var result = await command.ExecuteNonQueryAsync();
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    public async void SaveSessionScoreTime(int sessionId, int playerId, int timeAsInt)
    {
        try
        {
            await using var connection = await _dataSource.OpenConnectionAsync();
            await using var command = new NpgsqlCommand("INSERT INTO SESSION_SCORE (SESSION_ID, PLAYER_ID, SCORE_TYPE_ID, VALUE) " +
                "VALUES (@SESSION_ID, @PLAYER_ID, (SELECT SCORE_TYPE_ID FROM SCORE_TYPE WHERE CODE = 'Time'), @TIME_AS_INT)", connection);

            command.Parameters.AddWithValue("SESSION_ID", sessionId);
            command.Parameters.AddWithValue("PLAYER_ID", playerId);
            command.Parameters.AddWithValue("TIME_AS_INT", timeAsInt);

            var result = await command.ExecuteNonQueryAsync();
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    public async void SaveSessionScoreMoves(int sessionId, int playerId, int numOfMoves)
    {
        try
        {
            await using var connection = await _dataSource.OpenConnectionAsync();
            await using var command = new NpgsqlCommand("INSERT INTO SESSION_SCORE (SESSION_ID, PLAYER_ID, SCORE_TYPE_ID, VALUE) " +
                "VALUES (@SESSION_ID, @PLAYER_ID, (SELECT SCORE_TYPE_ID FROM SCORE_TYPE WHERE CODE = 'Moves'), @NUMOFMOVES)", connection);

            command.Parameters.AddWithValue("SESSION_ID", sessionId);
            command.Parameters.AddWithValue("PLAYER_ID", playerId);
            command.Parameters.AddWithValue("NUMOFMOVES", numOfMoves);

            var result = await command.ExecuteNonQueryAsync();
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    public async void SaveSessionScoreMisses(int sessionId, int playerId, int numOfMisses)
    {
        try
        {
            await using var connection = await _dataSource.OpenConnectionAsync();
            await using var command = new NpgsqlCommand("INSERT INTO SESSION_SCORE (SESSION_ID, PLAYER_ID, SCORE_TYPE_ID, VALUE) " +
                "VALUES (@SESSION_ID, @PLAYER_ID, (SELECT SCORE_TYPE_ID FROM SCORE_TYPE WHERE CODE = 'Misses'), @NUMOFMISSES)", connection);

            command.Parameters.AddWithValue("SESSION_ID", sessionId);
            command.Parameters.AddWithValue("PLAYER_ID", playerId);
            command.Parameters.AddWithValue("NUMOFMISSES", numOfMisses);

            var result = await command.ExecuteNonQueryAsync();
        }
        catch (Exception ex)
        {
            throw;
        }
    }
    public async void SaveSessionScoreLevel(int sessionId, int playerId, int selectedLevel)
    {
        try
        {
            await using var connection = await _dataSource.OpenConnectionAsync();
            await using var command = new NpgsqlCommand("INSERT INTO SESSION_SCORE (SESSION_ID, PLAYER_ID, SCORE_TYPE_ID, VALUE) " +
                "VALUES (@SESSION_ID, @PLAYER_ID, (SELECT SCORE_TYPE_ID FROM SCORE_TYPE WHERE CODE = 'Level'), @Selected_Level) ", connection);

            command.Parameters.AddWithValue("SESSION_ID", sessionId);
            command.Parameters.AddWithValue("PLAYER_ID", playerId);
            command.Parameters.AddWithValue("Selected_Level", selectedLevel);

            var result = await command.ExecuteNonQueryAsync();
        }
        catch (Exception ex)
        {
            throw;
        }

    }



    public async Task<List<SessionScores>> GetHighScoreList(int level)
    {
        try
        {
            List<SessionScores> _sessionScores = new List<SessionScores>();
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