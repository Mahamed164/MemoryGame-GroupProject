using PropertyChanged;
using SUP.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using PropertyChanged;


namespace SUP.ViewModels
{
    public class GameViewModel
    {
        private CancellationTokenSource? _cts;

        IRandomService _randomService;

        Random random = new Random();
       public void GetRandomNumber()
        { 
            _cts?.Cancel();
            _cts = new CancellationTokenSource();
            int [] cards = new int[20];

            for (int i = 0; i < cards.Length; i++) 
            {
                cards[i] = random.Next(1,11);

                foreach (int card in cards) 
                {
                
            }

            //int value = random.Next(1, 11);
            //await Task.Delay(1000);
            //return new NumberResult(value);
        }

    }
}
