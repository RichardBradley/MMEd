using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.ComponentModel;
using MMEd.Util;
using System.Drawing;

// Represents an AI steering / respawn direction map.

namespace MMEd.Chunks
{
  public class SteeringImageChunk : Chunk, Viewers.ImageViewer.IImageProvider, IReindexableChunk
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
        mBitmapCache = new Bitmap[5];
      }
    }

    //set the steering image to be all zero
    public void Clear()
    {
      Array.Clear(Data, 0, Data.Length);
      mBitmapCache = new Bitmap[5];
    }

    public SteeringImageChunk() { }

    public SteeringImageChunk(int idx)
    {
      mIdx = idx;
      Data = new byte[64];
    }

    public SteeringImageChunk(int idx, byte xiValue)
    {
      mIdx = idx;
      Data = new byte[64];
      FlushToValue(xiValue);
    }

    public SteeringImageChunk(int idx, Stream inStr)
    {
      mIdx = idx;
      Deserialise(inStr);
    }

    public override string Name
    {
      get
      {
        return string.Format("[{0}] Steering Image", mIdx);
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

    public void CopyFrom(SteeringImageChunk xiFrom)
    {
      Array.Copy(xiFrom.Data, this.Data, Data.Length);
    }

    public override List<string> GetDifferences(Chunk xiChunk)
    {
      if (ByteArrayComparer.CompareStatic(mData, ((SteeringImageChunk)xiChunk).mData) != 0)
      {
        List<string> lRet = base.GetDifferences(xiChunk);
        lRet.Add("Changed steering image #" + mIdx.ToString());
        return lRet;
      }

      return base.GetDifferences(xiChunk);
    }

    #region IImageProvider Members

    Bitmap[] mBitmapCache = new Bitmap[5];
    Pen mActivePen = Pens.Red;

    public Image ToImage()
    {
      return ToImage(0, mActivePen);
    }

    public Image ToImage(byte xiOrientation)
    {
      return ToImage(xiOrientation, mActivePen);
    }

    ///========================================================================
    /// Method : ToImage
    /// 
    /// <summary>
    /// 	Get an image of the steering map, rotated appropriately.
    /// </summary>
    /// <param name="xiOrientation"></param>
    /// <returns></returns>
    ///========================================================================
    public Image ToImage(byte xiOrientation, Pen xiPen)
    {
      //=======================================================================
      // If an out-of-range orientation is encountered, treat as zero. There
      // is no evidence for this being correct.
      //=======================================================================
      if (xiOrientation < 0 || xiOrientation > 4)
      {
        xiOrientation = 0;
      }

      if (xiPen.Color != mActivePen.Color)
      {
        mBitmapCache = new Bitmap[5];
        mActivePen = xiPen;
      }

      if (mBitmapCache[xiOrientation] == null)
      {
        mBitmapCache[xiOrientation] = ToBitmapUncached(xiOrientation);
      }

      return mBitmapCache[xiOrientation];
    }

    public Bitmap ToBitmapUncached(byte xiOrientation)
    {
      Bitmap lBmp = new Bitmap(8 * SCALE, 8 * SCALE);
      Graphics g = Graphics.FromImage(lBmp);
      bool[,] lDrawn = new bool[8,8];

      for (int x = 0; x < 8; x++)
      {
        for (int y = 0; y < 8; y++)
        {
          if (lDrawn[x,y])
          {
            continue;
          }

          int lDirection = GetDirection(xiOrientation, x, y);
          int lWidth = 1;
          int lHeight = 1;
          int lFarX = x;
          int lFarY = y;

          //===================================================================
          // Try to find as many pixels to the right and below this one with 
          // the same direction as possible, so we can combine them together
          // into one arrow on the image.
          //===================================================================
          while (lFarX < 7 || lFarY < 7)
          {
            bool lCanExtendRight = lFarX < 7;
            for (int lTestY = y; lCanExtendRight && lTestY <= lFarY; lTestY++)
            {
              if (GetDirection(xiOrientation, lFarX + 1, lTestY) != lDirection)
              {
                lCanExtendRight = false;
              }
            }

            if (lCanExtendRight)
            {
              lWidth++;
              lFarX += 1;
            }

            bool lCanExtendDown = lFarY < 7;
            for (int lTestX = x; lCanExtendDown && lTestX <= lFarX; lTestX++)
            {
              if (GetDirection(xiOrientation, lTestX, lFarY + 1) != lDirection)
              {
                lCanExtendDown = false;
              }
            }

            if (lCanExtendDown)
            {
              lHeight++;
              lFarY += 1;
            }

            if (!lCanExtendRight && !lCanExtendDown)
            {
              break;
            }
            else if (Math.Abs((lFarX - x) - (lFarY - y)) > 1)
            {
              break; // Don't make a shape that's too long and thin
            }
          }

          //===================================================================
          // Mark all the squares we're drawing in one go as having been drawn,
          // so we don't try and draw them again.
          //===================================================================
          for (int lDrawnX = x; lDrawnX <= lFarX; lDrawnX++)
          {
            for (int lDrawnY = y; lDrawnY <= lFarY; lDrawnY++)
            {
              lDrawn[lDrawnX,lDrawnY] = true;
            }
          }

          //===================================================================
          // Draw the arrow
          //===================================================================
          double lMidX = x + (lFarX - x) / 2.0;
          double lMidY = y + (lFarY - y) / 2.0;
          double lSize = Math.Sqrt(((1 + lFarX - x) ^ 2) + ((1 + lFarY - y) ^ 2));

          Pen lWidePen = (Pen)mActivePen.Clone();
          lWidePen.Width = (float)lSize * 0.75f;
          Utils.DrawArrow(g, lWidePen, new Point((int)((lMidX * SCALE) + (SCALE / 2.0)), (int)((lMidY * SCALE) + (SCALE / 2.0))), lDirection, (int)(lSize * SCALE), (int)Math.Ceiling(lWidePen.Width), true);

          //===================================================================
          // Uncomment this section to debug
          //===================================================================
          // lWidePen.Width = 2;
          // g.DrawRectangle(lWidePen, SCALE * x, SCALE * y, SCALE * (1 + lFarX - x), SCALE * (1 + lFarY - y));
        }
      }
      return lBmp;
    }

    ///========================================================================
    /// Method : GetDirection
    /// 
    /// <summary>
    /// 	Get the direction (as an angle 0...4095) of a pixel, given the
    ///   orientation of the square.
    /// </summary>
    /// <param name="xiOrientation"></param>
    /// <param name="xiX"></param>
    /// <param name="xiY"></param>
    /// <returns></returns>
    ///========================================================================
    private int GetDirection(byte xiOrientation, int xiX, int xiY)
    {
      return xiOrientation == 0 ?
        Data[8 * xiX + xiY] * 256 :
        (xiOrientation * 1024) + (Data[8 * xiX + xiY] * -256);
    }

    public void FlushToValue(byte xiNewValue)
    {
      for (int i = 0; i < Data.Length; i++)
      {
        Data[i] = xiNewValue;
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
      mBitmapCache = new Bitmap[5];
    }

    public const int SCALE = 16;

    #endregion

    #region General information about steering

    public static string GetDirectionName(byte xiSteeringDirection)
    {
      return sDirectionNames[xiSteeringDirection];
    }

    private static string[] sDirectionNames = new string[] {
      "N", // 0
      "NNE",
      "NE",
      "ENE",
      "E", // 4
      "ESE",
      "SE",
      "SSE",
      "S", // 8
      "SSW",
      "SW",
      "WSW",
      "W", // 12
      "WNW",
      "NW",
      "NNW"
    };

    #endregion

  }
}
