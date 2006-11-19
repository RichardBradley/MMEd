using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace MMEd.Ad3ds
{
  class UnknownChunk : Chunk
  {
    public UnknownChunk(UInt16 xiType)
    {
      mType = xiType;
    }

    public override void Deserialise(Stream xiInStream, int xiLength)
    {
      xiInStream.Seek(xiLength, SeekOrigin.Current);
    }

    public override void Serialise(Stream xiOutStream)
    {
      throw new Exception("The method or operation is not implemented.");
    }

    public override void AddChild(Chunk xiChild)
    {
      throw new Exception("The method or operation is not implemented.");
    }

    public override int Length
    {
      get { throw new Exception("The method or operation is not implemented."); }
    }

    private UInt16 mType;
  }
}
