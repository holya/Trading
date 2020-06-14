using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Trading.Common;

namespace Trading.Common
{
    public struct Reference
    {
        public double Price;
        public DateTime DateTime;
        public short HitCount;
        public ReferenceType Type;
        public Bar Owner;
        public bool Relevance;
    }

    //public class Reference
    //{
    //    public double Price;
    //    public DateTime DateTime;
    //    public TimeFrame Timeframe; //The references will be timeframe specific and color coded for easier comprehension
    //    public ReferenceType Type;
    //    public Bar Owner;
    //    public bool Relevance;

    //    public Reference(Bar owner)
    //    {
    //        Owner = owner;
    //        addReferenceBar(owner);
    //    }

    //    List<Bar> referenceLocations = new List<Bar>();
    //    Action<Bar> addReference;

    //    public void addReferenceBar(Bar owner)
    //    {
    //        if (owner.PreviousBar.Direction != owner.Direction)
    //            referenceLocations.Add(owner);
    //    }

    //    public bool isRefRelevant(Bar owner) { return Relevance; }

    //    public void referenceListDepopulate()
    //    {
    //        if (isRefRelevant(Owner))
    //            referenceLocations.Remove(Owner);
    //    }
    //}
}
