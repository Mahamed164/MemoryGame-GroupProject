using Microsoft.VisualBasic.Devices;
using PropertyChanged;
using SUP.Commands;
using SUP.Models;
using SUP.Views;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Input;

namespace SUP.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class HighScoreViewModel
    {
        public ICommand ReturnCmd { get; }
        public ICommand ChangeHighScoreListCmd { get; set; }
        public ObservableCollection<SessionScores> CurrentHighScoreList { get; set; }
        public ObservableCollection<ObservableCollection<SessionScores>> AllHighScores { get; set; }
        public int SelectedLevel { get; set; }

        public HighScoreViewModel(ICommand returnCmd, ObservableCollection<ObservableCollection<SessionScores>> allHighScores)
        {
            ChangeHighScoreListCmd = new RelayCommand(ChangeHighScoreList);
            ReturnCmd = returnCmd;
            AllHighScores = allHighScores;
            CurrentHighScoreList = AllHighScores.First();

            foreach (var highscoreList in AllHighScores)
            {
                SetHighScoreNumbers(highscoreList);
            }
        }

        public void ChangeHighScoreList(object parameter)
        {
            CurrentHighScoreList.Clear();
        }

        public void SetHighScoreNumbers(ObservableCollection<SessionScores> scores)
        {
            for (int i = 0; i < scores.Count; i++) 
            {
                scores[i].Rankings = i + 1;
            }
        }
    }
}