using PropertyChanged;
using SUP.Commands;
using SUP.ViewModels.Scores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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





        public MainShellViewModel()
        {
            StartGameCmd = new RelayCommand(StartGame);
            FinishGameCmd = new RelayCommand(FinishGame);
            RestartCmd = new RelayCommand(RestartGame);
            SaveScoreCmd = new RelayCommand(SaveScore);
            HighScoreCmd = new RelayCommand(OpenHighScores);
   


            _startview = new StartViewModel(StartGameCmd, HighScoreCmd);
            CurrentView = _startview;


        }
        public void StartGame(object parameter)
        {
            CurrentView = new MemoryBoardViewModel(FinishGameCmd, RestartCmd, _startview.Level);
        }


        public void FinishGame(object parameter)
        {
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
        public void SaveScore(object parameter)
        {
            CurrentView = new SaveScoreViewModel(Moves, Misses, TimerText);
        }

        public void OpenHighScores(object parameter)
        {
            LatestView = CurrentView;
            CurrentView = new HighScoreViewModel(new RelayCommand(p =>
            {
                CurrentView = LatestView;
            }));
        }



    }
}
