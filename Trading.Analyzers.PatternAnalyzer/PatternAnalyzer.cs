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

            PatternList.Add(pattern); // Adding the first bar from the patterns point of view will essentially mean "create a new pattern"

            if (newBar.Direction < BarDirection.Balance) 
                this.createReferenceForHighOfThisBar(newBar);
            else
                this.createReferenceForLowOfThisBar(newBar);
            // conclusion from testing: Reference should be made from first bar to detect pattern change

            addBar = addBarContinued;
        }

        private void addBarContinued(Bar newBar)
        {
            newBar.PreviousBar = LastBar;

            if (LastPattern.Direction == PatternDirection.Up)
            {
                switch (LastPattern.State)
                {
                    case PatternState.Continuation:
                        switch (newBar.Direction)
                        {
                            case BarDirection.Down:
                            case BarDirection.GapDown:
                                if (newBar.Low < LastSupport.Price)
                                {
                                    //is reference voided?
                                    Pattern newPattern = new Pattern(newBar); //new Pattern(newBar, this.LastPattern)
                                    PatternList.Add(newPattern);
                                }
                                else
                                {
                                    LastPattern.LastLeg.AddBar(newBar);
                                    LastPattern.State = PatternState.PullBack;
                                    createReferenceForHighOfThisBar(LastBar);
                                }
                                break;

                            case BarDirection.OutsideDown:
                                if (newBar.Low < LastSupport.Price)
                                {
                                    Pattern newPattern = new Pattern(newBar);
                                    PatternList.Add(newPattern);
                                }
                                else
                                {
                                    LastPattern.LastLeg.AddBar(newBar);
                                    LastPattern.State = PatternState.PullBack;
                                    createReferenceForHighOfThisBar(newBar);
                                    if (newBar.High > LastResistance.Price)
                                        createReferenceForHighOfThisBar(newBar); // major or stronger reference
                                }
                                break;

                            case BarDirection.Balance:
                                LastPattern.LastLeg.AddBar(newBar);
                                break;

                            case BarDirection.Up:
                            case BarDirection.GapUp:
                                LastPattern.LastLeg.AddBar(newBar);
                                break;
                            case BarDirection.OutsideUp:
                                if (newBar.Low < LastSupport.Price)
                                {
                                    Pattern newPattern = new Pattern(newBar);
                                    PatternList.Add(newPattern);
                                    newPattern.State = PatternState.Continuation;
                                }
                                break;
                            default:
                                break;
                        }
                        break;

                    case PatternState.PullBack:
                        switch (newBar.Direction)
                        {
                            case BarDirection.Down:
                            case BarDirection.GapDown:
                                if (newBar.Low < LastSupport.Price)
                                {
                                    Pattern newPattern = new Pattern(newBar);
                                    PatternList.Add(newPattern);
                                }
                                else
                                    LastPattern.LastLeg.AddBar(newBar);
                                break;

                            case BarDirection.OutsideDown:
                                if (newBar.Low < LastSupport.Price)
                                {
                                    Pattern newPattern = new Pattern(newBar);
                                    PatternList.Add(newPattern);
                                }
                                else
                                {
                                    if (newBar.High > LastResistance.Price)
                                        createReferenceForHighOfThisBar(newBar);
                                    LastPattern.LastLeg.AddBar(newBar);
                                }
                                break;

                            case BarDirection.Balance:
                                LastPattern.LastLeg.AddBar(newBar);
                                break;

                            case BarDirection.Up:
                            case BarDirection.GapUp:
                                LastPattern.LastLeg.AddBar(newBar);
                                createReferenceForLowOfThisBar(LastBar);
                                LastPattern.State = PatternState.Continuation;
                                break;

                            case BarDirection.OutsideUp:
                                if (newBar.Low < LastSupport.Price)
                                {
                                    Pattern newPattern = new Pattern(newBar);
                                    PatternList.Add(newPattern);
                                }
                                else
                                {
                                    if (newBar.High > LastResistance.Price)
                                        createReferenceForHighOfThisBar(newBar);
                                    LastPattern.LastLeg.AddBar(newBar);
                                    createReferenceForLowOfThisBar(newBar);
                                }
                                break;
                            default:
                                break;
                        }
                        break;

                    case PatternState.SideWays:
                        switch (newBar.Direction)
                        {
                            case BarDirection.Down:
                                break;
                            case BarDirection.GapDown:
                                break;
                            case BarDirection.OutsideDown:
                                break;
                            case BarDirection.Balance:
                                break;
                            case BarDirection.Up:
                                break;
                            case BarDirection.GapUp:
                                break;
                            case BarDirection.OutsideUp:
                                break;
                            default:
                                break;
                        }
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


        public void UpdateLastBar(Bar updatedBar)
        {
        }

    }
}
