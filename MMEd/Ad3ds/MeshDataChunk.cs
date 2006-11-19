using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace MMEd.Ad3ds
{
  class MeshDataChunk : Chunk
  {
    public override void Deserialise(Stream xiInStream, int xiLength)
    {
      return;
    }

    public override void AddChild(Chunk xiChild)
    {
      if (xiChild is VertexListChunk)
      {
        if (mVertexList != null)
        {
          throw new Exception("Unexpected second vertex list chunk in mesh data");
        }
        mVertexList = xiChild as VertexListChunk;
      }
      else if (xiChild is FaceListChunk)
      {
        if (mFaceList != null)
        {
          throw new Exception("Unexpected second face list chunk in mesh data");
        }
        mFaceList = xiChild as FaceListChunk;
      }
      else
      {
        Trace.WriteLine("Unrecognised child chunk of mesh data child details: " + xiChild);
      }
    }

    public override int Length
    {
      get
      {
        return 6 + mVertexList.Length + mFaceList.Length;
      }
    }

    public override void Serialise(Stream xiOutStream)
    {
      BinaryWriter lWriter = new BinaryWriter(xiOutStream);
      lWriter.Write(MESH_DATA);
      lWriter.Write(Length);
      mVertexList.Serialise(xiOutStream);
      mFaceList.Serialise(xiOutStream);
    }

    VertexListChunk mVertexList;
    FaceListChunk mFaceList;
  }
}
