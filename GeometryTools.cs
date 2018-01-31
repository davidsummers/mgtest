using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace MathLibrary
{
  public class GeometryTools
  {
    static GeometryTools( )
    {
      r = new Random( );
      mEmpty = new Color( Vector4.Zero );
    }

    private static Random r;
    public static Random Rand
    {
        get { return r; }
    }

    private static Color mEmpty;
    public static Color EmptyColor { get { return mEmpty; } }

#if FUTURE
    private static IBoxConfig mBoxGenerator;
    public static void SetBoxConfig(IBoxConfig bc)
    {
        mBoxGenerator = bc.GetConfig();
    }

    public static IBoxConfig GetBoxConfig()
    {
        if (mBoxGenerator == null)
            return null;
        else
            return mBoxGenerator.GetConfig();
    }
#endif

    public static Vector3 PositionFromAER(double az, double el, double r)
    {
        double er = el * ConversionConstants.DegToRad;
        double ar = az * ConversionConstants.DegToRad;

        float x = (float)(r * (Math.Cos(er) * Math.Cos(ar)));
        float z = (float)(r * (Math.Cos(er) * Math.Sin(ar)));
        float y = (float)(r * Math.Sin(er));

        return new Vector3(x, y, z);
    }

    public static Vector3 Intersects(
        Vector3 source, float radius,
        Vector3 p, Vector3 v)
    {
        v.Normalize();

        p -= 10 * radius * v;
        BoundingSphere sphere = new BoundingSphere(source, radius);
        Ray r = new Ray(p, v);

        float? intersection = r.Intersects(sphere);
        if (intersection.HasValue)
            return p + intersection.Value * v;
        else
            return Vector3.Zero;
    }

    public static List<VertexPositionColor> CreateLinesFromPoints(IList<Vector3> points, Color c)
    {
        List<VertexPositionColor> lines = new List<VertexPositionColor>();

        for (int i = 0; i < points.Count; ++i)
        {
            int i2 = i + 1;
            if (i2 == points.Count) i2 = 0;

            lines.Add(new VertexPositionColor(points[i], c));
            lines.Add(new VertexPositionColor(points[i2], c));
        }

        return lines;
    }

    public static List<VertexPositionColor> CreateLinesFromFaces(IList<Vector3> vertices, Color c)
    {
        List<VertexPositionColor> lines = new List<VertexPositionColor>();

        int numberOfFaces = vertices.Count / 3;
        for (int i = 0; i < numberOfFaces; ++i)
        {
            int faceStart = 3 * i;
            for (int j = 0; j < 3; ++j)
            {
                int idx1 = faceStart + j;
                int idx2 = faceStart + j + 1;
                if (j == 2) idx2 = faceStart;

                lines.Add(new VertexPositionColor(vertices[idx1], c));
                lines.Add(new VertexPositionColor(vertices[idx2], c));
            }
        }

        return lines;
    }

    public static bool IsOccluded(
            Vector3 source,
            Vector3 target,
            Vector3 sphereCenter,
            float rSquared,
            float obliquity,
            bool checkFront)
    {
        Vector3 A = source - sphereCenter;
        Vector3 B = target - sphereCenter;

        A.Y *= obliquity;
        B.Y *= obliquity;

        Vector3 aMinusB = A - B;

        float s = Vector3.Dot(A, aMinusB) / aMinusB.LengthSquared();
        if (s < 0)
        {
            // r = A.Magnitude();
            return false;
        }
        else if (checkFront && s > 1)
        {
            //r = B.Magnitude();
            return false;
        }
        else
        {
            double rs = (A + s * -aMinusB).LengthSquared();
            //r = Math.Sqrt(rs);
            return (rs <= rSquared);
        }
    }

#if FUTURE
    public static List<VOPrimitiveObject> CreateCone(
        VOController controller,
        Vec3 source, double coneLength,
        double azMinRad, double azMaxRad,
        double elMinRad, double elMaxRad)
    {
        List<VOPrimitiveObject> primitives = new List<VOPrimitiveObject>();

        List<Vec3> vertices = new List<Vec3>();
        List<KeyValuePair<Vector3, Vector3>> lines = new List<KeyValuePair<Vector3, Vector3>>();

        List<Vec3> conePoints = new List<Vec3>();

        EFG sourceEFG = new EFG(source, DateTime.Now);

        double[,] azels = new double[,] {
            { azMinRad, elMinRad },
            {azMinRad, elMaxRad},
            {azMaxRad, elMaxRad},
            {azMaxRad, elMinRad }};

        int pointsPerSide = 20;
        bool crossesZero = (azels[1, 0] > azels[2, 0]);
        for (int i = 0; i < 4; ++i)
        {
            int i2 = i + 1;
            if (i2 == 4) i2 = 0;
            double az1 = azels[i, 0];
            double az2 = azels[i2, 0];
            double el1 = azels[i, 1];
            double el2 = azels[i2, 1];

            if (crossesZero)
            {
                if (az1 > Math.PI)
                    az1 -= 2.0 * Math.PI;
                if (az2 > Math.PI)
                    az2 -= 2.0 * Math.PI;
            }

            double dAz = (az2 - az1) / (pointsPerSide + 1);
            double dEl = (el2 - el1) / (pointsPerSide + 1);

            double az = az1;
            double el = el1;
            for (int j = 0; j < pointsPerSide; ++j)
            {
                EFG rayVec = EFG.FromTopoAzEl(sourceEFG, az, el);
                conePoints.Add(source + (coneLength * rayVec.Vector));

                az += dAz;
                el += dEl;
            }

        }

        for (int i = 0; i < conePoints.Count; ++i)
        {
            int i2 = i + 1;
            if (i2 == conePoints.Count)
                i2 = 0;

            vertices.Add(source);
            vertices.Add(conePoints[i]);
            vertices.Add(conePoints[i2]);

            if (i % pointsPerSide == 0)
            {
                lines.Add(new KeyValuePair<Vector3, Vector3>(
                    GeometryTools.ConvertVector(source),
                    GeometryTools.ConvertVector(conePoints[i])));
            }
            lines.Add(new KeyValuePair<Vector3, Vector3>(
                GeometryTools.ConvertVector(conePoints[i]),
                GeometryTools.ConvertVector(conePoints[i2])));
        }

        Vector3 cs = GeometryTools.ConvertVector(source);
        VOLineCollection line = new VOLineCollection(controller, null, lines);
        line.ObjectSelectable = false;
        line.Recenter = false;
        line.Position = cs;
        primitives.Add(line);

        VOVertexCollection cone = new VOVertexCollection(controller, null,
            GeometryTools.ConvertVectors(vertices));
        cone.ObjectSelectable = false;
        cone.Recenter = false;
        cone.Position = cs;
        primitives.Add(cone);

        return primitives;
    }
#endif

#if FUTURE
    public static void OrderTrianglesClockWise(List<Vec3> triangles)
    {
        Vec3 center = MathLibrary.Box2Box.GetCentroid(triangles);
        int numFaces = triangles.Count / 3;
        for (int i = 0; i < numFaces; ++i)
        {
            int idx = 3 * i;
            Vec3 v1 = triangles[idx];
            Vec3 v2 = triangles[idx + 1];
            Vec3 v3 = triangles[idx + 2];

            Vec3 triangleCenter = (v1 + v2 + v3) / 3;
            Vec3 Z = (v1 - center).Normalized();
            Vec3 X = (v1 - triangleCenter).Normalized();
            Vec3 Y = Z.Cross(X);

            double[] angles = new double[3];
            for (int j = 0; j < 3; ++j)
            {
                Vec3 rayDir = (triangles[idx + j] - triangleCenter);
                double x = rayDir.Dot(X);
                double y = rayDir.Dot(Y);
                angles[j] = Math.Atan2(y, x);
            }
            for (int j = 0; j < 2; ++j)
            {
                for (int k = j + 1; k < 3; ++k)
                {
                    if (angles[j] < angles[k])
                    {
                        MathLibrary.Box2Box.SwapListIndices(triangles, idx + j, idx + k);
                    }
                }
            }

        }
    }
#endif

#if FUTURE
    public static void NormalizeVertices(
        IEnumerable<Vector3> points, bool recenter,
        ref Vector3 center, out List<Vector3> normPoints, out BoundingSphere bs)
    {
        int numPoints = 0;
        Vector3 midPoint = Vector3.Zero;

        if (recenter)
        {
            foreach (Vector3 p in points)
            {
                ++numPoints;
                midPoint += p;
            }
            midPoint /= numPoints;
            center = midPoint;
        }
        else
            midPoint = center;

        normPoints = new List<Vector3>();
        float largestRadius = 0.0f;
        foreach (Vector3 p in points)
        {
            Vector3 np = p - midPoint;
            normPoints.Add(np);

            float r = np.Length();
            if (r > largestRadius) largestRadius = r;
        }

        bs = new BoundingSphere(Vector3.Zero, largestRadius);
    }
#endif
    public static IEnumerable<Vector3> IterateByIndices(IEnumerable<Vector3> points, short[] indices)
    {
        List<Vector3> l = new List<Vector3>(points);
        for (short i = 0; i < indices.Length; ++i)
            yield return l[i];
    }

    public static IEnumerable<Vector3> ConvertVectors(IEnumerable<Vec3> points)
    {
        foreach (Vec3 p in points)
        {
            yield return ConvertVector(p);
        }
    }

    public static IEnumerable<Vec3> ConvertVectors(IEnumerable<Vector3> points)
    {
        foreach (Vector3 p in points)
        {
            yield return ConvertVector(p);
        }
    }

    protected void SetTexturePixel(ref ushort[] data, int width, int height, int x, int y, byte r, byte g, byte b)
    {
        ushort colour;
        r >>= 3;
        g >>= 2;
        b >>= 3;
        colour = (ushort)((r << 11) | (g << 5) | b);
        data[x + y * height] = colour;
    }

    protected void SetTexturePixel(ref ushort[] data, int width, int height, int x, int y, Color color)
    {
        byte r = color.R;
        byte g = color.G;
        byte b = color.B;

        SetTexturePixel(ref data, width, height, x, y, r, g, b);
    }
#if FUTURE
    public static Matrix GetECIRotationMatrix(
        CoordinateSystem.AxesType axes, DateTime t,
        Coordinates pos, Coordinates vel,
        Vector3 r, Vector3 v)
    {
        CoordinateSystem cs = null;
        if (axes == CoordinateSystem.AxesType.SEZ)
        {
            if (pos == null)
                pos = new ECI(GeometryTools.ConvertVector(r), t);

            cs = CoordinateSystem.GetSystem(pos, null, CoordinateSystem.AxesType.SEZ);
        }
        else
        {
            //if (inertial && pos != null && vel != null)
            //{
            //    vel.ReferenceCoords = pos;
            //    //Coordinates refCoords = vel.ReferenceCoords;
            //    //Coordinates origVel = vel;
            //    //vel.ReferenceCoords = pos;

            //    //ECI velECI = vel.ToCoordinates<ECI>();
            //    //velECI.ReferenceCoords = null;
            //    //vel = velECI.ToBase();

            //    //origVel.ReferenceCoords = refCoords;
            //}

            if (pos == null)
                pos = new ECI(GeometryTools.ConvertVector(r), t);
            if (vel == null)
            {
                vel = new ECI(GeometryTools.ConvertVector(v), t);
                vel.ReferenceCoords = pos;
            }

            cs = CoordinateSystem.GetSystem(pos, vel, axes);
        }

        Vec3 X = cs.XAxis.ToCoordinates<ECI>().Vector;
        Vec3 Y = cs.YAxis.ToCoordinates<ECI>().Vector;
        Vec3 Z = cs.ZAxis.ToCoordinates<ECI>().Vector;

        Matrix rot = new Matrix(
            (float)X.X, (float)X.Y, (float)X.Z, 0f,
            (float)Y.X, (float)Y.Y, (float)Y.Z, 0f,
            (float)Z.X, (float)Z.Y, (float)Z.Z, 0f,
              0f, 0f, 0f, 1f);

        return rot;
    }
#endif

#if FUTURE
    public static Mat3x3 ConvertMatrix(Matrix m)
    {
        return new Mat3x3(
            m.M11, m.M12, m.M13,
            m.M21, m.M22, m.M23,
            m.M31, m.M32, m.M33);
    }

    public static Matrix ConvertMatrix(Mat3x3 m)
    {
        return new Matrix(
            (float)m[0], (float)m[1], (float)m[2], 0,
            (float)m[3], (float)m[4], (float)m[5], 0,
            (float)m[6], (float)m[7], (float)m[8], 0,
            0, 0, 0, 1);
    }
#endif

    public static Vec3 ConvertVector(Vector3 v)
    {
        return new Vec3(v.X, v.Y, v.Z);
    }

    public static Vector3 ConvertVector(Vec3 v)
    {
        return new Vector3((float)v.X, (float)v.Y, (float)v.Z);
    }

#if FUTURE
    public static Microsoft.Xna.Framework.Color ConvertColor(System.Drawing.Color c)
    {
        return new Color(c.R, c.G, c.B, c.A);
    }
#endif

#if FUTURE
    public static System.Drawing.Color ConvertColor(Color c)
    {
        return System.Drawing.Color.FromArgb(c.A, c.R, c.G, c.B);
    }
#endif

#if FUTURE
    public static AzElRange CalculateAER(Vector3 source, Vector3 target)
    {
        Vector3 StoT = (target - source);

        double r = Math.Sqrt(StoT.X * StoT.X + StoT.Y * StoT.Y + StoT.Z * StoT.Z);
        double el = Math.Asin(StoT.Y / r);
        double az = Math.Atan2(StoT.Z, Math.Sqrt(StoT.X * StoT.X + StoT.Y * StoT.Y));

        return new AzElRange(az * ConversionConstants.RadToDeg, el * ConversionConstants.RadToDeg, r);
    }
#endif

    public static VertexPositionColor[] CreateLine(Color c, Vec3 p1, Vec3 p2)
    {
        VertexPositionColor l1 = new VertexPositionColor(
            new Vector3((float)p1.X, (float)p1.Y, (float)p1.Z),
            c);
        VertexPositionColor l2 = new VertexPositionColor(
            new Vector3((float)p1.X, (float)p1.Y, (float)p1.Z),
            c);

        return new VertexPositionColor[] { l1, l2 };
    }

    public static Matrix TranslateNormal(Matrix world, Matrix rotation)
    {
        Vector3 pos = world.Translation;
        world.Translation = Vector3.Zero;
        Matrix r = world * rotation;
        r.Translation = pos;

        return r;
    }

    public static float NextFloat()
    {
        return NextFloat(1.0);
    }

    public static float NextFloat(double d)
    {
        return (float)(r.NextDouble() * d);
    }

    public static Vector3 RandomPosition(double magnitude)
    {
        float mover2 = (float)(magnitude / 2.0);
        Vector3 o = new Vector3(
            NextFloat(magnitude) - mover2,
            NextFloat(magnitude) - mover2,
            NextFloat(magnitude) - mover2);

        return o;
    }

    public static Matrix RandomRotation()
    {
        Matrix m =
            Matrix.CreateRotationX(NextFloat(2 * Math.PI)) *
            Matrix.CreateRotationY(NextFloat(2 * Math.PI)) *
            Matrix.CreateRotationZ(NextFloat(2 * Math.PI));

        return m;
    }

    public static double ConeSteradians(double coneAngleDeg)
    {
        double rad = coneAngleDeg * ConversionConstants.DegToRad;
        Vec3 p1 = new Vec3(1, 0, 0);
        Vec3 p2 = p1.Rotate(Vec3.Axis.Y, rad);

        //double r = Math.Abs(p2.Z);
        double h = Math.Abs(1.0 - p2.X);

        return Math.PI * (2 * h);
    }

    public static double AreaTriangle(Vec3 p1, Vec3 p2, Vec3 p3)
    {
        Vec3 l1 = (p2 - p1);
        Vec3 l2 = (p3 - p1);

        return l1.Cross(l2).Magnitude() * .5;
    }

    public static List<Vec3> CullFaces(List<Vec3> points,
        params IEnumerable<Vec3>[] pointsToRemove)
    {
        List<Vec3> culled = new List<Vec3>();
        for (int i = 0; i < points.Count / 3; ++i)
        {
            int start = 3 * i;
            foreach (IEnumerable<Vec3> faceCollection in pointsToRemove)
            {
                bool removeFace = true;
                for (int j = start; j < start + 3; ++j)
                {
                    Vec3 hullPoint = points[j];
                    bool foundMatch = false;

                    foreach (Vec3 rp in faceCollection)
                    {
                        if ((hullPoint - rp).Magnitude() < 1.0e-6)
                        {
                            foundMatch = true;
                            break;
                        }
                    }

                    if (!foundMatch)
                    {
                        removeFace = false;
                        break;
                    }
                }

                if (removeFace)
                {
                    for (int j = 0; j < 3; ++j)
                    {
                        culled.Add(points[start]);
                        points.RemoveAt(start);
                    }
                    --i;
                    break;
                }
            }

        }

        return culled;
    }

    public static Color RandomColor()
    {
        return new Color(new Vector4(
                    (float)r.NextDouble(),
                    (float)r.NextDouble(),
                    (float)r.NextDouble(),
                    .95f)); //1.0f));
    }

    public static Matrix RotationFromNormal(Vector3 from)
    {
        if (from == Vector3.Zero) return Matrix.Identity;
        from.Normalize();

        return RotationFromNormal(from, Vector3.Up);

    }

    public static Matrix RotationFromNormal(Vector3 from, Vector3 to)
    {
        if (to == from) return Matrix.Identity;

        Vector3 axis = Vector3.Cross(from, to);
        if (axis == Vector3.Zero)
        {
            if (from.X != 0 || from.Y != 0)
                axis = new Vector3(-from.Y, from.X, from.Z);
            else
                axis = new Vector3(from.X, -from.Z, from.Y);
        }

        float ang = Vector3.Dot(from, to);
        ang = (float)Math.Acos(ang);  // 0 < ang < 180 deg

        //if (ang < -0.9999999999)
        //{  // vfrom = -vto special case
        //    int ir = uf.RecessiveIndex();
        //    Vec3 vei = Vec3.PrincipalAxis((Vec3.Axis)ir + 1);
        //    axis = vei.Cross(uf);
        //    ang = Math.PI;  // pi, or 180 degrees
        //}
        //else
        //{  // the general case
        //}

        Matrix R = FromAngleAxis(ang, axis);
        //Vector3 test = Vector3.Transform(from, R);
        return R;


        //u.Normalize();

        //Matrix m = Matrix.Identity;
        //Vector3 o = u;
        //float d = Vector3.Dot(u, Vector3.Up);
        //Vector3 up = Vector3.Up - d * u;
        //Vector3 right = Vector3.Cross(up, o);

        //Vector3 t = Vector3.Up;

        //m.Forward = u;
        //m.Up = Vector3.Up - Vector3.Multiply(u, d);
        //m.Right = Vector3.Cross(m.Up, m.Forward);


        //return m;
    }

    ///**
    // * fromAngleAxis: this = rotation matrix corresponding to a rotation through
    // *                       the angle "angle" (rad) about an axis along axis
    // *
    // * Beware that the angle is the clockwise rotation about the axis (left-handed
    // * convention).
    // *
    // * If w is a unit vector along axis, define matrix b = w * transpose( w ), and
    // * matrix c as the antisymmetric matrix associated with w (see antisym),
    // * then  b + (I - b) * Math.Cos( angle ) - c * Math.Sin( angle ).
    // *
    // * If the axis vector has a square magnitude < 1.0e-30, a warning is issued
    // * and this is set to the identity matrix.
    // *
    // * See function toAngleAxis for the inverse of this function.
    // */
    public static Matrix FromAngleAxis(float angle, Vector3 axis)
    {
        double vm = Math.Pow(axis.Length(), 2.0);
        if (vm < 1.0e-30)
        {
            //Warn::postWarning( "fromAngleAxis: Axis vector is zero" );
            return Matrix.Identity;
        }

        vm = 1.0 / Math.Sqrt(vm);
        double w0 = axis.X * vm;  // w unit vector components
        double w1 = axis.Y * vm;
        double w2 = axis.Z * vm;

        double s = Math.Sin(angle);
        double c = Math.Cos(angle);
        double cc = 1.0 - c;

        Matrix result = Matrix.Identity;

        double x, y, z;
        z = w0 * cc;
        // m11
        result.M11 = (float)(c + w0 * z);
        x = z * w1;
        y = w2 * s;
        //m12 
        result.M12 = (float)(x - y);
        //m21 
        result.M21 = (float)(x + y);
        x = z * w2;
        y = w1 * s;
        //m13 
        result.M13 = (float)(x + y);
        //m31 
        result.M31 = (float)(x - y);
        z = w1 * cc;
        // m22 
        result.M22 = (float)(c + w1 * z);
        x = z * w2;
        y = w0 * s;
        //m23 
        result.M23 = (float)(x - y);
        //m32 
        result.M32 = (float)(x + y);
        //m33 
        result.M33 = (float)(c + w2 * w2 * cc);

        return result;
    }

#if FUTURE
    public static List<KeyValuePair<Vector3, Vector3>> GetOutline(
        VOPrimitiveObject primitive,
        bool joinCoplanarFaces)
    {
        if (primitive.CurrentDrawMode == PrimitiveType.TriangleList)
        {
            primitive.GetVertices();
            Matrix w = Matrix.CreateScale(primitive.Scale) * primitive.Rotation * Matrix.CreateTranslation(primitive.Position);

            List<Vector3> vertices = new List<Vector3>();
            foreach (VertexPositionColor vpc in primitive.Vertices)
            {
                Vector3 vert = Vector3.Transform(vpc.Position, w);
                vertices.Add(vert);
            }

            if (vertices.Count % 3 != 0)
                return null;
            int numFaces = vertices.Count / 3;
            List<KeyValuePair<int, int>> verticesToSkip = new List<KeyValuePair<int, int>>();
            if (joinCoplanarFaces)
            {
                for (int i = 0; i < numFaces - 1; ++i)
                    for (int j = i + 1; j < numFaces; ++j)
                    {
                        Triangle t1 = new Triangle(vertices[3 * i], vertices[3 * i + 1], vertices[3 * i + 2]);
                        Triangle t2 = new Triangle(vertices[3 * j], vertices[3 * j + 1], vertices[3 * j + 2]);

                        Vector3 n1 = t1.GetNormal();
                        Vector3 n2 = t2.GetNormal();

                        if ((1.0f - Math.Abs(Vector3.Dot(n1, n2))) < 1e-3f)
                        {
                            List<KeyValuePair<int, int>> joinedPoints = Triangle.JoinedPoints(t1, t2);
                            if (joinedPoints.Count == 2)
                            {
                                verticesToSkip.Add(new KeyValuePair<int, int>(
                                    3 * i + joinedPoints[0].Key, 3 * i + joinedPoints[0].Value));
                                verticesToSkip.Add(new KeyValuePair<int, int>(
                                    3 * j + joinedPoints[1].Key, 3 * j + joinedPoints[1].Value));
                            }
                        }
                    }
            }

            List<KeyValuePair<Vector3, Vector3>> lines = new List<KeyValuePair<Vector3, Vector3>>();
            for (int i = 0; i < numFaces; ++i)
                for (int j = 0; j < 3; ++j)
                {
                    int idx = 3 * i + j;
                    int idx2 = idx + 1;
                    if (j == 2) idx2 -= 3;

                    bool skip = false;
                    foreach (KeyValuePair<int, int> line in verticesToSkip)
                    {
                        if (idx == line.Key && idx2 == line.Value ||
                            idx == line.Value && idx2 == line.Key)
                        {
                            skip = true;
                            break;
                        }
                    }

                    if (!skip)
                        lines.Add(new KeyValuePair<Vector3, Vector3>(vertices[idx], vertices[idx2]));
                }

            float delta = 1.0e-3f;
            // remove duplicate lines
            for (int i = 0; i < lines.Count - 1; ++i)
                for (int j = i + 1; j < lines.Count; ++j)
                {

                    Vector3 p1 = lines[i].Key;
                    Vector3 p2 = lines[i].Value;
                    Vector3 p3 = lines[j].Key;
                    Vector3 p4 = lines[j].Value;

                    float d1 = (p3 - p1).Length();
                    float d2 = (p4 - p2).Length();

                    if (d1 < delta && d2 < delta)
                    {
                        lines.RemoveAt(j--);
                        continue;
                    }

                    float d3 = (p4 - p1).Length();
                    float d4 = (p3 - p2).Length();
                    if (d3 < delta && d4 < delta)
                    {
                        lines.RemoveAt(j--);
                        continue;
                    }
                }

            return lines;
        }
        else
            return null;
    }
#endif
    //public static Color SetAlpha(Color c, float alpha)
    //{
    //    Vector4 v = c.ToVector4();
    //    v.W = alpha;
    //    return new Color(v);
    //}
  }
}
