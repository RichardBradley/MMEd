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
  }
}
