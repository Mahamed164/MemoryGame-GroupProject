using PropertyChanged;
using SUP.Commands;
using SUP.Models;
using SUP.Services;
using SUP.ViewModels.Scores;
using SUP.Views;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
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
        //från humanbenchmark video 8
        public double MusicVolume { get; set; } = 0.25;
        public double SfxVolume { get; set; } = 0.50;
        public bool MusicMuted { get; set; } = false;
        public bool SfxMuted { get; set; } = false;

        public object CurrentView { get; set; }
        public object LatestView;

        public ICommand StartGameCmd { get; }
        public ICommand FinishGameCmd { get; }
        public ICommand RestartCmd { get; }
        public ICommand SaveScoreCmd { get; }
        public ICommand HighScoreCmd { get; }
        public ICommand SaveCurrentScoreCmd { get; }
        public ICommand BackToStartCmd { get; }
        public ICommand ReturnCmd { get; }
        public ICommand RulesCmd { get; }


        EndViewModel EndViewModel { get; set; }
        private StartViewModel _startview;


        public int Misses { get; set; }
        public int Moves { get; set; }

        public string TimerText { get; set; }
        public string PlayerName { get; set; }
        public int PlayerID { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int SelectedLevel { get; set; }
        public int CurrentSessionId { get; set; }

        private readonly GameHubDbServices _db;
        private readonly IAudioService _audio;
        List<SessionScores> _highScores;

        //från human benchmark video 8
        private void OnMusicVolumeChanged() { _audio.SetMusicVolume((float)MusicVolume); }
        private void OnSfxVolumeChanged() { _audio.SetSfxVolume((float)SfxVolume); }
        private void OnMusicMutedChanged() { _audio.SetMusicMuted(MusicMuted); }
        private void OnSfxMutedChanged() { _audio.SetSfxMuted(SfxMuted); }


        //"efterlys stegljud"

        public MainShellViewModel(GameHubDbServices db, IAudioService audioService)
        {
            StartGameCmd = new RelayCommand(StartGame);
            FinishGameCmd = new RelayCommand(FinishGame);
            RestartCmd = new RelayCommand(RestartGame);
            SaveScoreCmd = new RelayCommand(SaveScore);
            HighScoreCmd = new RelayCommand(OpenHighScores);
            BackToStartCmd = new RelayCommand(BackToStart);
            ReturnCmd = new RelayCommand(Return);

            RulesCmd = new RelayCommand(_ =>
            {
                CurrentView = new RulesViewModel(BackToStartCmd);
            });


            _db = db;

            _startview = new StartViewModel(StartGameCmd, HighScoreCmd, RulesCmd, db);

            CurrentView = _startview;
            _audio = audioService;

            _audio.SetMusicLoop("Assets/Sounds/game-mode-on-356552.mp3", autoPlay: true);
            ApplyAudioSettings();
        }

        private void ApplyAudioSettings() //metod för att höja, sänka och mute/unmute
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

        public string? GetPlayerNameMessage(string inputName) //kan vara null
        {
            //string meddelanden som ska kopplas bindings till i startview

            string nameToLong = "För långt namn, max antal karaktärer är 20";
            string nameSpace = "Ej mellanslag innan eller efter namn";
            string nameRegex = $"Ej tillånta tecken. \r\n{regexString}";
            string nameSpaceRegex = $"Ej mellanslag innan eller efter namn.  \n \n" +
                $"Tillåtna specialtecken: 0-9 . _ -";



            if (inputName.Length > maxLenght)
            {
                //_startview.PlayerNameMessage = nameToLong;
                //MessageBox.Show(nameToLong);
                return nameToLong;
            }

            else if (inputName.StartsWith(" ") || inputName.EndsWith(" ") && !regex.IsMatch(inputName))
            {
                //_startview.PlayerNameMessage = nameSpaceRegex;
                //MessageBox.Show(nameSpaceRegex);
                return nameSpaceRegex;
            }

            else if (inputName.StartsWith(" ") || inputName.EndsWith(" "))
            {
                //_startview.PlayerNameMessage = nameSpace;
                //MessageBox.Show(nameSpace);
                return nameSpace;
            }

            else if (inputName.StartsWith(" ") && inputName.EndsWith(" "))
            {
                //_startview.PlayerNameMessage = nameSpace;
                //MessageBox.Show(nameSpace);
                return nameSpace;
            }

            else if (!regex.IsMatch(inputName))
            {
                //_startview.PlayerNameMessage = nameRegex;
                //MessageBox.Show(nameRegex);
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

            //int maxLenght = 20;

            var player = await _db.GetOrCreatePlayerAsync(_startview.PlayerName);
            PlayerName = player.Nickname;

            var playerNameMessage = GetPlayerNameMessage(PlayerName);
            if (playerNameMessage != null)
            {
                _startview.PlayerNameMessage = playerNameMessage;
                return;
            }

            else
            {
                CurrentView = new BoardViewModel(FinishGameCmd, RestartCmd, _startview.Level, _startview.GetPlayerList(), BackToStartCmd, _audio);

            }

        }

        //public async void AddPlayer()
        //{

        //    var player = await _db.GetOrCreatePlayerAsync(PlayerName);
        //    PlayerName = player.Nickname;

        //    var playerNameMessage = GetPlayerNameMessage(PlayerName);
        //    if (playerNameMessage != null)
        //    {

        //        _startview.MultiPlayerNameMessage = playerNameMessage;
        //        return;
        //    }
        //    else
        //    {
        //        MessageBox.Show("addplayer test");
        //    }

        //}

        //public ICommand AddPlayerCmd { get; set; }









        public async void FinishGame(object parameter)
        {
            var result = (ValueTuple<int, int, string, DateTime, DateTime, int, PlayerInformation[]>)parameter;

            Misses = result.Item1;
            Moves = result.Item2;
            TimerText = result.Item3;
            StartTime = result.Item4;
            EndTime = result.Item5;
            SelectedLevel = result.Item6;
            var players = result.Item7;

            if (_startview.IsMultiplayerSelected)
            {
                PlayerInformation winningPlayer = GetMultiplayerWinner(players);

                EndViewModel = MultiplayerEndViewModel(winningPlayer);
            }
            else
            {
                var player = await _db.GetOrCreatePlayerAsync(_startview.PlayerName);
                PlayerName = player.Nickname;
                PlayerID = player.Id;

                EndViewModel = await SingleplayerEndViewModel();
            }
            CurrentView = EndViewModel;
        }

        private async Task<EndViewModel> SingleplayerEndViewModel()
        {

            CurrentSessionId = await _db.GetNewSessionId(StartTime, EndTime);
            return new EndViewModel(Misses, Moves, false, TimerText, StartTime, EndTime,
                                         SaveScoreCmd, RestartCmd, HighScoreCmd, BackToStartCmd, audioService: _audio);
        }

        private EndViewModel MultiplayerEndViewModel(PlayerInformation winningPlayer)
        {
            // Spara inte score som multiplayer
            return new EndViewModel(Misses, Moves, true, TimerText, StartTime, EndTime,
                                        null, RestartCmd, null, BackToStartCmd, winningPlayer, audioService: _audio);
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

        public async void SaveScore(object parameter)
        {
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

            string format = @"mm\:ss";
            int timeAsInt = 0;
            if (TimeSpan.TryParseExact(TimerText, format, null, out var span))
            {
                timeAsInt = (int)span.TotalSeconds;
            }
            _db.SaveFullGameSession(CurrentSessionId, StartTime, EndTime, PlayerID, timeAsInt, Moves, Misses, SelectedLevel);
        }
        public async void OpenHighScores(object parameter)
        {
            List<SessionScores> level1Scores = await _db.GetHighScoreList(1);
            List<SessionScores> level2Scores = await _db.GetHighScoreList(2);
            List<SessionScores> level3Scores = await _db.GetHighScoreList(3);

            LatestView = CurrentView;
            var players = await _db.GetPlayersForHighScoreAsync();

            CurrentView = new HighScoreViewModel(ReturnCmd, players, level1Scores, level2Scores, level3Scores);

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