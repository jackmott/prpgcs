using System.Runtime.InteropServices;
using System.Security;

namespace PRPG
{
    [SuppressUnmanagedCodeSecurity]
    public static class Noise
    {
        [SuppressUnmanagedCodeSecurity]
        [DllImport("FastNoise")]
        public static unsafe extern float* GetNoiseBlock(float startx, float starty);
        [SuppressUnmanagedCodeSecurity]
        [DllImport("FastNoise")]
        public static unsafe extern float GetNoisePoint(float x, float y);
        [SuppressUnmanagedCodeSecurity]
        [DllImport("FastNoise")]
        public static unsafe extern void InitNoise(int blockSize, int octaves, float gain, float lac, float freq);
       
    }
}
