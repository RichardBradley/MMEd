using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using NUnit.Framework;
using MMEd.Util;

namespace MMEd.Tests
{
  [TestFixture]
  public class UtilTests
  {
    [Test]
    public void TestEscape()
    {
      Assert.AreEqual("asdf\\0as\\u62378df", Utils.EscapeString("asdf\0as\u62378df"));
    }

    [Test]
    public void TestPS16bitColors()
    {
      Random rnd = new Random();
      for (int i = 0; i < 10; i++)
      {
        int rndRGB = (rnd.Next() & 0xf8f8f8) | unchecked((int)0xff000000);
        short there = Utils.ARGBColorToPS16bit(rndRGB);
        int back = Utils.PS16bitColorToARGB(there);
        Assert.AreEqual(string.Format("{0:x}", rndRGB), string.Format("{0:x}", back));
        Assert.AreEqual(rndRGB, back);
      }
      short[] specificTests = { unchecked((short)0x8000) };
      for (int i = 0; i < 10 + specificTests.Length; i++)
      {
        short rndPScol = i < specificTests.Length
            ? specificTests[i]
            : (short)(rnd.Next() & 0x7fff);
        int there = Utils.PS16bitColorToARGB(rndPScol);
        short back = Utils.ARGBColorToPS16bit(there);
        Assert.AreEqual(rndPScol, back);
      }
    }

    [Test]
    public void TestPS32bitColors()
    {
      Random rnd = new Random();
      for (int i = 0; i < 10; i++)
      {
        int rndRGB = rnd.Next(int.MinValue, int.MaxValue);
        Color there = Utils.PSRGBColorToColor(rndRGB);
        int back = Utils.ColorToPSRGBColor(there);
        Assert.AreEqual(string.Format("{0:x}", rndRGB), string.Format("{0:x}", back));
        Assert.AreEqual(rndRGB, back);
      }
      Color[] specificTests = { Color.FromArgb(unchecked((int)0xffffffff)) };
      for (int i = 0; i < 10 + specificTests.Length; i++)
      {
        Color rndcol = i < specificTests.Length
            ? specificTests[i]
            : Color.FromArgb(rnd.Next());
        int there = Utils.ColorToPSRGBColor(rndcol);
        Color back = Utils.PSRGBColorToColor(there);
        Assert.AreEqual(rndcol, back);
      }
    }

    [Test]
    public void TestByteArrayComparer()
    {
      string[] ss = new string[] { "", "asd", "ass", "qwer", "qwe" };
      StringComparer sComp = StringComparer.InvariantCulture;
      ByteArrayComparer bComp = new ByteArrayComparer();
      for (int i=0; i<ss.Length; i++)
        for (int j = 0; j < ss.Length; j++)
        {
          string sa = ss[i];
          string sb = ss[j];
          byte[] ba = Encoding.ASCII.GetBytes(sa);
          byte[] bb = Encoding.ASCII.GetBytes(sb);
          Assert.AreEqual(
            sComp.Compare(sa, sb),
            bComp.Compare(ba, bb));
        }
    }

    [Test]
    public void TestEquivalenceCollection()
    {
      EquivalenceCollection<int, int> lColl = new EquivalenceCollection<int, int>();
      int[] lCount = new int[10];
      Random rnd = new Random();
      for (int i = 0; i < 50; i++)
      {
        int v = rnd.Next(10);
        lCount[v]++;
        lColl.Add(i, v);
      }
      int[] lCount2 = new int[lCount.Length];
      foreach (List<int> lList in lColl)
      {
        int i = lList[0];
        Assert.AreEqual(0, lCount2[i]);
        lCount2[i] = lList.Count;
        Assert.AreEqual(lCount[i], lCount2[i]);
      }
      for (int i = 0; i < lCount.Length; i++)
      {
        Assert.AreEqual(lCount[i], lCount2[i]);
      }
    }
  }
}
