
using Microsoft.Xna.Framework;


namespace MonoGame
{
  /// <summary>
  /// This is the base shape.
  /// </summary>
  public class Shape : GraphicsObject
  {
    public Shape( string shapeType_, string instanceName_, Game game_, string effectName_ = "" )
      : base( instanceName_, game_, effectName_ )
    {
      m_ShapeType = shapeType_;
    }

    /// <summary>
    /// LoadContent will be called once per game and is the place to load
    /// all of your content.
    /// </summary>
    public override void LoadContent( )
    { 
      // Load any associated texture
      base.LoadContent( );
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
    }

    public override BoundingSphere GetBoundingSphere( bool recalculate_ = false )
    {
      BoundingSphere boundingSphere = base.GetBoundingSphere( );

      if ( recalculate_ == true || boundingSphere == null || boundingSphere.Radius == 0.0f )
      {
        // TODO
      }

      return boundingSphere;
    }

    public override void UpdateWorld( )
    {
      base.UpdateWorld( );
    }

    public string GetShapeType( )
    {
      return m_ShapeType;
    }

    #region private

    #endregion

    #region Instance Variables

    string m_ShapeType;

    #endregion
  }
}
