#include "FastNoise.h"
#include <stdio.h>

inline int fastFloor(float x) {
	int xi = (int)x;
	return x < xi ? xi - 1 : xi;
}

SIMD onef = SetOne(1.0);
SIMDi one = SetOnei(1);
SIMDi ffi = SetOnei(0xff);
SIMD zero = SetZero();
SIMD F2 = SetOne(0.36602540378);
SIMD G2 = SetOne(0.2113248654);
SIMD G22 = SetOne(2.0*0.2113248654);
SIMD pfive = SetOne(0.5);
SIMD seventy = SetOne(70.0);



#define dotSIMD(x1,y1,x2,y2) Add(Mul(x1, x2), Mul(y1, y2))


inline SIMD simplex2D(const SIMD &x,const SIMD &y) {
	um128i i, j;		
	USIMD s;
	s.m = Mul(F2, Add(x,y));

#ifdef SSE41
	i.m = ConvertToInt(Floor(Add(x, s.m)));
	j.m = ConvertToInt(Floor(Add(y, s.m)));	
#endif
	//drop out to scalar if we don't
#ifndef SSE41
	um128 ux;
	ux.m = x;
	um128 uy;
	uy.m = y;
	
	for (int x = 0; x < VECTOR_SIZE; x++)
	{
		i.a[x] = fastFloor((ux).a[x] + s.a[x]);
		j.a[x] = fastFloor((uy).a[x] + s.a[x]);		
	}
#endif

	SIMD t = Mul(ConvertToFloat(Addi(i.m, j.m)), G2);
	SIMD X0 = Sub(ConvertToFloat(i.m), t);
	SIMD Y0 = Sub(ConvertToFloat(j.m), t);	
	SIMD x0 = Sub(x, X0);
	SIMD y0 = Sub(y, Y0);	

	um128i i1, j1;
	i1.m = Andi(one, CastToInt(GreaterThanOrEq(x0, y0)));
	j1.m = Andi(one, CastToInt(GreaterThan(y0, x0)));
	
	
	SIMD x1 = Add(Sub(x0, ConvertToFloat(i1.m)), G2);
	SIMD y1 = Add(Sub(y0, ConvertToFloat(j1.m)), G2);	
	SIMD x2 = Add(Sub(x0,onef), G22);
	SIMD y2 = Add(Sub(y0,onef), G22);	
	

	um128i ii;
	ii.m = Andi(i.m, ffi);
	um128i jj;
	jj.m = Andi(j.m, ffi);
	
	um128i gi0, gi1, gi2;

	for (int i = 0; i < VECTOR_SIZE; i++)
	{
		gi0.a[i] = permMOD12[ii.a[i] + perm[jj.a[i]]];
		gi1.a[i] = permMOD12[ii.a[i] + i1.a[i] + perm[jj.a[i] + j1.a[i]]];
		gi2.a[i] = permMOD12[ii.a[i] + 1 + perm[jj.a[i] + 1]];		
	}

	SIMD t0 = Sub(Sub(pfive, Mul(x0, x0)), Mul(y0, y0));
	SIMD t1 = Sub(Sub(pfive, Mul(x1, x1)), Mul(y1, y1));
	SIMD t2 = Sub(Sub(pfive, Mul(x2, x2)), Mul(y2, y2));
		
	SIMD t0q = Mul(t0, t0);
	t0q = Mul(t0q, t0q);
	SIMD t1q = Mul(t1, t1);
	t1q = Mul(t1q, t1q);
	SIMD t2q = Mul(t2, t2);
	t2q = Mul(t2q, t2q);
	

	USIMD
		gi0x, gi0y,
		gi1x, gi1y,
		gi2x, gi2y;
	

	for (int i = 0; i < VECTOR_SIZE; i++)
	{
		gi0x.a[i] = gradX[gi0.a[i]];
		gi0y.a[i] = gradY[gi0.a[i]];
	

		gi1x.a[i] = gradX[gi1.a[i]];
		gi1y.a[i] = gradY[gi1.a[i]];
	

		gi2x.a[i] = gradX[gi2.a[i]];
		gi2y.a[i] = gradY[gi2.a[i]];
			

	}

	SIMD n0 = Mul(t0q, dotSIMD(gi0x.m, gi0y.m,x0, y0));
	SIMD n1 = Mul(t1q, dotSIMD(gi1x.m, gi1y.m,x1, y1));
	SIMD n2 = Mul(t2q, dotSIMD(gi2x.m, gi2y.m,x2, y2));
	

	//if ti < 0 then 0 else ni
	SIMD cond;

	cond = LessThan(t0, zero);
	n0 = BlendV(n0, zero, cond);
	cond = LessThan(t1, zero);
	n1 = BlendV(n1, zero, cond);
	cond = LessThan(t2, zero);
	n2 = BlendV(n2, zero, cond);
	

	return  Mul(onef, Add(n0, Add(n1, n2)));
}

SIMD* g_result = nullptr;
int g_blockSize;
int g_blockSizeSquared;
SIMD g_lac, g_gain;
int g_octaves;

SIMD mulFactor = SetOne(1.51);
SIMD offset = SetOne(0.5);
SIMD g_stepSizeS;
float g_stepSize;
float g_freq;
SIMD g_freqS;

inline void fbm (SIMD* out, const SIMD& x, const SIMD& y)
{
	SIMD freq = g_freqS;		
	SIMD vfx = Mul(x, freq);
	SIMD vfy = Mul(y, freq);
	*out = zero;
	*out = Add(*out, simplex2D(vfx, vfy));
	SIMD amplitude = g_gain;
	freq = Mul(freq, g_lac);

	for (int i = 1; i < g_octaves; i++)
	{
		vfx = Mul(x, freq);
		vfy = Mul(y, freq);
		*out = Add(*out, Mul(amplitude, simplex2D(vfx, vfy)));
		freq = Mul(freq, g_lac);
		amplitude = Mul(amplitude, g_gain);
	}
	*out = Add(Mul(mulFactor, *out), offset);
}




void InitNoise(int blockSize, int octaves, float gain, float lac,float freq) {
	g_blockSize = blockSize;	
	g_blockSizeSquared = blockSize*blockSize;
	g_lac = SetOne(lac);
	g_gain = SetOne(gain);
	g_octaves = octaves;
	g_freq = freq;
	g_freqS = SetOne(freq);
	g_stepSize = (1.0f/(float)blockSize)*freq;
	g_stepSizeS = SetOne(g_stepSize*VECTOR_SIZE);
	g_result = (SIMD*)_aligned_malloc(g_blockSizeSquared * sizeof(float), MEMORY_ALIGNMENT);	
}

float* GetNoiseBlock(float startx, float starty) {
	USIMD startxs;
	SIMD xs, ys;
	
				
	float inx = startx*g_freq;
	starty *= g_freq;
	for (int i = 0; i < VECTOR_SIZE; i++)
	{
		startxs.a[i] = inx;
		inx += g_stepSize;
	}
		
	int count = 0;
	for (int y = 0; y < g_blockSize; y++)
	{
		xs = startxs.m;
		ys = SetOne(starty);
		for (int x = 0; x < g_blockSize; x+=VECTOR_SIZE)
		{						
			fbm(&g_result[count], xs, ys);
			xs = Add(xs, g_stepSizeS);			
			count++;
		}
		starty += g_stepSize;
	}
	
	return (float*)g_result;
}


float GetNoisePoint(float x, float y) {
		
	SIMD xs = SetOne(x*g_freq);			
	SIMD ys = SetOne(y*g_freq);
	USIMD result;
		
	fbm(&result.m, xs, ys);
			
	return result.a[0];
}
