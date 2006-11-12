using System;
using System.Collections.Generic;
using System.Text;

using GLTK;

using MMEd.Chunks;

namespace MMEd.Viewers.ThreeDee
{
  public enum eMovementMode
  {
    FlyMode,
    InspectMode
  }

  public enum eDrawNormalsMode
  {
    DrawNormals,
    HideNormals
  }

  public enum eLightingMode
  {
    None,
    Headlight,
    OverheadLight
  }

  public enum eTextureMode
  {
    NormalTextures,
    NormalTexturesWithMetadata,
    BumpmapTextures
  }

  //indicates that an object can provide an enumeration of GLTK.Entity objects
  //to be redered as a ThreeDee scene
  public interface IEntityProvider
  {
    IEnumerable<Entity> GetEntities(Level xiLevel, eTextureMode xiTextureMode, eTexMetaDataEntries xiSelectedMetadata);
  }

  public interface IPositionable
  {
    Short3Coord OriginPosition
    {
      get;
      set;
    }
  }

  public interface IRotatable
  {
    Short3Coord RotationVector
    {
      get;
      set;
    }
  }
}
