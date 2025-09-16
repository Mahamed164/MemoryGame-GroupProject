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
        public bool IsLevelOneSelected { get; set; }
        public bool IsLevelTwoSelected { get; set; } = true;
        public bool IsLevelThreeSelected { get; set; }
        public int level;
        public int Level
        {
            get
            {
                SetLevel();// https://softwareengineering.stackexchange.com/questions/225354/logic-inside-class-properties-setters-getters
                return level;
            }
            set
            {
                level = value;
            }
        }

        private void SetLevel()
        {
            if (IsLevelOneSelected == true)
            {
                level = 1;
            }
            else if (IsLevelTwoSelected == true)
            {
                level = 2;
            }
            else if (IsLevelThreeSelected == true)
            {
                level = 3;
            }
        }
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
        public ICommand HighScoreCmd { get; }

        public StartViewModel(ICommand startGameCmd, ICommand highScoreCmd)
        {
            HighScoreCmd = highScoreCmd;

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
