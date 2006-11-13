using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using MMEd.Util;

namespace MMEd.Chunks
{
  public class CameraPosChunk : Chunk
  {
    int mIdx;

    [Description("Viewing direction: 0 is north, 1024 is east, etc.")]
    public short Direction;

    [Description("Camera distance from the car in a straight line along the xy plane")]
    public short Distance;

    [Description("Camera height, as a straight-line distance above the car")]
    public short Elevation;

    public CameraPosChunk() { }
    public CameraPosChunk(int idx, BinaryReader bin) 
    {
      mIdx = idx;
      Deserialise(bin);
    }

    public override void Deserialise(Stream inStr)
    {
      BinaryReader bin = new BinaryReader(inStr);
      Deserialise(bin);
    }

    public void Deserialise(BinaryReader bin)
    {
      Direction = bin.ReadInt16();
      Distance =  bin.ReadInt16();
      Elevation = bin.ReadInt16();
    }

    public override void Serialise(Stream outStr)
    {
      BinaryWriter bout = new BinaryWriter(outStr);
      Serialise(bout);
    }

    public void Serialise(BinaryWriter bout)
    {
      bout.Write(Direction);
      bout.Write(Distance);
      bout.Write(Elevation);
    }

    public override string Name
    {
      get
      {
        return string.Format("[{0}] CameraPos", mIdx);
      }
    }
  }
}