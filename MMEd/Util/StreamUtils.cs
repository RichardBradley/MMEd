using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using MMEd.Chunks;

namespace MMEd.Util
{
    abstract class StreamUtils
    {
        // returns true or false as the contents of the two streams
        // are equal or not equal
        // Will seek both streams to EOF if they are equal, or to
        // an undefined point after the first differeing byte if they 
        // are not.
        public static bool StreamsAreEqual(Stream a, Stream b)
        {
            const int BUF_SIZE = 256;
            byte[] bufA = new byte[BUF_SIZE];
            byte[] bufB = new byte[BUF_SIZE];
            while (true)
            {
                int lenReadA = a.Read(bufA, 0, BUF_SIZE);
                int lenReadB = b.Read(bufB, 0, BUF_SIZE);
                if (lenReadA != lenReadB) return false;
                if (lenReadA == 0) return true;
                // would Buffer.Equals work here?
                for (int i=0; i<BUF_SIZE; i++) 
                    if (bufA[i] != bufB[i])
                        return false;
            }
        }

        /// <summary>
        ///   Writes the given buffer to the stream
        /// </summary>
        public static void Write(Stream a, byte[] buff)
        {
            a.Write(buff, 0, buff.Length);
        }

        /// <summary>
        ///   Reads into the given buffer from the stream, throwing an
        ///   exception if the whole buffer cannot be read
        /// </summary>
        public static void EnsureRead(Stream a, byte[] buff)
        {
            EnsureRead(a, buff, 0, buff.Length);
        }
        /// <summary>
        ///   Reads into the given buffer from the stream, throwing an
        ///   exception if the whole buffer cannot be read
        /// </summary>
        public static void EnsureRead(Stream a, byte[] buff, int offset, int length)
        {
            int read = a.Read(buff, offset, length);
            if (read != length)
            {
                throw new IOException(string.Format("unable to read {0} bytes from stream - read only {1}", length, read));
            }
        }

        public static void WriteZeros(Stream a, int count)
        {
            WriteRepeatedBytes(a, count, 0);
        }

        public static void WriteRepeatedBytes(Stream xiStr, int xiCount, byte xiVal)
        {
            //probably faster to use locally allocated buff
            //array than to refer to a statically allocated one
            //which might need swapping into memory

            const int MAX_BUF_SIZE = 32; //this value should be tuned, but it's probably not worth it
            
            byte[] buff = new byte[Math.Min(xiCount, MAX_BUF_SIZE)];
            if (xiVal != 0) {
                for (int i=0; i<buff.Length; i++)
                    buff[i] = xiVal;
            }
            
            for (int len = 0; len < xiCount; len+=buff.Length) {
                xiStr.Write(buff, 0, Math.Min(buff.Length, xiCount-len));
            }
        }

        // reads a null terminated string from a stream, consuming but
        // not returning the NUL char
        //
        // All chars in the string must be printable ASCII chars, or ' '
        //
        // Throws an ArgumentException if reading is not possible.
        // The stream will be left in an undefined state.
        public static string ReadASCIINullTermString(Stream s)
        {
            StringBuilder acc = new StringBuilder();
            while (true)
            {
                int i = s.ReadByte();
                if (i == 0)
                {
                    return acc.ToString();
                }
                else if (i == -1)
                {
                    throw new ArgumentException("Encountered EOF while trying to read a null terminated string");
                }
                else if (i == ' ' ||
                    (i >= '!' && i <= '~'))
                {
                    acc.Append((char)i);
                }
                else
                {
                    throw new ArgumentException(string.Format("Encountered non-printable char ({0}) while trying to read a null terminated string", i));
                }
            }
        }

        public static void WriteASCIINullTermString(Stream strm, string s)
        {
            byte[] ascii = Encoding.ASCII.GetBytes(s);
            strm.Write(ascii, 0, ascii.Length);
            strm.WriteByte(0);
        }

        public static bool ReadShortFlag(BinaryReader bin)
        {
            switch (bin.ReadInt16())
            {
                case 0: return false;
                case 1: return true;
                default: throw new DeserialisationException("Short flag was neither 0 nor 1", bin.BaseStream.Position);
            }
        }

        public static void WriteShortFlag(BinaryWriter bout, bool xiValue)
        {
            bout.Write(xiValue ? (short)1 : (short)0);
        }
        
        public static bool ReadByteFlag(BinaryReader bin)
        {
            switch (bin.ReadByte())
            {
                case 0: return false;
                case 1: return true;
                default: throw new DeserialisationException("Byte flag was neither 0 nor 1", bin.BaseStream.Position);
            }
        }

        public static void WriteByteFlag(BinaryWriter bout, bool xiValue)
        {
            bout.Write(xiValue ? (byte)1 : (byte)0);
        }
    }
}
