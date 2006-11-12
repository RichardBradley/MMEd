using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using NUnit.Framework;
using MMEd.Util;
using System.Xml.Serialization;

// Tests the behaviour of various system libraries, to ensure that
// assumptions are safe

namespace MMEd.Tests
{
  [TestFixture]
  public class Libraries
  {
    [Test]
    public void TestBinaryReader()
    {
      //check that the underlying stream can be modified, and
      //binary reader won't complain or cache any state
      //
      //two int32s, 10 then 5
      byte[] data = new byte[] { 10, 0, 0, 0, 5, 0, 0, 0 };
      MemoryStream str = new MemoryStream(data);
      BinaryReader bin = new BinaryReader(str);
      Assert.AreEqual(10, bin.ReadInt32());
      str.Seek(-4, SeekOrigin.Current);
      Assert.AreEqual(10, bin.ReadInt32());
      Assert.AreEqual(5, bin.ReadInt32());
    }

    [Test]
    public void TestShortMultiplication()
    {
      short s1 = 3;
      short s2 = 4;
      //short s3 = s1 * s2; //(cast error!)
      //huh? There's no short multiplication operator?
      // They're both widened to ints? How bizarre!
      short s3 = (short)(s1 * s2);

      //now, you might think that they're just protecting you
      //from overflows.
      //But explain this then:
      int i1 = 3;
      int i2 = 4;
      int i3 = i1 * i2;
      //why wasn't that multiplication widened to a long, then?
    }

    [Test]
    public void TestNonASCIIChars()
    {
      byte[] data = new byte[] { 0xf8, (byte)'a', 0, (byte)'b' };
      string s = Encoding.GetEncoding("windows-1252").GetString(data);
      byte[] data2 = Encoding.GetEncoding("windows-1252").GetBytes(s);
      Assert.AreEqual(data, data2);
      Assert.AreEqual(data[0], data2[0]);
    }

    [Test]
    public void TestXmlArraySerialisation()
    {
      ClassWithArrays o = new ClassWithArrays();
      o.twoDeeCStyle = new int[2][];
      for (int i = 0; i < o.twoDeeCStyle.Length; i++)
      {
        o.twoDeeCStyle[i] = new int[2];
      }
      //o.twoDeeVBStyle = new int[2,2];
      StringWriter sw = new StringWriter();
      XmlSerializer xs = new XmlSerializer(typeof(ClassWithArrays));
      xs.Serialize(sw, o);
      Console.Out.WriteLine(sw.ToString());
    }

    public class ClassWithArrays
    {
      public int[][] twoDeeCStyle;

      // not XML Serialisable:
      //public int[,] twoDeeVBStyle;
    }

    enum SparseEnum
    {
      one = 1,
      three = 3
    }

    [Test]
    public void TestEnum()
    {
      //I would have expected this to throw an exception.
      SparseEnum lVal = (SparseEnum)2;
      string s = Enum.GetName(typeof(SparseEnum), lVal);
      Assert.AreEqual(s, null);
    }

    [Test]
    [ExpectedException(typeof(AssertionException))]
    public void ArrayCompare()
    {
      //this assertion fails :-(
      Assert.IsTrue(new byte[] { 1, 2, 3 } == new byte[] { 1, 2, 3 });
    }

    [Test]
    public void TestSortingStuff()
    {
      byte[] a = new byte[] {1}, b = new byte[] {1}, c = new byte[] {2};
      SortedDictionary<byte[], byte[]> sDict = new SortedDictionary<byte[],byte[]>(new ByteArrayComparer());
      sDict[a] = a;
      sDict[b] = b; //overwrites [a], since compare(a,b) == 0
      sDict[c] = c;
      // == is a reference operator here (see ArrayCompare() above)
      Assert.IsTrue(sDict[a] == b);
      Assert.IsTrue(sDict[b] == b);
      Assert.IsTrue(sDict[c] == c);
      Assert.IsTrue(sDict[new byte[] { 1 }] == b);
    }
  }
}
