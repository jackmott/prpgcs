using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;


namespace PRPG
{
    public static class PRPGUtils
    {
        public static (T obj, float value) MinBy<T>(this T[] array,Func<T,float> lambda) 
        {
            float minValue = float.MaxValue;
            T minT = default(T);
            foreach (var t in array)
            {
                var value = lambda.Invoke(t);
                if (value < minValue)
                {
                    minValue = value;
                    minT = t;
                }
            }
            return (minT, minValue);
        }

        public static (T obj, float value) ClosestTo<T>(this T[] array,Entity p) where T : Entity
        {
            float minValue = float.MaxValue;
            T minEntity = null;
            foreach (var e in array)
            {
                var value = Vector2.DistanceSquared(e.pos, p.pos);
                if (value < minValue)
                {
                    minValue = value;
                    minEntity = e;
                }
            }
            return (minEntity, minValue);
        }
    }
}
