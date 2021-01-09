#include "Constants.fxh"



float4 PSDarkFont(VertexShaderOutputPositionColorTexture input) : SV_Target0
{
	float4 textureColor = SAMPLE_TEXTURE(Texture, input.TextureCoordinate);
//	float  shadow  = SAMPLE_TEXTURE(Texture, input.TextureCoordinate- float2(1.5/128, 1.5/64.0) );
	float alpha = saturate(3 * (textureColor.a - 0.125) );
	textureColor.rgb = alpha;
	textureColor.a = (alpha);
	return textureColor * input.Color ;

}


TECHNIQUE(FontDark, VSDefault, PSDarkFont);
