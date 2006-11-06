using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.ComponentModel;
using MMEd.Util;

// represents one of those images after the key waypoints, and before
// the camera position array
//
// I don't know what they represent. They're possibly something to do with
// reset positions, and possibly referenced by TexMetaData[7]
//
// TODO: rename this class once the meaning is clearer

namespace MMEd.Chunks
{
  public class OddImageChunk : Chunk
  {
    int mIdx;

    [Description("The data.")]
    public byte[] Data;

    public OddImageChunk() { }
    public OddImageChunk(int idx, Stream inStr)
    {
      mIdx = idx;
      Deserialise(inStr);
    }

    public override string Name
    {
      get
      {
        return string.Format("[{0}] OddImage", mIdx);
      }
    }

    public override void Deserialise(Stream inStr)
    {
      Data = new byte[64];
      StreamUtils.EnsureRead(inStr, Data);
    }

    public override void Serialise(Stream outStr)
    {
      outStr.Write(Data, 0, Data.Length);
    }

    public override void ReplaceChild(Chunk xiFrom, Chunk xiTo)
    {
      throw new Exception("The method or operation is not implemented.");
    }
  }
}
