#if SM4
#define VERTEX_SHADER_VERSION vs_4_0_level_9_1
#define PIXEL_SHADER_VERSION  ps_4_0_level_9_1
#elif SM3
#define VERTEX_SHADER_VERSION vs_3_0
#define PIXEL_SHADER_VERSION  ps_3_0
#else
#define VERTEX_SHADER_VERSION vs_2_0
#define PIXEL_SHADER_VERSION  ps_2_0
#endif

//texture particleTexture;
float4x4 World;
float4x4 View;
float4x4 Proj;
float4   LineColor;

struct VS_INPUT
{
	float4 Position : SV_POSITION;
	//float4 EndPos : POSITION1;
	//float4 Color : COLOR0;
	//float4 TexCoord : TEXCOORD0;
	//float3 TestVal : TEXCOORD1;
};

struct PS_INPUT
{
	float4 Position : SV_POSITION;
	//float4 Color : COLOR0;
	//float2 TexCoord : TEXCOORD0;
};

//sampler Sampler = sampler_state
//{
	//Texture = <particleTexture>;
//};

PS_INPUT VertexShaderVS( VS_INPUT input_ )
{
	PS_INPUT Output;

	float4 pos_ws = mul( input_.Position, World );
	float4 pos_vs = mul( pos_ws, View );
	float4 pos_ps = mul( pos_vs, Proj );
		
	Output.Position = pos_ps;
	//Output.TexCoord = input.TexCoord.xy;
	//Output.Color = input.Color;
	return Output;
}

float4 PixelShaderPS( PS_INPUT input_ ) : COLOR0
{
	//float2 texCoord;
	//texCoord = input.TexCoord.xy;
	float4 Color = LineColor; // input.Color; // tex2D(Sampler, texCoord);
	//Color *= input.Color;
	return Color;	
}

technique PointSpriteTechnique
{
    pass P0
    {
        vertexShader = compile VERTEX_SHADER_VERSION VertexShaderVS( );
        pixelShader  = compile PIXEL_SHADER_VERSION  PixelShaderPS(  );
    }
}
