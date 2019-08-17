using System;
using System.Collections;

namespace Trading.Common
{
    public class Bar
    {
        //public double Head { get; set; }
        //public double Tail { get; set; }

        public double Open { get; set; }
        public double High { get; set; }
        public double Low { get; set; }
        public double Close { get; set; }
        public double Volume { get; set; }
        public DateTime DateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public DateTime PreviousBarDateTime { get { return PreviousBar.DateTime; } }
        public Bar PreviousBar { get; set; }

        public int BarCode { get { return (int)this.Direction; } }

        public Bar() { }

        private Bar(double open, double high, double low, double close, double volume, DateTime dateTime, DateTime endDateTime) : this()
        {
            this.Open = open;
            this.High = high;
            this.Low = low;
            this.Close = close;
            this.Volume = volume;
            this.DateTime = dateTime;
            this.EndDateTime = endDateTime;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="open"></param>
        /// <param name="high"></param>
        /// <param name="low"></param>
        /// <param name="close"></param>
        /// <param name="volume"></param>
        /// <param name="dateTime"></param>
        /// <param name="previousBar">If null, open is used to create a bar as open, high, low and close.</param>
        public Bar(double open, double high, double low, double close, double volume, DateTime dateTime, DateTime endDateTime, Bar previousBar = null)
            : this(open, high, low, close, volume, dateTime, endDateTime) // this is the same constructor as above!
        {
            if(previousBar != null)
                PreviousBar = previousBar;
            else
                this.PreviousBar = new Bar(open, open, open, open, 0, dateTime, endDateTime);
        }

        public BarDirection Direction
        {
            get
            {
                if(Low >= PreviousBar.Low && High <= PreviousBar.High)
                    return BarDirection.Balance;

                if(Low >= PreviousBar.Low && High > PreviousBar.High)
                {
                    if(Low > PreviousBar.High)
                        return BarDirection.GapUp;
                    return BarDirection.Up;
                }

                if(Low < PreviousBar.Low && High <= PreviousBar.High)
                {
                    if(High < PreviousBar.Low)
                        return BarDirection.GapDown;
                    return BarDirection.Down;
                }

                if(Close >= Open)
                    return BarDirection.OutsideUp;
                return BarDirection.OutsideDown;
            }
        }

        public bool IsSameDirection(Bar bar)
        {
            if (bar.Direction == BarDirection.Balance) return true;
            if (bar.Direction < BarDirection.Balance & Direction < BarDirection.Balance) return true;
            if (bar.Direction > BarDirection.Balance & Direction > BarDirection.Balance) return true;
            return false;
        }

        public virtual void Update(Bar bar)
        {
            //Open = bar.Open;
            if(bar.High > High)
                High = bar.High;
            if(bar.Low < Low)
                Low = bar.Low;

            Close = bar.Close;
            Volume += bar.Volume;
            //DateTime = bar.DateTime;
        }

        public override string ToString()
        {
            return $"{Open},{High},{Low},{Close},{Volume},{DateTime},{EndDateTime}";
        }
    }
}
