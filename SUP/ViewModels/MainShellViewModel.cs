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
        public MainShellViewModel()
        {
            StartGameCmd = new RelayCommand(StartGame);
            CurrentView = new StartViewModel(StartGameCmd);

        }
        public void StartGame(object parameter)
        {
            CurrentView = new GameViewModel();

        }

    }
}   
