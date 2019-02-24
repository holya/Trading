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
        //public Resolution Resolution { get; set; }
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

        public List<Pattern> PatternsList { get; } = new List<Pattern>();
        

        public List<Reference> RefList { get; } = new List<Reference>();
        #endregion

        public LegAnalyzer()
        {
            addBar = addFirstBar;
        }

        public void AddBarList(IEnumerable<Bar> barList)
        {
            for (int i = 0; i < barList.Count(); i++)
            {
                addBar(barList.ElementAt(i));
            }
            _onAnalyzerPopulated(this, new AnalyzerPopulatedEventArgs { LegList = this.LegList });
        }

        private Action<Bar> addBar;
        private void addFirstBar(Bar newBar)
        {
            var d = newBar;
            var newLeg = new Leg(d);
            d.PreviousBar = new Bar(d.Open, d.Open, d.Open, d.Open, d.Volume, d.DateTime, d.EndDateTime);
            LegList.Add(newLeg);

            PatternsList.Add(new Pattern(newLeg));
            addBar = addBarContinued;
        }
        private void addBarContinued(Bar newBar)
        {
            newBar.PreviousBar = LastBar;
            if (!LastLeg.AddBar(newBar))
            {
                LegList.Add(new Leg(newBar));
                //create new reference point of the last Leg
            }

            if (!PatternsList.Last().AddBar(newBar))
            {
                PatternsList.Add(new Pattern(newBar));
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

        public void AddBar(Bar newBar)
        {
            addBar(newBar);
            _onNewBarAdded(this, new NewBarAddedEventArgs { LastLeg = LastLeg });
        }

        public void UpdateLastBar(Bar updatedBar)
        {
            LastbarUpdateEventEnum updateEnum;
            bool isUpdateTickWithinLastBar = updatedBar.High > LastBar.High || updatedBar.Low < LastBar.Low;
            bool isCloseSame = updatedBar.Close == LastBar.Close;
            var oldDir = LastBar.Direction;
            LastBar.Update(updatedBar);
            if (LastBar.Direction != oldDir)
            {
                var savedLastBar = LastBar;
                LastLeg.BarList.Remove(LastBar);
                if (LastLeg.BarCount == 0)
                    LegList.Remove(LastLeg);
                if (LegList.Count == 0)
                    addBar = this.addFirstBar;
                addBar(savedLastBar);
                updateEnum = LastbarUpdateEventEnum.TypeChanged;
            }
            else if (!isUpdateTickWithinLastBar)
                updateEnum = LastbarUpdateEventEnum.Expanded;
            else if (!isCloseSame)
                updateEnum = LastbarUpdateEventEnum.CloseUpdated;
            else
                updateEnum = LastbarUpdateEventEnum.NoPriceChange;

            _onLastBarUpdated(this, new LastBarUpdatedEventArgs { LastBar = LastBar, UpdateEnum = updateEnum });
        }

        public void Reset()
        {
            this.LegList.Clear();
            addBar = this.addFirstBar;
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
