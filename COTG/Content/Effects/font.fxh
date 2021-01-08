#include "Constants.fxh"

float4 PSLightFont(VertexShaderOutputPositionColorTexture input) : SV_Target0
{
	float4 textureColor = SAMPLE_TEXTURE(Texture, input.TextureCoordinate);
//	float  shadow  = SAMPLE_TEXTURE(Texture, input.TextureCoordinate- float2(1.5/128, 1.5/64.0) );
	float alpha = textureColor.a;
	textureColor.rgb = saturate( 2*(alpha)-0.375);
	textureColor.a = sqrt(alpha);
	return textureColor * input.Color ;

}


TECHNIQUE(FontLight, VSDefault, PSLightFont);
