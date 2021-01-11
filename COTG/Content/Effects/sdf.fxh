#include "Constants.fxh"

half4 PSSDF(VertexShaderOutputPositionColorTexture input) : SV_Target0
{
//	float4 textureColor = SAMPLE_TEXTURE(Texture, input.TextureCoordinate);
////	float  shadow  = SAMPLE_TEXTURE(Texture, input.TextureCoordinate- float2(1.5/128, 1.5/64.0) );
//	float alpha = textureColor.a;
//	alpha = saturate((alpha - 0.5) * 4 + 0.5);
//	textureColor.rgb = input.Color * alpha;
//	textureColor.a = alpha;
	half2 uv = input.TextureCoordinate;
	half dxdu = 1 / ddx(uv.x);
	half dydv = 1 / ddy(uv.y);
	half alpha = saturate(min(min((uv.x) * dxdu, (1 - uv.x) * dxdu),
						      min((uv.y) * dydv, (1 - uv.y) * dydv)) * 0.5) * input.Color.a; // 2 pixel fade
	
	
	
	////	float  shadow  = SAMPLE_TEXTURE(Texture, input.TextureCoordinate- float2(1.5/128, 1.5/64.0) );
//	float alpha = textureColor.a;
//	alpha = saturate((alpha - 0.5) * 4 + 0.5);
//	textureColor.rgb = input.Color * alpha;
//	textureColor.a = alpha;
	return (input.Color.rgb*alpha, alpha);
	

}

VertexShaderOutputPositionColorTexture VSSDF(VertexShaderInputPositionColorTexture input)
{
	VertexShaderOutputPositionColorTexture output;
	output.Position = mul(input.Position, WorldViewProjection);
	output.Color = input.Color;
	output.TextureCoordinate = input.TextureCoordinate;//	(input.TextureCoordinate - 0.5) * (58.0 / 64.0) + 0.5;
	return output;
}

TECHNIQUE(SDF, VSSDF, PSSDF);
