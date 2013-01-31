using System;
using System.Collections.Generic;
using System.Text;
using MMEd;
using MMEd.Chunks;
using System.Drawing;
using System.Windows.Forms;

namespace MMEd.Viewers
{
  public class ImageViewer : Viewer
  {
    public interface IImageProvider
    {
      Image ToImage();
    }

    private ImageViewer(MainForm xiMainForm) : base(xiMainForm) { }

    public override bool CanViewChunk(Chunk xiChunk)
    {
      return xiChunk is IImageProvider;
    }

    // Create an instance of the viewer manager class
    public static Viewer InitialiseViewer(MainForm xiMainForm)
    {
      return new ImageViewer(xiMainForm);
    }

    private Chunk mLastSubject = null;

    public override void SetSubject(Chunk xiChunk)
    {
      if (!(xiChunk is IImageProvider)) xiChunk = null;
      if (mLastSubject == xiChunk) return;
      if (xiChunk == null)
      {
        mMainForm.ImgPictureBox.Image = null;
      }
      else
      {
        Image im = ((IImageProvider)xiChunk).ToImage();
        mMainForm.ImgPictureBox.Image = im;
        if (im != null)
        {
          int scaleFactor =
            Math.Max(1, 128/Math.Max(Math.Max(im.Width, im.Height), 1));
          if (scaleFactor != 1)
          {
            mMainForm.ImgPictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
            mMainForm.ImgPictureBox.Width = im.Width*scaleFactor;
            mMainForm.ImgPictureBox.Height = im.Height*scaleFactor;
          }
          else
          {
            mMainForm.ImgPictureBox.SizeMode = PictureBoxSizeMode.AutoSize;
          }
        }
      }
      mLastSubject = xiChunk;
    }

    public override System.Windows.Forms.TabPage Tab
    {
      get { return mMainForm.ViewTabImg; }
    }
  }
}
