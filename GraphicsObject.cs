using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace MonoGame
{
  #region declarations

  public delegate void GraphicsObjectUpdateMethod( GraphicsObject graphicsObject_, GameTime time_ );

  #endregion

  /// <summary>
  /// This is the base graphics object.
  /// </summary>
  public class GraphicsObject
  {
    public GraphicsObject( string instanceName_, Game game_, string effectName_ = "" )
    {
      m_InstanceName = instanceName_;
      m_Game         = game_;
      m_EffectName    = effectName_;

      m_Position    = new Vector3( 0, 0, 0 );
      m_Orientation = new Vector3( 0, 0, 0 );
      m_Scale       = new Vector3( 1, 1, 1 );
      UpdateWorld( );
    }

    /// <summary>
    /// LoadContent will be called once per game and is the place to load
    /// all of your content.
    /// </summary>
    public virtual void LoadContent( )
    {
      if ( m_EffectName != "" )
      {
        m_Effect = m_Game.Content.Load<Effect>( "Effects/" + m_EffectName );
      }
    }

    /// <summary>
    /// UnloadContent will be called once per game and is the place to unload
    /// game-specific content.
    /// </summary>
    public virtual void UnloadContent( )
    {
      // TODO: Unload any non ContentManager content here
    }

    /// <summary>
    /// Allows the game to run logic such as updating the world,
    /// checking for collisions, gathering input, and playing audio.
    /// </summary>
    /// <param name="gameTime">Provides a snapshot of timing values.</param>
    public virtual void Update( GameTime gameTime_ )
    {
      if ( m_OnUpdate != null )
      {
        m_OnUpdate( this, gameTime_ );
      }
    }

    /// <summary>
    /// This is called when the game should draw itself.
    /// </summary>
    /// <param name="gameTime">Provides a snapshot of timing values.</param>
    public virtual void Draw( GameTime gameTime_, Camera camera_ )
    {
    }

    public virtual string GetInstanceName( )
    {
      return m_InstanceName;
    }

    public Game GetGame( )
    {
      return m_Game;
    }

    public Effect GetEffect( )
    {
      return m_Effect;
    }

    public void SetEffect( Effect effect_ )
    {
      m_Effect = effect_;
    }

    public virtual Matrix GetWorldMatrix( )
    {
      return m_World;
    }

    public virtual Vector3 GetPosition( )
    {
      return m_Position;
    }

    public virtual void SetPosition( Vector3 position_ )
    {
      m_Position = position_;
      UpdateWorld( );
    }

    public virtual Vector3 GetOrientation( )
    {
      return m_Orientation;
    }

    public virtual void SetOrientation( Vector3 orientation_ )
    {
      m_Orientation = orientation_;
      UpdateWorld( );
    }

    public virtual Vector3 GetScale( )
    {
      return m_Scale;
    }

    public virtual void SetScale( Vector3 scale_ )
    {
      m_Scale = scale_;
      UpdateWorld( );
    }

    public virtual Texture2D GetTexture( )
    {
      return m_Texture;
    }

    public virtual void SetTexture( Texture2D texture_ )
    {
      m_Texture = texture_;
    }

    public virtual BoundingSphere GetBoundingSphere( bool recalculate_ = false )
    {
      if ( recalculate_ == true || m_BoundingSphere == null || m_BoundingSphere.Radius == 0.0f )
      {
          m_BoundingSphere = new BoundingSphere( );
      }

      return m_BoundingSphere;
    }

    public virtual void SetBoundingSphere( BoundingSphere boundingSphere_ )
    {
      m_BoundingSphere = boundingSphere_;
    }

    public virtual void UpdateWorld( )
    {
      m_Rotation = Quaternion.CreateFromYawPitchRoll( m_Orientation.X, m_Orientation.Y, m_Orientation.Z );
      m_World = Matrix.CreateScale(          m_Scale    ) *
                Matrix.CreateFromQuaternion( m_Rotation ) *
                Matrix.CreateTranslation(    m_Position );
    }

    public virtual void AddUpdater( GraphicsObjectUpdateMethod callback_ )
    {
      m_OnUpdate += callback_;
    }

    #region private


    #endregion

    #region Instance Variables

    private string  m_InstanceName;

    private Game    m_Game;

    private Vector3    m_Position;
    private Vector3    m_Orientation;
    private Vector3    m_Scale;
    private Quaternion m_Rotation;
    private Matrix     m_World;

    private string     m_EffectName;
    private Effect     m_Effect;

    private Texture2D  m_Texture;

    private BoundingSphere m_BoundingSphere;

    private event GraphicsObjectUpdateMethod m_OnUpdate;

    #endregion
  }
}
