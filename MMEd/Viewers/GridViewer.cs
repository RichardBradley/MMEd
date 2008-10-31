using System;
using System.Collections;
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
      new PropertyController(this, "ShowWaypoints").BindTo(mMainForm.GridViewShowWaypointsCheck);

      new PropertyController(this, "OverlayGridColor").BindTo(mMainForm.OverlaySelectorGrid);
      new PropertyController(this, "OverlayCameraColor").BindTo(mMainForm.OverlaySelectorCamera);
      new PropertyController(this, "OverlayRespawnColor").BindTo(mMainForm.OverlaySelectorRespawn);

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
      if (ViewMode == eViewMode.EditMetadata ||
        ViewMode == eViewMode.FillMetadata)
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
                mMainForm.CurrentLevel.GetTileById(mSubject.TextureIds[x][y]).ToBitmap(),
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
                OddImageChunk oic = mMainForm.CurrentLevel.GetOddById(mSubject.TexMetaData[x][y][(int)SelectedMeta]);

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
                BumpImageChunk bic = mMainForm.CurrentLevel.GetBumpById(mSubject.TexMetaData[x][y][(int)eTexMetaDataEntries.Bumpmap]);

                if (bic != null)
                {
                  DrawTransparentImage(e, bic.ToImage(), lBumpDest);
                }
                break;

              case eViewMode.EditMetadata:
              case eViewMode.FillMetadata:
                //draw the selected metadata on as text
                string text = string.Format("{0}", mSubject.TexMetaData[x][y][(int)SelectedMeta]);
                Utils.DrawString(e.Graphics, text, GetMidpoint(x, y));
                break;
            }
          }
          catch (NullReferenceException err)
          {
            Console.Error.WriteLine(err);
          }
        }
      }

      if (ShowObjects)
      {
        if (mWireFrameCache == null)
        {
          mWireFrameCache = new List<ColoredPolygon>();

          IEnumerable<Entity> lScene
            = mSubject.GetEntities(mMainForm.CurrentLevel, MMEd.Viewers.ThreeDee.eTextureMode.WireFrame, eTexMetaDataEntries.Odds);

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

      // Draw overlays
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
            if (OverlayGridColor != Color.Transparent)
            {
              DrawGridOverlay(e.Graphics, new Pen(OverlayGridColor), x, y);
            }

            if (OverlayCameraColor != Color.Transparent)
            {
              DrawCameraOverlay(e.Graphics, new Pen(OverlayCameraColor), x, y);
            }

            if (OverlayRespawnColor != Color.Transparent)
            {
              DrawRespawnOverlay(e.Graphics, new Pen(OverlayRespawnColor), x, y);
            }

            if (ShowWaypoints)
            {
              DrawWaypointsOverlay(e.Graphics, x, y);
            }
          }
          catch (NullReferenceException err)
          {
            Console.Error.WriteLine(err);
          }
        }
      }

      //highlight editable square
      Rectangle lHighlightRect = new Rectangle();
      Point lMousePos = mMainForm.GridDisplayPanel.PointToClient(Cursor.Position);
      switch (ViewMode)
      {
        case eViewMode.EditBumpSquares:
        case eViewMode.EditMetadata:
        case eViewMode.FillMetadata:
        case eViewMode.EditTextures:
        case eViewMode.EditCameras:
        case eViewMode.EditWaypoints:
          lHighlightRect = new Rectangle(
            lMousePos.X / mSubjectTileWidth * mSubjectTileWidth,
            lMousePos.Y / mSubjectTileHeight * mSubjectTileHeight,
            mSubjectTileWidth,
            mSubjectTileHeight);
          break;
        case eViewMode.EditBumpPixels:
        case eViewMode.EditRespawns:
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

      InvalidateGridDisplay();
    }

    private Point GetMidpoint(int x, int y)
    {
      return new Point((x * mSubjectTileWidth + (mSubjectTileWidth / 2)),
        (y * mSubjectTileHeight + (mSubjectTileHeight / 2)));
    }

    private Point GetCornerPoint(int x, int y)
    {
      return new Point((x * mSubjectTileWidth),
        (y * mSubjectTileHeight));
    }

    private Point GetPointInTile(int x, int y, int xiXOffset, int xiYOffset)
    {
      // Offset to the correct place in the tile, splitting it into an 8x8 grid.
      int xOffset = xiXOffset * (mSubjectTileWidth / 8);
      int yOffset = xiYOffset * (mSubjectTileHeight / 8);

      // Since each of the points in the 8x8 grid may be larger than one pixel,
      // also adjust it to the centre of that point.
      int xAdjust = (mSubjectTileWidth / 16);
      int yAdjust = (mSubjectTileHeight / 16);
      return new Point((x * mSubjectTileWidth + xOffset + xAdjust),
        (y * mSubjectTileHeight + yOffset + yAdjust));
    }

    private void DrawGridOverlay(Graphics g, Pen p, int x, int y)
    {
      Point lTopLeft = GetCornerPoint(x, y);
      g.DrawRectangle(
        p,
        lTopLeft.X,
        lTopLeft.Y,
        mSubjectTileWidth - 1,
        mSubjectTileHeight - 1);
    }

    private void DrawCameraOverlay(Graphics g, Pen p, int x, int y)
    {
      CameraPosChunk lCamera =
        mMainForm.CurrentLevel.GetCameraById(mSubject.TexMetaData[x][y][(int)eTexMetaDataEntries.CameraPos]);
      lCamera.Draw(g, p, GetMidpoint(x, y), mSubjectTileWidth);
    }

    private void DrawRespawnOverlay(Graphics g, Pen p, int x, int y)
    {
      byte lWaypointValue = mSubject.TexMetaData[x][y][(byte)eTexMetaDataEntries.Waypoint];

      // If the waypoint value is 0 then the respawn value doesn't matter - 
      // don't show it.
      if (lWaypointValue == 0)
      {
        return;
      }

      byte lZeroValue = mSubject.TexMetaData[x][y][(byte)eTexMetaDataEntries.Odds];
      byte lTwoValue = mSubject.TexMetaData[x][y][(byte)eTexMetaDataEntries.Two];
      byte lFourValue = mSubject.TexMetaData[x][y][(byte)eTexMetaDataEntries.Four];
      byte lSevenValue = mSubject.TexMetaData[x][y][(byte)eTexMetaDataEntries.Seven];
      Point lMidpoint = GetMidpoint(x, y);
      int lLineLength = (int)(mSubjectTileWidth * 0.4);

      if (Array.IndexOf(sNoRespawnValues, lTwoValue) >= 0)
      {
        Utils.DrawCross(g, p, lMidpoint, (int)(mSubjectTileWidth * 0.4));
      }
      else
      {
        // Get the odds image and find the point within it indicated by the
        // Seven value.
        int lRespawnX = (int)(lSevenValue / 16);
        int lRespawnY = (int)(lSevenValue % 16);
        OddImageChunk lOdd = mMainForm.CurrentLevel.GetOddById(mSubject.TexMetaData[x][y][0]);
        byte lOddValue = lOdd.Data[8 * lRespawnX + lRespawnY];

        // Calculate the direction from the combination of the Four value and
        // the value taken from the odds image.
        int lDirection = 2048 + lFourValue == 0 ?
          lOddValue * 256 :
          (lFourValue * 1024) + (lOddValue * -256);

        if (lFourValue == 0)
        {
          lDirection = lOddValue * 256;
        }
        else
        {
          lDirection = (lFourValue * 1024) + (lOddValue * -256);
        }

        Point lRespawnPoint = GetPointInTile(x, y, lRespawnX, lRespawnY);
        Utils.DrawArrow(g, p, lRespawnPoint, lDirection, lLineLength, false);
      }
    }

    private void DrawWaypointsOverlay(Graphics g, int x, int y)
    {
      byte lWaypoint = mSubject.TexMetaData[x][y][(byte)eTexMetaDataEntries.Waypoint];

      if (lWaypoint == 0)
      {
        return;
      }

      if (lWaypoint == mCurrentWaypoint)
      {
        Utils.DrawCircle(g, new Pen(Color.Green, 3), GetMidpoint(x, y), (int)(mSubjectTileWidth * 0.3));
      }

      KeyWaypointsChunk.KeySection lKeySection =
        mMainForm.CurrentLevel.SHET.GetKeySectionByWaypoint(lWaypoint);
      Point lTopLeft = GetCornerPoint(x, y);
      Point lTopRight = GetCornerPoint(x, y);
      Point lBottomLeft = GetCornerPoint(x, y);
      Point lBottomRight = GetCornerPoint(x, y);
      lTopRight.Offset(new Point(mSubjectTileWidth, 0));
      lBottomLeft.Offset(new Point(0, mSubjectTileHeight));
      lBottomRight.Offset(new Point(mSubjectTileWidth, mSubjectTileHeight));

      DrawWaypointOverlayLine(
        g,
        lWaypoint,
        lKeySection,
        x,
        y - 1,
        lTopLeft,
        lTopRight);
      DrawWaypointOverlayLine(
        g,
        lWaypoint,
        lKeySection,
        x - 1,
        y,
        lTopLeft, 
        lBottomLeft);
      DrawWaypointOverlayLine(
        g,
        lWaypoint,
        lKeySection,
        x,
        y + 1,
        lBottomLeft, 
        lBottomRight);
      DrawWaypointOverlayLine(
        g,
        lWaypoint,
        lKeySection,
        x + 1,
        y,
        lTopRight, 
        lBottomRight);
      Utils.DrawString(g, string.Format("{0}", lWaypoint), GetMidpoint(x, y));
    }

    private void DrawWaypointOverlayLine(
      Graphics g,
      byte xiCurrentWaypoint,
      KeyWaypointsChunk.KeySection xiCurrentKeySection,
      int xNeighbour, 
      int yNeighbour,
      Point xiLineStart,
      Point xiLineEnd)
    {
      Color lColor = Color.Transparent;

      // Check for the edge of the flat.
      if (xNeighbour < 0 ||
        yNeighbour < 0 ||
        xNeighbour >= mSubject.Width ||
        yNeighbour >= mSubject.Height)
      {
        lColor = xiCurrentKeySection == null ? Color.White : Color.Red;
      }
      else
      {
        // Compare to neighbouring square
        byte lNeighbourWaypoint =
          mSubject.TexMetaData[xNeighbour][yNeighbour][(byte)eTexMetaDataEntries.Waypoint];

        if (xiCurrentKeySection != null)
        {
          // If this is a key section, outline in red unless the neighbour is in 
          // the same section.
          KeyWaypointsChunk.KeySection lNeighbourKeySection =
            mMainForm.CurrentLevel.SHET.GetKeySectionByWaypoint(lNeighbourWaypoint);

          if (xiCurrentKeySection != lNeighbourKeySection)
          {
            lColor = Color.Red;
          }
        }
        else if (lNeighbourWaypoint == 0)
        {
          // For non-key sections, just draw a white border around the 
          // waypoint squares. Ideally it should also test whether the neighbour
          // falls into the same non-key section and draw a white line if not,
          // but that's a sufficiently borderline case that it's ignored.
          lColor = Color.White;
        }
      }

      if (lColor != Color.Transparent)
      {
        g.DrawLine(new Pen(lColor), xiLineStart, xiLineEnd);
      }
    }

    private void MakeWaypointChange(int x, int y, eWaypointAction xiAction)
    {
      byte lWaypoint = mSubject.TexMetaData[x][y][(byte)eTexMetaDataEntries.Waypoint];

      switch (xiAction)
      {
        case eWaypointAction.Insert:
        case eWaypointAction.AddIncrement:
          if (mCurrentWaypoint == 255 && lWaypoint == 0)
          {
            System.Windows.Forms.MessageBox.Show(
              mMainForm,
              "You can't add a waypoint higher than 255 - delete some of the existing ones first.",
              "Error",
              MessageBoxButtons.OK,
              MessageBoxIcon.Error);
          }
          else if ((lWaypoint == 0) || (xiAction == eWaypointAction.AddIncrement))
          {
            mCurrentWaypoint++;

            if (xiAction == eWaypointAction.Insert)
            {
                AdjustWaypoints(mCurrentWaypoint, 1);
            }
            mSubject.TexMetaData[x][y][(byte)eTexMetaDataEntries.Waypoint] = mCurrentWaypoint;
          }
          else
          {
            mCurrentWaypoint = (byte)(lWaypoint == 0 ? 0 : lWaypoint - 1);
            AdjustWaypoints(lWaypoint, -1);
            mSubject.TexMetaData[x][y][(byte)eTexMetaDataEntries.Waypoint] = 0;
          }
          break;
        case eWaypointAction.Duplicate:
          mSubject.TexMetaData[x][y][(byte)eTexMetaDataEntries.Waypoint] = mCurrentWaypoint;
          break;
        case eWaypointAction.Decrement:
          mCurrentWaypoint = mSubject.TexMetaData[x][y][(byte)eTexMetaDataEntries.Waypoint];
          if (mCurrentWaypoint > 1)
          {
              mCurrentWaypoint--;
              AdjustWaypoints(mCurrentWaypoint + 1, -1);
          }
          break;

        case eWaypointAction.Eyedropper:
          mCurrentWaypoint = lWaypoint;
          break;
        case eWaypointAction.Erase:
          mSubject.TexMetaData[x][y][(byte)eTexMetaDataEntries.Waypoint] = 0;

          if (lWaypoint == mCurrentWaypoint)
          {
            SetCurrentWaypointToDefault();
          }
          break;
        case eWaypointAction.Waypoints:
          SHETChunk lShet = mMainForm.CurrentLevel.SHET;

          if (lShet.GetKeySectionByWaypoint(lWaypoint) == null)
          {
            lShet.AddKeySection(lWaypoint);
          }
          else
          {
            lShet.RemoveKeySection(lWaypoint);
          }
          break;
      }
    }

    private void SetCurrentWaypointToDefault()
    {
      mCurrentWaypoint = 0;

      for (int x = 0; x < mSubject.Width; x++)
      {
        for (int y = 0; y < mSubject.Height; y++)
        {
          byte lWaypoint = mSubject.TexMetaData[x][y][(byte)eTexMetaDataEntries.Waypoint];

          mCurrentWaypoint = Math.Max(mCurrentWaypoint, lWaypoint);
        }
      }
    }

    private void AdjustWaypoints(int xiFrom, int xiChangeBy)
    {
      SHETChunk lShet = mMainForm.CurrentLevel.SHET;

      foreach (FlatChunk lFlat in lShet.Flats)
      {
        if (lFlat.TexMetaData == null)
        {
          continue;
        }

        for (int x = 0; x < lFlat.Width; x++)
        {
          for (int y = 0; y < lFlat.Height; y++)
          {
            byte lWaypoint = lFlat.TexMetaData[x][y][(byte)eTexMetaDataEntries.Waypoint];

            if (lWaypoint >= xiFrom)
            {
              int lNewValue = Math.Max(0, lWaypoint + xiChangeBy);
              lFlat.TexMetaData[x][y][(byte)eTexMetaDataEntries.Waypoint] = (byte)lNewValue;
            }
          }
        }
      }

      lShet.AdjustKeySections(xiFrom, xiChangeBy);
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
        TIMChunk firstTim = mMainForm.CurrentLevel.GetTileById(topLeftTexIdx);
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

        int lTexX = (int)Math.Floor(x);
        int lTexY = (int)Math.Floor(y);
        int lPxX = (e.X % mSubjectTileWidth) / (mSubjectTileWidth / 8);
        int lPxY = (e.Y % mSubjectTileHeight) / (mSubjectTileHeight / 8);

        byte lBumpIdx = mSubject.TexMetaData == null ? (byte)0 : mSubject.TexMetaData[lTexX][lTexY][(int)eTexMetaDataEntries.Bumpmap];
        BumpImageChunk lBumpChunk = mMainForm.CurrentLevel.GetBumpById(lBumpIdx);
        string lBumpString = null;

        if (lBumpChunk != null)
        {
          byte lBumpPixel = lBumpChunk.GetPixelType(lPxX, lPxY);
          lBumpString = "Bump: " + BumpImageChunk.GetBumpTypeInfo(lBumpPixel).Name;
        }

        mMainForm.GridViewerStatusLabel.Text = string.Format(
          "Tex Coord: ({0:0}, {1:0}) Flat Coord: ({2:0}, {3:0}) {4} {5}",
          lTexX, lTexY, x * mSubject.ScaleX, y * mSubject.ScaleY,
          lWorldCoord,
          lBumpString);

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
        int lPxX = (e.X % mSubjectTileWidth) / (mSubjectTileWidth / 8);
        int lPxY = (e.Y % mSubjectTileHeight) / (mSubjectTileHeight / 8);

        switch (ViewMode)
        {
          case eViewMode.EditMetadata:
          case eViewMode.FillMetadata:
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

                if (ViewMode == eViewMode.FillMetadata)
                {
                  for (int lX = 0; lX < mSubject.TexMetaData.Length; lX++)
                  {
                    for (int lY = 0; lY < mSubject.TexMetaData[x].Length; lY++)
                    {
                      mSubject.TexMetaData[lX][lY][(int)SelectedMeta] = (byte)val;
                    }
                  }
                }
                else
                {
                  mSubject.TexMetaData[x][y][(int)SelectedMeta] = (byte)val;
                }

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
              UpdateBumpPixel(x, y, lPxX, lPxY, lNewVal);
              InvalidateGridDisplay();
            }
            break;

          case eViewMode.EditCameras:
            //edit camera positions
            if (mKeyOrMouseToSelPicBoxDict.ContainsKey(e.Button))
            {
              PictureBox lSel = mKeyOrMouseToSelPicBoxDict[e.Button];
              mSubject.TexMetaData[x][y][(int)eTexMetaDataEntries.CameraPos] = (byte)lSel.Tag;
              InvalidateGridDisplay();
            }
            break;

          case eViewMode.EditRespawns:
            //edit respawns
            if (mKeyOrMouseToSelPicBoxDict.ContainsKey(e.Button))
            {
              PictureBox lSel = mKeyOrMouseToSelPicBoxDict[e.Button];
              SetRespawnPosition(x, y, new RespawnSetting(lPxX, lPxY, (byte)lSel.Tag));
              InvalidateGridDisplay();
            }
            break;

          case eViewMode.EditWaypoints:
            //edit waypoints
            if (mKeyOrMouseToSelPicBoxDict.ContainsKey(e.Button))
            {
              PictureBox lSel = mKeyOrMouseToSelPicBoxDict[e.Button];
              MakeWaypointChange(x, y, (eWaypointAction)(byte)lSel.Tag);
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
          BumpImageChunk bic = mMainForm.CurrentLevel.GetBumpById(lBumpImageIdx);
          bic.SetPixelType(xiBumpPxX, xiBumpPxY, xiNewVal);
          break;

        default: // i.e. > 1
          SHETChunk lShet = mMainForm.CurrentLevel.SHET;
          BumpImageChunk lNewBump = null;
          int lNewBumpId = -1;

          if (lShet.UnusedBumps.Count == 0)
          {
            //=================================================================
            // Create a new bump
            //=================================================================
            lNewBumpId = lShet.BumpImages.mChildren.Length;
            lNewBump = new BumpImageChunk(lNewBumpId);
            int lSizeIncrease = lShet.AddBump(lNewBump);
            lShet.TrailingZeroByteCount -= lSizeIncrease;

            if (lShet.TrailingZeroByteCount < 0)
            {
              MessageBox.Show(string.Format(
                "WARNING: You have just run out of space in your level file - you will need to free up {0} bytes before you can save your changes.",
                -lShet.TrailingZeroByteCount));
            }

            //=================================================================
            // Update our usage count array to include the new bump
            //=================================================================
            int[] lNewBumpImageUsageCountArray = new int[lNewBumpId + 1];
            Array.Copy(mBumpImageUsageCountArray, lNewBumpImageUsageCountArray, mBumpImageUsageCountArray.Length);
            mBumpImageUsageCountArray = lNewBumpImageUsageCountArray;
          }
          else
          {
            //=================================================================
            // Find the first unused bump and use that
            //=================================================================
            foreach (DictionaryEntry lEntry in lShet.UnusedBumps)
            {
              lNewBumpId = (int)lEntry.Key;
              lNewBump = (BumpImageChunk)lEntry.Value;
              break;
            }

            lShet.UnusedBumps.Remove(lNewBumpId);
          }

          //===================================================================
          // Update the bump with the desired contents, and update the terrain
          // square to use it
          //===================================================================
          BumpImageChunk lOldBump = mMainForm.CurrentLevel.GetBumpById(lBumpImageIdx);
          lNewBump.CopyFrom(lOldBump);
          lNewBump.SetPixelType(xiBumpPxX, xiBumpPxY, xiNewVal);
          mSubject.TexMetaData[xiTexX][xiTexY][(int)eTexMetaDataEntries.Bumpmap]
           = (byte)lNewBumpId;
          mBumpImageUsageCountArray[lBumpImageIdx]--;
          mBumpImageUsageCountArray[lNewBumpId]++;
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
           || ViewMode == eViewMode.EditBumpPixels 
           || ViewMode == eViewMode.EditCameras
           || ViewMode == eViewMode.EditRespawns
           || ViewMode == eViewMode.EditWaypoints)
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
            int lPxX = (p.X % mSubjectTileWidth) / (mSubjectTileWidth / 8);
            int lPxY = (p.Y % mSubjectTileHeight) / (mSubjectTileHeight / 8);
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
              UpdateBumpPixel(x, y, lPxX, lPxY, lNewVal);
            }
            else if (ViewMode == eViewMode.EditCameras)
            {
              mSubject.TexMetaData[x][y][(int)eTexMetaDataEntries.CameraPos] = (byte)lSel.Tag;
            }
            else if (ViewMode == eViewMode.EditRespawns)
            {
              SetRespawnPosition(x, y, new RespawnSetting(lPxX, lPxY, (byte)lSel.Tag));
            }
            else if (ViewMode == eViewMode.EditWaypoints)
            {
              MakeWaypointChange(x, y, (eWaypointAction)(byte)lSel.Tag);
            }
            InvalidateGridDisplay();
            return;
          }

          Point lClientPt = c.PointToClient(lMousePt);
          c = c.GetChildAtPoint(lClientPt);
        }
      }
    }

    private void SetRespawnPosition(
      int x, 
      int y, 
      RespawnSetting xiRespawnSetting)
    {
      SHETChunk lShet = mMainForm.CurrentLevel.SHET;

      // eDirection.None is used to mean 'no respawn allowed', which is coded 
      // on a different metadata sheet.
      if (xiRespawnSetting.Direction == eDirection.None)
      {
        mSubject.TexMetaData[x][y][(byte)eTexMetaDataEntries.Two] = FlatChunk.LAYERTWO_NORESPAWN;
        return;
      }
      else if (Array.IndexOf(sNoRespawnValues, mSubject.TexMetaData[x][y][(byte)eTexMetaDataEntries.Two]) >= 0)
      {
        mSubject.TexMetaData[x][y][(byte)eTexMetaDataEntries.Two] = FlatChunk.LAYERTWO_DEFAULT;
      }

      // Find an Odd image which can be used for this direction, at one of the four orientations
      int lOddId = FindOddMatchingDirection(ref xiRespawnSetting);

      if (lOddId == int.MinValue)
      {
        xiRespawnSetting.RotateTo(eOrientation.North);

        if (lShet.UnusedOdds.Count == 0)
        {
          lOddId = lShet.OddImages.mChildren.Length;
          OddImageChunk lNewOdd = new OddImageChunk(
            lOddId,
            (byte)xiRespawnSetting.Direction);
          int lSizeIncrease = lShet.AddOdd(lNewOdd);
          lShet.TrailingZeroByteCount -= lSizeIncrease;

          if (lShet.TrailingZeroByteCount < 0)
          {
            MessageBox.Show(string.Format(
              "WARNING: You have just run out of space in your level file - you will need to free up {0} bytes before you can save your changes.",
              -lShet.TrailingZeroByteCount));
          }
        }
        else
        {
          foreach (DictionaryEntry lEntry in lShet.UnusedOdds)
          {
            // Just get the first unused.
            lOddId = (int)lEntry.Key;
            OddImageChunk lOdd = (OddImageChunk)lEntry.Value;
            lOdd.FlushToValue((byte)xiRespawnSetting.Direction);
            break;
          }

          lShet.UnusedOdds.Remove(lOddId);
        }
      }

      // Use 4 instead of 0 for orientation North, because orientation 0 
      // reverses the directions...
      mSubject.TexMetaData[x][y][(byte)eTexMetaDataEntries.Four] =
       xiRespawnSetting.Orientation == eOrientation.North ? (byte)4 : (byte)xiRespawnSetting.Orientation;
      mSubject.TexMetaData[x][y][(byte)eTexMetaDataEntries.Odds] = (byte)lOddId;
      
      // Rotate back to north to set position (layer seven)
      xiRespawnSetting.RotateTo(eOrientation.North);
      byte lSevenValue = PointToLayerSevenValue(new Point(xiRespawnSetting.X, xiRespawnSetting.Y));
      mSubject.TexMetaData[x][y][(byte)eTexMetaDataEntries.Seven] = lSevenValue;
    }

    private byte PointToLayerSevenValue(Point xiPoint)
    {
      return (byte)(xiPoint.X * 16 + xiPoint.Y);
    }

    private int FindOddMatchingDirection(ref RespawnSetting xbRespawnSetting)
    {
      SHETChunk lShet = mMainForm.CurrentLevel.SHET;

      for (int i = 0; i < lShet.OddImages.mChildren.Length; i++)
      {
        if (!(lShet.OddImages.mChildren[i] is OddImageChunk))
        {
          continue;
        }

        OddImageChunk lOdd = (OddImageChunk)lShet.OddImages.mChildren[i];

        if (xbRespawnSetting.MatchToOdd(lOdd))
        {
          return i;
        }
      }

      return int.MinValue;
    }

    #region ViewMode property

    private eViewMode mViewMode = eViewMode.ViewOnly;
    public eViewMode ViewMode
    {
      get { return mViewMode; }
      set
      {
        const int PADDING = 5;

        if (value == eViewMode.EditWaypoints && mCurrentWaypoint == 0)
        {
          SetCurrentWaypointToDefault();
        }

        if ((mSubject == null || mSubject.TexMetaData == null)
         && (value == eViewMode.EditBumpPixels
          || value == eViewMode.EditBumpSquares
          || value == eViewMode.EditCameras
          || value == eViewMode.EditRespawns
          || value == eViewMode.EditWaypoints
          || value == eViewMode.EditMetadata
          || value == eViewMode.FillMetadata
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
          || ViewMode == eViewMode.EditBumpPixels
          || ViewMode == eViewMode.EditCameras
          || ViewMode == eViewMode.EditRespawns
          || ViewMode == eViewMode.EditWaypoints)
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
              TIMChunk c = mMainForm.CurrentLevel.GetTileById(i);
              if (c != null) im = c.ToImage();
              if (im.Width != this.mSubjectTileWidth
               || im.Height != this.mSubjectTileHeight)
              {
                im = null;
              }
            }
            else if (ViewMode == eViewMode.EditBumpSquares)
            {
              BumpImageChunk bim = mMainForm.CurrentLevel.GetBumpById(i);
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
            else if (ViewMode == eViewMode.EditCameras)
            {
              // Skip all camera positions between 1 and 50 - they're not used
              // for multiplayer mode.
              if (i == 1)
              {
                i = 51;
              }

              CameraPosChunk cpc = mMainForm.CurrentLevel.GetCameraById(i);
              if (cpc != null) im = cpc.ToImage();
            }
            else if (ViewMode == eViewMode.EditRespawns)
            {
              im = GetRespawnBrush(i);
            }
            else if (ViewMode == eViewMode.EditWaypoints)
            {
              im = GetWaypointBrush(i);
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
          mBumpImageUsageCountArray = new int[mMainForm.CurrentLevel.SHET.BumpImages.mChildren.Length];
          int lBumpIdx = (int)eTexMetaDataEntries.Bumpmap;
          foreach (FlatChunk flat in mMainForm.CurrentLevel.SHET.Flats)
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

    private Image GetRespawnBrush(int i)
    {
      Bitmap lRet = new Bitmap(4 * SCALE, 4 * SCALE);
      Graphics g = Graphics.FromImage(lRet);
      Pen p = new Pen(Color.Red, 1);
      Point lMidpoint = new Point(2 * SCALE, 2 * SCALE);

      if (i == 0)
      {
        Utils.DrawCross(g, p, lMidpoint, (int)(mSubjectTileWidth * 0.8));
        return lRet;
      }
      else if (i <= FlatChunk.LAYERZERO_HIGHESTDIRECTION + 1)
      {
        g.DrawRectangle(p, 0, 0, 63, 63);
        Utils.DrawArrow(g, p, lMidpoint, 4096 - ((i - 1) * 256), (int)(mSubjectTileWidth * 0.4), true);
        return lRet;
      }
      else
      {
        return null;
      }
    }

    private Image GetWaypointBrush(int i)
    {
      if (i > (int)eWaypointAction.Waypoints)
      {
        return null;
      }

      string lLabel = "";

      switch (i)
      {
        case (int)eWaypointAction.Insert:
          lLabel = "Add path";
          break;
        case (int)eWaypointAction.AddIncrement:
          lLabel = "Add";
          break;
        default:
          lLabel = ((eWaypointAction)i).ToString();
          break;
      }

      int lWidth = 4 * SCALE;
      int lHeight = 4 * SCALE;
      Bitmap lRet = new Bitmap(lWidth, lHeight);
      Graphics g = Graphics.FromImage(lRet);
      Font lFont = new Font(FontFamily.GenericMonospace, 10);
      int lTextWidth = (int)(g.MeasureString(lLabel, lFont).Width);

      if (lTextWidth > lWidth)
      {
        lWidth = (int)lTextWidth;
        lRet = new Bitmap(lWidth, lHeight);
        g = Graphics.FromImage(lRet);
      }

      Pen p = new Pen(Color.Red, 1);
      Point lMidpoint = new Point(lWidth / 2, lHeight / 2);
      g.DrawRectangle(p, 0, 0, lWidth - 1, lHeight - 1);
      Utils.DrawString(g, lLabel, lMidpoint);

      //switch (i)
      //{
      //  case (int)eWaypointAction.Insert:
      //    Utils.DrawString(g, "Add path", lMidpoint);
      //    break;
      //  case (int)eWaypointAction.Paint:
      //    Utils.DrawString(g, "Duplicate", lMidpoint);
      //    break;
      //  case (int)eWaypointAction.SetTo:
      //    Utils.DrawString(g, "Eyedropper", lMidpoint);
      //    break;
      //  case (int)eWaypointAction.ToggleKey:
      //    Utils.DrawString(g, "Waypoints", lMidpoint);
      //    break;
      //  default:
      //    Utils.DrawString(g, ((eWaypointAction)i).ToString(), lMidpoint);
      //    break;
      //}
      //qqTLP Reset mCurrentWaypoint
      return lRet;
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

    #region ShowWaypoints property

    private bool mShowWaypoints = false;
    public bool ShowWaypoints
    {
      get { return mShowWaypoints; }
      set
      {
        mShowWaypoints = value;
        if (OnShowWaypointsChanged != null) OnShowWaypointsChanged(this, null);
        InvalidateGridDisplay();
      }
    }
    public event EventHandler OnShowWaypointsChanged;

    #endregion

    #region OverlayGridColor property

    private Color mOverlayGridColor = Color.Transparent;
    public Color OverlayGridColor
    {
      get { return mOverlayGridColor; }
      set
      {
        mOverlayGridColor = value;
        if (OnOverlayGridColorChanged != null) OnOverlayGridColorChanged(this, null);
        InvalidateGridDisplay();
      }
    }
    public event EventHandler OnOverlayGridColorChanged;

    #endregion

    #region OverlayCameraColor property

    private Color mOverlayCameraColor = Color.Transparent;
    public Color OverlayCameraColor
    {
      get { return mOverlayCameraColor; }
      set
      {
        mOverlayCameraColor = value;
        if (OnOverlayCameraColorChanged != null) OnOverlayCameraColorChanged(this, null);
        InvalidateGridDisplay();
      }
    }
    public event EventHandler OnOverlayCameraColorChanged;

    #endregion

    #region OverlayRespawnColor property

    private Color mOverlayRespawnColor = Color.Transparent;
    public Color OverlayRespawnColor
    {
      get { return mOverlayRespawnColor; }
      set
      {
        mOverlayRespawnColor = value;
        if (OnOverlayRespawnColorChanged != null) OnOverlayRespawnColorChanged(this, null);
        InvalidateGridDisplay();
      }
    }
    public event EventHandler OnOverlayRespawnColorChanged;

    #endregion

    #region Enums

    public enum eViewMode
    {
      ViewOnly,
      ViewBump,
      ViewOdds,
      EditTextures,
      EditBumpSquares,
      EditBumpPixels,
      EditCameras,
      EditRespawns,
      EditWaypoints,
      EditMetadata,
      FillMetadata
    }

    private enum eOrientation
    {
      Unknown = -1,
      North = 0,
      East = 1,
      South = 2,
      West = 3
    }

    private enum eDirection
    {
      None = -1,
      N = 0,
      NNW = 1,
      NW = 2,
      WNW = 3,
      W = 4,
      WSW = 5,
      SW = 6,
      SSW = 7,
      S = 8,
      SSE = 9,
      SE = 10,
      ESE = 11,
      E = 12,
      ENE = 13,
      NE = 14,
      NNE = 15
    }

    private enum eWaypointAction
    {
      Insert = 0,
      AddIncrement = 1,
      Duplicate = 2,
      Decrement = 3,
      Eyedropper = 4,
      Erase = 5,
      Waypoints = 6,
    }

    #endregion

    #region Helper classes

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

    private class RespawnSetting
    {
      public RespawnSetting(int x, int y, int xiNewDirectionValue)
      {
        X = x;
        Y = y;
        Orientation = eOrientation.North;

        if (xiNewDirectionValue > FlatChunk.LAYERZERO_HIGHESTDIRECTION + 1)
        {
          throw new Exception("Tried to set respawn direction to a value higher than the maximum");
        }

        // The direction value passed in is based on the brush numbering and so needs to be reduced by one.
        Direction = (eDirection)(xiNewDirectionValue - 1);
      }

      public bool MatchToOdd(OddImageChunk xiOdd)
      {
        eOrientation lOriginalOrientation = Orientation;

        for (int lOrientation = (int)eOrientation.North; (int)lOrientation <= (int)eOrientation.West; lOrientation++)
        {
          RotateTo((eOrientation)lOrientation);

          // y needs to be flipped.
          if (xiOdd.GetPixelType(X, (FlatChunk.ODDDIMENSION - 1) - Y) == (byte)Direction)
          {
            return true;
          }
        }

        RotateTo(lOriginalOrientation);
        return false;
      }

      public void RotateTo(eOrientation xiOrientation)
      {
        int lDifference = xiOrientation - Orientation;

        if (lDifference <= 0)
        {
          lDifference += 4;
        }

        int lOldX = X;
        int lOldY = Y;
        int lMaxValue = FlatChunk.ODDDIMENSION - 1;

        switch (lDifference)
        {
          case 0:
          // No change
          case 1:
            X = lMaxValue - lOldY;
            Y = lOldX;
            Direction += 4;
            break;
          case 2:
            X = lMaxValue - lOldX;
            Y = lMaxValue - lOldY;
            Direction += 8;
            break;
          case 3:
            X = lOldY;
            Y = lMaxValue - lOldX;
            Direction += 12;
            break;
        }

        if ((int)Direction >= 16)
        {
          Direction -= 16;
        }

        Orientation = xiOrientation;
      }

      public int X;
      public int Y;
      public eOrientation Orientation = eOrientation.North;
      public eDirection Direction = eDirection.N;
    }

    #endregion

    private ImageAttributes mTransparencyAttributes = new ImageAttributes();
    private byte mCurrentWaypoint = 0;
    private static byte[] sNoRespawnValues = new byte[] { 2, 5, 8, 12 };
    private const int SCALE = 16;
  }
}

