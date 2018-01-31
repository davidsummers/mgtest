using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using MathLibrary;

namespace MonoGame
{
  /// <summary>
  /// This is the main type for your game.
  /// </summary>
  public class mgtest : Game
  {
    public mgtest( )
    {
      m_Graphics = new GraphicsDeviceManager( this );
      this.Content.RootDirectory = "Content";

      m_Scene = new Scene( );
      Camera camera = m_Scene.GetCamera( );
#if NO_UPDATE_CAMERA_POSITION
      camera.AddUpdater( ( Camera camera_, GameTime gameTime_ ) =>
      {
        Vector3 cameraPosition = camera_.GetPosition( );
        cameraPosition += new Vector3( -0.1f, 0, 0 );
        camera_.SetPosition( cameraPosition );
      } );
#endif

      const float earthRadius = 6371000f;

      // Calculate Distance and set distance and camera far plane
      double distance = 4 * earthRadius;
      float  farClip  = (float) ( 4 * distance );
      camera.SetNearPlane( 10.0 );
      camera.SetFarPlane( farClip );
      camera.SetTarget( new Vector3( -1, 0, 0 ) );

#if !HIDE_EARTH
      m_Earth = new EarthModel( "Earth1", this );
      m_Earth.SetScale( new Vector3( earthRadius, earthRadius, earthRadius ) );
      m_Earth.SetPosition( new Vector3( (float) -distance, 0, 0 ) );
      m_Earth.AddUpdater( ( GraphicsObject gameObject_, GameTime gameTime_ ) =>
      {
        Vector3 orientation = gameObject_.GetOrientation( );
        orientation.X -= 0.001f;
        gameObject_.SetOrientation( orientation );     
      } ); 
      m_Scene.Add( m_Earth.GetInstanceName( ), m_Earth );
#endif

#if HIDE_HELICOPTER
      m_Helicopter = new HelicopterModel( "Helicopter", "Helicopter1", this, "" );
      m_Helicopter.SetPosition( new Vector3( -20, 0, 0 ) );
      m_Helicopter.AddUpdater( ( GraphicsObject gameObject_, GameTime gameTime_ ) =>
      {
        Vector3 orientation = gameObject_.GetOrientation( );
        orientation.X -= 0.01f; 
        gameObject_.SetOrientation( orientation );     
      } ); 
      m_Scene.Add( m_Helicopter.GetInstanceName( ), m_Helicopter );
#endif

#if !HIDE_CIRCLE
      m_Circle = new Circle( "Circle1", this );
      m_Circle.SetPosition( m_Earth.GetPosition( ) );
      m_Circle.SetRadius( earthRadius );
      m_Circle.AddUpdater( ( GraphicsObject gameObject_, GameTime gameTime_ ) =>
      {
        Vector3 orientation = gameObject_.GetOrientation( );
        orientation.X -= 0.001f; 
        gameObject_.SetOrientation( orientation );     
      } ); 
      m_Scene.Add( m_Circle.GetInstanceName( ), m_Circle );
#endif

      camera.SetTarget( m_Earth.GetPosition( ) );
    }

    /// <summary>
    /// Allows the game to perform any initialization it needs to before starting to run.
    /// This is where it can query for any required services and load any non-graphic
    /// related content.  Calling base.Initialize will enumerate through any components
    /// and initialize them as well.
    /// </summary>
    protected override void Initialize( )
    {
      // TODO: Add your initialization logic here

      base.Initialize( );
    }

    /// <summary>
    /// LoadContent will be called once per game and is the place to load
    /// all of your content.
    /// </summary>
    protected override void LoadContent( )
    {
      m_Scene.LoadContent( );
    }

    /// <summary>
    /// UnloadContent will be called once per game and is the place to unload
    /// game-specific content.
    /// </summary>
    protected override void UnloadContent( )
    {
      // TODO: Unload any non ContentManager content here
    }

    /// <summary>
    /// Allows the game to run logic such as updating the world,
    /// checking for collisions, gathering input, and playing audio.
    /// </summary>
    /// <param name="gameTime">Provides a snapshot of timing values.</param>
    protected override void Update( GameTime gameTime_ )
    {
      // Check to see if we need to quit the program.
      if ( GamePad.GetState( PlayerIndex.One ).Buttons.Back == ButtonState.Pressed || Keyboard.GetState( ).IsKeyDown( Keys.Escape ) )
      {
        Exit( );
      }

#if KEEP_POSITION
      // Update model position.
      Vector3 position = m_Model.GetPosition( );
      position -= new Vector3( 0.01f, 0, 0 );
      m_Model.SetPosition( position );
#endif

      m_Scene.Update( gameTime_ );

      // Update base game.
      base.Update( gameTime_ );
    }

    /// <summary>
    /// This is called when the game should draw itself.
    /// </summary>
    /// <param name="gameTime">Provides a snapshot of timing values.</param>
    protected override void Draw( GameTime gameTime_ )
    {
      GraphicsDevice.Clear( Color.CornflowerBlue );

      m_Scene.Draw( gameTime_ );

      base.Draw( gameTime_ );
    }

    #region private

    #endregion

    #region InstanceVars

    private GraphicsDeviceManager m_Graphics;

    private Scene  m_Scene;

    private HelicopterModel m_Helicopter;
    private EarthModel      m_Earth;
    private Circle          m_Circle;

    #endregion
  }
}
