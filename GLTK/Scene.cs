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
      xiSender.ResetLights();
      
      xiSender.SetCamera(mCamera);

      foreach (Light lLight in mLights)
      {
        if (lLight.On)
        {
          xiSender.SetLight(lLight);
        }
      }

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

    public void AddLight(Light xiLight)
    {
      mLights.Add(xiLight);
    }

    public void RemoveLight(Light xiLight)
    {
      if (mLights.Contains(xiLight))
      {
        mLights.Remove(xiLight);
      }
    }

    public void Clear()
    {
        mObjects.Clear();
    }

    public int ObjectCount
    {
      get
      {
        return mObjects.Count;
      }
    }

    public Entity this[int xiIndex]
    {
      get
      {
        return mObjects[xiIndex];
      }
    }

    private List<Light> mLights = new List<Light>();
    private List<Entity> mObjects = new List<Entity>();
    private Camera mCamera = new Camera();
  }
}
