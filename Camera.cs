using System;

using Microsoft.Xna.Framework;

using MathLibrary;

namespace MonoGame
{

  public delegate void CameraUpdateMethod( Camera camera_, GameTime time_ );

  /// <summary>
  /// This is a Camera.
  /// </summary>
  public class Camera
  {
    public Camera( )
    {
      m_Target      = new Vector3( -1, 0, 0 ); // Set target to FROM origin to this position.
      m_Orientation = new Vector3(  0, 1, 0 ); // Up
      SetPosition(    new Vector3(  0, 0, 0 ) );

      m_NearPlane   = 0.1f;
      m_FarPlane    = 100f;
      m_FieldOfView = 45f;
      m_AspectRatio = 800f / 600f;

      CalculateView( );

      CalculateProjection( );
    }


    public virtual void AddUpdater( CameraUpdateMethod callback_ )
    {
      m_OnUpdate += callback_;
    }

    /// <summary>
    /// Allows the game to run logic such as updating the world,
    /// checking for collisions, gathering input, and playing audio.
    /// </summary>
    /// <param name="gameTime">Provides a snapshot of timing values.</param>
    public void Update( GameTime gameTime_ )
    {
      if ( m_OnUpdate != null )
      {
        m_OnUpdate( this, gameTime_ );
      }

      CalculateView( );
    }

    public Matrix GetView( )
    {
      return m_View;
    }

    public void SetView( Matrix view_ )
    {
      m_View = view_;
    }

    public Matrix GetProjection( )
    {
      return m_Projection;
    }

    public void SetProjection( Matrix projection_ )
    {
      m_Projection = projection_;
    }

    public Vector3 GetPosition( )
    {
      return m_Position;
    }

    public void SetPosition( Vector3 position_ )
    {
      m_Position = position_;
      m_PositionMatrix = Matrix.CreateTranslation( -m_Position );
      CalculateView( );
    }

    public Matrix GetPositionMatrix( )
    {
      return m_PositionMatrix;
    }

    public Vector3 GetTarget( )
    {
      return m_Target;  
    }

    public void SetTarget( Vector3 target_ )
    {
      m_Target = target_;
      CalculateView( );
    }

    public double GetFieldOfView( )
    {
      return m_FieldOfView;
    }

    public void SetFieldOfView( double fov_ )
    {
      m_FieldOfView = fov_;
      CalculateProjection( );
    }

    public double GetAspectRatio( )
    {
      return m_AspectRatio;
    }

    public void SetAspectRatio( double aspectRatio_ )
    {
      m_AspectRatio = aspectRatio_;
      CalculateProjection( );
    }

    public double GetNearPlane( )
    {
      return m_NearPlane;
    }

    public void SetNearPlane( double nearPlane_ )
    {
      m_NearPlane = nearPlane_;
      CalculateProjection( );
    }

    public double GetFarPlane( )
    {
      return m_FarPlane;
    }

    public void SetFarPlane( double farPlane_ )
    {
      m_FarPlane = farPlane_;
      CalculateProjection( );
    }
    #region private

    private void CalculateView( )
    {
      // Note: To get rid of spatial jitter, the camera is *ALWAYS* located at the origin.
      // When we move, we will actually need to move all the objects in the system
      // in the reverse vector instead of moving the camera in the forward vector.
      m_View = Matrix.CreateLookAt( Vector3.Zero, m_Target - m_Position, m_Orientation );
    }

    private void CalculateProjection( )
    {
      m_Projection = Matrix.CreatePerspectiveFieldOfView(
        (float) ( m_FieldOfView * ConversionConstants.DegToRad ),
        (float) m_AspectRatio,
        (float) m_NearPlane,
        (float) m_FarPlane );
    }

    #endregion

    #region InstanceVars

    private Vector3 m_Position;
    private Vector3 m_Orientation;
    private Vector3 m_Target;

    private Matrix m_PositionMatrix;
    private Matrix m_View;
    private Matrix m_Projection;

    private double m_NearPlane;
    private double m_FarPlane;

    private double m_AspectRatio;
    private double m_FieldOfView;

    private event CameraUpdateMethod m_OnUpdate;

    #endregion
  }
}
