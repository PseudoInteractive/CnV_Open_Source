#include "Macros.fxh"
#include "Structures.fxh"

DECLARE_TEXTURE(Texture, 0);
DECLARE_TEXTURE(Normal, 1);

BEGIN_CONSTANTS

float4 planetGains; // emissive, diffuse specular

MATRIX_CONSTANTS


END_CONSTANTS




VertexShaderOutputPositionColorTexture VertexShaderFunctionPositionColorTexture(VertexShaderInputPositionColorTexture input)
{
	VertexShaderOutputPositionColorTexture output;
	float4 cameraPosition = input.Position;
	float2 posGain = cameraPosition.xy;
	float dz = planetGains.x + dot(posGain, posGain)*planetGains.y;
	//z = (x*x + y*y)* planetGains.x + planeyGains.y
	//d/dx z = (2*x)* planetGains.x
	cameraPosition.z += dz;
	output.Position = mul(cameraPosition, WorldViewProjection);
	output.Color = input.Color;
	output.TextureCoordinate.xy = input.TextureCoordinate.xy;
	
	return output;
}

float4 PixelShaderFunctionPositionColorTexture(VertexShaderOutputPositionColorTexture input) : SV_Target0
{
	float4 textureColor = SAMPLE_TEXTURE(Texture, input.TextureCoordinate);
	
	textureColor *= input.Color;
	textureColor.rgb *= textureColor.a; // pre multiply
	return textureColor;

}

TECHNIQUE(PositionColorTexture, VertexShaderFunctionPositionColorTexture, PixelShaderFunctionPositionColorTexture);
