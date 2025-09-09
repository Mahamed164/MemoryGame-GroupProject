using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PropertyChanged;

namespace SUP.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class NumberResult
    {
        public int Number { get; set; }

        public NumberResult(int number)
        {
            Number = number;
        }
    }
}
