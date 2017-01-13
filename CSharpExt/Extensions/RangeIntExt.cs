﻿using Noggog;
using System;

namespace System
{
    public static class RangeIntExt
    {
        public static int Get(this RangeInt range, RandomSource rand)
        {
            if (range.Min == range.Max)
            {
                return range.Min;
            }
            else
            {
                return rand.Next(range.Min, range.Max + 1);
            }
        }

        public static int GetNormalDist(this RangeInt range, RandomSource rand)
        {
            if (range.Min == range.Max)
            {
                return range.Min;
            }
            else
            {
                return rand.NextNormalDist(range.Min, range.Max + 1);
            }
        }
    }
}
