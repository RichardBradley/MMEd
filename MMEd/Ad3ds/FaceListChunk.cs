using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace MMEd.Ad3ds
{
  class FaceListChunk : Chunk
  {
    public override void Deserialise(Stream xiInStream, int xiLength)
    {
      BinaryReader lReader = new BinaryReader(xiInStream);
      int lCount = (int)lReader.ReadUInt16();
      for (int ii = 0; ii < lCount; ++ii)
      {
        Face lFace;
        lFace.Vertex1 = lReader.ReadUInt16();
        lFace.Vertex2 = lReader.ReadUInt16();
        lFace.Vertex3 = lReader.ReadUInt16();

        lReader.ReadUInt16(); // these are flags, but we ignore them
        mFaces.Add(lFace);
      }
    }

    public override void AddChild(Chunk xiChild)
    {
      throw new Exception("The method or operation is not implemented.");
    }

    public override string ToString()
    {
      return base.ToString() + " count: " + mFaces.Count;
    }

    public override int Length
    {
      get
      {
        return 6 + 2 + (mFaces.Count * 8);
      }
    }

    public override void Serialise(Stream xiOutStream)
    {
      BinaryWriter lWriter = new BinaryWriter(xiOutStream);
      lWriter.Write(FACE_LIST);
      lWriter.Write(Length);
      lWriter.Write((ushort)mFaces.Count);
      foreach (Face lFace in mFaces)
      {
        lWriter.Write(lFace.Vertex1);
        lWriter.Write(lFace.Vertex2);
        lWriter.Write(lFace.Vertex3);
        lWriter.Write((ushort)0x002); // this seems to work
      }
    }

    public void AddFace(Face xiFace)
    {
      mFaces.Add(xiFace);
    }

    List<Face> mFaces = new List<Face>();
  }
}
