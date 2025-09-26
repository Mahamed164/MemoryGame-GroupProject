using Microsoft.VisualBasic.Devices;
using NAudio.Dsp;
using Npgsql;
using PropertyChanged;
using SUP.Commands;
using SUP.Common;
using SUP.Models;
using SUP.Services;
using SUP.Views.Converters;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing.Imaging.Effects;
using System.Globalization;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;


namespace SUP.ViewModels;
[AddINotifyPropertyChangedInterface]
public class BoardViewModel : ISupportsCardInput
{
    public ICommand PressCardIndexCommand { get; }
    public ICommand FinishGameCommand { get; }
    public ObservableCollection<CardViewModel> Cards { get; } = new();

    // lista för att hålla koll på vilka kort som är vända
    private List<CardViewModel> turnedCards = new();

    List<Cards> _cards = new List<Cards>();

    EndViewModel endViewModel = new EndViewModel();

    public MainShellViewModel MainShellVM { get; set; } = new();

    int completedPairs;
    int numOffGuesses;

    public int Level { get; set; } = 2; //den börjar på 2
    public PlayerInformation[] Players { get; set; }
    public int CurrentPlayer;
    public string PlayerOneLabel { get; set; }
    public string PlayerTwoLabel { get; set; } = string.Empty;
    public string PlayerOneHighlighted { get; set; } = "normal";
    public string PlayerTwoHighlighted { get; set; } = "normal";
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }

    public GameTimer timer = new GameTimer();
    public string TimerText { get; set; } = "00:00";
    public bool hasStarted = false;

    public ICommand RestartCmd { get; }
    public ICommand BackToStartCmd { get; }


    private readonly IAudioService _audio;

    public event Action<string>? RequestStepSound;

    public BoardViewModel(IAudioService _audioService)
    {

    }

    public BoardViewModel(ICommand finishGameCommand, ICommand restartCmd, int level,
                            List<string> playerList, ICommand backToStartCmd, IAudioService _audioService)
    {
        Level = level;
        Players = new PlayerInformation[playerList.Count];

        for (int i = 0; i < playerList.Count; i++)
        {
            Players[i] = new() { Name = playerList[i] };
        }
        UpdatePlayerLabel();

        FinishGameCommand = finishGameCommand;
        RestartCmd = restartCmd;
        BackToStartCmd = backToStartCmd;

        ConfigureCards();
        
        timer.Reset();
        UpdateTimer();

        _audio = _audioService;

        _audio.LoadSfx(new Dictionary<string, string>()
        {
            { "flipCard", "Assets/Sounds/Sfx/flipCard_edited.mp3" }
        });

        _audio.SetSfxVolume(1);
        _audio.SetSfxMuted(false);
    }
    private async void UpdateTimer()
    {
        while (true)
        {
            await Task.Delay(1000);
            TimerText = timer.GetTime();
        }
    }
    public async void OnButtonClicked(CardViewModel card)
    {

        if (!hasStarted)
        {
            StartTime = DateTime.Now;
            timer.Start();
            hasStarted = true;
        }
        await TurnCardsAsync(card);



        CheckForCompletion();

    }

    private async void CheckForCompletion()
    {
        if (completedPairs == (Cards.Count / 2))
        {
            timer.Stop();
            EndTime = DateTime.Now;
            int mistakes = numOffGuesses - (Cards.Count / 2);
           await PlayConfettiGif();
            FinishGameCommand?.Execute((mistakes, numOffGuesses, TimerText, StartTime, EndTime, Level, Players));
        }
    }
    private async Task TurnCardsAsync(CardViewModel card) // Kan man lägga multiplayer här?
    {
        
        if (!card.FaceUp) 
        {
            _audio.PlaySfx("flipCard");
        }

        if (card.FaceUp)
        {

            return;
        }
        if (turnedCards.Count >= 2)
        {
            return;
        }

        card.FaceUp = true;
        turnedCards.Add(card);

        if (turnedCards.Count == 2)
        {
            await CheckForMatchingCardsAsync();
        }
        UpdatePlayerLabel();

        // You use the switch expression to evaluate a single expression from a list of candidate expressions based on a pattern match with an input expression.
        // https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/switch-expression 
        // https://www.w3schools.com/cs/cs_switch.php
    }

    private async Task CheckForMatchingCardsAsync()
    {
        numOffGuesses++;
        Players[CurrentPlayer].Guesses++;

        // om korten inte matchar vänd tillbaka
        if (turnedCards[0].Id != turnedCards[1].Id)
        {
            await Task.Delay(800);
            turnedCards[0].FaceUp = false;
            turnedCards[1].FaceUp = false;
        }
        else
        {
            completedPairs++;
            Players[CurrentPlayer].CorrectGuesses++;
        }

        CurrentPlayer = (CurrentPlayer + 1) % Players.Length;
        turnedCards.Clear();
    }

    private void UpdatePlayerLabel()
    {
        StringBuilder stringBuilder = new StringBuilder();
        if (Players.Length == 1)
        {
            foreach (var player in Players)
            {
                stringBuilder.AppendLine(player.Name);
                stringBuilder.Append("Gissningar: ");
                stringBuilder.AppendLine(player.Guesses.ToString());
                stringBuilder.Append("Rätt gissningar: ");
                stringBuilder.AppendLine(player.CorrectGuesses.ToString());
                stringBuilder.Append("Precision: ");
                stringBuilder.Append(player.Accuracy.ToString());
                stringBuilder.AppendLine("%");
                stringBuilder.AppendLine();
            }
            PlayerOneLabel = stringBuilder.ToString();
        }
        else
        {
            if (0 == (CurrentPlayer + 1) % Players.Length)
            {
                PlayerOneHighlighted = "Normal";
                PlayerTwoHighlighted = "Bold";

                foreach (var player in Players)
                {
                    stringBuilder.AppendLine(player.Name);
                    stringBuilder.Append("Gissningar: ");
                    stringBuilder.AppendLine(player.Guesses.ToString());
                    stringBuilder.Append("Rätt gissningar: ");
                    stringBuilder.AppendLine(player.CorrectGuesses.ToString());
                    stringBuilder.Append("Precision: ");
                    stringBuilder.Append(player.Accuracy.ToString());
                    stringBuilder.AppendLine("%");
                    stringBuilder.AppendLine();
                    if (player == Players[0])
                    {
                        PlayerOneLabel = stringBuilder.ToString();
                        stringBuilder.Clear();
                    }
                    else if (player == Players[1])
                    {
                        PlayerTwoLabel = stringBuilder.ToString();
                        stringBuilder.Clear();
                    }
                }
            }
            else if (1 == (CurrentPlayer + 1) % Players.Length)
            {
                PlayerTwoHighlighted = "Normal";
                PlayerOneHighlighted = "Bold";

                foreach (var player in Players)
                {
                    stringBuilder.AppendLine(player.Name);
                    stringBuilder.Append("Gissningar: ");
                    stringBuilder.AppendLine(player.Guesses.ToString());
                    stringBuilder.Append("Rätt gissningar: ");
                    stringBuilder.AppendLine(player.CorrectGuesses.ToString());
                    stringBuilder.Append("Precision: ");
                    stringBuilder.Append(player.Accuracy.ToString());
                    stringBuilder.AppendLine("%");
                    stringBuilder.AppendLine();

                    if (player == Players[1])
                    {
                        PlayerTwoLabel = stringBuilder.ToString();
                        stringBuilder.Clear();
                    }
                    else if (player == Players[0])
                    {
                        PlayerOneLabel = stringBuilder.ToString();
                        stringBuilder.Clear();
                    }
                }
            }

        }
    
    }

    private void ConfigureCards()
    {
        completedPairs = 0;
        var shuffled = MakeCards(Level);

        Cards.Clear();

        foreach (var card in shuffled)
        {
            
            Cards.Add(new CardViewModel(card, OnButtonClicked));
        }
        //(var cardId in shuffled)
        //for (int i = 0; i < shuffled.Count; i++)
        //{
        //    string sfx = $"memory.cardAll{i}";
        //    var card = new Cards(i);
        //    Cards.Add(new CardViewModel(card, OnButtonClicked, sfx));
        //}
    }
 
    public List<Cards> MakeCards(int level)
    {
        Level = level;
        Random random = new Random();
        _cards.Clear();
        int numberOfCardPairs;

        switch (Level)
        {
            default:
            case 1:
                numberOfCardPairs = 6;
                break;
            case 2:
                numberOfCardPairs = 10;
                break;
            case 3:
                numberOfCardPairs = 15;
                break;
        }
        for (int i = 0; i < numberOfCardPairs; i++)
        {
            _cards.Add(new Cards(i));
            _cards.Add(new Cards(i));
        }
        return _cards = _cards.OrderBy(x => random.Next()).ToList();
    }
    public async Task PlayConfettiGif()
    {
     
    }

    //public async Task<Cards> MakeCardSpeedAsync(int level)
    //{//tänker att level 1 inte behöver vändas tillbaka av sig självt iom att det ska vara så lätt som möjligt
    //    Level = level;
    //    int delayLevel2 = 500;
    //    int delayLevel3 = 250;
    //    if(level == 2)
    //    {
            
    //    }

    //    //om ett kort är klickat, if level = 1 hoppa över, if level = 2 ska den vändas tillbaka efter delay 500 (t.ex.)
    //    //if level = 3 ska den vändas tillbaka efter delay 250 (t.ex) 
    //}
}