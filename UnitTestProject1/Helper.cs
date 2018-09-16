﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trading.Common;

namespace UnitTestProject1
{
    public static class Helper
    {
        public static Bar GetUpBar()
        {
            return new FxBar(50, 51, 100, 101, 0, 1, 60, 61, 100, new DateTime(2018, 01, 01, 00, 00, 00));
        }
        public static Bar GetDownBar()
        {
            return new FxBar(50, 51, 100, 101, 0, 1, 40, 41, 100, new DateTime(2018, 01, 01, 00, 00, 00));
        }


        public static Bar GetUpBar(Bar previousBar, DateTime dateTime)
        {
            var pb = (FxBar)previousBar;
            return new FxBar(pb.Open + 10, pb.AskOpen +10, pb.High + 10, pb.AskHigh +10,
                            pb.Low + 10, pb.AskLow + 10, pb.Close + 10, pb.AskClose +10,
                            100, dateTime);
        }

        public static Bar GetDownBar(Bar previousBar, DateTime dateTime)
        {
            var pb = (FxBar)previousBar;
            return new FxBar(pb.Open - 10, pb.AskOpen-10, pb.High - 10, pb.AskHigh-10,
                            pb.Low - 10, pb.AskLow-10, pb.Close - 10, pb.AskClose-10,
                            100, dateTime);
        }

        //public static Leg GetUpLeg(int numberOfBars)
        //{
        //    Leg leg = new Leg(GetUpBar());
        //    for(int i = 0; i < numberOfBars; i++)
        //    {

        //    }

        //}
    }
}
