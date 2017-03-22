using System.Runtime.InteropServices;

namespace PRPG
{
    public static class Noise
    {
        [DllImport("FastNoise")]
        public static unsafe extern float* GetNoiseBlock(float startx, float starty);
        [DllImport("FastNoise")]
        public static unsafe extern float GetNoisePoint(float x, float y);
        [DllImport("FastNoise")]
        public static unsafe extern void InitNoise(int blockSize, int octaves, float gain, float lac, float freq);
       
    }
}
