using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Diagnostics;
using Tao.OpenGl;

namespace GLTK
{
  public struct Vector
  {
    public static Vector Zero = new Vector(0, 0, 0);
    public static Vector XAxis = new Vector(1, 0, 0);
    public static Vector YAxis = new Vector(0, 1, 0);
    public static Vector ZAxis = new Vector(0, 0, 1);

    public Vector(double x, double y, double z)
    {
      mElements = new double[3];

      mElements[0] = x;
      mElements[1] = y;
      mElements[2] = z;
    }

    public double Length
    {
      get
      {
        return Math.Sqrt(
          mElements[0] * mElements[0]
          + mElements[1] * mElements[1]
          + mElements[2] * mElements[2]);
      }
    }

    public double LengthSquared
    {
      get
      {
        return mElements[0] * mElements[0]
          + mElements[1] * mElements[1]
          + mElements[2] * mElements[2];
      }
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

    public double this[int xiIndex]
    {
      get
      {
        return mElements[xiIndex];
      }
    }

    public Vector Normalise()
    {
      double lLength = Length;
      if (lLength == 0)
      {
        return Vector.Zero;
      }
      else
      {
        return this / lLength;
      }
    }

    public double Dot(Vector xiOther)
    {
      return mElements[0] * xiOther.mElements[0]
           + mElements[1] * xiOther.mElements[1]
           + mElements[2] * xiOther.mElements[2];
    }

    public static Vector operator +(Vector xiLhs, Vector xiRhs)
    {
      return new Vector(
        xiLhs[0] + xiRhs[0],
        xiLhs[1] + xiRhs[1],
        xiLhs[2] + xiRhs[2]);
    }

    public static Point operator +(Point xiPoint, Vector xiVector)
    {
      return new Point(
        xiPoint[0] + xiVector[0],
        xiPoint[1] + xiVector[1],
        xiPoint[2] + xiVector[2]);
    }

    public static Vector operator -(Vector xiLhs, Vector xiRhs)
    {
      return new Vector(
        xiLhs[0] - xiRhs[0],
        xiLhs[1] - xiRhs[1],
        xiLhs[2] - xiRhs[2]);
    }

    public static Point operator -(Point xiPoint, Vector xiVector)
    {
      return new Point(
        xiPoint[0] - xiVector[0],
        xiPoint[1] - xiVector[1],
        xiPoint[2] - xiVector[2]);
    }

    public static Vector operator ^(Vector xiLhs, Vector xiRhs)
    {
      return new Vector(
        (xiLhs[1] * xiRhs[2]) - (xiLhs[2] * xiRhs[1]),
        (xiLhs[2] * xiRhs[0]) - (xiLhs[0] * xiRhs[2]),
        (xiLhs[0] * xiRhs[1]) - (xiLhs[1] * xiRhs[0]));
    }

    public static double operator *(Vector xiLhs, Vector xiRhs)
    {
      return (xiLhs[0] * xiRhs[0]) + (xiLhs[1] * xiRhs[1]) + (xiLhs[2] * xiRhs[2]);
    }

    public static Vector operator *(Vector xiVector, double xiScalar)
    {
      return new Vector(
        xiVector[0] * xiScalar,
        xiVector[1] * xiScalar,
        xiVector[2] * xiScalar);
    }

    public static Vector operator *(double xiScalar, Vector xiVector)
    {
      return new Vector(
        xiVector[0] * xiScalar,
        xiVector[1] * xiScalar,
        xiVector[2] * xiScalar);
    }

    public static Vector operator /(Vector xiVector, double xiScalar)
    {
      return new Vector(
        xiVector[0] / xiScalar,
        xiVector[1] / xiScalar,
        xiVector[2] / xiScalar);
    }

    public static Vector operator /(double xiScalar, Vector xiVector)
    {
      return new Vector(
        xiVector[0] / xiScalar,
        xiVector[1] / xiScalar,
        xiVector[2] / xiScalar);
    }

    public static Vector operator -(Vector xiOther)
    {
      return new Vector(-xiOther[0], -xiOther[1], -xiOther[2]);
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

      Vector lOther = (Vector)obj;

      return lOther[0].Equals(mElements[0])
        && lOther[1].Equals(mElements[1])
        && lOther[2].Equals(mElements[2]);
    }

    public static bool operator == (Vector xiLhs, Vector xiRhs)
    {
      return xiLhs.Equals(xiRhs);
    }

    public static bool operator !=(Vector xiLhs, Vector xiRhs)
    {
      return !xiLhs.Equals(xiRhs);
    }

    public override int GetHashCode()
    {
      return base.GetHashCode(); //qq
    }

    private double[] mElements;
  }
}
