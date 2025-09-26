using PropertyChanged;
using SUP.Models;
using SUP.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;


namespace SUP.ViewModels;
[AddINotifyPropertyChangedInterface]

public class EndViewModel
{
    //ICOMMAND
    public ICommand SaveScoreCmd { get; }
    public ICommand RestartCmd { get; }
    public ICommand HighScoreCmd { get; }
    public ICommand BackToStartCmd { get; }

    //PROP
    public int Missed { get; set; }
    public int Moves { get; set; }
    public bool IsMultiplayer { get; set; }
    public string TimerText { get; set; }
    public string TimeAsText { get; set; }
    public string TotalTimeInSeconds { get; set; }
    public string EndViewMessage { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public BitmapImage SelectedImage { get; set; }
    public bool PlayConfetti { get; set; }
    
    //VARIABLER
    public GameTimer _timer = new GameTimer();
    public string endViewMessage;
    private readonly IAudioService _audio;


    public EndViewModel(int misses, int moves, bool isMultiplayer, string timer,
                        DateTime startTime, DateTime endTime,
                        ICommand saveScoreCmd, ICommand restartCmd, ICommand highScoreCmd, ICommand backToStartCmd,
                        PlayerInformation winningPlayer = null, IAudioService audioService = null) //?????
    {
        _audio = audioService;
        PlayConfetti = true;

        Missed = misses;
        Moves = moves;
        IsMultiplayer = isMultiplayer;
        TimerText = timer;
        StartTime = startTime;
        EndTime = endTime;
        SaveScoreCmd = saveScoreCmd;
        RestartCmd = restartCmd;
        HighScoreCmd = highScoreCmd;
        BackToStartCmd = backToStartCmd;
        TimeAsText = SetTimerText();
        CreateEndViewMessage(winningPlayer);
        ConfettiTimer();
        PlayVictorySound(_audio);

    }

    private void PlayVictorySound(IAudioService _audioService)
    {
        _audio.LoadSfx(new Dictionary<string, string>()
        {
            { "victory", "Assets/Sounds/Sfx/victory.mp3" }
        });

        _audio.SetSfxVolume(1);
        _audio.SetSfxMuted(false);
        _audio.PlaySfx("victory");
    }

    private void CreateEndViewMessage(PlayerInformation winningPlayer)
    {
        if (IsMultiplayer)
        {
            EndViewMessage = $"Grattis {winningPlayer.Name}!\nDu vann med {winningPlayer.Accuracy}% precision\n{winningPlayer.CorrectGuesses}/{winningPlayer.Guesses} rätt!";
        }
        else
        {
            EndViewMessage = $"Du hittade alla kort med {Missed} missar på {Moves} drag!\nDet tog {TimeAsText}.\n\nBra jobbat!";
        }
    }

    public string SetTimerText()
    {
        string format = @"mm\:ss";

        if (TimeSpan.TryParseExact(TimerText, format, null, out var span))
        {
            TotalTimeInSeconds = span.TotalSeconds.ToString();

            if (span.TotalSeconds < 60) //Hämtar värdet av TimeSpan och retunerar totala sekunderna 
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

                }
                else
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

    public async Task ConfettiTimer()
    {
        await Task.Delay(500);
        SelectedImage = new BitmapImage(new Uri("/SUP;component/Assets/Gifs/congratulations-7600.gif", UriKind.Relative));

        // https://github.com/XamlAnimatedGif/WpfAnimatedGif/blob/master/WpfAnimatedGif.Demo/MainWindow.xaml.cs
        // https://stackoverflow.com/questions/210922/how-do-i-get-an-animated-gif-to-work-in-wpf
    }

}