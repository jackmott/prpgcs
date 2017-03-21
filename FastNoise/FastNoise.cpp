#include "FastNoise.h"


inline int fastFloor(float x) {
	int xi = (int)x;
	return x<xi ? xi - 1 : xi;
}

__m128 onef { 1.0f,1.0f,1.0f,1.0f};
__m128i one { 1,1,1,1 };
__m128i ffi { 0xff,0xff,0xff,0xff };
__m128 six { 6.0,6.0,6.0,6.0 };
__m128 fifteen { 15.0,15.0,15.0,15.0 };
__m128i fifteeni { 15,15,15,15 };
__m128i seveni{ 7,7,7,7 };
__m128 ten{ 10.0,10.0,10.0,10.0 };
__m128i fouri{ 4,4,4,4 };
__m128i zeroi{ 0,0,0,0 };
__m128 zero{ 0.0,0.0,0.0,0.0 };
__m128i twoi{ 2,2,2,2 };

inline __m128 gradSIMD2d(__m128i *hash, __m128  *x, __m128 *y) {

	__m128i h = Andi(*hash, seveni);
	__m128 h4 = ConvertToFloat(LessThani(h, fouri));
	__m128 u = BlendV(*x,*y, h4);
	__m128 v = BlendV(*y,*x, h4);

	__m128 h1 = ConvertToFloat(Equali(zeroi, Andi(h, one)));
	__m128 h2 = ConvertToFloat(Equali(zeroi, Andi(h, twoi)));

	return Add(BlendV(Sub(zero, u), u, h1), BlendV(Sub(zero, v), v, h2));
}

__m128 Perlin2D(unsigned char offset, __m128* x, __m128* y)
{
	um128i ix0, iy0, ix1, iy1;
	__m128 fx0,fy0,fx1,fy1;
		
	um128 ux; ux.m = *x;
	um128 uy; uy.m = *y;
	for (int i = 0; i < VECTOR_SIZE; i++)
	{
		ix0.a[i] = fastFloor((ux).a[i]);
		iy0.a[i] = fastFloor((uy).a[i]);
	}

	fx0 = Sub(*x, ConvertToFloat(ix0.m));
	fy0 = Sub(*y, ConvertToFloat(iy0.m));

	fx1 = Sub(fx0, onef);
	fy1 = Sub(fy0, onef);
	

	ix1.m = Andi(Addi(ix0.m, one), ffi);
	iy1.m = Andi(Addi(iy0.m, one), ffi);
	

	ix0.m = Andi(ix0.m, ffi);
	iy0.m = Andi(iy0.m, ffi);
	
	
	__m128 
	t = Mul(fy0, six);
	t = Sub(t, fifteen);
	t = Mul(t, fy0);
	t = Add(t, ten);
	t = Mul(t, fy0);
	t = Mul(t, fy0);
	t = Mul(t, fy0);

	
	__m128 
	s = Mul(fx0, six);
	s = Sub(s, fifteen);
	s = Mul(s, fx0);
	s = Add(s, ten);
	s = Mul(s, fx0);
	s = Mul(s, fx0);
	s = Mul(s, fx0);

	um128i p[4];
	for (int i = 0; i < VECTOR_SIZE; i++)
	{
		p[0].a[i] = perm[ix0.a[i] + perm[iy0.a[i]]];
		p[1].a[i] = perm[ix0.a[i] + perm[iy1.a[i]]];
		p[2].a[i] = perm[ix1.a[i] + perm[iy0.a[i]]];
		p[3].a[i] = perm[ix1.a[i] + perm[iy1.a[i]]];
	}

	__m128 nx0 = gradSIMD2d(&p[0].m, &fx0,& fy0);
	__m128 nx1 = gradSIMD2d(&p[1].m, &fx0, &fy1);

	__m128 n0 = Add(nx0, Mul(t, Sub(nx1, nx0)));

	nx0 = gradSIMD2d(&p[2].m, &fx1, &fy0);
	nx1 = gradSIMD2d(&p[3].m, &fx1, &fy1);

	__m128 n1 = Add(nx0, Mul(t, Sub(nx1, nx0)));

	return Add(nx0, Mul(s, Sub(n1, n0)));
}