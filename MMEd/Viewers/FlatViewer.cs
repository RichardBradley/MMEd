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
  ///==========================================================================
  ///  Class : FlatViewer
  /// 
  /// <summary>
  /// 	Viewer for raw Flat information. Use Grid and 3D viewers to actually 
  ///   change the tiles on the Flat, but this viewer will let you edit the
  ///   flat's position and size, and add/remove objects and weapons
  ///   (TODO - MWR is currently working on that last bit...)
  /// </summary>
  ///==========================================================================
  class FlatViewer : Viewer
  {
    private FlatViewer(MainForm xiMainForm)
      : base(xiMainForm)
    {
      mMainForm.FlatViewerCommitBtn.Click += new System.EventHandler(this.CommitButton_Click);
    }

    public void CommitButton_Click(object xiSender, EventArgs xiArgs)
    {
      if (mSubject == null)
      {
        MessageBox.Show("Can't do that -- mSubject is null!");
        return;
      }

      //=======================================================================
      // Save simple values
      //=======================================================================
      mSubject.DeclaredName = Panel.NameTextBox.Text;
      mSubject.OriginPosition.X = short.Parse(Panel.OriginXTextBox.Text);
      mSubject.OriginPosition.Y = short.Parse(Panel.OriginYTextBox.Text);
      mSubject.OriginPosition.Z = short.Parse(Panel.OriginZTextBox.Text);
      mSubject.RotationVector.X = short.Parse(Panel.RotationXTextBox.Text);
      mSubject.RotationVector.Y = short.Parse(Panel.RotationYTextBox.Text);
      mSubject.RotationVector.Z = short.Parse(Panel.RotationZTextBox.Text);
      mSubject.ScaleX = short.Parse(Panel.ScaleXTextBox.Text);
      mSubject.ScaleY = short.Parse(Panel.ScaleYTextBox.Text);

      //=======================================================================
      // Change width and height, if appropriate
      //=======================================================================
      short lNewWidth = short.Parse(Panel.WidthTextBox.Text);
      short lNewHeight = short.Parse(Panel.HeightTextBox.Text);

      if (mSubject.Width != lNewWidth || mSubject.Height != lNewHeight)
      {
        int lSizeIncrease = mSubject.Resize(lNewWidth, lNewHeight);
        mMainForm.Level.SHET.TrailingZeroByteCount -= lSizeIncrease;

        if (mMainForm.Level.SHET.TrailingZeroByteCount < 0)
        {
          MessageBox.Show("WARNING: You do not currently have enough spare space at the end of your level file. " +
            "You will need to remove some data from the file before you can save to disk.", 
            "Resizing Flat", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
      }
    }

    // FlatChunks can be viewed
    public override bool CanViewChunk(Chunk xiChunk)
    {
      return xiChunk is FlatChunk;
    }

    // Create an instance of the viewer manager class
    public static Viewer InitialiseViewer(MainForm xiMainForm)
    {
      return new FlatViewer(xiMainForm);
    }

    private FlatChunk mSubject = null;

    public override void SetSubject(Chunk xiChunk)
    {
      if (mSubject == xiChunk) return;

      mSubject = xiChunk as FlatChunk;

      if (mSubject == null)
      {
        //=====================================================================
        // Clear all the controls
        //=====================================================================
        foreach (Control lControl in Panel.Controls)
        {
          TextBox lTextBox = lControl as TextBox;
          if (lTextBox != null)
          {
            lTextBox.Text = "";
          }
        }
      }
      else
      {
        //=====================================================================
        // Set all the controls to the correct value
        //=====================================================================
        Panel.IdTextBox.Text = mSubject.DeclaredIdx.ToString();
        Panel.NameTextBox.Text = mSubject.DeclaredName;
        Panel.OriginXTextBox.Text = mSubject.OriginPosition.X.ToString();
        Panel.OriginYTextBox.Text = mSubject.OriginPosition.Y.ToString();
        Panel.OriginZTextBox.Text = mSubject.OriginPosition.Z.ToString();
        Panel.RotationXTextBox.Text = mSubject.RotationVector.X.ToString();
        Panel.RotationYTextBox.Text = mSubject.RotationVector.Y.ToString();
        Panel.RotationZTextBox.Text = mSubject.RotationVector.Z.ToString();
        Panel.WidthTextBox.Text = mSubject.Width.ToString();
        Panel.HeightTextBox.Text = mSubject.Height.ToString();
        Panel.ScaleXTextBox.Text = mSubject.ScaleX.ToString();
        Panel.ScaleYTextBox.Text = mSubject.ScaleY.ToString();
      }
    }

    public override System.Windows.Forms.TabPage Tab
    {
      get { return mMainForm.ViewTabFlat; }
    }

    protected Util.FlatEditorPanel Panel
    {
      get { return mMainForm.FlatPanel; }
    }
  }
}
