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
        public Bar LastBar => LastLeg.LastBar;

        private Pattern()
        {
            LegList = new List<Leg>();
        }
        public Pattern(Bar bar) : this()
        {
            var newLeg = new Leg(bar);
            LegList.Add(newLeg);
            this.Direction = newLeg.Direction == LegDirection.Up ? PatternDirection.Up : PatternDirection.Down;
        }
        public Pattern(Leg leg) : this()
        {
            LegList.Add(leg);
            this.Direction = leg.Direction == LegDirection.Up ? PatternDirection.Up : PatternDirection.Down;
        }

        public PatternDirection Direction { get; private set; }

        public PatternState State { get; set; }

    }
}
