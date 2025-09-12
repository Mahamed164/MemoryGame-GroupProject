using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUP.ViewModels
{

    public class Result
    {
        public int Misses { get; set; }
        public int Guesses { get; set; }
        public string TimerText { get; set; }



        public void SetResult(int misses, int guesses, string timeText)
        {
            Misses = misses;
            Guesses = guesses;
            TimerText = timeText;
        }

       
    }
}