using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Diagnostics;
using Tao.OpenGl;

namespace GLTK
{
  public struct Point
  {
    public static Point Origin = new Point(0, 0, 0);

    public Point(double x, double y, double z)
    {
      mElements = new double[3];
      mElements[0] = x; mElements[1] = y; mElements[2] = z;
    }

    public double this[int xiIndex]
    {
      get
      {
        return mElements[xiIndex];
      }
    }

    public Vector GetPositionVector()
    {
      return new Vector(mElements[0], mElements[1], mElements[2]);
    }

    public override bool Equals(object obj)
    {
      if (obj == null)
      {
        return false;
      }

      if (ReferenceEquals(obj, this))
      {
        return true;
      }
      Point lOther = (Point)obj;

      return lOther[0].Equals(mElements[0])
        && lOther[1].Equals(mElements[1])
        && lOther[2].Equals(mElements[2]);
    }

    public static bool operator ==(Point xiLhs, Point xiRhs)
    {
      return xiLhs.Equals(xiRhs);
    }

    public static bool operator !=(Point xiLhs, Point xiRhs)
    {
      return !xiLhs.Equals(xiRhs);
    }

    public static Vector operator -(Point xiLhs, Point xiRhs)
    {
      return new Vector(
        xiLhs[0] - xiRhs[0],
        xiLhs[1] - xiRhs[1],
        xiLhs[2] - xiRhs[2]);
    }

    public double x
    {
      get
      {
        return mElements[0];
      }
    }

    public double y
    {
      get
      {
        return mElements[1];
      }
    }

    public double z
    {
      get
      {
        return mElements[2];
      }
    }

    public override int GetHashCode()
    {
      return base.GetHashCode(); //qq
    }

    private double[] mElements;
  }
}
