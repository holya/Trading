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
        //public List<Pattern> PatternList { get; } = new List<Pattern>();

        public Leg LastLeg => LegList.Last();
        public Leg FirstLeg => LegList.First();
        public Bar LastBar => LastLeg.LastBar;
        //public Pattern LastPattern => PatternList.Last();
        public List<Reference> RefList { get; } = new List<Reference>();
        public double LastPrice => LastLeg.LastBar.Close;
        protected Reference LastSupport => RefList.Last(r => r.Price <= LastPrice);
        protected Reference LastResistance => RefList.Last(r => r.Price >= LastPrice);
        public Pattern PreviousPattern { get; }

        private Pattern()
        {
            LegList = new List<Leg>();
        }
        public Pattern(Bar bar) : this() { LegList.Add(new Leg(bar)); } //????
        public Pattern(Bar bar, Pattern previousPattern) : this(bar) { PreviousPattern = previousPattern; }

        public PatternDirection Direction // ????
        {
            get
            {
                if (this.FirstLeg.Direction == LegDirection.Up)
                    return PatternDirection.Up;
                return PatternDirection.Down;
            }
        }

        public PatternState State
        {
            get
            {
                if (this.Direction == PatternDirection.Up)
                {
                    if (LegList.Last().Direction == LegDirection.Up)
                    {

                        return PatternState.Continuation1;
                    }
                }
                else
                {

                }
            } 
        }

        public bool AddBar(Bar newBar)
        {
            if (this.Direction == PatternDirection.Up)
            {

            }
            else
            {

            }
        }

    }
}
