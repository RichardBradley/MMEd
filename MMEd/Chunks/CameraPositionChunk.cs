using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Text;
using MMEd.Util;

namespace MMEd.Chunks
{
  public class CameraPosChunk : Chunk, Viewers.ImageViewer.IImageProvider
  {
    int mIdx;

    [Description("Viewing direction: 0 is north, 1024 is east, etc.")]
    public short Direction;

    [Description("Camera distance from the car in a straight line along the xy plane")]
    public short Distance;

    [Description("Camera height, as a straight-line distance above the car")]
    public short Elevation;

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
      Pen p = new Pen(Color.White, 2);
      Point lMidPoint = new Point(4 * SCALE, 4 * SCALE);
      Point lEndPoint = new Point(0, 0);//qqTLP

      int lMin = 0;
      int lMax = 8 * SCALE;
      int lMaxDistance = lMax - lMin;

      // Display the elevation as a circle, where a larger circle indicates a
      // higher elevation. Choose 1000 as a arbitrary cut-off maximum.
      // For view mode it would be nice to expand the bitmap and allow the
      // camera lines to use more space and overlap each other. But that 
      // would make an edit mode unusable.
      int lElevation = Math.Min((int)Elevation, 1000);
      int lCircleRadius = (lElevation * lMaxDistance) / (1000 * 2);
      g.DrawEllipse(
        p,
        lMidPoint.X - lCircleRadius,
        lMidPoint.Y - lCircleRadius,
        lCircleRadius * 2,
        lCircleRadius * 2);

      // Draw a line to show direction and distance. Add an arrowhead
      // to make it look prettier and to keep direction clear when
      // distance is zero.
      int lDistance = Math.Min((int)Distance, 1000);
      int lLineLength = (lDistance * lMaxDistance) / (1000 * 2);
      int lArrowHeadLength = SCALE;

      double lAngle = Math.PI * ((double)Direction / 2048);
      Point lLineEnd = new Point(
        lMidPoint.X - (int)(lLineLength * Math.Sin(lAngle)),
        lMidPoint.Y + (int)(lLineLength * Math.Cos(lAngle)));

      double lArrowAngle1 = lAngle + (Math.PI / 6);
      double lArrowAngle2 = lAngle - (Math.PI / 6);
      Point lArrowHead1 = new Point(
        lMidPoint.X - (int)(lArrowHeadLength * Math.Sin(lArrowAngle1)),
        lMidPoint.Y + (int)(lArrowHeadLength * Math.Cos(lArrowAngle1)));
      Point lArrowHead2 = new Point(
        lMidPoint.X - (int)(lArrowHeadLength * Math.Sin(lArrowAngle2)),
        lMidPoint.Y + (int)(lArrowHeadLength * Math.Cos(lArrowAngle2)));

      g.DrawLine(p, lMidPoint, lLineEnd);
      g.DrawLine(p, lMidPoint, lArrowHead1);
      g.DrawLine(p, lMidPoint, lArrowHead2);

      return lBmp;
    }

//    private int 
    Bitmap mBitmapCache;
    public const int SCALE = 16;
  }
}