using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using System.Text.RegularExpressions;
using System.Collections;
using NUnit.Framework;
using MMEd.Chunks;
using MMEd.Util;

namespace MMEd.Tests
{
  [TestFixture]
  public class Serialisation
  {
    private string GetBaseDir()
    {
      //are we on the Softwire network?
      //N.B. This test only really works if you're rtb
      return System.Environment.GetEnvironmentVariable("USERDOMAIN") == "ZOO"
        ? @"E:\MMs\mm3\public_html\MMs\MICRO"
        : @"D:\mm3\public_html\MMs\MICRO";
    }

    private string GetFullFileName(string xiShortName)
    {
      return GetBaseDir() +
          Regex.Replace(xiShortName, "([A-Z]+)([0-9].DAT)", "\\$1\\$1$2");
    }

    /*
    private string[] GetSampleLevelFiles()
    {
        string baseDir = GetBaseDir();

        if (false)
        {
            //qq just one level for now (TODO: make the tests below work for all levels)
            return new string[] { baseDir + @"\MICRO\BREAKY\BREAKY1.DAT" };
        }
        else
        {
            ArrayList acc = new ArrayList();
            foreach (string file in Directory.GetFiles(baseDir + @"\MICRO", "*.DAT", SearchOption.AllDirectories))
            {
                if (Regex.IsMatch(file, "[A-Z]+\\\\[A-Z]+[0-9]\\.DAT$"))
                    acc.Add(file);
            }
            return (string[])acc.ToArray(typeof(string));
        }
    }
    */

    //this is a weaker test than TestSerialisationIsInvertible
    //but has been left here as it's sometimes handy for debugging
    private void TestBinarySerialisationIsInvertible(string xiFilename)
    {
      Console.Out.WriteLine("Testing file {0}", xiFilename);
      using (Stream inStr = File.OpenRead(xiFilename))
      {
        Level deserLev = new Level();
        deserLev.Deserialise(inStr);
        MemoryStream memStr = new MemoryStream();
        inStr.Seek(0, SeekOrigin.Begin);
        DebugOutputStreamWithExpectations outStr = new DebugOutputStreamWithExpectations(inStr, memStr);
        deserLev.Serialise(outStr);
        inStr.Seek(0, SeekOrigin.Begin);
        memStr.Seek(0, SeekOrigin.Begin);
        Assert.IsTrue(StreamUtils.StreamsAreEqual(inStr, memStr));
      }
    }

    //this is a weaker test than TestSerialisationIsInvertible
    //but has been left here as it's sometimes handy for debugging
    private void TestBinaryUnkSerialisationIsInvertible(string xiFilename)
    {
      Console.Out.WriteLine("Testing file {0}", xiFilename);
      using (Stream inStr = File.OpenRead(xiFilename))
      {
        FileChunk deser = new FileChunk();
        deser.Deserialise(inStr);
        MemoryStream memStr = new MemoryStream();
        inStr.Seek(0, SeekOrigin.Begin);
        DebugOutputStreamWithExpectations outStr = new DebugOutputStreamWithExpectations(inStr, memStr);
        deser.Serialise(outStr);
        inStr.Seek(0, SeekOrigin.Begin);
        memStr.Seek(0, SeekOrigin.Begin);
        Assert.IsTrue(StreamUtils.StreamsAreEqual(inStr, memStr));
      }
    }

    //tests that both XML and Binary serialisation is invertible
    private void TestSerialisationIsInvertible(string xiFilename)
    {
      Console.Out.WriteLine("Testing file {0}", xiFilename);
      using (Stream inStr = File.OpenRead(xiFilename))
      {
        Level levelFromBinary = new Level();
        levelFromBinary.Deserialise(inStr);
        StringWriter sw = new StringWriter();
        XmlSerializer xs = new XmlSerializer(typeof(Level));
        xs.Serialize(sw, levelFromBinary);

        //now the XML version of the level is in sw
        Level levelFromXMLfromBinary = (Level)xs.Deserialize(new StringReader(sw.ToString()));

        //first check that the XML serialisation is the same
        StringWriter sw2 = new StringWriter();
        xs.Serialize(sw2, levelFromBinary);
        Assert.AreEqual(sw.ToString(), sw2.ToString());

        //now see if the XML-ified level will still write binary correctly:
        MemoryStream memStr = new MemoryStream();
        inStr.Seek(0, SeekOrigin.Begin);
        DebugOutputStreamWithExpectations outStr = new DebugOutputStreamWithExpectations(inStr, memStr);
        levelFromXMLfromBinary.Serialise(outStr);
        inStr.Seek(0, SeekOrigin.Begin);
        memStr.Seek(0, SeekOrigin.Begin);
        Assert.IsTrue(StreamUtils.StreamsAreEqual(inStr, memStr));
      }
    }

    //qq rtb tmp
    [Test]
    public void TestSerialisationCHOOSE()
    {
      //N.B. This test only really works if you're rtb
      string file = System.Environment.GetEnvironmentVariable("USERDOMAIN") == "ZOO"
        ? @"E:\MMs\mm3\public_html\MMs\FRONTEND\CHOOSE.DAT"
        : @"D:\mm3\binextract\MMV3\FRONTEND\CHOOSE.DAT";
      TestBinaryUnkSerialisationIsInvertible(file);
    }

    // There must be a way to dynamically generate tests for NUnit...
    #region tests

    [Test]
    public void TestSerialisationBEACH1()
    {
      TestSerialisationIsInvertible(GetFullFileName("BEACH1.DAT"));
    }

    [Test]
    public void TestSerialisationBEACH2()
    {
      TestSerialisationIsInvertible(GetFullFileName("BEACH2.DAT"));
    }

    [Test]
    public void TestSerialisationBEACH4()
    {
      TestSerialisationIsInvertible(GetFullFileName("BEACH4.DAT"));
    }

    [Test]
    public void TestSerialisationBEACH5()
    {
      TestSerialisationIsInvertible(GetFullFileName("BEACH5.DAT"));
    }

    [Test]
    public void TestSerialisationBEACH6()
    {
      TestSerialisationIsInvertible(GetFullFileName("BEACH6.DAT"));
    }

    [Test]
    public void TestSerialisationBEACH3()
    {
      TestSerialisationIsInvertible(GetFullFileName("BEACH3.DAT"));
    }

    [Test]
    public void TestSerialisationBREAKY1()
    {
      TestSerialisationIsInvertible(GetFullFileName("BREAKY1.DAT"));
    }

    [Test]
    public void TestSerialisationBREAKY2()
    {
      TestSerialisationIsInvertible(GetFullFileName("BREAKY2.DAT"));
    }

    [Test]
    public void TestSerialisationBREAKY3()
    {
      TestSerialisationIsInvertible(GetFullFileName("BREAKY3.DAT"));
    }

    [Test]
    public void TestSerialisationBREAKY4()
    {
      TestSerialisationIsInvertible(GetFullFileName("BREAKY4.DAT"));
    }

    [Test]
    public void TestSerialisationBREAKY5()
    {
      TestSerialisationIsInvertible(GetFullFileName("BREAKY5.DAT"));
    }

    [Test]
    public void TestSerialisationBREAKY6()
    {
      TestSerialisationIsInvertible(GetFullFileName("BREAKY6.DAT"));
    }

    [Test]
    public void TestSerialisationBREAKY7()
    {
      TestSerialisationIsInvertible(GetFullFileName("BREAKY7.DAT"));
    }

    [Test]
    public void TestSerialisationCHERRY1()
    {
      TestSerialisationIsInvertible(GetFullFileName("CHERRY1.DAT"));
    }

    [Test]
    public void TestSerialisationCHERRY2()
    {
      TestSerialisationIsInvertible(GetFullFileName("CHERRY2.DAT"));
    }

    [Test]
    public void TestSerialisationCHERRY3()
    {
      TestSerialisationIsInvertible(GetFullFileName("CHERRY3.DAT"));
    }

    [Test]
    public void TestSerialisationCHERRY4()
    {
      TestSerialisationIsInvertible(GetFullFileName("CHERRY4.DAT"));
    }

    [Test]
    public void TestSerialisationCHERRY5()
    {
      TestSerialisationIsInvertible(GetFullFileName("CHERRY5.DAT"));
    }

    [Test]
    public void TestSerialisationGARDEN1()
    {
      TestSerialisationIsInvertible(GetFullFileName("GARDEN1.DAT"));
    }

    [Test]
    public void TestSerialisationGARDEN2()
    {
      TestSerialisationIsInvertible(GetFullFileName("GARDEN2.DAT"));
    }

    [Test]
    public void TestSerialisationGARDEN3()
    {
      TestSerialisationIsInvertible(GetFullFileName("GARDEN3.DAT"));
    }

    [Test]
    public void TestSerialisationGARDEN4()
    {
      TestSerialisationIsInvertible(GetFullFileName("GARDEN4.DAT"));
    }

    [Test]
    public void TestSerialisationGARDEN5()
    {
      TestSerialisationIsInvertible(GetFullFileName("GARDEN5.DAT"));
    }

    [Test]
    public void TestSerialisationGARDEN6()
    {
      TestSerialisationIsInvertible(GetFullFileName("GARDEN6.DAT"));
    }

    [Test]
    public void TestSerialisationGARDEN7()
    {
      TestSerialisationIsInvertible(GetFullFileName("GARDEN7.DAT"));
    }

    [Test]
    public void TestSerialisationLAB1()
    {
      TestSerialisationIsInvertible(GetFullFileName("LAB1.DAT"));
    }

    [Test]
    public void TestSerialisationLAB2()
    {
      TestSerialisationIsInvertible(GetFullFileName("LAB2.DAT"));
    }

    [Test]
    public void TestSerialisationLAB3()
    {
      TestSerialisationIsInvertible(GetFullFileName("LAB3.DAT"));
    }

    [Test]
    public void TestSerialisationLAB4()
    {
      TestSerialisationIsInvertible(GetFullFileName("LAB4.DAT"));
    }

    [Test]
    public void TestSerialisationLAB5()
    {
      TestSerialisationIsInvertible(GetFullFileName("LAB5.DAT"));
    }

    [Test]
    public void TestSerialisationLAB6()
    {
      TestSerialisationIsInvertible(GetFullFileName("LAB6.DAT"));
    }

    [Test]
    public void TestSerialisationLAB7()
    {
      TestSerialisationIsInvertible(GetFullFileName("LAB7.DAT"));
    }

    [Test]
    public void TestSerialisationPOOL1()
    {
      TestSerialisationIsInvertible(GetFullFileName("POOL1.DAT"));
    }

    [Test]
    public void TestSerialisationPOOL2()
    {
      TestSerialisationIsInvertible(GetFullFileName("POOL2.DAT"));
    }

    [Test]
    public void TestSerialisationPOOL3()
    {
      TestSerialisationIsInvertible(GetFullFileName("POOL3.DAT"));
    }

    [Test]
    public void TestSerialisationPOOL4()
    {
      TestSerialisationIsInvertible(GetFullFileName("POOL4.DAT"));
    }

    [Test]
    public void TestSerialisationPOOL5()
    {
      TestSerialisationIsInvertible(GetFullFileName("POOL5.DAT"));
    }

    [Test]
    public void TestSerialisationREST1()
    {
      TestSerialisationIsInvertible(GetFullFileName("REST1.DAT"));
    }

    [Test]
    public void TestSerialisationREST2()
    {
      TestSerialisationIsInvertible(GetFullFileName("REST2.DAT"));
    }

    [Test]
    public void TestSerialisationREST3()
    {
      TestSerialisationIsInvertible(GetFullFileName("REST3.DAT"));
    }

    [Test]
    public void TestSerialisationREST4()
    {
      TestSerialisationIsInvertible(GetFullFileName("REST4.DAT"));
    }

    [Test]
    public void TestSerialisationREST5()
    {
      TestSerialisationIsInvertible(GetFullFileName("REST5.DAT"));
    }

    [Test]
    public void TestSerialisationSCHOOL1()
    {
      TestSerialisationIsInvertible(GetFullFileName("SCHOOL1.DAT"));
    }

    [Test]
    public void TestSerialisationSCHOOL2()
    {
      TestSerialisationIsInvertible(GetFullFileName("SCHOOL2.DAT"));
    }

    [Test]
    public void TestSerialisationSCHOOL3()
    {
      TestSerialisationIsInvertible(GetFullFileName("SCHOOL3.DAT"));
    }

    [Test]
    public void TestSerialisationSCHOOL4()
    {
      TestSerialisationIsInvertible(GetFullFileName("SCHOOL4.DAT"));
    }

    [Test]
    public void TestSerialisationSCHOOL5()
    {
      TestSerialisationIsInvertible(GetFullFileName("SCHOOL5.DAT"));
    }

    [Test]
    public void TestSerialisationSCHOOL6()
    {
      TestSerialisationIsInvertible(GetFullFileName("SCHOOL6.DAT"));
    }

    #endregion
  }
}
