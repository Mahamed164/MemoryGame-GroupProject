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
        public List<Cards> MakeNumbersAndColors()
        {
            var cards = new List<Cards>();

            for (int i = 1; i <= 10; i++) // ändrar till att i = 1 och <= 10
            {
                cards.Add(new Cards(i));
                cards.Add(new Cards(i));

            }

            return cards = cards.OrderBy(x => random.Next()).ToList();

        }
    }
}
