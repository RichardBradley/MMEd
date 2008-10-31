using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Collections;
using System.ComponentModel;
using MMEd.Util;

// The key waypoints get their own chunk, even though
// it's not a very complicated data structure

namespace MMEd.Chunks
{
  public class KeyWaypointsChunk : Chunk
  {
    [Description(@"These sections may not be cut. From and To are inclusive, and index 
        the waypoint elements in the TexMetaData array of the Flats")]
    public KeySection[] KeySections;

    public class KeySection
    {
      public byte From;
      public byte To;
      public KeySection(byte xiFrom, byte xiTo) { From = xiFrom; To = xiTo; }
      public KeySection() { }

      public override bool Equals(object obj)
      {
        return obj != null && 
          obj is KeySection &&
          (From == ((KeySection)obj).From) && 
          (To == ((KeySection)obj).To);
      }

      public override int GetHashCode()
      {
        return From + To * 256;
      }
    }

    public KeyWaypointsChunk() { }
    public KeyWaypointsChunk(BinaryReader bin) { Deserialise(bin); }

    public override void Deserialise(Stream inStr)
    {
      Deserialise((new BinaryReader(inStr)));
    }

    public void Deserialise(BinaryReader bin)
    {
      short keyCount = bin.ReadInt16();
      if (keyCount > 256)
        throw new DeserialisationException(string.Format("Bad keypoint count: {0}", keyCount), bin.BaseStream.Position);
      KeySections = new KeySection[keyCount];
      for (int i = 0; i < keyCount; i++)
      {
        KeySections[i] = new KeySection(bin.ReadByte(), bin.ReadByte());
      }
    }

    public override void Serialise(Stream outStr)
    {
      BinaryWriter bout = new BinaryWriter(outStr);
      bout.Write((short)(KeySections.Length));
      for (int i = 0; i < KeySections.Length; i++)
      {
        bout.Write(KeySections[i].From);
        bout.Write(KeySections[i].To);
      }
    }

    public override string Name
    {
      get
      {
        return "Key Waypoints";
      }
    }

    public override List<string> GetDifferences(Chunk xiChunk)
    {
      KeyWaypointsChunk lOther = xiChunk as KeyWaypointsChunk;

      if (!Utils.ArrayCompare(KeySections, lOther.KeySections))
      {
        List<string> lRet = base.GetDifferences(xiChunk);
        lRet.Add("Changed key waypoints");
        return lRet;
      }

      return base.GetDifferences(xiChunk);
    }

    public override void ReplaceChild(Chunk xiFrom, Chunk xiTo)
    {
      throw new Exception("The method or operation is not implemented.");
    }
  }
}
