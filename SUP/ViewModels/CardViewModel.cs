using PropertyChanged;
using SUP.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace SUP.ViewModels
{
    public class CardViewModel
    {
        public int Id { get; set; }
        public ICommand ClickCommand { get; set; }

        public bool IsClicked { get; set; }

        public string HexCode { get; set; }

        public CardViewModel(int id, Action<int> onClick)
        {
            Id = id;
            ClickCommand = new RelayCommand(_ => onClick(Id));
        }

        Random random = new Random();

        public RandomColorGenerator(int index)
        {
            for (int i = 0; i < 10; i++)
            {
                tasks[i] = _gameService.RollDiceAsync(_cts.Token);
            }

        }

        //private static readonly SolidColorBrush HotPink = Make("#FF69B4");
        //private static readonly SolidColorBrush Red = Make("#DC143C");
        //private static readonly SolidColorBrush LightPink = Make("#FFB6C1");
        //private static readonly SolidColorBrush Green = Make("#98FB98");
        //private static readonly SolidColorBrush Blue = Make("#27ADF5");
        //private static readonly SolidColorBrush DarkBlue = Make("#2B07F5");
        //private static readonly SolidColorBrush Yellow = Make("#F1F507");
        //private static readonly SolidColorBrush Orange = Make("#F59E07");
        //private static readonly SolidColorBrush Purple = Make("#F527EB");
        //private static readonly SolidColorBrush Turquoise = Make("#27EBF5");
        //private static readonly SolidColorBrush White = Make("#FCFCFA");
    }
}
