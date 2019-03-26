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
        public List<Reference> RefList { get; } = new List<Reference>();
        public List<Leg> LegList { get; private set; }
       
        public Leg LastLeg => LegList.Last();
        public Bar LastBar => LastLeg.LastBar;

        private Pattern()
        {
            LegList = new List<Leg>();
        }
        public Pattern(Bar bar) : this() { LegList.Add(new Leg(bar)); }
        public Pattern(Leg leg) : this() { LegList.Add(leg); }

        private double PatternStart
        {
            get
            {
                if (LegList.First().Direction == LegDirection.Up)
                    return LegList.First().FirstBar.Low;
                else
                    return LegList.First().FirstBar.High;
            }
        }

        public PatternDirection Direction
        {
            get
            {
                return LegList.First().Direction == LegDirection.Up ? PatternDirection.Up : PatternDirection.Down;
            }
        }

        public State State
        {
            get
            {
                if (Direction == PatternDirection.Up)
                {
                    if (LastLeg.Direction == LegDirection.Down)
                    {
                        if (LastLeg.LastBar.Low >= PatternStart)
                            return State.PullBack1;
                        else
                            return State.PullBack2;
                    }
                    if (LastLeg.PreviousLeg.FirstBar.High >= LastLeg.LastBar.High)
                        return State.Continuation1;
                    else
                        return State.Continuation2;
                }
                else
                {
                    if (LastLeg.Direction == LegDirection.Up)
                    {
                        if (LastLeg.LastBar.High < PatternStart && LastLeg.LastBar.High == PatternStart)
                            return State.PullBack1;
                        else
                            return State.PullBack2;
                    }
                    if (LastLeg.PreviousLeg.FirstBar.Low <= LastLeg.LastBar.Low)
                        return State.Continuation1;
                    else
                        return State.Continuation2;
                }
            }
        }

        public bool AddBar(Bar bar)
        {
            if (Direction == PatternDirection.Up)
            {
                if (bar.Low < LegList.Last(l => l.Direction == LegDirection.Up).Low)
                    return false;

                if (LastLeg.Direction == LegDirection.Up)
                {
                    if (bar.Direction == BarDirection.Up || bar.Direction == BarDirection.Balance)
                    {
                        LastLeg.AddBar(bar);
                    }
                    else
                    {
                        LegList.Add(new Leg(bar, LastLeg));
                    }
                }

                return true;
            }
            else
            {
                if (bar.High > LegList.Last(l => l.Direction == LegDirection.Down).High)
                    return false;

                if (LastLeg.Direction == LegDirection.Up)
                {
                    if (bar.Direction == BarDirection.Up || bar.Direction == BarDirection.Balance)
                    {
                        LastLeg.AddBar(bar);
                        return true;
                    }
                    else
                    {
                        LegList.Add(new Leg(bar, LastLeg));
                        return true;
                    }
                }

                return true;
            }
        }
    }
}
