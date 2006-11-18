using System;
using System.Collections.Generic;
using System.Drawing;
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
      xiMainForm.SliderDirection.ValueChanged += new EventHandler(SliderDirection_ValueChanged);
      xiMainForm.SliderDistance.ValueChanged += new EventHandler(SliderDistance_ValueChanged);
      xiMainForm.SliderElevation.ValueChanged += new EventHandler(SliderElevation_ValueChanged);
      xiMainForm.TextDirection.TextChanged += new EventHandler(TextDirection_TextChanged);
      xiMainForm.TextDistance.TextChanged += new EventHandler(TextDistance_TextChanged);
      xiMainForm.TextElevation.TextChanged += new EventHandler(TextElevation_TextChanged);
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
      SetDirection(mSubject.Direction);
      mMainForm.TextDistance.Text = mSubject.Distance.ToString();
      mMainForm.TextElevation.Text = mSubject.Elevation.ToString();
    }

    private void SetDirection(int xiValue)
    {
      if (mUpdating) return;

      mUpdating = true;

      int lSliderValue =
        (xiValue / (2048 / mMainForm.SliderDirection.Maximum)) % 
        (mMainForm.SliderDirection.Maximum * 2);

      if (lSliderValue > mMainForm.SliderDirection.Maximum)
      {
        lSliderValue += (mMainForm.SliderDirection.Minimum * 2);
      }

      if (lSliderValue < mMainForm.SliderDirection.Minimum)
      {
        lSliderValue += (mMainForm.SliderDirection.Maximum * 2);
      }

      mMainForm.SliderDirection.Value = lSliderValue;
      mMainForm.TextDirection.Text = xiValue.ToString();
      mSubject.Direction = (short)xiValue;
      UpdateCameraImage();
      mUpdating = false;
    }

    private void SetDistance(int xiValue)
    {
      if (mUpdating) return;

      mUpdating = true;
      int lSliderValue = (xiValue / (mArbitraryMaximum / mMainForm.SliderDistance.Maximum));
      lSliderValue = Math.Min(mMainForm.SliderDistance.Maximum, lSliderValue);
      lSliderValue = Math.Max(mMainForm.SliderDistance.Minimum, lSliderValue);
      mMainForm.SliderDistance.Value = lSliderValue;
      mMainForm.TextDistance.Text = xiValue.ToString();
      mSubject.Distance = (short)xiValue;
      UpdateCameraImage();
      mUpdating = false;
    }

    private void SetElevation(int xiValue)
    {
      if (mUpdating) return;

      mUpdating = true;
      int lSliderValue = (xiValue / (mArbitraryMaximum / mMainForm.SliderElevation.Maximum));
      lSliderValue = Math.Min(mMainForm.SliderElevation.Maximum, lSliderValue);
      lSliderValue = Math.Max(mMainForm.SliderElevation.Minimum, lSliderValue);
      mMainForm.SliderElevation.Value = lSliderValue;
      mMainForm.TextElevation.Text = xiValue.ToString();
      mSubject.Elevation = (short)xiValue;
      UpdateCameraImage();
      mUpdating = false;
    }

    private void UpdateCameraImage()
    {
      Panel lPanel = mMainForm.PanelCameraImage;
      Bitmap lImage = new Bitmap(lPanel.Width, lPanel.Height);
      Graphics g = Graphics.FromImage(lImage);
      g.FillRectangle(
        new SolidBrush(Color.White), 
        0, 
        0, 
        lPanel.Width, 
        lPanel.Height);
      mSubject.Draw(
        g,
        new Pen(Color.Black),
        new Point(lPanel.Width / 2, lPanel.Height / 2),
        lPanel.Width);
      lPanel.BackgroundImage = lImage;
    }

    private void SetSliderDirection(TrackBar xiSlider, int xiNewValue)
    {
      xiSlider.Value = xiNewValue;
    }

    public override System.Windows.Forms.TabPage Tab
    {
      get { return mMainForm.ViewTabCamera; }
    }

    private int ParseValue(string xiValue)
    {
      if (xiValue == null || xiValue == "")
      {
        return int.MinValue;
      }

      try
      {
        return int.Parse(xiValue);
      }
      catch
      {
        return int.MinValue;
      }
    }

    void TextDirection_TextChanged(object sender, EventArgs e)
    {
      int lNewValue = ParseValue(mMainForm.TextDirection.Text);

      if (lNewValue == int.MinValue)
      {
        return;
      }

      SetDirection(lNewValue);
    }

    void TextDistance_TextChanged(object sender, EventArgs e)
    {
      int lNewValue = ParseValue(mMainForm.TextDistance.Text);

      if (lNewValue == int.MinValue)
      {
        return;
      }

      SetDistance(lNewValue);
    }

    void TextElevation_TextChanged(object sender, EventArgs e)
    {
      int lNewValue = ParseValue(mMainForm.TextElevation.Text);

      if (lNewValue == int.MinValue)
      {
        return;
      }

      SetElevation(lNewValue);
    }

    void SliderDirection_ValueChanged(object sender, EventArgs e)
    {
      SetDirection(mMainForm.SliderDirection.Value * 2048 / mMainForm.SliderDirection.Maximum);
    }

    void SliderDistance_ValueChanged(object sender, EventArgs e)
    {
      SetDistance(mMainForm.SliderDistance.Value * (mArbitraryMaximum / mMainForm.SliderDistance.Maximum));
    }

    void SliderElevation_ValueChanged(object sender, EventArgs e)
    {
      SetElevation(mMainForm.SliderElevation.Value * (mArbitraryMaximum / mMainForm.SliderElevation.Maximum));
    }

    private bool mUpdating = false;
    private static int mArbitraryMaximum = 1000;
  }
}
