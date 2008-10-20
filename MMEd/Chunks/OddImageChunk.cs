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
  public class OddImageChunk : Chunk, Viewers.ImageViewer.IImageProvider, IReindexableChunk
  {
    int mIdx;
    private byte[] mData;

    [Description("The data.")]
    public byte[] Data
    {
      get { return mData; }
      set
      {
        mData = value;
        mBitmapCache = null;
      }
    }

    //set the odd to be all zero
    public void Clear()
    {
      Array.Clear(Data, 0, Data.Length);
      mBitmapCache = null;
    }

    public OddImageChunk() { }
    public OddImageChunk(int idx, byte xiValue)
    {
      mIdx = idx;
      Data = new byte[64];
      FlushToValue(xiValue);
    }

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

    public void FlushToValue(byte xiNewValue)
    {
      for (int i = 0; i < Data.Length; i++)
      {
        Data[i] = xiNewValue;
      }
    }

    public static OddTypeInfo GetOddTypeInfo(byte xiVal)
    {
      if (xiVal > mOddTypes.Length)
      {
        return null;
      }
      else
      {
        return mOddTypes[(int)xiVal];
      }
    }

    // Get the eBumpType for the supplied coordinates
    public byte GetPixelType(int xiX, int xiY)
    {
      return Data[8 * xiX + xiY];
    }

    // Set the eBumpType for the supplied coordinates
    public void SetPixelType(int xiX, int xiY, byte xiType)
    {
      Data[8 * xiX + xiY] = xiType;
      mBitmapCache = null;
    }

    private static Color[] mPalette = makePalette();

    private static Color[] makePalette()
    {
      Color[] acc = new Color[64];

      acc[0] = Color.FromArgb(255, 0, 0);
      acc[1] = Color.FromArgb(255, 64, 0);
      acc[2] = Color.FromArgb(255, 128, 0);
      acc[3] = Color.FromArgb(255, 192, 0);
      acc[4] = Color.FromArgb(255, 255, 0);
      acc[5] = Color.FromArgb(192, 255, 64);
      acc[6] = Color.FromArgb(128, 255, 128);
      acc[7] = Color.FromArgb(64, 255, 192);
      acc[8] = Color.FromArgb(0, 255, 255);
      acc[9] = Color.FromArgb(0, 192, 255);
      acc[10] = Color.FromArgb(0, 128, 255);
      acc[11] = Color.FromArgb(0, 64, 255);
      acc[12] = Color.FromArgb(0, 0, 255);
      acc[13] = Color.FromArgb(64, 0, 192);
      acc[14] = Color.FromArgb(128, 0, 128);
      acc[15] = Color.FromArgb(192, 0, 64);

      acc[16] = Color.FromArgb(255, 255, 255);
      acc[17] = Color.FromArgb(224, 224, 224);
      acc[18] = Color.FromArgb(192, 192, 192);
      acc[19] = Color.FromArgb(160, 160, 160);
      acc[20] = Color.FromArgb(128, 128, 128);

      for (int i = 21; i < acc.Length; i++)
      {
        acc[i] = Color.FromArgb(0, 0, 0);
      }
      return acc;
    }

    public const int SCALE = 16;

    #endregion

    private static OddTypeInfo[] mOddTypes = MakeOddTypes();

    private static OddTypeInfo[] MakeOddTypes()
    {
      mOddTypes = new OddTypeInfo[64];
      for (int i = 0; i < 64; i++)
      {
        mOddTypes[i] = new OddTypeInfo(i.ToString(), mPalette[i], i.ToString());
      }
      return mOddTypes;
    }
    
    public const int HIGHEST_KNOWN_Odd_TYPE = 45;

    public class OddTypeInfo
    {
      public Color Color;
      public string Name;
      public string Description;

      public OddTypeInfo(string xiName, Color xiColor, string xiDescription)
      {
        Name = xiName;
        Color = xiColor;
        Description = xiDescription;
      }

      public OddTypeInfo(string xiName, int xiColor)
        : this(xiName, xiColor, null) { }

      public OddTypeInfo(string xiName, int xiColor, string xiDescription)
        : this(xiName, Color.FromArgb(xiColor | ~0x00ffffff), xiDescription) { }
    }

  }
}
