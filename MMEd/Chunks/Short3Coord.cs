using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using GLTK;

// A 3-vector with short components

namespace MMEd.Chunks
{
  // Must include Matrix here to give the XmlSerializer a handle on the GLTK
  // assembly, even though no GLTK.Matrix'es should be emitted.
  // Note that you can't reference GLTK.Point here, as it will clash with 
  // Drawing.Point
  [XmlInclude(typeof(Matrix))]
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

    public Short3Coord()
    {
    }

    public Short3Coord(short x, short y, short z)
    {
      X = x;
      Y = y;
      Z = z;
    }

    public double Norm()
    {
      double dX = (double)X;
      double dY = (double)Y;
      double dZ = (double)Z;
      return Math.Sqrt(dX * dX + dY * dY + dZ * dZ);
    }

    public override string ToString()
    {
      return string.Format("({0},{1},{2})", X, Y, Z);
    }

    public override bool Equals(object obj)
    {
      return obj != null &&
        obj is Short3Coord &&
        X == ((Short3Coord)obj).X &&
        Y == ((Short3Coord)obj).Y &&
        Z == ((Short3Coord)obj).Z;
    }

    public override int GetHashCode()
    {
      return X + Y * 256 + Z * 65536;
    }
  }
}
