using System;
using System.Collections.Generic;
using System.Text;

// compares byte arrays using lexical ordering on their elements

namespace MMEd.Util
{
  public class ByteArrayComparer : IComparer<byte[]>
  {
    public int Compare(byte[] a, byte[] b)
    {
      return CompareStatic(a, b);
    }

    // returns
    // -1 if a < b
    // 0  if a == b
    // +1 if a > b
    public static int CompareStatic(byte[] a, byte[] b)
    {
      if (a == null || b == null)
      {
        if (a == b)
        {
          return 0;
        }
        else
        {
          return a == null ? -1 : 1;
        }
      }
      int lCompareLength = Math.Min(a.Length, b.Length);
      for (int i = 0; i < lCompareLength; i++)
      {
        if (a[i] < b[i]) return -1;
        if (a[i] > b[i]) return 1;
      }
      if (a.Length < b.Length) return -1;
      if (a.Length > b.Length) return 1;
      return 0;
    }

    ///========================================================================
    ///  Static Method : Find
    /// 
    /// <summary>
    /// 	Returns the first position after xiOffset that the byte array 
    ///   xiSearch is found within the byte array xiSource.
    /// </summary>
    /// <param name="xiSource"></param>
    /// <param name="xiSearch"></param>
    /// <param name="xiOffset"></param>
    /// <returns>Offset within xiSource, or -1 for no match</returns>
    ///========================================================================
    public static int Find(byte[] xiSource, byte[] xiSearch, int xiOffset)
    {
      for (int ii = xiOffset; ii < xiSource.Length - xiSearch.Length; ii++)
      {
        bool lMatch = true;

        for (int jj = 0; jj < xiSearch.Length; jj++)
        {
          if (xiSource[ii + jj] != xiSearch[jj])
          {
            lMatch = false;
            break;
          }
        }

        if (lMatch)
        {
          return ii;
        }
      }

      return -1;
    }

  }
}
