using PropertyChanged;
using SUP.Commands;
using SUP.Services;
using SUP.ViewModels.Scores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace SUP.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class MainShellViewModel
    {
        public object CurrentView { get; set; }
        public object LatestView;
        public ICommand StartGameCmd { get; }
        public ICommand FinishGameCmd { get; }
        public ICommand RestartCmd { get; }
        public ICommand SaveScoreCmd { get; }
        public ICommand HighScoreCmd { get; }
        public ICommand SaveCurrentScoreCmd { get; }
        EndViewModel EndViewModel { get; set; }
        private StartViewModel _startview;


        public int Misses { get; set; }
        public int Moves { get; set; }
        public string TimerText { get; set; }
        public string PlayerName {  get; set; }
        public int PlayerID { get; set; }


        private readonly GameHubDbServices _db;


        public MainShellViewModel(GameHubDbServices db)
        {
            StartGameCmd = new RelayCommand(StartGame);
            FinishGameCmd = new RelayCommand(FinishGame);
            RestartCmd = new RelayCommand(RestartGame);
            SaveScoreCmd = new RelayCommand(SaveScore);
            HighScoreCmd = new RelayCommand(OpenHighScores);

            _db = db;

            _startview = new StartViewModel(StartGameCmd, HighScoreCmd);

            CurrentView = _startview;
        }
        public void StartGame(object parameter)
        {
            CurrentView = new MemoryBoardViewModel(FinishGameCmd, RestartCmd, _startview.Level);
        }

        public async void FinishGame(object parameter)

        {
            var player = await _db.GetOrCreatePlayerAsync(_startview.PlayerName);
            PlayerName = player.Nickname;
            PlayerID = player.Id;
            var result = (ValueTuple<int, int, string>)parameter;
            Misses = result.Item1;
            Moves = result.Item2;
            TimerText = result.Item3;
            EndViewModel = new EndViewModel(Misses, Moves, TimerText, SaveScoreCmd, RestartCmd, HighScoreCmd);
            CurrentView = EndViewModel;
        }
        public void RestartGame(object parameter)
        {
            CurrentView = new MemoryBoardViewModel(FinishGameCmd, RestartCmd, _startview.Level);
        }
        public async void SaveScore(object parameter)
        {

            CurrentView = new SaveScoreViewModel(Moves, Misses, TimerText, PlayerName, PlayerID, HighScoreCmd);

            try
            {
                var player = await _db.GetOrCreatePlayerAsync(_startview.PlayerName);

                // Här kan vi utöka att spara utöver namn ex (moves, misses, tid osv) kanske?

                MessageBox.Show($"Ditt namn '{player.Nickname}' har sparats i databasen!", "Sparat");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Kunde inte spara resultatet. Fel: " + ex.Message, "Fel");
            }
        }
        public async void OpenHighScores(object parameter)
        {
            LatestView = CurrentView;
            var players = await _db.GetPlayersForHighScoreAsync();
            CurrentView = new HighScoreViewModel(new RelayCommand(p =>
            {
                CurrentView = LatestView;
            }), players);
        }
    }
}
