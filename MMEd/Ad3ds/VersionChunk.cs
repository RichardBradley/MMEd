using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace MMEd.Ad3ds
{
  class VersionChunk : Chunk
  {
    public override void Deserialise(Stream xiInStream, int xiLength)
    {
      BinaryReader lReader = new BinaryReader(xiInStream);
      mVersion = lReader.ReadInt32();
    }

    public override void AddChild(Chunk xiChild)
    {
      throw new Exception("The method or operation is not implemented.");
    }

    public override string ToString()
    {
      return base.ToString() + " version: " + mVersion;
    }

    public override int Length
    {
      get { return 10; }
    }

    public override void Serialise(Stream xiOutStream)
    {
      BinaryWriter lWriter = new BinaryWriter(xiOutStream);
      lWriter.Write(VERSION);
      lWriter.Write(Length);
      lWriter.Write((int)3);
    }

    int mVersion;
  }
}
