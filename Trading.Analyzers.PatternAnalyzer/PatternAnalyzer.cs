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
        public PatternAnalyzer() => addBar = addFirstBar;

        public void AddBar(Bar newBar) => addBar(newBar);

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
            newBar.PreviousBar = bar0;

            var leg0 = new Leg(bar0);
            var leg = new Leg(newBar, leg0);

            var pattern0 = new Pattern(bar0);
            var pattern = new Pattern(newBar, pattern0);

            //if (newBar.Direction < BarDirection.Balance)
            //    this.createReferenceForHighOfThisBar(newBar);
            //else
            //    this.createReferenceForLowOfThisBar(newBar);

            addBar = addBarContinued;
        }

        private void addBarContinued(Bar newBar)
        {
            newBar.PreviousBar = LastBar;

            if (this.LastPattern.Direction == PatternDirection.Up)
            {
                switch (this.LastPattern.State)
                {
                    case PatternState.Continuation1:
                        break;
                    case PatternState.Continuation2:
                        break;
                    case PatternState.PullBack1:
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
                switch (this.LastPattern.State)
                {
                    case PatternState.Continuation1:
                        break;
                    case PatternState.Continuation2:
                        break;
                    case PatternState.PullBack1:
                        break;
                    case PatternState.PullBack2:
                        break;
                    case PatternState.HeadAndShoulder:
                        break;
                    default:
                        break;
                }
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


        public void UpdateLastBar(Bar updatedBar)
        {
        }

    }
}
