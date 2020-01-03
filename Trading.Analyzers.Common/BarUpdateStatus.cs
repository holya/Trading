using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trading.Analyzers.Common
{
    public enum BarUpdateStatus
    {
        NoPriceChange,
        CloseUpdated,
        Expanded,
        TypeChanged
    }
}
