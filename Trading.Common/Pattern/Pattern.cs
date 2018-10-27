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

        public Pattern()
        {
            LegList = new List<Leg>();
        }

        public PatternDirection Direction
        {
            get
            {
                return LegList.First().Direction == LegDirection.Up ? PatternDirection.Up : PatternDirection.Down;
            }
        }
        //public PatternType Type
        //{
        //    get
        //    {
        //        if (Direction == PatternDirection.Up)
        //        {

        //        }
        //        else
        //        {

        //        }
        //    }
        //}

        //public bool AddLeg(Leg leg)
        //{
        //    if(Direction == PatternDirection.Up)
        //    {
        //        if (leg.Low < LegList.Last(l => l.Direction == LegDirection.Up).Low)
        //            return false;
        //        if(leg.Direction == LegDirection.Up)
        //    }
        //}
    }
}
