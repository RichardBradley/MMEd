using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

// A 3-vector with short components

namespace MMEd.Chunks
{
    public class Short3Coord
    {
        public short X, Y, Z;

        /// <summary>
        ///  Reads a Short3Coord from the stream, followed by two
        ///  zero bytes (i.e. so the structure is 64 bits)
        /// </summary>
        public static Short3Coord ReadShort3Coord64(BinaryReader bin)
        {
            Short3Coord acc = new Short3Coord();
            acc.X = bin.ReadInt16();
            acc.Y = bin.ReadInt16();
            acc.Z = bin.ReadInt16();
            if (bin.ReadInt16() != 0)
                throw new DeserialisationException("Expecting two zero bytes", bin.BaseStream.Position);
            return acc;
        }

        // see ReadShort3Coord64
        public void WriteShort3Coord64(BinaryWriter bout)
        {
            bout.Write(X);
            bout.Write(Y);
            bout.Write(Z);
            bout.Write((short)0);
        }
    }
}
