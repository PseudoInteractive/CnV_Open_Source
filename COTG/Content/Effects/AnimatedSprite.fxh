#include "Constants.fxh"


static const float spriteSheetDu = 1.0 / 12.0;

float4 PSSpriteAnim(VertexShaderOutputPositionColorTexture input) : SV_Target0
{
	float2 uv0 = input.TextureCoordinate.xy;
	float2 uv1 = float2((uv0.x + spriteSheetDu)%1.0, uv0.y);
	float4 t0 = SAMPLE_TEXTURE(Texture, uv0);
	float4 t1 = SAMPLE_TEXTURE(Texture, uv1);
	
	float4 tX = lerp(t0, t1, input.Color.r); // blend between frames
	tX.rgb *= tX.a;
	tX.a *= input.Color.a;
	return tX;
}

TECHNIQUE(SpriteAnim, VSDefault, PSSpriteAnim);
