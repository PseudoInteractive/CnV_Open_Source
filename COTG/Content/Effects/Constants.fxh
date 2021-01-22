#ifndef __Constants__
#define __Constants__
#include "Macros.fxh"
#include "Structures.fxh"

cbuffer Parameters : register(b0) { 
float4x4 WorldViewProjection;

	float3 lightPosition;
	float3 cameraPosition;
	float4 lightGains; // emissive, diffuse speciular
	float4 planetGains; // emissive, diffuse speciular
	float4 lightAmbient;
	float4 lightColor;
	float4 lightSpecular;

};

DECLARE_TEXTURE(Texture, 0);
DECLARE_TEXTURE(Normal, 1);

Texture2D<float4> Font : register(t7);
SamplerState FontSampler : register(s7)
{
	Filter = Linear;
	MipLODBias = -1.5;
	AddressU = Border;
	AddressV = Border;
};


#endif
