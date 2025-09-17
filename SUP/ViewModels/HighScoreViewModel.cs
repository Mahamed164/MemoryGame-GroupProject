using SUP.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SUP.ViewModels
{
    public class HighScoreViewModel
    {
        public ICommand ReturnCmd { get; }
        public ObservableCollection<Player> HighScores { get; set; }

        public HighScoreViewModel(ICommand returnCmd, List<Player> players)
        {
            ReturnCmd = returnCmd;
            HighScores = new ObservableCollection<Player>(players);
        }
    }
}