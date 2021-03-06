﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trading.Common;

namespace Tests
{
    public static class Helper
    {
        public static Bar GetUpBar()
        {
            return new FxBar(50, 51, 100, 101, 10, 11, 60, 61, 10, DateTime.Now, DateTime.Now);
        }
        public static Bar GetDownBar()
        {
            return new FxBar(50, 51, 100, 101, 10, 11, 40, 41, 10, DateTime.Now, DateTime.Now);
        }


        public static Bar GetUpBar(Bar previousBar, DateTime dateTime)
        {
            var pb = (FxBar)previousBar;
            return new FxBar(pb.Open + 10, pb.AskOpen +10, pb.High + 10, pb.AskHigh +10,
                            pb.Low + 10, pb.AskLow + 10, pb.Close + 10, pb.AskClose +10,
                            100, dateTime, dateTime);
        }

        public static Bar GetOutsideUpBar(Bar previousBar, DateTime dateTime)
        {
            var pb = (FxBar)previousBar;
            return new FxBar(pb.Open + 20, pb.AskOpen + 20, pb.High + 20, pb.AskHigh + 20,
                            pb.Low - 10, pb.AskLow - 10, pb.Close + 20, pb.AskClose + 20,
                            100, dateTime, dateTime);
        }

        public static Bar GetDownBar(Bar previousBar, DateTime dateTime)
        {
            var pb = (FxBar)previousBar;
            return new FxBar(pb.Open - 10, pb.AskOpen-10, pb.High - 10, pb.AskHigh-10,
                            pb.Low - 10, pb.AskLow-10, pb.Close - 10, pb.AskClose-10,
                            100, dateTime, dateTime);
        }

        public static Bar GetBalanceBar(Bar previousBar, DateTime dateTime)
        {
            var pb = (FxBar)previousBar;
            return new FxBar(pb.Open, pb.AskOpen, pb.High - 10, pb.AskHigh - 10,
                            pb.Low + 10, pb.AskLow + 10, pb.Close, pb.AskClose,
                            100, dateTime, dateTime);
        }


        public static Leg GetUpLeg(int numberOfBars, Leg previousLeg = null)
        {
            FxBar[] barlist = new FxBar[numberOfBars + 1];
            FxBar upBar = new FxBar(30, 31, 60, 61, 10, 11, 40, 41, 0, DateTime.Now, DateTime.Now);
            barlist[0] = upBar;
            Leg leg = new Leg(upBar, previousLeg);
            for (int i = 1; i < numberOfBars; i++)
            {
                barlist[i] = (new FxBar
                {
                    Open = upBar.Open + (i + 10),
                    AskOpen = upBar.AskOpen + (i + 10),
                    High = upBar.High + (i + 10),
                    AskHigh = upBar.AskHigh + (i + 10),
                    Low = upBar.Low + (i + 10),
                    AskLow = upBar.AskLow + (i + 10),
                    Close = upBar.Close + (i + 10),
                    AskClose = upBar.AskClose + (i + 10),
                    PreviousBar = barlist[i - 1]
                });

                leg.AddBar(barlist[i]);
            }

            return leg;
        }

        public static Leg GetDownLeg(int numberOfBars, Leg previousLeg = null)
        {
            FxBar[] barlist = new FxBar[numberOfBars + 1];
            FxBar downBar = new FxBar(60, 61, 120, 121, 40, 41, 50, 51, 0, DateTime.Now, DateTime.Now);
            barlist[0] = downBar;
            Leg leg = new Leg(downBar, previousLeg);
            for (int i = 1; i < numberOfBars; i++)
            {
                barlist[i] = (new FxBar
                {
                    Open = downBar.Open - (i + 10),
                    AskOpen = downBar.AskOpen - (i + 10),
                    High = downBar.High - (i + 10),
                    AskHigh = downBar.AskHigh - (i + 10),
                    Low = downBar.Low - (i + 10),
                    AskLow = downBar.AskLow - (i + 10),
                    Close = downBar.Close - (i + 10),
                    AskClose = downBar.AskClose - (i + 10),
                    PreviousBar = barlist[i - 1]
                });

                leg.AddBar(barlist[i]);
            }

            return leg;
        }

    }
}
