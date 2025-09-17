using Microsoft.Extensions.Configuration;
using System.Configuration;
using System.Data;
using System.Windows;
using Npgsql;
using SUP.Services;
using SUP.Models;



namespace SUP;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private NpgsqlDataSource _dataSource = null;

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        var config = new ConfigurationBuilder().AddUserSecrets<App>().Build();
        //.AddUserSecrets<App>().Build();
        var connectionString = config.GetConnectionString("Production");
        _dataSource = NpgsqlDataSource.Create(connectionString);

        var db = new GameHubDbServices(_dataSource);
        var player = await db.GetOrCreatePlayerAsync("test2");

    }
}
