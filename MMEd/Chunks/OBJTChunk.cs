using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.ComponentModel;
using System.Collections;
using MMEd.Util;

namespace MMEd.Chunks
{
  public class OBJTChunk : Chunk
  {

    public Chunk[] mChildren;

    public OBJTChunk() { }
    public OBJTChunk(Stream inStr, BinaryReader bin) { Deserialise(inStr, bin); }

    public override void Deserialise(Stream inStr)
    {
      BinaryReader bin = new BinaryReader(inStr);
      Deserialise(inStr, bin);
    }

    public void Deserialise(Stream inStr, BinaryReader bin)
    {
      List<Chunk> lChildren = new List<Chunk>();
      int lObjtIdx = 0;
      while (true)
      {
        int lLen = bin.ReadInt32();
        int lType = bin.ReadInt32();
        if (lType == 0)
        {
          lChildren.Add(new TMDChunk(lObjtIdx++, inStr));
        }
        else
        {
          lChildren.Add(new TypedRawDataChunk(string.Format("raw, type {0}, len {1}", lType, lLen), lType, bin.ReadBytes(lLen)));

          //raw data chunks of type -1 count in the objt indexing scheme:
          if (lType == -1) lObjtIdx++;

          if (lLen == 0)
            break;
        }
      }
      mChildren = lChildren.ToArray();
    }


    public override void Serialise(Stream outStr)
    {
      BinaryWriter bout = new BinaryWriter(outStr);
      foreach (Chunk c in mChildren)
      {
        if (c is TMDChunk)
        {
          bout.Write((int)((TMDChunk)c).DataLength);
          bout.Write((int)0); //type
          ((TMDChunk)c).Serialise(outStr, bout);
        }
        else if (c is TypedRawDataChunk)
        {
          bout.Write((int)((TypedRawDataChunk)c).mData.Length);
          bout.Write(((TypedRawDataChunk)c).TypeId);
          ((TypedRawDataChunk)c).Serialise(outStr);
        }
        else
        {
          throw new Exception("Unexpected type in mChildren");
        }
      }
    }

    public override string Name
    {
      get
      {
        return "OBJT";
      }
    }

    public override Chunk[] GetChildren()
    {
      return mChildren;
    }

    public override void ReplaceChild(Chunk xiFrom, Chunk xiTo)
    {

    }
  }
}
