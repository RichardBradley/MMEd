using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;

namespace GLTK
{
  /// <summary>
  /// A 4x4 matrix representing a linear transformation of 3-space, with a translation.
  /// 
  /// To mesh better with OpenGL, the elements are stored internally in column-major
  /// order, i.e. this[y,x] -- hence this[0,x] gives the first row for x=0,1,2,3
  ///
  /// Confusingly, the list constructor Matrix(a,b,c....) accepts arguments in row-major
  /// order.
  /// </summary>
  public struct Matrix : IXmlSerializable
  {
    public static Matrix Identity = new Matrix(
      1, 0, 0, 0,
      0, 1, 0, 0,
      0, 0, 1, 0,
      0, 0, 0, 1);

    public static Matrix Rotation(double xiAngle, Vector xiAxis)
    {
      if (xiAxis.Length == 0)
      {
        throw new Exception("Cannot have axis of zero length");
      }

      double t = 1 - Math.Cos(xiAngle);
      double s = Math.Sin(xiAngle);
      double c = Math.Cos(xiAngle);
      double x = xiAxis.x / xiAxis.Length;
      double y = xiAxis.y / xiAxis.Length;
      double z = xiAxis.z / xiAxis.Length;

      return new Matrix(
        t * x * x + c, t * x * y - s * z, t * x * z + s * y, 0,
        t * x * y + s * z, t * y * y + c, t * y * z - s * x, 0,
        t * x * z - s * y, t * y * z + s * x, t * z * z + c, 0,
        0, 0, 0, 1);
    }

    public static Matrix Translation(double x, double y, double z)
    {
      return new Matrix(
  1, 0, 0, 0,
  0, 1, 0, 0,
  0, 0, 1, 0,
  x, y, z, 1);
    }

    public static Matrix Translation(Vector xiVector)
    {
      return Translation(xiVector.x, xiVector.y, xiVector.z);
    }

    public static Matrix ScalingMatrix(double x, double y, double z)
    {
      return new Matrix(
        x, 0, 0, 0,
        0, y, 0, 0,
        0, 0, z, 0,
        0, 0, 0, 1);
    }

    /// <summary>
    /// Constructs a new matrix, given the following entries (in row-major order),
    /// i.e. (r1_x, r1_y, r1_z, 0, r2_x, ...)
    /// 
    /// The values will be stored internally in column-major order.
    /// </summary>
    public Matrix(double aa, double ab, double ac, double ad,
        double ba, double bb, double bc, double bd,
        double ca, double cb, double cc, double cd,
        double da, double db, double dc, double dd)
    {
      mElements = new double[4, 4];
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

    // TODO this method will perform very badly in certain cases
    // see e.g. the public domain JAMA library for examples of how to 
    // fix this.
    //
    // We could use e.g. alglib instead
    // http://www.alglib.net/translator/man/manual.csharp.html
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

    /// <summary>
    /// TODO: document this method...
    /// 
    /// Not 100% sure what this does -- it came from the GLTK code.
    /// I think it keeps the rotations the same but makes them more 'normalised'?
    /// </summary>
    public Matrix Orthogonalise()
    {
      Vector lColumn1 = new Vector(this[1, 0], this[1, 1], this[1, 2]);
      Vector lColumn2 = new Vector(this[2, 0], this[2, 1], this[2, 2]);

      Vector lNewColumn0 = (lColumn1 ^ lColumn2).Normalise();
      Vector lNewColumn1 = lColumn1.Normalise();
      Vector lNewColumn2 = (lNewColumn0 ^ lColumn1).Normalise();

      return new Matrix(
        lNewColumn0.x, lNewColumn0.y, lNewColumn0.z, 0,
        lNewColumn1.x, lNewColumn1.y, lNewColumn1.z, 0,
        lNewColumn2.x, lNewColumn2.y, lNewColumn2.z, 0,
        this[3, 0], this[3, 1], this[3, 2], this[3, 3]);
    }

    /// <summary>
    /// If possible, decomposes this matrix into S*R*T
    /// for a translation T, a rotation R and a scaling S.
    ///
    /// The rotation is represented as an angle in radians about an axis.
    ///
    /// http://graphcomp.com/info/specs/sgi/vrml/spec/part1/concepts.html#CoordinateSystems
    /// http://graphcomp.com/info/specs/sgi/vrml/spec/part1/nodesRef.html#Transform
    /// </summary>
    public void Decompose(out Vector translation, out Vector rotationAxis, out double rotationAngle, out Vector scale)
    {

      translation = new Vector(this[3, 0], this[3, 1], this[3, 2]);

      var M11 = this[0, 0];
      var M12 = this[0, 1];
      var M13 = this[0, 2];
      var M14 = this[0, 3];
      var M21 = this[1, 0];
      var M22 = this[1, 1];
      var M23 = this[1, 2];
      var M24 = this[1, 3];
      var M31 = this[2, 0];
      var M32 = this[2, 1];
      var M33 = this[2, 2];
      var M34 = this[2, 3];
      var M41 = this[3, 0];
      var M42 = this[3, 1];
      var M43 = this[3, 2];

      float xs = (Math.Sign(M11 * M12 * M13 * M14) < 0) ? -1f : 1f;
      float ys = (Math.Sign(M21 * M22 * M23 * M24) < 0) ? -1f : 1f;
      float zs = (Math.Sign(M31 * M32 * M33 * M34) < 0) ? -1f : 1f;

      var scaleX = xs * (float)Math.Sqrt(M11 * M11 + M12 * M12 + M13 * M13);
      var scaleY = ys * (float)Math.Sqrt(M21 * M21 + M22 * M22 + M23 * M23);
      var scaleZ = zs * (float)Math.Sqrt(M31 * M31 + M32 * M32 + M33 * M33);
      scale = new Vector(scaleX,scaleY,scaleZ);

      if (scaleX == 0.0 || scaleY == 0.0 || scaleZ == 0.0)
      {
        rotationAngle = 0;
        rotationAxis = new Vector(1, 0, 0);
        return;
      }

      Matrix m1 = new Matrix(M11 / scaleX, M12 / scaleX, M13 / scaleX, 0,
                             M21 / scaleY, M22 / scaleY, M23 / scaleY, 0,
                             M31 / scaleZ, M32 / scaleZ, M33 / scaleZ, 0,
                             0, 0, 0, 1);

      var rotation = Quaternion.CreateFromRotationMatrix(m1);

      rotation.ToAxisAngle(out rotationAxis, out rotationAngle);
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

      Matrix lOther = (Matrix) obj;

      return
        FloatHelper.AlmostEqual(mElements[0, 0], lOther[0, 0]) &&
        FloatHelper.AlmostEqual(mElements[0, 1], lOther[0, 1]) &&
        FloatHelper.AlmostEqual(mElements[0, 2], lOther[0, 2]) &&
        FloatHelper.AlmostEqual(mElements[0, 3], lOther[0, 3]) &&
        FloatHelper.AlmostEqual(mElements[1, 0], lOther[1, 0]) &&
        FloatHelper.AlmostEqual(mElements[1, 1], lOther[1, 1]) &&
        FloatHelper.AlmostEqual(mElements[1, 2], lOther[1, 2]) &&
        FloatHelper.AlmostEqual(mElements[1, 3], lOther[1, 3]) &&
        FloatHelper.AlmostEqual(mElements[2, 0], lOther[2, 0]) &&
        FloatHelper.AlmostEqual(mElements[2, 1], lOther[2, 1]) &&
        FloatHelper.AlmostEqual(mElements[2, 2], lOther[2, 2]) &&
        FloatHelper.AlmostEqual(mElements[2, 3], lOther[2, 3]) &&
        FloatHelper.AlmostEqual(mElements[3, 0], lOther[3, 0]) &&
        FloatHelper.AlmostEqual(mElements[3, 1], lOther[3, 1]) &&
        FloatHelper.AlmostEqual(mElements[3, 2], lOther[3, 2]) &&
        FloatHelper.AlmostEqual(mElements[3, 3], lOther[3, 3]);
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

    /// <summary>
    /// Applies this matrix to the given Vector, excluding any transformation
    /// (because Vectors are location-independent directions -- if you want
    /// translation included then use Points instead)
    /// </summary>
    public static Vector operator *(Matrix xiMatrix, Vector xiVector)
    {
      return new Vector(
        xiMatrix[0, 0] * xiVector[0] + xiMatrix[0, 1] * xiVector[1] + xiMatrix[0, 2] * xiVector[2],
        xiMatrix[1, 0] * xiVector[0] + xiMatrix[1, 1] * xiVector[1] + xiMatrix[1, 2] * xiVector[2],
        xiMatrix[2, 0] * xiVector[0] + xiMatrix[2, 1] * xiVector[1] + xiMatrix[2, 2] * xiVector[2]);
    }

    /// <summary>
    /// Applies this matrix to the given Point, including any transformation
    /// </summary>
    public static Point operator *(Matrix xiMatrix, Point xiPoint)
    {
      // This looks like we are doing (xiMatrix.Transpose() * xiPoint), but remember
      // that our indices are in column-major order, so this is actually correct.
      return new Point(
        xiMatrix[0, 0] * xiPoint[0] + xiMatrix[0, 1] * xiPoint[1] + xiMatrix[0, 2] * xiPoint[2] + xiMatrix[0, 3],
        xiMatrix[1, 0] * xiPoint[0] + xiMatrix[1, 1] * xiPoint[1] + xiMatrix[1, 2] * xiPoint[2] + xiMatrix[1, 3],
        xiMatrix[2, 0] * xiPoint[0] + xiMatrix[2, 1] * xiPoint[1] + xiMatrix[2, 2] * xiPoint[2] + xiMatrix[2, 3]);
    }

    /// <summary>
    /// left-applies this matrix to the given Point, including any transformation
    /// </summary>
    public static Point operator *(Point xiPoint, Matrix xiMatrix)
    {
      // This looks like we are doing (xiMatrix.Transpose() * xiPoint), but remember
      // that our indices are in column-major order, so this is actually correct.
      return new Point(
        xiMatrix[0, 0] * xiPoint[0] + xiMatrix[1, 0] * xiPoint[1] + xiMatrix[2, 0] * xiPoint[2] + xiMatrix[3, 0],
        xiMatrix[0, 1] * xiPoint[0] + xiMatrix[1, 1] * xiPoint[1] + xiMatrix[2, 1] * xiPoint[2] + xiMatrix[3, 1],
        xiMatrix[0, 2] * xiPoint[0] + xiMatrix[1, 2] * xiPoint[1] + xiMatrix[2, 2] * xiPoint[2] + xiMatrix[3, 2]);
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

    public override string ToString()
    {
      return string.Format(
        @"[{0:e},{1:e},{2:e},{3:e},
{4:e},{5:e},{6:e},{7:e},
{8:e},{9:e},{10:e},{11:e},
{12:e},{13:e},{14:e},{15:e}]",
        // note column-major ordering:
        this[0, 0], this[0, 1], this[0, 2], this[0, 3],
        this[1, 0], this[1, 1], this[1, 2], this[1, 3],
        this[2, 0], this[2, 1], this[2, 2], this[2, 3],
        this[3, 0], this[3, 1], this[3, 2], this[3, 3]);
    }

    public Matrix Clone()
    {
      return new Matrix((double[,])mElements.Clone());
    }

    #region IXmlSerializable implementation

    public void WriteXml(XmlWriter writer)
    {
      for (int x = 0; x < 4; x++)
      {
        for (int y = 0; y < 4; y++)
        {
          writer.WriteElementString("Element", mElements[x,y].ToString());
        }
      }
    }

    public void ReadXml(XmlReader reader)
    {
      //the first element will be the name of the field. We just have to consume it!!
      reader.ReadStartElement();
      mElements = new double[4, 4];
      for (int x = 0; x < 4; x++)
      {
        for (int y = 0; y < 4; y++)
        {
          mElements[x, y] = reader.ReadElementContentAsDouble("Element", reader.NamespaceURI);
        }
      }
      reader.ReadEndElement();
    }

    public XmlSchema GetSchema()
    {
      return (null);
    }

    #endregion

    private double[,] mElements;
  }
}

