using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;

using Tao.OpenGl;
using Tao.Glfw;
using Tao.Platform.Windows;

namespace GLTK
{
  public enum RenderOptions
  {
    Default = 0,
    ShowNormals = 2
  }

  public abstract class AbstractRenderer : IDisposable
  {
    public void Attach(RenderingSurface xiSurface)
    {
      if (xiSurface.IsInDesignMode)
      {
        return;
      }

      Detach();

      mSurface = xiSurface;

      try
      {
        mRenderingContext = Wgl.wglCreateContext(mSurface.DeviceContext);
        if (mRenderingContext == IntPtr.Zero)
        {
          throw new Exception("Could not create rendering context");
        }

        lock (mRenderingContexts)
        {
          if (mRenderingContexts.Count != 0)
          {
            Wgl.wglShareLists(mRenderingContexts[0], mRenderingContext);
          }

          mRenderingContexts.Add(mRenderingContext);
        }
      }
      catch
      {
        Detach();
        throw;
      }

      mSurface.Resize += Surface_Resize;
      mSurface.ReleaseDeviceContext += Surface_ReleaseDeviceContext;
      mSurface.Paint += Surface_Paint;

      SetViewPort(mSurface.Width, mSurface.Height);

      Init();
    }

    public void Detach()
    {
      if (mRenderingContext != IntPtr.Zero)
      {
        try
        {
          using (ScopedLock lLock = ScopedLock.Lock(mRenderingContext))
          {
            if (Wgl.wglGetCurrentContext() == mRenderingContext)
            {
              if (!Wgl.wglMakeCurrent(IntPtr.Zero, IntPtr.Zero))
              {
                throw new Exception("Could not release rendering context");
              }
            }

            lock (mRenderingContexts)
            {
              mRenderingContexts.Remove(mRenderingContext);
              if (!Wgl.wglDeleteContext(mRenderingContext))
              {
                throw new Exception("Could not delete rendering context");
              }
            }
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
      if (NextFrame != null)
      {
        NextFrame(this, new NextFrameArgs(false));
      }
    }

    protected ScopedLock LockContext()
    {
      ScopedLock lLock = ScopedLock.Lock(mRenderingContext);
      try
      {
        if (Wgl.wglGetCurrentContext() != mRenderingContext)
        {
          if (!Wgl.wglMakeCurrent(mSurface.DeviceContext, mRenderingContext))
          {
            throw new Exception("Could not set the rendering context");
          }
        }
      }
      catch
      {
        lLock.Dispose();
        throw;
      }

      return lLock;
    }

    void Surface_ReleaseDeviceContext(object xiSender, DeviceContextEventArgs xiArgs)
    {
      Detach();
    }

    void Surface_Resize(object sender, EventArgs e)
    {
      SetViewPort(mSurface.Width, mSurface.Height);
    }

    public RenderingSurface RendereringSurface
    {
      get { return mSurface; }
    }

    protected virtual void Init()
    {
      using (ScopedLock lLock = LockContext())
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
    }

    protected void SetViewPort(int xiWidth, int xiHeight)
    {
      using (ScopedLock lLock = LockContext())
      {
        Gl.glViewport(0, 0, xiWidth, xiHeight);
      }
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
      using (ScopedLock lLock = LockContext())
      {
        Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);
      }

      ResetCounters();
    }

    public void ResetCounters()
    {
      mMeshCount = 0;
      mVertexCount = 0;
      mRenderTime = 0;
    }

    public void ClearDepthBuffer()
    {
      using (ScopedLock lLock = LockContext())
      {
        Gl.glClear(Gl.GL_DEPTH_BUFFER_BIT);
      }
    }

    public void SetCamera(Camera xiCamera)
    {
      using (ScopedLock lLock = LockContext())
      {
        Gl.glMatrixMode(Gl.GL_PROJECTION);
        Gl.glLoadIdentity();

        int[] lViewPort = new int[4];
        Gl.glGetIntegerv(Gl.GL_VIEWPORT, lViewPort);

        if (mPickMode)
        {
          Glu.gluPickMatrix((double)mPickX, (double)(lViewPort[3] - mPickY), 1, 1, lViewPort);
        }

        switch (xiCamera.ProjectionMode)
        {
          case eProjectionMode.Perspective:
            Glu.gluPerspective(
              xiCamera.Fov,
              (double)(lViewPort[0] - lViewPort[3]) / (double)(lViewPort[3] - lViewPort[1]),
              xiCamera.NearClip,
              xiCamera.FarClip);
            break;
         
          case eProjectionMode.Orthographic:
            double lSize = xiCamera.Position.GetPositionVector() * xiCamera.ZAxis;
            double lRatio = (double)lViewPort[3] / (double)lViewPort[2];
            Gl.glOrtho(
              -lSize / 2,
              lSize / 2,
              -lSize * lRatio / 2,
              lSize * lRatio / 2,
              xiCamera.NearClip,
              xiCamera.FarClip);
            break;
          
          default:
            throw new Exception("Unrecognised projection mode");
        }

        Gl.glMatrixMode(Gl.GL_MODELVIEW);
        Gl.glLoadMatrixd(xiCamera.Transform.Inverse().ToArray());
      }
    }

    public void ResetLights()
    {
      using (ScopedLock lLock = LockContext())
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
      SetLight(mNextLight, xiLight);

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
          throw new Exception("Only 8 lights are supported");
      }
    }

    public int SetLight(int xiLightId, Light xiLight)
    {
      int lGLLightId = GetLightId(xiLightId);
      using (ScopedLock lLock = LockContext())
      {
        Gl.glLightfv(
          lGLLightId,
          Gl.GL_POSITION,
          new float[] { 
          (float)xiLight.Position.x, 
          (float)xiLight.Position.y, 
          (float)xiLight.Position.z,
           1f});

        Gl.glLightfv(
          lGLLightId,
          Gl.GL_DIFFUSE,
          new float[] { 
          (float)xiLight.DiffuseColor.R  / 255, 
          (float)xiLight.DiffuseColor.G  / 255, 
          (float)xiLight.DiffuseColor.B  / 255,
          (float)xiLight.DiffuseIntensity});

        Gl.glLightfv(
          lGLLightId,
          Gl.GL_AMBIENT,
          new float[] { 
          (float)xiLight.AmbientColor.R  / 255, 
          (float)xiLight.AmbientColor.G  / 255, 
          (float)xiLight.AmbientColor.B  / 255,
          (float)xiLight.AmbientIntensity});

        Gl.glLightfv(
          lGLLightId,
          Gl.GL_SPECULAR,
          new float[] { 
          (float)xiLight.SpecularColor.R  / 255, 
          (float)xiLight.SpecularColor.G  / 255, 
          (float)xiLight.SpecularColor.B  / 255,
          (float)xiLight.SpecularIntensity});

        Gl.glEnable(lGLLightId);
      }

      return xiLightId;
    }

    public void EnableLighting()
    {
      using (ScopedLock lLock = LockContext())
      {
        Gl.glEnable(Gl.GL_LIGHTING);
        Gl.glEnable(Gl.GL_COLOR_MATERIAL);
        Gl.glColorMaterial(Gl.GL_FRONT_AND_BACK, Gl.GL_AMBIENT_AND_DIFFUSE);
      }
    }

    public void DisableLighting()
    {
      using (ScopedLock lLock = LockContext())
      {
        Gl.glDisable(Gl.GL_LIGHTING);
        Gl.glDisable(Gl.GL_COLOR_MATERIAL);
      }
    }

    public void PushTransform(Matrix xiTransform)
    {
      using (ScopedLock lLock = LockContext())
      {
        Gl.glPushMatrix();
        Gl.glMultMatrixd(xiTransform.ToArray());
      }
    }

    public void PopTransform()
    {
      using (ScopedLock lLock = LockContext())
      {
        Gl.glPopMatrix();
      }
    }

    // TODO: this is a failure of encapsulation
    // we should just store the bitmaps on the meshes and
    // do this mapping in the renderer
    public static int ImageToTextureId(Bitmap xiTexture)
    {
      // TODO: should return an IDisposable here, and UnLoadTexture on dispose
      // will need to rely on GC dispose, not using() though

      if (xiTexture == null)
      {
        return 0;
      }
      if (mImageToTextureIdMap.ContainsKey(xiTexture))
      {
        int lRet = mImageToTextureIdMap[xiTexture];
        Debug.WriteLine(string.Format("Image with hashcode #{0:x} already loaded as texture #{1:x}",
          xiTexture.GetHashCode(), lRet));
        return lRet;
      }
      else
      {
        int lRet = LoadTexture(xiTexture);
        Debug.WriteLine(string.Format("Loaded image with hashcode #{0:x} into vram as texture #{1:x}",
          xiTexture.GetHashCode(), lRet));
        mImageToTextureIdMap[xiTexture] = lRet;
        return lRet;
      }
    }

    //not guaranteed to be the same bitmap
    public static Bitmap TextureIdToImage(int xiTexID)
    {
      // while there is a "containsValue" method, there is no "fetchKeyByValue"
      // method :-(
      foreach (KeyValuePair<Bitmap, int> lEntry in mImageToTextureIdMap)
      {
        if (lEntry.Value == xiTexID)
        {
          return lEntry.Key;
        }
      }
      return null;
    }

    private static int LoadTexture(Bitmap xiTexture)
    {
      int lTextureId;
      lock (mRenderingContexts)
      {
        if (mRenderingContexts.Count == 0)
        {
          throw new Exception("No rendering contexts exist");
        }

        Rectangle rectangle = new Rectangle(0, 0, xiTexture.Width, xiTexture.Height);
        BitmapData lData = xiTexture.LockBits(
          new Rectangle(0, 0, xiTexture.Width, xiTexture.Height),
          ImageLockMode.ReadOnly,
          PixelFormat.Format24bppRgb);

        using (ScopedLock lLock = ScopedLock.Lock(mRenderingContexts[0]))
        {
          Gl.glGenTextures(1, out lTextureId);

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
        }

        xiTexture.UnlockBits(lData);
      }
      return lTextureId;
    }

    public Mesh Pick(int x, int y)
    {
      using (ScopedLock lLock = LockContext())
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

    public virtual void RenderScene(Scene xiScene, Camera xiCamera, RenderOptions xiOptions)
    {
      Clear();
      ResetLights();

      SetCamera(xiCamera);

      foreach (Light lLight in xiScene.Lights)
      {
        if (lLight.On)
        {
          SetLight(lLight);
        }
      }

      // first, render all the opaque objects,
      // then render all the translucent objects, in reverse depth order
      //
      // TODO: this sort order is wrong: we need to sort by depth, based on camera & matrix
      SortedList<Mesh, Matrix[]> lDepthSortedTranslucentMeshesToTransform =
        new SortedList<Mesh, Matrix[]>(new FirstTriMidPointDepthComparer());
      Stack<Matrix> lMatrixStack = new Stack<Matrix>();

      foreach (Entity lObject in xiScene.Objects)
      {
        PushTransform(lObject.Transform);
        lMatrixStack.Push(lObject.Transform);

        foreach (Mesh lMesh in lObject.Meshes)
        {
          if (lMesh.RenderMode == RenderMode.TranslucentFilled)
          {
            lDepthSortedTranslucentMeshesToTransform.Add(lMesh, lMatrixStack.ToArray());
          }
          else
          {
            RenderMesh(lMesh, xiOptions);
          }
        }

        PopTransform();
        lMatrixStack.Pop();
      }

      // now render the translucent polys
      if (lDepthSortedTranslucentMeshesToTransform.Count > 0)
      {
        Gl.glEnable(Gl.GL_BLEND);
        Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);

        foreach (KeyValuePair<Mesh, Matrix[]> lKV in lDepthSortedTranslucentMeshesToTransform)
        {
          foreach (Matrix lM in lKV.Value)
          {
            PushTransform(lM);
          }

          RenderMesh(lKV.Key, xiOptions);

          foreach (Matrix lM in lKV.Value)
          {
            PopTransform();
          }
        }

        Gl.glDisable(Gl.GL_BLEND);
      }
    }

    public virtual void RenderSingleObject(Entity xiObject, RenderOptions xiOptions)
    {
      PushTransform(xiObject.Transform);

      foreach (Mesh lMesh in xiObject.Meshes)
      {
        RenderMesh(lMesh, xiOptions);
      }
      
      PopTransform();
    }

    public void RenderMesh(Mesh xiMesh)
    {
      RenderMesh(xiMesh, RenderOptions.Default);
    }

    public void RenderMesh(Mesh xiMesh, RenderOptions xiOptions)
    {
      if (mPickMode)
      {
        using (ScopedLock lLock = LockContext())
        {
          Gl.glLoadName(mPickIndex);
        }
        mPickMeshes.Add(mPickIndex, xiMesh);
        ++mPickIndex;
      }

      DateTime lStart = DateTime.Now;
      RenderMeshInternal(xiMesh, xiOptions);

      mRenderTime += (DateTime.Now - lStart).Milliseconds;
      ++mMeshCount;
      mVertexCount += xiMesh.Vertices.Count;
    }

    protected abstract void RenderMeshInternal(Mesh xiMesh, RenderOptions xiOptions);

    private class FirstTriMidPointDepthComparer : IComparer<Mesh>
    {
      public int Compare(Mesh xiLeft, Mesh xiRight)
      {
        return xiLeft.GetHashCode().CompareTo(xiRight.GetHashCode());
      }
    }

    public RenderMode FixedRenderMode
    {
      get { return mFixedRenderMode; }
      set { mFixedRenderMode = value; }
    }

    public RenderMode DefaultRenderMode
    {
      get { return mDefaultRenderMode; }
      set { mDefaultRenderMode = value; }
    }

    public int VertexCount
    {
      get { return mVertexCount; }
    }

    public int MeshCount
    {
      get { return mMeshCount; }
    }

    public int RenderTime
    {
      get { return mRenderTime; }
    }

    private RenderMode mFixedRenderMode = RenderMode.Undefined;
    private RenderMode mDefaultRenderMode = RenderMode.Wireframe;


    public delegate void NextFrameEventHandler(AbstractRenderer xiSender, EventArgs xiArgs);
    public event NextFrameEventHandler NextFrame;

    private RenderingSurface mSurface;
    private IntPtr mRenderingContext = IntPtr.Zero;
    private static List<IntPtr> mRenderingContexts = new List<IntPtr>();
    private static Dictionary<Bitmap, int> mImageToTextureIdMap = new Dictionary<Bitmap, int>();

    private bool mPickMode = false;
    private int mPickIndex = 0;
    private Dictionary<int, Mesh> mPickMeshes = new Dictionary<int, Mesh>();
    private double mPickX;
    private double mPickY;

    private int mNextLight = 0;

    private int mVertexCount = 0;
    private int mMeshCount = 0;
    private int mRenderTime = 0;
  }
}
