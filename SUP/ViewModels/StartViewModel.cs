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
        private string playerName;

        

        public string PlayerName
        {
            get { return playerName; }
            set
            {
                playerName = value;

                if (string.IsNullOrEmpty(playerName))
                {
                    Greeting = "Player Name";
                }
                else
                {
                    Greeting = "Hej " + playerName + ", klicka på Start Game när du är redo!";
                }
            }
        }
        public string Greeting { get; set; }
        public ICommand StartGameCmd { get;}
        
        public StartViewModel(ICommand startGameCmd) 
        {
            StartGameCmd = startGameCmd;
            Greeting = "Player Name";
        }

        

    }
}
