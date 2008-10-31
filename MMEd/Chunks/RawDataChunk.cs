using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using MMEd.Util;

// Just a wrapper to organise the tree slightly better

namespace MMEd.Chunks
{
  public class RawDataChunk : Chunk
  {
    //for serialisation
    public RawDataChunk() { }

    public RawDataChunk(string xiName, byte[] xiData)
    {
      mData = xiData;
      GivenName = xiName;
    }
    public string GivenName;
    public byte[] mData;

    public override void Deserialise(System.IO.Stream inStr)
    {
      throw new Exception("The method or operation is not implemented.");
    }

    public override void Serialise(System.IO.Stream outStr)
    {
      outStr.Write(mData, 0, mData.Length);
    }

    public override string Name
    {
      get
      {
        return GivenName;
      }
    }

    public override List<string> GetDifferences(Chunk xiChunk)
    {
      if (ByteArrayComparer.CompareStatic(mData, ((RawDataChunk)xiChunk).mData) != 0)
      {
        List<string> lRet = base.GetDifferences(xiChunk);
        lRet.Add("Changed data named " + GivenName);
        return lRet;
      }

      return base.GetDifferences(xiChunk);
    }

    public override void ReplaceChild(Chunk xiFrom, Chunk xiTo)
    {
      throw new Exception("The method or operation is not implemented.");
    }
  }
}
