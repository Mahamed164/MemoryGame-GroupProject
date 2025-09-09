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
    public int Missed { get; set; }
    public int Moves { get; set; }

    public ICommand SaveScoreCmd { get; }
    public ICommand RestartCmd { get; }

    public EndViewModel(ICommand saveScoreCmd, ICommand restartCmd)
    {
        SaveScoreCmd = saveScoreCmd;
        RestartCmd = restartCmd;
    }
}
