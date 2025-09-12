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
using System.Windows.Input;
using System.Windows.Media;

namespace SUP.ViewModels;
[AddINotifyPropertyChangedInterface]
public class MemoryBoardViewModel : ISupportsCardInput
{
    public ICommand PressCardIndexCommand { get; }
    public ObservableCollection<CardViewModel> Cards { get; } = new();
    FaceUpBrushConverter FUBC = new FaceUpBrushConverter();

    // lista för att hålla koll på vilka kort som är vända
    private List<CardViewModel> turnedCards = new();

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
            await Task.Delay(800);

            // om korten inte matchar vänd tbx
            if (turnedCards[0].Id != turnedCards[1].Id)
            {
                turnedCards[0].FaceUp = false;
                turnedCards[1].FaceUp = false;
            }
            turnedCards.Clear();
        }

    }

    private void ConfigureCards()
    {
        //for (int i = 0; i < 20; i++)
        //{
        //    Cards.Add(new CardViewModel(i, OnButtonClicked));
        //}


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
        List<Cards> cards = new List<Cards>();

        for (int i = 0; i < 10; i++) // ändrar till att i = 1 och <= 10
        {
            cards.Add(new Cards(i));
            cards.Add(new Cards(i));

        }

        return cards = cards.OrderBy(x => random.Next()).ToList();

    }
}




