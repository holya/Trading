using System;
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
            return new Bar(50, 100, 0, 60, 10, new DateTime(2018, 01, 01, 00, 00, 00));
        }
        public static Bar GetDownBar()
        {
            return new Bar(50, 100, 0, 40, 10, new DateTime(2018, 01, 01, 00, 00, 00));
        }


        public static Bar GetUpBar(Bar previousBar, DateTime dateTime)
        {
            return new Bar(previousBar.Open + 10, previousBar.High + 10,
                            previousBar.Low + 10, previousBar.Close + 10, 
                            previousBar.Volume + 10, dateTime, previousBar);
        }

        public static Bar GetDownBar(Bar previousBar, DateTime dateTime)
        {
            return new Bar(previousBar.Open - 10, previousBar.High - 10,
                            previousBar.Low - 10, previousBar.Close - 10,
                            previousBar.Volume + 10, dateTime, previousBar);
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
