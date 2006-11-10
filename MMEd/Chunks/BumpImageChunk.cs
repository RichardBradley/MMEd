using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections;
using MMEd.Util;

// represents a bump image
//
// referenced by TexMetaData[6]

namespace MMEd.Chunks
{
  public class BumpImageChunk : Chunk, Viewers.ImageViewer.IImageProvider
  {
    //qq It would be really good if this list could be merged
    //   with the list in makeBumpTypeToBumpTypeInfoMap, to reduce
    //   duplication of info. Is it possible to annotate Enum entries with
    //   attributes?
    public enum eBumpType
    {
      plain = 0x00,
      wall = 0x01,
      milk = 0x02,
      syrup = 0x03,
      ketchup = 0x04,
      roadBorder = 0x05,
      roadBorder2 = 0x06,
      water = 0x07,
      hole = 0x08, // A void or hole in the surface
      lilyPad = 0x09,
      grassStalk = 0x0A,
      unknown0B = 0x0B,
      unknown0C = 0x0C,
      roadBorderPool = 0x0D, // Edge of the track in a pool table course
      poolPocket = 0x0E, // A pool pocket - death trap
      unknown0F = 0x0F,
      unknown10 = 0x10,
      clearGoo = 0x11,
      powderSpill = 0x12,
      greenGoo = 0x13,
      redGoo = 0x14,
      roadBorderLab = 0x15,
      jumpWoosh = 0x16,
      unknown17 = 0x17,
      unknown18 = 0x18,
      enterTestTube = 0x19,
      unknown1A = 0x1A,
      unknown1B = 0x1B,
      microGooEdge = 0x1C,
      microGooMiddle = 0x1D,
      exitMicroscope = 0x1E,
      unknown1F = 0x1F,
      jumpWhoosh2 = 0x20,
      enterTeleport = 0x21,
      unknown22 = 0x22,
      sand = 0x23,
      unknown24 = 0x24,
      unknown25 = 0x25,
      unknown26 = 0x26,
      unknown27 = 0x27, // Driving over this has no discernable effect in Rack & Roll.
      teleport1 = 0x28, // Teleport. Destination unknown.
      teleport2 = 0x29, // Teleport. Destination unknown.
      teleport3 = 0x2A, // Teleport. Destination unknown.
      teleport4 = 0x2B, // Teleport. Destination unknown.
      teleport5 = 0x2C, // Teleport. Destination unknown.
      splash = 0x2D, // A "splash" - death trap.
      unknown2E = 0x2E,
      unknown2F = 0x2F,
    }

    public BumpImageChunk() { }
    public BumpImageChunk(int idx, Stream inStr)
    {
      mIdx = idx;
      Deserialise(inStr);
    }

    public override string Name
    {
      get
      {
        return string.Format("[{0}] BumpImage", mIdx);
      }
    }

    public override void Deserialise(Stream inStr)
    {
      Data = new byte[64];
      StreamUtils.EnsureRead(inStr, Data);
    }

    public override void Serialise(Stream outStr)
    {
      outStr.Write(Data, 0, Data.Length);
    }

    Bitmap mBitmapCache;
    public Image ToImage()
    {
        if (mBitmapCache == null)
        {
            mBitmapCache = ToBitmapUncached();
        }
        return mBitmapCache;
    }

    public Bitmap ToBitmapUncached()
    {
      Bitmap lBmp = new Bitmap(8 * SCALE, 8 * SCALE);
      Graphics g = Graphics.FromImage(lBmp);
      for (int x = 0; x < 8; x++)
        for (int y = 0; y < 8; y++) 
        {
            Brush b = new SolidBrush(mPalette[Data[8 * x + y]]); //(note x-y swap)
            g.FillRectangle(b, x*SCALE, y*SCALE, SCALE, SCALE);
        }
      return lBmp;
    }

    // Get the eBumpType for the supplied coordinates
    public eBumpType GetPixelType(int xiX, int xiY)
    {
      // Brackets are needed so that int division happens first
      return (eBumpType)Data[8 * xiX + xiY];
    }

    // Set the eBumpType for the supplied coordinates
    public void SetPixelType(int xiX, int xiY, eBumpType xiType)
    {
      // Brackets are needed so that int division happens first
      Data[8 * xiX + xiY] = (byte)xiType;
      mBitmapCache = null;
    }

    public static Hashtable BumpTypeToBumpTypeInfoMap = makeBumpTypeToBumpTypeInfoMap();

    private static Hashtable makeBumpTypeToBumpTypeInfoMap()
    {
      // Should this be in some kind of data file, for easy editing?
      // This will probably be fine, while everyone has VS
      Hashtable ht = new Hashtable();
      BTF(ht, eBumpType.plain, 0x000000);
      BTF(ht, eBumpType.wall, 0xcccccc);
      BTF(ht, eBumpType.milk, 0xf8e8d8);
      BTF(ht, eBumpType.syrup, 0xa87020);
      BTF(ht, eBumpType.ketchup, 0xff0000);
      BTF(ht, eBumpType.roadBorder, 0x89cba0);
      BTF(ht, eBumpType.roadBorder2, 0x89cbbf);
      BTF(ht, eBumpType.water, 0x4551ec);
      BTF(ht, eBumpType.hole, 0xffffff);
      BTF(ht, eBumpType.lilyPad, 0xff00ff);
      BTF(ht, eBumpType.grassStalk, 0xff00ff);
      BTF(ht, eBumpType.unknown0B, 0xff00ff);
      BTF(ht, eBumpType.unknown0C, 0xff00ff);
      BTF(ht, eBumpType.roadBorderPool, 0x89cbde);
      BTF(ht, eBumpType.poolPocket, 0xeeeeee);
      BTF(ht, eBumpType.unknown0F, 0xff00ff);
      BTF(ht, eBumpType.unknown10, 0xff00ff);
      BTF(ht, eBumpType.clearGoo, 0xff00ff);
      BTF(ht, eBumpType.powderSpill, 0xff00ff);
      BTF(ht, eBumpType.greenGoo, 0xff00ff);
      BTF(ht, eBumpType.redGoo, 0xff00ff);
      BTF(ht, eBumpType.roadBorderLab, 0xff00ff);
      BTF(ht, eBumpType.jumpWoosh, 0xffe400);
      BTF(ht, eBumpType.unknown17, 0xff00ff);
      BTF(ht, eBumpType.unknown18, 0xff00ff);
      BTF(ht, eBumpType.enterTestTube, 0xff00ff);
      BTF(ht, eBumpType.unknown1A, 0xff00ff);
      BTF(ht, eBumpType.unknown1B, 0xff00ff);
      BTF(ht, eBumpType.microGooEdge, 0xff00ff);
      BTF(ht, eBumpType.microGooMiddle, 0xff00ff);
      BTF(ht, eBumpType.exitMicroscope, 0xff00ff);
      BTF(ht, eBumpType.unknown1F, 0xff00ff);
      BTF(ht, eBumpType.jumpWhoosh2, 0xff00ff);
      BTF(ht, eBumpType.enterTeleport, 0xff00ff);
      BTF(ht, eBumpType.unknown22, 0xff00ff);
      BTF(ht, eBumpType.sand, 0xc9b549);
      BTF(ht, eBumpType.unknown24, 0xff00ff);
      BTF(ht, eBumpType.unknown25, 0xff00ff);
      BTF(ht, eBumpType.unknown26, 0xff00ff);
      BTF(ht, eBumpType.unknown27, 0xff00ff);
      BTF(ht, eBumpType.teleport1, 0xffaa00);
      BTF(ht, eBumpType.teleport2, 0xffdd00);
      BTF(ht, eBumpType.teleport3, 0xffbb00);
      BTF(ht, eBumpType.teleport4, 0xffee00);
      BTF(ht, eBumpType.teleport5, 0xffcc00);
      BTF(ht, eBumpType.splash, 0x4444ff);
      BTF(ht, eBumpType.unknown2E, 0xff00ff);
      BTF(ht, eBumpType.unknown2F, 0xff00ff);
      return ht;
    }

    private static void BTF(Hashtable xiHt, eBumpType xiVal, int xiColor)
    {
      xiHt[xiVal] = new BumpTypeInfo(xiVal, Color.FromArgb(xiColor | ~0x00ffffff));
    }

    private static Color[] mPalette = makePalette();

    private static Color[] makePalette()
    {
      Color[] acc = new Color[64];
      for (int i = 0; i < acc.Length; i++)
      {
        BumpTypeInfo bti = (BumpTypeInfo)BumpTypeToBumpTypeInfoMap[(eBumpType)i];
        acc[i] = bti == null ? Color.Magenta : bti.Color;
      }
      return acc;
    }

    public class BumpTypeInfo
    {
      public eBumpType Val;
      public Color Color;
      
      public BumpTypeInfo(eBumpType xiVal, Color xiColor)
      {
        Val = xiVal;
        Color = xiColor;
      }
      public BumpTypeInfo() { }
    }

    public override void ReplaceChild(Chunk xiFrom, Chunk xiTo)
    {
      throw new Exception("The method or operation is not implemented.");
    }

    int mIdx;
    [Description("The data.")]
    public byte[] Data;
    public const int SCALE = 16;
  }
}
