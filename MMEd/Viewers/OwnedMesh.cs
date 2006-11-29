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

    public OwnedMesh(object xiOwner, PolygonMode xiMode) 
      : base(xiMode) 
    { 
      this.mOwner = xiOwner; 
    }

    [System.Xml.Serialization.XmlIgnore()]
    public object Owner
    {
      get { return mOwner; }
      set { mOwner = value; }
    }
    private object mOwner;
  }
}
