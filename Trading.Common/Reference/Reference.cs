using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Trading.Common;

namespace Trading.Common
{
    public struct Reference
    {
        public double Price;
        public DateTime DateTime;
        public short HitCount;
        public ReferenceType Type;
        public Bar Owner;
    }
}
