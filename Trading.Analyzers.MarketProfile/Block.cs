using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Trading.Common;

namespace Trading.Analyzers
{
    public class Block : IComparable<Block>
    {
        public double High { get; set; }
        public double Low { get; set; }
        public ushort HitCount { get; set; }
        public BlockType Type { get; set; }
        public List<Bar> CrossedBars { get; set; }

        //public Block(double High, double low)
        //{
        //    this.High = High;
        //    this.Low = low;
        //}


        public int CompareTo(Block block)
        {
            return this.Low.CompareTo(block.Low);
        }
    }
}
