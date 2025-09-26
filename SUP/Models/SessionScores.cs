using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUP.Models
{

    public class SessionScores
    {
        public int SessionId { get; set; }
        public string Nickname { get; set; }
        public DateTime TimeOfPlay { get; set; }
        public int Moves { get; set; }
        public int Misses { get; set; }
        public string TimerText { get; set; }
        public int Level { get; set; }
    }
}