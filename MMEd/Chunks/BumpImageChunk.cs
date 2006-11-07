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
    int mIdx;

    [Description("The data.")]
    public byte[] Data;

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
      int scale = 8;
      Bitmap lBmp = new Bitmap(8 * scale, 8 * scale);
      for (int x = 0; x < 8 * scale; x++)
        for (int y = 0; y < 8 * scale; y++)
          lBmp.SetPixel(x, y, palette[Data[8 * (x / scale) + y / scale]]); //(note x-y swap)

      // Outline the squares in black
      for (int x = 0; x < 8; x++)
      {
        for (int y = 0; y < 8 * scale; y++)
        {
          lBmp.SetPixel(x * scale, y, Color.Black);
        }
      }
      for (int y = 0; y < 8; y++)
      {
        for (int x = 0; x < 8 * scale; x++)
        {
          lBmp.SetPixel(x, y * scale, Color.Black);
        }
      }
      return lBmp;
    }

    // Get the BumpTypeInfo for the supplied coordinates
    public BumpTypeInfo GetInfo(int xiX, int xiY)
    {
      Byte lByte = Data[(8 * xiX) + xiY];

      return (BumpTypeInfo)BumpTypeToBumpTypeInfoMap[(byte)lByte];
    }

    // Set the BumpTypeInfo for the supplied coordinates
    public void SetInfo(int xiX, int xiY, BumpTypeInfo xiInfo)
    {
      Data[(8 * xiX) + xiY] = xiInfo.Val;
    }

    public static Hashtable BumpTypeToBumpTypeInfoMap = makeBumpTypeToBumpTypeInfoMap();

    private static Hashtable makeBumpTypeToBumpTypeInfoMap()
    {
      // Should this be in some kind of data file, for easy editing?
      // This will probably be fine, while everyone has VS
      Hashtable ht = new Hashtable();
      BTF(ht, 0x00, 0x000000, "plain");
      BTF(ht, 0x01, 0xcccccc, "wall");
      BTF(ht, 0x02, 0xf8e8d8, "milk");
      BTF(ht, 0x03, 0xa87020, "syrup");
      BTF(ht, 0x04, 0xff0000, "ketchup");
      BTF(ht, 0x05, 0x89cba0, "road border");
      BTF(ht, 0x06, 0x89cbbf, "road border2");
      BTF(ht, 0x07, 0x4551ec, "water");
      BTF(ht, 0x08, 0xff00ff, "unknown");
      BTF(ht, 0x09, 0xff00ff, "unknown");
      BTF(ht, 0x0A, 0xff00ff, "unknown");
      BTF(ht, 0x0B, 0xff00ff, "unknown");
      BTF(ht, 0x0C, 0xff00ff, "unknown");
      BTF(ht, 0x0D, 0xff00ff, "unknown");
      BTF(ht, 0x0E, 0xff00ff, "unknown");
      BTF(ht, 0x0F, 0xff00ff, "unknown");
      BTF(ht, 0x10, 0xff00ff, "unknown");
      BTF(ht, 0x11, 0xff00ff, "unknown");
      BTF(ht, 0x12, 0xff00ff, "unknown");
      BTF(ht, 0x13, 0xff00ff, "unknown");
      BTF(ht, 0x14, 0xff00ff, "unknown");
      BTF(ht, 0x15, 0xff00ff, "unknown");
      BTF(ht, 0x16, 0xffe400, "jump woosh");
      BTF(ht, 0x17, 0xff00ff, "unknown");
      BTF(ht, 0x18, 0xff00ff, "unknown");
      BTF(ht, 0x19, 0xff00ff, "unknown");
      BTF(ht, 0x1A, 0xff00ff, "unknown");
      BTF(ht, 0x1B, 0xff00ff, "unknown");
      BTF(ht, 0x1C, 0xff00ff, "unknown");
      BTF(ht, 0x1D, 0xff00ff, "unknown");
      BTF(ht, 0x1E, 0xff00ff, "unknown");
      BTF(ht, 0x1F, 0xff00ff, "unknown");
      BTF(ht, 0x20, 0xff00ff, "unknown");
      BTF(ht, 0x21, 0xff00ff, "unknown");
      BTF(ht, 0x22, 0xff00ff, "unknown");
      BTF(ht, 0x23, 0xc9b549, "sand");
      BTF(ht, 0x24, 0xff00ff, "unknown");
      BTF(ht, 0x25, 0xff00ff, "unknown");
      BTF(ht, 0x26, 0xff00ff, "unknown");
      BTF(ht, 0x27, 0xff00ff, "unknown");
      BTF(ht, 0x28, 0xff00ff, "unknown");
      BTF(ht, 0x29, 0xff00ff, "unknown");
      BTF(ht, 0x2A, 0xff00ff, "unknown");
      BTF(ht, 0x2B, 0xff00ff, "unknown");
      BTF(ht, 0x2C, 0xff00ff, "unknown");
      BTF(ht, 0x2D, 0xff00ff, "unknown");
      BTF(ht, 0x2E, 0xff00ff, "unknown");
      BTF(ht, 0x2F, 0xff00ff, "unknown");
      return ht;
    }

    private static void BTF(Hashtable xiHt, byte xiVal, int xiColor, string xiDescription)
    {
      xiHt[xiVal] = new BumpTypeInfo(xiVal, Color.FromArgb(xiColor | ~0x00ffffff), xiDescription);
    }

    private static Color[] palette = makePalette();

    private static Color[] makePalette()
    {
      Color[] acc = new Color[256];
      for (int i = 0; i < acc.Length; i++)
      {
        BumpTypeInfo bti = (BumpTypeInfo)BumpTypeToBumpTypeInfoMap[(byte)i];
        acc[i] = bti == null ? Color.Magenta : bti.Color;
      }
      return acc;
    }

    public class BumpTypeInfo
    {
      public byte Val; public Color Color; public string Description;
      public BumpTypeInfo(byte xiVal, Color xiColor, string xiDescription)
      {
        Val = xiVal;
        Color = xiColor;
        Description = xiDescription;
      }
      public BumpTypeInfo() { }
    }

    public override void ReplaceChild(Chunk xiFrom, Chunk xiTo)
    {
      throw new Exception("The method or operation is not implemented.");
    }
  }
}
