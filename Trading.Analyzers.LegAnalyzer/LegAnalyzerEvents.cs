using System;
using Trading.Analyzers.Common;

namespace Trading.Analyzers.LegAnalyzer
{
    public enum LegAnalyzerUpdateEventEnum
    {
        CloseUpdated,
        LastBarExpanded,
        NewLegAdded,
        NewBarAdded,
    }


    public partial class LegAnalyzer
    {
        //public event EventHandler<NewLegAddedEventArgs> NewLegAdded; 
        //private void _onNewLegAddedEvent(Leg newLeg)
        //{
        //    NewLegAdded?.Invoke(this, new NewLegAddedEventArgs { NewLeg = this.LastLeg });
            
        //}

        //public event EventHandler<NewBarAddedEventArgs> NewBarAdded;
        //private void _onNewBarAddedEvent(Bar newBar)
        //{
        //    NewBarAdded?.Invoke(this, new NewBarAddedEventArgs { NewBar = this.LastBar });
        //}

        public event EventHandler<LegAnalyzerUpdatedEventArgs> AnalyzerUpdated;
        private void _onAnalyzerUpdated(LegAnalyzerUpdateEventEnum eventEnum)
        {
            AnalyzerUpdated?.Invoke(this, new LegAnalyzerUpdatedEventArgs { Leg = this.LastLeg, EventEnum = eventEnum });
        }
    }

    //public class NewLegAddedEventArgs : EventArgs
    //{
    //    public Leg NewLeg { get; set; }
    //}

    //public class NewBarAddedEventArgs : EventArgs
    //{
    //    public Bar NewBar { get; set; }
    //}

    public class LegAnalyzerUpdatedEventArgs : EventArgs
    {
        public Leg Leg { get; set; }

        public LegAnalyzerUpdateEventEnum EventEnum { get; set; }
    }



}
