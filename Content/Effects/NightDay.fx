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

uniform const float3 DiffuseColor  = 1;
uniform const float  Alpha         = 1;
uniform const float3 EmissiveColor = 0;
uniform const float3 SpecularColor = 1;
uniform const float  SpecularPower = 16;


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

struct CommonVSOutput
{
  float4 Pos_ws;
  float4 Pos_ps;
  float4 Diffuse;
  float3 Specular;
  float  TxBlend;
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

struct VSInput
{
  float4 Position : SV_POSITION;
};

struct VSInputVc
{
  float4 Position : SV_POSITION;
  float4 Color    : COLOR;
};

struct VSInputNm
{
  float4 Position : SV_POSITION;
  float3 Normal   : NORMAL;
};

struct VSInputNmVc
{
  float4 Position : SV_POSITION;
  float3 Normal   : NORMAL;
  float4 Color    : COLOR;
};

struct VSInputTx
{
  float4 Position : SV_POSITION;
  float2 TexCoord : TEXCOORD0;
};

struct VSInputTxVc
{
  float4 Position : SV_POSITION;
  float2 TexCoord : TEXCOORD0;
  float4 Color    : COLOR;
};

struct VSInputNmTx
{
  float4 Position : SV_POSITION;
  float2 TexCoord : TEXCOORD0;
  float3 Normal   : NORMAL;
};

struct VSInputNmTxVc
{
  float4 Position : SV_POSITION;
  float2 TexCoord : TEXCOORD0;
  float3 Normal   : NORMAL;
  float4 Color    : COLOR;
};


//-----------------------------------------------------------------------------
// Vertex shader outputs
//-----------------------------------------------------------------------------

struct VertexLightingVSOutput
{
  float4 PositionPS : SV_POSITION;		// Position in projection space
  float4 Diffuse    : COLOR0;
  float4 Specular   : COLOR1;		// Specular.rgb and fog factor
};

struct VertexLightingVSOutputTx
{
  float4 PositionPS : SV_POSITION;		// Position in projection space
  float4 Diffuse    : COLOR0;
  float4 Specular   : COLOR1;
  float2 TexCoord   : TEXCOORD0;
  float2 TexCoord1  : TEXCOORD1;
};

struct PixelLightingVSOutput
{
  float4 PositionPS : SV_POSITION;		// Position in projection space
  float4 PositionWS : TEXCOORD0;
  float3 NormalWS   : TEXCOORD1;
  float4 Diffuse    : COLOR0;		// diffuse.rgb and alpha
};

struct PixelLightingVSOutputTx
{
  float4 PositionPS : SV_POSITION;		// Position in projection space
  float2 TexCoord   : TEXCOORD0;
	//float2	TexCoord1	: TEXCOORD1;
  float4 PositionWS : TEXCOORD1;
  float3 NormalWS   : TEXCOORD2;
  float4 Diffuse    : COLOR0;		// diffuse.rgb and alpha
};


//-----------------------------------------------------------------------------
// Pixel shader inputs
//-----------------------------------------------------------------------------

struct VertexLightingPSInput
{
  float4 Diffuse  : COLOR0;
  float4 Specular : COLOR1;
};

struct VertexLightingPSInputTx
{
  float4 Diffuse   : COLOR0;
  float4 Specular  : COLOR1;
  float2 TexCoord  : TEXCOORD0;
  float2 TexCoord1 : TEXCOORD1;
};

struct PixelLightingPSInput
{
  float4 PositionWS : TEXCOORD0;
  float3 NormalWS   : TEXCOORD1;
  float4 Diffuse    : COLOR0;		// diffuse.rgb and alpha
};

struct PixelLightingPSInputTx
{
  float2 TexCoord   : TEXCOORD0;
	// float2	TexCoord1	: TEXCOORD1;
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
  // pct = pct * 1.25 + .25;
  pct = saturate(pct * 5 + .5);
  pct = sqrt(pct);

  return pct;
  //pct = (pct + 1.0) / 2.0;
  //return pct * pct;
}

CommonVSOutput ComputeCommonVSOutput( float4 position )
{
  CommonVSOutput vout;

  float4 pos_ws = mul(position, World);
  float4 pos_vs = mul(pos_ws, View);
  float4 pos_ps = mul(pos_vs, Projection);
  vout.Pos_ws = pos_ws;
  vout.Pos_ps = pos_ps;

  vout.Diffuse	= float4(DiffuseColor.rgb + EmissiveColor, Alpha);
  vout.Specular	= 0;
  vout.TxBlend	= 0; // ComputeFogFactor(length(EyePosition - pos_ws ));

  return vout;
}


CommonVSOutput ComputeCommonVSOutputWithLighting( float4 position_, float3 normal_ )
{
  CommonVSOutput vout;
	
  float4 pos_ws = mul( position_, World );
  float4 pos_vs = mul( pos_ws, View );
  float4 pos_ps = mul( pos_vs, Projection );
  vout.Pos_ws = pos_ws;
  vout.Pos_ps = pos_ps;

  float3 N = normalize( mul( float4( normal_, 1.0f ), World ) ).xyz;
  float3 posToEye = EyePosition - pos_ws.xyz;
  float3 E = normalize( posToEye );
  ColorPair lightResult = ComputeLights( E, N );
	
  vout.Diffuse	= float4( lightResult.Diffuse.rgb, Alpha );
  vout.Specular	= lightResult.Specular;
  vout.TxBlend	=  ComputeTxBlend( N ); // ComputeFogFactor( length( posToEye ) );
	
  return vout;
}


//-----------------------------------------------------------------------------
// Vertex shaders
//-----------------------------------------------------------------------------

VertexLightingVSOutput VSBasic( VSInput vin )
{
  VertexLightingVSOutput vout;

  CommonVSOutput cout = ComputeCommonVSOutput( vin.Position );

  vout.PositionPS = cout.Pos_ps;
  vout.Diffuse    = cout.Diffuse;
  vout.Specular   = float4( cout.Specular, cout.TxBlend );

  return vout;
}


VertexLightingVSOutput VSBasicVc( VSInputVc vin )
{
  VertexLightingVSOutput vout;

  CommonVSOutput cout = ComputeCommonVSOutput(vin.Position);

  vout.PositionPS = cout.Pos_ps;
  vout.Diffuse    = cout.Diffuse * vin.Color;
  vout.Specular   = float4( cout.Specular, cout.TxBlend );

  return vout;
}


VertexLightingVSOutput VSBasicNm( VSInputNm vin )
{
  VertexLightingVSOutput vout;

  CommonVSOutput cout = ComputeCommonVSOutputWithLighting( vin.Position, vin.Normal );

  vout.PositionPS = cout.Pos_ps;
  vout.Diffuse    = cout.Diffuse;
  vout.Specular   = float4( cout.Specular, cout.TxBlend );
	
  return vout;
}


VertexLightingVSOutput VSBasicNmVc( VSInputNmVc vin )
{
  VertexLightingVSOutput vout;

  CommonVSOutput cout = ComputeCommonVSOutputWithLighting( vin.Position, vin.Normal );

  vout.PositionPS = cout.Pos_ps;
  vout.Diffuse    = cout.Diffuse * vin.Color;
  vout.Specular   = float4( cout.Specular, cout.TxBlend );

  return vout;
}


VertexLightingVSOutputTx VSBasicTx( VSInputTx vin )
{
  VertexLightingVSOutputTx vout;

  CommonVSOutput cout = ComputeCommonVSOutput( vin.Position );

  vout.PositionPS = cout.Pos_ps;
  vout.Diffuse    = cout.Diffuse;
  vout.Specular   = float4( cout.Specular, cout.TxBlend );
  vout.TexCoord   = vin.TexCoord;
  vout.TexCoord1  = vin.TexCoord;

  return vout;
}


VertexLightingVSOutputTx VSBasicTxVc( VSInputTxVc vin )
{
  VertexLightingVSOutputTx vout;

  CommonVSOutput cout = ComputeCommonVSOutput( vin.Position );

  vout.PositionPS = cout.Pos_ps;
  vout.Diffuse    = cout.Diffuse * vin.Color;
  vout.Specular   = float4( cout.Specular, cout.TxBlend );
  vout.TexCoord   = vin.TexCoord;
  vout.TexCoord1  = vin.TexCoord;

  return vout;
}


VertexLightingVSOutputTx VSBasicNmTx( VSInputNmTx vin )
{
  VertexLightingVSOutputTx vout;

  CommonVSOutput cout = ComputeCommonVSOutputWithLighting( vin.Position, vin.Normal );

  vout.PositionPS = cout.Pos_ps;
  vout.Diffuse    = cout.Diffuse;
  vout.Specular   = float4( cout.Specular, cout.TxBlend );
  vout.TexCoord   = vin.TexCoord;
  vout.TexCoord1  = vin.TexCoord;
	
  return vout;
}


VertexLightingVSOutputTx VSBasicNmTxVc( VSInputNmTxVc vin )
{
  VertexLightingVSOutputTx vout;

  CommonVSOutput cout = ComputeCommonVSOutputWithLighting( vin.Position, vin.Normal );

  vout.PositionPS = cout.Pos_ps;
  vout.Diffuse    = cout.Diffuse * vin.Color;
  vout.Specular   = float4( cout.Specular, cout.TxBlend );
  vout.TexCoord   = vin.TexCoord;
  vout.TexCoord1  = vin.TexCoord;

  return vout;
}


//-----------------------------------------------------------------------------
// Per-pixel lighting vertex shaders
//-----------------------------------------------------------------------------

PixelLightingVSOutput VSBasicPixelLightingNm( VSInputNm vin_ )
{
  PixelLightingVSOutput vout;

  float4 pos_ws = mul( vin_.Position, World );
  float4 pos_vs = mul( pos_ws, View );
  float4 pos_ps = mul( pos_vs, Projection );

  vout.PositionPS     = pos_ps;
  vout.PositionWS.xyz = pos_ws.xyz;
  // vout.PositionWS.w	= ComputeFogFactor( length( EyePosition - pos_ws ) );
  vout.NormalWS       = normalize( mul( float4( vin_.Normal, 1.0f ), World ) ).xyz;
  vout.PositionWS.w   = ComputeTxBlend( vout.NormalWS );
  vout.Diffuse        = float4( DiffuseColor, Alpha ); // vin.Diffuse; // float4( 1, 1, 1, Alpha );

  return vout;
}


PixelLightingVSOutput VSBasicPixelLightingNmVc( VSInputNmVc vin_ )
{
  PixelLightingVSOutput vout;

  float4 pos_ws = mul( vin_.Position, World );
  float4 pos_vs = mul( pos_ws, View );
  float4 pos_ps = mul( pos_vs, Projection );

  vout.PositionPS     = pos_ps;
  vout.PositionWS.xyz = pos_ws.xyz;
  // vout.PositionWS.w = ComputeFogFactor( length( EyePosition - pos_ws ) );
  vout.NormalWS       = normalize( mul( float4( vin_.Normal, 1.0f ) , World ) ).xyz;
  vout.PositionWS.w   = ComputeTxBlend( vout.NormalWS );
  vout.Diffuse.rgb    = vin_.Color.rgb;
  vout.Diffuse.a      = vin_.Color.a * Alpha;

  return vout;
}


PixelLightingVSOutputTx VSBasicPixelLightingNmTx( VSInputNmTx vin_ )
{
  PixelLightingVSOutputTx vout;

  float4 pos_ws = mul( vin_.Position, World );
  float4 pos_vs = mul( pos_ws, View );
  float4 pos_ps = mul( pos_vs, Projection );

  vout.PositionPS     = pos_ps;
  vout.PositionWS.xyz = pos_ws.xyz;
  // vout.PositionWS.w	= ComputeFogFactor(length(EyePosition - pos_ws));
  vout.NormalWS       = normalize( mul( float4( vin_.Normal, 1 ), World ) ).xyz;
  vout.PositionWS.w   = ComputeTxBlend( vout.NormalWS );
  vout.Diffuse        = float4( 1, 1, 1, Alpha );
  vout.TexCoord       = vin_.TexCoord;
  // vout.TexCoord1  = vin.TexCoord;
	
  return vout;
}


PixelLightingVSOutputTx VSBasicPixelLightingNmTxVc( VSInputNmTxVc vin_ )
{
  PixelLightingVSOutputTx vout;

  float4 pos_ws = mul( vin_.Position, World );
  float4 pos_vs = mul( pos_ws, View );
  float4 pos_ps = mul( pos_vs, Projection );

  vout.PositionPS     = pos_ps;
  vout.PositionWS.xyz = pos_ws.xyz;
  // vout.PositionWS.w = ComputeFogFactor( length( EyePosition - pos_ws ) );
  vout.NormalWS       = normalize( mul( float4( vin_.Normal, 1 ), World ) ).xyz;
  vout.PositionWS.w   = ComputeTxBlend( vout.NormalWS );
  vout.Diffuse.rgb    = vin_.Color.rgb;
  vout.Diffuse.a      = vin_.Color.a * Alpha;
  vout.TexCoord       = vin_.TexCoord;
  // vout.TexCoord1    = vin.TexCoord;
	
  return vout;
}


//-----------------------------------------------------------------------------
// Pixel shaders
//-----------------------------------------------------------------------------

float4 PSBasic( VertexLightingPSInput pin ) : COLOR
{
  float4 color = pin.Diffuse + float4( pin.Specular.rgb, 0 );
  // color.rgb = lerp( color.rgb, FogColor, pin.Specular.w );
  // color = float4( .1, .2, .8, 1 );

  return color;
}


float4 PSBasicTx( VertexLightingPSInputTx pin ) : COLOR
{
  float4 c1    = tex2D( DayTextureSampler,   pin.TexCoord  );
  float4 c2    = tex2D( NightTextureSampler, pin.TexCoord1 );
  float  pct   = pin.Specular.w;
  float4 color = ( c1 * pct + c2 * ( 1.0 - pct ) ) * pin.Diffuse + float4( pin.Specular.rgb, 0 );
  // color.rgb  = lerp( color.rgb, FogColor, pin.Specular.w );
  return color;
}


float4 PSBasicPixelLighting( PixelLightingPSInput pin ) : COLOR
{
  //float3 posToEye = EyePosition - pin.PositionWS.xyz;

  //float3 N = normalize( pin.NormalWS );
  //float3 E = normalize( posToEye );
 
  //ColorPair lightResult = ComputePerPixelLights( E, N );

  //float4 diffuse = float4( lightResult.Diffuse * pin.Diffuse.rgb, pin.Diffuse.a );
  //float4 color = diffuse + float4( lightResult.Specular, 0 );
  // color.rgb = lerp( color.rgb, FogColor, pin.PositionWS.w );

  //return color;

  float3 posToEye = EyePosition - pin.PositionWS.xyz;

  float3 N = normalize( pin.NormalWS );
  float3 E = normalize( posToEye );

  ColorPair lightResult = ComputePerPixelLights( E, N );

  float4 diffuse = float4(EmissiveColor, 0) + float4(lightResult.Diffuse * pin.Diffuse.rgb, pin.Diffuse.a);
  float4 color   = diffuse + float4( lightResult.Specular, 0 );
  // color.rgb = lerp( color.rgb, FogColor, pin.PositionWS.w );

  return color;
}


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
  // color.rgb = lerp( color.rgb, FogColor, pin.PositionWS.w );

  return color;
}


//-----------------------------------------------------------------------------
// Shader and technique definitions
//-----------------------------------------------------------------------------


Technique NightDayEffect0
{
  pass
  {
    VertexShader = compile VERTEX_SHADER_VERSION VSBasic( );
    PixelShader  = compile PIXEL_SHADER_VERSION  PSBasic( );
  }
}

Technique NightDayEffect1
{
  pass
  {
    VertexShader = compile VERTEX_SHADER_VERSION VSBasicVc( );
    PixelShader  = compile PIXEL_SHADER_VERSION  PSBasic(   );
  }
}

Technique NightDayEffect2
{
  pass
  {
    VertexShader = compile VERTEX_SHADER_VERSION VSBasicTx( );
    PixelShader  = compile PIXEL_SHADER_VERSION  PSBasicTx( );
  }
}

Technique NightDayEffect3
{
  pass
  {
    VertexShader = compile VERTEX_SHADER_VERSION VSBasicTxVc( );
    PixelShader  = compile PIXEL_SHADER_VERSION  PSBasicTx(   );
  }
}

Technique NightDayEffect4
{
  pass
  {
    VertexShader = compile VERTEX_SHADER_VERSION VSBasicNm( );
    PixelShader  = compile PIXEL_SHADER_VERSION  PSBasic(   );
  }
}

Technique NightDayEffect5
{
  pass
  {
    VertexShader = compile VERTEX_SHADER_VERSION VSBasicNmVc( );
    PixelShader  = compile PIXEL_SHADER_VERSION  PSBasic(     );
  }
}


Technique NightDayEffect6
{
  pass
  {
    VertexShader = compile VERTEX_SHADER_VERSION VSBasicNmTx( );
    PixelShader  = compile PIXEL_SHADER_VERSION  PSBasicTx(   );
  }
}

Technique NightDayEffect7
{
  pass
  {
    VertexShader = compile VERTEX_SHADER_VERSION VSBasicNmTxVc( );
    PixelShader  = compile PIXEL_SHADER_VERSION  PSBasicTx(     );
  }
}

Technique NightDayEffect8
{
  pass
  {
    VertexShader = compile VERTEX_SHADER_VERSION VSBasicPixelLightingNm( );
    PixelShader  = compile PIXEL_SHADER_VERSION  PSBasicPixelLighting(   );
  }
}

Technique NightDayEffect9
{
  pass
  {
    VertexShader = compile VERTEX_SHADER_VERSION VSBasicPixelLightingNmVc( );
    PixelShader  = compile PIXEL_SHADER_VERSION  PSBasicPixelLighting(     );
  }
}

Technique NightDayEffect10
{
  pass
  {
    VertexShader = compile VERTEX_SHADER_VERSION VSBasicPixelLightingNmTx( );
    PixelShader  = compile PIXEL_SHADER_VERSION  PSBasicPixelLightingTx(   );
  }
}

Technique NightDayEffect11
{
  pass
  {
    VertexShader = compile VERTEX_SHADER_VERSION VSBasicPixelLightingNmTxVc( );
    PixelShader  = compile PIXEL_SHADER_VERSION  PSBasicPixelLightingTx(     );
  }
}
