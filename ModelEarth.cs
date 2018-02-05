using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace MonoGame
{
  /// <summary>
  /// This is the EarhtModel class.
  /// </summary>
  public class EarthModel : GameModel
  {
    public EarthModel( string instanceName_, Game game_ )
      : base( "EarthModel", instanceName_, game_, "NightDay" )
    {
    }

    public override void LoadContent( )
    {
      base.LoadContent( );

      Game game = GetGame( );
      m_DayTexture   = game.Content.Load<Texture2D>( "Textures/WorldDay"   );
      m_NightTexture = game.Content.Load<Texture2D>( "Textures/WorldNight" );

      Effect effect = GetEffect( );
      Model  model  = GetModel( );

      foreach ( ModelMesh mesh in model.Meshes )
      {
        foreach ( ModelMeshPart part in mesh.MeshParts )
        {
          part.Effect = effect;
          effect.Parameters[ "DayTexture"   ].SetValue( m_DayTexture   );
          effect.Parameters[ "NightTexture" ].SetValue( m_NightTexture );
        }
      }
    }

    /// <summary>
    /// This is called when the game should draw itself.
    /// </summary>
    /// <param name="gameTime">Provides a snapshot of timing values.</param>
    public override void Draw( GameTime gameTime_, Camera camera_ )
    {
      Model     model       = GetModel( );
      Matrix    world       = GetWorldMatrix( );

      Vector3 sunDir = m_SunDir;
      sunDir.Normalize( );

      foreach ( ModelMesh mesh in model.Meshes )
      {
        foreach ( Effect effect in mesh.Effects )
        {
          effect.Parameters[ "DayTexture"   ].SetValue( m_DayTexture   );
          effect.Parameters[ "NightTexture" ].SetValue( m_NightTexture );

          effect.Parameters[ "EyePosition"            ].SetValue( Vector3.Zero ); // Because camera is ALWAYS at origin.

          effect.Parameters[ "DiffuseColor"           ].SetValue( m_Diffuse );
          effect.Parameters[ "Alpha"                  ].SetValue( 1.0f );
          effect.Parameters[ "EmissiveColor"          ].SetValue( m_EmmisiveColor );
          effect.Parameters[ "SpecularColor"          ].SetValue( m_Specular );
          effect.Parameters[ "SpecularPower"          ].SetValue( 16f );
          effect.Parameters[ "AmbientLightColor"      ].SetValue( m_Ambient );

          effect.Parameters[ "DirLight0Direction"     ].SetValue( sunDir );
          effect.Parameters[ "DirLight0DiffuseColor"  ].SetValue( m_SunDiffuse );
          effect.Parameters[ "DirLight0SpecularColor" ].SetValue( m_SunSpecular );

          effect.Parameters[ "DirLight1Direction"     ].SetValue( sunDir );
          effect.Parameters[ "DirLight1DiffuseColor"  ].SetValue( m_SunDiffuse );
          effect.Parameters[ "DirLight1SpecularColor" ].SetValue( m_SunSpecular );

          effect.Parameters[ "DirLight2Direction"     ].SetValue( sunDir );
          effect.Parameters[ "DirLight2DiffuseColor"  ].SetValue( m_SunDiffuse );
          effect.Parameters[ "DirLight2SpecularColor" ].SetValue( m_SunSpecular );

          effect.Parameters[ "World"                  ].SetValue( mesh.ParentBone.Transform * world * camera_.GetPositionMatrix( ) );
          effect.Parameters[ "Projection"             ].SetValue( camera_.GetProjection( ) );
          effect.Parameters[ "View"                   ].SetValue( camera_.GetView( ) );

          const int shaderVal = 10; // Effect # 10 in shader effect file.
          effect.CurrentTechnique = effect.Techniques[ shaderVal ];
        } 

        mesh.Draw( );
      }
    }


    #region private methods

    #endregion

    #region private instance variables

    private Vector3 m_Ambient       = new Vector3( 1,     1,     1     );
    private Vector3 m_Blue          = Color.Blue.ToVector3( );
    private Vector3 m_Diffuse       = new Vector3( 0.34f, 0.34f, 0.34f );
    private Vector3 m_EmmisiveColor = Vector3.Zero;
    private Vector3 m_Specular      = new Vector3( 0.3f,  0.3f,  0.2f  );

    private Vector3 m_SunDir        = new Vector3( 0, 0, 10 );
    private Vector3 m_SunDiffuse    = Vector3.One;
    private Vector3 m_SunSpecular   = new Vector3( .4f, .4f, .3f );
 
    private Texture2D m_DayTexture;
    private Texture2D m_NightTexture;


    #endregion
  }
}
