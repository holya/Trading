using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trading.Analyzers.Common;
using Trading.Common;

namespace Trading.Analyzers.PatternAnalyzer
{
    public partial class PatternAnalyzer
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

        public event EventHandler<BarUpdateEventArgs> LastBarUpdated;
        private void _onLastBarUpdated(object sender, BarUpdateEventArgs eventArgs)
        {
            LastBarUpdated?.Invoke(this, eventArgs);
        }

    }

    public class AnalyzerPopulatedEventArgs : EventArgs
    {
        public IEnumerable<Pattern> LegList { get; set; }
    }
    public class NewBarAddedEventArgs : EventArgs
    {
        public Leg LastLeg { get; set; }
    }
    public class BarUpdateEventArgs : EventArgs
    {
        public Bar LastBar { get; set; }
        public BarUpdateStatus UpdateEnum { get; set; }
    }

}
