using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using SUP.Models;
using System.Windows;

namespace SUP.Services;

public class GameHubDbServices
{
    private readonly NpgsqlDataSource _dataSource;

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
                                (select 1 from player where nickname = @nickname)
                                returning player_id, nickname"; //försöker insert, men det funkar bara om nickname inte redan finns

        //statement 2 = hämtar id och nickname på spelare där nickname i databasen är det samma som man skriver in i spelet (@nickname) -- istället för att insert
        const string selectSqlStmt = @"select player_id, nickname
                                    from player
                                    where nickname = @nickname";

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

    public async Task<List<Player>> GetPlayersForHighScoreAsync()
    {
        const string sqlStmt = "select player_id, nickname from player order by nickname";
        var players = new List<Player>();

        await using var connection = await _dataSource.OpenConnectionAsync();
        await using var command = new NpgsqlCommand(sqlStmt, connection);
        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            players.Add(new Player()
            {
                Id = reader.GetInt32(0),
                Nickname = reader.GetString(1)
            });
        }
        return players;
    }

    public async void SaveFullGameSession(DateTime startTime, DateTime endTime, int playerId, int timeAsInt, int numOfMoves, int numOfMisses)
    {
        int sessionId = await SaveAndGetSessionAsync(startTime, endTime);
        SaveSessionParticipantAsync(sessionId, playerId);
        SaveSessionScoreTime(sessionId, playerId, timeAsInt);
        SaveSessionScoreMoves(sessionId, playerId, numOfMoves);
        SaveSessionScoreMisses(sessionId, playerId, numOfMisses);
    }

    public async Task<int> SaveAndGetSessionAsync(DateTime startTime, DateTime endTime)
    {
        try
        {
            int sessionId = 0;

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
            throw;
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
}