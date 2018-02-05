#if OPENGL
#define SV_POSITION POSITION
#define UORS static
#define VERTEX_SHADER_VERSION vs_3_0
#define PIXEL_SHADER_VERSION  ps_3_0
#else
#define SV_POSITION POSITION
#define UORS uniform
#define VERTEX_SHADER_VERSION vs_4_0_level_9_1
#define PIXEL_SHADER_VERSION  ps_4_0_level_9_1
#endif

//-----------------------------------------------------------------------------
// BasicEffect.fx
//
// This is a simple shader that supports 1 ambient and 3 directional lights.
// All lighting computations happen in world space.
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------


//-----------------------------------------------------------------------------
// Texture sampler
//-----------------------------------------------------------------------------

uniform const texture DayTexture;
uniform const texture NightTexture;

uniform const sampler DayTextureSampler = sampler_state
{
  Texture = (DayTexture);
  MipFilter = Linear;
  MinFilter = Linear;
  MagFilter = Linear;
};

uniform const sampler NightTextureSampler = sampler_state
{
  Texture = (NightTexture);
  MipFilter = Linear;
  MinFilter = Linear;
  MagFilter = Linear;
};

uniform const float3	EyePosition;	// in world space


//-----------------------------------------------------------------------------
// Material settings
//-----------------------------------------------------------------------------

UORS const float3 DiffuseColor  = 1;
UORS const float  Alpha         = 1;
UORS const float3 EmissiveColor = 0;
UORS const float3 SpecularColor = 1;
UORS const float  SpecularPower = 16;


//-----------------------------------------------------------------------------
// Lights
// All directions and positions are in world space and must be unit vectors
//-----------------------------------------------------------------------------

uniform const float3 AmbientLightColor;

uniform const float3 DirLight0Direction;
uniform const float3 DirLight0DiffuseColor;
uniform const float3 DirLight0SpecularColor;

uniform const float3 DirLight1Direction;
uniform const float3 DirLight1DiffuseColor;
uniform const float3 DirLight1SpecularColor;

uniform const float3 DirLight2Direction;
uniform const float3 DirLight2DiffuseColor;
uniform const float3 DirLight2SpecularColor;

//-----------------------------------------------------------------------------
// Matrices
//-----------------------------------------------------------------------------

uniform const float4x4 World;
uniform const float4x4 View;
uniform const float4x4 Projection;


//-----------------------------------------------------------------------------
// Structure definitions
//-----------------------------------------------------------------------------

struct ColorPair
{
  float3 Diffuse;
  float3 Specular;
};

//-----------------------------------------------------------------------------
// Shader I/O structures
// Nm: Normal
// Tx: Texture
// Vc: Vertex color
//
// Nm Tx Vc
//  0  0  0	VSInput
//  0  0  1 VSInputVc
//  0  1  0 VSInputTx
//  0  1  1 VSInputTxVc
//  1  0  0 VSInputNm
//  1  0  1 VSInputNmVc
//  1  1  0 VSInputNmTx
//  1  1  1 VSInputNmTxVc


//-----------------------------------------------------------------------------
// Vertex shader inputs
//-----------------------------------------------------------------------------
struct VSInputNmTx
{
  float4 Position : SV_POSITION;
  float2 TexCoord : TEXCOORD0;
  float3 Normal   : NORMAL;
};

//-----------------------------------------------------------------------------
// Vertex shader outputs
//-----------------------------------------------------------------------------
struct PixelLightingVSOutputTx
{
  float4 PositionPS : SV_POSITION;		// Position in projection space
  float2 TexCoord   : TEXCOORD0;
  float4 PositionWS : TEXCOORD1;
  float3 NormalWS   : TEXCOORD2;
  float4 Diffuse    : COLOR0;		// diffuse.rgb and alpha
};

//-----------------------------------------------------------------------------
// Pixel shader inputs
//-----------------------------------------------------------------------------
struct PixelLightingPSInputTx
{
  float2 TexCoord   : TEXCOORD0;
  float4 PositionWS : TEXCOORD1;
  float3 NormalWS   : TEXCOORD2;
  float4 Diffuse: COLOR0;		// diffuse.rgb and alpha
};

//-----------------------------------------------------------------------------
// Compute lighting
// E: Eye-Vector
// N: Unit vector normal in world space
//-----------------------------------------------------------------------------
ColorPair ComputeLights( float3 E, float3 N )
{
  ColorPair result;

  result.Diffuse  = AmbientLightColor;
  result.Specular = 0;

  // Directional Light 0
  float3 L         = -DirLight0Direction;
  float3 H         = normalize( E + L );
  float2 ret       = lit( dot( N, L ), dot( N, H ), SpecularPower ).yz;
  result.Diffuse  += DirLight0DiffuseColor  * ret.x;
  result.Specular += DirLight0SpecularColor * ret.y;

  // Directional Light 1
  L                = -DirLight1Direction;
  H                = normalize( E + L );
  ret              = lit( dot( N, L ), dot( N, H ), SpecularPower ).yz;
  result.Diffuse  += DirLight1DiffuseColor  * ret.x;
  result.Specular += DirLight1SpecularColor * ret.y;

  // Directional Light 2
  L                = -DirLight2Direction;
  H                = normalize( E + L );
  ret              = lit( dot( N, L ), dot( N, H ), SpecularPower ).yz;
  result.Diffuse  += DirLight2DiffuseColor  * ret.x;
  result.Specular += DirLight2SpecularColor * ret.y;		

  result.Diffuse  *= DiffuseColor;
  result.Diffuse  += EmissiveColor;
  result.Specular *= SpecularColor;

  return result;
}

//-----------------------------------------------------------------------------
// Compute per-pixel lighting.
// When compiling for pixel shader 2.0, the lit intrinsic uses more slots
// than doing this directly ourselves, so we don't use the intrinsic.
// E: Eye-Vector
// N: Unit vector normal in world space
//-----------------------------------------------------------------------------
ColorPair ComputePerPixelLights( float3 E, float3 N )
{
  ColorPair result;

  result.Diffuse = AmbientLightColor;
  result.Specular = 0;

  // Light0
  float3 L = -DirLight0Direction;
  float3 H = normalize( E + L );
  float dt = max(0,dot( L, N ) );
  result.Diffuse += DirLight0DiffuseColor * dt;

  if ( dt != 0 )
  {
    result.Specular += DirLight0SpecularColor * pow( max( 0 , dot( H, N ) ), SpecularPower );
  }

  // Light1
  L = -DirLight1Direction;
  H = normalize( E + L );
  dt = max( 0, dot( L, N) );
  result.Diffuse += DirLight1DiffuseColor * dt;

  if ( dt != 0 )
  {
    result.Specular += DirLight1SpecularColor * pow( max( 0, dot( H, N ) ), SpecularPower );
  }

  // Light2
  L = -DirLight2Direction;
  H = normalize( E + L );
  dt = max(0,dot( L, N ) );
  result.Diffuse += DirLight2DiffuseColor * dt;

  if ( dt != 0 )
  {
    result.Specular += DirLight2SpecularColor * pow( max( 0, dot( H, N ) ), SpecularPower );
  }

  result.Diffuse  *= DiffuseColor;
  result.Diffuse  += EmissiveColor;
  result.Specular *= SpecularColor;
		
  return result;
}

float ComputeTxBlend( float3 normal )
{
  float pct = dot(-normal, DirLight0Direction);
  pct = saturate(pct * 5 + .5);
  pct = sqrt(pct);

  return pct;
}


//-----------------------------------------------------------------------------
// Per-pixel lighting vertex shaders
//-----------------------------------------------------------------------------
PixelLightingVSOutputTx VSBasicPixelLightingNmTx( VSInputNmTx vin_ )
{
  PixelLightingVSOutputTx vout;

  float4 pos_ws = mul( vin_.Position, World );
  float4 pos_vs = mul( pos_ws, View );
  float4 pos_ps = mul( pos_vs, Projection );

  vout.PositionPS     = pos_ps;
  vout.PositionWS.xyz = pos_ws.xyz;
  vout.NormalWS       = normalize( mul( float4( vin_.Normal, 1 ), World ) ).xyz;
  vout.PositionWS.w   = ComputeTxBlend( vout.NormalWS );
  vout.Diffuse        = float4( 1, 1, 1, Alpha );
  vout.TexCoord       = vin_.TexCoord;
	
  return vout;
}

//-----------------------------------------------------------------------------
// Pixel shaders
//-----------------------------------------------------------------------------
float4 PSBasicPixelLightingTx( PixelLightingPSInputTx pin ) : COLOR
{
  float3 posToEye = EyePosition - pin.PositionWS.xyz;

  float3 N = normalize( pin.NormalWS );
  float3 E = normalize( posToEye );

  ColorPair lightResult = ComputePerPixelLights( E, N );

  float4 c1  = tex2D( DayTextureSampler,   pin.TexCoord );
  float4 c2  = tex2D( NightTextureSampler, pin.TexCoord );
  float  pct = pin.PositionWS.w;

  float4 diffuse = ( c1 * pct + c2 * ( 1.0 - pct ) ) * float4( lightResult.Diffuse * pin.Diffuse.rgb, pin.Diffuse.a );
  float4 color   = diffuse + float4( lightResult.Specular, 0 );

  return color;
}

//-----------------------------------------------------------------------------
// Shader and technique definitions
//-----------------------------------------------------------------------------
Technique NightDayEffect0
{
  pass
  {
    VertexShader = compile VERTEX_SHADER_VERSION VSBasicPixelLightingNmTx( );
    PixelShader  = compile PIXEL_SHADER_VERSION  PSBasicPixelLightingTx(   );
  }
}
