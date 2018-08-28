using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Trading.Common;
using Trading.Analyzers.Common;
      
namespace Trading.Analyzers.LegAnalyzer
{
    public partial class LegAnalyzer
    {
        private Action<Bar> addBar;


        #region Properties

        public Bar LastBar { get { return this.LastLeg.LastBar; } } 
        public Leg LastLeg { get { return this.LegList.Last(); } }
        public double Close { get { return this.LastBar.Close; } }

        //public double BarVolitility { get { return this.LastBar.High - this.LastBar.Low / 100; } }
        //public double LastSupport { get { return this.RefList.FirstOrDefault(r => r.Price < this.Close).Price; } }
        //public double LastResistance { get { return this.RefList.FirstOrDefault(r => r.Price < this.Close).Price; } }
        public List<Leg> LegList { get; private set; }
        public int LegsCount { get { return this.LegList.Count; } }
        

        //public List<double> AdvanceList { get; private set; }
        //public List<double> DeclineList { get; private set; }

        public List<Reference> RefList { get; private set; }

        #endregion

        public LegAnalyzer()
        {
            this.LegList = new List<Leg>();
            this.RefList = new List<Reference>();

            this.addBar = this.doFirstLeg;  
        }
        public int BarsCount { get { return LegList.Sum(leg => leg.BarCount); } }
        public Resolution Resolution { get; set; }

        public void AddBarList(IEnumerable<Tuple<double, double, double, double, double, DateTime>> barList)
        {
            foreach(var item in barList)
            {
                AddBar(item.Item1, item.Item2, item.Item3, item.Item4, item.Item5, item.Item6);
            }
        }
        public void AddBarList2(IEnumerable<Bar> barList)
        {
            foreach(var bar in barList)
            {
                this.addBar(bar);
            }
        }
        public void AddBarList(IEnumerable<Bar> barList)
        {
            var d = barList.ElementAt(0);
            d.PreviousBar = new Bar(d.Open, d.Open, d.Open, d.Open, d.Volume, d.DateTime);
            LegList.Add(new Leg(d));

            for (int i = 1; i < barList.Count(); i++)
            {
                var currentBar = barList.ElementAt(i);
                currentBar.PreviousBar = barList.ElementAt(i - 1);
                if (currentBar.PreviousBar.IsSameDirection(currentBar))
                {
                    LegList.Last().AddBar(currentBar);
                    continue;
                }
                LegList.Add(new Leg(currentBar) { PreviousLeg = LegList.Last() });
            }
        }


        public void AddBar(double open, double high, double low, double close, double volume, DateTime time)
        {
            this.addBar(new Bar(open, high, low, close, volume, time));
            
        }
        public void AddBar(Bar bar)
        {
            this.addBar(bar);
        }

        private void doFirstLeg(Bar bar)
        {
            double newBarOpen = bar.Open;
            var prevBar = new Bar(newBarOpen, newBarOpen, newBarOpen, newBarOpen, 0, bar.DateTime, null);
            bar.PreviousBar = prevBar;

            var leg = new Leg(bar);
            leg.PreviousLeg = leg;
            this.LegList.Add(leg);

            this.RefList.Add(leg.Direction == LegDirection.Up ? 
                new Reference() { Price = leg.Low, DateTime = leg.StartDateTime, Owner = this.LastBar } :
                new Reference() { Price = leg.High, DateTime = leg.StartDateTime, Owner = this.LastBar });
            
            this.addBar = this.continueBuildingLeg;
            this._onAnalyzerUpdated(LegAnalyzerUpdateEventEnum.NewLegAdded);
        }

        private void continueBuildingLeg(Bar bar)
        {
            //var bar = new Bar(open, high, low, close, volume, time, this.LastBar);
            bar.PreviousBar = LastBar;

            if(this.LastLeg.Direction == LegDirection.Up)
            {
                switch(bar.Direction)
                {
                    case BarDirection.Up:
                    case BarDirection.GapUp:
                    case BarDirection.Balance:
                        this.LastLeg.AddBar(bar);
                        this.analyzeprevRefs();

                        this._onAnalyzerUpdated(LegAnalyzerUpdateEventEnum.NewBarAdded);
                        return;
                    case BarDirection.GapDown:
                    case BarDirection.Down:
                        this.addReferenceForHighOfThisBar(this.LastLeg.HighestBar);
                        break;
                    case BarDirection.OutsideDown:
                        if(bar.High < this.LastLeg.HighestBar.High)
                            this.addReferenceForHighOfThisBar(this.LastLeg.HighestBar);
                        addReferenceForHighOfThisBar(bar);
                        break;
                    case BarDirection.OutsideUp:
                        this.addReferenceForHighOfThisBar(this.LastLeg.HighestBar);
                        this.addReferenceForLowOfThisBar(bar);
                        break;
                }
            }
            else 
            {
                switch(bar.Direction)
                {
                    case BarDirection.Down:
                    case BarDirection.GapDown:
                    case BarDirection.Balance:
                        this.LastLeg.AddBar(bar);
                        this.analyzeprevRefs();

                        this._onAnalyzerUpdated(LegAnalyzerUpdateEventEnum.NewBarAdded);
                        return;
                    case BarDirection.GapUp:
                    case BarDirection.Up:
                        this.addReferenceForLowOfThisBar(this.LastLeg.LowestBar);
                        break;
                    case BarDirection.OutsideDown:
                        this.addReferenceForLowOfThisBar(this.LastLeg.LowestBar);
                        this.addReferenceForHighOfThisBar(bar);
                        break;
                    case BarDirection.OutsideUp:
                        if(bar.Low > this.LastLeg.LowestBar.Low)
                            this.addReferenceForLowOfThisBar(this.LastLeg.LowestBar);
                        this.addReferenceForLowOfThisBar(bar);
                        break;
                }
            }

            this.analyzeprevRefs();
            var leg = new Leg(bar, this.LastLeg);
            this.LegList.Add(leg);
            this._onAnalyzerUpdated(LegAnalyzerUpdateEventEnum.NewLegAdded);
        }

        private void addReferenceForHighOfThisBar(Bar bar)
        {
            this.RefList.Add(new Reference { Price = bar.High, DateTime = bar.DateTime, Owner = bar });
        }

        private void addReferenceForLowOfThisBar(Bar bar)
        {
            this.RefList.Add(new Reference { Price = bar.Low, DateTime = bar.DateTime, Owner = bar });
        }

        public void UpdateBar(double open, double high, double low, double close, double volume, DateTime time)
        {
            if(!this.lastBarShouldBeUpdated(open, high, low, close))
            {
                this.LastBar.Volume += volume;
                this.LastBar.Close = close;

                this._onAnalyzerUpdated(LegAnalyzerUpdateEventEnum.CloseUpdated);
                return;
            }

            var oldLegDirection = this.LastLeg.Direction;
            var oldLastBarDirection = this.LastBar.Direction;
            if(high > this.LastBar.High)
                LastBar.High = high;
            if(low < this.LastBar.Low)
                LastBar.Low = low;
            this.LastBar.Close = close;
            this.LastBar.Volume += volume;
            //this.LastBar.DateTime = time;
            BarDirection newBarDirection = this.LastBar.Direction;

            if(oldLegDirection == LegDirection.Up)
            {
                switch(oldLastBarDirection)
                {
                    case BarDirection.Balance:
                        switch(newBarDirection)
                        {
                            case BarDirection.Down:
                                var l = this.LastBar;
                                this.LastLeg.BarList.Remove(this.LastBar);
                                this.addBar(l);
                                break;
                            case BarDirection.OutsideDown:
                                var nb = this.LastBar;
                                this.LastLeg.BarList.Remove(this.LastBar);
                                this.addBar(nb);
                                break;
                            case BarDirection.Balance:
                                this._onAnalyzerUpdated(LegAnalyzerUpdateEventEnum.LastBarExpanded);
                                break;
                            case BarDirection.Up:
                                this._onAnalyzerUpdated(LegAnalyzerUpdateEventEnum.LastBarExpanded);
                                break;
                            case BarDirection.OutsideUp:
                                var newBar = this.LastBar;
                                this.LastLeg.BarList.Remove(this.LastBar);
                                this.addBar(newBar);
                                break;
                        }
                        break;
                    case BarDirection.Up:
                        switch(newBarDirection)
                        {
                            case BarDirection.OutsideDown:
                                var newBar = this.LastBar;
                                this.LastLeg.BarList.Remove(this.LastBar);
                                if(this.LastLeg.BarCount == 0)
                                {
                                    this.LegList.Remove(this.LastLeg);
                                    this.RefList.Remove(this.RefList.Last());
                                }
                                this.addBar(newBar);
                                break;
                            case BarDirection.Up:
                                this._onAnalyzerUpdated(LegAnalyzerUpdateEventEnum.LastBarExpanded);
                                break;
                            case BarDirection.OutsideUp:
                                var lb = this.LastBar;
                                this.LastLeg.BarList.Remove(this.LastBar);
                                if(this.LastLeg.BarCount == 0)
                                {
                                    this.LegList.Remove(this.LastLeg);
                                    this.RefList.Remove(this.RefList.Last());
                                }
                                this.addBar(lb);
                                break;
                        }
                        break;
                    case BarDirection.GapUp:
                        switch(newBarDirection)
                        {
                            case BarDirection.OutsideDown:
                                var lb = this.LastBar;
                                this.LastLeg.BarList.Remove(this.LastBar);
                                if(this.LastLeg.BarCount == 0)
                                {
                                    this.LegList.Remove(this.LastLeg);
                                    this.RefList.Remove(this.RefList.Last());
                                }
                                this.addBar(lb);
                                break;
                            case BarDirection.Up:
                            case BarDirection.GapUp:
                                this._onAnalyzerUpdated(LegAnalyzerUpdateEventEnum.LastBarExpanded);
                                break;
                            case BarDirection.OutsideUp:
                                var nb = this.LastBar;
                                this.LastLeg.BarList.Remove(this.LastBar);
                                if(this.LastLeg.BarCount == 0)
                                {
                                    this.LegList.Remove(this.LastLeg);
                                    this.RefList.Remove(this.RefList.Last());
                                }
                                this.addBar(nb);
                                break;
                        }
                        break;
                    case BarDirection.OutsideUp:
                        switch(newBarDirection)
                        {
                            case BarDirection.OutsideDown:
                                this.RefList.Remove(this.RefList.Last());
                                var nb = this.LastBar;
                                this.LastLeg.BarList.Remove(this.LastBar);
                                if(this.LastLeg.BarCount == 0)
                                {
                                    this.LegList.Remove(this.LastLeg);
                                    if(this.LegsCount == 0)
                                        this.addBar = this.doFirstLeg;
                                }
                                this.addBar(nb);
                                break;
                            case BarDirection.OutsideUp:
                                this._onAnalyzerUpdated(LegAnalyzerUpdateEventEnum.LastBarExpanded);
                                break;
                        }
                        break;
                }
            }
            else
            {
                switch(oldLastBarDirection)
                {
                    case BarDirection.Down:
                        switch(newBarDirection)
                        {
                            case BarDirection.Down:
                                this._onAnalyzerUpdated(LegAnalyzerUpdateEventEnum.LastBarExpanded);
                                break;
                            case BarDirection.OutsideDown:
                                var lb = this.LastBar;
                                this.LastLeg.BarList.Remove(this.LastBar);
                                if(this.LastLeg.BarCount == 0)
                                {
                                    this.LegList.Remove(this.LastLeg);
                                    this.RefList.Remove(this.RefList.Last());
                                }
                                this.addBar(lb);
                                break;
                            case BarDirection.OutsideUp:
                                var nb = this.LastBar;
                                this.LastLeg.BarList.Remove(this.LastBar);
                                if(this.LastLeg.BarCount == 0)
                                {
                                    this.LegList.Remove(this.LastLeg);
                                    this.RefList.Remove(this.RefList.Last());
                                }
                                this.addBar(nb);
                                break;
                        }
                        break;
                    case BarDirection.GapDown:
                        switch(newBarDirection)
                        {
                            case BarDirection.Down:
                                this._onAnalyzerUpdated(LegAnalyzerUpdateEventEnum.LastBarExpanded);
                                break;
                            case BarDirection.GapDown:
                                this._onAnalyzerUpdated(LegAnalyzerUpdateEventEnum.LastBarExpanded);
                                break;
                            case BarDirection.OutsideDown:
                                var lb = this.LastBar;
                                this.LastLeg.BarList.Remove(this.LastBar);
                                if(this.LastLeg.BarCount == 0)
                                {
                                    this.LegList.Remove(this.LastLeg);
                                    this.RefList.Remove(this.RefList.Last());
                                }

                                this.addBar(lb);
                                break;
                            case BarDirection.OutsideUp:
                                var nb = this.LastBar;
                                this.LastLeg.BarList.Remove(this.LastBar);
                                if(this.LastLeg.BarCount == 0)
                                {
                                    this.LegList.Remove(this.LastLeg);
                                    this.RefList.Remove(this.RefList.Last());
                                }
                                this.addBar(nb);
                                break;
                        }
                        break;
                    case BarDirection.OutsideDown:
                        switch(newBarDirection)
                        {
                            case BarDirection.OutsideDown:
                                this._onAnalyzerUpdated(LegAnalyzerUpdateEventEnum.LastBarExpanded);
                                break;
                            case BarDirection.OutsideUp:
                                this.RefList.Remove(this.RefList.Last());
                                var lb = this.LastBar;
                                this.LastLeg.BarList.Remove(this.LastBar);
                                if(this.LastLeg.BarCount == 0)
                                {
                                    this.LegList.Remove(this.LastLeg);
                                    if(this.LegsCount == 0)
                                        this.addBar = this.doFirstLeg;
                                }
                                this.addBar(lb);
                                break;
                        }
                        break;
                    case BarDirection.Balance:
                        switch(newBarDirection)
                        {
                            case BarDirection.Down:
                                this._onAnalyzerUpdated(LegAnalyzerUpdateEventEnum.LastBarExpanded);
                                break;
                            case BarDirection.OutsideDown:
                                var newBar = this.LastBar;
                                this.LastLeg.BarList.Remove(this.LastBar);
                                this.addBar(newBar);
                                break;
                            case BarDirection.Balance:
                                this._onAnalyzerUpdated(LegAnalyzerUpdateEventEnum.LastBarExpanded);
                                break;
                            case BarDirection.Up:
                                var nb = this.LastBar;
                                this.LastLeg.BarList.Remove(this.LastBar);
                                this.addBar(nb);
                                break;
                            case BarDirection.OutsideUp:
                                var lb = this.LastBar;
                                this.LastLeg.BarList.Remove(this.LastBar);
                                if(lb.Low > this.LastLeg.Low)
                                    this.addReferenceForLowOfThisBar(this.LastLeg.LowestBar);

                                this.LegList.Add(new Leg(lb));
                                this.addReferenceForLowOfThisBar(lb);
                                this._onAnalyzerUpdated(LegAnalyzerUpdateEventEnum.NewLegAdded);
                                break;
                        }
                        break;
                }
            }
        }

        private bool lastBarShouldBeUpdated(double open, double newBarHigh, double newBarLow, double newBarClose)
        {
            if(newBarHigh > this.LastBar.High || newBarLow < this.LastBar.Low)
                return true;

            if(this.LastBar.Direction == BarDirection.OutsideUp && newBarClose < this.LastBar.Open)
                return true;

            if(this.LastBar.Direction == BarDirection.OutsideDown && newBarClose >= this.LastBar.Open)
                return true;

            return false;
        }

        public Leg LegsBack(int legsBack)
        {
            if(legsBack <= LegsCount) return null;
            return LegList[LegsCount - 1 - legsBack];
        }

        #region Reference Management
        //private void analyzeReferences()
        //{
        //    var refListCurrentLegCrossing =
        //        this.RefList.FindAll(r => r.Price > this.LastLeg.Low && r.Price < this.LastLeg.High
        //        && r.DateTime != this.LastLeg.StartDateTime);

        //    if(refListCurrentLegCrossing.Count > 0)
        //    {

        //        foreach(var refCurrentLegCrossing in refListCurrentLegCrossing)
        //        {
        //            var legListCrossingCurrentRef = this.LegList.FindAll(l =>
        //            l.StartDateTime > refCurrentLegCrossing.DateTime &&
        //            l.Low < refCurrentLegCrossing.Price &&
        //            l.High > refCurrentLegCrossing.Price);


        //            refCurrentLegCrossing.HitCount = legListCrossingCurrentRef.Count;
        //        }

        //        bool isVPattern = false;
        //        if(this.LegsCount > 1 && this.LastLeg is UpLeg && this.LastLeg.High > this.LegList[this.LegsCount - 2].High)
        //            isVPattern = true;
        //        if(this.LegsCount > 1 && this.LastLeg is DownLeg && this.LastLeg.Low < this.LegList[this.LegsCount - 2].Low)
        //            isVPattern = true;


        //        if(isVPattern)
        //        {
        //            foreach(var r in refListCurrentLegCrossing)
        //            {
        //                if(r.HitCount > 1)
        //                {
        //                    r.Status = ReferenceStatus.Off;
        //                    //this.RefList.Remove(r);
        //                }
        //            }
        //        }
        //    }
        //}
        #endregion

        private void analyzeprevRefs()
        {
            if(this.LegsCount < 2)
                return;

            if(this.LastLeg.Direction == LegDirection.Up && this.LastLeg.High > this.LegList[LegsCount - 2].High ||
                    this.LastLeg.Direction == LegDirection.Down && LastLeg.Low < LegList[LegsCount - 2].Low)
            {

                RefList.RemoveAll(r => r.Price < LastLeg.High && r.Price > LastLeg.Low);

            }
        }
    }
}
