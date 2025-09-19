using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using PropertyChanged;

namespace SUP.ViewModels;
[AddINotifyPropertyChangedInterface]

public class EndViewModel
{
    //MemoryBoardViewModel mBVM = new MemoryBoardViewModel();
    public int Missed { get; set; }
    public int Moves { get; set; }

    public string TimerText { get; set; }
    public string TimeAsText { get; set; } 
    public string TotalTimeInSeconds { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }

    public ICommand SaveScoreCmd { get; }
    public ICommand RestartCmd { get; }
    public ICommand HighScoreCmd { get; }


    public EndViewModel()
    {

    }
    public EndViewModel(int misses, int moves, string timer, DateTime startTime, DateTime endTime, ICommand saveScoreCmd, ICommand restartCmd, ICommand highScoreCmd)
    {
        SaveScoreCmd = saveScoreCmd;
        RestartCmd = restartCmd;
        HighScoreCmd = highScoreCmd;
        Missed = misses;
        Moves = moves;
        TimerText = timer;
        StartTime = startTime;
        EndTime = endTime;

        TimeAsText = SetTimerText();
        
    }

    public void SetScore()
    {
    }


    public string SetTimerText()
    {
        string format = @"mm\:ss";

        if(TimeSpan.TryParseExact(TimerText, format, null, out var span))
        {
            TotalTimeInSeconds = span.TotalSeconds.ToString();

            if(span.TotalSeconds < 60) //Hämtar värdet av TimeSpan och retunerar totala sekunderna 
            {
                return (int)span.TotalSeconds + " sekunder"; //ifall det är sekunder skriv tiden +  sekunder
            }
            else
            {
                int mins = (int)span.TotalMinutes;
                int secs = (int)span.Seconds; //inte total för då stod det 62 istället för 2 
                string resultMins = "";
                string resultSecs = "";
                if (mins == 1)
                {
                    resultMins = " minut och "; /*+ secs + " sekunder";*/ //singular ifall det bara är en minut 
                    resultSecs = " sekunder";

                }else
                {
                    resultMins = " minuter och "; /*+ secs + " sekunder";*/ //plural ifall det är flera minuter 
                    resultSecs = " sekunder";
                }

                return mins + resultMins + secs + resultSecs;
            }

        }

        return TimerText;

        //Källa: https://learn.microsoft.com/en-us/dotnet/api/system.timespan.tryparse?view=net-9.0 - denna började jag med men insåg att det var hh:mm inte mm:ss som behvös i vårt spel
        //Converts the specified string representation of a time interval to its TimeSpan equivalent and returns a value that indicates whether the conversion succeeded.

        /*
         * Källa för tryparseexact: https://learn.microsoft.com/en-us/dotnet/api/system.timespan.tryparseexact?view=net-9.0 
         * Converts the string representation of a time interval to its TimeSpan equivalent, and returns a value that indicates whether the conversion succeeded. 
         * The format of the string representation must match a specified format exactly. --> måste göra om det till mm:ss (minuter:sekunder) format
         */

        //Allmän källa om TimeSpan: https://learn.microsoft.com/en-us/dotnet/api/system.timespan?view=net-9.0

    }


}

