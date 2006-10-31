using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Collections;
using System.ComponentModel;
using MMEd.Util;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

// Represents an entire MMv3 level (PS version)

namespace MMEd.Chunks
{
    //qq are these really necessary? some unit tests fail without them, but they're
    //   a bit strange
    [XmlInclude(typeof(NamedImageGroup)), XmlInclude(typeof(TIMChunk)),
    XmlInclude(typeof(OddImageChunk)), XmlInclude(typeof(BumpImageChunk))]
    public class Level : Chunk
    {
        [Description("Four int32s at the start of the file, meaning unknown")]
        public int[] Header;

        [Description("After the header, there's a big block of zero bytes")]
        public int ZeroByteCount;

        [Description("The textures and sprites")]
        public GroupingChunk NamedImageGroups;

        [Description("Until I can figure out the format")]
        public RawDataChunk OBJTs;

        [Description("most of the level data is in here")]
        public SHETChunk SHET;

        public TIMChunk GetTileById(short xiId)
        {
            //assume the first named image group is tiles
            return ((NamedImageGroup)NamedImageGroups.mChildren[1]).mChildren[xiId] as TIMChunk;
        }

        public override void Deserialise(Stream inStr)
        {
            if (inStr is FileStream)
            {
                mName = ((FileStream)inStr).Name;
            }

            //BinaryReader is little-endian
            BinaryReader bin = new BinaryReader(inStr);

            //first, a header of 4 int32s:
            Header = new int[4];
            for (int i=0; i<Header.Length; i++)
                Header[i] = bin.ReadInt32();

            //then, a block of zero bytes (int32 aligned)
            int nextInt = bin.ReadInt32();
            ZeroByteCount = 0;
            while (nextInt == 0) {
                ZeroByteCount+=4;
                nextInt = bin.ReadInt32();
            }

            ArrayList imageGroups = new ArrayList();

            //now, an array of image or comment structs
            while (nextInt != 0)
            {
                bin.BaseStream.Seek(-4, SeekOrigin.Current);
                NamedImageGroup nim = new NamedImageGroup();
                nim.Deserialise(bin.BaseStream);
                imageGroups.Add(nim);
                nextInt = bin.ReadInt32();
            }

            //nextInt is len == 0. 
            //There will be one more zero int32, then OBJTs
            nextInt = bin.ReadInt32();
            if (nextInt != 0)
                throw new DeserialisationException(string.Format("Expecting int32 0, found {0} at offset {1}", nextInt, inStr.Position - 4));
            
            //make a child to hold the EOF marker
            imageGroups.Add(new RawDataChunk("EOF marker", new byte[8]));

            //put the children so far in a grouping chunk
            NamedImageGroups = new GroupingChunk("textures and sprites", (Chunk[])imageGroups.ToArray(typeof(Chunk)));

            //read the rest of the file into a "remainder"
            //byte array so we can search for the SHET
            long lStartOfObjts = inStr.Position;
            byte[] lRest = new byte[inStr.Length - lStartOfObjts];

            StreamUtils.EnsureRead(inStr, lRest);

            //use a heuristic to find the SHET.
            //TODO: between here and the SHET is an array of OBJTs
            //in TMD format. I can't get a reliable handle on them yet.
            //See TMPtoVRML.php for work so far on this.
            //
            //we need an 8bit char set (ASCII is 7bit). windows-1252 will do
            string lRestAsString = Encoding.GetEncoding("windows-1252").GetString(lRest);
            Regex lSHETStartRegex = new Regex(
                "([a-zA-Z0-9'()*+.\\-_ ]{10,13})\0([a-zA-Z0-9'()*+.\\-_ ]{10,13})\0(.{7})",
                RegexOptions.CultureInvariant | RegexOptions.Singleline);

            MatchCollection matches = lSHETStartRegex.Matches(lRestAsString);
            if (matches.Count != 1)
                throw new DeserialisationException(string.Format("Expecting exactly one match for SHET start regex. Found {0}", matches.Count));

            int SHETstart = matches[0].Index; //(relative to end of images section)

            byte[] objts = new byte[SHETstart];
            Array.Copy(lRest, 0, objts, 0, SHETstart);
            OBJTs = new RawDataChunk("OBJTs", objts);

            //seek to the start of the SHET, and pretend we got here without cheating!
            objts = lRest = null;
            inStr.Seek(lStartOfObjts + SHETstart, SeekOrigin.Begin);

            SHET = new SHETChunk(inStr, bin);
        }

        public override void Serialise(Stream outStr)
        {
            BinaryWriter bout = new BinaryWriter(outStr);

            //first, a header of 4 int32s:
            for (int i = 0; i < Header.Length; i++)
                bout.Write(Header[i]);

            //then, a block of zero bytes (int32 aligned)
            for (int i = 0; i < ZeroByteCount; i++)
                bout.Write((byte)0);

            foreach (Chunk child in this.GetChildren())
            {
                child.Serialise(outStr);
            }
        }

        public override Chunk[] GetChildren()
        {
            ArrayList acc = new ArrayList();
            acc.Add(NamedImageGroups);
            acc.Add(OBJTs);
            acc.Add(SHET);
            return (Chunk[])acc.ToArray(typeof(Chunk));
        }

        private string mName = "MMv3 Level";

        public override string Name
        {
            get { return mName; }
        }

         public override void ReplaceChild(Chunk xiFrom, Chunk xiTo)
        {
            if (xiFrom == NamedImageGroups)
            {
                NamedImageGroups = (GroupingChunk)xiTo;
            }
            else if (xiFrom == OBJTs)
            {
                OBJTs = (RawDataChunk)xiTo;
            }
            else if (xiFrom == SHET)
            {
                SHET = (SHETChunk)xiFrom;
            }
            else
            {
                throw new ArgumentException("xifrom not found!");
            }            
        }
    }
}
