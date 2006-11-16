using System;
using System.Collections.Generic;
using System.Text;
using MMEd;
using MMEd.Chunks;
using System.Drawing;
using System.Windows.Forms;

namespace MMEd.Viewers
{
  public class OddViewer : Viewer
  {
    private OddViewer(MainForm xiMainForm)
      : base(xiMainForm)
    {
      mMainForm.OddViewPictureBox.Click += new System.EventHandler(this.OddViewPictureBox_Click);
      mMainForm.OddEditPictureBox.Click += new System.EventHandler(this.OddEditPictureBox_Click);
      mMainForm.OddEditFillButton.Click += new EventHandler(OddEditFillButton_Click);
    }

    public override bool CanViewChunk(Chunk xiChunk)
    {
      return xiChunk is OddImageChunk;
    }

    // Create an instance of the viewer manager class
    public static Viewer InitialiseViewer(MainForm xiMainForm)
    {
      return new OddViewer(xiMainForm);
    }

    public override void SetSubject(Chunk xiChunk)
    {
      if (!(xiChunk is OddImageChunk))
      {
        mChunk = null;
      }
      else
      {
        mChunk = (OddImageChunk)xiChunk;
      }

      if (mLastSubject == mChunk)
      {
        return;
      }

      if (mChunk == null)
      {
        mMainForm.OddEditPictureBox.Image = null;
      }
      else
      {
        byte lType = mChunk.GetPixelType(mX, mY);
        mMainForm.OddTypeLabel.Text = GetOddTypeName(lType);
        SetUpDropDown(lType);

        RefreshView();
      }

      mLastSubject = xiChunk;
    }

    private static string GetOddTypeName(byte val)
    {
      if (OddImageChunk.GetOddTypeInfo(val) == null)
      {
        return string.Format("{0:x} (Unknown)", val);
      }
      else
      {
        return OddImageChunk.GetOddTypeInfo(val).Name;
      }
    }

    protected void RefreshView()
    {
      Bitmap lBmp = mChunk.ToBitmapUncached();

      // Outline the squares in black
      for (int x = 0; x < 8; x++)
      {
        for (int y = 0; y < 8 * mScale; y += 2)
        {
          lBmp.SetPixel(x * mScale, y, Color.Black);
          lBmp.SetPixel(x * mScale, y + 1, Color.White);
        }
      }
      for (int y = 0; y < 8; y++)
      {
        for (int x = 0; x < 8 * mScale; x += 2)
        {
          lBmp.SetPixel(x, y * mScale, Color.Black);
          lBmp.SetPixel(x + 1, y * mScale, Color.White);
        }
      }

      // Outline selected pixel in white
      for (int x = mX * mScale; x < (mX + 1) * mScale; ++x)
      {
        lBmp.SetPixel(x, mY * mScale, Color.White);
        lBmp.SetPixel(x, ((mY + 1) * mScale) - 1, Color.White);
      }
      for (int y = mY * mScale; y < (mY + 1) * mScale; ++y)
      {
        lBmp.SetPixel(mX * mScale, y, Color.White);
        lBmp.SetPixel(((mX + 1) * mScale) - 1, y, Color.White);
      }

      mMainForm.OddEditPictureBox.Image = lBmp;
      mMainForm.OddViewPictureBox.Image = lBmp;
    }

    public override System.Windows.Forms.TabPage Tab
    {
      get { return mMainForm.ViewTabOdd; }
    }

    protected void SetUpDropDown(byte xiSelected)
    {
      for (byte b = 0; b < 50; b++)
      {
        mMainForm.OddCombo.Items.Add(GetOddTypeName(b));
      }

      mMainForm.OddCombo.SelectedIndex = xiSelected;
    }

    public void OddViewPictureBox_Click(object sender, EventArgs e)
    {
      MouseEventArgs lArgs = (MouseEventArgs)e;
      mX = lArgs.X / mScale;
      mY = lArgs.Y / mScale;

      byte lType = mChunk.GetPixelType(mX, mY);
      mMainForm.OddTypeLabel.Text = GetOddTypeName(lType);

      RefreshView();
    }

    public void OddEditPictureBox_Click(object sender, EventArgs e)
    {
      MouseEventArgs lArgs = (MouseEventArgs)e;
      mX = lArgs.X / mScale;
      mY = lArgs.Y / mScale;

      byte lType = (byte)mMainForm.OddCombo.SelectedIndex;
      mChunk.SetPixelType(mX, mY, lType);
      mMainForm.OddTypeLabel.Text = GetOddTypeName(lType);

      RefreshView();
    }

    public void OddEditFillButton_Click(object sender, EventArgs e)
    {
      byte lType = (byte)mMainForm.OddCombo.SelectedIndex;

      for (int x = 0; x < 8; x++)
      {
        for (int y = 0; y < 8; y++)
        {
          mChunk.SetPixelType(x, y, lType);
        }
      }

      mMainForm.OddTypeLabel.Text = GetOddTypeName(lType);
      RefreshView();
    }

    private int mScale = OddImageChunk.SCALE;
    private OddImageChunk mChunk;
    private Chunk mLastSubject = null;
    private int mX = 0;
    private int mY = 0;
  }
}
