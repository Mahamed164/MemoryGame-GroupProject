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
    public ObservableCollection<CardViewModel> Cards { get; private set; } = new();


    public MemoryBoardViewModel()
    {
        PressCardIndexCommand = new RelayCommand(p =>
        {
            if (p != null) return;
            OnButtonClicked(Convert.ToInt32(p));
        });
        ConfigurePads();
    }

    private async void OnButtonClicked(int index)
    {
        throw new NotImplementedException();
    }

    private void ConfigurePads()
    {
        for (int i = 0; i < 9; i++)
        {
            Cards.Add(new CardViewModel(i, OnButtonClicked));
        }
    }

    
}
