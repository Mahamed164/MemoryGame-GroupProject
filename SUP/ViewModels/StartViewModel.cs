using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using PropertyChanged;
using SUP.Commands;

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
                    Greeting = "Spelarnamn:";
                }
                else
                {
                    Greeting = "Hej " + playerName + ", klicka på spela när du är redo!";
                }
            }
        }
        public string Greeting { get; set; }
        public ICommand StartGameCmd { get; }

        public StartViewModel(ICommand startGameCmd)
        {
            StartGameCmd = new RelayCommand(p =>
            {
                if (string.IsNullOrWhiteSpace(PlayerName))
                {
                    MessageBox.Show("Vänligen ange ditt spelarnamn.", "Spelarnamn");
                    return;
                }
                startGameCmd.Execute(p);
            });
            Greeting = "Spelarnamn:";
        }
    }
}
