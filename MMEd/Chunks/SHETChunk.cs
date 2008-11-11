using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.ComponentModel;
using System.Collections;
using System.Xml.Serialization;
using MMEd.Util;
using System.Windows.Forms;
using MMEd.Viewers.ThreeDee;
using GLTK;

namespace MMEd.Chunks
{
  public class SHETChunk : Chunk, IEntityProvider
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
    /// Method : AddFlat
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

    ///========================================================================
    /// Method : DeleteFlat
    /// 
    /// <summary>
    /// 	Delete a Flat, returning the size of the deleted Flat in bytes
    /// </summary>
    /// <param name="xiFlat"></param>
    /// <returns></returns>
    ///========================================================================
    public int DeleteFlat(FlatChunk xiFlat)
    {
      FlatChunk[] lNewFlats = new FlatChunk[Flats.Length - 1];
      int lIndex = 0;

      foreach (FlatChunk lFlat in Flats)
      {
        if (lFlat != xiFlat)
        {
          lNewFlats[lIndex++] = lFlat;
        }
      }

      Flats = lNewFlats;
      return xiFlat.ByteSize;
    }

    ///========================================================================
    /// Method : AddOdd
    /// 
    /// <summary>
    /// 	Add a new Odd, returning the size of the added Odd in bytes
    /// </summary>
    /// <param name="xiOdd"></param>
    /// <returns></returns>
    ///========================================================================
    public int AddOdd(OddImageChunk xiOdd)
    {
      Chunk[] lNewOddArray = new Chunk[OddImages.mChildren.Length + 1];
      Array.Copy(OddImages.mChildren, lNewOddArray, OddImages.mChildren.Length);
      lNewOddArray[OddImages.mChildren.Length] = xiOdd;
      OddImages.mChildren = lNewOddArray;

      return xiOdd.Data.Length;
    }

    ///========================================================================
    /// Method : AddBump
    /// 
    /// <summary>
    /// 	Add a new Bump, returning the size of the added Bump in bytes
    /// </summary>
    /// <param name="xiOdd"></param>
    /// <returns></returns>
    ///========================================================================
    public int AddBump(BumpImageChunk xiBump)
    {
      Chunk[] lNewBumpArray = new Chunk[BumpImages.mChildren.Length + 1];
      Array.Copy(BumpImages.mChildren, lNewBumpArray, BumpImages.mChildren.Length);
      lNewBumpArray[BumpImages.mChildren.Length] = xiBump;
      BumpImages.mChildren = lNewBumpArray;

      return xiBump.Data.Length;
    }


    #region Bump manipulation

    [XmlIgnore]
    public Hashtable UnusedBumps
    {
      get
      {
        if (mUnusedBumps == null)
        {
          mUnusedBumps = new Hashtable();
          FindUnusedBumps();
        }

        return mUnusedBumps;
      }
      set
      {
        mUnusedBumps = value;
      }
    }

    private void FindUnusedBumps()
    {
      //=======================================================================
      // Get a list of all bumps to start with.
      //=======================================================================
      for (int i = 0; i < BumpImages.mChildren.Length; i++)
      {
        if (!(BumpImages.mChildren[i] is BumpImageChunk))
        {
          continue;
        }

        BumpImageChunk lBump = (BumpImageChunk)BumpImages.mChildren[i];
        mUnusedBumps[i] = lBump;
      }

      //=======================================================================
      // Now remove all used bumps.
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
            int lBumpId = lFlat.TexMetaData[x][y][(byte)eTexMetaDataEntries.Bumpmap];

            if (mUnusedBumps.ContainsKey(lBumpId))
            {
              mUnusedBumps.Remove(lBumpId);
            }
          }
        }
      }
    }

    private Hashtable mUnusedBumps = null;

    #endregion

    #region Odds manipulation

    [XmlIgnore]
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
            int lOddId = lFlat.TexMetaData[x][y][(byte)eTexMetaDataEntries.Odds];

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

    #region Key waypoints

    public KeyWaypointsChunk.KeySection GetKeySectionByWaypoint(byte xiWaypoint)
    {
      return (KeyWaypointsChunk.KeySection)KeySectionsByWaypoint[xiWaypoint];
    }

    public void AdjustKeySections(int xiFrom, int xiChangeBy, int xiWrap)
    {
      foreach (KeyWaypointsChunk.KeySection lKeySection in KeyWaypoints.KeySections)
      {
        if (lKeySection.From >= xiFrom)
        {
          int lNewValue = lKeySection.From + xiChangeBy;
          if (lNewValue <= 0)
          {
            if (xiWrap > 0)
            {
              lNewValue += xiWrap;
            }
            else
            {
              lNewValue = 0;
            }
          }
          else if (lNewValue > xiWrap)
          {
            lNewValue -= xiWrap;
          }
          lKeySection.From = (byte)lNewValue;
        }

        if (lKeySection.To >= xiFrom)
        {
          int lNewValue = lKeySection.To + xiChangeBy;
          if (lNewValue <= 0)
          {
            if (xiWrap > 0)
            {
              lNewValue += xiWrap;
            }
            else
            {
              lNewValue = 0;
            }
          }
          else if (lNewValue > xiWrap)
          {
            lNewValue -= xiWrap;
          }
          lKeySection.To = (byte)lNewValue;
        }
      }

      mKeySectionsByWaypoint = null;
    }

    public void AddKeySection(byte xiWaypoint)
    {
      if (GetKeySectionByWaypoint(xiWaypoint) != null || 
        xiWaypoint == 0)
      {
        return;
      }

      KeyWaypointsChunk.KeySection lPreviousSection = 
        GetKeySectionByWaypoint((byte)(xiWaypoint - 1));
      KeyWaypointsChunk.KeySection lFollowingSection = 
        GetKeySectionByWaypoint((byte)(xiWaypoint + 1));//qqTLP What if it hits the limit (not here but when inserting)?

      int lPreviousByteSize = KeyWaypoints.ByteSize;

      if (lPreviousSection == null && lFollowingSection == null)
      {
        KeyWaypointsChunk.KeySection lNewSection = 
          new KeyWaypointsChunk.KeySection(xiWaypoint, xiWaypoint);
        KeyWaypointsChunk.KeySection[] lNewSectionArray = 
          new KeyWaypointsChunk.KeySection[KeyWaypoints.KeySections.Length + 1];
        Array.Copy(
          KeyWaypoints.KeySections, 
          lNewSectionArray, 
          KeyWaypoints.KeySections.Length);
        lNewSectionArray[KeyWaypoints.KeySections.Length] = lNewSection;
        KeyWaypoints.KeySections = lNewSectionArray;
      }
      else if (lPreviousSection == null)
      {
        lFollowingSection.From = xiWaypoint;
      }
      else if (lFollowingSection == null)
      {
        lPreviousSection.To = xiWaypoint;
      }
      else
      {
        lPreviousSection.To = lFollowingSection.To;
        ArrayList lSections = new ArrayList(KeyWaypoints.KeySections);
        lSections.Remove(lFollowingSection);
        KeyWaypoints.KeySections = (KeyWaypointsChunk.KeySection[])
          lSections.ToArray(typeof(KeyWaypointsChunk.KeySection));
      }

      TrailingZeroByteCount += lPreviousByteSize - KeyWaypoints.ByteSize;

      if (TrailingZeroByteCount < 0)
      {
        MessageBox.Show("You have run out of space in your level file! Please reduce the size of the file before saving.", "MMEd", MessageBoxButtons.OK, MessageBoxIcon.Stop);
      }

      mKeySectionsByWaypoint = null;
    }

    public void RemoveKeySection(byte xiWaypoint)
    {
      KeyWaypointsChunk.KeySection lSection =
        GetKeySectionByWaypoint(xiWaypoint);

      if (lSection == null || xiWaypoint == 0)
      {
        return;
      }

      KeyWaypointsChunk.KeySection lPreviousSection =
        GetKeySectionByWaypoint((byte)(xiWaypoint - 1));
      KeyWaypointsChunk.KeySection lFollowingSection =
        GetKeySectionByWaypoint((byte)(xiWaypoint + 1));

      int lPreviousByteSize = KeyWaypoints.ByteSize;

      if (lPreviousSection == null && lFollowingSection == null)
      {
        ArrayList lSections = new ArrayList(KeyWaypoints.KeySections);
        lSections.Remove(lSection);
        KeyWaypoints.KeySections = (KeyWaypointsChunk.KeySection[])
          lSections.ToArray(typeof(KeyWaypointsChunk.KeySection));
      }
      else if (lPreviousSection == null)
      {
        lFollowingSection.From = (byte)(xiWaypoint + 1);
      }
      else if (lFollowingSection == null)
      {
        lPreviousSection.To = (byte)(xiWaypoint - 1);
      }
      else
      {
        KeyWaypointsChunk.KeySection lNewSection =
        new KeyWaypointsChunk.KeySection((byte)(xiWaypoint + 1), lFollowingSection.To);
        KeyWaypointsChunk.KeySection[] lNewSectionArray =
          new KeyWaypointsChunk.KeySection[KeyWaypoints.KeySections.Length + 1];
        Array.Copy(
          KeyWaypoints.KeySections,
          lNewSectionArray,
          KeyWaypoints.KeySections.Length);
        lNewSectionArray[KeyWaypoints.KeySections.Length] = lNewSection;
        KeyWaypoints.KeySections = lNewSectionArray;
        lPreviousSection.To = (byte)(xiWaypoint - 1);
      }

      TrailingZeroByteCount += lPreviousByteSize - KeyWaypoints.ByteSize;

      if (TrailingZeroByteCount < 0)
      {
        MessageBox.Show("You have run out of space in your level file! Please reduce the size of the file before saving.", "MMEd", MessageBoxButtons.OK, MessageBoxIcon.Stop);
      }

      mKeySectionsByWaypoint = null;
    }

    private Hashtable KeySectionsByWaypoint
    {
      get
      {
        if (mKeySectionsByWaypoint == null)
        {
          FindKeySections();
        }

        return mKeySectionsByWaypoint;
      }
    }

    private void FindKeySections()
    {
      mKeySectionsByWaypoint = new Hashtable();

      if (KeyWaypoints == null || KeyWaypoints.KeySections == null)
      {
        return;
      }

      foreach (KeyWaypointsChunk.KeySection lKeySection in KeyWaypoints.KeySections)
      {
        for (int i = lKeySection.From; i <= lKeySection.To; i++)
        {
          mKeySectionsByWaypoint[(byte)i] = lKeySection;
        }
      }
    }

    private Hashtable mKeySectionsByWaypoint = null;

    #endregion


    #region IEntityProvider Members

    public IEnumerable<GLTK.Entity> GetEntities(Chunk xiRootChunk, eTextureMode xiTextureMode, eTexMetaDataEntries xiSelectedMetadata)
    {
      if (!(xiRootChunk is Level))
      {
        throw new Exception("xiRootChunk must be Level for SHETChunk.GetEntities");
      }

      List<Entity> lAcc = new List<Entity>();

      foreach (FlatChunk fl in Flats)
      {
        lAcc.AddRange(fl.GetEntities((Level)xiRootChunk, xiTextureMode, xiSelectedMetadata));
      }

      return lAcc;
    }

    #endregion
  }
}
