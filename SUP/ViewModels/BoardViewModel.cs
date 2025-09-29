using Microsoft.VisualBasic.Devices;
using NAudio.Dsp;
using Npgsql;
using PropertyChanged;
using SUP.Commands;
using SUP.Common;
using SUP.Enums;
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
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;


namespace SUP.ViewModels;
[AddINotifyPropertyChangedInterface]
public class BoardViewModel : ISupportsCardInput
{
    // Commands
    public ICommand PressCardIndexCommand { get; }
    public ICommand FinishGameCommand { get; }
    public ICommand RestartCmd { get; }
    public ICommand BackToStartCmd { get; }

    // Properties
    public MainShellViewModel MainShellVM { get; set; } = new();
    public LevelSelect ChosenLevel { get; set; } = LevelSelect.Medium; //den börjar på 2
    public int Level { get; set; }
    public PlayerInformation[] Players { get; set; }
    public string PlayerOneLabel { get; set; }
    public string PlayerTwoLabel { get; set; } = string.Empty;
    public string PlayerOneHighlighted { get; set; } = "normal";
    public string PlayerTwoHighlighted { get; set; } = "normal";
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string TimerText { get; set; } = "00:00";

    // Variabler
    public GameTimer timer = new GameTimer();
    public bool hasStarted = false;
    public int CurrentPlayer;
    int completedPairs;
    int numOffGuesses;

    private List<CardViewModel> turnedCards = new(); // Lista över alla vända kort
    List<Cards> _cards = new List<Cards>(); // Lista med kort som innehåller datan från Cards-Model
    public ObservableCollection<CardViewModel> Cards { get; } = new(); // De kort som visas på memorybrädet

    // Ljudrelaterat
    private readonly IAudioService _audio;
    public event Action<string>? RequestStepSound;

    public BoardViewModel(IAudioService _audioService)
    {

    }

    public BoardViewModel(ICommand finishGameCommand, ICommand restartCmd, LevelSelect level,
                            List<string> playerList, ICommand backToStartCmd, IAudioService _audioService)
    {
        CreateListOfPlayers(level, playerList);
        UpdatePlayerLabel();

        FinishGameCommand = finishGameCommand;
        RestartCmd = restartCmd;
        BackToStartCmd = backToStartCmd;

        ConfigureCards();

        timer.Reset();
        UpdateTimer();
        _audio = _audioService;
        PlayCardSound(_audioService);
    }

    private void PlayCardSound(IAudioService _audioService)
    {
        _audio.LoadSfx(new Dictionary<string, string>()
        {
            { "flipCard", "Assets/Sounds/Sfx/flipCard_edited.mp3" }
        });

        _audio.SetSfxVolume(1);
        _audio.SetSfxMuted(false);
    }

    private void CreateListOfPlayers(LevelSelect level, List<string> playerList)
    {
        ChosenLevel = level;
        Players = new PlayerInformation[playerList.Count];

        for (int i = 0; i < playerList.Count; i++)
        {
            Players[i] = new() { Name = playerList[i] };
        }
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
            Result currentResult = new Result(mistakes, numOffGuesses, TimerText, StartTime, EndTime, Level);
            FinishGameCommand?.Execute((currentResult, Players));
        }
    }
    private async Task TurnCardsAsync(CardViewModel card)
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
    }

    private async Task CheckForMatchingCardsAsync()
    {
        numOffGuesses++;
        Players[CurrentPlayer].Guesses++;

        // Om korten inte matchar vänd tillbaka
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
                CreateStringBuilderForPlayerLabel(stringBuilder, player);
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
                    CreateStringBuilderForPlayerLabel(stringBuilder, player);
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
                    CreateStringBuilderForPlayerLabel(stringBuilder, player);

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

    private static void CreateStringBuilderForPlayerLabel(StringBuilder stringBuilder, PlayerInformation player)
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

    private void ConfigureCards()
    {
        completedPairs = 0;
        var shuffled = CreatePairsFromLevel(ChosenLevel);

        Cards.Clear();

        foreach (var card in shuffled)
        {
            Cards.Add(new CardViewModel(card, OnButtonClicked));
        }
    }

    public List<Cards> CreatePairsFromLevel(LevelSelect level)
    {
        ChosenLevel = level;
        Random random = new Random();
        _cards.Clear();
        int numberOfCardPairs;

        switch (ChosenLevel)
        {
            default:
            case LevelSelect.Easy:
                numberOfCardPairs = 6;
                Level = 1;
                break;
            case LevelSelect.Medium:
                numberOfCardPairs = 10;
                Level = 2;
                break;
            case LevelSelect.Hard:
                numberOfCardPairs = 15;
                Level = 3;
                break;
                // You use the switch expression to evaluate a single expression from a list of candidate expressions based on a pattern match with an input expression.
                // https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/switch-expression 
                // https://www.w3schools.com/cs/cs_switch.php
        }
        return AddCards(random, numberOfCardPairs);
    }

    private List<Cards> AddCards(Random random, int numberOfCardPairs)
    {
        for (int i = 0; i < numberOfCardPairs; i++)
        {
            _cards.Add(new Cards(i));
            _cards.Add(new Cards(i));
        }
        return _cards = _cards.OrderBy(x => random.Next()).ToList();
    }
}