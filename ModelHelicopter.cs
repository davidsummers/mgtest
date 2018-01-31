using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace MonoGame
{
  /// <summary>
  /// This is the model that will be loaded.
  /// </summary>
  public class HelicopterModel : GameModel
  {
    public HelicopterModel( string modelName_, string instanceName_, Game game_, string effectName_ = "" )
      : base( "Helicopter", instanceName_, game_, effectName_ )
    {
    }

    /// <summary>
    /// This is called when the game should draw itself.
    /// </summary>
    /// <param name="gameTime">Provides a snapshot of timing values.</param>
    public override void Draw( GameTime gameTime_, Camera camera_ )
    {
      Model     model       = GetModel( );
      Effect    modelEffect = GetEffect( );
      Matrix    world       = GetWorldMatrix( );
      Texture2D texture     = GetTexture( );

      foreach ( ModelMesh mesh in model.Meshes )
      {
        
        if ( modelEffect != null )
        {
          foreach ( ModelMeshPart part in mesh.MeshParts )
          {
            part.Effect = modelEffect;
            modelEffect.Parameters[ "p_World"      ].SetValue( mesh.ParentBone.Transform * world * camera_.GetPositionMatrix( ) );
            modelEffect.Parameters[ "p_View"       ].SetValue( camera_.GetView( )                );
            modelEffect.Parameters[ "p_Projection" ].SetValue( camera_.GetProjection( )          );
          } 
        }
        else
        {
          foreach ( BasicEffect effect in mesh.Effects )
          {
            // Transforms
            effect.World                  = mesh.ParentBone.Transform * world * camera_.GetPositionMatrix( );
            effect.View                   = camera_.GetView( );
            effect.Projection             = camera_.GetProjection( );

            // Lighting
            effect.EnableDefaultLighting( );
            effect.LightingEnabled        = true;
            effect.PreferPerPixelLighting = true;

  #if !AMBIENT_OFF
            effect.AmbientLightColor = new Vector3( 1.0f, 1.0f, 1.0f );
  #endif
  #if EMMISIVE_OFF
            effect.EmissiveColor     = new Vector3( 0,    0,    1    );
  #endif
            // Directional Lighting
            effect.DirectionalLight0.DiffuseColor  = new Vector3( 1.0f,  0, 0 ); // Red
            effect.DirectionalLight0.Direction     = new Vector3( 0,    -1, 0 ); // - Y-Axis
            effect.DirectionalLight0.SpecularColor = new Vector3( 0,     1, 0 ); // Green Specular Lighting.
            effect.DirectionalLight0.Enabled       = false;
        
            // Texture
            effect.TextureEnabled         = true;

            // Fog
            effect.FogEnabled = false;
            effect.FogColor = Color.White.ToVector3( ); // Normally make this the same color as background.
            effect.FogStart =  9.95f;
            effect.FogEnd   = 10.25f;
          }
        }

        mesh.Draw( );
      }
    }


    #region private

    #endregion

    #region Instance Variables

    #endregion
  }
}
