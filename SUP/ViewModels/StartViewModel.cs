using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
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
        public List <string> PlayerList { get; set; } = [];
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
        public bool IsSinglePlayerSelected { get; set; } = true;
        public bool IsMultiplayerSelected { get; set; }
      

        public List<string> GetPlayerList()
        {
            if (IsSinglePlayerSelected == true)
            {
                List<string> playerName = new List<string>();
                playerName.Add(PlayerName);
                return playerName;
                
            }
            if (IsMultiplayerSelected == true)
            {
                return PlayerList;
            }
            throw new Exception("nEJ");
        }
        public Visibility ShowList
        {
            get
            {
                if (IsMultiplayerSelected == true)
                {
                    return Visibility.Visible;
                }
                return Visibility.Hidden;
            }
        }
        public bool CanStart
        {
            get
            {
                if (IsMultiplayerSelected == true)
                {
                    if (PlayerList.Count >= 2)
                        return true;
                }
                else
                {
                    return true;
                }
                    return false;
            }
        }

        private string playerName;

        public string PlayerName
        {
            get { return playerName; }
            set
            {
                playerName = value;

                if (string.IsNullOrEmpty(playerName) && PlayerList.Count ==0)
                {
                    Greeting = "Spelarnamn:";
                }
                else if (IsMultiplayerSelected == true)
                {
                    Greeting = "Hej " +  string.Join(" & ", PlayerList) + ", klicka på spela när du är redo!";
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
        public ICommand AddPlayerCmd { get; }

        public StartViewModel(ICommand startGameCmd, ICommand highScoreCmd )
        {
            HighScoreCmd = highScoreCmd;
            AddPlayerCmd = new RelayCommand(p =>
            {
                if (PlayerList.Count < 2)
                {
                    PlayerList.Add(PlayerName);
                    PlayerName = "";
                    PlayerList = PlayerList.ToList();
                }

                else { MessageBox.Show("Nu är ni redan två spelare!!"); }

            });

            StartGameCmd = new RelayCommand(p =>
            {
                
                    if (string.IsNullOrWhiteSpace(PlayerName) && PlayerList.Count <2)
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
