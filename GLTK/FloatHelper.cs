using System;
using System.Collections.Generic;
using System.Text;

namespace GLTK
{
  /// <summary>
  /// see http://stackoverflow.com/questions/3874627/floating-point-comparison-functions-for-c-sharp
  /// </summary>
  public static class FloatHelper
  {
    public static unsafe int FloatToInt32Bits(float f)
    {
      return *((int*)&f);
    }

    public static unsafe long DoubleToInt64Bits(double f)
    {
      return *((long*)&f);
    }

    public static bool AlmostEqual(float a, float b, int maxDeltaBits)
    {
      // zero comparison doesn't work with the bits method
      if (a == 0 || b == 0)
      {
        if (Math.Abs(a) <= 1e-6 && Math.Abs(b) <= 1e-6)
        {
          return true;
        }
      }

      int aInt = FloatToInt32Bits(a);
      if (aInt < 0)
        aInt = Int32.MinValue - aInt;

      int bInt = FloatToInt32Bits(b);
      if (bInt < 0)
        bInt = Int32.MinValue - bInt;

      int intDiff = Math.Abs(aInt - bInt);
      int maxDiff = (1 << maxDeltaBits);
      return intDiff <= maxDiff;
    }

    public static bool AlmostEqual(double a, double b, int maxDeltaBits)
    {
      // zero comparison doesn't work with the bits method
      if (a == 0 || b == 0)
      {
        if (Math.Abs(a) <= 1e-15 && Math.Abs(b) <= 1e-15)
        {
          return true;
        }
      }

      long aInt = DoubleToInt64Bits(a);
      if (aInt < 0)
        aInt = Int64.MinValue - aInt;

      long bInt = DoubleToInt64Bits(b);
      if (bInt < 0)
        bInt = Int64.MinValue - bInt;

      long intDiff = Math.Abs(aInt - bInt);
      long maxDiff = (1L << maxDeltaBits);
      return intDiff <= maxDiff;
    }

    public static bool AlmostEqual(double a, double b)
    {
      // A default value of "30" differing bits might seem like a lot here
      // but these are 64 bit doubles. A 30 bit difference is the difference
      // between 2.0 and 2.00000047
      return AlmostEqual(a, b, 30);
    }
  }
}
