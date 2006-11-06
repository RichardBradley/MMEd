using System;
using System.Collections.Generic;
using System.Text;
using GLTK;

// A GLTK.Mesh with a pointer back to its owner

namespace MMEd.Viewers
{
  public class OwnedMesh : Mesh
  {
    public OwnedMesh() { }
    public OwnedMesh(object xiOwner) { this.mOwner = xiOwner; }
    private object mOwner;
    public object Owner
    {
      get { return mOwner; }
      set { mOwner = value; }
    }
  }
}
