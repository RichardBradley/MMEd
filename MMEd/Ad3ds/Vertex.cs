using System;
using System.Collections.Generic;
using System.Text;

namespace MMEd.Ad3ds
{
  struct Vertex
  {
    public Vertex(float xx, float yy, float zz)
    {
      x = xx;
      y = yy;
      z = zz;
    }

    public float x;
    public float y;
    public float z;
  }
}
