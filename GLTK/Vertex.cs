using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Xml.Serialization;

namespace GLTK
{
  public class Vertex
  {
    public Vertex() { }

    public Vertex(Point xiPosition)
    {
      mPosition = xiPosition;
    }

    public Vertex(Point xiPosition, Color xiColor)
    {
      mPosition = xiPosition;
      mColor = xiColor;
    }

    public Vertex(Point xiPosition, int xiTexCoordX, int xiTexCoordY)
    {
      mPosition = xiPosition;
      mTexCoordX = xiTexCoordX;
      mTexCoordY = xiTexCoordY;
    }

    public Vertex(Point xiPosition, Vector xiNormal, Color xiColor)
    {
      mPosition = xiPosition;
      mNormal = xiNormal;
      mColor = xiColor;
    }

    public Vertex(Point xiPosition, Vector xiNormal, Color xiColor, int xiTexCoordX, int xiTexCoordY)
    {
      mPosition = xiPosition;
      mNormal = xiNormal;
      mColor = xiColor;
      mTexCoordX = xiTexCoordX;
      mTexCoordY = xiTexCoordY;
    }

    public Point Position
    {
      get { return mPosition; }
      set { mPosition = value; }
    }

    public Vector Normal
    {
      get { return mNormal; }
      set { mNormal = value; }
    }

    [XmlIgnore]
    public Color Color
    {
      get { return mColor; }
      set { mColor = value; }
    }

    //required for XML serialisation, since Color structs can't be
    //serialised directly
    public int ColorAsARGB
    {
      get { return Color.ToArgb(); }
      set { Color = Color.FromArgb(value); }
    }

    public double TexCoordX
    {
      get { return mTexCoordX; }
      set { mTexCoordX = value; }
    }

    public double TexCoordY
    {
      get { return mTexCoordY; }
      set { mTexCoordY = value; }
    }

    Point mPosition = Point.Origin;
    Vector mNormal = Vector.Zero;
    Color mColor = Color.White;
    double mTexCoordX;
    double mTexCoordY;
  }
}
