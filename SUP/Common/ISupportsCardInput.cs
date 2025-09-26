using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SUP.Common
{
    public interface ISupportsCardInput
    {
        public ICommand PressCardIndexCommand { get; }
    }
}