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
    // Commands
    public ICommand SaveScoreCmd { get; }
    public ICommand RestartCmd { get; }
    public ICommand HighScoreCmd { get; }
    public ICommand BackToStartCmd { get; }

    // Properties
    public bool IsMultiplayer { get; set; }
    public string EndViewMessage { get; set; }
    public BitmapImage SelectedImage { get; set; }
    public bool PlayConfetti { get; set; }
    public Result CurrentResult { get; set; }

    // Variabler
    public GameTimer _timer = new GameTimer();
    public string endViewMessage;
    private readonly IAudioService _audio;

    public EndViewModel(Result currentResult, bool isMultiplayer,
                        ICommand saveScoreCmd, ICommand restartCmd, ICommand highScoreCmd, ICommand backToStartCmd,
                        PlayerInformation winningPlayer = null, IAudioService audioService = null) //?????
    {
        _audio = audioService;
        PlayConfetti = true;
        CurrentResult = currentResult;
        IsMultiplayer = isMultiplayer;
        SaveScoreCmd = saveScoreCmd;
        RestartCmd = restartCmd;
        HighScoreCmd = highScoreCmd;
        BackToStartCmd = backToStartCmd;
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
            EndViewMessage = $"Du hittade alla kort med {CurrentResult.Misses} missar på {CurrentResult.Guesses} drag!\nDet tog {CurrentResult.TimerText}.\n\nBra jobbat!";
        }
    }
    public async Task ConfettiTimer()
    {
        await Task.Delay(500);
        SelectedImage = new BitmapImage(new Uri("/SUP;component/Assets/Gifs/congratulations-7600.gif", UriKind.Relative));

        // https://github.com/XamlAnimatedGif/WpfAnimatedGif/blob/master/WpfAnimatedGif.Demo/MainWindow.xaml.cs
        // https://stackoverflow.com/questions/210922/how-do-i-get-an-animated-gif-to-work-in-wpf
    }
}