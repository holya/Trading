using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trading.Common;

namespace Trading.Common
{
    public class Pattern
    {
        public List<Leg> LegList { get; private set; }
       
        public Leg LastLeg => LegList.Last();
        public Leg FirstLeg => LegList.First();
        public Bar LastBar => LastLeg.LastBar;
        public Pattern PreviousPattern { get; set; }

        private Pattern()
        {
            LegList = new List<Leg>();
        }
        public Pattern(Bar bar) : this() { LegList.Add(new Leg(bar)); }
        public Pattern(Leg leg) : this() { LegList.Add(leg); }
        public Pattern(Bar bar, Pattern previousPattern) : this(bar) { PreviousPattern = previousPattern; }
        public Pattern(Leg leg, Pattern previousPattern) : this(leg) { PreviousPattern = previousPattern; }

        public PatternDirection Direction { get; set; }

        public PatternState State { get; set; }

    }
}
