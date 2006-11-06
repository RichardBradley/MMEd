using System;
using System.Collections.Generic;
using System.Text;

namespace GLTK
{
  public enum RenderMode
  {
    Points,
    Wireframe,
    Filled,
    Textured
  }

  public class Mesh
  {
    public List<Vertex> Vertices
    {
      get { return mVertices; }
    }

    public void AddTriangle(Vertex v1, Vertex v2, Vertex v3)
    {
      double d = v1.Normal.x; d = v2.Normal.x; d = v3.Normal.x; //qq
      mVertices.Add(v1);
      mVertices.Add(v2);
      mVertices.Add(v3);
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

    List<Vertex> mVertices = new List<Vertex>();
    int mTexture = 0;
    RenderMode mRenderMode = RenderMode.Textured;
  }
}
