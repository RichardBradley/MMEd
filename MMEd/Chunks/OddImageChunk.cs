using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.ComponentModel;
using MMEd.Util;
using System.Drawing;

// represents one of those images after the key waypoints, and before
// the camera position array
//
// I don't know what they represent. They're possibly something to do with
// reset positions, and possibly referenced by TexMetaData[7]
// [The above comment looks relatively unlikely based on an attempt to overlay
//  this data onto Rack & Roll in graphical format. However it's still possible]
//
// TODO: rename this class once the meaning is clearer

namespace MMEd.Chunks
{
  public class OddImageChunk : Chunk, Viewers.ImageViewer.IImageProvider
  {
    int mIdx;

    [Description("The data.")]
    public byte[] Data;

    public OddImageChunk() { }
    public OddImageChunk(int idx, Stream inStr)
    {
      mIdx = idx;
      Deserialise(inStr);
    }

    public override string Name
    {
      get
      {
        return string.Format("[{0}] OddImage", mIdx);
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

    public override void ReplaceChild(Chunk xiFrom, Chunk xiTo)
    {
      throw new Exception("The method or operation is not implemented.");
    }

    #region IImageProvider Members

    Bitmap mBitmapCache;
    public System.Drawing.Image ToImage()
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
          g.FillRectangle(b, x * SCALE, y * SCALE, SCALE, SCALE);
        }
      return lBmp;
    }

    private static Color[] mPalette = makePalette();

    private static Color[] makePalette()
    {
      Color[] acc = new Color[64];
      for (int i = 0; i < 21; i++)
      {
        acc[i] = Color.FromArgb((int)(255 * ((double)i/21)), 0, 0);
      }
      for (int i = 21; i < 42; i++)
      {
        acc[i] = Color.FromArgb(0, (int)(255 * ((double)(i - 21) / 21)), 0);
      }
      for (int i = 42; i < acc.Length; i++)
      {
        acc[i] = Color.FromArgb(0, 0, (int)(255 * ((double)(i - 42) / (acc.Length - 42))));
      }
      return acc;
    }

    public const int SCALE = 16;

    #endregion
  }
}
