using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Diagnostics;
using Tao.OpenGl;

namespace GLTK
{
  public enum eProjectionMode
  {
    Orthographic,
    Perspective
  }

  public class Camera : Entity
  {
    public Camera()
    {
    }

    public Camera(double xiFov, double xiNearClip, double xiFarClip)
    {
      mFov = xiFov;
      mNearClip = xiNearClip;
      mFarClip = xiFarClip;
    }

    public double Fov
    {
      get { return mFov; }
      set { mFov = value; }
    }

    public double NearClip
    {
      get { return mNearClip; }
      set { mNearClip = value; }
    }

    public double FarClip
    {
      get { return mFarClip; }
      set { mFarClip = value; }
    }

    public eProjectionMode ProjectionMode
    {
      get { return mProjectionMode; }
      set { mProjectionMode = value; }
    }

    private double mFov = 45;
    private double mNearClip = 0.1;
    private double mFarClip = 100;
    private eProjectionMode mProjectionMode = eProjectionMode.Perspective;
  }
}


