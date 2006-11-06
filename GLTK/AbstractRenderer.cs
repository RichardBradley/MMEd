using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

using Tao.OpenGl;
using Tao.Glfw;
using Tao.Platform.Windows;

namespace GLTK
{
  public abstract class AbstractRenderer : IDisposable
  {
    public void Attach(RenderingSurface xiSurface)
    {
      lock (typeof(AbstractRenderer))
      {
        Detach();

        mSurface = xiSurface;

        try
        {
          mRenderingContext = Wgl.wglCreateContext(mSurface.DeviceContext);
          if (mRenderingContext == IntPtr.Zero)
          {
            throw new Exception("Could not create rendering context");
          }

          MakeCurrent();
        }
        catch
        {
          Detach();
        }

        mSurface.Resize += Surface_Resize;
        mSurface.ReleaseDeviceContext += Surface_ReleaseDeviceContext;
        mSurface.Paint += Surface_Paint;

        SetViewPort(mSurface.Width, mSurface.Height);

        Init();
      }
    }

    public void Detach()
    {
      if (mRenderingContext != IntPtr.Zero)
      {
        try
        {
          if (!Wgl.wglMakeCurrent(IntPtr.Zero, IntPtr.Zero))
          {
            throw new Exception("Could not release rendering context");
          }

          if (!Wgl.wglDeleteContext(mRenderingContext))
          {
            throw new Exception("Could not delete rendering context");
          }
        }
        finally
        {
          try
          {
            if (mSurface != null)
            {
              mSurface.Resize -= Surface_Resize;
              mSurface.ReleaseDeviceContext -= Surface_ReleaseDeviceContext;
              mSurface.Paint -= Surface_Paint;
            }
          }
          finally
          {
            mSurface = null;
            mRenderingContext = IntPtr.Zero;
          }
        }
      }
    }

    void Surface_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
    {
      lock (typeof(AbstractRenderer))
      {
        MakeCurrent();

        if (NextFrame != null)
        {
          NextFrame(this, new NextFrameArgs(false));
        }
      }
    }

    void MakeCurrent()
    {
      if (!Wgl.wglMakeCurrent(mSurface.DeviceContext, mRenderingContext))
      {
        throw new Exception("Could not set the rendering context");
      }
    }

    void Surface_ReleaseDeviceContext(object xiSender, DeviceContextEventArgs xiArgs)
    {
      Detach();
    }

    void Surface_Resize(object sender, EventArgs e)
    {
      SetViewPort(mSurface.Width, mSurface.Height);
    }

    protected virtual void Init()
    {
      Gl.glEnable(Gl.GL_TEXTURE_2D);
      Gl.glShadeModel(Gl.GL_SMOOTH);
      Gl.glClearColor(0, 0, 0, 0.5f);
      Gl.glClearDepth(1);
      Gl.glEnable(Gl.GL_DEPTH_TEST);
      Gl.glDepthFunc(Gl.GL_LEQUAL);
      Gl.glHint(Gl.GL_PERSPECTIVE_CORRECTION_HINT, Gl.GL_NICEST);
      Gl.glColorMaterial(Gl.GL_FRONT_AND_BACK, Gl.GL_AMBIENT_AND_DIFFUSE);
    }

    protected void SetViewPort(int xiWidth, int xiHeight)
    {
      Gl.glViewport(0, 0, xiWidth, xiHeight);
    }

    public void Dispose()
    {
      Detach();
    }

    protected void CheckReady()
    {
      if (mSurface == null)
      {
        throw new InvalidOperationException("Renderer not ready");
      }
    }

    public void Clear()
    {
      CheckReady();
      Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);
    }

    public void SetCamera(Camera xiCamera)
    {
      Gl.glMatrixMode(Gl.GL_PROJECTION);
      Gl.glLoadIdentity();

      int[] lViewPort = new int[4];
      Gl.glGetIntegerv(Gl.GL_VIEWPORT, lViewPort);

      if (mPickMode)
      {
        Glu.gluPickMatrix((double)mPickX, (double)(lViewPort[3] - mPickY), 1, 1, lViewPort);
      }

      Glu.gluPerspective(
        xiCamera.Fov, 
        (double)(lViewPort[0] - lViewPort[3]) / (double)(lViewPort[3] - lViewPort[1]), 
        xiCamera.NearClip, 
        xiCamera.FarClip);

      Gl.glMatrixMode(Gl.GL_MODELVIEW);
      Gl.glLoadMatrixd(xiCamera.Transform.Inverse().ToArray());
    }

    public void ResetLights()
    {
      lock (typeof(AbstractRenderer))
      {
        Gl.glDisable(Gl.GL_LIGHT0);
        Gl.glDisable(Gl.GL_LIGHT1);
        Gl.glDisable(Gl.GL_LIGHT2);
        Gl.glDisable(Gl.GL_LIGHT3);
        Gl.glDisable(Gl.GL_LIGHT4);
        Gl.glDisable(Gl.GL_LIGHT5);
        Gl.glDisable(Gl.GL_LIGHT6);
        Gl.glDisable(Gl.GL_LIGHT7);
      }

      mNextLight = 0;
    }

    public int SetLight(Light xiLight)
    {
      SetLight(GetLightId(mNextLight), xiLight);

      int lRet = mNextLight;

      if (mNextLight > 6)
      {
        mNextLight = 0;
      }
      else
      {
        ++mNextLight;
      }

      return lRet;
    }

    private static int GetLightId(int xiLightNumber)
    {
      switch (xiLightNumber)
      {
        case 0:
          return Gl.GL_LIGHT0;
        case 1:
          return Gl.GL_LIGHT1;
        case 2:
          return Gl.GL_LIGHT2;
        case 3:
          return Gl.GL_LIGHT3;
        case 4:
          return Gl.GL_LIGHT4;
        case 5:
          return Gl.GL_LIGHT5;
        case 6:
          return Gl.GL_LIGHT6;
        case 7:
          return Gl.GL_LIGHT7;
        default:
          throw new Exception("Only 7 lights are supported");
      }
    }

    public int SetLight(int xiLightId, Light xiLight)
    {
      lock (typeof(AbstractRenderer))
      {
        Gl.glLightfv(
          xiLightId,
          Gl.GL_POSITION,
          new float[] { 
          (float)xiLight.Position.x, 
          (float)xiLight.Position.y, 
          (float)xiLight.Position.z,
           1f});

        Gl.glLightfv(
          xiLightId,
          Gl.GL_DIFFUSE,
          new float[] { 
          (float)xiLight.DiffuseColor.R  / 255, 
          (float)xiLight.DiffuseColor.G  / 255, 
          (float)xiLight.DiffuseColor.B  / 255,
          (float)xiLight.DiffuseIntensity});

        Gl.glLightfv(
          xiLightId,
          Gl.GL_AMBIENT,
          new float[] { 
          (float)xiLight.AmbientColor.R  / 255, 
          (float)xiLight.AmbientColor.G  / 255, 
          (float)xiLight.AmbientColor.B  / 255,
          (float)xiLight.AmbientIntensity});

        Gl.glLightfv(
          xiLightId,
          Gl.GL_SPECULAR,
          new float[] { 
          (float)xiLight.SpecularColor.R  / 255, 
          (float)xiLight.SpecularColor.G  / 255, 
          (float)xiLight.SpecularColor.B  / 255,
          (float)xiLight.SpecularIntensity});

        Gl.glEnable(GetLightId(mNextLight));
      }

      return xiLightId;
    }

    public void EnableLighting()
    {
      Gl.glEnable(Gl.GL_LIGHTING);
      Gl.glEnable(Gl.GL_COLOR_MATERIAL);
      Gl.glColorMaterial(Gl.GL_FRONT_AND_BACK, Gl.GL_AMBIENT_AND_DIFFUSE);
    }

    public void DisableLighting()
    {
      Gl.glDisable(Gl.GL_LIGHTING);
      Gl.glDisable(Gl.GL_COLOR_MATERIAL);
    }

    public void PushTransform(Matrix xiTransform)
    {
      Gl.glPushMatrix();
      Gl.glMultMatrixd(xiTransform.ToArray());
    }

    public void PopTransform()
    {
      Gl.glPopMatrix();
    }

    public int ImageToTextureId(Bitmap xiTexture)
    {
      //WeakReference lRef = new WeakReference(xiTexture); //gives strange behaviour
      Bitmap lRef = xiTexture;
      if (mImageToTextureIdMap.Contains(lRef))
      {
        return (int)mImageToTextureIdMap[lRef];
      }
      else
      {
        int lRet = LoadTexture(xiTexture);
        mImageToTextureIdMap[lRef] = lRet;
        return lRet;
      }
    }

    private int LoadTexture(Bitmap xiTexture)
    {
      int lTextureId;
      Gl.glGenTextures(1, out lTextureId);

      //qq disabled because:
      // 1) the Flat textures were upside down
      // 2) this function modifies the bitmap which is it passed, which
      //    is bad and unexpected
      // I don't know if (1) is inherent in GL (i.e. if this change is correct)
      // or if it's because the z-axis is inverted in the levels (i.e. this change
      // is incorrect)
      // (2) needs to be fixed if the change to fix (1) is reverted.
      //
      //xiTexture.RotateFlip(RotateFlipType.RotateNoneFlipY);
      //
      Rectangle rectangle = new Rectangle(0, 0, xiTexture.Width, xiTexture.Height);
      BitmapData lData = xiTexture.LockBits(
        new Rectangle(0, 0, xiTexture.Width, xiTexture.Height),
        ImageLockMode.ReadOnly,
        PixelFormat.Format24bppRgb);

      Gl.glBindTexture(Gl.GL_TEXTURE_2D, lTextureId);
      Gl.glTexImage2D(
        Gl.GL_TEXTURE_2D, 
        0, 
        Gl.GL_RGB8, 
        xiTexture.Width,
        xiTexture.Height, 
        0, 
        Gl.GL_BGR, 
        Gl.GL_UNSIGNED_BYTE, 
        lData.Scan0);

      Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_LINEAR);
      Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_LINEAR);

      xiTexture.UnlockBits(lData);

      return lTextureId;
    }

    public Mesh Pick(int x, int y)
    {
      lock (typeof(AbstractRenderer))
      {
        int lPickIndex = int.MinValue;

        try
        {
          mPickMode = true;
          mPickX = x;
          mPickY = y;
          mPickMeshes.Clear();
          mPickIndex = 0;

          int[] lPickBuffer = new int[512];

          Gl.glSelectBuffer(lPickBuffer.Length, lPickBuffer);

          Gl.glRenderMode(Gl.GL_SELECT);

          Gl.glInitNames();
          Gl.glPushName(0);

          Gl.glMatrixMode(Gl.GL_PROJECTION);
          Gl.glPushMatrix();
          Gl.glMatrixMode(Gl.GL_MODELVIEW);

          if (NextFrame != null)
          {
            NextFrame(this, new NextFrameArgs(true));
          }

          Gl.glMatrixMode(Gl.GL_PROJECTION);
          Gl.glPopMatrix();
          Gl.glMatrixMode(Gl.GL_MODELVIEW);

          int lHits = Gl.glRenderMode(Gl.GL_RENDER);

          int lDepth = int.MaxValue;
          for (int ii = 0; ii < lHits; ++ii)
          {
            if (lPickBuffer[ii * 4 + 1] < lDepth)
            {
              lDepth = lPickBuffer[ii * 4 + 1];
              lPickIndex = lPickBuffer[ii * 4 + 3];
            }
          }
        }
        finally
        {
          mPickMode = false;
        }

        if (mPickMeshes.ContainsKey(lPickIndex))
        {
          return mPickMeshes[lPickIndex];
        }
        else
        {
          return null;
        }
      }
    }

    public void RenderMesh(Mesh xiMesh)
    {
      if (mPickMode)
      {
        Gl.glLoadName(mPickIndex);
        mPickMeshes.Add(mPickIndex, xiMesh);
        ++mPickIndex;
      }

      RenderMeshInternal(xiMesh);
    }

    protected abstract void RenderMeshInternal(Mesh xiMesh);

    public delegate void NextFrameEventHandler(AbstractRenderer xiSender, EventArgs xiArgs);
    public event NextFrameEventHandler NextFrame;

    private RenderingSurface mSurface;
    private IntPtr mRenderingContext = IntPtr.Zero;
    private Hashtable mImageToTextureIdMap = new Hashtable();

    private bool mPickMode = false;
    private int mPickIndex = 0;
    private Dictionary<int, Mesh> mPickMeshes = new Dictionary<int, Mesh>();
    private double mPickX;
    private double mPickY;

    private int mNextLight = 0;
  }
}
