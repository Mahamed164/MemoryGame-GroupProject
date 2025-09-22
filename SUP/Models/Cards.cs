using SUP.ViewModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SUP.Models
{
    public class Cards
    {
        public int Id { get; set; }
        public ImageSource Image { get; set; }
        public bool FaceUp { get; set; }


        

        //https://www.mooict.com/c-tutorial-create-a-superhero-memory-game/
        private static ImageSource[] _images;

        public Cards(int id)
        {
            Id = id;
            Image = _images[id];
            FaceUp = false;
            
        }

        static Cards()
        {
            _images = new ImageSource[]
            {
                //https://stackoverflow.com/questions/20699572/what-is-difference-between-urikind-relative-and-urikind-absolute

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
            };
        }
    }
}
