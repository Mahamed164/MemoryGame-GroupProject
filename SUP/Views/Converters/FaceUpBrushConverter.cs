using PropertyChanged;
using SUP.Enums;
using SUP.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace SUP.Views.Converters //basically väldigt likt humanbenchmark sequence spelets kod
{
    [AddINotifyPropertyChangedInterface]

    public class FaceUpBrushConverter : IMultiValueConverter
    {
        private static readonly SolidColorBrush Backside = Make("#c4c3d0"); //lavendelgrå från internet

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length == 2 && values[0] is bool faceUp && values[1] is Brush color)
            {
                if (faceUp == true)
                {
                    return color;
                }
                else
                {
                    return Backside;
                }
            }

            return Backside;
        }
        //https://learn.microsoft.com/en-us/dotnet/api/system.windows.data.multibinding?view=windowsdesktop-9.0
        // https://stackoverflow.com/questions/7078820/implementing-imultivalueconverter-to-convert-between-units


        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
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