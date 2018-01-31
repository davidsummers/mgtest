using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace MonoGame
{
  /// <summary>
  /// This is the model that will be loaded.
  /// </summary>
  public class GameModel : GraphicsObject
  {
    public GameModel( string modelName_, string instanceName_, Game game_, string effectName_ = "" )
      : base( instanceName_, game_, effectName_ )
    {
      m_ModelName   = modelName_;
    }

    /// <summary>
    /// LoadContent will be called once per game and is the place to load
    /// all of your content.
    /// </summary>
    public override void LoadContent( )
    { 
      // Load any associated texture
      base.LoadContent( );

      Game game = GetGame( );
      m_Model  = game.Content.Load<Model>( "Models/" + m_ModelName );
    }

    /// <summary>
    /// UnloadContent will be called once per game and is the place to unload
    /// game-specific content.
    /// </summary>
    public override void UnloadContent( )
    {
      base.UnloadContent( );
    }

    /// <summary>
    /// Allows the game to run logic such as updating the world,
    /// checking for collisions, gathering input, and playing audio.
    /// </summary>
    /// <param name="gameTime">Provides a snapshot of timing values.</param>
    public override void Update( GameTime gameTime_ )
    {
      base.Update( gameTime_ );
    }

    /// <summary>
    /// This is called when the game should draw itself.
    /// </summary>
    /// <param name="gameTime">Provides a snapshot of timing values.</param>
    public override void Draw( GameTime gameTime_, Camera camera_ )
    {
      base.Draw( gameTime_, camera_ );
      Matrix world = GetWorldMatrix( );
      m_Model.Draw( world * camera_.GetPositionMatrix( ), camera_.GetView( ), camera_.GetProjection( ) );
    }

    public Model GetModel( )
    {
      return m_Model;
    }

    public override BoundingSphere GetBoundingSphere( bool recalculate_ = false )
    {
      BoundingSphere boundingSphere = base.GetBoundingSphere( );

      if ( recalculate_ == true || boundingSphere == null || boundingSphere.Radius == 0.0f )
      {
        if ( m_Model != null )
        {
          boundingSphere = base.GetBoundingSphere( true ); // true = Create a new one.

          foreach ( ModelMesh mesh in m_Model.Meshes )
          {
            Matrix world = GetWorldMatrix( );
            BoundingSphere mbs = mesh.BoundingSphere.Transform( world );
            boundingSphere = BoundingSphere.CreateMerged( boundingSphere, mbs );
          }
        }
      }

      return boundingSphere;
    }

    public override void UpdateWorld( )
    {
      base.UpdateWorld( );
    }

    #region private

    #endregion

    #region Instance Variables

    private string  m_ModelName;
    private Model   m_Model;

    #endregion
  }
}
