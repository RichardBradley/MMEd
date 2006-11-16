using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

// represents an entire MMv3 file (not necessarily a level)
//
// deserialise will scan through the file, searching for known datastructures

namespace MMEd.Chunks
{
  public class FileChunk : Chunk
  {
    public FileChunk() { }
    public FileChunk(Stream inStr) { Deserialise(inStr); }

    public override void Deserialise(Stream inStr)
    {
      if (inStr is FileStream)
      {
        mName = ((FileStream)inStr).Name;
      }

      List<Chunk> lChildren = new List<Chunk>();
      MemoryStream lUnparseableBuff = new MemoryStream();
      long lUnparseableBuffStart = 0;
      int lNextRead = inStr.ReadByte();

      //what are bytes 1-8 of the two structs?
      byte[] lTIMStart4bpp = new byte[] { 0, 0, 0, 8, 0, 0, 0 };
      byte[] lTIMStart8bpp = new byte[] { 0, 0, 0, 9, 0, 0, 0 };
      byte[] lTMDStart = new byte[] { 0, 0, 0, 0, 0, 0, 0 };
      byte[] lSevenBuff = new byte[7];

      while (lNextRead != -1)
      {
        byte lNextByte = (byte)lNextRead;
        bool lByteParsed = false;

        // might it be a TIM?
        if (lNextByte == (byte)TIMChunk.TIM_MAGIC_NUMBER)
        {
          long lPos = inStr.Position;
          try
          {
            inStr.Read(lSevenBuff, 0, 7);
            if (SixOfSevenAreZero(lSevenBuff))
            {
              inStr.Seek(-8, SeekOrigin.Current);
              TIMChunk lTIM = new TIMChunk(inStr, string.Format("TIM at {0}", inStr.Position));
              if (lUnparseableBuff.Length != 0)
              {
                lChildren.Add(new RawDataChunk(
                  string.Format("data at {0}", lUnparseableBuffStart), 
                  lUnparseableBuff.ToArray()));
                lUnparseableBuff.SetLength(0);
                lUnparseableBuffStart = inStr.Position;
              }
              lChildren.Add(lTIM);
              lByteParsed = true;
            }
            else
            {
              inStr.Seek(-7, SeekOrigin.Current);
            }
          }
          catch
          {
            inStr.Seek(lPos, SeekOrigin.Begin);
          }
        }
        else if (lNextByte == (byte)TMDChunk.TMD_MAGIC_NUMBER) // maybe a TMD?
        {
          long lPos = inStr.Position;
          try
          {
            inStr.Read(lSevenBuff, 0, 7);
            if (CompareSevenBytes(lSevenBuff, lTMDStart))
            {
              inStr.Seek(-8, SeekOrigin.Current);
              TMDChunk lTMD = new TMDChunk(inStr, string.Format("TMD at {0}", inStr.Position));
              if (lUnparseableBuff.Length != 0)
              {
                lChildren.Add(new RawDataChunk(
                  string.Format("data at {0}", lUnparseableBuffStart),
                  lUnparseableBuff.ToArray()));
                lUnparseableBuff.SetLength(0);
                lUnparseableBuffStart = inStr.Position;
              }
              lChildren.Add(lTMD);
              lByteParsed = true;
            }
            else
            {
              inStr.Seek(-7, SeekOrigin.Current);
            }
          }
          catch (Exception)
          {
            inStr.Seek(lPos, SeekOrigin.Begin);
          }
        }

        if (!lByteParsed) // dunno what this is
        {
          lUnparseableBuff.WriteByte(lNextByte);
        }

        lNextRead = inStr.ReadByte();
      }

      // add any left over bytes as a child
      if (lUnparseableBuff.Length != 0)
      {
        lChildren.Add(new RawDataChunk("data", lUnparseableBuff.ToArray()));
      }

      // finish
      mChildren = lChildren.ToArray();
    }

    public override void Serialise(System.IO.Stream outStr)
    {
      //qq use for not foreach to get idx
      //foreach (Chunk lChunk in mChildren)
      for (int i=0; i<mChildren.Length; i++)
      {
        Chunk lChunk = mChildren[i];
        lChunk.Serialise(outStr);
      }
    }

    public override void ReplaceChild(Chunk xiFrom, Chunk xiTo)
    {
      for (int i = 0; i < mChildren.Length; i++)
        if (xiFrom == mChildren[i])
        {
          mChildren[i] = xiTo;
          return;
        }
      throw new ArgumentException("xifrom not found!");
    }

    private Chunk[] mChildren = new Chunk[0];
    public override Chunk[] GetChildren()
    {
      Chunk[] lTmp = (Chunk[])mChildren.Clone();
      Array.Sort(lTmp, new TypeComparer());
      return lTmp;
    }

    private class TypeComparer : System.Collections.IComparer
    {
      public int Compare(object a, object b)
      {
        if (a == null || b == null)
        {
          return a == null ? (b == null ? 0 : -1) : 1;
        }
        else
        {
          return string.Compare(a.GetType().Name, b.GetType().Name);
        }
      }
    }

    private string mName = "FileChunk";
    public override string Name
    {
      get { return mName; }
    }

    private bool CompareSevenBytes(byte[] a, byte[] b)
    {
      return a[0] == b[0]
      && a[1] == b[1]
      && a[2] == b[2]
      && a[3] == b[3]
      && a[4] == b[4]
      && a[5] == b[5]
      && a[6] == b[6];
    }

    private bool SixOfSevenAreZero(byte[] a)
    {
      return a[0] == 0
      && a[1] == 0
      && a[2] == 0
      && a[4] == 0
      && a[5] == 0
      && a[6] == 0;
    }
  }
}
