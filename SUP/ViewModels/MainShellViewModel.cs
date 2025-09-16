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
        EndViewModel EndViewModel { get; set; }
        private StartViewModel _startview;
        


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

        public void PassScoreToEndView(object parameter)
        {

        }
        public void FinishGame(object parameter)
        {
            var result = (ValueTuple<int, int, string>)parameter;
            int misses = result.Item1;
            int moves = result.Item2;
            string timer = result.Item3;
            EndViewModel = new EndViewModel(misses, moves, timer, SaveScoreCmd, RestartCmd, HighScoreCmd);
            CurrentView = EndViewModel;

        }

        public void RestartGame(object parameter)
        {
            CurrentView = new MemoryBoardViewModel(FinishGameCmd, RestartCmd, _startview.Level  );
        }
        public void SaveScore(object parameter)
        {
            CurrentView = new SaveScoreViewModel();
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
