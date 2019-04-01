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
        public Bar LastBar => LastPattern.LastBar;
        public Pattern LastPattern => PatternList.Last();
        public List<Pattern> PatternList { get; } = new List<Pattern>();
        public int LegsCount => PatternList.Count;
        private State PatternState;

        public List<Reference> RefList { get; } = new List<Reference>();
        #endregion
        public PatternAnalyzer() { addBar = addFirstBar; }

        public void AddBarList(IEnumerable<Bar> barList)
        {
            for (int i = 0; i < barList.Count(); i++)
            {
                addBar(barList.ElementAt(i));
            }
            //_onAnalyzerPopulated(this, new AnalyzerPopulatedEventArgs { LegList = this.LegList });
        }

        private Action<Bar> addBar;
        private void addFirstBar(Bar newBar)
        {
            var d = newBar;
            d.PreviousBar = new Bar(d.Open, d.Open, d.Open, d.Open, d.Volume, d.DateTime, d.EndDateTime);
            PatternList.Add(new Pattern(new Leg(d)));

            addBar = addBarContinued;
        }
        private void addBarContinued(Bar newBar)
        {
            newBar.PreviousBar = LastBar;

            if (LastPattern.Direction == PatternDirection.Up)
            {
                createReferenceForLowOfThisBar(LastPattern.LastLeg.FirstBar);
                //First leg of pattern is up
                if (LastPattern.LastLeg.Direction == LegDirection.Up)
                {
                    if (newBar.Direction >= BarDirection.Balance)
                    {
                        LastPattern.LastLeg.AddBar(newBar);
                        createReferenceForHighOfThisBar(newBar);
                        PatternState = State.Continuation1;
                    }
                    
                    if (newBar.Direction <= BarDirection.Balance)
                    {
                        PatternList.Add(new Pattern(newBar));
                        createReferenceForHighOfThisBar(newBar.PreviousBar);
                        PatternState = newBar.Low > LastPattern.LastLeg.PreviousLeg.FirstBar.Low ?
                            State.PullBack1 : State.PullBack2;
                    }
                }
                else
                {

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
