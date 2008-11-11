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
  public class BumpImageChunk : Chunk, Viewers.ImageViewer.IImageProvider, IReindexableChunk
  {
    public BumpImageChunk() { }

    public BumpImageChunk(int idx) 
    {
      mIdx = idx;
      Data = new byte[64];
    }

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
      for (int x = 0; x < 8; x++)
        for (int y = 0; y < 8; y++) 
        {
           //(note x-y swap)
           Brush b = new SolidBrush(GetColorForBumpType(Data[8 * x + y]));
           g.FillRectangle(b, x*SCALE, y*SCALE, SCALE, SCALE);
        }
      return lBmp;
    }

    public static Color GetColorForBumpType(byte xiVal)
    {
      if (xiVal > mBumpTypes.Length || mBumpTypes[xiVal] == null)
      {
        return Color.Magenta;
      }
      else
      {
        return mBumpTypes[(int)xiVal].Color;
      }
    }

    public static BumpTypeInfo GetBumpTypeInfo(byte xiVal)
    {
      if (xiVal > mBumpTypes.Length)
      {
        return null;
      }
      else
      {
        return mBumpTypes[(int)xiVal];
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

    private static BumpTypeInfo[] mBumpTypes = MakeBumpTypes();

    private static BumpTypeInfo[] MakeBumpTypes()
    {
      mBumpTypes = new BumpTypeInfo[64];
      mBumpTypes[0x00] = new BumpTypeInfo("plain", 0x000000);
      mBumpTypes[0x01] = new BumpTypeInfo("wall", 0xff00ff, "Impassable wall");
      mBumpTypes[0x02] = new BumpTypeInfo("milk", 0xf8e8d8);
      mBumpTypes[0x03] = new BumpTypeInfo("syrup", 0xa87020);
      mBumpTypes[0x04] = new BumpTypeInfo("ketchup", 0xff0000);
      mBumpTypes[0x05] = new BumpTypeInfo("roadBorder", 0x89cba0);
      mBumpTypes[0x06] = new BumpTypeInfo("roadBorder2", 0x89cbbf);
      mBumpTypes[0x07] = new BumpTypeInfo("water", 0x4551ec);
      mBumpTypes[0x08] = new BumpTypeInfo("hole", 0xffffff, "A void or hole in the surface");
      mBumpTypes[0x09] = new BumpTypeInfo("lilyPad", 0x33ff33, "NB This doesn't do much on a random pool course");
      mBumpTypes[0x0A] = new BumpTypeInfo("grassStalk", 0x00ff00);
      mBumpTypes[0x0B] = new BumpTypeInfo("jumpWoosh3", 0xffd400, "A super speedup if you're going fast enough (found on the jump between tables in Rack)");
      mBumpTypes[0x0C] = new BumpTypeInfo("unknown0C", 0x606060, "This is present in Rack & Roll, but driving over it has no discernable effect");
      mBumpTypes[0x0D] = new BumpTypeInfo("roadBorderPool", 0x89cbde, "Edge of the track in a pool table course");
      mBumpTypes[0x0E] = new BumpTypeInfo("poolPocket", 0xeeeeee, "A pool pocket - death trap");
      mBumpTypes[0x0F] = new BumpTypeInfo("lowSurvivalHeight", 0x00dddd, "Driving off the edge of this means you die even if you don't fall very far. (Also without it,the edge of the Rack & Roll table sometimes appears over the car rather than under it)");
      mBumpTypes[0x10] = new BumpTypeInfo("swapVehicle", 0x229022, "Toggle between player 1 and 2 cars");
      mBumpTypes[0x11] = new BumpTypeInfo("clearGoo", 0xffeedd);
      mBumpTypes[0x12] = new BumpTypeInfo("powderSpill", 0x3333ff);
      mBumpTypes[0x13] = new BumpTypeInfo("greenGoo", 0x33ff33);
      mBumpTypes[0x14] = new BumpTypeInfo("redGoo", 0xff3333, "Makes you invisible");
      mBumpTypes[0x15] = new BumpTypeInfo("roadBorderLab", 0x89cbfd);
      mBumpTypes[0x16] = new BumpTypeInfo("jumpWhoosh", 0xffe400);
      mBumpTypes[0x17] = new BumpTypeInfo("unknown17", 0x707070, "In Rack & Roll, this has no discernable effect");
      mBumpTypes[0x18] = new BumpTypeInfo("unknown18", 0x787878, "In Rack & Roll, this has no discernable effect");
      mBumpTypes[0x19] = new BumpTypeInfo("enterTestTube", 0xff9900);
      mBumpTypes[0x1a] = new BumpTypeInfo("unknown1a", 0x808080, "In Rack & Roll, this has no discernable effect");
      mBumpTypes[0x1b] = new BumpTypeInfo("unknown1b", 0x888888, "In Rack & Roll, this has no discernable effect");
      mBumpTypes[0x1C] = new BumpTypeInfo("microGooEdge", 0xaaaa00);
      mBumpTypes[0x1D] = new BumpTypeInfo("microGooMiddle", 0x888800);
      mBumpTypes[0x1E] = new BumpTypeInfo("exitMicroscope", 0xff7700, "Ascend vertically; screen blanks; reappear, rather smaller, at world coord approx (3000, 8300)");
      mBumpTypes[0x1F] = new BumpTypeInfo("unknown1f", 0x909090, "Some sort of teleport with funny screen movements, but a bit hard to tell what");
      mBumpTypes[0x20] = new BumpTypeInfo("jumpWhoosh2", 0xfff400, "Allegedly like jumpWhoosh. But appears to have a one-way property in some cases, and energetic attempts to violate this result in a \"splash\"");
      mBumpTypes[0x21] = new BumpTypeInfo("enterTeleport", 0xff8800, "Screen jumps to somewhere over y=10000; some funny stuff with camera angles happens; cars suddenly reappear at world coord approx (920, 1600)");
      mBumpTypes[0x22] = new BumpTypeInfo("lilyPadEdge", 0x66FF66, "Land/water boundary, can be jumped");
      mBumpTypes[0x23] = new BumpTypeInfo("sand", 0xc9b549);
      mBumpTypes[0x24] = new BumpTypeInfo("jumpWheee", 0xffc400, "You jump a bit and go 'Wheee!'");
      mBumpTypes[0x25] = new BumpTypeInfo("teleport6", 0xffff00, "Teleport to world coord approx (7060, 11450) facing towards (0?, 0?)");
      mBumpTypes[0x26] = new BumpTypeInfo("jumpWheee2", 0xffb400, "You jump a bit and go 'Wheee!'");
      mBumpTypes[0x27] = new BumpTypeInfo("splash2", 0x4455ff, "A big \"splash\" - death trap");
      mBumpTypes[0x28] = new BumpTypeInfo("teleport1", 0xffaa00, "Teleport to world coord approx (175, 7000) facing towards (10000, 0)");
      mBumpTypes[0x29] = new BumpTypeInfo("teleport2", 0xffdd00, "Teleport to world coord approx (115, 2645) facing towards (10000, 0)");
      mBumpTypes[0x2A] = new BumpTypeInfo("teleport3", 0xffbb00, "Teleport to world coord approx (5100, 11000) facing towards (0, 0)");
      mBumpTypes[0x2B] = new BumpTypeInfo("teleport4", 0xffee00, "Teleport to world coord approx (2550, 7000) facing towards (0, 0)");
      mBumpTypes[0x2C] = new BumpTypeInfo("teleport5", 0xffcc00, "Teleport to world coord approx (2640, 2700) facing towards (10000, 0)");
      mBumpTypes[0x2D] = new BumpTypeInfo("splash", 0x4444ff, "A \"splash\" - death trap");
      return mBumpTypes;
    }

    // this wasn't necessary under the enum scheme, but I think it's
    // preferable to having the list of types twice...
    public const int HIGHEST_KNOWN_BUMP_TYPE = 45;

    public class BumpTypeInfo
    {
      public Color Color;
      public string Name;
      public string Description;
      
      public BumpTypeInfo(string xiName, Color xiColor, string xiDescription)
      {
        Name = xiName;
        Color = xiColor;
        Description = xiDescription;
      }

      public BumpTypeInfo(string xiName, int xiColor)
        : this(xiName, xiColor, null) { }

      public BumpTypeInfo(string xiName, int xiColor, string xiDescription)
        : this(xiName, Color.FromArgb(xiColor | ~0x00ffffff), xiDescription) { }
    }

    public override void ReplaceChild(Chunk xiFrom, Chunk xiTo)
    {
      throw new Exception("The method or operation is not implemented.");
    }

    public void CopyFrom(BumpImageChunk xiFrom)
    {
      Array.Copy(xiFrom.Data, this.Data, Data.Length);
    }

    //set the bump to be all zero
    public void Clear()
    {
      Array.Clear(mData, 0, mData.Length);
      mBitmapCache = null;
    }

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

    public override List<string> GetDifferences(Chunk xiChunk)
    {
      if (ByteArrayComparer.CompareStatic(mData, ((BumpImageChunk)xiChunk).mData) != 0)
      {
        List<string> lRet = base.GetDifferences(xiChunk);
        lRet.Add("Changed bump #" + mIdx.ToString());
        return lRet;
      }

      return base.GetDifferences(xiChunk);
    }

    int mIdx;
    private byte[] mData;
    public const int SCALE = 16;
  }
}
