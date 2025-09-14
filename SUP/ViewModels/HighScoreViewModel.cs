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
    
    public HighScoreViewModel(ICommand returnCmd)
        {
            ReturnCmd = returnCmd;
        }
    }
}