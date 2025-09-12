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
        public void FinishGame(object parameter)
        {
            CurrentView = new EndViewModel(SaveScoreCmd, RestartCmd, HighScoreCmd);
        }

        public void RestartGame(object parameter)
        {

        }
        public void SaveScore(object parameter)
        {

        }

        public void OpenHighScores(object parameter)
        {

        }

    }
}
