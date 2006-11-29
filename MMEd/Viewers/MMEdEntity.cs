using System;
using System.Collections.Generic;
using System.Text;

namespace MMEd.Viewers
{
  public class MMEdEntity : GLTK.Entity
  {
    public MMEdEntity() { }

    public MMEdEntity(object xiOwner)
    {
      mOwner = xiOwner;
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
