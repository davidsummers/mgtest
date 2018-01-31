using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using MathLibrary;

namespace MonoGame
{
  /// <summary>
  /// This is the circle shape.
  /// </summary>
  public class Circle : Shape
  {
    public Circle( string instanceName_, Game game_ )
      : base( "Circle", instanceName_, game_, "SimpleEffect" )
    {
      List<Vec3> points = new List<Vec3>();
      double dt = 2.0;
      for ( double th = 0; th <= 360.0 + dt; th += dt )
      {
          double thR = th * ConversionConstants.DegToRad;
          points.Add( new Vec3( Math.Cos( thR ), Math.Sin( thR ), 0.0 ) );
      }

      m_CircleVertices = ConnectPoints( points );
    }

    public static LineVert[] ConnectPoints( IEnumerable<Vec3> points_ )
    {
      List<LineVert> quads = new List<LineVert>();

      Vec3 pL = null;
      foreach ( Vec3 p in points_ )
      {
        if ( pL != null )
        {
          quads.Add( new LineVert( GeometryTools.ConvertVector( p ) ) );
        }

        pL = p;
      }

      return quads.ToArray();
    }

    /// <summary>
    /// LoadContent will be called once per game and is the place to load
    /// all of your content.
    /// </summary>
    public override void LoadContent( )
    {
      // Load any associated effect.
      base.LoadContent( );
      m_VertexDeclaration = new VertexDeclaration( LineVert.VertexElements );
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

      GraphicsDevice myDevice = GetGame( ).GraphicsDevice;

      // Save oldBlendState and oldDepthStencilState.
      BlendState        oldBlendState        = myDevice.BlendState;
      DepthStencilState oldDepthStencilState = myDevice.DepthStencilState;

      // Create newBlendState and newDepthStencilState.
      BlendState        newBlendState        = new BlendState( );
      DepthStencilState newDepthStencilState = new DepthStencilState( );

      newBlendState        = BlendState.Opaque;
      newDepthStencilState = DepthStencilState.Default;

      // Set graphics state to newBlenState and newDepthStencilState.
      myDevice.BlendState        = newBlendState;
      myDevice.DepthStencilState = newDepthStencilState;

      VertexBuffer vertexBuffer = new VertexBuffer(
          myDevice,
          m_VertexDeclaration,
          m_VertexDeclaration.GetVertexElements( ).GetLength( 0 ),
          BufferUsage.None );
      myDevice.SetVertexBuffer( vertexBuffer );

      bool isVisible = true;

      Matrix world = GetWorldMatrix( );

      Effect effect = GetEffect( );

      if ( isVisible && m_CircleVertices.Length > 0 )
      {
        effect.Parameters[ "World"     ].SetValue( world * camera_.GetPositionMatrix( ) );
        effect.Parameters[ "View"      ].SetValue( camera_.GetView( ) );
        effect.Parameters[ "Proj"      ].SetValue( camera_.GetProjection( ) );
        effect.Parameters[ "LineColor" ].SetValue( m_Color.ToVector4( ) * (float) m_Brightness );

        for ( int ps = 0; ps < effect.CurrentTechnique.Passes.Count; ps++ )
        {
          effect.CurrentTechnique.Passes[ps].Apply( );
          myDevice.DrawUserPrimitives<LineVert>( PrimitiveType.LineStrip, m_CircleVertices, 0, m_CircleVertices.Length - 1 );
        }
      }

      // Restore oldBlendState and oldDepthStencilState.
      myDevice.BlendState        = oldBlendState;
      myDevice.DepthStencilState = oldDepthStencilState;
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

    public double GetRadius( )
    {
      Vector3 scale = GetScale( );
      return scale.X;
    }

    public void SetRadius( double radius_ )
    {
      SetScale( new Vector3( (float) radius_, (float) radius_, (float) radius_ ) );
    }

    #region private

    #endregion

    #region Instance Variables

    private LineVert [] m_CircleVertices;

    private VertexDeclaration m_VertexDeclaration;

    private Color m_Color = new Color( 1.0f, 0f, 0f );

    private double m_Brightness = 1.0f;

    #endregion
  }
}
