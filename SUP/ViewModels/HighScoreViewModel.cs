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
        // public ObservableCollection<Player> HighScores { get; set; }
        public ObservableCollection<SessionScores> HighScoresLevel1 { get; set; }
        public ObservableCollection<SessionScores> HighScoresLevel2 { get; set; }
        public ObservableCollection<SessionScores> HighScoresLevel3 { get; set; }
        public ObservableCollection<SessionScores> CurrentHighScoreList { get; set; }
        public ObservableCollection<ObservableCollection<SessionScores>> AllHighScores { get; set; }
        public int SelectedLevel { get; set; }

        public HighScoreViewModel(ICommand returnCmd, List<Player> players, List<SessionScores> level1Scores, List<SessionScores> level2Scores, List<SessionScores> level3Scores)
        {
            ChangeHighScoreListCmd = new RelayCommand(ChangeHighScoreList);
            ReturnCmd = returnCmd;
            //HighScores = new ObservableCollection<Player>(players);


            HighScoresLevel1 = new ObservableCollection<SessionScores>(level1Scores);
            HighScoresLevel2 = new ObservableCollection<SessionScores>(level2Scores);
            HighScoresLevel3 = new ObservableCollection<SessionScores>(level3Scores);

            AllHighScores = new ObservableCollection<ObservableCollection<SessionScores>>()
            {
                HighScoresLevel1,
                HighScoresLevel2,
                HighScoresLevel3
            };
            CurrentHighScoreList = HighScoresLevel1;
            
        }

        
        public void ChangeHighScoreList(object parameter)
        {
            CurrentHighScoreList.Clear();
            
        }
    }
}