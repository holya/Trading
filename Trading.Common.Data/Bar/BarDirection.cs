using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Trading.Common
{
    public enum BarDirection
    {
        Down,
        GapDown,
        OutsideDown,

        Balance,

        Up,
        GapUp,
        OutsideUp,
    }
}
