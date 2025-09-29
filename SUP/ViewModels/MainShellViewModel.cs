using PropertyChanged;
using SUP.Commands;
using SUP.Models;
using SUP.Services;
using SUP.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Xml.Serialization;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace SUP.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class MainShellViewModel
    {
        // Commands
        public ICommand StartGameCmd { get; }
        public ICommand FinishGameCmd { get; }
        public ICommand RestartCmd { get; }
        public ICommand SaveScoreCmd { get; }
        public ICommand HighScoreCmd { get; }
        public ICommand SaveCurrentScoreCmd { get; }
        public ICommand BackToStartCmd { get; }
        public ICommand ReturnCmd { get; }
        public ICommand RulesCmd { get; }


        //Ljud - Från human benchmark video 8
        public double MusicVolume { get; set; } = 0.25;
        public double SfxVolume { get; set; } = 0.50;
        public bool MusicMuted { get; set; } = false;
        public bool SfxMuted { get; set; } = false;


        EndViewModel EndViewModel { get; set; }
        public object CurrentView { get; set; }
        public string PlayerName { get; set; }
        public Result CurrentResult { get; set; }

        private readonly GameHubDbServices _db;
        private readonly IAudioService _audio;
        private StartViewModel _startview;
        public object LatestView;
        IdForPlayerAndSession IdForPlayerAndSession { get; set; }

        private void OnMusicVolumeChanged() { _audio.SetMusicVolume((float)MusicVolume); }
        private void OnSfxVolumeChanged() { _audio.SetSfxVolume((float)SfxVolume); }
        private void OnMusicMutedChanged() { _audio.SetMusicMuted(MusicMuted); }
        private void OnSfxMutedChanged() { _audio.SetSfxMuted(SfxMuted); }

        public MainShellViewModel(GameHubDbServices db, IAudioService audioService)
        {
            StartGameCmd = new RelayCommand(StartGame);
            FinishGameCmd = new RelayCommand(FinishGame);
            RestartCmd = new RelayCommand(RestartGame);
            SaveScoreCmd = new RelayCommand(SaveScore);
            HighScoreCmd = new RelayCommand(OpenHighScores);
            BackToStartCmd = new RelayCommand(BackToStart);
            ReturnCmd = new RelayCommand(Return);
            RulesCmd = new RelayCommand(OpenRules);

            _db = db;

            IdForPlayerAndSession = new IdForPlayerAndSession(0, 0);

            _startview = new StartViewModel(StartGameCmd, HighScoreCmd, RulesCmd, db);

            CurrentView = _startview;
            _audio = audioService;

            _audio.SetMusicLoop("Assets/Sounds/game-mode-on-356552.mp3", autoPlay: true);
            ApplyAudioSettings();
        }

        private void ApplyAudioSettings() // Metod för att höja, sänka och mute/unmute
        {
            _audio.SetMusicVolume((float)MusicVolume);
            _audio.SetSfxVolume((float)SfxVolume);
            _audio.SetMusicMuted(MusicMuted);
            _audio.SetSfxMuted(SfxMuted);
        }

        public MainShellViewModel()
        {
        }

        private Regex regex = new Regex(@"^[0-9A-Za-z.\s_-]+$"); //https://stackoverflow.com/questions/13353663/what-is-the-regular-expression-to-allow-uppercase-lowercase-alphabetical-charac
        private readonly string regexString = $"Tillåtna specialtecken: 0-9 . _ -";
        private int maxLenght = 20;

        public string? ControlPlayerNameMessage(string inputName) //kan vara null
        {
            string nameToLong = "För långt namn, max antal karaktärer är 20";
            string nameSpace = "Ej mellanslag före eller efter namn";
            string nameRegex = $"Ej tillåtna tecken! \r\n{regexString}";
            string nameSpaceRegex = $"Ej mellanslag före eller efter namn.  \n \n" +
                $"Tillåtna specialtecken: 0-9 . _ -";

            if (inputName.Length > maxLenght)
            {
                return nameToLong;
            }

            else if (inputName.StartsWith(" ") || inputName.EndsWith(" ") && !regex.IsMatch(inputName))
            {
                return nameSpaceRegex;
            }

            else if (inputName.StartsWith(" ") || inputName.EndsWith(" "))
            {
                return nameSpace;
            }

            else if (inputName.StartsWith(" ") && inputName.EndsWith(" "))
            {
                return nameSpace;
            }

            else if (!regex.IsMatch(inputName))
            {
                return nameRegex;
            }
            return null;
        }

        public async void StartGame(object parameter)
        {
            if (_startview.IsMultiplayerSelected)
            {
                CurrentView = new BoardViewModel(FinishGameCmd, RestartCmd, _startview.Level, _startview.GetPlayerList(), BackToStartCmd, _audio);
                return;
            }

            var player = await _db.GetOrCreatePlayerAsync(_startview.PlayerName);
            PlayerName = player.Nickname;

            var playerNameMessage = ControlPlayerNameMessage(PlayerName);
            _startview.PlayerNameMessage = playerNameMessage;
            if (playerNameMessage != null)
            {
                return;
            }
            else
            {
                CurrentView = new BoardViewModel(FinishGameCmd, RestartCmd, _startview.Level, _startview.GetPlayerList(), BackToStartCmd, _audio);
            }
        }

        public async void FinishGame(object parameter)
        {
            var result = (ValueTuple<Result, PlayerInformation[]>)parameter;

            CurrentResult = result.Item1;
            var players = result.Item2;

            if (_startview.IsMultiplayerSelected)
            {
                PlayerInformation winningPlayer = GetMultiplayerWinner(players);

                EndViewModel = MultiplayerEndViewModel(winningPlayer);
            }
            else
            {
                
                var player = await _db.GetOrCreatePlayerAsync(_startview.PlayerName);
                PlayerName = player.Nickname;
                IdForPlayerAndSession.PlayerId = player.Id;
                IdForPlayerAndSession.SessionId = 0;

                EndViewModel = await SingleplayerEndViewModel();
            }
            CurrentView = EndViewModel;
        }

        private async Task<EndViewModel> SingleplayerEndViewModel()
        {
            return new EndViewModel(CurrentResult, false, SaveScoreCmd, RestartCmd, HighScoreCmd, BackToStartCmd, null, _audio);
        }

        private EndViewModel MultiplayerEndViewModel(PlayerInformation winningPlayer)
        {
            return new EndViewModel(CurrentResult, true, null, RestartCmd, null, BackToStartCmd, winningPlayer, _audio);
        }

        private static PlayerInformation GetMultiplayerWinner(PlayerInformation[] players)
        {
            PlayerInformation winningPlayer = players[0];
            for (int i = 1; i < players.Length; i++)
            {
                if (players[i].Accuracy > winningPlayer.Accuracy)
                {
                    winningPlayer = players[i];
                }
                else if (players[i].Accuracy == winningPlayer.Accuracy && players[i].Guesses < winningPlayer.Guesses) //tiebreaker på antal guessas
                {
                    winningPlayer = players[i];
                }
            }
            return winningPlayer;
        }

        public void RestartGame(object parameter)
        {
            CurrentView = new BoardViewModel(FinishGameCmd, RestartCmd, _startview.Level, _startview.GetPlayerList(), BackToStartCmd, _audio);
        }
        public void OpenRules(object parameter)
        {
            CurrentView = new RulesViewModel(BackToStartCmd);
        }
        public async void SaveScore(object parameter)
        {
            if (IdForPlayerAndSession.SessionId == 0)
            {
                IdForPlayerAndSession.SessionId = await _db.GetNewSessionId(CurrentResult);
            }
            await ShowSaveScoreMessage();
        }

        private async Task ShowSaveScoreMessage()
        {
            if (IdForPlayerAndSession.SessionId != 0)
            {
                var player = await _db.GetOrCreatePlayerAsync(_startview.PlayerName);
                bool sessionSaved = await _db.SaveFullGameSession(IdForPlayerAndSession, CurrentResult);
                if (sessionSaved == true)
                {
                    MessageBox.Show($"Ditt resultat har sparats i topplistan, '{player.Nickname}'!", "Sparat");
                }
                else
                {
                    MessageBox.Show("Ditt resultat har redan sparats i topplistan!", "Ok");
                }
            }
            else
            {
                MessageBox.Show("Session kan inte sparas i offlineläge");
            }
        }

        //private void ParseTime()
        //{
        //    string format = @"mm\:ss";
        //    int timeAsInt = 0;
        //    if (TimeSpan.TryParseExact(TimerText, format, null, out var span))
        //    {
        //        timeAsInt = (int)span.TotalSeconds;
        //    }
        //}

        public async void OpenHighScores(object parameter)
        {
            ObservableCollection<SessionScores> level1Scores = await _db.GetHighScoreList(1);
            ObservableCollection<SessionScores> level2Scores = await _db.GetHighScoreList(2);
            ObservableCollection<SessionScores> level3Scores = await _db.GetHighScoreList(3);

            ObservableCollection<ObservableCollection<SessionScores>> allHighScores = new ObservableCollection<ObservableCollection<SessionScores>>
            {
                level1Scores,
                level2Scores,
                level3Scores
            };
            LatestView = CurrentView;

            CurrentView = new HighScoreViewModel(ReturnCmd, allHighScores);
        }
        public void BackToStart(object parameter)
        {
            _startview.PlayerName = "";
            _startview.PlayerList.Clear();
            _startview.Greeting = "Spelarnamn:";

            CurrentView = _startview;
        }
        public void Return(object parameter)
        {
            CurrentView = LatestView;
        }
    }
}