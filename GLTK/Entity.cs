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
      mTransform = mTransform * Matrix.Translation(xiVector);

      Reorthogonalise();
    }

    public void RotateAboutWorldOrigin(double xiAngle, Vector xiAxis)
    {
      mTransform = mTransform * Matrix.Rotation(xiAngle, xiAxis);

      Reorthogonalise();
    }

    //rotates about the Entity's current origin.
    public void Rotate(double xiAngle, Vector xiAxis)
    {
      Point lStart = Position;
      Position = Point.Origin;
      RotateAboutWorldOrigin(xiAngle, xiAxis);
      Position = lStart;
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
        mTransform = GetTransformOrthogonalised();
      }
    }

    public Matrix GetTransformOrthogonalised()
    {
      return mTransform.Orthogonalise();
    }

    public List<Mesh> Meshes
    {
      get { return mMeshes; }
      set { mMeshes = value; }
    }

    public Cuboid GetBoundingBox()
    {
      Cuboid lRet = new Cuboid();
      lRet.XMin = double.MaxValue;
      lRet.YMin = double.MaxValue;
      lRet.ZMin = double.MaxValue;
      lRet.XMax = double.MinValue;
      lRet.YMax = double.MinValue;
      lRet.ZMax = double.MinValue;

      foreach (Mesh lMesh in Meshes)
      {
        foreach (Vertex lVertex in lMesh.Vertices)
        {
          if (lVertex.Position.x < lRet.XMin) lRet.XMin = lVertex.Position.x;
          if (lVertex.Position.x > lRet.XMax) lRet.XMax = lVertex.Position.x;
          if (lVertex.Position.y < lRet.YMin) lRet.YMin = lVertex.Position.y;
          if (lVertex.Position.y > lRet.YMax) lRet.YMax = lVertex.Position.y;
          if (lVertex.Position.z < lRet.ZMin) lRet.ZMin = lVertex.Position.z;
          if (lVertex.Position.z > lRet.ZMax) lRet.ZMax = lVertex.Position.z;
        }
      }
      return lRet;
    }

    private List<Mesh> mMeshes = new List<Mesh>();
    private int mOperationCount = 0;
    private Matrix mTransform = Matrix.Identity;
  }
}


