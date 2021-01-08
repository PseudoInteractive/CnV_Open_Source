﻿#ifndef __Constants__
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
};

DECLARE_TEXTURE(Texture, 0);
DECLARE_TEXTURE(Normal, 1);

#endif
