using System;
using System.Collections.Generic;
using System.Text;
using MMEd;
using MMEd.Chunks;
using System.Drawing;
using System.Windows.Forms;

namespace MMEd.Viewers
{
  public class SteeringViewer : Viewer
  {
    private SteeringViewer(MainForm xiMainForm)
      : base(xiMainForm)
    {
      mMainForm.SteeringViewPictureBox.Click += new System.EventHandler(this.SteeringViewPictureBox_Click);
      mMainForm.SteeringEditPictureBox.Click += new System.EventHandler(this.SteeringEditPictureBox_Click);
      mMainForm.SteeringEditFillButton.Click += new EventHandler(SteeringEditFillButton_Click);
    }

    public override bool CanViewChunk(Chunk xiChunk)
    {
      return xiChunk is SteeringImageChunk;
    }

    // Create an instance of the viewer manager class
    public static Viewer InitialiseViewer(MainForm xiMainForm)
    {
      return new SteeringViewer(xiMainForm);
    }

    public override void SetSubject(Chunk xiChunk)
    {
      if (!(xiChunk is SteeringImageChunk))
      {
        mChunk = null;
      }
      else
      {
        mChunk = (SteeringImageChunk)xiChunk;
      }

      if (mLastSubject == mChunk)
      {
        return;
      }

      if (mChunk == null)
      {
        mMainForm.SteeringEditPictureBox.Image = null;
      }
      else
      {
        byte lType = mChunk.GetPixelType(mX, mY);
        mMainForm.SteeringTypeLabel.Text = GetSteeringDirectionName(lType);
        SetUpDropDown(lType);

        RefreshView();
      }

      mLastSubject = xiChunk;
    }

    private static string GetSteeringDirectionName(byte val)
    {
      return SteeringImageChunk.GetDirectionName(val);
    }

    protected void RefreshView()
    {
      Bitmap lBmp = mChunk.ToBitmapUncached(0);

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

      mMainForm.SteeringEditPictureBox.Image = lBmp;
      mMainForm.SteeringViewPictureBox.Image = lBmp;
    }

    public override System.Windows.Forms.TabPage Tab
    {
      get { return mMainForm.ViewTabSteering; }
    }

    protected void SetUpDropDown(byte xiSelected)
    {
      for (byte b = 0; b <= FlatChunk.STEERING_HIGHESTDIRECTION; b++)
      {
        mMainForm.SteeringCombo.Items.Add(GetSteeringDirectionName(b));
      }

      mMainForm.SteeringCombo.SelectedIndex = xiSelected;
    }

    public void SteeringViewPictureBox_Click(object sender, EventArgs e)
    {
      MouseEventArgs lArgs = (MouseEventArgs)e;
      mX = lArgs.X / mScale;
      mY = lArgs.Y / mScale;

      byte lType = mChunk.GetPixelType(mX, mY);
      mMainForm.SteeringTypeLabel.Text = GetSteeringDirectionName(lType);

      RefreshView();
    }

    public void SteeringEditPictureBox_Click(object sender, EventArgs e)
    {
      MouseEventArgs lArgs = (MouseEventArgs)e;
      mX = lArgs.X / mScale;
      mY = lArgs.Y / mScale;

      byte lType = (byte)mMainForm.SteeringCombo.SelectedIndex;
      mChunk.SetPixelType(mX, mY, lType);
      mMainForm.SteeringTypeLabel.Text = GetSteeringDirectionName(lType);

      RefreshView();
    }

    public void SteeringEditFillButton_Click(object sender, EventArgs e)
    {
      byte lType = (byte)mMainForm.SteeringCombo.SelectedIndex;

      for (int x = 0; x < 8; x++)
      {
        for (int y = 0; y < 8; y++)
        {
          mChunk.SetPixelType(x, y, lType);
        }
      }

      mMainForm.SteeringTypeLabel.Text = GetSteeringDirectionName(lType);
      RefreshView();
    }

    private int mScale = SteeringImageChunk.SCALE;
    private SteeringImageChunk mChunk;
    private Chunk mLastSubject = null;
    private int mX = 0;
    private int mY = 0;
  }
}
