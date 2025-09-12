using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUP.ViewModels
{
    public class MockTimerFunction
    {
        public bool isRunning = false;
        public int seconds = 0;

        public void Reset()
        {
            isRunning = false;
            seconds = 0;
        }

        public void Start()
        {
            if (!isRunning)
            {
                isRunning = true;
                CountTime();
            }
        }

        private async void CountTime()
        {
            while (isRunning)
            {
                await Task.Delay(1000);
                seconds++;
            }
        }

        public string GetTime() // Returnerar tiden som sträng med min och sek. 
                                // Ex 75 sek blir "01:15" med hjälp av formatet nedan: 
        {
            TimeSpan ts = TimeSpan.FromSeconds(seconds); // https://learn.microsoft.com/en-us/dotnet/api/system.timespan.fromseconds?view=net-9.0 
            return ts.ToString(@"mm\:ss");
        }

        public void Stop()
        {
            isRunning = false;
        }
    }
}

