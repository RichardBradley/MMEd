using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using GLTK;

using MMEd.Viewers.ThreeDee;

namespace MMEd.Viewers
{
  class MMEdEditorView : GLTK.View
  {
    public MMEdEditorView(ThreeDeeEditor xiViewer, Scene xiScene, Camera xiCamera, AbstractRenderer xiRenderer) 
      : base(xiScene, xiCamera, xiRenderer)
    {
      mViewer = xiViewer;
    }

    protected override void RenderScene()
    {
      if (mCamera.ProjectionMode != eProjectionMode.Orthographic
        && mViewer.DrawNormalsMode == eDrawNormalsMode.DrawNormals)
      {
        mRenderer.RenderScene(mScene, mCamera, RenderOptions.ShowNormals);
      }
      else
      {
        base.RenderScene();
      }

      if (mActiveEntity == null)
      {
        foreach (Entity lEntity in mScene.Entities)
        {
          foreach (Mesh lMesh in lEntity.Meshes)
          {
            OwnedMesh lOm = lMesh as OwnedMesh;
            if (lOm != null && lOm.Owner == mViewer.ActiveObject)
            {
              mActiveEntity = lEntity;
              goto breakouter;
            }
          }
        }
        breakouter: ;
      }

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
        mRenderer.RenderSingleObject(lBox, RenderOptions.Default);
        mActiveEntity = null;
      }
    }

    private Entity mActiveEntity = null;
    private ThreeDeeEditor mViewer;
  }
}
