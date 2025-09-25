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

        

        public bool IsLevelOneSelected { get; set; }
        public bool IsLevelTwoSelected { get; set; } = true;
        public bool IsLevelThreeSelected { get; set; }

        public List <string> PlayerList { get; set; } = [];

        public string PlayerNameMessage { get; set; }
        private string playerName { get; set; }


        public string MultiPlayerNameMessage { get; set; }

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
        public ICommand AddPlayerCmd { get; set; }


        public MainShellViewModel MainShellVM  = new();
        



        //sparar för att det ska fortsättas 

        //private Regex regex = new Regex(@"^[0-9A-Za-z.\s_-]+$"); //https://stackoverflow.com/questions/13353663/what-is-the-regular-expression-to-allow-uppercase-lowercase-alphabetical-charac
        //private readonly string regexString = $"Tillåtna specialtecken: 0-9 . _ -";
        //private int maxLenght = 20;

        private readonly GameHubDbServices _db;

        public StartViewModel(ICommand startGameCmd, ICommand highScoreCmd, GameHubDbServices db)
        {
            _db = db;
            HighScoreCmd = highScoreCmd;

            //AddPlayerCmd = new RelayCommand(AddPlayer);


            //knappen ska kontrollera texten
            //om bra --> lägg till
            // om dålig --> felmeddelande

            //problem:
            /*
             * 1. den hoppar över metoden AddPlayer!!
             */


            AddPlayerCmd = new RelayCommand(CheckPlayerName);

            //if(AddPlayer != null)
            //{
            //    if (PlayerList.Count < 2)
            //    {
            //        PlayerList.Add(PlayerName);
            //        PlayerName = "";
            //        PlayerList = PlayerList.ToList();
            //    }
            //    else
            //    {
            //        MessageBox.Show("Nu är ni redan två spelare!!");
            //    }
            //}
            //else
            //{
            //    MessageBox.Show("test");//AddPlayerCmd = new RelayCommand(MainShellVM.AddPlayer);
            //}



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

        public Player player { get; set; } = new();

        public async void CheckPlayerName(object parameter)
        {
            //lägg till knappen gör playername tom


            //var player = await _db.GetOrCreatePlayerAsync(PlayerName);
            


            var multiplayerNameMessage = MainShellVM.GetPlayerNameMessage(PlayerName);
            
            if (multiplayerNameMessage != null)
            {
                MultiPlayerNameMessage = multiplayerNameMessage;
                return;
            }
            else
            {
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


            //hur kan den åka in i Addplayer direkt?
            //slå ihop metoderna?

            //MainShellVM.AddPlayer();
            //if (MainShellVM.AddPlayer == null)
            //{
            //    if (PlayerList.Count < 2)
            //    {
            //        PlayerList.Add(PlayerName);
            //        PlayerName = "";
            //        PlayerList = PlayerList.ToList();
            //    }
            //    else
            //    {
            //        MessageBox.Show("Nu är ni redan två spelare!!");
            //    }
            //}
            //else
            //{
            //    //MultiPlayerNameMessage = playerNameMessage;

            //    MessageBox.Show("test");//AddPlayerCmd = new RelayCommand(MainShellVM.AddPlayer);
            //}
        }

        //public async void AddPlayer(object parameter)
        //{

        //    var player = await _db.GetOrCreatePlayerAsync(PlayerName);
        //    PlayerName = player.Nickname;

        //    var playerNameMessage = MainShellVM.GetPlayerNameMessage(PlayerName);
        //    if (playerNameMessage != null)
        //    {
                
        //        MultiPlayerNameMessage = playerNameMessage;
        //        return;
        //    }
        //    else
        //    {
        //        MessageBox.Show("addplayer test");
        //    }

        //}

    }
}