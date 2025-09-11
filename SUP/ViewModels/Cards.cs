using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SUP.ViewModels
{
    public class Cards
    {
        public int Id { get; set; }

        public Brush Color { get; set; }

        public bool FaceUp { get; set; }

        public Cards(int id)
        {
            Id = id;
            Color = ColorSelector(id);
            FaceUp = false;
        }

        public Brush ColorSelector(int id)
        {
            Brush[] colors = new Brush[]
            {
                Brushes.Red,
                Brushes.Blue,
                Brushes.Green,
                Brushes.Yellow,
                Brushes.Orange,
                Brushes.Pink,
                Brushes.Purple,
                Brushes.Turquoise,
                Brushes.Lime,
                Brushes.LightSalmon

            };
            return colors[id - 1]; //id-1 pga om index 10 väljs blir det "out of range"
        }
    }
}
