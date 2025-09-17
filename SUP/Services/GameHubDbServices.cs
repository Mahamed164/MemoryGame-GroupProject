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

        const string sqlStmt = @"insert into player(nickname)
                        values (@nickname)
                        on conflict(nickname)
                        do update set nickname = Excluded.nickname
                        returning player_id, nickname";

        try
        {
            await using var connection = await _dataSource.OpenConnectionAsync();
            using var command = new NpgsqlCommand(sqlStmt, connection);
            command.Parameters.AddWithValue("@nickname", nickname);

            await using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Player() { Id = reader.GetInt32(0), Nickname = reader.GetString(1) };
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
