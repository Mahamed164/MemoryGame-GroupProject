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


    public MockTimerFunction timer = new MockTimerFunction();
    public string TimerText { get; set; } = "00:00";
    public bool hasStarted = false;
    public ICommand RestartCmd { get; }
    public MemoryBoardViewModel()
    {
       

    }


    public MemoryBoardViewModel(ICommand finishGameCommand,ICommand restartCmd )
    {
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
            int mistakes = numOffGuesses - (Cards.Count/2);
            FinishGameCommand?.Execute((mistakes, numOffGuesses, TimerText));
        }

    }

    private int CalculateAccuracy()
    {
        int accuracySubtract;
        int amountOfPairs = _cards.Count / 2;
        accuracySubtract = 100 / amountOfPairs;

        return accuracySubtract;
    }


    private async Task TurnCardsAsync(CardViewModel card)
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
            completedPairs++;
            await Task.Delay(800);

            // om korten inte matchar vänd tbx
            if (turnedCards[0].Id != turnedCards[1].Id)
            {
                turnedCards[0].FaceUp = false;
                turnedCards[1].FaceUp = false;
                completedPairs--;
               
            }
            turnedCards.Clear();
        }

    }

    private void ConfigureCards()
    {
      
        completedPairs = 0;
        var shuffled = MakeNumbersAndColors();

        Cards.Clear();

        foreach (var card in shuffled)
        {
            Cards.Add(new CardViewModel(card, OnButtonClicked));
        }
    }

    public List<Cards> MakeNumbersAndColors()
    {
        Random random = new Random();
        _cards.Clear();

        for (int i = 0; i < 10; i++)
        {
            _cards.Add(new Cards(i));
            _cards.Add(new Cards(i));

        }

        return  _cards.OrderBy(x => random.Next()).ToList();

    }
}




