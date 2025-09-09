using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PropertyChanged;
using SUP.ViewModels;

namespace SUP.Services
{

    public interface IRandomService
    {
        Task<NumberResult> RandomNumbers(CancellationToken ct);
    }
}

