using SUP.Enums;
using SUP.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace SUP.Views.Converters //basically väldigt likt humanbenchmark sequence spelets kod
{
    public class FaceUpBrushConverter : IValueConverter
    {

        private static readonly SolidColorBrush Backside = Make("#c4c3d0"); //lavendelgrå från internet
        

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            

            if(value is CardViewModel card)
            {
                if(card.FaceUp)
                {
                    return card.Color;
                }
                else
                {
                    return Backside;
                }
            }

            return Backside;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public static SolidColorBrush Make(string hex)
        {
            var color = (Color)ColorConverter.ConvertFromString(hex)!; 
            var b = new SolidColorBrush(color);
            return b;
        }
    }
}
