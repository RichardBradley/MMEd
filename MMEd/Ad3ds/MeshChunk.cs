using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace MMEd.Ad3ds
{
  class MeshChunk : Chunk
  {
    public MeshChunk()
    {
    }

    public MeshChunk(string xiName)
    {
      mName = xiName;
    }

    public override void Deserialise(Stream xiInStream, int xiLength)
    {
      BinaryReader lReader = new BinaryReader(xiInStream);
      string lName = "";
      char lChar = lReader.ReadChar();
      while (lChar != 0)
      {
        lName += lChar;
        lChar = lReader.ReadChar();
      }

      mName = lName;
      return;
    }

    public override void AddChild(Chunk xiChild)
    {
      if (xiChild is MeshDataChunk)
      {
        if (mMeshData != null)
        {
          throw new Exception("Unexpected second mesh data chunk in mesh '" + mName + "'");
        }
        mMeshData = xiChild as MeshDataChunk;
      }
      else
      {
        Trace.WriteLine("Unrecognised child chunk of mesh '" + mName + "' child details: " + xiChild);
      }
    }

    public override int Length
    {
      get 
      {
        return 6 + mName.Length + 1 + mMeshData.Length;
      }
    }

    public override void Serialise(Stream xiOutStream)
    {
      BinaryWriter lWriter = new BinaryWriter(xiOutStream);
      lWriter.Write(MESH_START);
      lWriter.Write(Length);
      lWriter.Write(mName.ToCharArray());
      lWriter.Write((byte)0);
      mMeshData.Serialise(xiOutStream);
    }

    public override string ToString()
    {
      return base.ToString() + " name: " + mName;
    }

    string mName;
    MeshDataChunk mMeshData;
  }
}
