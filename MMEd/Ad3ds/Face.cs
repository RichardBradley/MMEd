using System;
using System.Collections.Generic;
using System.Text;

namespace MMEd.Ad3ds
{
  struct Face
  {
    public Face(ushort xi1, ushort xi2, ushort xi3)
    {
      Vertex1 = xi1;
      Vertex2 = xi2;
      Vertex3 = xi3;
    }
    public ushort Vertex1;
    public ushort Vertex2;
    public ushort Vertex3;
  }
}
