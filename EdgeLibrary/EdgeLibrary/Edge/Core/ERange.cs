﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EdgeLibrary.Edge
{
    public class ERange
    {
        public float Min;
        public float Max;

        protected float temp;

        public ERange(float min, float max)
        {
            Min = min;
            Max = max;
            temp = 0;

            reorder();
        }

        public ERange(float number) : this(number, number) { }

        public static ERange RangeWithDiffer(float number, float differ)
        {
            return new ERange(number - differ/2, number + differ/2);
        }

        public static ERange Range(float min, float max) 
        { 
            return new ERange(min, max); 
        }

        public float GetRandom(Random random)
        {
            reorder();
            return random.Next((int)Min, (int)Max);
        }

        protected void reorder()
        {
            if (Min > Max)
            {
                temp = Min;
                Min = Max;
                Max = temp;
            }
        }
    }

    public struct ERangeArray
    {
        public List<ERange> Ranges;

        public ERangeArray(params ERange[] items)
        {
            Ranges = new List<ERange>(items);
        }

        public float[] GetRandom(Random random)
        {
            float[] numbers = new float[Ranges.Count];

            for (int i = 0; i < Ranges.Count; i++)
            {
                numbers[i] = Ranges[i].GetRandom(random);
            }

            return numbers;
        }

        public float GetRandom(int index, Random random)
        {
            return Ranges[index].GetRandom(random);
        }
    }
}
