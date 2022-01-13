#include "Constants.fxh"

VertexShaderOutputPositionColor VSNoTexture(VertexShaderInputPositionColorTexture input)
{
	VertexShaderOutputPositionColor output;
	output.Position = mul(input.Position, WorldViewProjection);
	output.Color = input.Color;
	return output;
}


float4 PSNoTexture(VertexShaderOutputPositionColor input) : SV_Target0
{
	
	return input.Color;
}

TECHNIQUE(NoTexture, VSNoTexture, PSNoTexture);
