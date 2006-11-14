using System;
using System.Collections.Generic;
using System.Text;
using MMEd;
using MMEd.Util;
using MMEd.Chunks;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using GLTK;
using Point = System.Drawing.Point;

// Views and edits the 2D data in the Flats

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
      new PropertyController(this, "ShowObjects").BindTo(mMainForm.GridViewShowObjectsCheck);

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

      // main drawing loop; iterate over tex squares:
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
            // Draw the main texture for the square (all views)
            e.Graphics.DrawImageUnscaled(
                mMainForm.Level.GetTileById(mSubject.TextureIds[x][y]).ToBitmap(),
                x * mSubjectTileWidth,
                y * mSubjectTileHeight);

            switch (ViewMode)
            {
              case eViewMode.ViewOdds:
                Rectangle lOddDest = new Rectangle(
                    x * mSubjectTileWidth,
                    y * mSubjectTileHeight,
                    mSubjectTileWidth,
                    mSubjectTileHeight);
                OddImageChunk oic = mMainForm.Level.GetOddById(mSubject.TexMetaData[x][y][(int)SelectedMeta]);

                // If we don't have an odd to draw here, bic is null
                // and nothing is drawn - just leave the base texture as-is.
                if (oic != null)
                {
                  DrawTransparentImage(e, oic.ToImage(), lOddDest);
                }
                break;

              case eViewMode.ViewBump:
              case eViewMode.EditBumpSquares:
              case eViewMode.EditBumpPixels:
                // Draw the bump map on top as a transparent image.
                Rectangle lBumpDest = new Rectangle(
                    x * mSubjectTileWidth,
                    y * mSubjectTileHeight,
                    mSubjectTileWidth,
                    mSubjectTileHeight);
                BumpImageChunk bic = mMainForm.Level.GetBumpById(mSubject.TexMetaData[x][y][(int)eTexMetaDataEntries.Bumpmap]);

                if (bic != null)
                {
                  DrawTransparentImage(e, bic.ToImage(), lBumpDest);
                }
                break;

              case eViewMode.EditMetadata:
                //draw the selected metadata on as text
                string text = string.Format("{0}", mSubject.TexMetaData[x][y][(int)SelectedMeta]);

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
                break;
            }
          }
          catch (NullReferenceException err)
          {
            Console.Error.WriteLine(err);
          }
        }
      }

      //draw objects?
      if (ShowObjects)
      {
        if (mWireFrameCache == null)
        {
          mWireFrameCache = new List<ColoredPolygon>();

          IEnumerable<Entity> lScene
            = mSubject.GetEntities(mMainForm.Level, MMEd.Viewers.ThreeDee.eTextureMode.WireFrame, eTexMetaDataEntries.Zero);

          //find the entity correspoding to mSubject
          Entity lSubjectsEntity = null;
          foreach (Entity ent in lScene)
          {
            if (ent is MMEdEntity && ((MMEdEntity)ent).Owner == mSubject)
            {
              lSubjectsEntity = ent;
              break;
            }
          }

          //the subject should always have an entity:
          if (lSubjectsEntity == null) throw new Exception("The selected subject is non-null, but it has no corresponding entity in the scene");

          //there are four co-ord systems at play:
          // 1) World coords (i.e. level coords). This is the coo-ords the entities will be in
          // 2) Simple Flat coords. This is the coord space the Flat was created in, in which
          //    the tex squares are at integer coords from (0,0) to (mSubject.Width, mSubject.Height)
          // 3) Flat coords. This is the simple flat coords scaled up by (mSubject.ScaleX, mSubject.ScaleY)
          //    These are the coords which appear on the status bar as "Flat Coord"
          // 4) Paint coords. This is the coord space in which the flat is painted onto the grid, which has
          //    the tex squares at integer coords from (0,0) 
          //    to (mSubject.Width+*mSubjectTileWidth, mSubject.Height*mSubjectTileHeight)
          //
          // The objects are defined in World coords (1), and the GLTK display matrix for the flat is (2) -> (1)
          // So we can go from (1) -> (2) -> (4) without needing to bother with (3)

          Matrix lMapWorldCoordsToSimpleFlatCoords = lSubjectsEntity.Transform.Inverse();

          Matrix lMapWorldCoordsToPaintCoords = lMapWorldCoordsToSimpleFlatCoords
            * Matrix.ScalingMatrix(mSubjectTileWidth, mSubjectTileHeight, 1);

          //iterate over the objects in the scene, and transform them into Flat co-ords
          //then draw the faces in wireframe
          foreach (Entity ent in lScene)
          {
            if (ent is MMEdEntity && ((MMEdEntity)ent).Owner is FlatChunk.ObjectEntry)
            {
              foreach (Mesh m in ent.Meshes)
              {
                for (int lFaceStart = 0; lFaceStart < m.Vertices.Count; lFaceStart += (int)m.PolygonMode)
                {
                  Point[] lPointBuff = new Point[(int)m.PolygonMode];

                  for (int v = 0; v < (int)m.PolygonMode; v++)
                  {
                    GLTK.Point lVertexInWorldCoords = m.Vertices[lFaceStart + v].Position
                      * ent.Transform;

                    GLTK.Point lVertexInPaintCoords = lVertexInWorldCoords
                      * lMapWorldCoordsToPaintCoords;

                    lPointBuff[v] = new Point((int)lVertexInPaintCoords.x, (int)lVertexInPaintCoords.y);
                  }

                  //don't bother blending the colours, just pick the colour from 
                  // the first vertex
                  mWireFrameCache.Add(new ColoredPolygon(
                    m.Vertices[lFaceStart].Color,
                    lPointBuff));
                }
              }
            }
          }
        } //end filling mWireframeCache

        //can't the Graphics object do its own bloody clipping?
        //it seems to be much faster to do it myself!
        RectangleF clipF = e.Graphics.ClipBounds;
        Rectangle clip = new Rectangle(
          (int)Math.Floor(clipF.Left),
          (int)Math.Floor(clipF.Top),
          (int)Math.Ceiling(clipF.Right - clipF.Left),
          (int)Math.Ceiling(clipF.Bottom - clipF.Top));

        foreach (ColoredPolygon p in mWireFrameCache)
        {
          foreach (Point pt in p.Vertices)
          {
            if (clip.Contains(pt))
            {
              e.Graphics.DrawPolygon(
                new Pen(p.Color),
                p.Vertices);
              break; //i.e. continue outer
            }
          }
        }
      }

      //highlight editable square
      Rectangle lHighlightRect = new Rectangle();
      Point lMousePos = mMainForm.GridDisplayPanel.PointToClient(Cursor.Position);
      switch(ViewMode)
      {
        case eViewMode.EditBumpSquares:
        case eViewMode.EditMetadata:
        case eViewMode.EditTextures:
          lHighlightRect = new Rectangle(
            lMousePos.X / mSubjectTileWidth * mSubjectTileWidth,
            lMousePos.Y / mSubjectTileHeight * mSubjectTileHeight,
            mSubjectTileWidth,
            mSubjectTileHeight);
          break;
        case eViewMode.EditBumpPixels:
          int lBumpPixWidth = mSubjectTileWidth / 8;
          int lBumpPixHeight = mSubjectTileHeight / 8;
          lHighlightRect = new Rectangle(
            lMousePos.X / lBumpPixWidth * lBumpPixWidth,
            lMousePos.Y / lBumpPixHeight * lBumpPixHeight,
            mSubjectTileWidth / 8,
            mSubjectTileHeight / 8);
          break;
      }

      if (lHighlightRect.Width > 0)
      {
        //assume graphics clip will take care of clipping
        e.Graphics.DrawRectangle(
            new Pen(Color.Red, 2.0f),
            lHighlightRect);
      }
    }

    private class ColoredPolygon
    {
      public Color Color;
      public Point[] Vertices;

      public ColoredPolygon(Color xiColor, Point[] xiVertices)
      {
        Color = xiColor;
        Vertices = xiVertices;
      }
    }

    // a precomputed cache of the objects in the whole level, as coloured polys
    // in the paint co-ords of the grid view
    private List<ColoredPolygon> mWireFrameCache = null;

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

      mWireFrameCache = null;

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

        string lWorldCoord = "World Coord: Only available for non-rotated sheets";
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

        switch (ViewMode)
        {
          case eViewMode.EditMetadata:
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
            break;

          case eViewMode.EditTextures:
            //edit textures mode
            if (mKeyOrMouseToSelPicBoxDict.ContainsKey(e.Button))
            {
              PictureBox lSel = mKeyOrMouseToSelPicBoxDict[e.Button];
              mSubject.TextureIds[x][y] = (byte)lSel.Tag;
              InvalidateGridDisplay();
            }
            break;

          case eViewMode.EditBumpSquares:
            //edit bump mode
            if (mKeyOrMouseToSelPicBoxDict.ContainsKey(e.Button))
            {
              PictureBox lSel = mKeyOrMouseToSelPicBoxDict[e.Button];
              mSubject.TexMetaData[x][y][(int)eTexMetaDataEntries.Bumpmap] = (byte)lSel.Tag;
              InvalidateGridDisplay();
            }
            break;

          case eViewMode.EditBumpPixels:
            //edit bump mode
            if (mKeyOrMouseToSelPicBoxDict.ContainsKey(e.Button))
            {
              PictureBox lSel = mKeyOrMouseToSelPicBoxDict[e.Button];
              byte lNewVal = (byte)lSel.Tag;
              int lBumpPxX = (e.X % mSubjectTileWidth) / (mSubjectTileWidth / 8);
              int lBumpPxY = (e.Y % mSubjectTileHeight) / (mSubjectTileHeight / 8);

              UpdateBumpPixel(x, y, lBumpPxX, lBumpPxY, lNewVal);

              InvalidateGridDisplay();
            }
            break;
        }
      }
    }

    private void UpdateBumpPixel(
      int xiTexX,  //the x-offset of the tex square in the grid
      int xiTexY,
      int xiBumpPxX, //the x-offset of the bump pixel in the square
      int xiBumpPxY,
      byte xiNewVal)
    {
      byte lBumpImageIdx = mSubject.TexMetaData[xiTexX][xiTexY][(int)eTexMetaDataEntries.Bumpmap];

      if (lBumpImageIdx >= mBumpImageUsageCountArray.Length)
      {
        MessageBox.Show("Cannot edit that bump square: it indexes a non-existant bump. Please edit it numerically first");
        return;
      }

      //how to update the bump pix depends on how many tex squares
      //use that bump pix
      switch (mBumpImageUsageCountArray[lBumpImageIdx])
      {
        case 0:

          throw new Exception("Interal error: unreachable statement!");

        case 1:
          BumpImageChunk bic = mMainForm.Level.GetBumpById(lBumpImageIdx);
          bic.SetPixelType(xiBumpPxX, xiBumpPxY, xiNewVal);
          break;

        default: // i.e. > 1
          //find the first bump image > 0 which isn't in use
          for (int lNewBumpImageIdx = 1; lNewBumpImageIdx < mBumpImageUsageCountArray.Length; lNewBumpImageIdx++)
          {
            if (mBumpImageUsageCountArray[lNewBumpImageIdx] == 0)
            {
              BumpImageChunk newBump = mMainForm.Level.GetBumpById(lNewBumpImageIdx);
              BumpImageChunk oldBump = mMainForm.Level.GetBumpById(lBumpImageIdx);
              newBump.CopyFrom(oldBump);
              newBump.SetPixelType(xiBumpPxX, xiBumpPxY, xiNewVal);
              mSubject.TexMetaData[xiTexX][xiTexY][(int)eTexMetaDataEntries.Bumpmap]
               = (byte)lNewBumpImageIdx;
              mBumpImageUsageCountArray[lBumpImageIdx]--;
              mBumpImageUsageCountArray[lNewBumpImageIdx]++;
              return;
            }
          }
          MessageBox.Show(@"The requested change cannot be performed:
There are no free bump images which are not already in use!
Try running ""Reindex bump"" on the level (in the Actions tab)");
          break;
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
         && (ViewMode == eViewMode.EditTextures 
           || ViewMode == eViewMode.EditBumpSquares
           || ViewMode == eViewMode.EditBumpPixels)
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
            else if (ViewMode == eViewMode.EditBumpSquares)
            {
              mSubject.TexMetaData[x][y][(int)eTexMetaDataEntries.Bumpmap] = (byte)lSel.Tag;
            }
            else if (ViewMode == eViewMode.EditBumpPixels)
            {
             byte lNewVal = (byte)lSel.Tag;
              int lBumpPxX = (p.X % mSubjectTileWidth) / (mSubjectTileWidth / 8);
              int lBumpPxY = (p.Y % mSubjectTileHeight) / (mSubjectTileHeight / 8);

              UpdateBumpPixel(x, y, lBumpPxX, lBumpPxY, lNewVal);
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
      EditBumpSquares,
      EditBumpPixels,
      EditMetadata
    }

    private eViewMode mViewMode = eViewMode.ViewOnly;
    public eViewMode ViewMode
    {
      get { return mViewMode; }
      set
      {
        const int PADDING = 5;

        if ((mSubject == null || mSubject.TexMetaData == null)
         && (value == eViewMode.EditBumpPixels
          || value == eViewMode.EditBumpSquares
          || value == eViewMode.EditMetadata
          || value == eViewMode.ViewBump))
        {
          value = eViewMode.ViewOnly; //reject change!
        }

        mViewMode = value;

        // update the editing palette
        //
        mMainForm.GridViewPalettePanel.SuspendLayout();
        mMainForm.GridViewPalettePanel.Controls.Clear();
        if (ViewMode == eViewMode.EditTextures 
          || ViewMode == eViewMode.EditBumpSquares
          || ViewMode == eViewMode.EditBumpPixels)
        {
          Point lNextPbTL = new Point(0, 0);
          IEnumerator<object> lKeys = mKeyOrMouseToSelPicBoxDict.Keys.GetEnumerator();
          PictureBox lPalPB = null;

          //create shared GDI objects outside of loop:
          Font lFont = null;
          Brush lNameFGBrush = null, lNameBGBrush = null;
          if (ViewMode == eViewMode.EditBumpPixels)
          {
            lFont = new Font(FontFamily.GenericSansSerif, 8);
            lNameBGBrush = new SolidBrush(Color.White);
            lNameFGBrush = new SolidBrush(Color.Black);
          }

          //loop over allowed values
          for (int i = 0; i < 256; i++)
          {
            Image im = null;

            //fetch or create the appropriate palette image for the byte value "i"
            if (ViewMode == eViewMode.EditTextures)
            {
              TIMChunk c = mMainForm.Level.GetTileById(i);
              if (c != null) im = c.ToImage();
              if (im.Width != this.mSubjectTileWidth
               || im.Height != this.mSubjectTileHeight)
              {
                im = null;
              }
            }
            else if (ViewMode == eViewMode.EditBumpSquares)
            {
              BumpImageChunk bim = mMainForm.Level.GetBumpById(i);
              if (bim != null) im = bim.ToImage();
            }
            else if (ViewMode == eViewMode.EditBumpPixels)
            {
              if (i < BumpImageChunk.HIGHEST_KNOWN_BUMP_TYPE)
              {
                BumpImageChunk.BumpTypeInfo bti = BumpImageChunk.GetBumpTypeInfo((byte)i);

                string lBumpTypeName = bti == null
                  ? string.Format("0x{0:x}", i)
                  : bti.Name;
                Bitmap lBmp = new Bitmap(64, 64);
                Graphics g = Graphics.FromImage(lBmp);
                SizeF lBumpNameSize = g.MeasureString(lBumpTypeName, lFont);

                //grow the bmp if needed (it'll be scaled back by the PictureBox
                if (lBumpNameSize.Width > lBmp.Width)
                {
                  lBmp = new Bitmap((int)lBumpNameSize.Width, 64);
                  g = Graphics.FromImage(lBmp);
                }

                g.Clear(BumpImageChunk.GetColorForBumpType((byte)i));

                //write the name on:
                float xf = lBmp.Width / 2 - lBumpNameSize.Width / 2;
                float yf = 64 / 2 - lBumpNameSize.Height / 2;

                g.FillRectangle(lNameBGBrush, xf, yf, lBumpNameSize.Width, lBumpNameSize.Height);

                g.DrawString(
                    lBumpTypeName,
                    lFont,
                    lNameFGBrush,
                    xf,
                    yf);

                im = lBmp;
              }
            }

            // have image, add it to editing palette
            if (im != null)
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

          //if there were fewer palette entries than there are keys,
          //fill the remaining keys with the last palette entry
          while (lKeys.MoveNext())
          {
            SetSelImage(lKeys.Current, lPalPB);
          }

          mMainForm.GridViewSelImageGroupBox.Visible = true;
        }
        else //not edit tex or edit bump
        {
          mMainForm.GridViewSelImageGroupBox.Visible = false;
        }
        mMainForm.GridViewPalettePanel.ResumeLayout();

        // make a list of how many times each bump tex is used, to be used
        // in editing the bump map, pixel by pixel
        if (ViewMode == eViewMode.EditBumpPixels)
        {
          mBumpImageUsageCountArray = new int[mMainForm.Level.SHET.BumpImages.mChildren.Length];
          int lBumpIdx = (int)eTexMetaDataEntries.Bumpmap;
          foreach (FlatChunk flat in mMainForm.Level.SHET.Flats)
          {
            if (flat.TexMetaData != null)
            {
              foreach (byte[][] row in flat.TexMetaData)
              {
                foreach (byte[] entry in row)
                {
                  int lBumpId = entry[lBumpIdx];
                  if (lBumpId < mBumpImageUsageCountArray.Length)
                  {
                    mBumpImageUsageCountArray[lBumpId]++;
                  }
                  else
                  {
                    Console.Error.WriteLine("Tex square in Flat {0} references non-existant bump id {1}",
                      flat.Name,
                      lBumpId);
                  }
                }
              }
            }
          }
        }

        if (OnViewModeChanged != null) OnViewModeChanged(this, null);
        InvalidateGridDisplay();
      }
    }
    public event EventHandler OnViewModeChanged;

    #endregion

    private int[] mBumpImageUsageCountArray;

    #region SelectedMeta property

    private eTexMetaDataEntries mSelectedMeta = eTexMetaDataEntries.Waypoint;
    public eTexMetaDataEntries SelectedMeta
    {
      get { return mSelectedMeta; }
      set
      {        
        mSelectedMeta = value;
        if (OnSelectedMetaChanged != null) OnSelectedMetaChanged(this, null);
        InvalidateGridDisplay();
      }
    }
    public event EventHandler OnSelectedMetaChanged;

    #endregion

    #region ShowObjects property

    private bool mShowObjects = false;
    public bool ShowObjects
    {
      get { return mShowObjects; }
      set
      {
        mShowObjects = value;
        if (OnShowObjectsChanged != null) OnShowObjectsChanged(this, null);
        InvalidateGridDisplay();
      }
    }
    public event EventHandler OnShowObjectsChanged;

    #endregion

    private ImageAttributes mTransparencyAttributes = new ImageAttributes();
  }
}

