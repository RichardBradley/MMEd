using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

// Just a wrapper to organise the tree slightly better

namespace MMEd.Chunks
{
    public class GroupingChunk : Chunk
    {
        //for serialisation
        public GroupingChunk() { }

        public GroupingChunk(string xiName, Chunk[] xiChildren)
        {
            mChildren = xiChildren;
            GivenName = xiName;
        }
        public string GivenName;
        public Chunk[] mChildren;

        public override void Deserialise(System.IO.Stream inStr)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override void Serialise(System.IO.Stream outStr)
        {
            foreach (Chunk lChunk in mChildren)
            {
                lChunk.Serialise(outStr);
            }
        }

        public override string Name
        {
            get
            {
                return GivenName;
            }
         }

        public override Chunk[] GetChildren()
        {
             return mChildren;
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

    }
}
