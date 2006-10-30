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
            _4BPP   = 0x00000008,
            _4BPPNC = 0x00000000, // No CLUT
            _8BPP   = 0x00000009,
            _8BPPNC = 0x00000001, // No CLUT
            _16BPP  = 0x00000002
        }

        public const int TIM_MAGIC_NUMBER = 0x10;

        #region public fields

        [Description(@"Bits per pixel. Only 4bpp supported in this implementation. 
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
        public int[] Palette;

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

        public void Deserialise(System.IO.Stream inStr, int xiExpectedDataSize)
        {
            long lStartOffset = xiExpectedDataSize >= 0 ? inStr.Position : -1;

            // magic number
            BinaryReader bin = new BinaryReader(inStr);
            int lTIMMagicNumber = bin.ReadInt32();
            if (TIM_MAGIC_NUMBER != lTIMMagicNumber)
            {
                throw new DeserialisationException(string.Format("TIM should start with 4 byte magic number 0x10, found {0:x8}", lTIMMagicNumber), inStr.Position-4);
            }

            // bits per pixel
            int lBpp = bin.ReadInt32();
            if (lBpp != (int)TimBPP._4BPP)
            {
                throw new TIMTypeNotImplementedException(string.Format("Only 4BPP (0x8) TIMs are supported. Found {0:x8}", lBpp), inStr.Position-8);
            }
            BPP = (TimBPP)lBpp;

            // Now the main header:
            ClutSize    = bin.ReadInt32();
            PaletteOrgX = bin.ReadInt16(); 
            PaletteOrgY = bin.ReadInt16(); 
            ClutColors  = bin.ReadInt16();
            ClutCount   = bin.ReadInt16(); 

            //sanity checks
            if (ClutCount * ClutColors * 2 != ClutSize - 12)
                throw new DeserialisationException("bad TIM: ClutCount*ClutColors*2 != ClutSize - 12", inStr.Position);

            //TODO:
            if (ClutCount != 1)
                throw new DeserialisationException("Multi-clut TIMs are not yet supported. Please extend TIMChunk.cs using info from timgfx.txt");

            //CLUT:
            Palette = new int[ClutCount * ClutColors];
            for (int i = 0; i < Palette.Length; i++)
            {
                Palette[i] = Utils.PS16bitColorToARGB(bin.ReadInt16());
            }

            //Second header:
            ImageDataSize = bin.ReadInt32();
            ImageOrgX     = bin.ReadInt16(); 
            ImageOrgY     = bin.ReadInt16(); 
            //qq short * short is int !?!
            ImageWidth    = (short)(4 * bin.ReadInt16()); //qq may need changing when > 4bpp is supported
            ImageHeight   = bin.ReadInt16();

            //sanity checks
            //some observed TIMs fail this check:
            if (ImageWidth * ImageHeight / 2 != ImageDataSize - 12)
            {
                //throw new DeserialisationException
                System.Diagnostics.Trace.WriteLine(string.Format("bad TIM: ImageWidth * ImageHeight / 2 != ImageDataSize - 12, near {0}", inStr.Position));
            }

            //read the image data
            ImageData = bin.ReadBytes(ImageWidth * ImageHeight / 2);

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
                bout.Write((short)Utils.ARGBColorToPS16bit(Palette[i]));
            }

            //Second header:
            bout.Write(ImageDataSize);
            bout.Write(ImageOrgX);
            bout.Write(ImageOrgY);
            bout.Write((short)(ImageWidth / 4));
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
                // (use ImageData.Length + 12, not ImageDataSize, because of occasionally failed consistency checks)
                return 8 + ClutSize + ImageData.Length + 12 + (ZeroPadding == null ? 0 : ZeroPadding.Length);
            }
        }

        public override string Name
        {
            get { return 
                mIdx > 0 ?
                
                string.Format("[{0}] TIM", mIdx) : "TIM"; }
        }

        public Image ToImage()
        {
            return ToBitmap();
        }

       public Bitmap ToBitmap()
        {
            if (mBitmapCache != null &&
                mBitmapCache.IsAlive)
            {
                return (Bitmap)mBitmapCache.Target;
            }
            Bitmap acc = new Bitmap(ImageWidth, ImageHeight, PixelFormat.Format32bppArgb);
            for (int y = 0; y < ImageHeight; y++)
            {
                for (int x = 0; x < ImageWidth; x += 2)
                {
                    //qq 4bpp only!
                    byte v = ImageData[y * ImageWidth / 2 + x / 2];
                    Color left = Color.FromArgb(Palette[v & 0xf]);
                    Color right = Color.FromArgb((Palette[(v >> 4) & 0xf]));
                    acc.SetPixel(x, y, left);
                    acc.SetPixel(x + 1, y, right);
                }
            }
            mBitmapCache = new WeakReference(acc);
            return acc;
        }

        WeakReference mBitmapCache;
        int mIdx=-1;
        public TIMChunk() { }
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
    }
}
