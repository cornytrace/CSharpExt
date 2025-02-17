using System;
using System.Collections.Generic;
using System.Linq;

namespace Noggog
{
    public class RandomSource
    {
        private static System.Random randSource = new Random();
        public int NumQueries { get; private set; }
        private readonly System.Random rand;
        public readonly int OriginalSeed;
        private double? spareRoll;

        public RandomSource()
        {
            this.OriginalSeed = randSource.Next();
            this.rand = new System.Random(OriginalSeed);
        }

        public RandomSource(int seed)
        {
            this.OriginalSeed = seed;
            this.rand = new System.Random(seed);
        }

        public int Next()
        {
            this.NumQueries++;
            return rand.Next();
        }

        public int Next(int min, int max)
        {
            this.NumQueries++;
            return rand.Next(min, max);
        }

        public int Next(int max)
        {
            this.NumQueries++;
            return rand.Next(max);
        }

        public double NextDouble()
        {
            this.NumQueries++;
            return rand.NextDouble();
        }

        public bool NextPercent(double percent)
        {
            this.NumQueries++;
            return percent > rand.NextDouble();
        }

        public bool NextPercent(int percent)
        {
            this.NumQueries++;
            return percent >= rand.Next(1, 100);
        }

        public Percent NextPercent()
        {
            this.NumQueries++;
            return new Percent(rand.NextDouble());
        }

        public bool NextBool()
        {
            this.NumQueries++;
            return rand.Next(2) == 1;
        }

        public float NextAngle()
        {
            this.NumQueries++;
            return (float)(rand.NextDouble() * 360);
        }

        public int NextNegative()
        {
            return NextBool() ? -1 : 1;
        }

        public double NextDouble(double magn)
        {
            this.NumQueries++;
            return rand.NextDouble() * magn * 2 - magn;
        }

        public float NextRotationDegree()
        {
            this.NumQueries++;
            return (float)(rand.NextDouble() * 360);
        }
        
        private double NextMargsalia(bool useSpare)
        {
            double magn;
            if (useSpare && spareRoll.HasValue)
            {
                magn = spareRoll.Value;
                spareRoll = null;
            }
            else
            {
                double roll1 = this.NextDouble() * 2 - 1;
                double roll2 = this.NextDouble() * 2 - 1;
                double s = Math.Pow(roll1, 2) + Math.Pow(roll2, 2);
                if (s >= 1)
                { // Retry
                    return NextNormalDist(useSpare);
                }
                double commonTerm = Math.Sqrt(-2 * Math.Log(s) / s);
                magn = roll1 * commonTerm;
                if (!spareRoll.HasValue)
                {
                    spareRoll = roll2 * commonTerm;
                }
            }
            return magn;
        }

        public double NextNormalDist(double mean, double stdDev, bool useSpare)
        {
            return NextMargsalia(useSpare) * stdDev + mean;
        }

        public double NextNormalDist(bool useSpare = true)
        {
            return NextMargsalia(useSpare);
        }

        public double NextNormalDist(double min, double max, double wingCutoff = 2, bool useSpare = true)
        {
            if (max < min)
            {
                throw new ArgumentException("Max must be greater than or equal to min");
            }
            if (max == min)
            {
                return max;
            }
            double magn = NextNormalDist(useSpare);
            while (Math.Abs(magn) > wingCutoff)
            {
                magn = NextNormalDist(true);
            }
            magn = (magn + wingCutoff) / 2 / wingCutoff;
            if (magn.EqualsWithin(1d))
            {
                return max;
            }
            return magn * (max - min) + min;
        }

        public P2Double NextInUnitCircle()
        {
            double radians = 2 * Math.PI * this.NextDouble();
            return new P2Double(
                (float)Math.Cos(radians),
                (float)Math.Sin(radians));
        }

        public int NextNormalDist(int min, int max, double wingCutoff = 2, bool useSpare = true)
        {
            return (int)NextNormalDist(min, (double)max, wingCutoff, useSpare);
        }

        public ushort NextNormalDist(ushort min, ushort max, double wingCutoff = 2, bool useSpare = true)
        {
            return (ushort)NextNormalDist((int)min, (int)max, wingCutoff, useSpare);
        }

        public int NextNormalDist(RangeInt32 range, double wingCutoff = 2, bool useSpare = true)
        {
            return NextNormalDist(range.Min, range.Max, wingCutoff, useSpare);
        }

        public override string ToString()
        {
            return $"(Seed: {OriginalSeed}, #Queries: {this.NumQueries})";
        }
    }
}
