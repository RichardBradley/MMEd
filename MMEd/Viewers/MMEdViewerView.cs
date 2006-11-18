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

    protected override void RenderObject(GLTK.Entity xiObject, RenderOptions xiOptions)
    {
      if (mCamera.ProjectionMode != eProjectionMode.Orthographic
        && mViewer.DrawNormalsMode == eDrawNormalsMode.DrawNormals)
      {
        xiOptions |= RenderOptions.ShowNormals;
      }
      base.RenderObject(xiObject, xiOptions);
    }

    private ThreeDeeViewer mViewer;
  }
}
