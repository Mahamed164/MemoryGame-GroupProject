using PropertyChanged;
using SUP.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace SUP.ViewModels.Scores;
[AddINotifyPropertyChangedInterface]

public class SaveScoreViewModel
{
    public int Moves { get; set; }
    public int Misses { get; set; }
    public string TimerText { get; set; }
    public string PlayerName { get; set; }
    public int PlayerID { get; set; }
    public ICommand SaveCurrentScoreCmd { get; }
    public ICommand HighScoreCmd { get; }
    public string NumOfMoves { get; set; }
    public string NumOfMisses { get; set; }
    public string FullTimerText { get; set; }
    public SaveScoreViewModel(int moves, int misses, string timerText, string playerName, int playerID, ICommand highScoreCmd)
    {
        Moves = moves;
        Misses = misses;
        TimerText = timerText;
        PlayerName = playerName;
        PlayerID = playerID;
        SetScoreTexts();
        SaveCurrentScoreCmd = new RelayCommand(p => SaveCurrentScore());
        HighScoreCmd = highScoreCmd;
    }

    public void SetScoreTexts()
    {
        NumOfMoves = $"Antal drag: {Moves}";
        NumOfMisses = $"Antal missar: {Misses}";
        FullTimerText = $"Total tid: {TimerText}";
    }

    public void SaveCurrentScore()
    {
        MessageBox.Show("test");
    }
}

