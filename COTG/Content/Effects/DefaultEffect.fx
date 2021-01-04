#include "Macros.fxh"
#include "Structures.fxh"

DECLARE_TEXTURE(Texture, 0);

BEGIN_CONSTANTS



MATRIX_CONSTANTS



END_CONSTANTS

VertexShaderOutputPosition VertexShaderFunctionPosition(VertexShaderInputPosition input)
{
	VertexShaderOutputPosition output;
	output.Position = mul(input.Position, WorldViewProjection);
	return output;
}

float4 PixelShaderFunctionPosition(VertexShaderOutputPosition input) : SV_Target0
{
	return float4(1,1,1,1);
}

VertexShaderOutputPositionTexture VertexShaderFunctionPositionTexture(VertexShaderInputPositionTexture input)
{
	VertexShaderOutputPositionTexture output;
	output.Position = mul(input.Position, WorldViewProjection);
	output.TextureCoordinate = input.TextureCoordinate;
	return output;
}

float4 PixelShaderFunctionPositionTexture(VertexShaderOutputPositionTexture input) : SV_Target0
{
	return SAMPLE_TEXTURE(Texture, input.TextureCoordinate) ;
}

VertexShaderOutputPositionColor VertexShaderFunctionPositionColor(VertexShaderInputPositionColor input)
{
	VertexShaderOutputPositionColor output;
	output.Position = mul(input.Position, WorldViewProjection);
	output.Color = input.Color;
	return output;
}

float4 PixelShaderFunctionPositionColor(VertexShaderOutputPositionColor input) : SV_Target0
{
	return input.Color ;
}

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
	textureColor.rgb *= textureColor.a;// pre multiply
	return textureColor;
}

TECHNIQUE(PositionColorTexture, VertexShaderFunctionPositionColorTexture, PixelShaderFunctionPositionColorTexture);
TECHNIQUE(Position, VertexShaderFunctionPosition, PixelShaderFunctionPosition);
TECHNIQUE(PositionTexture, VertexShaderFunctionPositionTexture, PixelShaderFunctionPositionTexture);
TECHNIQUE(PositionColor, VertexShaderFunctionPositionColor, PixelShaderFunctionPositionColor);
