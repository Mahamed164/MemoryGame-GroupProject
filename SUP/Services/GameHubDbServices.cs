using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using SUP.Models;
using PropertyChanged;

namespace SUP.Services;

[AddINotifyPropertyChangedInterface]

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

                //throw new InvalidOperationException("Något gick fel. Kunde inte skapa spelare");
            }
            var selectCommand = new NpgsqlCommand(selectSqlStmt, connection);
            {
                selectCommand.Parameters.AddWithValue("@nickname", nickname);

                await using var selectReader = await selectCommand.ExecuteReaderAsync();
                if(await selectReader.ReadAsync())
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
}
