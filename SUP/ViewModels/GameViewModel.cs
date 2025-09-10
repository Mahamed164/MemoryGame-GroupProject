using PropertyChanged;
using SUP.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using PropertyChanged;


namespace SUP.ViewModels;
[AddINotifyPropertyChangedInterface]

public class GameViewModel
{
    private CancellationTokenSource? _cts;

    IRandomService _randomService;

    Random random = new Random();

    public void GetRandomNumber()
    {
        _cts?.Cancel();
        _cts = new CancellationTokenSource();
        //int[] cards = new int[20];
        List<int> randomNumbers = new List<int>();

        for (int i = 0; i < 10; i++)
        {
            randomNumbers.Add(i);
            randomNumbers.Add(i);


            //card[i] = random.Next(1, 11);

            //foreach (int card in cards)
            //{
            //if (cards[i])//om det slumpade talet för cards[i](tror att det är rätt variabel)
            //                  //INTE redan finns med i cards så ska den siffran användas
            //{
            //    //då ska det läggas in i "mot" kortet så att kortet har ett Id och ett RandomNumber. 
            //    //Det ska då finnas 20 olika index men 10 olika randomnumbers som är fördelade 2 och 2 över indexen
            //}
            //else
            //{
            //    //testa igen
            //}

        }

        randomNumbers = randomNumbers.OrderBy(x => random.Next()).ToList();

        //int value = random.Next(1, 11);
        //await Task.Delay(1000);
        //return new NumberResult(value);
    }

}

