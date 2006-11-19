using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace MMEd.Ad3ds
{
  public abstract class Chunk
  {
    public const ushort MAGIC_NUMBER = 0x4D4D;
    public const ushort VERSION = 0x0002;
    public const ushort RGB1 = 0x0011;
    public const ushort QUANTITY = 0x0030;
    public const ushort OBJ_START = 0x3D3D;

    public const ushort MESH_START = 0x4000;
    public const ushort MESH_DATA = 0x4100;
    public const ushort VERTEX_LIST = 0x4110;
    public const ushort VERTEX_OPTIONS = 0x4111;
    public const ushort FACE_LIST = 0x4120;
    public const ushort MESH_MATERIAL = 0x4130;
    public const ushort MESH_TEX_COORDS = 0x4140;

    public const ushort MATERIAL_NAME = 0xA000;
    public const ushort MATERIAL_AMBIENT_COLOR = 0xA010;
    public const ushort MATERIAL_DIFFUSE_COLOR = 0xA020;
    public const ushort MATERIAL_SPECULAR_COLOR = 0xA030;
    public const ushort MATERIAL_TRANSPARENCY = 0xA050;
    public const ushort MATERIAL_TEXTURE_START = 0xA200;
    public const ushort MATERIAL_TEXTURE_NAME = 0xA300;
    public const ushort MATERIAL_TEXTURE_OPTIONS = 0xA351;
    public const ushort MATERIAL_TEXTURE_XSCALE = 0xA354;
    public const ushort MATERIAL_TEXTURE_YSCALE = 0xA356;
    public const ushort MATERIAL_TEXTURE_XOFFSET = 0xA35A;
    public const ushort MATERIAL_TEXTURE_YOFFSET = 0xA35C;
    public const ushort MATERIAL_START = 0xAFFF;

    public abstract void Deserialise(Stream xiInStream, int xiLength);
    public abstract void Serialise(Stream xiOutStream);
    public abstract int Length { get; }
    public abstract void AddChild(Chunk xiChild);

    internal static FileChunk ReadFile(Stream xiInStream)
    {
      return (FileChunk)ReadChunk(xiInStream);
    }

    protected static Chunk CreateChunk(ushort xiId)
    {
      switch (xiId)
      {
        case MAGIC_NUMBER:
          return new FileChunk();
        case VERSION:
          return new VersionChunk();
        case OBJ_START:
          return new ObjectChunk();
        case MESH_START:
          return new MeshChunk();
        case MESH_DATA:
          return new MeshDataChunk();
        case VERTEX_LIST:
          return new VertexListChunk();
        case FACE_LIST:
          return new FaceListChunk();
        default:
          return new UnknownChunk(xiId);
      }
    }

    protected static Chunk ReadChunk(Stream xiInStream)
    {
      while (xiInStream.Position < xiInStream.Length)
      {
        BinaryReader lReader = new BinaryReader(xiInStream);
        
        long lStartPosition = xiInStream.Position;

        ushort lId = lReader.ReadUInt16();
        int lLength = lReader.ReadInt32();
        long lEndPosition = lStartPosition + lLength;

        Chunk lChunk = Chunk.CreateChunk(lId);
        lChunk.Deserialise(xiInStream, lLength - 6);

        Trace.WriteLine(string.Format("Read Chunk; type: 0x{0} length: {1} data: {2}", lId.ToString("X4"), lLength, lChunk));

        while (xiInStream.Position < lLength + lStartPosition)
        {
          lChunk.AddChild(ReadChunk(xiInStream));
        }

        if (xiInStream.Position != lEndPosition)
        {
          throw new Exception("Stream position error expected: " + lEndPosition + " actual: " + xiInStream.Position);
        }
      }
      return null;
    }
  }
}
