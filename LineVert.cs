using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MathLibrary
{
  public struct LineVert : IVertexType
  {
    public Vector3 Position;
    public Vector4 Data;

    public LineVert( Vector3 position_ )
    {
      Position = position_;
      Data = new Vector4( .25f, .25f, 0, 0 ); // Vector4.Zero;
    }

    public static readonly VertexElement[] VertexElements = new VertexElement[]
    {
      new VertexElement(                 0, VertexElementFormat.Vector3, VertexElementUsage.Position,          0 ),
      new VertexElement( sizeof(float) * 3, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 0 ),
    };

    public VertexDeclaration VertexDeclaration
    {
      get { return new VertexDeclaration( VertexElements ); }
    }

    public static int SizeInBytes
    {
      get
      {
        return sizeof(float) * 11;
      }
    }

    public static bool operator !=( LineVert left_, LineVert right_ )
    {
        return left_.GetHashCode( ) != right_.GetHashCode( );
    }

    public static bool operator ==( LineVert left_, LineVert right_ )
    {
        return left_.GetHashCode( ) == right_.GetHashCode( );
    }

    public override bool Equals( object obj_ )
    {
        return this == (LineVert) obj_;
    }

    public override int GetHashCode( )
    {
        return Position.GetHashCode( ) | Data.GetHashCode( );
    }

    public override string ToString( )
    {
        return Position.ToString( );
    }
  }
}
