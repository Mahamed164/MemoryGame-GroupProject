using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using PropertyChanged;

namespace SUP.ViewModels;
[AddINotifyPropertyChangedInterface]

public class EndViewModel
{
    //MemoryBoardViewModel mBVM = new MemoryBoardViewModel();
    public int Missed { get; set; }
    public int Moves { get; set; }

    public string TimerText { get; set; }

    public ICommand SaveScoreCmd { get; }
    public ICommand RestartCmd { get; }
    public ICommand HighScoreCmd { get; }

    public EndViewModel()
    {

    }
    public EndViewModel(int misses, int moves, string timer, ICommand saveScoreCmd, ICommand restartCmd, ICommand highScoreCmd)
    {
        SaveScoreCmd = saveScoreCmd;
        RestartCmd = restartCmd;
        HighScoreCmd = highScoreCmd;
        Missed = misses;
        Moves = moves;
        TimerText = timer;
    }

    public void SetScore()
    {
    }
}

