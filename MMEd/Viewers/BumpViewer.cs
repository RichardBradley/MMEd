using System;
using System.Collections.Generic;
using System.Text;
using MMEd;
using MMEd.Chunks;
using System.Drawing;
using System.Windows.Forms;

namespace MMEd.Viewers
{
  public class BumpViewer : Viewer
  {
    private BumpViewer(MainForm xiMainForm) : base(xiMainForm) { }

    public override bool CanViewChunk(Chunk xiChunk)
    {
      return xiChunk is BumpImageChunk;
    }

    // Create an instance of the viewer manager class
    public static Viewer InitialiseViewer(MainForm xiMainForm)
    {
      return new BumpViewer(xiMainForm);
    }

    private Chunk mLastSubject = null;

    public override void SetSubject(Chunk xiChunk)
    {
      if (!(xiChunk is BumpImageChunk)) xiChunk = null;
      if (mLastSubject == xiChunk) return;
      if (xiChunk == null)
      {
        mMainForm.BumpPictureBox.Image = null;
      }
      else
      {
        BumpImageChunk lBump = (BumpImageChunk)xiChunk;
        Image im = lBump.ToImage();
        mMainForm.BumpPictureBox.Image = im;
        int scaleFactor =
            Math.Max(1, 128 / Math.Max(Math.Max(im.Width, im.Height), 1));
        if (scaleFactor != 1)
        {
          mMainForm.BumpPictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
          mMainForm.BumpPictureBox.Width = im.Width * scaleFactor;
          mMainForm.BumpPictureBox.Height = im.Height * scaleFactor;
        }
        else
        {
          mMainForm.BumpPictureBox.SizeMode = PictureBoxSizeMode.AutoSize;
        }

        mMainForm.BumpCombo.Text = lBump.GetInfo(0, 0).Description;
      }
      mLastSubject = xiChunk;
    }

    public override System.Windows.Forms.TabPage Tab
    {
      get { return mMainForm.ViewTabBump; }
    }
  }
}
