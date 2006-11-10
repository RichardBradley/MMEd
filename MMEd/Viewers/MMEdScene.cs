using System;
using System.Collections.Generic;
using System.Text;

namespace MMEd.Viewers
{
  class MMEdScene : GLTK.Scene
  {
    public override void AddObject(GLTK.Entity xiObject)
    {
      base.AddObject(xiObject);
      MMEdEntity lMe = xiObject as MMEdEntity;
      if (lMe != null && lMe.Owner != null)
      {
        mObjects[lMe.Owner] = lMe;
      }
    }

    public override void AddRange(IEnumerable<GLTK.Entity> xiCollection)
    {
      base.AddRange(xiCollection);

      foreach (GLTK.Entity lEntity in xiCollection)
      {
        MMEdEntity lMe = lEntity as MMEdEntity;
        if (lMe != null && lMe.Owner != null)
        {
          mObjects[lMe.Owner] = lMe;
        }
      }
    }

    public override void RemoveObject(GLTK.Entity xiObject)
    {
      base.RemoveObject(xiObject);
      MMEdEntity lMe = xiObject as MMEdEntity;
      if (lMe != null && mObjects.ContainsKey(lMe.Owner))
      {
        mObjects.Remove(lMe.Owner);
      }
    }

    public void RemoveMMEdObject(object xiObject)
    {
      if (mObjects.ContainsKey(xiObject))
      {
        RemoveObject(mObjects[xiObject]);
      }
    }

    Dictionary<object, GLTK.Entity> mObjects = new Dictionary<object, GLTK.Entity>();
  }
}
