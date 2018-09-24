using System;
using System.Collections.Generic;
using Trading.Common;

namespace Trading.Analyzers.LegAnalyzer
{
    public enum LastbarUpdateEventEnum
    {
        NoPriceChange,
        CloseUpdated,
        Expanded,
        TypeChanged
    }

    public partial class LegAnalyzer
    {
        public event EventHandler<AnalyzerPopulatedEventArgs> AnalyzerPopulated;
        private void _onAnalyzerPopulated(object sender, AnalyzerPopulatedEventArgs eventArgs)
        {
            AnalyzerPopulated?.Invoke(this, eventArgs);
        }

        public event EventHandler<NewBarAddedEventArgs> NewBarAdded;
        private void _onNewBarAdded(object sender, NewBarAddedEventArgs eventArgs)
        {
            NewBarAdded?.Invoke(this, eventArgs);
        }

        public event EventHandler<LastBarUpdatedEventArgs> LastBarUpdated;
        private void _onLastBarUpdated(object sender, LastBarUpdatedEventArgs eventArgs)
        {
            LastBarUpdated?.Invoke(this, eventArgs);
        }
    }

    public class AnalyzerPopulatedEventArgs : EventArgs
    {
        public IEnumerable<Leg> LegList { get; set; }
    }
    public class NewBarAddedEventArgs : EventArgs
    {
        public Leg LastLeg { get; set; }
    }
    public class LastBarUpdatedEventArgs : EventArgs
    {
        public Bar LastBar { get; set; }
        public LastbarUpdateEventEnum UpdateEnum { get; set; }
    }
}
