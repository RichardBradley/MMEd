using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

// Just a wrapper to organise the tree slightly better

namespace MMEd.Chunks
{
    public class RawDataChunk : Chunk
    {
        //for serialisation
        public RawDataChunk() { }

        public RawDataChunk(string xiName, byte[] xiData)
        {
            mData = xiData;
            GivenName = xiName;
        }
        public string GivenName;
        public byte[] mData;

        public override void Deserialise(System.IO.Stream inStr)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override void Serialise(System.IO.Stream outStr)
        {
            outStr.Write(mData, 0, mData.Length);
        }

        public override string Name
        {
            get
            {
                return GivenName;
            }
        }

        public override void ReplaceChild(Chunk xiFrom, Chunk xiTo)
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }
}
