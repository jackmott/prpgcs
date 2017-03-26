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

        public static int NormalInt(int mean, int variance)
        {
            return (int)NormalFloat(mean,variance);
        }

        
        public static float NormalFloat(float mean, float variance)
        {
            double u1 = 1.0f - r.NextDouble(); //uniform(0,1] random doubles
            double u2 = 1.0f - r.NextDouble();
            float randStdNormal =(float)( Math.Sqrt(-2.0 * Math.Log(u1)) *
                                   Math.Sin(2.0 * Math.PI * u2)); //random normal(0,1)            
            return mean + variance * randStdNormal; //random normal(mean,stdDev^2)            
        }

        public static int NormalInt(int mean, int variance, int min, int max)
        {
            return MathHelper.Clamp(NormalInt(mean, variance), min, max);
        }

        public static float NormalFloat(float mean, float variance, float min, float max)
        {
            return MathHelper.Clamp(NormalFloat(mean, variance), min, max);
        }

        public static Vector2 Vector2(float firstMax, float secondMax)
        {
            return new Vector2(Float(0.0f, firstMax), Float(0.0f, secondMax));
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
