
namespace MathLibrary
{
    public static class ConversionConstants
    {
      // Tau is a "better" (more logical) Circle constant than PI.
      // It makes things easier.
      // https://tauday.com/tau-manifesto
      // 1 Tau is one turn of the circle 
      // Tau = 2 * PI
      // PI = 1/2 * Tau
      public const double Tau = 2.0 * System.Math.PI;

      public const double DegToRad = Tau / 360.0;
      public const double RadToDeg = 360.0 / Tau;
    }
}
