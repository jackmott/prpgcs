#include "FastNoise.h"
#include <stdio.h>

inline int fastFloor(float x) {
	int xi = (int)x;
	return x < xi ? xi - 1 : xi;
}

__m128 onef = SetOne(1.0);
__m128i one = SetOnei(1);
__m128i ffi = SetOnei(0xff);
__m128 zero = SetZero();
__m128 F2 = SetOne(0.36602540378);
__m128 G2 = SetOne(0.2113248654);
__m128 G22 = SetOne(2.0*0.2113248654);
__m128 pfive = SetOne(0.5);
__m128 seventy = SetOne(70.0);


inline __m128 dotSIMD(const __m128 &x1, const __m128 &y1, const __m128 &x2, const __m128 &y2 )
{
	__m128 xx = Mul(x1, x2);
	__m128 yy = Mul(y1, y2);
	return Add(xx,yy);
}


inline __m128 simplex2D(const __m128 &x,const __m128 &y) {
	um128i i, j;		
	um128 s;
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

	__m128 t = Mul(ConvertToFloat(Addi(i.m, j.m)), G2);
	__m128 X0 = Sub(ConvertToFloat(i.m), t);
	__m128 Y0 = Sub(ConvertToFloat(j.m), t);	
	__m128 x0 = Sub(x, X0);
	__m128 y0 = Sub(y, Y0);	

	um128i i1, j1;
	i1.m = Andi(one, CastToInt(GreaterThanOrEq(x0, y0)));
	j1.m = Andi(one, CastToInt(GreaterThan(y0, x0)));
	
	
	__m128 x1 = Add(Sub(x0, ConvertToFloat(i1.m)), G2);
	__m128 y1 = Add(Sub(y0, ConvertToFloat(j1.m)), G2);	
	__m128 x2 = Add(Sub(x0,onef), G22);
	__m128 y2 = Add(Sub(y0,onef), G22);	
	

	um128i ii;
	ii.m = Andi(i.m, ffi);
	um128i jj;
	jj.m = Andi(j.m, ffi);
	
	um128i gi0, gi1, gi2;
#ifndef AVX2
	for (int i = 0; i < VECTOR_SIZE; i++)
	{
		gi0.a[i] = permMOD12[ii.a[i] + perm[jj.a[i]]];
		gi1.a[i] = permMOD12[ii.a[i] + i1.a[i] + perm[jj.a[i] + j1.a[i]]];
		gi2.a[i] = permMOD12[ii.a[i] + 1 + perm[jj.a[i] + 1]];		
	}
#else
	SIMDi pkk = Gather(perm, kk.m, 4);
	SIMDi pkkk1 = Gather(perm, Addi(kk.m, k1.m), 4);
	SIMDi pkkk2 = Gather(perm, Addi(kk.m, k2.m), 4);
	SIMDi pkk1 = Gather(perm, Addi(kk.m, one), 4);

	SIMDi pjj = Gather(perm, Addi(jj.m, pkk), 4);
	SIMDi pjjj1 = Gather(perm, Addi(jj.m, Addi(j1.m, pkkk1)), 4);
	SIMDi pjjj2 = Gather(perm, Addi(jj.m, Addi(j2.m, pkkk2)), 4);
	SIMDi pjj1 = Gather(perm, Addi(jj.m, Addi(one, pkk1)), 4);


	gi0.m = Gather(permMOD12, Addi(ii.m, pjj), 4);
	gi1.m = Gather(permMOD12, Addi(i1.m, Addi(ii.m, pjjj1)), 4);
	gi2.m = Gather(permMOD12, Addi(i2.m, Addi(ii.m, pjjj2)), 4);
	gi3.m = Gather(permMOD12, Addi(one, Addi(ii.m, pjj1)), 4);
#endif

	
	__m128 t0 = Sub(Sub(pfive, Mul(x0, x0)), Mul(y0, y0));
	__m128 t1 = Sub(Sub(pfive, Mul(x1, x1)), Mul(y1, y1));
	__m128 t2 = Sub(Sub(pfive, Mul(x2, x2)), Mul(y2, y2));
		
	__m128 t0q = Mul(t0, t0);
	t0q = Mul(t0q, t0q);
	__m128 t1q = Mul(t1, t1);
	t1q = Mul(t1q, t1q);
	__m128 t2q = Mul(t2, t2);
	t2q = Mul(t2q, t2q);
	

	um128
		gi0x, gi0y,
		gi1x, gi1y,
		gi2x, gi2y;
	
#ifndef AVX2
	for (int i = 0; i < VECTOR_SIZE; i++)
	{
		gi0x.a[i] = gradX[gi0.a[i]];
		gi0y.a[i] = gradY[gi0.a[i]];
	

		gi1x.a[i] = gradX[gi1.a[i]];
		gi1y.a[i] = gradY[gi1.a[i]];
	

		gi2x.a[i] = gradX[gi2.a[i]];
		gi2y.a[i] = gradY[gi2.a[i]];
			

	}
#else
	gi0x.m = Gatherf(gradX, gi0.m, 4);
	gi0y.m = Gatherf(gradY, gi0.m, 4);
	gi0z.m = Gatherf(gradZ, gi0.m, 4);

	gi1x.m = Gatherf(gradX, gi1.m, 4);
	gi1y.m = Gatherf(gradY, gi1.m, 4);
	gi1z.m = Gatherf(gradZ, gi1.m, 4);

	gi2x.m = Gatherf(gradX, gi2.m, 4);
	gi2y.m = Gatherf(gradY, gi2.m, 4);
	gi2z.m = Gatherf(gradZ, gi2.m, 4);

	gi3x.m = Gatherf(gradX, gi3.m, 4);
	gi3y.m = Gatherf(gradY, gi3.m, 4);
	gi3z.m = Gatherf(gradZ, gi3.m, 4);
#endif

	__m128 n0 = Mul(t0q, dotSIMD(gi0x.m, gi0y.m,x0, y0));
	__m128 n1 = Mul(t1q, dotSIMD(gi1x.m, gi1y.m,x1, y1));
	__m128 n2 = Mul(t2q, dotSIMD(gi2x.m, gi2y.m,x2, y2));
	

	//if ti < 0 then 0 else ni
	__m128 cond;

	cond = LessThan(t0, zero);
	n0 = BlendV(n0, zero, cond);
	cond = LessThan(t1, zero);
	n1 = BlendV(n1, zero, cond);
	cond = LessThan(t2, zero);
	n2 = BlendV(n2, zero, cond);
	

	return  Mul(onef, Add(n0, Add(n1, n2)));
}

__m128* g_result = nullptr;
int BLOCK_SIZE;
int BLOCK_SIZE_SQ;
__m128 g_lac, g_gain;
int g_octaves;


inline void fbm (__m128* out, const __m128& x, const __m128& y)
{
	__m128 freq = onef;
	__m128 amplitude = onef;
	*out = zero;
	for (int i = 0; i < g_octaves; i++)
	{
		__m128 vfx = Mul(x, freq);
		__m128 vfy = Mul(y, freq);
		*out = Add(*out, Mul(amplitude, simplex2D(vfx, vfy)));
		freq = Mul(freq, g_lac);
		amplitude = Mul(amplitude, g_gain);
	}
			
}



void InitNoise(int blockSize, int octaves, float gain, float lac) {
	BLOCK_SIZE = blockSize;	
	BLOCK_SIZE_SQ = blockSize*blockSize;
	g_lac = SetOne(lac);
	g_gain = SetOne(gain);
	g_octaves = octaves;
	g_result = (__m128*)_aligned_malloc(BLOCK_SIZE_SQ * sizeof(float), MEMORY_ALIGNMENT);	
}

float* GetNoiseBlock(float startx, float starty, float stepsize) {
	um128 startxs;
	__m128 xs, ys;
	
		
	__m128 stepSizeS = SetOne(stepsize*VECTOR_SIZE);
	
	float inx = startx;
	for (int i = 0; i < VECTOR_SIZE; i++)
	{
		startxs.a[i] = inx;
		inx += stepsize;
	}
		
	int count = 0;
	for (int y = 0; y < BLOCK_SIZE; y++)
	{
		xs = startxs.m;
		ys = SetOne(starty);
		for (int x = 0; x < BLOCK_SIZE; x+=VECTOR_SIZE)
		{						
			fbm(&g_result[count], xs, ys);
			xs = Add(xs, stepSizeS);			
			count++;
		}
		starty += stepsize;
	}
	
	return (float*)g_result;
}
