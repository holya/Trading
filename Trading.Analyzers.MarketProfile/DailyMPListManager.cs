using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Trading.Common;

namespace Trading.Analyzers
{
    public class DailyMPListManager
    {
        //private DateTime lastBarDateTime;
        private Action<Bar> addBar;

        public MPAnalyzer LastMP { get { return this.MPList.Last(); } }
        public List<MPAnalyzer> MPList { get; private set; }

        public DailyMPListManager()
        {
            this.MPList = new List<MPAnalyzer>();
            this.addBar = this.addFirstBar;
        }

        public void AddBar(Bar bar)
        {
            this.addBar(bar);

            //this.lastBarDateTime = bar.DateTime;
        }

        private void addFirstBar(Bar bar)
        {
            this.addNewMp(bar);

            this.addBar = this.continueAddingBar;
        }

        private void continueAddingBar(Bar bar)
        {
            if(this.LastMP.EndDateTime.Day == bar.DateTime.Day)
                this.LastMP.AddBar(bar);
            else
            {
                this.addNewMp(bar);
            }
        }

        public void UpdateBar(Bar bar)
        {
            this.LastMP.UpdatBar(bar);
        }

        private void addNewMp(Bar bar)
        {
            var newMP = new MPAnalyzer();
            newMP.AddBar(bar);
            this.MPList.Add(newMP);
        }

    }
}
