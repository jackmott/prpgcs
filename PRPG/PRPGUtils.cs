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
            T minT = array[0];
            for (int i = 1; i < array.Length;i++)
            {
                var t = array[i];
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
            
            T minEntity = array[0];
            float minValue = Vector2.DistanceSquared(minEntity.pos, p.pos);
            for (int i = 1; i < array.Length;i++)
            {
                var e = array[i];
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
