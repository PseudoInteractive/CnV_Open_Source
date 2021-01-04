#include "Macros.fxh"
#include "Structures.fxh"

DECLARE_TEXTURE(Texture, 0);

BEGIN_CONSTANTS



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

float4 PixelShaderFunctionPositionColorTexture(VertexShaderOutputPositionColorTexture input) : SV_Target0
{
	float4 textureColor = SAMPLE_TEXTURE(Texture, input.TextureCoordinate);
	
	textureColor *= input.Color ;

	return textureColor;
}

TECHNIQUE(PositionColorTexture, VertexShaderFunctionPositionColorTexture, PixelShaderFunctionPositionColorTexture);
