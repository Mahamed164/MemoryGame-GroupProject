using PropertyChanged;
using SUP.Commands;
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
        public ICommand StartGameCmd { get; }
        public ICommand FinishGameCmd { get; }
        public ICommand RestartCmd { get; }
        public ICommand SaveScoreCmd { get; }
        public ICommand HighScoreCmd { get; }
        EndViewModel EndViewModel { get; set; }

        public MainShellViewModel()
        {
            StartGameCmd = new RelayCommand(StartGame);
            FinishGameCmd = new RelayCommand(FinishGame);
            RestartCmd = new RelayCommand(RestartGame);
            SaveScoreCmd = new RelayCommand(SaveScore);
            HighScoreCmd = new RelayCommand(OpenHighScores);

            CurrentView = new StartViewModel(StartGameCmd);
        }
        public void StartGame(object parameter)
        {
            CurrentView = new MemoryBoardViewModel(FinishGameCmd);
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
            CurrentView = new MemoryBoardViewModel(FinishGameCmd);
        }
        public void SaveScore(object parameter)
        {

        }

        public void OpenHighScores(object parameter)
        {

        }

    }
}
