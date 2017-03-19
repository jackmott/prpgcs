using System;
using System.Collections.Generic;

namespace PRPG
{
    public static class RandUtil
    {
        public static Random r = new Random();

        // intex 
        public static float Float(float min, float max)
        {
            float range = max - min;
            float x = (float)r.NextDouble();
            x *= range;
            x += min;
            return x;
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
