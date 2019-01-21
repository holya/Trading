using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Trading.Common;

namespace Trading.Analyzers
{
    public class MPAnalyzer
    {
        private List<Bar> barList;
        private List<Block> blockList;
        private int pocBlockIndex;
        
        public MPAnalyzer()
        {
            this.barList = new List<Bar>();
            this.blockList = new List<Block>();
        }

        public List<Block> BlockList { get { this.blockList.Sort();  return this.blockList; } }

        public double BarListCount { get { return this.barList.Count; } }
        public DateTime StartDateTime { get { return this.barList.First().DateTime; } }
        public DateTime EndDateTime { get { return this.barList.Last().DateTime; } }
        public Block PocBlock { get; private set; }
        public double Poc { get; private set; }
        public double Open { get { return this.barList.First().Open; } }

        public double High { get { return this.blockList.Max(b => b.High); } }
        public double Low { get { return this.blockList.Min(b => b.Low); } }


        public double Close { get { return this.barList.Last().Close; } }
        public double ValueAreaHigh { get; private set; }
        public double ValueAreaLow { get; private set; }


        public void AddBar(Bar bar)
        {
            this.barList.Add(bar);
            this.addBarToMp(bar);
        }

        //public void addBarList()
        //{
        //    foreach(var b in barList)
        //    {

        //        this.addBarToMp(b);
        //    }
        //}


        public void UpdatBar(Bar updatedLastBar)
        {
            this.barList[this.barList.Count - 1] = updatedLastBar;
            this.blockList.Clear();

            foreach(var b in this.barList)
                this.addBarToMp(b);
        }

        private void addBarToMp(Bar bar)
        {
            if(this.blockList.Count == 0)
            {
                this.blockList.Add(new Block { High = bar.High, Low = bar.Low, HitCount = 1 });
                return;
            }

            //var blocksCompletelyCoveredByBar = 
            this.blockList.FindAll(block => block.High <= bar.High && block.Low >= bar.Low).ForEach(block => block.HitCount++);
            //blocksCompletelyCoveredByBar?.ForEach(block => block.HitCount++);


            var blockSplittedByBarHigh = this.blockList.Find(block => 
                    block.High > bar.High && 
                    block.Low < bar.High &&
                    block.Low >= bar.Low);
            if(blockSplittedByBarHigh != null)
            {
                this.blockList.Add(new Block { High = blockSplittedByBarHigh.High, Low = bar.High, HitCount = blockSplittedByBarHigh.HitCount });

                blockSplittedByBarHigh.High = bar.High;
                blockSplittedByBarHigh.HitCount++;
            }

            var blockSplittedByBarLow = this.blockList.Find(block => 
                    block.High > bar.Low && 
                    block.Low < bar.Low &&
                    block.High <= bar.High);
            if(blockSplittedByBarLow != null)
            {
                this.blockList.Add(new Block { High = bar.Low, Low = blockSplittedByBarLow.Low, HitCount = blockSplittedByBarLow.HitCount });

                blockSplittedByBarLow.Low = bar.Low;
                blockSplittedByBarLow.HitCount++;
            }

            var blockHigherAndLowerThanBar = this.blockList.Find(block => 
            block.High > bar.High && 
            block.Low < bar.Low);
            if(blockHigherAndLowerThanBar != null)
            {
                this.blockList.Add(new Block { High = blockHigherAndLowerThanBar.High, Low = bar.High, HitCount = blockHigherAndLowerThanBar.HitCount });
                this.blockList.Add(new Block { High = bar.Low, Low = blockHigherAndLowerThanBar.Low, HitCount = blockHigherAndLowerThanBar.HitCount });

                blockHigherAndLowerThanBar.High = bar.High;
                blockHigherAndLowerThanBar.Low = bar.Low;
                blockHigherAndLowerThanBar.HitCount++;
            }


            if(bar.Low < this.Low)
            {

                Block outsideDownBlock = new Block { Low = bar.Low, High = this.Low, HitCount = 1 };

                if(bar.High < this.Low)
                {
                    var gapBlock = new Block { High = this.Low, Low = bar.High };
                    this.blockList.Add(gapBlock);

                    outsideDownBlock.High = bar.High;
                }

                this.blockList.Add(outsideDownBlock);
            }

            if(bar.High > this.High)
            {
                Block outsideUpBlock = new Block { High = bar.High, Low = this.High, HitCount = 1 };

                if(bar.Low > this.High)
                {
                    var gapBlock = new Block { High = bar.Low, Low = this.High };
                    this.blockList.Add(gapBlock);

                    outsideUpBlock.Low = bar.Low;
                }

                this.blockList.Add(outsideUpBlock);
            }
        }


        private void addBlock(double low, double high, ushort hitCount)
        {
            this.blockList.Add(new Block { Low = low, High = high, HitCount = hitCount });
        }

        private void calcPoc()
        {
            double high = this.blockList.Max(p => p.High);
            double low = this.blockList.Min(p => p.Low);
            double mid = (high + low) / 2;

            this.PocBlock = this.blockList[0];
            Block nextBlock;
            this.pocBlockIndex = 0;

            for(int x = 1; x < this.blockList.Count; x++)
            {
                nextBlock = this.blockList[x];

                if(nextBlock.HitCount > this.PocBlock.HitCount)
                {
                    this.PocBlock = nextBlock;
                    this.pocBlockIndex = x;
                }
                else if(nextBlock.HitCount == this.PocBlock.HitCount)
                {
                    //this.PocBlock = this.closerBlock(this.PocBlock, nextBlock, mid);
                    if(!((this.closerNum(this.PocBlock, mid) - mid) < (this.closerNum(nextBlock, mid) - mid)))
                    {
                        this.PocBlock = nextBlock;
                        this.pocBlockIndex = x;
                    }
                    //this.pocBlockIndex = x;
                }
            }

            this.Poc = this.closerNum(this.PocBlock, mid);

        }
        private Block closerBlock(Block first, Block second, double compareNum)
        {
            return Math.Abs(this.closerNum(first, compareNum) - compareNum) <
                    Math.Abs(this.closerNum(second, compareNum) - compareNum) ? first : second;

        }
        private double closerNum(Block block, double compareNum)
        {
            if(block.Low <= compareNum && block.High >= compareNum)
                return compareNum;

            return Math.Abs(compareNum - block.Low) < Math.Abs(compareNum - block.High) ? block.Low : block.High;
        }

        private void calcValueArea()
        {
            if(this.blockList.Count == 1)
            {

            }
            int highRowCount = this.pocBlockIndex + 1;
            int lowRowCount = this.pocBlockIndex - 1;
            bool lowReached = false;
            bool highReached = false;
            int tpoCountCutoff = (int)(this.blockList.Sum(p => p.HitCount * 0.7));
            int totalTpo = this.PocBlock.HitCount;
            this.ValueAreaHigh = this.PocBlock.High;
            this.ValueAreaLow = this.PocBlock.Low;

            //double smallestUnitHeight = blockList.Min(p => Math.Abs(p.High) - Math.Abs(p.Low));
            //foreach (var block in this.blockList)
            //{
            //    var a = block.High * 100;
            //    var b = block.Low * 100;
            //    var total = (double)(a - b) * block.HitCount * 0.7;

            //    tpoCountCutoff += (int)total;
            //}

            while(totalTpo <= tpoCountCutoff/* && highRowCount < this.blockList.Count && lowRowCount < this.blockList.Count*/)
            {

                Block highBlock = this.blockList[highRowCount > this.blockList.Count - 1 ? this.blockList.Count - 1 : highRowCount];
                Block lowBlock = this.blockList[lowRowCount <= 0 ? 0 : lowRowCount];
                if(!highReached && highBlock.HitCount > lowBlock.HitCount)
                {
                    //if (!highReached)
                    //{
                    totalTpo += highBlock.HitCount;
                    highRowCount++;
                    //}
                    this.ValueAreaHigh = highBlock.High;
                }
                else if(!lowReached && highBlock.HitCount < lowBlock.HitCount)
                {
                    //if (!lowReached)
                    //{
                    totalTpo += lowBlock.HitCount;
                    lowRowCount--;
                    //}
                    this.ValueAreaLow = lowBlock.Low;
                }
                else
                {
                    if(!highReached)
                    {
                        totalTpo += highBlock.HitCount;
                        highRowCount++;
                    }
                    this.ValueAreaHigh = highBlock.High;

                    if(!lowReached)
                    {
                        totalTpo += lowBlock.HitCount;
                        lowRowCount--;
                    }
                    this.ValueAreaLow = lowBlock.Low;
                }

                if(highRowCount > this.blockList.Count - 1)
                    highReached = true;
                if(lowRowCount < 0)
                    lowReached = true;
            }

            //this.blockList.First(p => p.Low <= this.ValueAreaLow && p.High >= this.ValueAreaLow).Type = BlockType.ValueAreaLow;
            //this.blockList.First(p => p.Low <= this.ValueAreaHigh && p.High >= this.ValueAreaHigh).Type = BlockType.ValueAreaHigh;

            this.PocBlock.Type = BlockType.Poc;

            //Setup buying and selling tail
            var firstBlock = this.blockList.First();
            if(firstBlock.HitCount == 1)
                firstBlock.Type = BlockType.BuyingTail;
            var lastBlock = this.blockList.Last();
            if(lastBlock.HitCount == 1)
                lastBlock.Type = BlockType.SellingTail;
            //Setup singlePrint
            var singlePrints = this.blockList.Where(block => block.HitCount == 1 && block != firstBlock && block != lastBlock);
            foreach(var sp in singlePrints)
                sp.Type = BlockType.SinglePrint;

        }
    }
}
