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
           

            _db = db;

            _startview = new StartViewModel(StartGameCmd, HighScoreCmd);

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

        public async void StartGame(object parameter)
        {
            int maxLenght = 20;
            var player = await _db.GetOrCreatePlayerAsync(_startview.PlayerName);
            PlayerName = player.Nickname;
            Regex regex = new Regex(@"^[0-9A-Za-z.\s_-]+$"); //https://stackoverflow.com/questions/13353663/what-is-the-regular-expression-to-allow-uppercase-lowercase-alphabetical-charac
            //MatchCollection matches = regex.Matches(PlayerName);
            string regexString = $"Tillåtna specialtecken: 0-9 . _ -";
            //&& PlayerName.Contains(regex.ToString())


            //string meddelanden som ska kopplas bindings till i startview

            string nameToLong = "För långt namn, max antal karaktärer är 20";
            string nameSpace = "Inte tillåtet med mellanslag innan eller efter namn";
            string nameRegex = $"Ej tillånta tecken. \r\n{regexString}";
            string nameSpaceRegex = $"Inte tillåtet med mellanslag innan eller efter namn \n" +
                $"Tillåtna specialtecken: 0-9 . _ -";



            if (PlayerName.Length > maxLenght )
            {
                MessageBox.Show(nameToLong);
            }

            else if (PlayerName.StartsWith(" ") || PlayerName.EndsWith(" ") && !regex.IsMatch(PlayerName))
            {
                MessageBox.Show(nameSpaceRegex);
            }

            else if(PlayerName.StartsWith(" ") || PlayerName.EndsWith(" "))
            {
                MessageBox.Show(nameSpace);
            }
           

            else if (PlayerName.StartsWith(" ") && PlayerName.EndsWith(" "))
            {
                MessageBox.Show(nameSpace);
            }

            else if (!regex.IsMatch(PlayerName))
            {
                MessageBox.Show(nameRegex);
            }

            



            //källa else if https://www.w3schools.com/cs/cs_conditions_elseif.php 
            //med bara if statements blev else alltid utfört om inte varenda if statement stämde
            //https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/statements/selection-statements 
            //källa REGEX: https://forum.uipath.com/t/if-string-contains-space-in-the-end-or-in-the-beginning/390782/5 

            else
            {
                CurrentView = new BoardViewModel(FinishGameCmd, RestartCmd, _startview.Level, _startview.GetPlayerList(), BackToStartCmd, _audio);

            }

            /*^ : start of string
            [ : beginning of character group
            a-z : any lowercase letter
            A-Z : any uppercase letter
            0-9 : any digit
            _ : underscore
            ] : end of character group
            * : zero or more of the given characters
            $ : end of string

            
            \w is equivalent to [A-Za-z0-9_]
            Using the + quantifier you'll match one or more characters. 
            If you want to accept an empty string too, use * instead.
            https://stackoverflow.com/questions/336210/regular-expression-for-alphanumeric-and-underscores 
             */

        }

        private bool HasSpecialCharacters(string playerName)
        {
            return playerName.Any(ch => char.IsLetterOrDigit(ch));
        }

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

                EndViewModel = SingleplayerEndViewModel();
            }
            CurrentView = EndViewModel;
        }

        private EndViewModel SingleplayerEndViewModel()
        {
            return new EndViewModel(Misses, Moves, false, TimerText, StartTime, EndTime,
                                         SaveScoreCmd, RestartCmd, HighScoreCmd, BackToStartCmd);
        }

        private EndViewModel MultiplayerEndViewModel(PlayerInformation winningPlayer)
        {
            // Spara inte score som multiplayer
            return new EndViewModel(Misses, Moves, true, TimerText, StartTime, EndTime,
                                        null, RestartCmd, null, BackToStartCmd, winningPlayer);
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
            _db.SaveFullGameSession(StartTime, EndTime, PlayerID, timeAsInt, Moves, Misses, SelectedLevel);
        }
        public async void OpenHighScores(object parameter)
        {
            List<SessionScores> level1Scores = await _db.GetHighScoreList(1);
            List<SessionScores> level2Scores = await _db.GetHighScoreList(2);
            List<SessionScores> level3Scores = await _db.GetHighScoreList(3);

            LatestView = CurrentView;
            var players = await _db.GetPlayersForHighScoreAsync();

            CurrentView = new HighScoreViewModel(new RelayCommand(p =>
            {
                CurrentView = LatestView;
            }), players, level1Scores, level2Scores, level3Scores);

        }
        public void BackToStart(object parameter)
        {
            _startview.PlayerName = "";
            _startview.PlayerList.Clear();
            _startview.Greeting = "Spelarnamn:";

            CurrentView = _startview;
        }
    }
}