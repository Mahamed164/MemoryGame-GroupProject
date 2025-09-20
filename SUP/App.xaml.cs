using Microsoft.Extensions.Configuration;
using System.Configuration;
using System.Data;
using System.Windows;
using Npgsql;
using SUP.Services;
using SUP.Models;
using SUP.ViewModels;



namespace SUP;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private NpgsqlDataSource _dataSource = null;
    private IAudioService _audio;

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);


        _audio = new NAudioService();

        var memoryGame = new BoardViewModel(_audio);

        var config = new ConfigurationBuilder().AddUserSecrets<App>().Build();
        //.AddUserSecrets<App>().Build();
        var connectionString = config.GetConnectionString("Production");
        _dataSource = NpgsqlDataSource.Create(connectionString);

        var db = new GameHubDbServices(_dataSource);
        var mainShellVm = new MainShellViewModel(db, _audio);

        

        var mainWindow = new MainWindow
        {
            DataContext = mainShellVm
        };
        mainWindow.Show();

    }   
}
