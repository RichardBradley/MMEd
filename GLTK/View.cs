using System;
using System.Collections.Generic;
using System.Text;

namespace GLTK
{
  public class View
  {
    public View(Scene xiScene, Camera xiCamera, AbstractRenderer xiRenderer)
    {
      mScene = xiScene;
      mCamera = xiCamera;
      mRenderer = xiRenderer;
      xiRenderer.NextFrame += new AbstractRenderer.NextFrameEventHandler(Renderer_NextFrame);
    }

    void Renderer_NextFrame(AbstractRenderer xiSender, EventArgs xiArgs)
    {
      RenderScene();
    }

    protected virtual void RenderScene()
    {
      mRenderer.Clear();
      mRenderer.ResetLights();

      SetCamera();

      foreach (Light lLight in mScene.Lights)
      {
        SetupLight(lLight);
      }

      foreach (Entity lObject in mScene.Objects)
      {
        RenderObject(lObject, RenderOptions.Default);
      }
    }

    protected virtual void SetCamera()
    {
      mRenderer.SetCamera(mCamera);
    }

    protected virtual void SetupLight(Light xiLight)
    {
      if (xiLight.On)
      {
        mRenderer.SetLight(xiLight);
      }
    }

    protected virtual void RenderObject(Entity xiObject, RenderOptions xiOptions)
    {
      mRenderer.PushTransform(xiObject.Transform);
      foreach (Mesh lMesh in xiObject.Meshes)
      {
        RenderMesh(lMesh, xiOptions);
      }
      mRenderer.PopTransform();
    }

    protected virtual void RenderMesh(Mesh xiMesh, RenderOptions xiOptions)
    {
      mRenderer.RenderMesh(xiMesh, xiOptions);
    }

    public Camera Camera
    {
      get { return mCamera; }
      set { mCamera = value; }
    }

    public AbstractRenderer Renderer
    {
      get { return mRenderer; }
    }

    protected AbstractRenderer mRenderer;
    protected Scene mScene;
    protected Camera mCamera;
  }
}
