using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using MMEd.Util;

// Represents a PS TIM image
//
// Note that Sony calls a palette a "Colour Lookup Table" or CLUT
// for some reason best known to themselves

namespace MMEd.Chunks
{
  public class TIMChunk : Chunk, Viewers.ImageViewer.IImageProvider
  {
    public enum TimBPP
    {
      _4BPP = 0x00000008,
      _4BPPNC = 0x00000000, // No CLUT
      _8BPP = 0x00000009,
      _8BPPNC = 0x00000001, // No CLUT
      _16BPP = 0x00000002
    }

    public const int TIM_MAGIC_NUMBER = 0x10;

    #region public fields

    [Description(@"Bits per pixel. Only a subset supported in this implementation. 
        It could be extended using the information in timgfx.txt")]
    public TimBPP BPP;

    [Description("The total size, in bytes, of the CLUTs, + 12")]
    public int ClutSize;

    [Description("The x-offset of our small palette in the active palette")]
    public short PaletteOrgX;

    [Description("The y-offset of our small palette in the active palette")]
    public short PaletteOrgY;

    [Description("The number of colours in each CLUT")]
    public short ClutColors;

    [Description("The number of CLUTs")]
    public short ClutCount;

    [Description("The palette, as defined by the CLUT(s)")]
    public short[] Palette;

    [Description("The total size, in bytes, of the image data, + 12")]
    public int ImageDataSize;

    [Description("The x-offset of our image in the VRAM")]
    public short ImageOrgX;

    [Description("The y-offset of our image in the VRAM")]
    public short ImageOrgY;

    [Description("The width of the image, in pixels")]
    public short ImageWidth;

    [Description("The height of the image, in pixels")]
    public short ImageHeight;

    [Description("The image data itself")]
    public byte[] ImageData;

    [Description("Zeroes at the end")]
    public byte[] ZeroPadding;

    #endregion

    public TIMChunk() { }
    public TIMChunk(System.IO.Stream inStr, string xiName) { mName = xiName; Deserialise(inStr); }

    public void Deserialise(System.IO.Stream inStr, int xiExpectedDataSize)
    {
      long lStartOffset = xiExpectedDataSize >= 0 ? inStr.Position : -1;

      // magic number
      BinaryReader bin = new BinaryReader(inStr);
      int lTIMMagicNumber = bin.ReadInt32();
      if (TIM_MAGIC_NUMBER != lTIMMagicNumber)
      {
        throw new DeserialisationException(string.Format("TIM should start with 4 byte magic number 0x10, found 0x{0:x}", lTIMMagicNumber), inStr.Position - 4);
      }

      // bits per pixel
      BPP = (TimBPP)bin.ReadInt32();
      if (BPP != TimBPP._4BPP
        && BPP != TimBPP._8BPP
        && BPP != TimBPP._16BPP)
      {
        throw new TIMTypeNotImplementedException(string.Format("Only 4BPP, 8BPP or 16BPP TIMs are supported. Found 0x{0:x} BPP type", BPP), inStr.Position - 8);
      }

      if (BPP != TimBPP._16BPP)
      {
        // Now the palette header:
        ClutSize = bin.ReadInt32();
        PaletteOrgX = bin.ReadInt16();
        PaletteOrgY = bin.ReadInt16();
        ClutColors = bin.ReadInt16();
        ClutCount = bin.ReadInt16();

        //sanity checks
        if (ClutCount * ClutColors * 2 != ClutSize - 12)
          throw new DeserialisationException("bad TIM: ClutCount*ClutColors*2 != ClutSize - 12", inStr.Position);

        //TODO:
        if (ClutCount != 1)
          throw new DeserialisationException("Multi-clut TIMs are not yet supported. Please extend TIMChunk.cs using info from timgfx.txt");

        //CLUT:
        Palette = new short[ClutCount * ClutColors];
        for (int i = 0; i < Palette.Length; i++)
        {
          Palette[i] = bin.ReadInt16();
        }
      }

      //colour count
      int lPixelsPerTwoBytes;
      switch (BPP)
      {
        case TimBPP._4BPP:
          if (ClutColors != 16) throw new DeserialisationException("bad TIM: ClutColors != 16, but BPP == 4", inStr.Position);
          lPixelsPerTwoBytes = 4;
          break;
        case TimBPP._8BPP:
          if (ClutColors != 256) throw new DeserialisationException("bad TIM: ClutColors != 256, but BPP == 8", inStr.Position);
          lPixelsPerTwoBytes = 2;
          break;
        case TimBPP._16BPP:
          if (ClutColors != 0) throw new DeserialisationException("bad TIM: ClutColors != 0, but BPP == 16", inStr.Position);
          lPixelsPerTwoBytes = 1;
          break;
        default: throw new Exception("unreachable case");
      }

      //Second header:
      ImageDataSize = bin.ReadInt32();
      ImageOrgX = bin.ReadInt16();
      ImageOrgY = bin.ReadInt16();
      //short * short is int !?!
      ImageWidth = (short)(lPixelsPerTwoBytes * bin.ReadInt16());
      ImageHeight = bin.ReadInt16();

      //sanity checks
      //some observed TIMs fail this check:
      if (ImageWidth * ImageHeight / lPixelsPerTwoBytes * 2 != ImageDataSize - 12)
      {
        //throw new DeserialisationException
        System.Diagnostics.Trace.WriteLine(string.Format("bad TIM: ImageWidth * ImageHeight / PixelsPerByte != ImageDataSize - 12, near {0}", inStr.Position));
      }

      //read the image data
      ImageData = bin.ReadBytes(ImageWidth * ImageHeight / lPixelsPerTwoBytes * 2);

      if (xiExpectedDataSize >= 0)
      {
        long lObservedSize = inStr.Position - lStartOffset;
        if (lObservedSize < xiExpectedDataSize)
        {
          ZeroPadding = bin.ReadBytes((int)(xiExpectedDataSize - lObservedSize));
          foreach (byte b in ZeroPadding)
            if (b != 0)
              throw new DeserialisationException(string.Format("Non zero value found in zero-padding section: {0}", b), inStr.Position);
        }
        else if (lObservedSize > xiExpectedDataSize)
        {
          throw new DeserialisationException(string.Format("Expected TIM datasize was {0}, found {1}", xiExpectedDataSize, lObservedSize));
        }
      }
    }

    public override void Deserialise(Stream inStr)
    {
      Deserialise(inStr, -1);
    }

    public override void Serialise(System.IO.Stream outStr)
    {
      BinaryWriter bout = new BinaryWriter(outStr);

      // magic number
      bout.Write((int)TIM_MAGIC_NUMBER);

      // bits per pixel (fixed atm)
      bout.Write((int)BPP);

      // bits per pixel
      if (BPP != TimBPP._4BPP
        && BPP != TimBPP._8BPP
        && BPP != TimBPP._16BPP)
      {
        throw new Exception(string.Format("Only 4BPP, 8BPP or 16BPP TIMs are supported. Found 0x{0:x8} BPP type", BPP));
      }

      if (BPP != TimBPP._16BPP)
      {
        // Now the main header:
        bout.Write(ClutSize);
        bout.Write(PaletteOrgX);
        bout.Write(PaletteOrgY);
        bout.Write(ClutColors);
        bout.Write(ClutCount);

        //TODO:
        if (ClutCount != 1)
          throw new DeserialisationException("Multi-clut TIMs are not yet supported. Please extend TIMChunk.cs using info from timgfx.txt");

        //CLUT:
        for (int i = 0; i < Palette.Length; i++)
        {
          bout.Write((short)Palette[i]);
        }
      }

      //colour count
      int lPixelsPerTwoBytes;
      switch (BPP)
      {
        case TimBPP._4BPP:
          if (ClutColors != 16) throw new Exception("bad TIM: ClutColors != 16, but BPP == 4");
          lPixelsPerTwoBytes = 4;
          break;
        case TimBPP._8BPP:
          if (ClutColors != 256) throw new Exception("bad TIM: ClutColors != 256, but BPP == 8");
          lPixelsPerTwoBytes = 2;
          break;
        case TimBPP._16BPP:
          if (ClutColors != 0) throw new Exception("bad TIM: ClutColors != 0, but BPP == 16");
          lPixelsPerTwoBytes = 1;
          break;
        default: throw new Exception("unreachable case");
      }

      //Second header:
      bout.Write(ImageDataSize);
      bout.Write(ImageOrgX);
      bout.Write(ImageOrgY);
      bout.Write((short)(ImageWidth / lPixelsPerTwoBytes));
      bout.Write(ImageHeight);

      //the image data. Not currently synched with ToBitmap()
      bout.Write(ImageData);

      if (ZeroPadding != null)
        bout.Write(ZeroPadding);
    }

    /// <summary>
    ///   The length, in bytes, of this TIM when serialised
    /// </summary>
    public int DataLength
    {
      get
      {
        if (BPP != TimBPP._4BPP
          && BPP != TimBPP._8BPP
          && BPP != TimBPP._16BPP)
        {
          throw new Exception(string.Format("Only 4BPP TIMs are supported. Found 0x{0:x} BPP type", BPP));
        }

        // (use ImageData.Length + 12, not ImageDataSize, because of occasionally failed consistency checks)
        return 8 + ClutSize + ImageData.Length + 12 + (ZeroPadding == null ? 0 : ZeroPadding.Length);
      }
    }

    private string mName;
    public override string Name
    {
      get
      {
        return mName == null
        ? (mIdx >= 0
          ? string.Format("[{0}] TIM", mIdx)
          : "TIM")
        : mName;
      }
    }

    public void InvalidateBitmapCache()
    {
      mBitmapCache = null;
    }

    public Image ToImage()
    {
      return ToBitmap();
    }

    public Bitmap ToBitmap()
    {
      if (mBitmapCache == null)
      {
        try
        {
          mBitmapCache = new Bitmap(CreateBMPStream());
        }
        catch (Exception e)
        {
          //I seem to get a fair number of GDI+ exceptions from the
          //unmanaged call. Don't really know why.
          Console.Error.WriteLine(e);
          switch (BPP)
          {
            case TimBPP._4BPP:
              mBitmapCache = CreateBitmapManaged4bpp();
              break;
            case TimBPP._8BPP:
              mBitmapCache = CreateBitmapManaged8bpp();
              break;
            case TimBPP._16BPP:
              mBitmapCache = CreateBitmapManaged16bpp();
              break;
            default: throw new Exception(string.Format("Unsupported TIM type. Found 0x{0:x8} BPP type", BPP));
          }
        }
      }
      return mBitmapCache;
    }

    //creates a bitmap using managed code only
    private Bitmap CreateBitmapManaged4bpp()
    {
      Bitmap acc = new Bitmap(ImageWidth, ImageHeight, PixelFormat.Format32bppArgb);
      for (int y = 0; y < ImageHeight; y++)
      {
        for (int x = 0; x < ImageWidth; x += 2)
        {
          byte v = ImageData[y * ImageWidth / 2 + x / 2];
          Color left = Color.FromArgb(Utils.PS16bitColorToARGB(Palette[v & 0xf]));
          Color right = Color.FromArgb(Utils.PS16bitColorToARGB(Palette[(v >> 4) & 0xf]));
          acc.SetPixel(x, y, left);
          acc.SetPixel(x + 1, y, right);
        }
      }
      return acc;
    }

    //creates a bitmap using managed code only
    private Bitmap CreateBitmapManaged8bpp()
    {
      Bitmap acc = new Bitmap(ImageWidth, ImageHeight, PixelFormat.Format32bppArgb);
      try
      {
        for (int y = 0; y < ImageHeight; y++)
        {
          for (int x = 0; x < ImageWidth; x++)
          {
            byte v = ImageData[y * ImageWidth + x];
            Color c = Color.FromArgb(Utils.PS16bitColorToARGB(Palette[v]));
            acc.SetPixel(x, y, c);
          }
        }
      }
      catch
      {
        //qq continue on error. Needed for PRERACE.DAT @ 466444
      }
      return acc;
    }

    //creates a bitmap using managed code only
    private Bitmap CreateBitmapManaged16bpp()
    {
      Bitmap acc = new Bitmap(ImageWidth, ImageHeight, PixelFormat.Format32bppArgb);
      for (int y = 0; y < ImageHeight; y++)
      {
        for (int x = 0; x < ImageWidth; x++)
        {
          byte lo = ImageData[y * ImageWidth * 2 + x * 2];
          byte hi = ImageData[y * ImageWidth * 2 + x * 2 + 1];

          Color c = Color.FromArgb(Utils.PS16bitColorToARGB((short)(lo | (hi << 8))) | ~0xffffff);
          acc.SetPixel(x, y, c);
        }
      }
      return acc;
    }

    public Stream CreateBMPStream()
    {
      if (this.BPP != TimBPP._4BPP
        && this.BPP != TimBPP._8BPP
        && this.BPP != TimBPP._16BPP)
      {
        throw new Exception(string.Format("Unsupported TIM type. Found 0x{0:x8} BPP type", BPP));
      }

      //create a BMP stream
      int lBpp;
      int lDataOffset;
      int lDataSize;
      int lFileSize;
      switch (BPP)
      {
        case TimBPP._4BPP:
          lBpp = 4;
          lDataOffset = 54 + 16 * 4;
          lDataSize = ImageData.Length;
          lFileSize = lDataOffset + lDataSize;
          break;
        case TimBPP._8BPP:
          lBpp = 8;
          lDataOffset = 54 + 256 * 4;
          lDataSize = ImageData.Length;
          lFileSize = lDataOffset + ImageData.Length;
          break;
        case TimBPP._16BPP:
          lBpp = 32;
          lDataOffset = 54;
          lDataSize = ImageData.Length * 2;
          lFileSize = lDataOffset + lDataSize;
          break;
        default: throw new Exception("unreachable case");
      }

      MemoryStream mem = new MemoryStream(lFileSize);
      BinaryWriter bout = new BinaryWriter(mem);
      bout.Write((byte)'B'); //bfType[1]
      bout.Write((byte)'M'); //bfType[2]
      bout.Write(lFileSize); //bfSize
      bout.Write((int)0); //bfReserved1, bfReserved2
      bout.Write(lDataOffset); //bfOffBits
      bout.Write(40); //biSize
      bout.Write((int)ImageWidth);//biWidth
      bout.Write((int)ImageHeight);//biHeight
      bout.Write((short)1);//biPlanes
      bout.Write((short)lBpp);//biBitCount
      bout.Write((int)0);//biCompression
      bout.Write((int)lDataSize);//biSizeImage (0 is valid when no compression)
      bout.Write((int)0);//biXPelsPerMeter
      bout.Write((int)0);//biYPelsPerMeter
      bout.Write((int)0);//biClrUsed (0 = derived from biBitCount)
      bout.Write((int)0);//biClrImportant
      if (Palette != null)
      {
        foreach (short c in Palette)
        {
          bout.Write((int)(Utils.PS16bitColorToARGB(c) & 0xffffff));
        }
      }

      switch (BPP)
      {
        case TimBPP._4BPP:
          {
            int h = ImageHeight;
            int w = ImageWidth / 2;

            for (int y = h - 1; y >= 0; y--)
            {
              for (int x = 0; x < w; x++)
              {
                byte b = ImageData[y * w + x];
                bout.Write((byte)(((b >> 4) & 0xf) | ((b << 4) & 0xf0)));
              }
            }
          } break;

        case TimBPP._8BPP:
          {
            int h = ImageHeight;
            int w = ImageWidth;

            for (int y = h - 1; y >= 0; y--)
            {
              for (int x = 0; x < w; x++)
              {
                bout.Write(ImageData[y * w + x]);
              }
            }
          } break;

        case TimBPP._16BPP:
          for (int y = ImageHeight - 1; y >= 0; y--)
          {
            for (int x = 0; x < ImageWidth; x++)
            {
              byte lo = ImageData[y * ImageWidth * 2 + x * 2];
              byte hi = ImageData[y * ImageWidth * 2 + x * 2 + 1];

              bout.Write((int)(Utils.PS16bitColorToARGB((short)(lo | (hi << 8))) & 0xffffff));
            }
          }
          break;

        default: throw new Exception("unreachable case");
      }

      if (mem.Length != lFileSize) throw new Exception();
      mem.Seek(0, SeekOrigin.Begin);

      return mem;
    }

    Bitmap mBitmapCache;
    int mIdx = -1;

    public TIMChunk(int xiIdx, Stream xiInStr, int xiExpectedLength)
    {
      mIdx = xiIdx;
      Deserialise(xiInStr, xiExpectedLength);
    }

    public override void ReplaceChild(Chunk xiFrom, Chunk xiTo)
    {
      throw new Exception("The method or operation is not implemented.");
    }

    public class TIMTypeNotImplementedException : DeserialisationException
    {
      public TIMTypeNotImplementedException(string xiMessage, long xiOffset) : base(xiMessage, xiOffset) { }
    }

    public void FillDataFromBitmap(Bitmap xiBmp)
    {
      //simple checks:
      if (xiBmp.Width != ImageWidth
        || xiBmp.Height != ImageHeight)
      {
        throw new Exception(string.Format("BMP is different size to TIM ({0} vs ({1},{2}))",
          xiBmp.Size, ImageWidth, ImageHeight));
      }

      if (BPP != TIMChunk.TimBPP._4BPP
        && BPP != TIMChunk.TimBPP._8BPP
        && BPP != TIMChunk.TimBPP._16BPP)
      {
        throw new Exception("Only 4BPP, 8BPP or 16BPP TIMs supported");
      }

      int w = xiBmp.Width;
      int h = xiBmp.Height;
      if (BPP == TimBPP._4BPP || BPP == TimBPP._8BPP)
      {
        //paletted:
        //

        //we don't insist that the BMP is paletted, just that it only uses colours in the palette
        Dictionary<int, int> lColorToPaletteIdx = new Dictionary<int, int>();
        for (int i = Palette.Length - 1; i >= 0; i--)
        {
          lColorToPaletteIdx[Utils.PS16bitColorToARGB(Palette[i])] = i;
        }

        int x = -1, y = -1;
        Color c = Color.Black;
        try
        {
          if (BPP == TimBPP._4BPP)
          {
            int rowLen = w / 2;
            for (y = 0; y < h; y++)
            {
              for (x = 0; x < w; x += 2)
              {
                c = xiBmp.GetPixel(x, y);
                int left = lColorToPaletteIdx[c.ToArgb()];
                c = xiBmp.GetPixel(x + 1, y);
                int right = lColorToPaletteIdx[c.ToArgb()];
                byte b = (byte)(left | (right << 4));
                ImageData[y * rowLen + x / 2] = b;
              }
            }
          }
          else if (BPP == TimBPP._8BPP)
          {
            int rowLen = w;
            for (y = 0; y < h; y++)
            {
              for (x = 0; x < w; x++)
              {
                c = xiBmp.GetPixel(x, y);
                int entry = lColorToPaletteIdx[c.ToArgb()];
                byte b = (byte)entry;
                ImageData[y * rowLen + x] = b;
              }
            }
          }
          else throw new Exception("unreachable case");
        }
        catch (KeyNotFoundException)
        {
          throw new Exception(string.Format("The color 0x{0:x} is used in the BMP at ({1},{2}), but doesn't appear in the TIM palette",
            c.ToArgb(), x, y));
        }
      }
      else if (BPP == TimBPP._16BPP)
      {
        int rowLen = w * 2;
        for (int y = 0; y < h; y++)
        {
          for (int x = 0; x < w; x++)
          {
            Color c = xiBmp.GetPixel(x, y);
            short entry = Utils.ARGBColorToPS16bit(c.ToArgb());
            byte lo = (byte)entry;
            byte hi = (byte)(entry >> 8);
            ImageData[y * rowLen + x * 2] = lo;
            ImageData[y * rowLen + x * 2 + 1] = hi;
          }
        }
      }
      else throw new Exception("unreachable case");

      InvalidateBitmapCache();
    }
  }
}
