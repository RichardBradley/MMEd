using System;
using System.Collections.Generic;
using System.Text;

namespace GLTK
{
  public enum RenderMode
  {
    Undefined =0,
    Points = 1,
    Wireframe = 2,
    Filled = 3,
    Textured = 4
  }

  // the values of the entries in this enumeration should
  // be the vertex count of the faces
  public enum PolygonMode
  {
    Triangles = 3,
    Lines = 2,
    Quads = 4
  }

  public class Mesh
  {
    public Mesh()
    {
    }

    public Mesh(PolygonMode xiMode)
    {
      mPolygonMode = xiMode;
    }

    public List<Vertex> Vertices
    {
      get { return mVertices; }
    }

    public void AddFace(params Vertex[] xiVertices)
    {
      int lExpectedLength = 0;
      switch (mPolygonMode)
      {
        case PolygonMode.Lines:
          lExpectedLength  = 2;
          break;
        case PolygonMode.Triangles:
          lExpectedLength = 3;
          break;
        case PolygonMode.Quads:
          lExpectedLength = 4;
          break;
      }

      if (xiVertices.Length != lExpectedLength)
      {
        throw new Exception("Incorrect number of vertices");
      }

      mVertices.AddRange(xiVertices);
    }

    public int Texture
    {
      get { return mTexture; }
      set { mTexture = value; }
    }

    public RenderMode RenderMode
    {
      get { return mRenderMode; }
      set { mRenderMode = value; }
    }

    public PolygonMode PolygonMode
    {
      get { return mPolygonMode; }
    }

    List<Vertex> mVertices = new List<Vertex>();
    int mTexture = 0;
    RenderMode mRenderMode = RenderMode.Textured;
    PolygonMode mPolygonMode = PolygonMode.Triangles;
  }
}
