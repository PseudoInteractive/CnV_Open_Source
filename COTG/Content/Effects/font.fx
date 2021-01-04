#include "Macros.fxh"
#include "Structures.fxh"

DECLARE_TEXTURE(Texture, 0);

BEGIN_CONSTANTS



MATRIX_CONSTANTS


END_CONSTANTS



VertexShaderOutputPositionColorTexture FontVertexShader(VertexShaderInputPositionColorTexture input)
{
	VertexShaderOutputPositionColorTexture output;
	output.Position = mul(input.Position, WorldViewProjection);
	output.Color = input.Color;
	output.TextureCoordinate = input.TextureCoordinate;
	return output;
}

float4 FontPixelShader(VertexShaderOutputPositionColorTexture input) : SV_Target0
{
	float4 textureColor = SAMPLE_TEXTURE(Texture, input.TextureCoordinate);
//	float  shadow  = SAMPLE_TEXTURE(Texture, input.TextureCoordinate- float2(1.5/128, 1.5/64.0) );
	float alpha = textureColor.a;
	textureColor.rgb = saturate( 2*(alpha)-0.375);
	textureColor.a = sqrt(alpha);
	return textureColor * input.Color ;

}


TECHNIQUE(PositionColorTexture, FontVertexShader, FontPixelShader);
