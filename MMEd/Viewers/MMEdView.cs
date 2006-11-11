using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using GLTK;

using MMEd.Viewers.ThreeDee;

namespace MMEd.Viewers
{
  class MMEdView : GLTK.View
  {
    public MMEdView(ThreeDeeEditor xiViewer, Scene xiScene, Camera xiCamera, AbstractRenderer xiRenderer) 
      : base(xiScene, xiCamera, xiRenderer)
    {
      mViewer = xiViewer;
    }

    protected override void RenderObject(GLTK.Entity xiObject, RenderOptions xiOptions)
    {
      if (mCamera.ProjectionMode != eProjectionMode.Orthographic
        && mViewer.DrawNormalsMode == eDrawNormalsMode.DrawNormals)
      {
        xiOptions |= RenderOptions.ShowNormals;
      }
      base.RenderObject(xiObject, xiOptions);

      if (mActiveEntity == null)
      {
        foreach (Mesh lMesh in xiObject.Meshes)
        {
          OwnedMesh lOm = lMesh as OwnedMesh;
          if (lOm != null && lOm.Owner == mViewer.ActiveObject)
          {
            mActiveEntity = xiObject;
            break;
          }
        }
      }
    }

    protected override void RenderScene()
    {
      base.RenderScene();

      if (mActiveEntity != null)
      {
        Mesh lBoundingMesh = new Mesh(PolygonMode.Quads);
        Cuboid lBounds = mActiveEntity.GetBoundingBox();

        lBoundingMesh.AddFace(
          new Vertex(new GLTK.Point(lBounds.XMin, lBounds.YMin, lBounds.ZMax), Color.Red),
          new Vertex(new GLTK.Point(lBounds.XMax, lBounds.YMin, lBounds.ZMax), Color.Red),
          new Vertex(new GLTK.Point(lBounds.XMax, lBounds.YMax, lBounds.ZMax), Color.Red),
          new Vertex(new GLTK.Point(lBounds.XMin, lBounds.YMax, lBounds.ZMax), Color.Red));
        lBoundingMesh.AddFace(
          new Vertex(new GLTK.Point(lBounds.XMin, lBounds.YMin, lBounds.ZMin), Color.Red),
          new Vertex(new GLTK.Point(lBounds.XMin, lBounds.YMax, lBounds.ZMin), Color.Red),
          new Vertex(new GLTK.Point(lBounds.XMax, lBounds.YMax, lBounds.ZMin), Color.Red),
          new Vertex(new GLTK.Point(lBounds.XMax, lBounds.YMin, lBounds.ZMin), Color.Red));
        lBoundingMesh.AddFace(
          new Vertex(new GLTK.Point(lBounds.XMin, lBounds.YMax, lBounds.ZMin), Color.Red),
          new Vertex(new GLTK.Point(lBounds.XMin, lBounds.YMax, lBounds.ZMax), Color.Red),
          new Vertex(new GLTK.Point(lBounds.XMax, lBounds.YMax, lBounds.ZMax), Color.Red),
          new Vertex(new GLTK.Point(lBounds.XMax, lBounds.YMax, lBounds.ZMin), Color.Red));
        lBoundingMesh.AddFace(
          new Vertex(new GLTK.Point(lBounds.XMin, lBounds.YMin, lBounds.ZMin), Color.Red),
          new Vertex(new GLTK.Point(lBounds.XMax, lBounds.YMin, lBounds.ZMin), Color.Red),
          new Vertex(new GLTK.Point(lBounds.XMax, lBounds.YMin, lBounds.ZMax), Color.Red),
          new Vertex(new GLTK.Point(lBounds.XMin, lBounds.YMin, lBounds.ZMax), Color.Red));
        lBoundingMesh.AddFace(
          new Vertex(new GLTK.Point(lBounds.XMax, lBounds.YMin, lBounds.ZMin), Color.Red),
          new Vertex(new GLTK.Point(lBounds.XMax, lBounds.YMax, lBounds.ZMin), Color.Red),
          new Vertex(new GLTK.Point(lBounds.XMax, lBounds.YMax, lBounds.ZMax), Color.Red),
          new Vertex(new GLTK.Point(lBounds.XMax, lBounds.YMin, lBounds.ZMax), Color.Red));
        lBoundingMesh.AddFace(
          new Vertex(new GLTK.Point(lBounds.XMin, lBounds.YMin, lBounds.ZMin), Color.Red),
          new Vertex(new GLTK.Point(lBounds.XMin, lBounds.YMin, lBounds.ZMax), Color.Red),
          new Vertex(new GLTK.Point(lBounds.XMin, lBounds.YMax, lBounds.ZMax), Color.Red),
          new Vertex(new GLTK.Point(lBounds.XMin, lBounds.YMax, lBounds.ZMin), Color.Red));

        lBoundingMesh.RenderMode = RenderMode.Wireframe;

        Entity lBox = new Entity();
        lBox.Transform = mActiveEntity.Transform;
        lBox.Meshes.Add(lBoundingMesh);

        mRenderer.ClearDepthBuffer();
        RenderObject(lBox, RenderOptions.Default);
        mActiveEntity = null;
      }
    }

    private Entity mActiveEntity = null;
    private ThreeDeeEditor mViewer;
  }
}
