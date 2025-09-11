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
        var shuffled = cardFunction.MakeNumbersAndColors();

        Cards.Clear();

        foreach (var card in shuffled)
        {
            Cards.Add(new CardViewModel(card, OnButtonClicked));
        }
    }

    
}
