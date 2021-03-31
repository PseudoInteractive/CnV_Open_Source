#include "Constants.fxh"

VertexShaderOutputLit VSLit(VertexShaderInputPositionColorTexture input)
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
	
	output.cameraC = output.Position.xyw;
	return output;
}
float AddSmooth(float a, float b)
{
	return a + b - a * b;
}
float3 AddSmooth3(float3 a, float3 b)
{
	return a + b;//	-a * b;
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

static const half smoothness = 0.25;
static const half perceptualRoughness = (1.0 - smoothness); // .5
static const half roughness = perceptualRoughness * perceptualRoughness; // .25
static const half alphaSq = __sqr(__sqr(roughness));


half BRDFSpec(half nh, half lh)
{
	half vf =  (nh * nh) * (alphaSq - 1) + 1;
	return alphaSq / (4 * 3*__sqr(vf) * __sqr(lh) * (roughness + 0.5));

}


half4 PSLit(VertexShaderOutputLit input) : SV_Target0
{
	half4 normalTex = SAMPLE_TEXTURE(Normal, input.uv.xy);
	half4 tex = SAMPLE_TEXTURE(Texture, input.uv.xy);
	half3 albedo = tex.rgb;
	half3 positionInPixels = input.Position.xyz;
	half3 Li = normalize(lightPosition.xyz - positionInPixels.xyz);
	half3 Lo = normalize(cameraReferencePosition - positionInPixels.xyz);
	half alpha = input.Color.a * tex.a;

//	(zx, zy, 1)x(0, 1, 0) = 1,0,-zx
//	(zx, zy, 1)x(1, 0, 0) = 0,1, -zy
//	normal.x = normalTex.a - 0.5;
//	normal.y = normalTex.y- 0.5;
//	normal.z = 0.5;
//	float3 x = float3(1, 0, -zx);
//	float3 y = float3(0, 1, -zy);
//	float3 z = float3(z.x, z.y, 1);
	half3 tN = normalTex.rgb - 0.5;
	half3 N = half3(tN.z * input.uv.z + tN.x,
							tN.z * input.uv.w + tN.y, 
							tN.z - tN.x*input.uv.z - tN.y*input.uv.w);
	N = normalize(N);
	//N.x *= -1;
	

	
	half ao = saturate(4 *( max(max(albedo.r, albedo.g), albedo.b) - 0.125) );
                 
		// Half-vector between Li and Lo.
	half3 Lh = normalize(Li + Lo);
		// Calculate angles between surface normal and various light vectors.
	half cosLi = max(0.0, dot(N, Li));
	half cosLh = max(0.0, dot(N, Lh));

		// Lambert diffuse BRDF.
		// We don't scale by 1/PI for lighting & material units to be more convenient.
	half3 diffuse = albedo * (cosLi * lightColor.rgb + lightAmbient.rgb);

		// Cook-Torrance specular microfacet BRDF.
	half3 specular = BRDFSpec(cosLh, 1) * ao * lightSpecular.rgb;
	//(F * D * G*ao) / max(Epsilon, 4.0 * cosLi * cosLo);

		// Total contribution for this light.
	half3 rgb = (diffuse + specular) * input.Color.rgb;

//rgb = 2 * rgb / (1 + rgb);
	return half4(rgb * (alpha), alpha);

}


TECHNIQUE(Lit, VSLit, PSLit);
