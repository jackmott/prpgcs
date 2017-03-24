using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace PRPG
{
    public static class RandUtil
    {
        public static Random r = new Random(1);

        // intex 
        public static float Float(float min, float max)
        {
            float range = max - min;
            float x = (float)r.NextDouble();
            x *= range;
            x += min;
            return x;
        }

        public static Vector2 Vector2(float firstMax, float secondMax)
        {
            return new Vector2(RandUtil.Float(0.0f, firstMax), RandUtil.Float(0.0f, secondMax));
        }

        public static bool OneInN(int N)
        {
            return r.Next(N) == 0;
            
        }
        public static float Float(float max)
        {
            return (float)r.NextDouble() * max;
        }

        public static bool Bool()
        {
            return r.Next(2) == 0;
        }

        public static int Int(int min, int max)
        {
            return r.Next(min, max + 1);
        }

        public static int Int(int max)
        {
            return r.Next(max + 1);
        }

        public static int IntEx(int max)
        {
            return r.Next(max);
        }

        public static int IntEx(int min,int max)
        {
            return r.Next(min,max);
        }

        public static T Index<T>(IList<T> list)
        {
            return list[r.Next(list.Count)];
        }

        public static T Index<T>(T[] array)
        {
            return array[r.Next(array.Length)];
        }

    }
}
