#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

float4x4 p_World;
float4x4 p_View;
float4x4 p_Projection;

float4 p_AmbientColor = float4( 1, 1, 1, 1 );
float  p_AmbientIntensity = 0.5;

struct VertexShaderInput
{
	float4 m_Position : POSITION0;
};


struct VertexShaderOutput
{
	float4 m_Position : POSITION0;
};


VertexShaderOutput VertexShaderFunction( in VertexShaderInput input_ )
{
	VertexShaderOutput output;

  float4 worldPosition = mul( input_.m_Position, p_World );
  float4 viewPosition  = mul( worldPosition, p_View );

	output.m_Position = mul( viewPosition, p_Projection );

	return output;
}

float4 PixelShaderFunction( VertexShaderOutput input_ ) : COLOR0
{
	return p_AmbientColor * p_AmbientIntensity;
}

technique Ambient
{
	pass Pass1
	{
		VertexShader = compile VS_SHADERMODEL VertexShaderFunction( );
		PixelShader  = compile PS_SHADERMODEL PixelShaderFunction( );
	}
};
