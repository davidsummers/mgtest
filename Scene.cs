using System.Collections.Generic;

using Microsoft.Xna.Framework;

namespace MonoGame
{
  /// <summary>
  /// This is a Scene.  A Scene has a camera and a list of world objects.
  /// </summary>
  public class Scene
  {
    #region public

    public Scene( )
    {
      // Initialize Camera
      m_Camera = new Camera( );

      // Initialize scene list.
      m_ObjectList = new Dictionary< string, GraphicsObject >( );
    }

    public void Add( string instanceName_, GraphicsObject object_ )
    {
      m_ObjectList.Add( instanceName_, object_ );
    }

    public void LoadContent( )
    {
      foreach ( GraphicsObject obj in m_ObjectList.Values )
      {
        obj.LoadContent( );
      }
    }

    /// <summary>
    /// Allows the game to run logic such as updating the world,
    /// checking for collisions, gathering input, and playing audio.
    /// </summary>
    /// <param name="gameTime">Provides a snapshot of timing values.</param>
    public void Update( GameTime gameTime_ )
    {
      foreach ( GraphicsObject obj in m_ObjectList.Values )
      {
        // Call any model class updates.
        obj.Update( gameTime_ );
      }

      m_Camera.Update( gameTime_ );
    }

    public void Draw( GameTime gameTime_ )
    {
      foreach ( GraphicsObject obj in m_ObjectList.Values )
      {
        obj.Draw( gameTime_, m_Camera );
      }
    }

    #endregion

    #region accessors

    public Camera GetCamera( )
    {
      return m_Camera;
    }

    #endregion

    #region InstanceVars

    private Camera                               m_Camera;
    private Dictionary< string, GraphicsObject > m_ObjectList;

    #endregion
  }
}
