#ifndef __Macros__
#define __Macros__

#define __sqr(__a) ((__a)*(__a))
static const float Epsilon = 1.0 / 4096.0;
static const float PI = 3.141592;
#define half min16float
#define half2 min16float2
#define half3 min16float3
#define half4 min16float4
#define half3x3 min16float3x3
#define half3x4 min16float3x4


#if SM4

// Macros for targetting shader model 4.0 (DX11)

#define TECHNIQUE(name, vsname, psname ) \
	technique name { pass { VertexShader = compile vs_4_0 vsname (); PixelShader = compile ps_4_0 psname(); } }

#define BEGIN_CONSTANTS     cbuffer Parameters : register(b0) { \
float4x4 WorldViewProjection;


#define MATRIX_CONSTANTS
#define END_CONSTANTS       };

#define _vs(r)
#define _ps(r)
#define _cb(r)

#define DECLARE_TEXTURE(Name, index) \
    Texture2D<float4> Name : register(t##index); \
    sampler Name##Sampler : register(s##index)

#define DECLARE_CUBEMAP(Name, index) \
    TextureCube<float4> Name : register(t##index); \
    sampler Name##Sampler : register(s##index)

#define SAMPLE_TEXTURE(Name, texCoord)  Name.Sample(Name##Sampler, texCoord)
#define SAMPLE_CUBEMAP(Name, texCoord)  Name.Sample(Name##Sampler, texCoord)



#else

// Macros for targetting shader model 2.0 (DX9)

#define TECHNIQUE(name, vsname, psname ) \
	technique name { pass { VertexShader = compile vs_2_0 vsname (); PixelShader = compile ps_2_0 psname(); } }

#define BEGIN_CONSTANTS
#define MATRIX_CONSTANTS
#define END_CONSTANTS

#define _vs(r)  : register(vs, r)
#define _ps(r)  : register(ps, r)
#define _cb(r)

#define DECLARE_TEXTURE(Name, index) \
    sampler2D Name : register(s##index);

#define DECLARE_CUBEMAP(Name, index) \
    samplerCUBE Name : register(s##index);

#define SAMPLE_TEXTURE(Name, texCoord)  tex2D(Name, texCoord)
#define SAMPLE_CUBEMAP(Name, texCoord)  texCUBE(Name, texCoord)


#endif


#endif