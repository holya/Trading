using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Trading.Common;
using Trading.Analyzers.Common;

namespace Trading.Analyzers.PatternAnalyzer
{
    public partial class PatternAnalyzer
    {        
        #region Properties

        public List<Pattern> PatternList { get; } = new List<Pattern>();
        public Bar LastBar => LastPattern.LastBar;
        public Pattern LastPattern => PatternList.Last();
        public int LegsCount => PatternList.Count;
        public double LastPrice => LastPattern.LastBar.Close;
        public List<Reference> RefList { get; } = new List<Reference>();
        protected Reference LastSupport => RefList.Last(r => r.Price <= LastPrice);
        protected Reference LastResistance => RefList.Last(r => r.Price >= LastPrice);

        #endregion
        public PatternAnalyzer() { addBar = addFirstBar; }


        public void AddBarList(IEnumerable<Bar> barList)
        {
            for (int i = 0; i < barList.Count(); i++)
            {
                addBar(barList.ElementAt(i));
            }
        }

        private Action<Bar> addBar;
        private void addFirstBar(Bar newBar)
        {
            var bar0 = new Bar(newBar.Open, newBar.Open, newBar.Open, newBar.Open, newBar.Volume, newBar.DateTime, newBar.EndDateTime);
            var leg0 = new Leg(bar0);

            newBar.PreviousBar = bar0;
            var leg = new Leg(newBar, leg0);

            var pattern = new Pattern(leg);
            var patternDirection = pattern.Direction = leg.Direction == LegDirection.Up ? PatternDirection.Up : PatternDirection.Down;
            pattern.State = PatternState.Continuation1;
            PatternList.Add(pattern);

            if (newBar.Direction < BarDirection.Balance)
                this.createReferenceForHighOfThisBar(newBar);
            else
                this.createReferenceForLowOfThisBar(newBar);

            addBar = addBarContinued;
        }

        private void addBarContinued(Bar newBar)
        {
            newBar.PreviousBar = LastBar;



            if (LastPattern.Direction == PatternDirection.Up)
            {
                if(newBar.Low < LastSupport.Price)
                {
                    // Create a new down pattern, setting up all its props
                    PatternList.Add(new Pattern(newBar));
                    this.createReferenceForLowOfThisBar(newBar);
                    return;
                }

                switch (LastPattern.State)
                {
                    case PatternState.Continuation1:
                        if (newBar.Direction > BarDirection.Balance)
                        {
                            //if (newBar.High > LastResistance.Price)
                            //{
                            //    var changedPattern = new Pattern(newBar);
                            //    PatternList.Add(changedPattern);
                            //    changedPattern.State = PatternState.Continuation2;
                            //    return;
                            //}

                            LastPattern.LastLeg.AddBar(newBar);
                            return;
                        }

                        if (newBar.Direction < BarDirection.Balance)
                        {
                            var changedPattern = new Pattern(newBar);
                            PatternList.Add(changedPattern);
                            changedPattern.State = PatternState.PullBack1;
                            this.createReferenceForHighOfThisBar(LastBar);
                            return;
                        }

                        if (newBar.Direction == BarDirection.Balance)
                            LastPattern.LastLeg.AddBar(newBar);
                        break;

                    case PatternState.Continuation2:
                        break;
                    case PatternState.PullBack1:
                        if (LastPattern.LastLeg.PreviousLeg.Direction == LegDirection.Up)
                        {
                            if (newBar.Direction > BarDirection.Balance)
                            {
                                if (newBar.High < LastResistance.Price)
                                {
                                    var changedPattern = new Pattern(newBar);
                                    PatternList.Add(changedPattern);
                                    changedPattern.State = PatternState.Continuation1;
                                    this.createReferenceForLowOfThisBar(LastBar);
                                    return;
                                }
                                else
                                {
                                    var changedPattern = new Pattern(newBar);
                                    PatternList.Add(changedPattern);
                                    changedPattern.State = PatternState.Continuation2;
                                    this.createReferenceForLowOfThisBar(LastBar);
                                    return;
                                }
                            }

                            if (newBar.Direction <= BarDirection.Balance)
                            {
                                LastPattern.LastLeg.AddBar(newBar);
                                return;
                            }

                        }
                        else
                        {

                        }

                        break;
                    case PatternState.PullBack2:
                        break;
                    case PatternState.HeadAndShoulder:
                        break;
                    default:
                        break;
                }

            }
            else
            {

            }
        }

        private void createReferenceForLowOfThisBar(Bar bar)
        {
            createReference(bar.Low, bar.DateTime, bar, ReferenceType.Support);
        }
        private void createReferenceForHighOfThisBar(Bar bar)
        {
            createReference(bar.High, bar.DateTime, bar, ReferenceType.Resistance);
        }

        private void createReference(double price, DateTime dateTime, Bar owner, ReferenceType type)
        {
            RefList.Add(new Reference { Price = price, DateTime = dateTime, Owner = owner, Type = type });
        }

        public void AddBar(Bar newBar)
        {
            addBar(newBar);
        }

        public void UpdateLastBar(Bar updatedBar)
        {
        }

    }
}
