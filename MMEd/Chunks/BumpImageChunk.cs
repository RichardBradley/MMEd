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
  public class BumpImageChunk : Chunk, Viewers.BumpViewer.IBumpProvider, Viewers.ImageViewer.IImageProvider
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
      return lBmp;
    }

    public static Hashtable BumpTypeToBumpTypeInfoMap = makeBumpTypeToBumpTypeInfoMap();

    private static Hashtable makeBumpTypeToBumpTypeInfoMap()
    {
      // Should this be in some kind of data file, for easy editing?
      // This will probably be fine, while everyone has VS
      Hashtable ht = new Hashtable();
      BTF(ht, 0, 0x000000, "plain");
      BTF(ht, 1, 0xcccccc, "wall");
      BTF(ht, 2, 0xf8e8d8, "milk");
      BTF(ht, 3, 0xa87020, "syrup");
      BTF(ht, 4, 0xff0000, "ketchup");
      BTF(ht, 5, 0x89cba0, "road border");
      BTF(ht, 6, 0x89cbbf, "road border2");
      BTF(ht, 7, 0x4551ec, "water");
      BTF(ht, 0x16, 0xffe400, "jump woosh");
      BTF(ht, 0x23, 0xc9b549, "sand");
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
