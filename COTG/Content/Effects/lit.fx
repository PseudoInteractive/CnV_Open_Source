#include "Macros.fxh"
#include "Structures.fxh"

DECLARE_TEXTURE(Texture, 0);
DECLARE_TEXTURE(Normal, 1);

BEGIN_CONSTANTS

float3 lightPosition;
float3 cameraPosition;
float4 lightGains; // emissive, diffuse speciular
float4 planetGains; // emissive, diffuse speciular
float4 lightAmbient;
float4 lightColor;


MATRIX_CONSTANTS


END_CONSTANTS




VertexShaderOutputLit VertexShaderFunctionPositionColorTexture(VertexShaderInputPositionColorTexture input)
{
	VertexShaderOutputLit output;
	float4 cameraPosition = input.Position;
	float2 posGain = cameraPosition.xy;
	float dz = planetGains.x + dot(posGain, posGain)*planetGains.y;
	//z = (x*x + y*y)* planetGains.x + planeyGains.y
	//d/dx z = (2*x)* planetGains.x
	cameraPosition.z += dz;
	output.Position = mul(cameraPosition, WorldViewProjection);
	output.Color = input.Color;
	output.uv.xy = input.TextureCoordinate.xy;
	
	output.uv.z = cameraPosition.x * (planetGains.z);
	output.uv.w = cameraPosition.y * (planetGains.z);
	return output;
}
float AddSmooth(float a, float b)
{
	return a + b - a * b;
}
float3 AddSmooth3(float3 a, float3 b)
{
	return a + b - a * b;
}
float3 AddSmooth31(float3 a, float b)
{
	return a + b - a * b;
}
inline half FresnelLerp(half F0, half F90, half cosA)
{
       //half t = Pow5 (1 - cosA);   // ala Schlick interpoliation
             //   half t = pow(1 - cosA, _FresnelPower);
	half t = pow(1 - cosA,5);
	return lerp(F0, F90, t);
}

static const half smoothness = 0.32;
static const half perceptualRoughness = (1.0 - smoothness); // .5
static const half roughness = perceptualRoughness * perceptualRoughness; // .25
static const float3 Fdielectric = 0.04;
//static const float metalness = 0.25;
static const float gaSchlickGGXK = __sqr(roughness + 1.0) / 8.0;
static const float alphaSq = __sqr(__sqr(roughness));

// GGX/Towbridge-Reitz normal distribution function.
// Uses Disney's reparametrization of alpha = roughness^2.
float ndfGGX(float cosLh)
{

	float denom = (cosLh * cosLh) * (alphaSq - 1.0) + 1.0;
	return alphaSq / (PI * denom * denom);
}



// Single term for separable Schlick-GGX below.
float gaSchlickG1(float cosTheta)
{
	return cosTheta / (cosTheta * (1.0 - gaSchlickGGXK) + gaSchlickGGXK);
}

// Schlick-GGX approximation of geometric attenuation function using Smith's method.
float gaSchlickGGX(float cosLi, float cosLo)
{
	return gaSchlickG1(cosLi) * gaSchlickG1(cosLo);
}

// Shlick's approximation of the Fresnel factor.
float3 fresnelSchlick(float3 F0, float cosTheta)
{
	return F0 + (1.0 - F0) * pow(1.0 - cosTheta, 3.0);
}

float4 PixelShaderFunctionPositionColorTexture(VertexShaderOutputLit input) : SV_Target0
{
	float4 normalTex = SAMPLE_TEXTURE(Normal, input.uv.xy);
	float4 tex = SAMPLE_TEXTURE(Texture, input.uv.xy);
	float3 albedo = tex.rgb ;
	float3 positionInPixels = input.Position.xyz;
	float3 Li = normalize(lightPosition.xyz - positionInPixels.xyz);
	float3 Lo = normalize(cameraPosition - positionInPixels.xyz);
	float alpha = input.Color.a * tex.a;

//	(zx, zy, 1)x(0, 1, 0) = 1,0,-zx
//	(zx, zy, 1)x(1, 0, 0) = 0,1, -zy
//	normal.x = normalTex.a - 0.5;
//	normal.y = normalTex.y- 0.5;
//	normal.z = 0.5;
//	float3 x = float3(1, 0, -zx);
//	float3 y = float3(0, 1, -zy);
//	float3 z = float3(z.x, z.y, 1);
	float3 tN = normalTex.rgb - 0.5;
	float3 N = float3(tN.z * input.uv.z + tN.x,
							tN.z * input.uv.w + tN.y, 
							tN.z - tN.x*input.uv.z - tN.y*input.uv.w);
	N = normalize(N);
	
	
	float metalness = lightGains.x;


//	float spec = pow( specDot,6) * lightGains.z* specColor;
	float3 F0 = lerp(Fdielectric, tex.rgb, metalness);
	
	float ao = saturate(6 * dot(albedo, albedo) - 0.125);
                 
		// Half-vector between Li and Lo.
	float3 Lh = normalize(Li + Lo);
	float cosLo = max(0.0, dot(N, Lo));
		// Calculate angles between surface normal and various light vectors.
	float cosLi = max(0.0, dot(N, Li));
	float cosLh = max(0.0, dot(N, Lh));

		// Calculate Fresnel term for direct lighting. 
	float3 F = fresnelSchlick(F0, max(0.0, dot(Lh, Lo)));
		// Calculate normal distribution for specular BRDF.
	float D = ndfGGX(cosLh);
		// Calculate geometric attenuation for specular BRDF.
	float G = gaSchlickGGX(cosLi, cosLo);

		// Diffuse scattering happens due to light being refracted multiple times by a dielectric medium.
		// Metals on the other hand either reflect or absorb energy, so diffuse contribution is always zero.
		// To be energy conserving we must scale diffuse BRDF contribution based on Fresnel factor & metalness.
	float3 kd = lerp(float3(1, 1, 1) - F, float3(0, 0, 0), metalness);

		// Lambert diffuse BRDF.
		// We don't scale by 1/PI for lighting & material units to be more convenient.
	float3 diffuseBRDF = kd * albedo;

		// Cook-Torrance specular microfacet BRDF.
	float3 specularBRDF = (F * D * G) / max(Epsilon, 4.0 * cosLi * cosLo)*ao;

		// Total contribution for this light.
	float3 rgb = ((diffuseBRDF + specularBRDF) *lightColor.rgb*cosLi + lightAmbient.rgb * albedo)  * input.Color.rgb;

//rgb = 2 * rgb / (1 + rgb);
	return float4(rgb* (alpha), alpha);

}

TECHNIQUE(PositionColorTexture, VertexShaderFunctionPositionColorTexture, PixelShaderFunctionPositionColorTexture);
