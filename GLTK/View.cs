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
      mRenderer.RenderScene(mScene, mCamera, RenderOptions.Default);
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
