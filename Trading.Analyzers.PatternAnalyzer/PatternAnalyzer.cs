﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Trading.Common;
using Trading.Analyzers.Common;

namespace Trading.Analyzers.LegAnalyzer
{
    public partial class PatternAnalyzer
    {
        #region Properties
        public Bar LastBar => LastPattern.LastBar;
        public Pattern LastPattern => PatternList.Last();
        //public double Close => LastBar.Close;
        //public double BarVolitility { get { return this.LastBar.High - this.LastBar.Low / 100; } }
        //public double LastSupport { get { return this.RefList.FirstOrDefault(r => r.Price < this.Close).Price; } }
        //public double LastResistance { get { return this.RefList.FirstOrDefault(r => r.Price < this.Close).Price; } }
        public List<Pattern> PatternList { get; } = new List<Pattern>();
        public int LegsCount => PatternList.Count;
        //public int BarsCount => PatternList.Sum(p => p.BarCount);
        //public List<double> AdvanceList { get; private set; }
        //public List<double> DeclineList { get; private set; }

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
            if (!PatternList.Last().AddBar(newBar))
            {
                PatternList.Add(new Pattern(newBar));
            }
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
