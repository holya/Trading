using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trading.Common;

namespace Trading.Common
{
    public class Leg
    {
        public List<Bar> BarList { get; private set; }

        public double Open { get { return this.BarList.First().Open; } }
        public double High { get { return this.BarList.Max(b => b.High); } }
        public double Low { get { return this.BarList.Min(b => b.Low); } }
        public double Close { get { return this.LastBar.Close; } }
        public double Volume { get { return this.BarList.Sum(b => b.Volume); } }
        public Leg PreviousLeg { get; set; }
        public Bar LastBar { get { return this.BarList.LastOrDefault(); } }
        public Bar FirstBar { get { return this.BarList.First(); } }
        public DateTime StartDateTime { get { return this.BarList.First().DateTime; } }
        public int BarCount { get { return this.BarList.Count; } }

        public Bar HighestBar
        {
            get
            {
                var highestVal = this.BarList.Max(b => b.High);
                return this.BarList.Last(b => b.High == highestVal);
            }
        }
        public Bar LowestBar
        {
            get
            {
                var lowestVal = this.BarList.Min(b => b.Low);
                return this.BarList.Last(b => b.Low == lowestVal);
            }
        }

        public Leg()
        {
            this.BarList = new List<Bar>();
        }

        public Leg(Bar bar) : this()
        {
            this.BarList.Add(bar);
        }

        public Leg(Bar bar, Leg previousLeg) : this(bar)
        {
            PreviousLeg = previousLeg;
        }

        public bool AddBar(Bar bar)
        {
            if(this.Direction == LegDirection.Up && (bar.Direction == BarDirection.Up || bar.Direction == BarDirection.Balance))
            {
                this.BarList.Add(bar);
                return true;
            }

            if(this.Direction == LegDirection.Down && (bar.Direction == BarDirection.Down || bar.Direction == BarDirection.Balance))
            {
                this.BarList.Add(bar);
                return true;
            }

            return false;
        }
        

        public LegDirection Direction
        {
            get
            {
                return this.BarList.First().Direction > BarDirection.Balance ?
                            LegDirection.Up : LegDirection.Down;
            }
        }
    }
}
