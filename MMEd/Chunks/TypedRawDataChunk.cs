using System;
using System.Collections.Generic;
using System.Text;

// a raw data chunk with an integer type code

namespace MMEd.Chunks
{
  public class TypedRawDataChunk : RawDataChunk
  {
    public TypedRawDataChunk() { }
    public TypedRawDataChunk(string xiName, int xiType, byte[] xiBody)
      :
        base(xiName, xiBody)
    {
      this.mType = xiType;
    }

    public int TypeId
    {
      get { return mType; }
      set { mType = value; }
    }

    private int mType;
  }
}
