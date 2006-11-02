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
          NextFrame(this, new EventArgs());
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
    }

    protected void SetViewPort(int xiWidth, int xiHeight)
    {
      Gl.glViewport(0, 0, xiWidth, xiHeight);
      Gl.glMatrixMode(Gl.GL_PROJECTION);
      Gl.glLoadIdentity();
      Glu.gluPerspective(80, xiWidth / (double)xiHeight, 0.1, 1e5); //qq make param //45
      Gl.glMatrixMode(Gl.GL_MODELVIEW);
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

    public void SetCamera(Matrix xiTransform)
    {
      Gl.glMatrixMode(Gl.GL_MODELVIEW);
      Gl.glLoadMatrixd(xiTransform.Inverse().ToArray());
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

      xiTexture.RotateFlip(RotateFlipType.RotateNoneFlipY);
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

    public abstract void RenderMesh(Mesh xiMesh);

    public delegate void NextFrameEventHandler(AbstractRenderer xiSender, EventArgs xiArgs);
    public event NextFrameEventHandler NextFrame;

    private RenderingSurface mSurface;
    private IntPtr mRenderingContext = IntPtr.Zero;
    private Hashtable mImageToTextureIdMap = new Hashtable();
  }
}
