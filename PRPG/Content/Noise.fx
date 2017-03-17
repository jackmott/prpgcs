#if OPENGL
#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0_level_9_1
#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

sampler TextureSampler : register(s0);
float2 pos;

float hash(float2 p)  // replace this by something better
{
	p = 50.0*frac(p*0.3183099 + float2(0.71, 0.113));
	return -1.0 + 2.0*frac(p.x*p.y*(p.x + p.y));
}

float noise(in float2 p)
{
	float2 i = floor(p);
	float2 f = frac(p);

	float2 u = f*f*(3.0 - 2.0*f);

	return lerp(lerp(hash(i + float2(0.0, 0.0)),
		hash(i + float2(1.0, 0.0)), u.x),
		lerp(hash(i + float2(0.0, 1.0)),
			hash(i + float2(1.0, 1.0)), u.x), u.y);
}

// -----------------------------------------------



float4 NoisePixel(float4 color : COLOR0, float2 texCoord : TEXCOORD0) : COLOR0
{
	float4 colorMap[5];
	colorMap[0] = float4(0.0, 0.0, 1.0, 1.0);
	colorMap[1] = float4(0.0, 1.0, 0.0, 1.0);
	colorMap[2] = float4(0.7, 0.3, 0.1, 1.0);
	colorMap[3] = float4(0.5, 0.5, 0.5, 1.0);
	colorMap[4] = float4(1.0, 1.0, 1.0, 1.0);
	

	float f = 0.0;
	float2 p = texCoord + pos;

	p *= 8.0;
	float2x2 m = float2x2(1.6, 1.2, -1.2, 1.6);
	f = 0.5000*noise(p); 
	p = mul(p,m);
	f += 0.2500*noise(p); 
	p = mul(p, m);
	f += 0.1250*noise(p); 
	p = mul(p, m);
	f += 0.0625*noise(p); 	


	f = 0.5 + 0.5*f;
	

	int colorIndex = int(f*5.0);
	return colorMap[colorIndex];
}

technique Noise {
	pass Pass0 {
		PixelShader = compile ps_3_0 NoisePixel();
	}
}