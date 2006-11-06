using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace GLTK
{
  public class Light : Entity
  {
    public Light()
    {
    }

    public Light(Point xiPosition)
    {
      Position = xiPosition;
    }

    public Light(
                 Point xiPosition,
                 Color xiAmbientColor,
                 double xiAmbientIntensity,
                 Color xiDiffuseColor,
                 double xiDiffuseIntensity)
    {
      Position = xiPosition;
      mAmbientColor = xiAmbientColor;
      mAmbientIntensity = xiAmbientIntensity;
      mDiffuseColor = xiDiffuseColor;
      mDiffuseIntensity = xiDiffuseIntensity;
    }

    public double AmbientIntensity
    {
      get { return mAmbientIntensity; }
      set { mAmbientIntensity = value; }
    }

    public double DiffuseIntensity
    {
      get { return mDiffuseIntensity; }
      set { mDiffuseIntensity = value; }
    }

    public double SpecularIntensity
    {
      get { return mSpecularIntensity; }
      set { mSpecularIntensity = value; }
    }

    public Color AmbientColor
    {
      get { return mAmbientColor; }
      set { mAmbientColor = value; }
    }

    public Color SpecularColor
    {
      get { return mSpecularColor; }
      set { mSpecularColor = value; }
    }

    public Color DiffuseColor
    {
      get { return mDiffuseColor; }
      set { mDiffuseColor = value; }
    }

    public bool On
    {
      get { return mOn; }
      set { mOn = value; }
    }

    private bool mOn = true;
    private Color mDiffuseColor = Color.White;
    private Color mAmbientColor = Color.White;
    private Color mSpecularColor = Color.White;

    private double mDiffuseIntensity = 1;
    private double mAmbientIntensity = 1;
    private double mSpecularIntensity = 1;
  }
}
