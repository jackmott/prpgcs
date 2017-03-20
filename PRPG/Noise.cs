using System.Runtime.CompilerServices;

namespace PRPG
{
    public static class Noise
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float Lerp(float a, float b, float t) { return a + t * (b - a); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float ValCoord2D(int seed, int x, int y)
        {
            int n = seed;
            n ^= X_PRIME * x;
            n ^= Y_PRIME * y;

            return (n * n * n * 60493) / 2147483648.0f;
        }

        private static float SingleValue(int seed, float x, float y)
        {
            int x0 = FastFloor(x);
            int y0 = FastFloor(y);
            int x1 = x0 + 1;
            int y1 = y0 + 1;

            float xs, ys;

            xs = x - x0;
            ys = y - y0;


            float xf0 = Lerp(ValCoord2D(seed, x0, y0), ValCoord2D(seed, x1, y0), xs);
            float xf1 = Lerp(ValCoord2D(seed, x0, y1), ValCoord2D(seed, x1, y1), xs);

            return Lerp(xf0, xf1, ys);
        }

        public static float FractalFBM(int seed, float x, float y)
        {
            float gain = 2.0f;
            float lacunarity = 0.6f;
            float octaves = 4;
            float sum = 0f;
            float freq = 1f;
            float amplitude = 1f;

            for (int i = 0; i < octaves; i++) {
                sum += freq * Simplex(seed, x * amplitude, y * amplitude);
                freq *= lacunarity;
                amplitude *= gain;
            }

            return sum * 21.114f + 0.5f;
        }


        private const int X_PRIME = 1619;
        private const int Y_PRIME = 31337;
        private const float F2 = (float)(1.0 / 2.0);
        private const float G2 = (float)(1.0 / 4.0);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int FastFloor(float f) { return (f >= 0.0f ? (int)f : (int)f - 1); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float GradCoord2D(int seed, int x, int y, float xd, float yd)
        {
            int hash = seed;
            hash ^= X_PRIME * x;
            hash ^= Y_PRIME * y;

            hash = hash * hash * hash * 60493;
            hash = (hash >> 13) ^ hash;

            if ((hash & 4) == 0) {
                if ((hash & 1) != 0) {
                    if ((hash & 2) != 0)
                        return xd + yd;

                    return xd - yd;
                }
                if ((hash & 2) != 0)
                    return yd - xd;

                return -xd - yd;
            }

            if ((hash & 2) == 0)
                return ((hash & 1) != 0 ? -xd : -yd);

            return ((hash & 1) != 0 ? xd : yd);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Simplex(int seed, float x, float y)
        {
            float t = (x + y) * F2;
            int i = FastFloor(x + t);
            int j = FastFloor(y + t);

            t = (i + j) * G2;
            float X0 = i - t;
            float Y0 = j - t;

            float x0 = x - X0;
            float y0 = y - Y0;

            int i1, j1;
            if (x0 > y0) {
                i1 = 1;
                j1 = 0;
            }
            else {
                i1 = 0;
                j1 = 1;
            }

            float x1 = x0 - i1 + G2;
            float y1 = y0 - j1 + G2;
            float x2 = x0 - 1.0f + F2;
            float y2 = y0 - 1.0f + F2;

            float n0, n1, n2;

            t = 0.5f - x0 * x0 - y0 * y0;
            if (t < 0)
                n0 = 0;
            else {
                t *= t;
                n0 = t * t * GradCoord2D(seed, i, j, x0, y0);
            }

            t = 0.5f - x1 * x1 - y1 * y1;
            if (t < 0)
                n1 = 0;
            else {
                t *= t;
                n1 = t * t * GradCoord2D(seed, i + i1, j + j1, x1, y1);
            }

            t = 0.5f - x2 * x2 - y2 * y2;
            if (t < 0)
                n2 = 0;
            else {
                t *= t;
                n2 = t * t * GradCoord2D(seed, i + 1, j + 1, x2, y2);
            }

            return (n0 + n1 + n2);
        }



    }
}
