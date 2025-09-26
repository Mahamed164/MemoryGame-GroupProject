using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUP.Models
{
    public class Result
    {
        public int Misses { get; set; }
        public int Guesses { get; set; }
        public string TimerText { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int Level { get; set; }
        public int TimeAsInt {  get; set; }

        public Result(int misses, int guesses, string timeText, DateTime startTime, DateTime endTime, int level)
        {
            Misses = misses;
            Guesses = guesses;
            TimerText = timeText;
            StartTime= startTime; 
            EndTime= endTime;
            Level = level;
        }
    }
}