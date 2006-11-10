using System;
using System.Collections.Generic;
using System.Text;

namespace MMEd.Viewers
{
  class MMEdEntity : GLTK.Entity
  {
    public MMEdEntity(object xiOwner)
    {
      mOwner = xiOwner;
    }

    public object Owner
    {
      get { return mOwner; }
      set { mOwner = value; }
    }

    private object mOwner;
  }
}
