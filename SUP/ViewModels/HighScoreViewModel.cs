using PropertyChanged;
using SUP.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace SUP.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class HighScoreViewModel
    {
        public ICommand ReturnCmd { get; }
       // public ObservableCollection<Player> HighScores { get; set; }
        public ObservableCollection<SessionScores> HighScoresLevel1 { get; set; }
        public ObservableCollection<SessionScores> HighScoresLevel2 { get; set; }
        public ObservableCollection<SessionScores> HighScoresLevel3 { get; set; }
        public ObservableCollection<SessionScores> CurrentHighScoreList { get; set; }
        public ObservableCollection<ObservableCollection<SessionScores>> AllHighScores { get; set; }
        public ObservableCollection<int> Levels { get; set; } = new ObservableCollection<int>();
        public int SelectedLevel { get; set; }
        public HighScoreViewModel(ICommand returnCmd, List<Player> players, List<SessionScores> level1Scores, List<SessionScores> level2Scores, List<SessionScores> level3Scores)
        {
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
            ListForCBLevels();

        }

        private void ListForCBLevels()
        {
            for (int i = 0; i < 3; i++)
            {
                Levels.Add(i+1);
            }
            SelectedLevel = 1;

        }

        
    }
}