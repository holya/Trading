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
        #region Properties

        public Resolution Resolution { get; set; }
        public Bar LastBar { get { return this.LastLeg.LastBar; } } 
        public Leg LastLeg { get { return this.LegList.Last(); } }
        public double Close { get { return this.LastBar.Close; } }

        //public double BarVolitility { get { return this.LastBar.High - this.LastBar.Low / 100; } }
        //public double LastSupport { get { return this.RefList.FirstOrDefault(r => r.Price < this.Close).Price; } }
        //public double LastResistance { get { return this.RefList.FirstOrDefault(r => r.Price < this.Close).Price; } }
        public List<Leg> LegList { get; } = new List<Leg>();
        public int LegsCount { get { return this.LegList.Count; } }
        

        //public List<double> AdvanceList { get; private set; }
        //public List<double> DeclineList { get; private set; }

        public List<Reference> RefList { get; } = new List<Reference>();
        

        #endregion

        public LegAnalyzer(Resolution resolution)
        {
            Resolution = resolution;
        }
        public int BarsCount { get { return LegList.Sum(leg => leg.BarCount); } }

        public void AddBarList(IEnumerable<Bar> barList)
        {
            if (barList.Count() == 0)
                return;

            if (LegList.Count == 0)
            {
                var d = barList.ElementAt(0);
                d.PreviousBar = new Bar(d.Open, d.Open, d.Open, d.Open, d.Volume, d.DateTime);
                LegList.Add(new Leg(d));
            }

            for (int i = 1; i < barList.Count(); i++)
            {
                AddBar(barList.ElementAt(i));
            }
        }

        public void AddBar(Bar newBar)
        {
            newBar.PreviousBar = LastBar;
            if(LastBar.IsSameDirection(newBar))
            {
                LegList.Last().AddBar(newBar);
                return;
            }
            LegList.Add(new Leg(newBar) { PreviousLeg = LegList.Last() });
        }

        public void UpdateLastBar(Bar updatedBar)
        {
            if(updatedBar.DateTime <= LastBar.DateTime)
            {
                LastBar.Update(updatedBar);
                if (!LastBar.PreviousBar.IsSameDirection(LastBar))
                {
                    LegList.Add(new Leg(LastBar) { PreviousLeg = LegList.Last() });
                    LegList[LegList.Count - 1].BarList.Remove(LastBar);
                }
                return;
            }

            switch (Resolution.TimeFrame)
            {
                case TimeFrame.Yearly:
                    updatedBar.DateTime = LastBar.DateTime.AddYears(Resolution.Size);
                    break;
                case TimeFrame.Quarterly:
                    updatedBar.DateTime = LastBar.DateTime.AddMonths(Resolution.Size * 4);
                    break;
                case TimeFrame.Monthly:
                    updatedBar.DateTime = LastBar.DateTime.AddMonths(Resolution.Size);
                    break;
                case TimeFrame.Weekly:
                    updatedBar.DateTime = LastBar.DateTime.AddDays(Resolution.Size * 7);
                    break;
                case TimeFrame.Daily:
                    updatedBar.DateTime = LastBar.DateTime.AddDays(Resolution.Size);
                    break;
                case TimeFrame.Hourly:
                    updatedBar.DateTime = LastBar.DateTime.AddDays(Resolution.Size);
                    break;
                case TimeFrame.Minute:
                    updatedBar.DateTime = LastBar.DateTime.AddDays(Resolution.Size);
                    break;
                default:
                    break;
            }

            AddBar(updatedBar);
        }

        public void UpdateLastBar2(Bar updatedBar)
        {
            if(updatedBar.DateTime > LastBar.DateTime)
            {
                switch (Resolution.TimeFrame)
                {
                    case TimeFrame.Yearly:
                        updatedBar.DateTime = LastBar.DateTime.AddYears(Resolution.Size);
                        break;
                    case TimeFrame.Quarterly:
                        updatedBar.DateTime = LastBar.DateTime.AddMonths(Resolution.Size * 4);
                        break;
                    case TimeFrame.Monthly:
                        updatedBar.DateTime = LastBar.DateTime.AddMonths(Resolution.Size);
                        break;
                    case TimeFrame.Weekly:
                        updatedBar.DateTime = LastBar.DateTime.AddDays(Resolution.Size * 7);
                        break;
                    case TimeFrame.Daily:
                        updatedBar.DateTime = LastBar.DateTime.AddDays(Resolution.Size);
                        break;
                    case TimeFrame.Hourly:
                        updatedBar.DateTime = LastBar.DateTime.AddDays(Resolution.Size);
                        break;
                    case TimeFrame.Minute:
                        updatedBar.DateTime = LastBar.DateTime.AddDays(Resolution.Size);
                        break;
                    default:
                        break;
                }
                if (LastBar.IsSameDirection(updatedBar))
                {
                    LastLeg.AddBar(updatedBar);
                    return;
                }

                LegList.Add(new Leg(updatedBar) { PreviousLeg = LegList.Last() });
                return;
            }

            LastBar.Update(updatedBar);

            if (!updatedBar.IsSameDirection(LastBar))
            {
                LegList.Add(new Leg(LastBar) { PreviousLeg = LegList.Last() });
                LegList[LegList.Count - 1].BarList.Remove(LastBar);
            }
        }


        private void addReferenceForHighOfThisBar(Bar bar)
        {
            this.RefList.Add(new Reference { Price = bar.High, DateTime = bar.DateTime, Owner = bar });
        }

        private void addReferenceForLowOfThisBar(Bar bar)
        {
            this.RefList.Add(new Reference { Price = bar.Low, DateTime = bar.DateTime, Owner = bar });
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
