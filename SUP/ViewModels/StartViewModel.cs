using Npgsql;
using PropertyChanged;
using SUP.Commands;
using SUP.Models;
using SUP.Services;
using SUP.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Xml.Serialization;

namespace SUP.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class StartViewModel
    {
        // Commands
        public ICommand StartGameCmd { get; }
        public ICommand HighScoreCmd { get; }
        public ICommand RulesCmd { get; }
        public ICommand AddPlayerCmd { get; }
        public ICommand RemovePlayerCmd { get; }

        // Properties
        public bool IsLevelOneSelected { get; set; }
        public bool IsLevelTwoSelected { get; set; } = true;
        public bool IsLevelThreeSelected { get; set; }
        public List<string> PlayerList { get; set; } = [];
        private string playerName { get; set; }
        public string MultiPlayerNameMessage { get; set; }
        public string PlayerNameMessage { get; set; } // För binding i MainShellViewModel
        public bool IsSinglePlayerSelected { get; set; } = true;
        public bool IsMultiplayerSelected { get; set; }
        public string Greeting { get; set; }
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
        public string PlayerName
        {
            get { return playerName; }
            set
            {
                playerName = value;

                GreetingMessage();
            }
        }

        private void GreetingMessage()
        {
            if (string.IsNullOrEmpty(playerName) && PlayerList.Count == 0)
            {
                Greeting = "Spelarnamn:";
            }
            else if (IsMultiplayerSelected == true)
            {
                Greeting = "Hej " + string.Join(" & ", PlayerList) + ", klicka på spela när du är redo!";
            }
            else
            {
                Greeting = "Hej " + playerName + ", klicka på spela när du är redo!";
            }
        }

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

        //Variabler
        public MainShellViewModel MainShellVM = new();
        public int level;
        private readonly GameHubDbServices _db;

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

        // Sparar för att det ska fortsättas ?????
        //private Regex regex = new Regex(@"^[0-9A-Za-z.\s_-]+$"); //https://stackoverflow.com/questions/13353663/what-is-the-regular-expression-to-allow-uppercase-lowercase-alphabetical-charac
        //private readonly string regexString = $"Tillåtna specialtecken: 0-9 . _ -";
        //private int maxLenght = 20;

        public StartViewModel(ICommand startGameCmd, ICommand highScoreCmd, ICommand rulesCmd, GameHubDbServices db)

        {
            _db = db;
            HighScoreCmd = highScoreCmd;
            RulesCmd = rulesCmd;

            AddPlayerCmd = new RelayCommand(CheckPlayerName);

            RemovePlayerCmd = new RelayCommand(p =>
            {
                MessageBox.Show($"Nu är spelaren {PlayerName} borttagen");
                PlayerList.Remove(PlayerName);
                PlayerList = PlayerList.ToList();
            });

            StartGameCmd = new RelayCommand(p =>
            {
                if (string.IsNullOrWhiteSpace(PlayerName) && PlayerList.Count < 2)
                {
                    MessageBox.Show("Vänligen ange ditt spelarnamn.", "Spelarnamn");
                    return;
                }
                startGameCmd.Execute(p);
            });
            Greeting = "Spelarnamn:";
        }

        public async void CheckPlayerName(object parameter)
        {
            var multiplayerNameMessage = MainShellVM.ControlPlayerNameMessage(PlayerName);
            MultiPlayerNameMessage = multiplayerNameMessage;

            if (multiplayerNameMessage != null)
            {
                return;
            }
            else
            {
                if (PlayerList.Contains(PlayerName))
                {
                    MessageBox.Show("Det namnet är redan taget! Välj ett annat.");
                    return;
                }

                if (PlayerList.Count < 2)
                {
                    PlayerList.Add(PlayerName);
                    PlayerName = "";
                    PlayerList = PlayerList.ToList();
                }
                else
                {
                    MessageBox.Show("Nu är ni redan två spelare!!");
                }
            }
        }
    }   
}