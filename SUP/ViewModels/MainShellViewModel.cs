using PropertyChanged;
using SUP.Commands;
using SUP.Models;
using SUP.Services;
using SUP.ViewModels.Scores;
using System;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
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
        //från human benchmark video 8
        private void OnMusicVolumeChanged() { _audio.SetMusicVolume((float)MusicVolume); }
        private void OnSfxVolumeChanged() { _audio.SetSfxVolume((float)SfxVolume); }
        private void OnMusicMutedChanged() { _audio.SetMusicMuted(MusicMuted); }
        private void OnSfxMutedChanged() { _audio.SetSfxMuted(SfxMuted); }

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
        

        private readonly GameHubDbServices _db;
        private readonly IAudioService _audio;

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
            ApplyAudioSettings();
            _audio.SetMusicLoop("Assets/Sounds/game-mode-on-356552.mp3", autoPlay: true);
        }

        //från humanbenchmark video 8

        private void ApplyAudioSettings()
        {
            _audio.SetMusicVolume((float)MusicVolume);
            _audio.SetSfxVolume((float)SfxVolume);
            _audio.SetMusicMuted(MusicMuted);
            _audio.SetSfxMuted(SfxMuted);

        }


        public void StartGame(object parameter)
        {
            CurrentView = new MemoryBoardViewModel(FinishGameCmd, RestartCmd, _startview.Level, _startview.GetPlayerList(), BackToStartCmd);
        }

        public async void FinishGame(object parameter)

        {
            //var player = await _db.GetOrCreatePlayerAsync(_startview.PlayerName);
            //PlayerName = player.Nickname;
            //PlayerID = player.Id;

            var result = (ValueTuple<int, int, string, DateTime, DateTime>)parameter;
            Misses = result.Item1;
            Moves = result.Item2;
            TimerText = result.Item3;
            StartTime = result.Item4;
            EndTime = result.Item5;

            if (_startview.IsMultiplayerSelected)
            {
                // Spara inte score som multiplayer
                EndViewModel = new EndViewModel(Misses, Moves, TimerText, StartTime, EndTime, RestartCmd, BackToStartCmd, null, null); 
            }
            else
            {
                var player = await _db.GetOrCreatePlayerAsync(_startview.PlayerName);
                PlayerName = player.Nickname;
                PlayerID = player.Id;

                EndViewModel = new EndViewModel(Misses, Moves, TimerText, StartTime, EndTime, SaveScoreCmd, RestartCmd, HighScoreCmd, BackToStartCmd);
            }
            CurrentView = EndViewModel;

            //EndViewModel = new EndViewModel(Misses, Moves, TimerText, StartTime, EndTime, SaveScoreCmd, RestartCmd, HighScoreCmd, BackToStartCmd);
            //CurrentView = EndViewModel;
        }
        public void RestartGame(object parameter)
        {
            CurrentView = new MemoryBoardViewModel(FinishGameCmd, RestartCmd, _startview.Level, _startview.GetPlayerList(), BackToStartCmd);
        }
        public async void SaveScore(object parameter)
        {


            //CurrentView = new SaveScoreViewModel(Moves, Misses, TimerText, PlayerName, PlayerID, HighScoreCmd);

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

            string format = @"ss";
            int timeAsInt = 0;
            if (TimeSpan.TryParseExact(TimerText, format, null, out var span))
            {
                timeAsInt = (int)span.TotalSeconds;
            }
            _db.SaveFullGameSession(StartTime, EndTime, PlayerID, timeAsInt, Moves, Misses);
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
        public void BackToStart(object parameter)
        {
            _startview.PlayerName = "";
            _startview.PlayerList.Clear();
            _startview.Greeting = "Spelarnamn:";

            CurrentView = _startview;
        }
    }
}
