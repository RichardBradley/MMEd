using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Diagnostics;
using Tao.OpenGl;

namespace GLTK
{
  public struct Matrix
  {
    public static Matrix Identity = new Matrix(
      1, 0, 0, 0,
      0, 1, 0, 0,
      0, 0, 1, 0,
      0, 0, 0, 1);

    public Matrix(double aa, double ab, double ac, double ad,
        double ba, double bb, double bc, double bd,
        double ca, double cb, double cc, double cd,
        double da, double db, double dc, double dd)
    {
      mElements = new double[4,4];

      mElements[0, 0] = aa; mElements[0, 1] = ab; mElements[0, 2] = ac; mElements[0, 3] = ad;
      mElements[1, 0] = ba; mElements[1, 1] = bb; mElements[1, 2] = bc; mElements[1, 3] = bd;
      mElements[2, 0] = ca; mElements[2, 1] = cb; mElements[2, 2] = cc; mElements[2, 3] = cd;
      mElements[3, 0] = da; mElements[3, 1] = db; mElements[3, 2] = dc; mElements[3, 3] = dd;
    }

    public Matrix(double[,] xiArray)
    {
      mElements = new double[4, 4];

      mElements[0, 0] = xiArray[0, 0]; mElements[0, 1] = xiArray[0, 1]; mElements[0, 2] = xiArray[0, 2]; mElements[0, 3] = xiArray[0, 3];
      mElements[1, 0] = xiArray[1, 0]; mElements[1, 1] = xiArray[1, 1]; mElements[1, 2] = xiArray[1, 2]; mElements[1, 3] = xiArray[1, 3];
      mElements[2, 0] = xiArray[2, 0]; mElements[2, 1] = xiArray[2, 1]; mElements[2, 2] = xiArray[2, 2]; mElements[2, 3] = xiArray[2, 3];
      mElements[3, 0] = xiArray[3, 0]; mElements[3, 1] = xiArray[3, 1]; mElements[3, 2] = xiArray[3, 2]; mElements[3, 3] = xiArray[3, 3];
    }

    public Matrix(double[] xiArray)
    {
      mElements = new double[4, 4];

      mElements[0, 0] = xiArray[0]; mElements[0, 1] = xiArray[1]; mElements[0, 2] = xiArray[2]; mElements[0, 3] = xiArray[3];
      mElements[1, 0] = xiArray[4]; mElements[1, 1] = xiArray[5]; mElements[1, 2] = xiArray[6]; mElements[1, 3] = xiArray[7];
      mElements[2, 0] = xiArray[8]; mElements[2, 1] = xiArray[9]; mElements[2, 2] = xiArray[10]; mElements[2, 3] = xiArray[11];
      mElements[3, 0] = xiArray[12]; mElements[3, 1] = xiArray[13]; mElements[3, 2] = xiArray[14]; mElements[3, 3] = xiArray[15];
    }

    public double Determinant
    {
      get
      {
        return mElements[0, 0] * subDet(0, 0) - mElements[0, 1] * subDet(0, 1)
          + mElements[0, 2] * subDet(0, 2) - mElements[0, 3] * subDet(0, 3);
      }
    }

    public double Trace
    {
      get
      {
        return mElements[0, 0] + mElements[1, 1] + mElements[2, 2] + mElements[3, 3];
      }
    }

    public Matrix Inverse()
    {
      return new Matrix(subDet(0, 0), -subDet(1, 0), subDet(2, 0), -subDet(3, 0),
          -subDet(0, 1), subDet(1, 1), -subDet(2, 1), subDet(3, 1),
          subDet(0, 2), -subDet(1, 2), subDet(2, 2), -subDet(3, 2),
          -subDet(0, 3), subDet(1, 3), -subDet(2, 3), subDet(3, 3)) / Determinant;
    }

    public Matrix Transpose()
    {
      return new Matrix(mElements[0, 0], mElements[1, 0], mElements[2, 0], mElements[3, 0],
      mElements[0, 1], mElements[1, 1], mElements[2, 1], mElements[3, 1],
      mElements[0, 2], mElements[1, 2], mElements[2, 2], mElements[3, 2],
      mElements[0, 3], mElements[1, 3], mElements[2, 3], mElements[3, 3]);
    }

    public double this[int x, int y]
    {
      get
      {
        return mElements[x, y];
      }
    }

    public override bool Equals(object obj)
    {
      if (obj == null)
      {
        return false;
      }

      if (ReferenceEquals(this, obj))
      {
        return true;
      }

      Matrix lOther = (Matrix)obj;

      return
    mElements[0, 0] == lOther[0, 0] && mElements[0, 1] == lOther[0, 1] && mElements[0, 2] == lOther[0, 2] && mElements[0, 3] == lOther[0, 3] &&
    mElements[1, 0] == lOther[1, 0] && mElements[1, 1] == lOther[1, 1] && mElements[1, 2] == lOther[1, 2] && mElements[1, 3] == lOther[1, 3] &&
    mElements[2, 0] == lOther[2, 0] && mElements[2, 1] == lOther[2, 1] && mElements[2, 2] == lOther[2, 2] && mElements[2, 3] == lOther[2, 3] &&
    mElements[3, 0] == lOther[3, 0] && mElements[3, 1] == lOther[3, 1] && mElements[3, 2] == lOther[3, 2] && mElements[3, 3] == lOther[3, 3];
    }

    public override int GetHashCode()
    {
      return base.GetHashCode(); //qq
    }

    public static bool operator ==(Matrix xiLhs, Matrix xiRhs)
    {
      return xiLhs.Equals(xiRhs);
    }

    public static bool operator !=(Matrix xiLhs, Matrix xiRhs)
    {
      return !xiLhs.Equals(xiRhs);
    }


    public static Matrix operator +(Matrix xiLhs, Matrix xiRhs)
    {
      return new Matrix(xiLhs[0, 0] + xiRhs[0, 0], xiLhs[0, 1] + xiRhs[0, 1], xiLhs[0, 2] + xiRhs[0, 2], xiLhs[0, 3] + xiRhs[0, 3],
        xiLhs[1, 0] + xiRhs[1, 0], xiLhs[1, 1] + xiRhs[1, 1], xiLhs[1, 2] + xiRhs[1, 2], xiLhs[1, 3] + xiRhs[1, 3],
        xiLhs[2, 0] + xiRhs[2, 0], xiLhs[2, 1] + xiRhs[2, 1], xiLhs[2, 2] + xiRhs[2, 2], xiLhs[2, 3] + xiRhs[2, 3],
        xiLhs[3, 0] + xiRhs[3, 0], xiLhs[3, 1] + xiRhs[3, 1], xiLhs[3, 2] + xiRhs[3, 2], xiLhs[3, 3] + xiRhs[3, 3]);
    }

    public static Matrix operator -(Matrix xiLhs, Matrix xiRhs)
    {
      return new Matrix(xiLhs[0, 0] - xiRhs[0, 0], xiLhs[0, 1] - xiRhs[0, 1], xiLhs[0, 2] - xiRhs[0, 2], xiLhs[0, 3] - xiRhs[0, 3],
        xiLhs[1, 0] - xiRhs[1, 0], xiLhs[1, 1] - xiRhs[1, 1], xiLhs[1, 2] - xiRhs[1, 2], xiLhs[1, 3] - xiRhs[1, 3],
        xiLhs[2, 0] - xiRhs[2, 0], xiLhs[2, 1] - xiRhs[2, 1], xiLhs[2, 2] - xiRhs[2, 2], xiLhs[2, 3] - xiRhs[2, 3],
        xiLhs[3, 0] - xiRhs[3, 0], xiLhs[3, 1] - xiRhs[3, 1], xiLhs[3, 2] - xiRhs[3, 2], xiLhs[3, 3] - xiRhs[3, 3]);
    }

    public static Matrix operator *(Matrix xiLhs, Matrix xiRhs)
    {
      return new Matrix(
        xiLhs[0, 0] * xiRhs[0, 0] + xiLhs[0, 1] * xiRhs[1, 0] + xiLhs[0, 2] * xiRhs[2, 0] + xiLhs[0, 3] * xiRhs[3, 0],
        xiLhs[0, 0] * xiRhs[0, 1] + xiLhs[0, 1] * xiRhs[1, 1] + xiLhs[0, 2] * xiRhs[2, 1] + xiLhs[0, 3] * xiRhs[3, 1],
        xiLhs[0, 0] * xiRhs[0, 2] + xiLhs[0, 1] * xiRhs[1, 2] + xiLhs[0, 2] * xiRhs[2, 2] + xiLhs[0, 3] * xiRhs[3, 2],
        xiLhs[0, 0] * xiRhs[0, 3] + xiLhs[0, 1] * xiRhs[1, 3] + xiLhs[0, 2] * xiRhs[2, 3] + xiLhs[0, 3] * xiRhs[3, 3],

        xiLhs[1, 0] * xiRhs[0, 0] + xiLhs[1, 1] * xiRhs[1, 0] + xiLhs[1, 2] * xiRhs[2, 0] + xiLhs[1, 3] * xiRhs[3, 0],
        xiLhs[1, 0] * xiRhs[0, 1] + xiLhs[1, 1] * xiRhs[1, 1] + xiLhs[1, 2] * xiRhs[2, 1] + xiLhs[1, 3] * xiRhs[3, 1],
        xiLhs[1, 0] * xiRhs[0, 2] + xiLhs[1, 1] * xiRhs[1, 2] + xiLhs[1, 2] * xiRhs[2, 2] + xiLhs[1, 3] * xiRhs[3, 2],
        xiLhs[1, 0] * xiRhs[0, 3] + xiLhs[1, 1] * xiRhs[1, 3] + xiLhs[1, 2] * xiRhs[2, 3] + xiLhs[1, 3] * xiRhs[3, 3],

        xiLhs[2, 0] * xiRhs[0, 0] + xiLhs[2, 1] * xiRhs[1, 0] + xiLhs[2, 2] * xiRhs[2, 0] + xiLhs[2, 3] * xiRhs[3, 0],
        xiLhs[2, 0] * xiRhs[0, 1] + xiLhs[2, 1] * xiRhs[1, 1] + xiLhs[2, 2] * xiRhs[2, 1] + xiLhs[2, 3] * xiRhs[3, 1],
        xiLhs[2, 0] * xiRhs[0, 2] + xiLhs[2, 1] * xiRhs[1, 2] + xiLhs[2, 2] * xiRhs[2, 2] + xiLhs[2, 3] * xiRhs[3, 2],
        xiLhs[2, 0] * xiRhs[0, 3] + xiLhs[2, 1] * xiRhs[1, 3] + xiLhs[2, 2] * xiRhs[2, 3] + xiLhs[2, 3] * xiRhs[3, 3],

        xiLhs[3, 0] * xiRhs[0, 0] + xiLhs[3, 1] * xiRhs[1, 0] + xiLhs[3, 2] * xiRhs[2, 0] + xiLhs[3, 3] * xiRhs[3, 0],
        xiLhs[3, 0] * xiRhs[0, 1] + xiLhs[3, 1] * xiRhs[1, 1] + xiLhs[3, 2] * xiRhs[2, 1] + xiLhs[3, 3] * xiRhs[3, 1],
        xiLhs[3, 0] * xiRhs[0, 2] + xiLhs[3, 1] * xiRhs[1, 2] + xiLhs[3, 2] * xiRhs[2, 2] + xiLhs[3, 3] * xiRhs[3, 2],
        xiLhs[3, 0] * xiRhs[0, 3] + xiLhs[3, 1] * xiRhs[1, 3] + xiLhs[3, 2] * xiRhs[2, 3] + xiLhs[3, 3] * xiRhs[3, 3]);
    }

    public static Matrix operator *(Matrix xiMatrix, double xiScalar)
    {
      return new Matrix(
        xiMatrix[0, 0] * xiScalar, xiMatrix[0, 1] * xiScalar, xiMatrix[0, 2] * xiScalar, xiMatrix[0, 3] * xiScalar,
        xiMatrix[1, 0] * xiScalar, xiMatrix[1, 1] * xiScalar, xiMatrix[1, 2] * xiScalar, xiMatrix[1, 3] * xiScalar,
        xiMatrix[2, 0] * xiScalar, xiMatrix[2, 1] * xiScalar, xiMatrix[2, 2] * xiScalar, xiMatrix[2, 3] * xiScalar,
        xiMatrix[3, 0] * xiScalar, xiMatrix[3, 1] * xiScalar, xiMatrix[3, 2] * xiScalar, xiMatrix[3, 3] * xiScalar);
    }

    public static Matrix operator /(Matrix xiMatrix, double xiScalar)
    {
      return new Matrix(
        xiMatrix[0, 0] / xiScalar, xiMatrix[0, 1] / xiScalar, xiMatrix[0, 2] / xiScalar, xiMatrix[0, 3] / xiScalar,
        xiMatrix[1, 0] / xiScalar, xiMatrix[1, 1] / xiScalar, xiMatrix[1, 2] / xiScalar, xiMatrix[1, 3] / xiScalar,
        xiMatrix[2, 0] / xiScalar, xiMatrix[2, 1] / xiScalar, xiMatrix[2, 2] / xiScalar, xiMatrix[2, 3] / xiScalar,
        xiMatrix[3, 0] / xiScalar, xiMatrix[3, 1] / xiScalar, xiMatrix[3, 2] / xiScalar, xiMatrix[3, 3] / xiScalar);
    }

    public static Matrix operator *(double xiScalar, Matrix xiMatrix)
    {
      return new Matrix(
        xiMatrix[0, 0] * xiScalar, xiMatrix[0, 1] * xiScalar, xiMatrix[0, 2] * xiScalar, xiMatrix[0, 3] * xiScalar,
        xiMatrix[1, 0] * xiScalar, xiMatrix[1, 1] * xiScalar, xiMatrix[1, 2] * xiScalar, xiMatrix[1, 3] * xiScalar,
        xiMatrix[2, 0] * xiScalar, xiMatrix[2, 1] * xiScalar, xiMatrix[2, 2] * xiScalar, xiMatrix[2, 3] * xiScalar,
        xiMatrix[3, 0] * xiScalar, xiMatrix[3, 1] * xiScalar, xiMatrix[3, 2] * xiScalar, xiMatrix[3, 3] * xiScalar);
    }

    public static Vector operator *(Matrix xiMatrix, Vector xiVector)
    {
      return new Vector(
        xiMatrix[0, 0] * xiVector[0] + xiMatrix[0, 1] * xiVector[1] + xiMatrix[0, 2] * xiVector[2],
        xiMatrix[1, 0] * xiVector[0] + xiMatrix[1, 1] * xiVector[1] + xiMatrix[1, 2] * xiVector[2],
        xiMatrix[2, 0] * xiVector[0] + xiMatrix[2, 1] * xiVector[1] + xiMatrix[2, 2] * xiVector[2]);
    }


    public static Point operator *(Matrix xiMatrix, Point xiPoint)
    {
      return new Point(
        xiMatrix[0, 0] * xiPoint[0] + xiMatrix[0, 1] * xiPoint[1] + xiMatrix[0, 2] * xiPoint[2] + xiMatrix[0, 3],
        xiMatrix[1, 0] * xiPoint[0] + xiMatrix[1, 1] * xiPoint[1] + xiMatrix[1, 2] * xiPoint[2] + xiMatrix[1, 3],
        xiMatrix[2, 0] * xiPoint[0] + xiMatrix[2, 1] * xiPoint[1] + xiMatrix[2, 2] * xiPoint[2] + xiMatrix[2, 3]);
    }

    private double subDet(int k, int l)
    {
      double[] mat33 = new double[9];
      int a = 0;
      // Copy the 3x3 matrix into mat33[]
      for (int i = 0; i < 4; i++)
      {
        if (i != k)
        {		// skip the kth row
          for (int j = 0; j < 4; j++)
          {
            if (j != l)
            { //skip the lth column
              mat33[a] = mElements[i, j];
              a++;
            }
          }
        }
      }
      // Return the determinant of the 3x3 matrix
      return mat33[0] * (mat33[4] * mat33[8] - mat33[5] * mat33[7])
          - mat33[1] * (mat33[3] * mat33[8] - mat33[5] * mat33[6])
          + mat33[2] * (mat33[3] * mat33[7] - mat33[4] * mat33[6]);
    }

    public double[] ToArray()
    {
      return new double[16] {
        mElements[0, 0], mElements[0, 1], mElements[0, 2], mElements[0, 3],
        mElements[1, 0], mElements[1, 1], mElements[1, 2], mElements[1, 3],
        mElements[2, 0], mElements[2, 1], mElements[2, 2], mElements[2, 3],
        mElements[3, 0], mElements[3, 1], mElements[3, 2], mElements[3, 3] };
    }

    public Matrix Clone()
    {
      return new Matrix((double[,])mElements.Clone());
    }

    private double[,] mElements;

  }
}

