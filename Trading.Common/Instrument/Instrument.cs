using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trading.Common
{
    public struct Instrument
    {
        public Instrument(string name, InstrumentType type)
        {
            Name = name;
            Type = type;
        }
        public InstrumentType Type { get; set; }
        public string Name { get; set; }

    }
}
