using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using PropertyChanged;

namespace SUP.Views.Converters;

[AddINotifyPropertyChangedInterface]

public class FaceUpImageConverter : IMultiValueConverter // Samma som den tidigare FaceUpBrushConvertern fast med bild ist
{
    private static readonly ImageSource Backside = new BitmapImage(new Uri("pack://application:,,,/SUP;component/Assets/Images/backside.jpg", UriKind.Absolute));

    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.Length == 2 && values[0] is bool faceUp && values[1] is ImageSource image)
        {
            if (faceUp == true)
            {
                return image;
            }
            else
            {
                return Backside;
            }
        }

        return Backside;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
