#include "Constants.fxh"

VertexShaderOutputPositionColorTexture VSDefault(VertexShaderInputPositionColorTexture input)
{
	VertexShaderOutputPositionColorTexture output;
	output.Position = mul(input.Position, WorldViewProjection);
	output.Color = input.Color;
	output.TextureCoordinate = input.TextureCoordinate;
	return output;
}


float4 PSAlphaBlend(VertexShaderOutputPositionColorTexture input) : SV_Target0
{
	float4 textureColor = SAMPLE_TEXTURE(Texture, input.TextureCoordinate);
	
	textureColor *= input.Color ;
	textureColor.rgb *= textureColor.a;// pre multiply
	return textureColor;
}

TECHNIQUE(AlphaBlend, VSDefault,PSAlphaBlend);
