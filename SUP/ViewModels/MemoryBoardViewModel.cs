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
    int completedPairs;


    public MockTimerFunction timer = new MockTimerFunction();
    public string TimerText { get; set; } = "00:00";
    public bool hasStarted = false;
    public MemoryBoardViewModel()
    {
        //MCF.MakeNumbersAndColors();

        //PressCardIndexCommand = new RelayCommand(p =>
        //{
        //    if (p != null) return;
        //    OnButtonClicked(Convert.ToInt32(p));
        //});
        //ConfigureCards();

    }


    public MemoryBoardViewModel(ICommand finishGameCommand)
    {
        FinishGameCommand = finishGameCommand;

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
        if (completedPairs == 10)
        {
            FinishGameCommand?.Execute(null);
        }

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

        return _cards = _cards.OrderBy(x => random.Next()).ToList();

    }
}




