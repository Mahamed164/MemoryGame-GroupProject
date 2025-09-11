using PropertyChanged;
using SUP.Commands;
using SUP.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SUP.ViewModels;
[AddINotifyPropertyChangedInterface]
public class MemoryBoardViewModel : ISupportsCardInput
{
    public ICommand PressCardIndexCommand { get; }
    public ObservableCollection<CardViewModel> Cards { get; } = new();

    MockCardsFunction MCF = new MockCardsFunction();


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
    }

    private async void OnButtonClicked(CardViewModel card)
    {

        card.FaceUp = !card.FaceUp;
    }

    private void ConfigureCards()
    {
        //for (int i = 0; i < 20; i++)
        //{
        //    Cards.Add(new CardViewModel(i, OnButtonClicked));
        //}

        var cardFunction = new MockCardsFunction();
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

        for (int i = 1; i <= 10; i++) // ändrar till att i = 1 och <= 10
        {
            cards.Add(new Cards(i));
            cards.Add(new Cards(i));

        }

        return cards = cards.OrderBy(x => random.Next()).ToList();

    }
}




