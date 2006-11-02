using System;
using System.Collections.Generic;
using System.Text;

namespace GLTK
{
  public class Scene
  {
    public Scene(AbstractRenderer xiRenderer)
    {
      xiRenderer.NextFrame += new AbstractRenderer.NextFrameEventHandler(Renderer_NextFrame);
    }

    void Renderer_NextFrame(AbstractRenderer xiSender, EventArgs xiArgs)
    {
      xiSender.Clear();
      
      xiSender.SetCamera(mCamera);
      foreach (Entity lObject in mObjects)
      {
        xiSender.PushTransform(lObject.Transform);
        foreach (Mesh lMesh in lObject.Meshes)
        {
          xiSender.RenderMesh(lMesh);
        }
        xiSender.PopTransform();
      }
    }

    public Camera Camera
    {
      get { return mCamera; }
      set { mCamera = value; }
    }

    public void AddRange(IEnumerable<Entity> xiCollection)
    {
        mObjects.AddRange(xiCollection);
    }

    public void AddObject(Entity xiObject)
    {
      if (!mObjects.Contains(xiObject))
      {
        mObjects.Add(xiObject);
      }
    }

    public void RemoveObject(Entity xiObject)
    {
      if (mObjects.Contains(xiObject))
      {
        mObjects.Remove(xiObject);
      }
    }

    public void Clear()
    {
        mObjects.Clear();
    }

    private List<Entity> mObjects = new List<Entity>();
    private Camera mCamera = new Camera();
  }
}
