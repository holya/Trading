using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Trading.Common
{
    public struct Resolution
    {
        public Resolution(TimeFrame timeFrame, int size)
        {
            TimeFrame = timeFrame;
            Size = size;
        }
        public TimeFrame TimeFrame;
        public int Size;
    }
}
