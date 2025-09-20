using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SUP.ViewModels
{
    public class Cards
    {
        public int Id { get; set; }

        //public Brush Color { get; set; }
        public ImageSource Image { get; set; }
        public bool FaceUp { get; set; }

        //https://www.mooict.com/c-tutorial-create-a-superhero-memory-game/
        private static ImageSource[] _images;

        public Cards(int id)
        {
            Id = id;
            //Color = ColorSelector(id);
            Image = _images[id];
            FaceUp = false;
        }
        //https://www.mooict.com/c-tutorial-create-a-superhero-memory-game/
        static Cards()
        {
            _images = new ImageSource[]
            {
                 new BitmapImage(new Uri("pack://application:,,,/SUP;component/Assets/Images/image1.png", UriKind.Absolute)),
                 new BitmapImage(new Uri("pack://application:,,,/SUP;component/Assets/Images/image2.png", UriKind.Absolute)),
                 new BitmapImage(new Uri("pack://application:,,,/SUP;component/Assets/Images/image3.png", UriKind.Absolute)),
                 new BitmapImage(new Uri("pack://application:,,,/SUP;component/Assets/Images/image4.png", UriKind.Absolute)),
                 new BitmapImage(new Uri("pack://application:,,,/SUP;component/Assets/Images/image5.png", UriKind.Absolute)),
                 new BitmapImage(new Uri("pack://application:,,,/SUP;component/Assets/Images/image6.png", UriKind.Absolute)),
                 new BitmapImage(new Uri("pack://application:,,,/SUP;component/Assets/Images/image7.png", UriKind.Absolute)),
                 new BitmapImage(new Uri("pack://application:,,,/SUP;component/Assets/Images/image8.png", UriKind.Absolute)),
                 new BitmapImage(new Uri("pack://application:,,,/SUP;component/Assets/Images/image9.png", UriKind.Absolute)),
                 new BitmapImage(new Uri("pack://application:,,,/SUP;component/Assets/Images/image10.png", UriKind.Absolute)),
                 new BitmapImage(new Uri("pack://application:,,,/SUP;component/Assets/Images/image11.png", UriKind.Absolute)),
                 new BitmapImage(new Uri("pack://application:,,,/SUP;component/Assets/Images/image12.png", UriKind.Absolute)),
                 new BitmapImage(new Uri("pack://application:,,,/SUP;component/Assets/Images/image13.png", UriKind.Absolute)),
                 new BitmapImage(new Uri("pack://application:,,,/SUP;component/Assets/Images/image14.png", UriKind.Absolute)),
                 new BitmapImage(new Uri("pack://application:,,,/SUP;component/Assets/Images/image15.png", UriKind.Absolute)),

                 /*https://stackoverflow.com/questions/20699572/what-is-difference-between-urikind-relative-and-urikind-absolute
                 UriKind.Relative: Relative Uri would be "relative to the project's structure,
                 with a forward-slash character / we're specifying the root from the project. Example:
                 new Uri("/index.html ", UriKind.Relative)

                UriKind.Absolute: Absolute URIs are Complete url, which start with protocol name Example:
                new Uri("http://www.testdomain.com/index.html ", UriKind.Absolute)

                UriKind.RelativeOrAbsolute:
                in this case runtime will figure it out for itself and It will attempt to clean up the 
                UriString we provide and figure out where resources are.*/
            };
        }

        //public Brush ColorSelector(int id)
        //{
        //    Brush[] colors = new Brush[]
        //    {
        //        Brushes.Red,
        //        Brushes.Blue,
        //        Brushes.Green,
        //        Brushes.Yellow,
        //        Brushes.Orange,
        //        Brushes.Pink,
        //        Brushes.Purple,
        //        Brushes.Turquoise,
        //        Brushes.Lime,
        //        Brushes.LightSalmon

        //    };
        //    return colors[id]; //id-1 pga om index 10 väljs blir det "out of range"
        }
    }
