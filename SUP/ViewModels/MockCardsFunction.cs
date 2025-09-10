using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SUP.ViewModels
{

    public class MockCardsFunction
    {
        Random random = new Random();
        List<Cards> cards = new List<Cards>();
        public void MakeNumbersAndColors()
        {

            for (int i = 0; i < 10; i++)
            {
                cards.Add(new Cards(i));
                cards.Add(new Cards(i));

            }

            cards = cards.OrderBy(x => random.Next()).ToList();

        }
    }
}
