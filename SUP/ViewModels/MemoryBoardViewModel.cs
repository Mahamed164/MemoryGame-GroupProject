using Npgsql;
using PropertyChanged;
using SUP.Commands;
using SUP.Common;
using SUP.Views.Converters;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;


namespace SUP.ViewModels;
[AddINotifyPropertyChangedInterface]
public class MemoryBoardViewModel : ISupportsCardInput
{
    public ICommand PressCardIndexCommand { get; }
    public ICommand FinishGameCommand { get; }
    public ObservableCollection<CardViewModel> Cards { get; } = new();

    

    // lista för att hålla koll på vilka kort som är vända
    private List<CardViewModel> turnedCards = new();
    List<Cards> _cards = new List<Cards>();
    EndViewModel endViewModel = new EndViewModel();
    int completedPairs;
    int numOffGuesses;

    
    public int Level { get; set; }
    public PlayerInformation[] Players {  get; set; }
    public int CurrentPlayer;
    public string PlayerLabel { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    
    public MockTimerFunction timer = new MockTimerFunction();
    public string TimerText { get; set; } = "00:00";
    public bool hasStarted = false;
    public ICommand RestartCmd { get; }


    public MemoryBoardViewModel()

    {
       

    }


    public MemoryBoardViewModel(ICommand finishGameCommand,ICommand restartCmd,int level, List<string> playerList )
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
        ConfigureCards();
        timer.Reset();
        UpdateTimer();
    }

    private async void UpdateTimer()
    {
        while (true)
        {
            await Task.Delay(1000);
            TimerText = timer.GetTime();
        }
    }

    private async void OnButtonClicked(CardViewModel card)
    {

        if (!hasStarted)
        {
            StartTime = DateTime.Now;
            timer.Start();
            hasStarted = true;
        }

        await TurnCardsAsync(card);
        CheckForCompleation();
    }

    private async void CheckForCompleation()
    {
        if (completedPairs == (Cards.Count/2))
        {
            timer.Stop();
            EndTime = DateTime.Now;
            int mistakes = numOffGuesses - (Cards.Count/2);
            FinishGameCommand?.Execute((mistakes, numOffGuesses, TimerText, StartTime, EndTime));
        }

    }


    private async Task TurnCardsAsync(CardViewModel card) // Kan man lägga multiplayer här?
    {
       
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
            numOffGuesses++;
            Players[CurrentPlayer].Guesses++;

           
            await Task.Delay(800);

            // om korten inte matchar vänd tbx
            if (turnedCards[0].Id != turnedCards[1].Id)
            {
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
        UpdatePlayerLabel();
    }
    private void UpdatePlayerLabel()
    {
        // Här vill jag att det ska vara 2 spelare som vardera har ANTAL DRAG och ANTAL KORREKTA PAR
        // Varje spelare klickar på 2 kort och sedan går det vidare till nästas tur (visa spelarbyte med t.ex. fet stil på namn?)
        // Den som har flest antal korrekta par vinner
        // FRÅGOR????
        // 1. Lägga in namn även på multiplayer eller blir man spelare 1 och 2
        // 1.5 Hur ska det läggas in?? ska det dyka upp en till box för namn eller kan man återanvända namnboxen och fylla i en spelare i taget
        // 2. Här vill jag att det ska vara 2 spelare som vardera har ANTAL DRAG och ANTAL KORREKTA PAR
        // Varje spelare klickar på 2 kort och sedan går det vidare till nästas tur (visa spelarbyte med t.ex. fet stil på namn?)
        // Den som har flest antal korrekta par vinner
        // Vet inte om detta också ska ligga i databasen men det tar vi sen:)
        
        StringBuilder stringBuilder = new StringBuilder();
        foreach (PlayerInformation player in Players)
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
        PlayerLabel = stringBuilder.ToString();
    }

    private void ConfigureCards()
    {
        completedPairs = 0;
        var shuffled = MakeNumbersAndColors(Level);

        Cards.Clear();

        foreach (var card in shuffled)
        {
            Cards.Add(new CardViewModel(card, OnButtonClicked));
        }
    }
 

    public List<Cards> MakeNumbersAndColors(int level)
    {
        Level = level;
        Random random = new Random();
        _cards.Clear();
        int numberOfColors;
        switch (Level)
        {
            default:
            case 1:
                numberOfColors = 6;
                break;
            case 2:
                numberOfColors = 10;
                break;
            case 3:
                numberOfColors = 15;
                break;
        }
        for (int i = 0; i < numberOfColors; i++)
        {
            _cards.Add(new Cards(i));
            _cards.Add(new Cards(i));
        }
        return _cards = _cards.OrderBy(x => random.Next()).ToList();
    }

    
}
