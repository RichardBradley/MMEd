using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Text;
using MMEd.Util;

namespace MMEd.Chunks
{
  public class CameraPosChunk : Chunk, Viewers.ImageViewer.IImageProvider, IReindexableChunk
  {
    int mIdx;

    [Description("Viewing direction: 0 is north, 1024 is east, etc.")]
    public short Direction
    {
      get
      {
        return mDirection;
      }
      set
      {
        mDirection = value;
        mBitmapCache = null;
      }
    }

    [Description("Camera distance from the car in a straight line along the xy plane")]
    public short Distance
    {
      get
      {
        return mDistance;
      }
      set
      {
        mDistance = value;
        mBitmapCache = null;
      }
    }

    [Description("Camera height, as a straight-line distance above the car")]
    public short Elevation
    {
      get
      {
        return mElevation;
      }
      set
      {
        mElevation = value;
        mBitmapCache = null;
      }
    }

    public CameraPosChunk() { }
    public CameraPosChunk(int idx, BinaryReader bin) 
    {
      mIdx = idx;
      Deserialise(bin);
    }

    public override void Deserialise(Stream inStr)
    {
      BinaryReader bin = new BinaryReader(inStr);
      Deserialise(bin);
    }

    public void Deserialise(BinaryReader bin)
    {
      Direction = bin.ReadInt16();
      Distance =  bin.ReadInt16();
      Elevation = bin.ReadInt16();
    }

    public override void Serialise(Stream outStr)
    {
      BinaryWriter bout = new BinaryWriter(outStr);
      Serialise(bout);
    }

    public void Serialise(BinaryWriter bout)
    {
      bout.Write(Direction);
      bout.Write(Distance);
      bout.Write(Elevation);
    }

    public override string Name
    {
      get
      {
        return string.Format("[{0}] CameraPos", mIdx);
      }
    }

    public int Id
    {
      get
      {
        return mIdx;
      }
    }

    public Image ToImage()
    {
      return ToBitmap();
    }

    public Bitmap ToBitmap()
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
      Draw(g, new Pen(Color.Black, 2), new Point(4 * SCALE, 4 * SCALE), 8 * SCALE);
      return lBmp;
    }

    public void Draw(Graphics g, Pen p, Point xiMidpoint, int xiMaxDistance)
    {
      // Display the elevation as a circle, where a larger circle indicates a
      // higher elevation. Choose 1000 as a arbitrary cut-off maximum.
      // For view mode it would be nice to expand the bitmap and allow the
      // camera lines to use more space and overlap each other. But that 
      // would make an edit mode unusable.
      int lElevation = Math.Min((int)Elevation, 1000);
      int lCircleRadius = (lElevation * xiMaxDistance) / (1000 * 2);
      Utils.DrawCircle(g, p, xiMidpoint, lCircleRadius);

      // Draw a line to show direction and distance. Add an arrowhead
      // to make it look prettier and to keep direction clear when
      // distance is zero.
      int lDistance = Math.Min((int)Distance, 1000);
      int lLineLength = (lDistance * xiMaxDistance) / (1000 * 2);

      Utils.DrawArrow(g, p, xiMidpoint, Direction, lLineLength, false);
    }

    short mDirection;
    short mDistance;
    short mElevation;
    Bitmap mBitmapCache;
    public const int SCALE = 16;

    #region IReindexableChunk Members

    ///========================================================================
    /// Property : Data
    /// 
    /// <summary>
    /// 	Represent the camera position data as byte array
    /// </summary>
    ///========================================================================
    public byte[] Data
    {
      get
      {
        byte[] lRet = new byte[6];
        BitConverter.GetBytes(Direction).CopyTo(lRet, 0);
        BitConverter.GetBytes(Distance).CopyTo(lRet, 2);
        BitConverter.GetBytes(Elevation).CopyTo(lRet, 4);
        return lRet;
      }
      set
      {
        Direction = BitConverter.ToInt16(value, 0);
        Distance = BitConverter.ToInt16(value, 2);
        Elevation = BitConverter.ToInt16(value, 4);
      }
    }

    ///========================================================================
    /// Method : Clear
    /// 
    /// <summary>
    /// 	Reset the camera position data
    /// </summary>
    ///========================================================================
    public void Clear()
    {
      mDirection = 0;
      mDistance = 0;
      mElevation = 0;
    }

    #endregion
  }
}