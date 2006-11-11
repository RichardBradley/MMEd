using System;
using System.Collections.Generic;
using System.Text;
using MMEd;
using MMEd.Util;
using MMEd.Chunks;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;


namespace MMEd.Viewers
{
  public class GridViewer : Viewer
  {
    private GridViewer(MainForm xiMainForm)
      : base(xiMainForm)
    {
      mMainForm.GridDisplayPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.GridDisplayPanel_Paint);
      mMainForm.GridDisplayPanel.MouseMove += new MouseEventHandler(this.GridDisplayMouseMove);
      mMainForm.GridDisplayPanel.MouseClick += new MouseEventHandler(this.GridDisplayMouseClick);

      new PropertyController(this, "SelectedMeta").BindTo(mMainForm.GridViewMetaTypeCombo);
      new PropertyController(this, "ViewMode").BindTo(mMainForm.GridViewViewModeCombo);

      mMainForm.ViewerTabControl.KeyPress += new KeyPressEventHandler(this.ViewerTabControl_KeyPress);

      //set the transparency
      mMainForm.GridViewTransparencySlider.ValueChanged += new EventHandler(this.TransparencyLevelChange);
      TransparencyLevelChange(null, null);
    }

    private void GridDisplayPanel_Paint(object sender, PaintEventArgs e)
    {
      if (mSubject == null) return;

      // create GDI helper objects outside of the main loop, if we're
      // drawing numbers on later:
      int lNumberOffX = 0, lNumberOffY = 0;
      Font lNumberFont = null;
      Brush lNumberFGBrush = null, lNumberBGBrush = null;
      if (ViewMode == eViewMode.EditMetadata)
      {
        lNumberFont = new Font(FontFamily.GenericMonospace, 10);
        lNumberFGBrush = new SolidBrush(Color.Black);
        lNumberBGBrush = new SolidBrush(Color.White);
        lNumberOffX = mSubjectTileWidth / 2;
        lNumberOffY = mSubjectTileHeight / 2;
      }

      for (int x = (e.ClipRectangle.Left / mSubjectTileWidth);
          x < (e.ClipRectangle.Right / mSubjectTileWidth) + 1
          && x < mSubject.Width; x++)
      {
        for (int y = (e.ClipRectangle.Top / mSubjectTileHeight);
            y < (e.ClipRectangle.Bottom / mSubjectTileHeight) + 1
            && y < mSubject.Height; y++)
        {
          try
          {
            // Draw the main texture for the square.
            e.Graphics.DrawImageUnscaled(
                mMainForm.Level.GetTileById(mSubject.TextureIds[x][y]).ToBitmap(),
                x * mSubjectTileWidth,
                y * mSubjectTileHeight);

            if (ViewMode == eViewMode.ViewOdds)
            {
              Rectangle lDest = new Rectangle(
                  x * mSubjectTileWidth,
                  y * mSubjectTileHeight,
                  mSubjectTileWidth,
                  mSubjectTileHeight);
              OddImageChunk bic = mMainForm.Level.GetOddById(mSubject.TexMetaData[x][y][(int)SelectedMeta]);

              // If we don't have an odd to draw here, bic is null
              // and nothing is drawn - just leave the base texture as-is.
              if (bic != null)
              {
                DrawTransparentImage(e, bic.ToImage(), lDest);
              }
            }
            else if (ViewMode == eViewMode.EditBump || ViewMode == eViewMode.ViewBump)
            {
              // Draw the bump map on top as a transparent image.
              Rectangle lDest = new Rectangle(
                    x * mSubjectTileWidth,
                    y * mSubjectTileHeight,
                    mSubjectTileWidth,
                    mSubjectTileHeight);
              BumpImageChunk bic = mMainForm.Level.GetBumpById(mSubject.TexMetaData[x][y][(int)eTexMetaDataEntries.Bumpmap]);

              if (bic != null)
              {
                DrawTransparentImage(e, bic.ToImage(), lDest);
              }
            }

            if (ViewMode == eViewMode.EditMetadata)
            {
              string text = string.Format("{0:x}", mSubject.TexMetaData[x][y][(int)SelectedMeta]);

              SizeF size = e.Graphics.MeasureString(text, lNumberFont);

              float xf = x * mSubjectTileWidth + lNumberOffX - size.Width / 2;
              float yf = y * mSubjectTileHeight + lNumberOffY - size.Height / 2;

              e.Graphics.FillRectangle(lNumberBGBrush, xf, yf, size.Width, size.Height);

              e.Graphics.DrawString(
                  text,
                  lNumberFont,
                  lNumberFGBrush,
                  xf,
                  yf);
            }
          }
          catch (NullReferenceException err)
          {
            Console.Error.WriteLine(err);
          }
        }
      }

      //highlight editable square
      if (ViewMode == eViewMode.EditBump 
        || ViewMode == eViewMode.EditMetadata
        || ViewMode == eViewMode.EditTextures)
      {
        Point lMousePos = mMainForm.GridDisplayPanel.PointToClient(Cursor.Position);
        //assume graphics clip will take care of clipping
        e.Graphics.DrawRectangle(
            new Pen(Color.Red, 2.0f),
            lMousePos.X / mSubjectTileWidth * mSubjectTileWidth,
            lMousePos.Y / mSubjectTileHeight * mSubjectTileHeight,
            mSubjectTileWidth,
            mSubjectTileHeight);
      }
    }

    private void TransparencyLevelChange(object xiSender, EventArgs xiArgs)
    {
      float lAlpha = mMainForm.GridViewTransparencySlider.Value
        / (float)mMainForm.GridViewTransparencySlider.Maximum;

      // reset the mTransparencyAttributes matrix
      //
      // Define a colour matrix. This allows transformations
      // of the bitmap data using matrix operations. The
      // rows correspond to RGB + alpha and one other; here
      // the alpha is set to 50%.
      float[][] lMatrixDefinition = 
                  { 
                    new float[] {1, 0, 0, 0, 0},
                    new float[] {0, 1, 0, 0, 0},
                    new float[] {0, 0, 1, 0, 0},
                    new float[] {0, 0, 0, lAlpha, 0}, 
                    new float[] {0, 0, 0, 0, 1}
                  };
      ColorMatrix lMatrix = new ColorMatrix(lMatrixDefinition);
      mTransparencyAttributes.SetColorMatrix(lMatrix,
        ColorMatrixFlag.Default,
        ColorAdjustType.Bitmap);

      InvalidateGridDisplay();
    }

    private void DrawTransparentImage(PaintEventArgs e, Image xiImage, Rectangle xiDest)
    {
      e.Graphics.DrawImage(
        xiImage,
        xiDest,
        0, // These 5 params define which part of the source image to use - all of it.
        0,
        xiImage.Width,
        xiImage.Height,
        GraphicsUnit.Pixel,
        mTransparencyAttributes);
    }

    public override bool CanViewChunk(Chunk xiChunk)
    {
      return xiChunk is FlatChunk;
    }

    // Create an instance of the viewer manager class
    public static Viewer InitialiseViewer(MainForm xiMainForm)
    {
      return new GridViewer(xiMainForm);
    }

    private FlatChunk mSubject = null;
    int mSubjectTileWidth;
    int mSubjectTileHeight;


    public override void SetSubject(Chunk xiChunk)
    {

      if (!(xiChunk is FlatChunk)) xiChunk = null;
      if (mSubject == xiChunk) return;
      mSubject = (FlatChunk)xiChunk;

      if (xiChunk == null)
      {
        mMainForm.GridDisplayPanel.Width = 100;
        mMainForm.GridDisplayPanel.Height = 100;
        mMainForm.GridDisplayPanel.Controls.Clear();
      }
      else
      {
        //find the width and height of the tex components
        short topLeftTexIdx = mSubject.TextureIds[0][0];
        TIMChunk firstTim = mMainForm.Level.GetTileById(topLeftTexIdx);
        mSubjectTileHeight = firstTim.ImageHeight;
        mSubjectTileWidth = firstTim.ImageWidth;

        mMainForm.GridDisplayPanel.Width = mSubjectTileWidth * mSubject.Width;
        mMainForm.GridDisplayPanel.Height = mSubjectTileHeight * mSubject.Height;

        //init the selected image display boxes
        const int PADDING = 5, IMG_X_OFF = 32;
        object[] keys = new object[] { MouseButtons.Left, MouseButtons.Right, '1', '2', '3', '4', 'q', 'w', 'e', 'r' };
        mMainForm.GridViewSelPanel.Size = new Size(IMG_X_OFF + PADDING + 64, (64 + PADDING) * keys.Length);
        for (int i = 0; i < keys.Length; i++)
        {
          object key = keys[i];
          if (!mKeyOrMouseToSelPicBoxDict.ContainsKey(key))
          {
            Label lab = new Label();
            lab.Text = (MouseButtons.Left.Equals(key) ? "LMB"
                     : (MouseButtons.Right.Equals(key) ? "RMB" : key.ToString()));
            lab.AutoSize = true;
            mMainForm.GridViewSelPanel.Controls.Add(lab);
            mKeyOrMouseToLabelDict[key] = lab;
            PictureBox lPB = new PictureBox();
            mMainForm.GridViewSelPanel.Controls.Add(lPB);
            lPB.Size = new Size(64, 64);
            lPB.SizeMode = PictureBoxSizeMode.StretchImage;
            mKeyOrMouseToSelPicBoxDict[key] = lPB;
          }
          mKeyOrMouseToLabelDict[key].Location = new Point(0, (int)((i + 0.5) * (64 + PADDING) - 5));
          mKeyOrMouseToSelPicBoxDict[key].Location = new Point(IMG_X_OFF, i * (64 + PADDING));
        }
      }
      ViewMode = eViewMode.ViewOnly;
      ModeChanged(null, null);
    }

    public override System.Windows.Forms.TabPage Tab
    {
      get { return mMainForm.ViewTabGrid; }
    }

    private void InvalidateGridDisplayEvent(object sender, EventArgs e)
    {
      InvalidateGridDisplay();
    }

    private void InvalidateGridDisplay()
    {
      mMainForm.GridDisplayPanel.Invalidate();
    }

    private void GridDisplayMouseMove(object sender, MouseEventArgs e)
    {
      if (mSubject != null)
      {
        double x = e.X / (double)mSubjectTileWidth;
        double y = e.Y / (double)mSubjectTileHeight;

        string lWorldCoord = "Only available for non-rotated sheets";
        if (mSubject.RotationVector.Norm() == 0)
        {
          lWorldCoord = string.Format("World Coord: ({0:0}, {1:0}, {2:0})",
          x * mSubject.ScaleX + mSubject.OriginPosition.X,
          y * mSubject.ScaleY + mSubject.OriginPosition.Y,
          mSubject.OriginPosition.Z);
        }

        mMainForm.GridViewerStatusLabel.Text = string.Format(
          "Tex Coord: ({0:0}, {1:0}) Flat Coord: ({2:0}, {3:0}) {4}",
          x, y, x * mSubject.ScaleX, y * mSubject.ScaleY,
          lWorldCoord);

        //this seems a bit ott, but is needed for the red square highlight
        InvalidateGridDisplay();
      }
    }

    private void ModeChanged(object sender, EventArgs e)
    {
    }

    void PaletteImageMouseClick(object sender, MouseEventArgs e)
    {
      SetSelImage(e.Button, (PictureBox)sender);
    }

    private void GridDisplayMouseClick(object sender, MouseEventArgs e)
    {
      if (mSubject != null)
      {
        int x = e.X / mSubjectTileWidth;
        int y = e.Y / mSubjectTileHeight;
        if (ViewMode == eViewMode.EditMetadata)
        {
          //edit meta data mode
          if (mSubject.TexMetaData != null
             && x < mSubject.TexMetaData.Length
             && y < mSubject.TexMetaData[x].Length)
          {
            short val;

            val = mSubject.TexMetaData[x][y][(int)SelectedMeta];

            string lReply = Microsoft.VisualBasic.Interaction.InputBox(
              string.Format("Value at ({0},{1}) is {2} (0x{2:x}). New value:", x, y, val),
              "MMEd",
              val.ToString(), //Default value
              -1, //X coord
              -1);// Y coord

            if (lReply != null && lReply != "")
            {
              val = short.Parse(lReply);

              mSubject.TexMetaData[x][y][(int)SelectedMeta] = (byte)val;

              InvalidateGridDisplay();
            }
          }
        }
        else if (ViewMode == eViewMode.EditTextures)
        {
          //edit textures mode
          if (mKeyOrMouseToSelPicBoxDict.ContainsKey(e.Button))
          {
            PictureBox lSel = mKeyOrMouseToSelPicBoxDict[e.Button];
            mSubject.TextureIds[x][y] = (byte)lSel.Tag;
            InvalidateGridDisplay();
          }
        }
        else if (ViewMode == eViewMode.EditBump)
        {
          //edit bump mode
          if (mKeyOrMouseToSelPicBoxDict.ContainsKey(e.Button))
          {
            PictureBox lSel = mKeyOrMouseToSelPicBoxDict[e.Button];
            mSubject.TexMetaData[x][y][(int)eTexMetaDataEntries.Bumpmap] = (byte)lSel.Tag;
            InvalidateGridDisplay();
          }
        }
      }
    }

    Dictionary<object, PictureBox> mKeyOrMouseToSelPicBoxDict = new Dictionary<object, PictureBox>();
    Dictionary<object, Label> mKeyOrMouseToLabelDict = new Dictionary<object, Label>();
    private void SetSelImage(object xiKey, PictureBox xiNewVal)
    {
      if (mKeyOrMouseToSelPicBoxDict.ContainsKey(xiKey))
      {
        PictureBox lSel = mKeyOrMouseToSelPicBoxDict[xiKey];
        lSel.Tag = xiNewVal.Tag;
        lSel.Image = xiNewVal.Image;
        lSel.Size = new Size(64, 64);
      }
    }

    private void ViewerTabControl_KeyPress(object sender, KeyPressEventArgs e)
    {
      if (mMainForm.ViewerTabControl.SelectedTab == this.Tab
         && (ViewMode == eViewMode.EditTextures || ViewMode == eViewMode.EditBump)
         && mKeyOrMouseToSelPicBoxDict.ContainsKey(e.KeyChar))
      {
        //drill down the child heirarchy
        Point lMousePt = Cursor.Position;
        Control c = this.Tab;
        bool lIsPressOnGridViewPalettePanel = false;
        while (c != null)
        {
          if (c is PictureBox)
          {
            //the target of the key press.
            if (lIsPressOnGridViewPalettePanel)
            {
              SetSelImage(e.KeyChar, (PictureBox)c);
              e.Handled = true;
              return;
            }
          }
          else if (c == mMainForm.GridViewPalettePanel)
          {
            lIsPressOnGridViewPalettePanel = true;
          }
          else if (c == mMainForm.GridDisplayPanel)
          {
            Point p = mMainForm.GridDisplayPanel.PointToClient(lMousePt);
            int x = p.X / mSubjectTileWidth;
            int y = p.Y / mSubjectTileHeight;
            PictureBox lSel = mKeyOrMouseToSelPicBoxDict[e.KeyChar];
            if (ViewMode == eViewMode.EditTextures)
            {
              mSubject.TextureIds[x][y] = (byte)lSel.Tag;
            }
            else if (ViewMode == eViewMode.EditBump)
            {
              mSubject.TexMetaData[x][y][(int)eTexMetaDataEntries.Bumpmap] = (byte)lSel.Tag;
            }
            InvalidateGridDisplay();
            return;
          }

          Point lClientPt = c.PointToClient(lMousePt);
          c = c.GetChildAtPoint(lClientPt);
        }
      }
    }

    #region ViewMode property

    public enum eViewMode
    {
      ViewOnly,
      ViewBump,
      ViewOdds,
      EditTextures,
      EditBump,
      EditMetadata
    }

    private eViewMode mViewMode = eViewMode.ViewOnly;
    public eViewMode ViewMode
    {
      get { return mViewMode; }
      set
      {
        const int PADDING = 5;
        mViewMode = value;
        if (ViewMode == eViewMode.EditTextures || ViewMode == eViewMode.EditBump)
        {
          mMainForm.GridViewPalettePanel.SuspendLayout();
          mMainForm.GridViewPalettePanel.Controls.Clear();
          Point lNextPbTL = new Point(0, 0);
          IEnumerator<object> lKeys = mKeyOrMouseToSelPicBoxDict.Keys.GetEnumerator();
          PictureBox lPalPB = null;
          for (int i = 0; i < 256; i++)
          {
            //fetch im
            Chunk c = (ViewMode == eViewMode.EditTextures)
              ? (Chunk)mMainForm.Level.GetTileById(i)
              : (Chunk)mMainForm.Level.GetBumpById(i);

            if (c != null && c is ImageViewer.IImageProvider)
            {
              Image im = ((ImageViewer.IImageProvider)c).ToImage();
              if (ViewMode == eViewMode.EditBump ||
                 (im.Width == this.mSubjectTileWidth
                   && im.Height == this.mSubjectTileHeight))
              {
                lPalPB = new PictureBox();
                lPalPB.Image = im;
                mMainForm.GridViewPalettePanel.Size =
                  new Size(lNextPbTL.X + 64 + PADDING,
                           lNextPbTL.Y + 64);
                mMainForm.GridViewPalettePanel.Controls.Add(lPalPB);
                lPalPB.Bounds = new Rectangle(lNextPbTL, new Size(64, 64));
                lPalPB.SizeMode = PictureBoxSizeMode.StretchImage;
                lPalPB.Tag = (byte)i;
                lPalPB.MouseClick += new MouseEventHandler(PaletteImageMouseClick);
                lNextPbTL.Offset(64 + PADDING, 0);
                if (lKeys.MoveNext())
                {
                  SetSelImage(lKeys.Current, lPalPB);
                }
              }
            }
          }
          while (lKeys.MoveNext())
          {
            SetSelImage(lKeys.Current, lPalPB);
          }
          mMainForm.GridViewPalettePanel.ResumeLayout();
          mMainForm.GridViewSelImageGroupBox.Visible = true;
        }
        else //not edit tex or edit bump
        {
          mMainForm.GridViewSelImageGroupBox.Visible = false;
        }

        if (OnViewModeChange != null) OnViewModeChange(this, null);
        InvalidateGridDisplay();
      }
    }
    public event EventHandler OnViewModeChange;

    #endregion

    #region SelectedMeta property

    private eTexMetaDataEntries mSelectedMeta = eTexMetaDataEntries.Waypoint;
    public eTexMetaDataEntries SelectedMeta
    {
      get { return mSelectedMeta; }
      set
      {
        mSelectedMeta = value;
        if (OnSelectedMetaChange != null) OnSelectedMetaChange(this, null);
        InvalidateGridDisplay();
      }
    }
    public event EventHandler OnSelectedMetaChange;

    #endregion

    private ImageAttributes mTransparencyAttributes = new ImageAttributes();
  }
}

