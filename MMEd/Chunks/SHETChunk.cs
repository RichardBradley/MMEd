using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.ComponentModel;
using System.Collections;
using MMEd.Util;

namespace MMEd.Chunks
{
  public class SHETChunk : Chunk
  {
    [Description("Found in the header. Some kind of filename.")]
    public string HeaderString1;

    [Description("Found in the header. Some kind of filename.")]
    public string HeaderString2;

    [Description("Found in the header. Unknown.")]
    public short HeaderShort1;

    [Description("Found in the header. Unknown.")]
    public short HeaderShort2;

    [Description("Found in the header. Unknown.")]
    public byte HeaderByte;

    [Description("The flat surfaces in the level, including all drivable surfaces")]
    public FlatChunk[] Flats;

    [Description("These waypoints cannot be skipped")]
    public KeyWaypointsChunk KeyWaypoints;

    public GroupingChunk OddImages;

    [Description(@"Camera position arrays. I forget which way round these go. 
Each entry has three components, which are height, pitch and yaw, in some order")]
    public short[][] CameraPositions;

    public GroupingChunk BumpImages;

    public int TrailingZeroByteCount;

    public SHETChunk() { }
    public SHETChunk(Stream inStr, BinaryReader bin) { Deserialise(inStr, bin); }

    public override void Deserialise(Stream inStr)
    {
      BinaryReader bin = new BinaryReader(inStr);
      Deserialise(inStr, bin);
    }

    public void Deserialise(Stream inStr, BinaryReader bin)
    {
      //header
      HeaderString1 = StreamUtils.ReadASCIINullTermString(inStr);
      HeaderString2 = StreamUtils.ReadASCIINullTermString(inStr);
      HeaderShort1 = bin.ReadInt16();
      HeaderShort2 = bin.ReadInt16();
      HeaderByte = bin.ReadByte();
      int lSheetCount = bin.ReadInt16();

      Flats = new FlatChunk[lSheetCount];
      for (int i = 0; i < lSheetCount; i++)
      {
        Flats[i] = new FlatChunk(bin);
      }

      //key waypoints
      KeyWaypoints = new KeyWaypointsChunk(bin);

      //now the first set of images. I don't really know
      //what these do. They might be referred to by TexMetaData[7]
      short oddImgCount = bin.ReadInt16();
      OddImageChunk[] oddImages = new OddImageChunk[oddImgCount];
      for (int i = 0; i < oddImgCount; i++)
      {
        oddImages[i] = new OddImageChunk(i, inStr);
      }
      this.OddImages = new GroupingChunk("OddImages", oddImages);

      //now some camera positions:
      short cameraPositionCount = bin.ReadInt16();
      CameraPositions = new short[cameraPositionCount][];
      for (int i = 0; i < cameraPositionCount; i++)
      {
        CameraPositions[i] = new short[3];
        for (int j = 0; j < 3; j++)
        {
          CameraPositions[i][j] = bin.ReadInt16();
        }
      }

      //now the bump map prototypes. These are referred to by TexMetaData[6]
      short bumpImgCount = bin.ReadInt16();
      BumpImageChunk[] bumpImages = new BumpImageChunk[bumpImgCount];
      for (int i = 0; i < bumpImgCount; i++)
      {
        bumpImages[i] = new BumpImageChunk(i, inStr);
      }
      this.BumpImages = new GroupingChunk("BumpImages", bumpImages);

      //now trailing zeroes:
      int lNextByte;
      while (-1 != (lNextByte = inStr.ReadByte()))
      {
        if (lNextByte != 0) throw new DeserialisationException(string.Format("Expecting SHET to be followed by a block of zeroes. Found {0}", lNextByte), inStr.Position);
        TrailingZeroByteCount++;
      }
    }

    public override void Serialise(Stream outStr)
    {
      BinaryWriter bout = new BinaryWriter(outStr);

      //header
      StreamUtils.WriteASCIINullTermString(outStr, HeaderString1);
      StreamUtils.WriteASCIINullTermString(outStr, HeaderString2);
      bout.Write(HeaderShort1);
      bout.Write(HeaderShort2);
      bout.Write(HeaderByte);

      //flats
      bout.Write((short)Flats.Length);
      foreach (FlatChunk flat in Flats)
        flat.Serialise(outStr);

      //key waypoints 
      KeyWaypoints.Serialise(outStr);

      //now the oddImages 
      //TODO: could add a method to GroupingChunk to do this pattern
      bout.Write((short)OddImages.mChildren.Length);
      OddImages.Serialise(outStr);

      //now some camera positions:
      bout.Write((short)CameraPositions.Length);
      for (int i = 0; i < CameraPositions.Length; i++)
      {
        for (int j = 0; j < 3; j++)
        {
          bout.Write(CameraPositions[i][j]);
        }
      }

      //now the bumpImages 
      //TODO: could add a method to GroupingChunk to do this pattern
      bout.Write((short)BumpImages.mChildren.Length);
      BumpImages.Serialise(outStr);

      //now trailing zeroes:
      StreamUtils.WriteZeros(outStr, TrailingZeroByteCount);
    }

    public override string Name
    {
      get
      {
        return "SHET";
      }
    }

    public override Chunk[] GetChildren()
    {
      ArrayList acc = new ArrayList();
      acc.AddRange(Flats);
      acc.Add(KeyWaypoints);
      acc.Add(OddImages);
      acc.Add(BumpImages);
      return (Chunk[])acc.ToArray(typeof(Chunk));
    }

    public override void ReplaceChild(Chunk xiFrom, Chunk xiTo)
    {
      if (xiFrom is FlatChunk)
      {
        for (int i = 0; i < Flats.Length; i++)
          if (xiFrom == Flats[i])
          {
            Flats[i] = (FlatChunk)xiTo;
            return;
          }
        throw new ArgumentException("xifrom not found!");
      }
      else if (xiFrom == KeyWaypoints)
      {
        KeyWaypoints = (KeyWaypointsChunk)xiTo;
      }
      else if (xiFrom == OddImages)
      {
        OddImages = (GroupingChunk)xiFrom;
      }
      else if (xiFrom == BumpImages)
      {
        BumpImages = (GroupingChunk)xiFrom;
      }
      else
      {
        throw new ArgumentException("xifrom not found!");
      }
    }
  }
}
