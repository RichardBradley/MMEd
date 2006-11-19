using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace MMEd.Ad3ds
{
  class ObjectChunk : Chunk
  {
    public override void Deserialise(Stream xiInStream, int xiLength)
    {
      return;
    }

    public override void AddChild(Chunk xiChild)
    {
      if (xiChild is MeshChunk)
      {
        if (mMesh != null)
        {
          throw new Exception("Unexpected second mesh chunk in object");
        }
        mMesh = xiChild as MeshChunk;
      }
      else
      {
        Trace.WriteLine("Unrecognised child chunk of object, child details: " + xiChild);
      }
    }

    public override void Serialise(Stream xiOutStream)
    {
      BinaryWriter lWriter = new BinaryWriter(xiOutStream);
      lWriter.Write(OBJ_START);
      lWriter.Write(Length);
      mMesh.Serialise(xiOutStream);
    }

    public override int Length
    {
      get { return mMesh.Length + 6; }
    }

    MeshChunk mMesh;
  }
}
