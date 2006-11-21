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
    public GroupingChunk CameraPositions;

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
      CameraPosChunk[] cameraPositions = new CameraPosChunk[cameraPositionCount];
      for (int i = 0; i < cameraPositionCount; i++)
      {
        cameraPositions[i] = new CameraPosChunk(i, bin);
      }
      this.CameraPositions = new GroupingChunk("CameraPositions", cameraPositions);

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
      bout.Write((short)CameraPositions.mChildren.Length);
      CameraPositions.Serialise(outStr);

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
      acc.Add(CameraPositions);
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
        OddImages = (GroupingChunk)xiTo;
      }
      else if (xiFrom == CameraPositions)
      {
        CameraPositions = (GroupingChunk)xiTo;
      }
      else if (xiFrom == BumpImages)
      {
        BumpImages = (GroupingChunk)xiTo;
      }
      else
      {
        throw new ArgumentException("xifrom not found!");
      }
    }

    ///========================================================================
    ///  Method : AddFlat
    /// 
    /// <summary>
    /// 	Add a new Flat, returning the size of the added Flat in bytes
    /// </summary>
    /// <param name="xiFlat"></param>
    /// <returns></returns>
    ///========================================================================
    public int AddFlat(FlatChunk xiFlat)
    {
      FlatChunk[] lNewFlats = new FlatChunk[Flats.Length + 1];
      Array.Copy(Flats, lNewFlats, Flats.Length);
      lNewFlats[Flats.Length] = xiFlat;
      Flats = lNewFlats;

      return xiFlat.ByteSize;
    }

    public void AddOdd(OddImageChunk xiOdd)
    {
      Chunk[] lNewOddArray = new Chunk[OddImages.mChildren.Length + 1];
      Array.Copy(OddImages.mChildren, lNewOddArray, OddImages.mChildren.Length);
      lNewOddArray[OddImages.mChildren.Length] = xiOdd;
      OddImages.mChildren = lNewOddArray;
    }

    #region Odds manipulation

    public Hashtable UnusedOdds
    {
      get
      {
        if (mUnusedOdds == null)
        {
          mUnusedOdds = new Hashtable();
          FindUnusedOdds();
        }

        return mUnusedOdds;
      }
      set
      {
        mUnusedOdds = value;
      }
    }

    private void FindUnusedOdds()
    {
      //=======================================================================
      // Get a list of all odds to start with.
      //=======================================================================
      for (int i = 0; i < OddImages.mChildren.Length; i++)
      {
        if (!(OddImages.mChildren[i] is OddImageChunk))
        {
          continue;
        }

        OddImageChunk lOdd = (OddImageChunk)OddImages.mChildren[i];
        mUnusedOdds[i] = lOdd;
      }

      //=======================================================================
      // Now remove all used odds.
      //=======================================================================
      foreach (FlatChunk lFlat in Flats)
      {
        if (lFlat.TexMetaData == null)
        {
          continue;
        }

        for (int x = 0; x < lFlat.Width; x++)
        {
          for (int y = 0; y < lFlat.Height; y++)
          {
            int lOddId = lFlat.TexMetaData[x][y][(byte)eTexMetaDataEntries.Zero];

            if (mUnusedOdds.ContainsKey(lOddId))
            {
              mUnusedOdds.Remove(lOddId);
            }
          }
        }
      }
    }

    private Hashtable mUnusedOdds = null;

    #endregion
  }
}
