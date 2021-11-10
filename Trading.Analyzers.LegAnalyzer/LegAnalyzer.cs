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
        public Bar LastBar => LastLeg.LastBar;
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

        //public List<Pattern> PatternsList { get; } = new List<Pattern>();
        
        private Action<Bar> addBarDelegate;
        private Action<double, int> updateLastBarDelegate = delegate { };
        public List<Reference> RefList { get; } = new List<Reference>();
        #endregion

        public LegAnalyzer()
        {
            addBarDelegate = addFirstBar;
        }

        public void AddBarList(IEnumerable<Bar> barList)
        {
            for (int i = 0; i < barList.Count(); i++)
            {
                addBarDelegate(barList.ElementAt(i));
            }
        }
        public void AddBar(Bar newBar)
        {
            addBarDelegate(newBar);
        }


        private void addFirstBar(Bar newBar)
        {
            var d = newBar;
            var newLeg = new Leg(d);
            d.PreviousBar = new Bar(d.Open, d.Open, d.Open, d.Open, d.Volume, d.DateTime);
            LegList.Add(newLeg);

            addBarDelegate = addBar;
            updateLastBarDelegate = updateLastBar;
        }
        private void addBar(Bar newBar)
        {
            newBar.PreviousBar = LastBar;
            if (!LastLeg.AddBar(newBar))
            {
                LegList.Add(new Leg(newBar));
                //create new reference point of the last Leg

                //if(LastLeg.Direction == LegDirection.Up)
                //{
                //    createReferenceForLowOfThisBar(LastLeg.PreviousLeg.LowestBar);
                //    createReferenceForHighOfThisBar(newBar);
                //}
                //else
                //{
                //    createReferenceForHighOfThisBar(LastLeg.PreviousLeg.HighestBar);
                //    createReferenceForLowOfThisBar(newBar);
                //}
            }


            #region bar add 
            //if((LastLeg.Direction == LegDirection.Up && newBar.Low >= LastBar.Low) ||
            //   (LastLeg.Direction == LegDirection.Down && newBar.High <= LastBar.High))
            //{
            //    LegList.Last().AddBar(newBar);
            //}
            //else
            //    LegList.Add(new Leg(newBar) { PreviousLeg = LegList.Last() });
            #endregion

            //if(LastLeg.Direction == LegDirection.Up)
            //{
            //    if(LastLeg.PreviousLeg.Direction == LegDirection.Up)
            //    {
            //        createReferenceForHighOfThisBar(LastLeg.PreviousLeg.HighestBar);
            //        createReferenceForLowOfThisBar(LastBar);
            //    }
            //    else
            //    {
            //        if (LastBar.Low > LastLeg.PreviousLeg.LowestBar.Low)
            //            createReferenceForLowOfThisBar(LastLeg.PreviousLeg.LowestBar);
            //        if (LastBar.Direction == BarDirection.OutsideUp)
            //            createReferenceForLowOfThisBar(LastBar);
            //    }
            //}
            //else
            //{
            //    if (LastLeg.PreviousLeg.Direction == LegDirection.Down)
            //    {
            //        createReferenceForLowOfThisBar(LastLeg.PreviousLeg.LowestBar);
            //        createReferenceForHighOfThisBar(LastBar);
            //    }
            //    else
            //    {
            //        if (LastBar.High > LastLeg.PreviousLeg.HighestBar.High)
            //            createReferenceForHighOfThisBar(LastLeg.PreviousLeg.HighestBar);
            //        if (LastBar.Direction == BarDirection.OutsideDown)
            //            createReferenceForHighOfThisBar(LastBar);
            //    }
            //}
        }

        public void UpdateLastBar(double price, int volume)
        {
            updateLastBarDelegate(price, volume);
        }
        private void updateLastBar(double price, int volume)
        {
            LastBar.Update(price, volume);
        }

        private void createReferenceForLowOfThisBar(Bar bar)
        {
            createReference(bar.Low, bar.DateTime, bar);
        }
        private void createReferenceForHighOfThisBar(Bar bar)
        {
            createReference(bar.High, bar.DateTime, bar);
        }

        private void createReference(double price, DateTime dateTime, Bar owner)
        {
            RefList.Add(new Reference { Price = price, DateTime = dateTime, Owner = owner });
        }


        public void Reset()
        {
            this.LegList.Clear();
            addBarDelegate = this.addFirstBar;
            updateLastBarDelegate = delegate { };
        }

        //private void addReferenceForHighOfThisBar(Bar bar)
        //{
        //    this.RefList.Add(new Reference { Price = bar.High, DateTime = bar.DateTime, Owner = bar });
        //}

        //private void addReferenceForLowOfThisBar(Bar bar)
        //{
        //    this.RefList.Add(new Reference { Price = bar.Low, DateTime = bar.DateTime, Owner = bar });
        //}


        //private bool lastBarShouldBeUpdated(double open, double newBarHigh, double newBarLow, double newBarClose)
        //{
        //    if(newBarHigh > this.LastBar.High || newBarLow < this.LastBar.Low)
        //        return true;

        //    if(this.LastBar.Direction == BarDirection.OutsideUp && newBarClose < this.LastBar.Open)
        //        return true;

        //    if(this.LastBar.Direction == BarDirection.OutsideDown && newBarClose >= this.LastBar.Open)
        //        return true;

        //    return false;
        //}

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
