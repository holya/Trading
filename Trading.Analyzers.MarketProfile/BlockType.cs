using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Trading.Analyzers
{
    public enum BlockType
    {
        None, Poc, BuyingTail, SellingTail, SinglePrint, ValueAreaLow, ValueAreaHigh, Gap
    }
}
