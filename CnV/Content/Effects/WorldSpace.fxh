#include "Constants.fxh"

VertexShaderOutputWorld VSWorld(VertexShaderInputPositionTexture input)
{
	float4 cameraPosition;
	cameraPosition.z = input.Position.z - cameraC.z;
	cameraPosition.w = 1;
	cameraPosition.xy = (input.Position.xy - cameraC.xy)*pixelScale.x;
	float2 posGain = cameraPosition.xy;
	float dz = planetGains.x + dot(posGain, posGain)*planetGains.y;
	//z = (x*x + y*y)* planetGains.x + planeyGains.y
	//d/dx z = (2*x)* planetGains.x
	cameraPosition.z += dz;

	VertexShaderOutputWorld output;

	output.Position = mul(cameraPosition, WorldViewProjection);
	output.uv.xy = input.TextureCoordinate.xy;
	
	
	return output;
}


half4 PSWorld(VertexShaderOutputWorld input) : SV_Target0
{
	float4 result;
	result.rgb= SAMPLE_TEXTURE(Texture, input.uv).rgb*0.75;
	result.a = (result.r + result.g + result.b)*0.375;
	
	return result;
}


TECHNIQUE(WorldSpace, VSWorld, PSWorld);
