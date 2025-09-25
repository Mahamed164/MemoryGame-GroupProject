using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SUP.ViewModels
{
    public class RulesViewModel
    {
        public ICommand BackToStartCmd { get; }
        public RulesViewModel (ICommand backToStartCmd)
        {
            BackToStartCmd = backToStartCmd;
        }
    }
}
