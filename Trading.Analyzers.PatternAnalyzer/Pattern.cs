using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trading.Common;

namespace Trading.Analyzers.PatternAnalyzer
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
        public PatternType Type { get; set; }
        //{
        //    get
        //    {
        //        if(Direction == PatternDirection.Up)
        //        {

        //        }
        //        else
        //        {

        //        }
        //    }
        //}

        public void AddLeg(Leg leg) { LegList.Add(leg); }
    }
}
