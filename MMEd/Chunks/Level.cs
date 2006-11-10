using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Collections;
using System.ComponentModel;
using MMEd.Util;
using MMEd.Viewers;
using MMEd.Viewers.ThreeDee;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using GLTK;
using Bitmap = System.Drawing.Bitmap;

// Represents an entire MMv3 level (PS version)

namespace MMEd.Chunks
{
  //qq are these really necessary? some unit tests fail without them, but they're
  //   a bit strange
  [XmlInclude(typeof(NamedImageGroup)), XmlInclude(typeof(TIMChunk)),
  XmlInclude(typeof(OddImageChunk)), XmlInclude(typeof(BumpImageChunk)),
  XmlInclude(typeof(TMDChunk)), XmlInclude(typeof(TypedRawDataChunk))]
  public class Level : Chunk, Viewers.ThreeDee.IEntityProvider
  {
    [Description("Four int32s at the start of the file, meaning unknown")]
    public int[] Header;

    [Description("After the header, there's a big block of zero bytes")]
    public int ZeroByteCount;

    [Description("The textures and sprites")]
    public GroupingChunk NamedImageGroups;

    [Description("This is odd. The OBJT starts here, then stops and restarts")]
    public RawDataChunk OBJTFalseStart;

    [Description("The objects, and some other stuff")]
    public OBJTChunk OBJT;

    [Description("Dunno yet")]
    public RawDataChunk Dunno;

    [Description("most of the level data is in here")]
    public SHETChunk SHET;

    public TIMChunk GetTileById(int xiId)
    {
      foreach (NamedImageGroup nig in NamedImageGroups.mChildren)
      {
        if (xiId < nig.mChildren.Length)
        {
          return nig.mChildren[xiId] as TIMChunk;
        }
        else
        {
          xiId -= nig.mChildren.Length;
        }
      }
      return null;
    }

    public BumpImageChunk GetBumpById(int xiId)
    {
      if (xiId < SHET.BumpImages.mChildren.Length)
      {
        return (BumpImageChunk)SHET.BumpImages.mChildren[xiId];
      }
      else
      {
        return null;
      }
    }

    public OddImageChunk GetOddById(int xiId)
    {
      if (xiId < SHET.OddImages.mChildren.Length)
      {
        return (OddImageChunk)SHET.OddImages.mChildren[xiId];
      }
      else
      {
        return null;
      }
    }

    public TMDChunk GetObjtById(int xiId)
    {
      if (OBJT == null) return null;
      // the index includes any chunks of type -1, but not
      // random ones with typeId != -1
      foreach (Chunk c in OBJT.GetChildren())
      {
        if (c is TMDChunk || (c is TypedRawDataChunk && ((TypedRawDataChunk)c).TypeId == -1))
        {
          if (xiId == 0) return c as TMDChunk;
          else xiId--;
        }
      }
      return null;
    }

    public Bitmap GetTexturePageById(int xiId)
    {
      return VRAMViewer.GetInstance().GetTexturePage(this, xiId);
    }

    public bool WaypointIsKeyWaypoint(byte xiWaypoint)
    {
      foreach (KeyWaypointsChunk.KeySection ks in SHET.KeyWaypoints.KeySections)
      {
        if (ks.From <= xiWaypoint && ks.To >= xiWaypoint)
          return true;
      }
      return false;
    }

    public override void Deserialise(Stream inStr)
    {
      if (inStr is FileStream)
      {
        mName = ((FileStream)inStr).Name;
      }

      //BinaryReader is little-endian
      BinaryReader bin = new BinaryReader(inStr);

      //first, a header of 4 int32s:
      Header = new int[4];
      for (int i = 0; i < Header.Length; i++)
        Header[i] = bin.ReadInt32();

      //then, a block of zero bytes (int32 aligned)
      int nextInt = bin.ReadInt32();
      ZeroByteCount = 0;
      while (nextInt == 0)
      {
        ZeroByteCount += 4;
        nextInt = bin.ReadInt32();
      }

      ArrayList imageGroups = new ArrayList();

      //now, an array of image or comment structs
      while (nextInt != 0)
      {
        bin.BaseStream.Seek(-4, SeekOrigin.Current);
        NamedImageGroup nim = new NamedImageGroup();
        nim.Deserialise(bin.BaseStream);
        imageGroups.Add(nim);
        nextInt = bin.ReadInt32();
      }

      //nextInt is len == 0. 
      //There will be one more zero int32, then OBJTs
      nextInt = bin.ReadInt32();
      if (nextInt != 0)
        throw new DeserialisationException(string.Format("Expecting int32 0, found {0} at offset {1}", nextInt, inStr.Position - 4));

      //make a child to hold the EOF marker
      imageGroups.Add(new RawDataChunk("EOF marker", new byte[8]));

      //put the children so far in a grouping chunk
      NamedImageGroups = new GroupingChunk("textures and sprites", (Chunk[])imageGroups.ToArray(typeof(Chunk)));

      //Now the objects:
      //because of a bizzarre quirk here, the object chunk starts, then stops, then starts again
      long lObjtFalseStartPos = inStr.Position;

      try
      {
        OBJTChunk lFalseObjt = new OBJTChunk();
        lFalseObjt.Deserialise(inStr);
        OBJT = lFalseObjt;
      }
      catch (DeserialisationException e)
      {
        Console.Error.WriteLine("OBJT false start found, searching for restart...");
        //look for X,0,65,0 in first bit:
        inStr.Seek(lObjtFalseStartPos, SeekOrigin.Begin);
        int lX = bin.ReadInt32();
        if (bin.ReadInt32() != 0) throw new DeserialisationException("Expecting 0", inStr.Position);
        if (bin.ReadInt32() != 65) throw new DeserialisationException("Expecting 65", inStr.Position);
        if (bin.ReadInt32() != 0) throw new DeserialisationException("Expecting 0", inStr.Position);

        if (lX == 65) throw new Exception("I haven't planned for this case!");

        int[] lMatcher = new int[] { 0, 65, 0 };

        //longest observed false starts are Pool3 (1052 bytes) and Rest4 (2032 bytes)
        while (inStr.Position - lObjtFalseStartPos < 2100)
        {
          int lNext = bin.ReadInt32();
          if (lNext == lX)
          {
            if (Utils.ArrayCompare(lMatcher, new int[] { bin.ReadInt32(), bin.ReadInt32(), bin.ReadInt32() }))
            {
              int lFalseStartLen = (int)(inStr.Position - 4 * 4 - lObjtFalseStartPos);
              inStr.Seek(lObjtFalseStartPos, SeekOrigin.Begin);
              OBJTFalseStart = new RawDataChunk("OBJT False Start", bin.ReadBytes(lFalseStartLen));
              OBJT = new OBJTChunk();
              OBJT.Deserialise(inStr);
              break;
            }
          }
        }
        if (OBJT == null)
        {
          string lFileName = inStr is FileStream ? ((FileStream)inStr).Name : "unknown";
          System.Windows.Forms.MessageBox.Show(string.Format("Warning: OBJTs not loaded on level {0}: unable to find false start resumption after: {1}", lFileName, e));
          inStr.Seek(lObjtFalseStartPos, SeekOrigin.Begin);
        }
      }

      //read the rest of the file into a "remainder"
      //byte array so we can search for the SHET
      long lStartOfObjts = inStr.Position;
      byte[] lRest = new byte[inStr.Length - lStartOfObjts];

      StreamUtils.EnsureRead(inStr, lRest);

      //use a heuristic to find the SHET.
      //
      //we need an 8bit char set (ASCII is 7bit). windows-1252 will do
      string lRestAsString = Encoding.GetEncoding("windows-1252").GetString(lRest);
      Regex lSHETStartRegex = new Regex(
          "([a-zA-Z0-9'()*+.\\-_ ]{10,13})\0([a-zA-Z0-9'()*+.\\-_ ]{10,13})\0(.{7})",
          RegexOptions.CultureInvariant | RegexOptions.Singleline);

      MatchCollection matches = lSHETStartRegex.Matches(lRestAsString);
      if (matches.Count != 1)
        throw new DeserialisationException(string.Format("Expecting exactly one match for SHET start regex. Found {0}", matches.Count));

      int SHETstart = matches[0].Index; //(relative to end of images section)

      byte[] dunno = new byte[SHETstart];
      Array.Copy(lRest, 0, dunno, 0, SHETstart);
      Dunno = new RawDataChunk("Dunno", dunno);

      //seek to the start of the SHET, and pretend we got here without cheating!
      dunno = lRest = null;
      inStr.Seek(lStartOfObjts + SHETstart, SeekOrigin.Begin);

      SHET = new SHETChunk(inStr, bin);
    }

    public override void Serialise(Stream outStr)
    {
      BinaryWriter bout = new BinaryWriter(outStr);

      //first, a header of 4 int32s:
      for (int i = 0; i < Header.Length; i++)
        bout.Write(Header[i]);

      //then, a block of zero bytes (int32 aligned)
      for (int i = 0; i < ZeroByteCount; i++)
        bout.Write((byte)0);

      foreach (Chunk child in this.GetChildren())
      {
        child.Serialise(outStr);
      }
    }

    public override Chunk[] GetChildren()
    {
      ArrayList acc = new ArrayList();
      acc.Add(NamedImageGroups);
      if (OBJTFalseStart != null) acc.Add(OBJTFalseStart);
      if (OBJT != null) acc.Add(OBJT);
      if (Dunno != null) acc.Add(Dunno);
      acc.Add(SHET);
      return (Chunk[])acc.ToArray(typeof(Chunk));
    }

    private string mName = "MMv3 Level";

    public override string Name
    {
      get { return mName; }
    }

    public override void ReplaceChild(Chunk xiFrom, Chunk xiTo)
    {
      if (xiFrom == NamedImageGroups)
      {
        NamedImageGroups = (GroupingChunk)xiTo;
      }//qq
      else if (xiFrom == Dunno)
      {
        Dunno = (RawDataChunk)xiTo;
      }
      else if (xiFrom == SHET)
      {
        SHET = (SHETChunk)xiTo;
      }
      else
      {
          throw new Exception("TODO");
          throw new ArgumentException("xifrom not found!");
      }
    }

    public IEnumerable<GLTK.Entity> GetEntities(Level xiLevel, eTextureMode xiTextureMode, FlatChunk.TexMetaDataEntries xiSelectedMetadata)
    {
      List<Entity> lAcc = new List<Entity>();

      foreach (FlatChunk fl in SHET.Flats)
      {
        lAcc.AddRange(fl.GetEntities(xiLevel, xiTextureMode, xiSelectedMetadata));
      }

      return lAcc;
    }
  }
}
