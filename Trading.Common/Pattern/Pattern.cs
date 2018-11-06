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
        public Pattern() => LegList = new List<Leg>();
        public Pattern(Leg leg) : this() => LegList.Add(leg);
        public PatternDirection Direction => LegList.First().Direction == LegDirection.Up ? PatternDirection.Up : PatternDirection.Down;

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

        //public bool AddBar(Bar bar)
        //{
        //    if (Direction == PatternDirection.Up)
        //    {
        //        if (bar.Low < LegList.Last(l => l.Direction == LegDirection.Up).Low)
        //            return false;

        //        if(LastLeg.Direction == LegDirection.Up)
        //        {
        //            if(bar.Direction == BarDirection.Up || bar.Direction == BarDirection.Balance)
        //            {
        //                LastLeg.AddBar(bar);
        //                return true;
        //            }
        //            else
        //            {
        //                LegList.Add(new Leg(bar, LastLeg));
        //                return true;
        //            }
        //        }
        //    }
        //    else
        //    {
        //        if (bar.High > LegList.Last(l => l.Direction == LegDirection.Down).High)
        //            return false;

        //        if (LastLeg.Direction == LegDirection.Up)
        //        {
        //            if (bar.Direction == BarDirection.Up || bar.Direction == BarDirection.Balance)
        //            {
        //                LastLeg.AddBar(bar);
        //                return true;
        //            }
        //            else
        //            {
        //                LegList.Add(new Leg(bar, LastLeg));
        //                return true;
        //            }
        //        }


        //    }
        //}
    }
}
