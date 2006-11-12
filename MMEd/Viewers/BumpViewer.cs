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
    private BumpViewer(MainForm xiMainForm) : base(xiMainForm)
    {
      mMainForm.BumpViewPictureBox.Click += new System.EventHandler(this.BumpViewPictureBox_Click);
      mMainForm.BumpEditPictureBox.Click += new System.EventHandler(this.BumpEditPictureBox_Click);
      mMainForm.BumpEditFillButton.Click += new EventHandler(BumpEditFillButton_Click);
    }

    public override bool CanViewChunk(Chunk xiChunk)
    {
      return xiChunk is BumpImageChunk;
    }

    // Create an instance of the viewer manager class
    public static Viewer InitialiseViewer(MainForm xiMainForm)
    {
      return new BumpViewer(xiMainForm);
    }

    public override void SetSubject(Chunk xiChunk)
    {
      if (!(xiChunk is BumpImageChunk))
      {
        mChunk = null;
      }
      else
      {
        mChunk = (BumpImageChunk)xiChunk;
      }

      if (mLastSubject == mChunk)
      {
        return;
      }

      if (mChunk == null)
      {
        mMainForm.BumpEditPictureBox.Image = null;
      }
      else
      {
        byte lType = mChunk.GetPixelType(mX, mY);
        mMainForm.BumpTypeLabel.Text = GetBumpTypeName(lType);
        SetUpDropDown(lType);

        RefreshView();
      }

      mLastSubject = xiChunk;
    }

    private static string GetBumpTypeName(byte val)
    {
      if (BumpImageChunk.GetBumpTypeInfo(val) == null)
      {
        return string.Format("{0:x} (Unknown)", val);
      }
      else
      {
        return BumpImageChunk.GetBumpTypeInfo(val).Name;
      }
    }

    protected void RefreshView()
    {
      Bitmap lBmp = mChunk.ToBitmapUncached();
      
      // Outline the squares in black
      for (int x = 0; x < 8; x++)
      {
        for (int y = 0; y < 8 * mScale; y+=2)
        {
          lBmp.SetPixel(x * mScale, y, Color.Black);
          lBmp.SetPixel(x * mScale, y+1, Color.White);
      }
      }
      for (int y = 0; y < 8; y++)
      {
        for (int x = 0; x < 8 * mScale; x+=2)
        {
          lBmp.SetPixel(x, y * mScale, Color.Black);
          lBmp.SetPixel(x+1, y * mScale, Color.White);
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

      mMainForm.BumpEditPictureBox.Image = lBmp;
      mMainForm.BumpViewPictureBox.Image = lBmp;
    }

    public override System.Windows.Forms.TabPage Tab
    {
      get { return mMainForm.ViewTabBump; }
    }

    protected void SetUpDropDown(byte xiSelected)
    {
      for (byte b = 0; b < 50; b++)
      {
        mMainForm.BumpCombo.Items.Add(GetBumpTypeName(b));
      }

      mMainForm.BumpCombo.SelectedIndex = xiSelected;
    }

    public void BumpViewPictureBox_Click(object sender, EventArgs e)
    {
      MouseEventArgs lArgs = (MouseEventArgs)e;
      mX = lArgs.X / mScale;
      mY = lArgs.Y / mScale;

      byte lType = mChunk.GetPixelType(mX, mY);
      mMainForm.BumpTypeLabel.Text = GetBumpTypeName(lType);
      
      RefreshView();
    }

    public void BumpEditPictureBox_Click(object sender, EventArgs e)
    {
      MouseEventArgs lArgs = (MouseEventArgs)e;
      mX = lArgs.X / mScale;
      mY = lArgs.Y / mScale;

      byte lType = (byte)mMainForm.BumpCombo.SelectedIndex;
      mChunk.SetPixelType(mX, mY, lType);
      mMainForm.BumpTypeLabel.Text = GetBumpTypeName(lType);

      RefreshView();
    }

    public void BumpEditFillButton_Click(object sender, EventArgs e)
    {
      byte lType = (byte)mMainForm.BumpCombo.SelectedIndex;

      for (int x = 0; x < 8; x++)
      {
        for (int y = 0; y < 8; y++)
        {
          mChunk.SetPixelType(x, y, lType);
        }
      }

      mMainForm.BumpTypeLabel.Text = GetBumpTypeName(lType);
      RefreshView();
    }

    private int mScale = BumpImageChunk.SCALE;
    private BumpImageChunk mChunk;
    private Chunk mLastSubject = null;
    private int mX = 0;
    private int mY = 0;
  }
}
