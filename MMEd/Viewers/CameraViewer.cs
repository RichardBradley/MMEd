using System;
using System.Collections.Generic;
using System.Text;
using MMEd;
using MMEd.Chunks;
using System.Xml.Serialization;
using System.IO;
using System.Windows.Forms;

namespace MMEd.Viewers
{
  class CameraViewer : Viewer
  {
    private CameraViewer(MainForm xiMainForm)
      : base(xiMainForm)
    {
    }

    // Only CameraPosChunks can be viewed.
    public override bool CanViewChunk(Chunk xiChunk)
    {
      return xiChunk is CameraPosChunk;
    }

    // Create an instance of the viewer manager class
    public static Viewer InitialiseViewer(MainForm xiMainForm)
    {
      return new CameraViewer(xiMainForm);
    }

    private CameraPosChunk mSubject = null;

    public override void SetSubject(Chunk xiChunk)
    {
      if (mSubject == xiChunk || xiChunk == null) return;

      if (!(xiChunk is CameraPosChunk))
      {
        throw new InvalidOperationException("Tried to view chunk of type {0} in CameraViewer");
      }

      mSubject = (CameraPosChunk)xiChunk;
    }

    private void SetSliderDirection(TrackBar xiSlider, int xiNewValue)
    {
      xiSlider.Value = 100;
    }

    public override System.Windows.Forms.TabPage Tab
    {
      get { return mMainForm.ViewTabCamera; }
    }
  }
}
