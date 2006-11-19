using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace MMEd.Ad3ds
{
  class VertexListChunk : Chunk
  {
    public override void Deserialise(Stream xiInStream, int xiLength)
    {
      BinaryReader lReader = new BinaryReader(xiInStream);
      int lCount = (int)lReader.ReadUInt16();
      for (int ii = 0; ii < lCount; ++ii)
      {
        Vertex lVertex;
        lVertex.x = lReader.ReadSingle();
        lVertex.y = lReader.ReadSingle();
        lVertex.z = lReader.ReadSingle();
        mVertices.Add(lVertex);
      }
    }

    public override void AddChild(Chunk xiChild)
    {
      throw new Exception("The method or operation is not implemented.");
    }

    public override string ToString()
    {
      return base.ToString() + " count: " + mVertices.Count;
    }

    public override int Length
    {
      get
      {
        return 6 + 2 + (mVertices.Count * 12);
      }
    }

    public override void Serialise(Stream xiOutStream)
    {
      BinaryWriter lWriter = new BinaryWriter(xiOutStream);
      lWriter.Write(VERTEX_LIST);
      lWriter.Write(Length);
      lWriter.Write((ushort)mVertices.Count);
      foreach (Vertex lVertex in mVertices)
      {
        lWriter.Write(lVertex.x);
        lWriter.Write(lVertex.y);
        lWriter.Write(lVertex.z);
      }
    }

    public void AddVertex(Vertex xiVertex)
    {
      mVertices.Add(xiVertex);
    }

    List<Vertex> mVertices = new List<Vertex>();
  }
}
