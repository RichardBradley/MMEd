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
      unknown08 = 0x08,
      unknown09 = 0x09,
      unknown0A = 0x0A,
      unknown0B = 0x0B,
      unknown0C = 0x0C,
      unknown0D = 0x0D,
      unknown0E = 0x0E,
      unknown0F = 0x0F,
      unknown10 = 0x10,
      unknown11 = 0x11,
      unknown12 = 0x12,
      unknown13 = 0x13,
      unknown14 = 0x14,
      unknown15 = 0x15,
      jumpWoosh = 0x16,
      unknown17 = 0x17,
      unknown18 = 0x18,
      unknown19 = 0x19,
      unknown1A = 0x1A,
      unknown1B = 0x1B,
      unknown1C = 0x1C,
      unknown1D = 0x1D,
      unknown1E = 0x1E,
      unknown1F = 0x1F,
      unknown20 = 0x20,
      unknown21 = 0x21,
      unknown22 = 0x22,
      sand = 0x23,
      unknown24 = 0x24,
      unknown25 = 0x25,
      unknown26 = 0x26,
      unknown27 = 0x27,
      unknown28 = 0x28,
      unknown29 = 0x29,
      unknown2A = 0x2A,
      unknown2B = 0x2B,
      unknown2C = 0x2C,
      unknown2D = 0x2D,
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

    public Image ToImage()
    {
      Bitmap lBmp = new Bitmap(8 * mScale, 8 * mScale);
      for (int x = 0; x < 8 * mScale; x++)
        for (int y = 0; y < 8 * mScale; y++)
          lBmp.SetPixel(x, y, palette[Data[8 * (x / mScale) + y / mScale]]); //(note x-y swap)

      // Outline the squares in black
      for (int x = 0; x < 8; x++)
      {
        for (int y = 0; y < 8 * mScale; y += 2)
        {
          lBmp.SetPixel(x * mScale, y, Color.Black);
          lBmp.SetPixel(x * mScale, y + 1, Color.White);
        }
      }
      for (int y = 0; y < 8; y++)
      {
        for (int x = 0; x < 8 * mScale; x += 2)
        {
          lBmp.SetPixel(x, y * mScale, Color.Black);
          lBmp.SetPixel(x + 1, y * mScale, Color.White);
        }
      }

      // Outline selected pixel in white
      for (int x = mX * mScale; x < (mX + 1) * mScale; ++x)
      {
        lBmp.SetPixel(x, mY * mScale, Color.White);
        lBmp.SetPixel(x, ((mY + 1) * mScale) - 1, Color.White);
      }
      for (int y = mY * mScale; y < (mY + 1) * mScale; ++y)
      {
        lBmp.SetPixel(mX * mScale, y, Color.White);
        lBmp.SetPixel(((mX + 1) * mScale) - 1, y, Color.White);
      }
      return lBmp;
    }

    public void SetSelectedPixel(int xiX, int xiY)
    {
      mX = xiX / mScale;
      mY = xiY / mScale;
    }

    // Get the eBumpType for the supplied coordinates
    public eBumpType GetPixelType(int xiX, int xiY)
    {
      // Brackets are needed so that int division happens first
      return (eBumpType)Data[(8 * (xiX / mScale)) + (xiY / mScale)];
    }

    // Set the eBumpType for the supplied coordinates
    public void SetPixelType(int xiX, int xiY, eBumpType xiType)
    {
      // Brackets are needed so that int division happens first
      Data[(8 * (xiX / mScale)) + (xiY / mScale)] = (byte)xiType;
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
      BTF(ht, eBumpType.unknown08, 0xff00ff);
      BTF(ht, eBumpType.unknown09, 0xff00ff);
      BTF(ht, eBumpType.unknown0A, 0xff00ff);
      BTF(ht, eBumpType.unknown0B, 0xff00ff);
      BTF(ht, eBumpType.unknown0C, 0xff00ff);
      BTF(ht, eBumpType.unknown0D, 0xff00ff);
      BTF(ht, eBumpType.unknown0E, 0xff00ff);
      BTF(ht, eBumpType.unknown0F, 0xff00ff);
      BTF(ht, eBumpType.unknown10, 0xff00ff);
      BTF(ht, eBumpType.unknown11, 0xff00ff);
      BTF(ht, eBumpType.unknown12, 0xff00ff);
      BTF(ht, eBumpType.unknown13, 0xff00ff);
      BTF(ht, eBumpType.unknown14, 0xff00ff);
      BTF(ht, eBumpType.unknown15, 0xff00ff);
      BTF(ht, eBumpType.jumpWoosh, 0xffe400);
      BTF(ht, eBumpType.unknown17, 0xff00ff);
      BTF(ht, eBumpType.unknown18, 0xff00ff);
      BTF(ht, eBumpType.unknown19, 0xff00ff);
      BTF(ht, eBumpType.unknown1A, 0xff00ff);
      BTF(ht, eBumpType.unknown1B, 0xff00ff);
      BTF(ht, eBumpType.unknown1C, 0xff00ff);
      BTF(ht, eBumpType.unknown1D, 0xff00ff);
      BTF(ht, eBumpType.unknown1E, 0xff00ff);
      BTF(ht, eBumpType.unknown1F, 0xff00ff);
      BTF(ht, eBumpType.unknown20, 0xff00ff);
      BTF(ht, eBumpType.unknown21, 0xff00ff);
      BTF(ht, eBumpType.unknown22, 0xff00ff);
      BTF(ht, eBumpType.sand, 0xc9b549);
      BTF(ht, eBumpType.unknown24, 0xff00ff);
      BTF(ht, eBumpType.unknown25, 0xff00ff);
      BTF(ht, eBumpType.unknown26, 0xff00ff);
      BTF(ht, eBumpType.unknown27, 0xff00ff);
      BTF(ht, eBumpType.unknown28, 0xff00ff);
      BTF(ht, eBumpType.unknown29, 0xff00ff);
      BTF(ht, eBumpType.unknown2A, 0xff00ff);
      BTF(ht, eBumpType.unknown2B, 0xff00ff);
      BTF(ht, eBumpType.unknown2C, 0xff00ff);
      BTF(ht, eBumpType.unknown2D, 0xff00ff);
      BTF(ht, eBumpType.unknown2E, 0xff00ff);
      BTF(ht, eBumpType.unknown2F, 0xff00ff);
      return ht;
    }

    private static void BTF(Hashtable xiHt, eBumpType xiVal, int xiColor)
    {
      xiHt[xiVal] = new BumpTypeInfo(xiVal, Color.FromArgb(xiColor | ~0x00ffffff));
    }

    private static Color[] palette = makePalette();

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
    private int mX;
    private int mY;
    private const int mScale = 16;
  }
}
