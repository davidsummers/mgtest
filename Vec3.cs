using System;


namespace MathLibrary
{
  public class Vec3
  {
    private static Vec3 mXAxis = new Vec3( 1, 0, 0 );
    public static Vec3 XAxis { get { return mXAxis; } }

    private static Vec3 mYAxis = new Vec3( 0, 1, 0 );
    public static Vec3 YAxis { get { return mYAxis; } }

    private static Vec3 mZAxis = new Vec3( 0, 0, 1 );
    public static Vec3 ZAxis { get { return mZAxis; } }

    private static Vec3 mZero = new Vec3();
    public static Vec3 Empty { get { return mZero; } }

    #region Member Variables

    private double m_x;
    private double m_y;
    private double m_z;

    #endregion

    #region Constructors

    public Vec3( )
    {
      m_x = 0.0;
      m_y = 0.0;
      m_z = 0.0;
    }

    public Vec3( double x_, double y_, double z_ )
    {
      m_x = x_;
      m_y = y_;
      m_z = z_;
    }

    public Vec3( double[] xyz_ )
    {
      if ( xyz_.Length != 3 )
      {
        throw new ArgumentException( );
      }

      m_x = xyz_[0];
      m_y = xyz_[1];
      m_z = xyz_[2];
    }


    #endregion

    #region Operators

    public static Vec3 operator -( Vec3 lhs_, Vec3 rhs_ )
    {
      Vec3 result = new Vec3( );

      result.X = lhs_.X - rhs_.X;
      result.Y = lhs_.Y - rhs_.Y;
      result.Z = lhs_.Z - rhs_.Z;

      return result;
    }

    public static Vec3 operator -( Vec3 lhs_ )
    {
      Vec3 result = new Vec3( );

      result.X = -lhs_.X;
      result.Y = -lhs_.Y;
      result.Z = -lhs_.Z;

      return result;
    }

    public static Vec3 operator +( Vec3 lhs_, Vec3 rhs_ )
    {
      Vec3 result = new Vec3( );

      result.X = lhs_.X + rhs_.X;
      result.Y = lhs_.Y + rhs_.Y;
      result.Z = lhs_.Z + rhs_.Z;

      return result;
    }

    public static Vec3 operator /( Vec3 lhs_, double rhs_ )
    {
      Vec3 result = new Vec3( );

      result.X = lhs_.X / rhs_;
      result.Y = lhs_.Y / rhs_;
      result.Z = lhs_.Z / rhs_;

      return result;
    }

    public static Vec3 operator *( Vec3 lhs_, double rhs_ )
    {
      Vec3 result = new Vec3();

      result.X = lhs_.X * rhs_;
      result.Y = lhs_.Y * rhs_;
      result.Z = lhs_.Z * rhs_;

      return result;
    }

    public static Vec3 operator *( double lhs_, Vec3 rhs_ )
    {
      return rhs_ * lhs_;
    }

    public static explicit operator Vec3( double val_ )
    {
      if ( val_ == 0.0 )
      {
        return new Vec3( );
      }
      else
      {
        throw new ArgumentException( "Can not convert " + val_.ToString() + " to Vec3" );
      }
    }

    public double this[ int index_ ]
    {
      get
      {
        switch ( index_ )
        {
          case 0:  return m_x;
          case 1:  return m_y;
          case 2:  return m_z;
          default: throw new IndexOutOfRangeException( "Index must be 0/X, 1/Y, or 2/Z" );
        }
      }

      set
      {
        switch ( index_ )
        {
          case 0:  m_x = value; break;
          case 1:  m_y = value; break;
          case 2:  m_z = value; break;
          default: throw new IndexOutOfRangeException( "Index must be 0/X, 1/Y, or 2/Z" );
        }
      }
    }

    public double this[ Axis axis_ ]
    {
      get
      {
        switch ( axis_ )
        {
          case Axis.X:    return  m_x;
          case Axis.Y:    return  m_y;
          case Axis.Z:    return  m_z;
          case Axis.NegX: return -m_x;
          case Axis.NegY: return -m_y;
          case Axis.NegZ: return -m_z;
          default:
            throw new ArgumentException( );
        }
      }

      set
      {
        switch ( axis_ )
        {
          case Axis.X:    m_x =  value; break;
          case Axis.Y:    m_y =  value; break;
          case Axis.Z:    m_z =  value; break;
          case Axis.NegX: m_x = -value; break;
          case Axis.NegY: m_y = -value; break;
          case Axis.NegZ: m_z = -value; break;
          default:
            throw new ArgumentException( );
        }
      }
    }

    #endregion

    #region Public Methods

    public Vec3 Copy( )
    {
      return new Vec3( X, Y, Z );
    }

    public Vec3 Cross( Vec3 rhs_ )
    {
      double x, y, z;

      x = Y * rhs_.Z - rhs_.Y * Z;
      y = Z * rhs_.X - rhs_.Z * X;
      z = X * rhs_.Y - rhs_.X * Y;

      return new Vec3(x, y, z);
    }

    public static Vec3 Cross( Vec3 lhs_, Vec3 rhs_ )
    {
      return lhs_.Cross( rhs_ );
    }

    public Vec3 Normalized( )
    {
      double r = Magnitude( );

      if ( r < Double.Epsilon )
      {
        if ( IsZero( ) )
        {
          //Warn::postWarning("Vec3::unit: zero input vector");
        }
        else
        {
          //Warn::postWarning("Vec3::unit: very small input vector");
        }

        return new Vec3();
      }
      else
      {
        return this / r;
      }
    }

    public void Normalize()
    {
      Vec3 normal = Normalized( );
      X = normal.X;
      Y = normal.Y;
      Z = normal.Z;
    }

    public double Dot( Vec3 rhs_ )
    {
      double result;

      result = X * rhs_.X + Y * rhs_.Y + Z * rhs_.Z;

      return result;
    }

    public static double Dot( Vec3 lhs_, Vec3 rhs_ )
    {
      return lhs_.Dot( rhs_ );
    }

    public int DominantIndex( )
    {
      int i = 0;
      double x = Math.Abs( X );
      double y = Math.Abs( Y );

      if ( y > x )
      {
        x = y;
        i = 1;
      }
      if ( Math.Abs( Z) > x )
      {
        i = 2;
      }

      return i;
    }

    public int[] GetLargetsAxesDescending( )
    {
      int[] indices = new int[] { 0, 1, 2 };
      for ( int i = 0; i < 2; ++i )
      {
        for ( int j = i + 1; j < 3; ++j )
        {
          if ( Math.Abs( this[ indices[ i ] ] ) < Math.Abs( this[ indices[ j ] ] ) )
          {
            int index = indices[ i ];
            indices[ i ] = indices[ j ];
            indices[ j ] = index;
          }
        }
      }

      return indices;
    }

    /// <summary>
    /// returns an arbitrary vector perpendicular to this vector (assuming this isn't a zero vector)
    /// </summary>
    /// <returns></returns>
    public Vec3 GetPerpVector()
    {
      int[] indices = GetLargetsAxesDescending( );
      int i1 = indices[ 0 ];
      int i2 = indices[ 2 ];
      int i3 = indices[ 1 ];

      double z1 = this[ i1 ];
      double z2 = this[ i2 ];
      double z3 = this[ i3 ];

      Vec3 p = new Vec3( );
      p[ i1 ] = this[ i2 ];
      p[ i2 ] = this[ i1 ];
      p[ i3 ] = this[ i3 ];
      p = p.Cross( this ).Normalized( );

      // Debug.Assert( Math.Abs( p.Dot( this.Normalized( ) ) ) < .001 );
      return p;
    }

    public Vec3 GetRandomPerpVector( Random r_ )
    {
      double ms = MagnitudeSquared( );
      if ( ms == 0.0 )
      {
        throw new ArgumentException( "Zero vector" );
      }

      do
      {
        Vec3 v = new Vec3(
          r_.NextDouble( ) - 0.5,
          r_.NextDouble( ) - 0.5,
          r_.NextDouble( ) - 0.5 );
        Vec3 c = v.Cross( this ).Normalized( );
        ms = c.MagnitudeSquared( );
        if ( ms > 0.0 )
        {
          return c;
        }
      } while ( true );
    }

    /// <summary>
    /// returns a vector perendicular to x and y
    /// if x and y are nearly parallel, returns an arbitrary vector perpendicular to x
    /// </summary>
    /// <param name="x">primary direction vector</param>
    /// <param name="y">secondary direction vector, preferrably not parallel to x</param>
    /// <returns></returns>
    public static Vec3 SafeCross( Vec3 x_, Vec3 y_ )
    {
      x_ = x_.Normalized();
      y_ = y_.Normalized();

      if ( x_.Dot( y_ ) > .999 )
      {
        y_ = x_.GetPerpVector( );
      }

      return x_.Cross( y_ ).Normalized( );
    }

    public double MagnitudeSquared( )
    {
      double result;

      result = X * X + Y * Y + Z * Z;

      return result;
    }

    public double Magnitude( )
    {
      double result = Math.Sqrt( MagnitudeSquared( ) );
      return result;
    }

    public override string ToString( )
    {
      string result = X.ToString( ) + "|" + Y.ToString( ) + "|" + Z.ToString( );

      return result;
    }

    public override int GetHashCode( )
    {
      return ( X.GetHashCode( ) ^ Y.GetHashCode( ) ^ Z.GetHashCode( ) );
    }

    public override bool Equals( object obj_ )
    {
      if ( GetHashCode( ) == obj_.GetHashCode( ) )
      {
        Vec3 rhs = obj_ as Vec3;
        return ( this - rhs ).MagnitudeSquared( ) < 1.0e-6;
      }
      else
      {
        return false;
      }
    }

    #endregion

    #region Properties

    public double X
    {
      get
      {
        return m_x;
      }

      set
      {
        m_x = value;
      }
    }

    public double Y
    {
      get
      {
        return m_y;
      }

      set
      {
        m_y = value;
      }
    }

    public double Z
    {
      get
      {
        return m_z;
      }

      set
      {
        m_z = value;
      }
    }

    #endregion

    public void Zero()
    {
      m_x = 0;
      m_y = 0;
      m_z = 0;
    }

    public static bool IsAlmostEqual( Vec3 vl_, Vec3 vr_, double epsilon_ )
    {
      if ( vl_ == null )
      {
        if ( vr_ == null )
        {
          return true;
        }
        else
        {
          return false;
        }
      }
      else if ( vr_ == null )
      {
        return false;
      }
      else
      {
        return vl_.IsAlmostEqual( vr_, epsilon_ );
      }
    }

    public bool IsAlmostEqual( Vec3 v_, double epsilon_ )
    {
      if ( Math.Abs( this[ 0 ] - v_[ 0 ] ) > epsilon_ ||
           Math.Abs( this[ 1 ] - v_[ 1 ] ) > epsilon_ ||
           Math.Abs( this[ 2 ] - v_[ 2 ] ) > epsilon_ )
      {
        return false;
      }
      else
      {
        return true;
      }
    }

    public bool IsEqual( Vec3 v_ )
    {
      if ( this[ 0 ] != v_[ 0 ] || this[ 1 ] != v_[ 1 ] || this[ 2 ] != v_[ 2 ] )
      {
        return false;
      }
      else
      {
        return true;
      }
    }

    public static bool IsEqual( Vec3 u_, Vec3 v_ )
    {
      return u_.IsEqual( v_ );
    }

    public static double PctDiff( Vec3 v1_, Vec3 v2_ )
    {
      double dm = ( v2_ - v1_ ).Magnitude( );
      if ( dm == 0.0 )
      {
        return 0.0;
      }

      Vec3 v = ( v1_ + v2_ ) / 2.0;
      double m = v.Magnitude();
      if ( m == 0.0 )
      {
        return 100.0 * dm / Math.Max( v1_.Magnitude( ), v2_.Magnitude( ) );
      }
      else
      { 
        return 100.0 * dm / m;
      }
    }

    public bool IsZero( )
    {
      if ( X != 0.0 || Y != 0.0 || Z != 0.0 )
      {
        return false;
      }
      else
      {
       return true;
      }
    }

    //public Vec3 Negate( )
    //{
    //  return new Vec3( -X, -Y, -Z );
    //}

    public Vec3 Range( double lo_, double hi_ )
    {
      return new Vec3(
        ( X < lo_ ) ? lo_ : ( ( X > hi_ ) ? hi_ : X ),
        ( Y < lo_ ) ? lo_ : ( ( Y > hi_ ) ? hi_ : Y ),
        ( Z < lo_ ) ? lo_ : ( ( Z > hi_ ) ? hi_ : Z ) );
    }

    public int RecessiveIndex()
    {
      int i = 0;
      double x = Math.Abs( X );
      double y = Math.Abs( Y );

      if ( y < x )
      {
        x = y;
        i = 1;
      }

      if ( Math.Abs( Z ) < x )
      {
        i = 2;
      }

      return i;
    }

    public enum Axis
    {
      X = 1,
      Y = 2,
      Z = 3,
      NegX = 4,
      NegY = 5,
      NegZ = 6
    };

    public static bool IsInAxis( Axis a1_, Axis a2_ )
    {
      Vec3 v1 = PrincipalAxis( a1_ );
      Vec3 v2 = PrincipalAxis( a2_ );
      return ( v1.DominantIndex( ) == v2.DominantIndex( ) );
    }

    public Vec3 Rotate( Axis axis_, double angle_ )
    {
      double c = Math.Cos( angle_ );
      double s = Math.Sin( angle_ );
      double t;  // temp (in case this is v)
      Vec3 result = new Vec3( );
      switch ( axis_ )
      {
        case Axis.X:
          t = Y * c - Z * s;
          result.Z = Z * c + Y * s;
          result.Y = t;
          result.X = X;
          break;

        case Axis.Y:
          t = X * c + Z * s;
          result.Z = Z * c - X * s;
          result.X = t;
          result.Y = Y;
          break;

        case Axis.Z:
          t = X * c - Y * s;
          result.Y = Y * c + X * s;
          result.X = t;
          result.Z = Z;
          break;

        default:
          // Debug.Assert( axis_ < Axis.X || axis_ > Axis.Z );
          break;
      }

      return result;
    }

    //public void CopyFrom( Vec3 v_ )
    //{
    //  m_x = v_.X;
    //  m_y = v_.Y;
    //  m_z = v_.Z;
    //}

    public static Vec3 Rotate( Vec3 v_, Vec3 axis_, double angle_ )
    {
      if ( axis_.IsZero( ) || angle_ == 0.0 )
      {
        return v_.Copy( );
      }

      Vec3 u = axis_.Normalized( );
      double vdu = v_.Dot( u ); // v dot u

      Vec3 vu = u  * vdu;
      Vec3 vp = v_ - vu;       // component of v perpendicular to u
      Vec3 vc = u.Cross( vp ); // vp rotated +90 deg about u 

      double c = Math.Cos( angle_ );
      double s = Math.Sin( angle_ );

      return vu + vp * c + vc * s;
    }

    public static void Swap( Vec3 v_, Vec3 z_ )
    {
      for ( int i = 0; i < 3; ++i )
      {
        double t = v_[ i ];
        v_[i] = z_[ i ];
        z_[i] = t;
      }
    }

    public static Vec3 PrincipalAxis(Axis axis)
    {
      switch (axis)
      {
        // axis must be 1, 2, or 3
        case Axis.X:    return new Vec3(  1.0, 0.0, 0.0 );
        case Axis.Y:    return new Vec3(  0.0, 1.0, 0.0 );
        case Axis.Z:    return new Vec3(  0.0, 0.0, 1.0 );
        case Axis.NegX: return new Vec3( -1,   0,   0   );
        case Axis.NegY: return new Vec3(  0,  -1,   0   );
        case Axis.NegZ: return new Vec3(  0,   0,  -1   );
        default:        return new Vec3(  0.0, 0.0, 0.0 );
      }
    }

    public double AbsMax()
    {
      double x = Math.Abs( X );
      double y = Math.Abs( Y );
      if ( y > x )
      {
        x = y;
      }

      y = Math.Abs( Z );

      if ( y > x )
      {
        return y;
      }
      else
      {
        return x;
      }
    }

    public double Angle( Vec3 v_ )
    {
      double x = Math.Sqrt( v_.MagnitudeSquared( ) * MagnitudeSquared( ) );

      if ( x < Double.Epsilon )
      {
        //Warn::postWarning( "Vec3::angle: zero vector" );
        return 0.0;
      }

      double cang = this.Dot( v_ ) / x;
      if ( Math.Abs( cang ) < 0.866 )
      {
        return Math.Acos( cang );
      }

      Vec3 w = Cross( v_ );
      double sang = w.Magnitude( ) / x;

      return Math.Atan2( sang, cang );
    }

    public static double TriAngle( Vec3 source_, Vec3 target_, Vec3 other_ )
    {
      Vec3 los  = ( target_ - source_ ).Normalized( );
      Vec3 sToO = ( other_  - source_ ).Normalized( );

      return Math.Acos( sToO.Dot( los ) );
    }

    ///**
    // unitCross: this = unit vector along v x w  (vector cross product)
    // */
    public static Vec3 UnitCross( Vec3 v_, Vec3 w )
    {
      Vec3 u = v_.Cross( w );
      double magnitudeSq = u.MagnitudeSquared( );
      if ( magnitudeSq < Double.Epsilon )
      {
        u.Zero();
      }
      else
      {
        u = u / Math.Sqrt( magnitudeSq );
      }

      return u;
    }


    ///**
    // orthogonalPart: this = v + s * w, where the scalar s is chosen so that
    //                 this is orthogonal to w
    // */
    public static Vec3 OrthogonalPart( Vec3 v_, Vec3 w_ )
    {
      double s = w_.MagnitudeSquared( );

      if ( s <= 0.0 )
      { 
        // the w vector is zero, so this = v
        return new Vec3( v_.X, v_.Y, v_.Z );
      }

      s = -( v_.Dot( w_ ) ) / s;

      return v_ + s * w_;
    }

    ///**
    // orthogonalPart: this + s * v, where the scalar s is chosen so that
    //                 this is orthogonal to v
    // */
    public Vec3 OrthogonalPart( Vec3 v_ )
    {
      double s = v_.MagnitudeSquared( );

      if ( s <= 0.0 )
      {
        return this.Copy( );
      }

      s = -( Dot( v_ ) ) / s;

      return this * s;
    }

    public Vec3 Equate( Vec3 v_ )
    {
      X = v_.X;
      Y = v_.Y;
      Z = v_.Z;

      return this;
    }

    public Vec3 OneOver( )
    {
      return new Vec3( 1.0 / m_x, 1.0 / m_y, 1.0 / m_z );
    }

    public static Vec3 ScalarMultiply( Vec3 lhs_, Vec3 rhs_ )
    {
      return new Vec3( lhs_.X * rhs_.X, lhs_.Y * rhs_.Y, lhs_.Z * rhs_.Z );
    }

    public static Vec3 RandomVec( Random r_, double magnitude_ = 1.0)
    {
      magnitude_ *= 2.0;
      return new Vec3(
        ( r_.NextDouble( ) - .5 ) * magnitude_,
        ( r_.NextDouble( ) - .5 ) * magnitude_,
        ( r_.NextDouble( ) - .5 ) * magnitude_ );
    }

    public static Axis RandomAxis( Random r_ )
    {
      return (Axis) ( r_.Next( 6 ) + 1 );
    }

    public static Axis ReadAxis( string axis_ )
    {
      axis_ = axis_.ToLower( ).Trim( );
      switch ( axis_ )
      {
        case "x":
        case "+x":
          return Axis.X;

        case "y":
        case "+y":
          return Axis.Y;

        case "z":
        case "+z":
          return Axis.Z;

        case "-x": return Axis.NegX;
        case "-y": return Axis.NegY;
        case "-z": return Axis.NegZ;

        default:
          throw new ArgumentException( );
      }
    }

    public void Scale( double s_ )
    {
      m_x *= s_;
      m_y *= s_;
      m_z *= s_;
    }

    public void Set( double x_, double y_, double z_ )
    {
      this.X = x_;
      this.Y = y_;
      this.Z = z_;
    }

    public void Set( Vec3 v_ )
    {
      this.X = v_.X;
      this.Y = v_.Y;
      this.Z = v_.Y;
    }
  }
}
