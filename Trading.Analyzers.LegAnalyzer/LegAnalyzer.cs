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
        public Bar LastBar => LastLeg.BarCount != 0 ?
                    LastLeg.LastBar : LegList[LegList.Count - 2].LastBar;
        public Leg LastLeg => LegList.Last();
        public double Close => LastBar.Close;
        //public double BarVolitility { get { return this.LastBar.High - this.LastBar.Low / 100; } }
        //public double LastSupport { get { return this.RefList.FirstOrDefault(r => r.Price < this.Close).Price; } }
        //public double LastResistance { get { return this.RefList.FirstOrDefault(r => r.Price < this.Close).Price; } }
        public List<Leg> LegList { get; } = new List<Leg>();
        public int LegsCount => LegList.Count;
        public int BarsCount => LegList.Sum(leg => leg.BarCount);
        //public List<double> AdvanceList { get; private set; }
        //public List<double> DeclineList { get; private set; }

        public List<Reference> RefList { get; } = new List<Reference>();
        #endregion

        public LegAnalyzer(Resolution resolution)
        {
            addBar = addFirstBar;
            Resolution = resolution;
        }

        public void AddBarList(IEnumerable<Bar> barList)
        {
            if (barList.Count() == 0)
                return;

            for (int i = 0; i < barList.Count(); i++)
            {
                AddBar(barList.ElementAt(i));
            }
        }

        private Action<Bar> addBar;
        private void addFirstBar(Bar newBar)
        {
            var d = newBar;
            d.PreviousBar = new Bar(d.Open, d.Open, d.Open, d.Open, d.Volume, d.DateTime);
            LegList.Add(new Leg(d));
            addBar = addBarContiued;
        }
        private void addBarContiued(Bar newBar)
        {
            newBar.PreviousBar = LastBar;
            if(LastBar.IsSameDirection(newBar))
            {
                LegList.Last().AddBar(newBar);
                return;
            }
            LegList.Add(new Leg(newBar) { PreviousLeg = LegList.Last() });

        }
        public void AddBar(Bar newBar) => addBar(newBar);

        public void UpdateLastBar(Bar updatedBar)
        {
            //if(updatedBar.DateTime <= getEndTimeframeDateTime(LastBar.DateTime))
            //{
            var oldDir = LastBar.Direction;
            LastBar.Update(updatedBar);
            if(LastBar.Direction != oldDir)
            {
                var savedLastBar = LastBar;
                LastLeg.BarList.Remove(LastBar);
                addBar(savedLastBar);
            }
            //if (!LastBar.PreviousBar.IsSameDirection(LastBar))
            //{
            //    LegList.Add(new Leg(LastBar) { PreviousLeg = LegList.Last() });
            //    LegList[LegList.Count - 2].BarList.Remove(LastBar);
            //}
            //    return;
            //}

            //updatedBar.DateTime = getEndTimeframeDateTime(LastBar.DateTime);

            //AddBar(updatedBar);
        }
        //private DateTime getEndTimeframeDateTime(DateTime dt)
        //{
        //    var tf = Resolution.TimeFrame;
        //    var size = Resolution.Size;
        //    if(tf == TimeFrame.Yearly)
        //        return LastBar.DateTime.AddYears(Resolution.Size);
        //    if(tf == TimeFrame.Quarterly)
        //           LastBar.DateTime.AddMonths(Resolution.Size * 4);
        //    if(tf == TimeFrame.Monthly)
        //        return LastBar.DateTime.AddMonths(Resolution.Size);
        //    if(tf == TimeFrame.Weekly)
        //        return LastBar.DateTime.AddDays(Resolution.Size * 7);
        //    if(tf == TimeFrame.Daily)
        //        return LastBar.DateTime.AddDays(Resolution.Size);
        //    if(tf == TimeFrame.Hourly)
        //        return LastBar.DateTime.AddHours(Resolution.Size);

        //    return LastBar.DateTime.AddMinutes(Resolution.Size);
        //}


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
