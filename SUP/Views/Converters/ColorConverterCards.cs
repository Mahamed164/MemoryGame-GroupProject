using PropertyChanged;
using SUP.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace SUP.Views.Converters
{
    [AddINotifyPropertyChangedInterface]
    public class ColorConverterCards : IValueConverter
    {
        private static readonly SolidColorBrush HotPink = Make("#FF69B4");
        private static readonly SolidColorBrush Red = Make("#DC143C");
        private static readonly SolidColorBrush LightPink = Make("#FFB6C1");
        private static readonly SolidColorBrush Green = Make("#98FB98");
        private static readonly SolidColorBrush Blue = Make("#27ADF5");
        private static readonly SolidColorBrush DarkBlue = Make("#2B07F5");
        private static readonly SolidColorBrush Yellow = Make("#F1F507");
        private static readonly SolidColorBrush Orange = Make("#F59E07");
        private static readonly SolidColorBrush Purple = Make("#F527EB");
        private static readonly SolidColorBrush Turquoise = Make("#27EBF5");
        private static readonly SolidColorBrush White = Make("#FCFCFA");

        private static SolidColorBrush Make(string v)
        {
            throw new NotImplementedException();
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is FeedbackState state)
            {
                return state

                switch
                {
                    //FeedbackState.Wrong => Red,
                    //FeedbackState.Correct => Green,
                    //FeedbackState.Finished => LightPink,
                    //FeedbackState.Neutral => HotPink,
                    _ => White
                };
            }
            return HotPink;
            throw new NotImplementedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
