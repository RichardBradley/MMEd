using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace MMEd.Chunks
{
    public class DeserialisationException : IOException
    {
        public long Offset = -1;

        public DeserialisationException(string xiMessage) : base(xiMessage) { }
        public DeserialisationException(string xiMessage, long xiOffset)
            : this(string.Format("{0} at offset {1}", xiMessage, xiOffset))
        {
            this.Offset = xiOffset;
        }
    }
}
