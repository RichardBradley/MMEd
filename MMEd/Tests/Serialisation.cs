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
        private string[] GetSampleLevelFiles()
        {
            //qq just one level for now (TODO: make the tests below work for all levels)
            return new string[] { @"E:\MMs\mm3\public_html\MMs\MICRO\BREAKY\BREAKY1.DAT" };
            /*
            ArrayList acc = new ArrayList();
           foreach(string file in Directory.GetFiles(@"E:\MMs\mm3\public_html\MMs\MICRO", "*.DAT", SearchOption.AllDirectories))
            {
                if (Regex.IsMatch(file, "[A-Z]+\\\\[A-Z]+[0-9]\\.DAT$"))
                    acc.Add(file);
            }
            return (string[])acc.ToArray(typeof(string));
             */
        }

        [Test]
        public void TestBinarySerialisationIsInvertible()
        {
            foreach (string file in GetSampleLevelFiles())
            {
                Console.Out.WriteLine("Testing file {0}", file);
                using (Stream inStr = File.OpenRead(file))
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
        }

        [Test]
        public void TestXmlSerialisationIsInvertible()
        {
            foreach (string file in GetSampleLevelFiles())
            {
                Console.Out.WriteLine("Testing file {0}", file);
                using (Stream inStr = File.OpenRead(file))
                {
                    Level levelFromBinary = new Level();
                    levelFromBinary.Deserialise(inStr);
                    StringWriter sw = new StringWriter();
                    XmlSerializer xs = new XmlSerializer(typeof(Level));
                    xs.Serialize(sw, levelFromBinary);

                    //now the XML version of the level is in sw
                    Level levelFromXML = (Level)xs.Deserialize(new StringReader(sw.ToString()));

                    //first check that the XML serialisation is the same
                    StringWriter sw2 = new StringWriter();
                    xs.Serialize(sw2, levelFromBinary);
                    Assert.AreEqual(sw.ToString(), sw2.ToString());

                    //now see if the XML-ified level will still write binary correctly:
                    MemoryStream memStr = new MemoryStream();
                    inStr.Seek(0, SeekOrigin.Begin);
                    DebugOutputStreamWithExpectations outStr = new DebugOutputStreamWithExpectations(inStr, memStr);
                    levelFromXML.Serialise(outStr);
                    inStr.Seek(0, SeekOrigin.Begin);
                    memStr.Seek(0, SeekOrigin.Begin);
                    Assert.IsTrue(StreamUtils.StreamsAreEqual(inStr, memStr));
                }
            }
        }
    }
}
