using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PropertyChanged;
using SUP.ViewModels;

namespace SUP.Services
{
    [AddINotifyPropertyChangedInterface]
    public class RandomService : IRandomService
    {
        Random _random = new Random();

        public async Task<NumberResult> RandomNumbers(CancellationToken ct)
        {
            int number = _random.Next(1,11);
            return new NumberResult(number);
        }
    }
}
