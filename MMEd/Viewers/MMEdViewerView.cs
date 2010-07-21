using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using GLTK;

using MMEd.Viewers.ThreeDee;

namespace MMEd.Viewers
{
  class MMEdViewerView : GLTK.View
  {
    public MMEdViewerView(ThreeDeeViewer xiViewer, Scene xiScene, Camera xiCamera, AbstractRenderer xiRenderer)
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
    }

    private ThreeDeeViewer mViewer;
  }
}
