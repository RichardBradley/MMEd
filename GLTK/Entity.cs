using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Diagnostics;
using Tao.OpenGl;

namespace GLTK
{
  public class Entity
  {
    public Point Position
    {
      get
      {
        return new Point(mTransform[3, 0], mTransform[3, 1], mTransform[3, 2]);
      }
      set
      {
        mTransform = new Matrix(
          mTransform[0, 0], mTransform[0, 1], mTransform[0, 2], mTransform[0, 3],
          mTransform[1, 0], mTransform[1, 1], mTransform[1, 2], mTransform[1, 3],
          mTransform[2, 0], mTransform[2, 1], mTransform[2, 2], mTransform[2, 3],
          value.x, value.y, value.z, mTransform[3, 3]);
      }
    }

    public void Move(Vector xiVector)
    {
      Matrix lTransform = new Matrix(
        1, 0, 0, 0,
        0, 1, 0, 0,
        0, 0, 1, 0,
        xiVector.x, xiVector.y, xiVector.z, 1);

      mTransform = mTransform * lTransform;

      Reorthogonalise();
    }

    public void Rotate(double xiAngle, Vector xiAxis)
    {
      if (xiAxis.Length == 0)
      {
        return;
      }

      double t = 1 - Math.Cos(xiAngle);
      double s = Math.Sin(xiAngle);
      double c = Math.Cos(xiAngle);
      double x = xiAxis.x / xiAxis.Length;
      double y = xiAxis.y / xiAxis.Length;
      double z = xiAxis.z / xiAxis.Length;

      Matrix lRotation = new Matrix(
        t * x * x + c, t * x * y - s * z, t * x * z + s * y, 0,
        t * x * y + s * z, t * y * y + c, t * y * z - s * x, 0,
        t * x * z - s * y, t * y * z + s * x, t * z * z + c, 0,
        0, 0, 0, 1);

      mTransform = mTransform * lRotation;

      Reorthogonalise();
    }

    //the other "rotate" rotates about the world origin
    //this one should rotate about the object's origin.
    //I'm too confused to change all 'rotate's into 'rotate2's
    public void Rotate2(double xiAngle, Vector xiAxis)
    {
      Point start = Position;
      Position = Point.Origin;
      Rotate(xiAngle, xiAxis);
      Position = start;
    }

    public void LookAt(Point xiTarget, Vector xiUp)
    {
      Vector lV1 = (Position - xiTarget).Normalise();
      Vector lV2 = xiUp.Normalise();

      Vector lV3 = lV2 ^ lV1;
      lV2 = lV1 ^ lV3;

      mTransform = new Matrix(
        lV3.x, lV3.y, lV3.z, 0,
        lV2.x, lV2.y, lV2.z, 0,
        lV1.x, lV1.y, lV1.z, 0,
        mTransform[3, 0], mTransform[3, 1], mTransform[3, 2], mTransform[3, 3]);

      Reorthogonalise();
    }

    public void Scale(double x, double y, double z)
    {
      Matrix lScale = new Matrix(
        x, 0, 0, 0,
        0, y, 0, 0,
        0, 0, z, 0,
        0, 0, 0, 1);

      mTransform = mTransform * lScale;
    }

    public Matrix Transform
    {
      get { return mTransform; }
      set { mTransform = value; }
    }

    public Vector XAxis
    {
      get { return new Vector(mTransform[0, 0], mTransform[0, 1], mTransform[0, 2]).Normalise(); }
    }

    public Vector YAxis
    {
      get { return new Vector(mTransform[1, 0], mTransform[1, 1], mTransform[1, 2]).Normalise(); }
    }

    public Vector ZAxis
    {
      get { return new Vector(mTransform[2, 0], mTransform[2, 1], mTransform[2, 2]).Normalise(); }
    }

    private void Reorthogonalise()
    {
      ++mOperationCount;
      if (mOperationCount > 50)
      {
        mOperationCount = 0;

        Vector lColumn1 = new Vector(mTransform[1, 0], mTransform[1, 1], mTransform[1, 2]);
        Vector lColumn2 = new Vector(mTransform[2, 0], mTransform[2, 1], mTransform[2, 2]);

        Vector lNewColumn0 = (lColumn1 ^ lColumn2).Normalise();
        Vector lNewColumn1 = lColumn1.Normalise();
        Vector lNewColumn2 = (lNewColumn0 ^ lColumn1).Normalise();

        mTransform = new Matrix(
          lNewColumn0.x, lNewColumn0.y, lNewColumn0.z, 0,
          lNewColumn1.x, lNewColumn1.y, lNewColumn1.z, 0,
          lNewColumn2.x, lNewColumn2.y, lNewColumn2.z, 0,
          mTransform[3, 0], mTransform[3, 1], mTransform[3, 2], mTransform[3, 3]);
      }
    }

    public List<Mesh> Meshes
    {
      get { return mMeshes; }
      set { mMeshes = value; }
    }

    private List<Mesh> mMeshes = new List<Mesh>();
    private int mOperationCount = 0;
    private Matrix mTransform = Matrix.Identity;
  }
}


