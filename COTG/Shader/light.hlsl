// Copyright (c) Microsoft Corporation. All rights reserved.
//
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.


// This shader has two input textures:
//
//  - First is the image that will be processed by the sketch effect.
//
//  - Second is an overlay containing a pencil sketch texture. This combines
//    three different patterns of strokes in the red, green and blue channels.

#define D2D_INPUT_COUNT 1
#define D2D_REQUIRES_SCENE_POSITION 

#include "d2d1effecthelpers.hlsli"



// Settings controlling the edge detection filter.

// Randomly offsets the sketch overlay pattern to create a hand-drawn animation effect.
float3 lightPosition;
float3 cameraPosition;

float normalGain=1;


D2D_PS_ENTRY(main)
{
	float4 positionInPixels = D2DGetScenePosition();
    // Look up the original color from the source image.
	float4 color = D2DSampleInputAtOffset(0, float2(0,0)).rgba;
	float height = dot(color.rgb, float3(0.3333333, 0.3333333, 0.3333333));
	float dHdx = ddx(height) * normalGain;
	float dHdy = ddy(height) * normalGain;
	float3 normal = normalize( float3(dHdx, dHdy, 1) );
	
	float3 dL = positionInPixels.xyz - lightPosition.xyz;
	float3 lightDir = normalize(dL);
	float3 viewDir = positionInPixels.xyz - cameraPosition;
	float3 specDir = normalize(viewDir - dot(viewDir, normal) * 2 * normal);
	float spec = pow(saturate(-dot(specDir, lightDir)), 8);
	float diff = -dot(lightDir, normal);
	float3 base = diff * color.rgb;
	spec = spec * saturate(height * height * 4.0 - 0.25);
	float3 rgb = diff * color.rgb + spec;
	//rgb = 2 * rgb / (1 + rgb);
	return float4(rgb*color.a  , color.a);
}

//// The MIT License
//// Copyright © 2013 Inigo Quilez
//// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions: The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software. THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

//// Mikkelsen's technique for Bump Mapping Unparametrized Surfaces 
//// https://dl.dropboxusercontent.com/u/55891920/papers/mm_sfgrad_bump.pdf
//// Pretty much copy & pasted, with minor changes. It aliases quite a bit :(


//// make GPU_DERIVATIVES 1 for dFdx()/dFdy() based derivatives, which produces
//// 2x2 pixel artifacts (in a deferred renderer you could perhaps compute these
//// by manually differencing UVs in the uv buffer)

//#define GPU_DERIVATIVES 1

////===============================================================================================
////===============================================================================================
////===============================================================================================
////===============================================================================================
////===============================================================================================


//float3 doBump(in float3 pos, in float3 nor, in float signal, in float scale)
//{
//	float3 dpdx = dFdx(pos);
//	float3 dpdy = dFdy(pos);
    
//	float dbdx = dFdx(signal);
//	float dbdy = dFdy(signal);

//	float3 u = cross(dpdy, nor);
//	float3 v = cross(nor, dpdx);
//	float d = dot(dpdx, u);
	
//	float3 surfGrad = dbdx * u + dbdy * v;
//	return normalize(abs(d) * nor - scale * surfGrad);
//}




//float4 texcube(sampler2D sam, in float3 p, in float3 n)
//{
//	float4 x = texture(sam, p.yz);
//	float4 y = texture(sam, p.zx);
//	float4 z = texture(sam, p.xy);
//	return x * abs(n.x) + y * abs(n.y) + z * abs(n.z);
//}


//void calcCamera(out float3 ro, out float3 ta)
//{
//	float an = 3.1 + 0.25 * iTime;
//	ro = float3(2.5 * cos(an), 1.0, 2.5 * sin(an));
//	ta = float3(0.0, 1.0, 0.0);
//}

//void calcRayForPixel(in float2 pix, out float3 resRo, out float3 resRd)
//{
//	float2 p = (2.0 * pix - iResolution.xy) / iResolution.y;
	
//     // camera movement	
//	float3 ro, ta;
//	calcCamera(ro, ta);
//    // camera matrix
//	float3 ww = normalize(ta - ro);
//	float3 uu = normalize(cross(ww, float3(0.0, 1.0, 0.0)));
//	float3 vv = normalize(cross(uu, ww));
//	// create view ray
//	float3 rd = normalize(p.x * uu + p.y * vv + 1.5 * ww);
	
//	resRo = ro;
//	resRd = rd;
//}

//void mainImage(out float4 fragColor, in float2 fragCoord)
//{
//	float bump = smoothstep(-0.8, -0.7, cos(0.5 * iTime));

//	float3 ro, rd, ddx_ro, ddx_rd, ddy_ro, ddy_rd;
//	calcRayForPixel(fragCoord + float2(0.0, 0.0), ro, rd);
//	calcRayForPixel(fragCoord + float2(1.0, 0.0), ddx_ro, ddx_rd);
//	calcRayForPixel(fragCoord + float2(0.0, 1.0), ddy_ro, ddy_rd);

    
//    // sphere center	
//	float3 sc = float3(0.0, 1.0, 0.0);

//	float3 mate = float3(0.0);
	
//    // raytrace
//	float tmin = 10000.0;
//	float3 nor = float3(0.0);
//	float occ = 1.0;
//	float3 pos = float3(0.0);
	
//	// raytrace-plane
//	float h = (0.0 - ro.y) / rd.y;
//	if (h > 0.0)
//	{
//		tmin = h;
//		nor = float3(0.0, 1.0, 0.0);
//		pos = ro + h * rd;
		
//		float3 di = sc - pos;
//		float l = length(di);
		
     
//		mate = texture(iChannel0, 0.25 * pos.zx, .1 * l).xyz;
//		float signal = dot(mate, float3(0.33));
//		nor = doBump(pos, nor, signal, 0.15 * bump);
  

//		occ = 1.0; // - dot(nor,di/l)*1.0*1.0/(l*l); 
//	}

	
//    // shading/lighting	
//	float3 col = float3(0.9);
//	if (tmin < 100.0)
//	{
//		pos = ro + tmin * rd;
		
//		float sh = 1.5;
//		float3 lin = float3(0.8, 0.7, 0.6) * sh * clamp(dot(nor, float3(0.57703)), 0.0, 1.0);
		  
//		lin += sh * 1.0 * pow(clamp(dot(reflect(rd, nor), float3(0.57703)), 0.0, 1.0), 12.0);
//		col = mate * lin;
//	//	col = mix( col, float3(0.9), 1.0-exp( -0.003*tmin*tmin ) );
//	}
	

	
//	fragColor = float4(col, 1.0);
//}
