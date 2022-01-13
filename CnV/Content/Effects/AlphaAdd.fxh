#include "Constants.fxh"


float4 PSAlphaAdd(VertexShaderOutputPositionColorTexture input) : SV_Target0
{
	float4 textureColor = SAMPLE_TEXTURE(Texture, input.TextureCoordinate);
	
	textureColor *= input.Color ;

	return textureColor;
}

TECHNIQUE(AlphaAdd, VSDefault, PSAlphaAdd);
