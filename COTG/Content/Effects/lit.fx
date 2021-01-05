#include "Macros.fxh"
#include "Structures.fxh"

DECLARE_TEXTURE(Texture, 0);
DECLARE_TEXTURE(Normal, 1);

BEGIN_CONSTANTS

float3 lightPosition;
float3 cameraPosition;
float4 lightGains; // emissive, diffuse speciular



MATRIX_CONSTANTS


END_CONSTANTS




VertexShaderOutputPositionColorTexture VertexShaderFunctionPositionColorTexture(VertexShaderInputPositionColorTexture input)
{
	VertexShaderOutputPositionColorTexture output;
	output.Position = mul(input.Position, WorldViewProjection);
	output.Color = input.Color;
	output.TextureCoordinate = input.TextureCoordinate;
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

float4 PixelShaderFunctionPositionColorTexture(VertexShaderOutputPositionColorTexture input) : SV_Target0
{
	float4 normalTex = SAMPLE_TEXTURE(Normal, input.TextureCoordinate);
	float4 tex = SAMPLE_TEXTURE(Texture, input.TextureCoordinate);
	
	float3 positionInPixels = input.Position.xyz;
	float3 lightDir = normalize(positionInPixels.xyz - lightPosition.xyz);
	float3 viewDir = normalize(positionInPixels.xyz - cameraPosition);

	float3 normal;
//	normal.x = normalTex.a - 0.5;
//	normal.y = normalTex.y- 0.5;
//	normal.z = 0.5;
	normal = normalTex.rgb - 0.5;
	normal = normalize(normal);
	float3 specDir = (viewDir - dot(viewDir, normal) * 2 * normal);

	float specDot =saturate( -dot(specDir, lightDir));
	float specColor = saturate(4*dot(tex.rgb,tex.rgb));

	float spec = pow( specDot,6) * lightGains.z* specColor;

	float diff = AddSmooth(-dot(lightDir, normal)*lightGains.y, lightGains.x);
	float3 base = diff * tex.rgb * input.Color.rgb ;

	float3 rgb = AddSmooth31(base, spec) ;
	float alpha = input.Color.a * tex.a;
	//rgb = 2 * rgb / (1 + rgb);
	return float4(rgb* (alpha*lightGains.w), alpha);

}

TECHNIQUE(PositionColorTexture, VertexShaderFunctionPositionColorTexture, PixelShaderFunctionPositionColorTexture);
