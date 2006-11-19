using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace MMEd.Ad3ds
{
  class FileChunk : Chunk
  {
    public override void Deserialise(Stream xiInStream, int xiLength)
    {
      return;
    }

    public override void Serialise(Stream xiOutStream)
    {
      BinaryWriter lWriter = new BinaryWriter(xiOutStream);
      lWriter.Write(MAGIC_NUMBER);
      lWriter.Write(Length);
      mVersion.Serialise(xiOutStream);
      
      foreach (ObjectChunk lObject in mObjects)
      {
        lObject.Serialise(xiOutStream);
      }
    }

    public override int Length
    {
      get 
      {
        int lLength = 6 + mVersion.Length;
        foreach (ObjectChunk lObject in mObjects)
        {
          lLength += lObject.Length;
        }

        return lLength;
      }
    }

    public override void AddChild(Chunk xiChild)
    {
      if (xiChild is ObjectChunk)
      {
        mObjects.Add(xiChild as ObjectChunk);
      }
      else if (xiChild is VersionChunk)
      {
        mVersion = xiChild as VersionChunk;
      }
      else
      {
        Trace.WriteLine("Unrecognised child chunk of file, child details: " + xiChild);
      }
    }

    VersionChunk mVersion;
    List<ObjectChunk> mObjects = new List<ObjectChunk>();
  }
}
