using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using PropertyChanged;

namespace SUP.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class StartViewModel
    {
        public string PlayerName { get; set; }
        public ICommand StartGameCmd { get;}

        public StartViewModel(ICommand startGameCmd) 
        {
            StartGameCmd = startGameCmd;        
        }

        

    }
}
