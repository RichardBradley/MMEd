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
      new PropertyController(this, "OverlaySteeringColor").BindTo(mMainForm.OverlaySelectorSteering);
      new PropertyController(this, "OverlayBehaviourColor").BindTo(mMainForm.OverlaySelectorBehaviour);

      mMainForm.ViewerTabControl.KeyPress += new KeyPressEventHandler(this.ViewerTabControl_KeyPress);

      //set the transparency
      mMainForm.GridViewTransparencySlider.ValueChanged += new EventHandler(this.TransparencyLevelChange);
      TransparencyLevelChange(null, null);

      //set the zoom
      mMainForm.GridViewZoomSlider.ValueChanged += new EventHandler(this.ZoomLevelChange);
      ZoomLevelChange(null, null);
    }

    void ZoomLevelChange(object sender, EventArgs e)
    {
      mZoom = mMainForm.GridViewZoomSlider.Value / 20.0f;
      UpdateZoom();
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
        lNumberOffX = mSubjectTileScaledWidth / 2;
        lNumberOffY = mSubjectTileScaledHeight / 2;
      }

      // main drawing loop; iterate over tex squares:
      for (int x = (e.ClipRectangle.Left / mSubjectTileScaledWidth);
          x < (e.ClipRectangle.Right / mSubjectTileScaledWidth) + 1
          && x < mSubject.Width; x++)
      {
        for (int y = (e.ClipRectangle.Top / mSubjectTileScaledHeight);
            y < (e.ClipRectangle.Bottom / mSubjectTileScaledHeight) + 1
            && y < mSubject.Height; y++)
        {
          try
          {
            //=================================================================
            // Draw the main texture for the square
            //=================================================================
            Bitmap lTileBitmap = mMainForm.CurrentLevel.GetTileById(mSubject.TextureIds[x][y]).ToBitmap();
            Rectangle lTileRect = new Rectangle(
                x * mSubjectTileScaledWidth,
                y * mSubjectTileScaledHeight,
                mSubjectTileScaledWidth,
                mSubjectTileScaledHeight);

            e.Graphics.DrawImage(
                lTileBitmap,
                lTileRect);

            //=================================================================
            // Draw the bump map on top as a transparent image.
            //=================================================================
            Bitmap lBumpBitmap = mSubject.HasMetaData ?
              mMainForm.CurrentLevel.GetBumpById(mSubject.TexMetaData[x][y][(int)eTexMetaDataEntries.Bumpmap]).ToBitmap() : 
              null;

            if (lBumpBitmap != null)
            {
              e.Graphics.DrawImage(
                lBumpBitmap,
                lTileRect,
                0, // These 5 params define which part of the source image to use - all of it.
                0,
                lBumpBitmap.Width,
                lBumpBitmap.Height,
                GraphicsUnit.Pixel,
                mTransparencyAttributes);
            }

            switch (ViewMode)
            {
              case eViewMode.EditMetadata:
              case eViewMode.FillMetadata:
                //draw the selected metadata on as text
                if (mSubject.HasMetaData)
                {
                  string text = string.Format("{0}", mSubject.TexMetaData[x][y][(int)SelectedMeta]);
                  Utils.DrawString(e.Graphics, text, GetMidpoint(x, y));
                }
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
            = mSubject.GetEntities(mMainForm.CurrentLevel, MMEd.Viewers.ThreeDee.eTextureMode.WireFrame, eTexMetaDataEntries.Steering);

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
          //    to (mSubject.Width*mSubjectTileScaledWidth, mSubject.Height*mSubjectTileScaledHeight)
          //
          // The objects are defined in World coords (1), and the GLTK display matrix for the flat is (2) -> (1)
          // So we can go from (1) -> (2) -> (4) without needing to bother with (3)

          Matrix lMapWorldCoordsToSimpleFlatCoords = lSubjectsEntity.Transform.Inverse();

          Matrix lMapWorldCoordsToPaintCoords = lMapWorldCoordsToSimpleFlatCoords
            * Matrix.ScalingMatrix(mSubjectTileScaledWidth, mSubjectTileScaledHeight, 1);

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
      for (int x = (e.ClipRectangle.Left / mSubjectTileScaledWidth);
          x < (e.ClipRectangle.Right / mSubjectTileScaledWidth) + 1
          && x < mSubject.Width; x++)
      {
        for (int y = (e.ClipRectangle.Top / mSubjectTileScaledHeight);
            y < (e.ClipRectangle.Bottom / mSubjectTileScaledHeight) + 1
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

            if (OverlaySteeringColor != Color.Transparent)
            {
              DrawSteeringOverlay(e.Graphics, new Pen(OverlaySteeringColor), x, y);
            }

            if (OverlayBehaviourColor != Color.Transparent)
            {
              DrawBehaviourOverlay(e.Graphics, new Pen(OverlayBehaviourColor), x, y);
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
        case eViewMode.EditSteeringSquares:
        case eViewMode.EditMetadata:
        case eViewMode.FillMetadata:
        case eViewMode.EditTextures:
        case eViewMode.EditCameras:
        case eViewMode.EditWaypoints:
        case eViewMode.EditBehaviours:
          lHighlightRect = new Rectangle(
            lMousePos.X / mSubjectTileScaledWidth * mSubjectTileScaledWidth,
            lMousePos.Y / mSubjectTileScaledHeight * mSubjectTileScaledHeight,
            mSubjectTileScaledWidth,
            mSubjectTileScaledHeight);
          break;
        case eViewMode.EditBumpPixels:
        case eViewMode.EditSteeringPixels:
        case eViewMode.EditRespawns:
          int lTexX = (int)Math.Floor(lMousePos.X / (double)mSubjectTileScaledWidth);
          int lTexY = (int)Math.Floor(lMousePos.Y / (double)mSubjectTileScaledHeight);
          int lPxX = (int)((lMousePos.X % mSubjectTileScaledWidth) / ((double)mSubjectTileScaledWidth / 8.0));
          int lPxY = (int)((lMousePos.Y % mSubjectTileScaledHeight) / ((double)mSubjectTileScaledHeight / 8.0));

          lHighlightRect = new Rectangle(
            (int)((lTexX + (lPxX / 8.0)) * mSubjectTileScaledWidth),
            (int)((lTexY + (lPxY / 8.0)) * mSubjectTileScaledHeight),
            (int)(mSubjectTileScaledWidth / 8.0),
            (int)(mSubjectTileScaledHeight / 8.0));
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
      return new Point((x * mSubjectTileScaledWidth + (mSubjectTileScaledWidth / 2)),
        (y * mSubjectTileScaledHeight + (mSubjectTileScaledHeight / 2)));
    }

    private Point GetCornerPoint(int x, int y)
    {
      return new Point((x * mSubjectTileScaledWidth),
        (y * mSubjectTileScaledHeight));
    }

    private Point GetPointInTile(int x, int y, int xiXOffset, int xiYOffset)
    {
      // Offset to the correct place in the tile, splitting it into an 8x8 grid.
      int xOffset = xiXOffset * (mSubjectTileScaledWidth / 8);
      int yOffset = xiYOffset * (mSubjectTileScaledHeight / 8);

      // Since each of the points in the 8x8 grid may be larger than one pixel,
      // also adjust it to the centre of that point.
      int xAdjust = (mSubjectTileScaledWidth / 16);
      int yAdjust = (mSubjectTileScaledHeight / 16);
      return new Point((x * mSubjectTileScaledWidth + xOffset + xAdjust),
        (y * mSubjectTileScaledHeight + yOffset + yAdjust));
    }

    private void DrawGridOverlay(Graphics g, Pen p, int x, int y)
    {
      Point lTopLeft = GetCornerPoint(x, y);
      g.DrawRectangle(
        p,
        lTopLeft.X,
        lTopLeft.Y,
        mSubjectTileScaledWidth - 1,
        mSubjectTileScaledHeight - 1);
    }

    private void DrawCameraOverlay(Graphics g, Pen p, int x, int y)
    {
      if (!mSubject.HasMetaData)
      {
        return;
      }

      CameraPosChunk lCamera =
        mMainForm.CurrentLevel.GetCameraById(mSubject.TexMetaData[x][y][(int)eTexMetaDataEntries.CameraPos]);
      lCamera.Draw(g, p, GetMidpoint(x, y), mSubjectTileScaledWidth);
    }

    private void DrawSteeringOverlay(Graphics g, Pen p, int x, int y)
    {
      if (!mSubject.HasMetaData)
      {
        return;
      }

      SteeringImageChunk lSteeringImage = mMainForm.CurrentLevel.GetSteeringImageById(mSubject.TexMetaData[x][y][(int)eTexMetaDataEntries.Steering]);
      byte lOrientation = mSubject.TexMetaData[x][y][(int)eTexMetaDataEntries.Orientation];
      Rectangle lDest = new Rectangle(
          x * mSubjectTileScaledWidth,
          y * mSubjectTileScaledHeight,
          mSubjectTileScaledWidth,
          mSubjectTileScaledHeight);

      g.DrawImage(lSteeringImage.ToImage(lOrientation, p), lDest);
    }

    private void DrawBehaviourOverlay(Graphics g, Pen p, int x, int y)
    {
      if (!mSubject.HasMetaData)
      {
        return;
      }

      byte lBehaviourValue = mSubject.TexMetaData[x][y][(byte)eTexMetaDataEntries.Behaviour];
      FlatChunk.Behaviour lBehaviour = lBehaviourValue < FlatChunk.Behaviours.Length ?
        FlatChunk.Behaviours[lBehaviourValue] : null;

      Rectangle lTarget = new Rectangle(
          x * mSubjectTileScaledWidth,
          y * mSubjectTileScaledHeight,
          mSubjectTileScaledWidth,
          mSubjectTileScaledHeight);

      if (lBehaviour != null)
      {
        g.DrawImage(lBehaviour.ToBitmap(p), lTarget);
      }
    }

    private void DrawRespawnOverlay(Graphics g, Pen p, int x, int y)
    {
      if (!mSubject.HasMetaData)
      {
        return;
      }

      byte lWaypointValue = mSubject.TexMetaData[x][y][(byte)eTexMetaDataEntries.Waypoint];

      // If the waypoint value is 0 then the respawn value doesn't matter - 
      // don't show it.
      if (lWaypointValue == 0)
      {
        return;
      }

      byte lSteeringValue = mSubject.TexMetaData[x][y][(byte)eTexMetaDataEntries.Steering];
      byte lBehaviourValue = mSubject.TexMetaData[x][y][(byte)eTexMetaDataEntries.Behaviour];
      byte lOrientationValue = mSubject.TexMetaData[x][y][(byte)eTexMetaDataEntries.Orientation];
      byte lRespawnValue = mSubject.TexMetaData[x][y][(byte)eTexMetaDataEntries.RespawnPos];
      Point lMidpoint = GetMidpoint(x, y);
      int lLineLength = (int)(mSubjectTileScaledWidth * 0.4);

      if (Array.IndexOf(sNoRespawnValues, lBehaviourValue) >= 0)
      {
        Utils.DrawCross(g, p, lMidpoint, (int)(mSubjectTileScaledWidth * 0.4));
      }
      else
      {
        // Get the steering image and find the point within it indicated by the
        // respawn value.
        int lRespawnX = (int)(lRespawnValue / 16);
        int lRespawnY = (int)(lRespawnValue % 16);
        SteeringImageChunk lSteeringImage = mMainForm.CurrentLevel.GetSteeringImageById(mSubject.TexMetaData[x][y][0]);
        byte lSteeringDirection = lSteeringImage.GetPixelType(lRespawnX, lRespawnY);

        // Calculate the direction from the combination of the orientation value and
        // the value taken from the steering image.
        int lDirection = 2048 + lOrientationValue == 0 ?
          lSteeringDirection * 256 :
          (lOrientationValue * 1024) + (lSteeringDirection * -256);

        if (lOrientationValue == 0)
        {
          lDirection = lSteeringDirection * 256;
        }
        else
        {
          lDirection = (lOrientationValue * 1024) + (lSteeringDirection * -256);
        }

        Point lRespawnPoint = GetPointInTile(x, y, lRespawnX, lRespawnY);
        Utils.DrawArrow(g, p, lRespawnPoint, lDirection, lLineLength, 1, false);
      }
    }

    private void DrawWaypointsOverlay(Graphics g, int x, int y)
    {
      if (!mSubject.HasMetaData)
      {
        return;
      }

      byte lWaypoint = mSubject.TexMetaData[x][y][(byte)eTexMetaDataEntries.Waypoint];

      if (lWaypoint == 0)
      {
        return;
      }

      if (lWaypoint == mCurrentWaypoint)
      {
        Utils.DrawCircle(g, new Pen(Color.Green, 3), GetMidpoint(x, y), (int)(mSubjectTileScaledWidth * 0.3));
      }

      KeyWaypointsChunk.KeySection lKeySection =
        mMainForm.CurrentLevel.SHET.GetKeySectionByWaypoint(lWaypoint);
      Point lTopLeft = GetCornerPoint(x, y);
      Point lTopRight = GetCornerPoint(x, y);
      Point lBottomLeft = GetCornerPoint(x, y);
      Point lBottomRight = GetCornerPoint(x, y);
      lTopRight.Offset(new Point(mSubjectTileScaledWidth, 0));
      lBottomLeft.Offset(new Point(0, mSubjectTileScaledHeight));
      lBottomRight.Offset(new Point(mSubjectTileScaledWidth, mSubjectTileScaledHeight));

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
        case eWaypointAction.AddPath:
        case eWaypointAction.Add:
          if (mCurrentWaypoint == MAX_WAYPOINT && lWaypoint == 0)
          {
            System.Windows.Forms.MessageBox.Show(
              mMainForm,
              "You can't add a waypoint higher than " + MAX_WAYPOINT.ToString() + " - delete some of the existing ones first.",
              "Error",
              MessageBoxButtons.OK,
              MessageBoxIcon.Error);
          }
          else if ((lWaypoint == 0) || (xiAction == eWaypointAction.Add))
          {
            mCurrentWaypoint++;

            if (xiAction == eWaypointAction.AddPath)
            {
              AdjustWaypoints(mCurrentWaypoint, 1, -1);
            }
            mSubject.TexMetaData[x][y][(byte)eTexMetaDataEntries.Waypoint] = mCurrentWaypoint;
          }
          else
          {
            mCurrentWaypoint = (byte)(lWaypoint == 0 ? 0 : lWaypoint - 1);
            AdjustWaypoints(lWaypoint, -1, -1);
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
              AdjustWaypoints(mCurrentWaypoint + 1, -1, 1);
          }
          break;

        case eWaypointAction.Eyedropper:
          mCurrentWaypoint = lWaypoint;
          break;
        case eWaypointAction.Erase:
          mSubject.TexMetaData[x][y][(byte)eTexMetaDataEntries.Waypoint] = 0;

          if (lWaypoint == mCurrentWaypoint)
          {
            mCurrentWaypoint = GetDefaultWaypoint();
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
        case eWaypointAction.Restart:
          //===================================================================
          // Set the selected position to be the starting point
          //===================================================================
          if (lWaypoint == 0)
          {
            mSubject.TexMetaData[x][y][(byte)eTexMetaDataEntries.Waypoint] = 1;
          }
          else
          {
            int lMaxWaypoint = GetMaxWaypoint();

            AdjustWaypoints(1, 1 - lWaypoint, lMaxWaypoint);
          }
          break;
      }
    }

    private byte GetMaxWaypoint()
    {
      byte lMaxWaypoint = 0;

      foreach (FlatChunk lFlat in mMainForm.CurrentLevel.SHET.Flats)
      {
        if (!lFlat.HasMetaData)
        {
          continue;
        }

        for (int x = 0; x < lFlat.Width; x++)
        {
          for (int y = 0; y < lFlat.Height; y++)
          {
            byte lWaypoint = lFlat.TexMetaData[x][y][(byte)eTexMetaDataEntries.Waypoint];

            lMaxWaypoint = Math.Max(lMaxWaypoint, lWaypoint);
          }
        }
      }

      return lMaxWaypoint;
    }

    private byte GetDefaultWaypoint()
    {
      byte lDefaultWaypoint = 0;

      for (int x = 0; x < mSubject.Width; x++)
      {
        for (int y = 0; y < mSubject.Height; y++)
        {
          byte lWaypoint = mSubject.TexMetaData[x][y][(byte)eTexMetaDataEntries.Waypoint];

          lDefaultWaypoint = Math.Max(lDefaultWaypoint, lWaypoint);
        }
      }

      return lDefaultWaypoint;
    }

    private void AdjustWaypoints(int xiFrom, int xiChangeBy, int xiWrap)
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
              int lNewValue = lWaypoint + xiChangeBy;
              if (lNewValue <= 0)
              {
                if (xiWrap > 0)
                {
                  lNewValue += xiWrap;
                }
                else
                {
                  lNewValue = 0;
                }
              }
              else if (lNewValue > xiWrap)
              {
                lNewValue -= xiWrap;
              }
              lFlat.TexMetaData[x][y][(byte)eTexMetaDataEntries.Waypoint] = (byte)lNewValue;
            }
          }
        }
      }

      lShet.AdjustKeySections(xiFrom, xiChangeBy, xiWrap);
    }

    private const int MAX_WAYPOINT = 255;

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
    int mSubjectTileNativeWidth;
    int mSubjectTileNativeHeight;
    int mSubjectTileScaledWidth;
    int mSubjectTileScaledHeight;
    float mZoom = 1;

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
        mSubjectTileNativeHeight = firstTim.ImageHeight;
        mSubjectTileNativeWidth = firstTim.ImageWidth;
        UpdateZoom();

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
      //=======================================================================
      // Reset the ViewMode. This will ensure that the currently selected
      // mode is valid for the new subject.
      //=======================================================================
      ViewMode = ViewMode;
    }

    private void UpdateZoom()
    {
      if (mSubject == null)
      {
        return;
      }

      mWireFrameCache = null;
      mSubjectTileScaledWidth = (int)(mSubjectTileNativeWidth * mZoom);
      mSubjectTileScaledHeight = (int)(mSubjectTileNativeHeight * mZoom);
      mMainForm.GridDisplayPanel.Width = (int)(mSubjectTileScaledWidth * mSubject.Width);
      mMainForm.GridDisplayPanel.Height = (int)(mSubjectTileScaledHeight * mSubject.Height);
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

    void GridDisplayMouseWheel(object sender, MouseEventArgs e)
    {
      mZoom += e.Delta / 1000f;
      UpdateZoom();
      InvalidateGridDisplay();
    }

    private void GridDisplayMouseMove(object sender, MouseEventArgs e)
    {
      if (mSubject != null)
      {
        double x = e.X / (double)mSubjectTileScaledWidth;
        double y = e.Y / (double)mSubjectTileScaledHeight;

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
        int lPxX = (int)((e.X % mSubjectTileScaledWidth) / ((double)mSubjectTileScaledWidth / 8.0));
        int lPxY = (int)((e.Y % mSubjectTileScaledHeight) / ((double)mSubjectTileScaledHeight / 8.0));

        byte lBumpIdx = mSubject.TexMetaData == null ? (byte)0 : mSubject.TexMetaData[lTexX][lTexY][(int)eTexMetaDataEntries.Bumpmap];
        BumpImageChunk lBumpChunk = mMainForm.CurrentLevel.GetBumpById(lBumpIdx);
        string lBumpString = null;

        if (lBumpChunk != null)
        {
          byte lBumpPixel = lBumpChunk.GetPixelType(lPxX, lPxY);
          lBumpString = "Bump: " + BumpImageChunk.GetBumpTypeInfo(lBumpPixel).Name;
        }

        byte lSteeringIdx = mSubject.TexMetaData == null ? (byte)0 : mSubject.TexMetaData[lTexX][lTexY][(int)eTexMetaDataEntries.Steering];
        SteeringImageChunk lSteeringChunk = mMainForm.CurrentLevel.GetSteeringImageById(lSteeringIdx);
        string lSteeringString = null;

        if (lSteeringChunk != null)
        {
          byte lSteeringPixel = lSteeringChunk.GetPixelType(lPxX, lPxY);
          lSteeringString = "Steering: " + SteeringImageChunk.GetDirectionName(lSteeringPixel);
        }

        mMainForm.GridViewerStatusLabel.Text = string.Format(
          "Tex Coord: ({0:0}, {1:0}) Flat Coord: ({2:0}, {3:0}) {4} Tex:{5} {6} {7}",
          lTexX, lTexY, x * mSubject.ScaleX, y * mSubject.ScaleY,
          lWorldCoord,
          mSubject.TextureIds[lTexX][lTexY],
          lBumpString,
          lSteeringString);

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
        int x = e.X / mSubjectTileScaledWidth;
        int y = e.Y / mSubjectTileScaledHeight;
        int lPxX = (int)((e.X % mSubjectTileScaledWidth) / ((double)mSubjectTileScaledWidth / 8.0));
        int lPxY = (int)((e.Y % mSubjectTileScaledHeight) / ((double)mSubjectTileScaledHeight / 8));

        PictureBox lSel = mKeyOrMouseToSelPicBoxDict[e.Button];
        BrushData lBrush = BrushData.FromPictureBox(lSel);

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
            if (lBrush != null)
            {
              mSubject.TextureIds[x][y] = lBrush.Value;
              InvalidateGridDisplay();
            }
            break;

          case eViewMode.EditBumpSquares:
            //edit bump mode
            if (lBrush != null)
            {
              mSubject.TexMetaData[x][y][(int)eTexMetaDataEntries.Bumpmap] = lBrush.Value;
              InvalidateGridDisplay();
            }
            break;

          case eViewMode.EditBumpPixels:
            //edit bump mode (pixels)
            if (lBrush != null)
            {
              UpdateBumpPixel(x, y, lPxX, lPxY, lBrush.Value);
              InvalidateGridDisplay();
            }
            break;

          case eViewMode.EditSteeringSquares:
            // edit steering mode
            if (lBrush != null)
            {
              mSubject.TexMetaData[x][y][(int)eTexMetaDataEntries.Steering] = lBrush.Value;
              mSubject.TexMetaData[x][y][(int)eTexMetaDataEntries.Orientation] = lBrush.Orientation;
              InvalidateGridDisplay();
            }
            break;

          case eViewMode.EditSteeringPixels:
            // edit steering mode (pixels)
            if (lBrush != null)
            {
              UpdateSteeringPixel(x, y, lPxX, lPxY, lBrush.Value); 
              InvalidateGridDisplay();
            }
            break;

          case eViewMode.EditCameras:
            //edit camera positions
            if (lBrush != null)
            {
              mSubject.TexMetaData[x][y][(int)eTexMetaDataEntries.CameraPos] = lBrush.Value;
              InvalidateGridDisplay();
            }
            break;

          case eViewMode.EditRespawns:
            //edit respawns
            if (lBrush != null)
            {
              SetRespawnPosition(x, y, new RespawnSetting(lPxX, lPxY, lBrush.Value));
              InvalidateGridDisplay();
            }
            break;

          case eViewMode.EditWaypoints:
            //edit waypoints
            if (lBrush != null)
            {
              MakeWaypointChange(x, y, (eWaypointAction)lBrush.Value);
              InvalidateGridDisplay();
            }
            break;

          case eViewMode.EditBehaviours:
            //edit behaviours
            if (lBrush != null)
            {
              mSubject.TexMetaData[x][y][(int)eTexMetaDataEntries.Behaviour] = lBrush.Value;
              InvalidateGridDisplay();
            }
            break;
        }
      }
    }

    ///========================================================================
    /// Method : UpdateBumpPixel
    /// 
    /// <summary>
    /// 	Update an individual pixel in the bump maps
    /// </summary>
    /// <param name="xiTexX"></param>
    /// <param name="xiTexY"></param>
    /// <param name="xiBumpPxX"></param>
    /// <param name="xiBumpPxY"></param>
    /// <param name="xiNewVal"></param>
    ///========================================================================
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

    ///========================================================================
    /// Method : UpdateSteeringPixel
    /// 
    /// <summary>
    /// 	Update an individual pixel in the steering maps.
    /// 
    ///   Note that we don't just blindly accept the new value supplied - 
    ///   we assume it's relative to orientation 4, and if this square's
    ///   orientation is different we rotate it first.
    /// </summary>
    /// <param name="xiTexX"></param>
    /// <param name="xiTexY"></param>
    /// <param name="xiSteeringPxX"></param>
    /// <param name="xiSteeringPxY"></param>
    /// <param name="xiNewVal"></param>
    ///========================================================================
    private void UpdateSteeringPixel(
      int xiTexX,  //the x-offset of the tex square in the grid
      int xiTexY,
      int xiSteeringPxX, //the x-offset of the steering pixel in the square
      int xiSteeringPxY,
      byte xiNewVal)
    {
      byte lSteeringImageIdx = mSubject.TexMetaData[xiTexX][xiTexY][(int)eTexMetaDataEntries.Steering];
      byte lOrientation = mSubject.TexMetaData[xiTexX][xiTexY][(int)eTexMetaDataEntries.Orientation];

      if (lSteeringImageIdx >= mSteeringImageUsageCountArray.Length)
      {
        MessageBox.Show("Cannot edit that steering square: it indexes a non-existant steering image. Please edit it numerically first");
        return;
      }

      //=======================================================================
      // Re-orient the target value based on the actual orientation of the
      // square. Note that bytes wrap round from 0 to 255 if you add or subtract
      // to give an out-of-range value.
      //=======================================================================
      if (lOrientation > 0 && lOrientation < 4)
      {
        xiNewVal -= (byte)(16 - lOrientation * 4);

        if (xiNewVal >= 16)
        {
          xiNewVal += 16;
        }
      }
      else if (lOrientation == 0 && xiNewVal > 0)
      {
        xiNewVal = (byte)(16 - xiNewVal);
      }

      //how to update the steering pix depends on how many tex squares
      //use that steering pix
      switch (mSteeringImageUsageCountArray[lSteeringImageIdx])
      {
        case 0:
          throw new Exception("Interal error: unreachable statement!");

        case 1:
          SteeringImageChunk lSteeringChunk = mMainForm.CurrentLevel.GetSteeringImageById(lSteeringImageIdx);
          lSteeringChunk.SetPixelType(xiSteeringPxX, xiSteeringPxY, xiNewVal);
          break;

        default: // i.e. > 1
          SHETChunk lShet = mMainForm.CurrentLevel.SHET;
          SteeringImageChunk lNewSteering = null;
          int lNewSteeringId = -1;

          if (lShet.UnusedSteeringImages.Count == 0)
          {
            //=================================================================
            // Create a new steering image
            //=================================================================
            lNewSteeringId = lShet.SteeringImages.mChildren.Length;
            lNewSteering = new SteeringImageChunk(lNewSteeringId);
            int lSizeIncrease = lShet.AddSteeringImage(lNewSteering);
            lShet.TrailingZeroByteCount -= lSizeIncrease;

            if (lShet.TrailingZeroByteCount < 0)
            {
              MessageBox.Show(string.Format(
                "WARNING: You have just run out of space in your level file - you will need to free up {0} bytes before you can save your changes.",
                -lShet.TrailingZeroByteCount));
            }

            //=================================================================
            // Update our usage count array to include the new steering image
            //=================================================================
            int[] lNewSteeringImageUsageCountArray = new int[lNewSteeringId + 1];
            Array.Copy(mSteeringImageUsageCountArray, lNewSteeringImageUsageCountArray, mSteeringImageUsageCountArray.Length);
            mSteeringImageUsageCountArray = lNewSteeringImageUsageCountArray;
          }
          else
          {
            //=================================================================
            // Find the first unused steering image and use that
            //=================================================================
            foreach (DictionaryEntry lEntry in lShet.UnusedSteeringImages)
            {
              lNewSteeringId = (int)lEntry.Key;
              lNewSteering = (SteeringImageChunk)lEntry.Value;
              break;
            }

            lShet.UnusedSteeringImages.Remove(lNewSteeringId);
          }

          //===================================================================
          // Update the steering image with the desired contents, and update 
          // the terrain square to use it
          //===================================================================
          SteeringImageChunk lOldSteering = mMainForm.CurrentLevel.GetSteeringImageById(lSteeringImageIdx);
          lNewSteering.CopyFrom(lOldSteering);
          lNewSteering.SetPixelType(xiSteeringPxX, xiSteeringPxY, xiNewVal);
          mSubject.TexMetaData[xiTexX][xiTexY][(int)eTexMetaDataEntries.Steering]
           = (byte)lNewSteeringId;
          mSteeringImageUsageCountArray[lSteeringImageIdx]--;
          mSteeringImageUsageCountArray[lNewSteeringId]++;
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
           || ViewMode == eViewMode.EditSteeringSquares
           || ViewMode == eViewMode.EditSteeringPixels
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
            int x = p.X / mSubjectTileScaledWidth;
            int y = p.Y / mSubjectTileScaledHeight;
            int lPxX = (p.X % mSubjectTileScaledWidth) / (mSubjectTileScaledWidth / 8);
            int lPxY = (p.Y % mSubjectTileScaledHeight) / (mSubjectTileScaledHeight / 8);
            PictureBox lSel = mKeyOrMouseToSelPicBoxDict[e.KeyChar];
            BrushData lBrush = BrushData.FromPictureBox(lSel);
            if (ViewMode == eViewMode.EditTextures)
            {
              mSubject.TextureIds[x][y] = lBrush.Value;
            }
            else if (ViewMode == eViewMode.EditBumpSquares)
            {
              mSubject.TexMetaData[x][y][(int)eTexMetaDataEntries.Bumpmap] = lBrush.Value;
            }
            else if (ViewMode == eViewMode.EditBumpPixels)
            {
              UpdateBumpPixel(x, y, lPxX, lPxY, lBrush.Value);
            }
            else if (ViewMode == eViewMode.EditSteeringSquares)
            {
              mSubject.TexMetaData[x][y][(int)eTexMetaDataEntries.Steering] = lBrush.Value;
              mSubject.TexMetaData[x][y][(int)eTexMetaDataEntries.Orientation] = lBrush.Orientation;
            }
            else if (ViewMode == eViewMode.EditSteeringPixels)
            {
              UpdateSteeringPixel(x, y, lPxX, lPxY, lBrush.Value);
            }
            else if (ViewMode == eViewMode.EditCameras)
            {
              mSubject.TexMetaData[x][y][(int)eTexMetaDataEntries.CameraPos] = lBrush.Value;
            }
            else if (ViewMode == eViewMode.EditRespawns)
            {
              SetRespawnPosition(x, y, new RespawnSetting(lPxX, lPxY, lBrush.Value));
            }
            else if (ViewMode == eViewMode.EditWaypoints)
            {
              MakeWaypointChange(x, y, (eWaypointAction)lBrush.Value);
            }
            else if (ViewMode == eViewMode.EditBehaviours)
            {
              mSubject.TexMetaData[x][y][(int)eTexMetaDataEntries.Behaviour] = lBrush.Value;
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
        mSubject.TexMetaData[x][y][(byte)eTexMetaDataEntries.Behaviour] = FlatChunk.BEHAVIOUR_DEFAULTNORESPAWN;
        return;
      }
      else if (Array.IndexOf(sNoRespawnValues, mSubject.TexMetaData[x][y][(byte)eTexMetaDataEntries.Behaviour]) >= 0)
      {
        mSubject.TexMetaData[x][y][(byte)eTexMetaDataEntries.Behaviour] = FlatChunk.BEHAVIOUR_DEFAULT;
      }

      // Find a steering image which can be used for this direction, at one of the four orientations
      int lSteeringId = FindSteeringImageMatchingDirection(ref xiRespawnSetting);

      if (lSteeringId == int.MinValue)
      {
        xiRespawnSetting.RotateTo(eOrientation.North);

        if (lShet.UnusedSteeringImages.Count == 0)
        {
          lSteeringId = lShet.SteeringImages.mChildren.Length;
          SteeringImageChunk lNewSteeringImage = new SteeringImageChunk(
            lSteeringId,
            (byte)xiRespawnSetting.Direction);
          int lSizeIncrease = lShet.AddSteeringImage(lNewSteeringImage);
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
          foreach (DictionaryEntry lEntry in lShet.UnusedSteeringImages)
          {
            // Just get the first unused.
            lSteeringId = (int)lEntry.Key;
            SteeringImageChunk lSteeringImage = (SteeringImageChunk)lEntry.Value;
            lSteeringImage.FlushToValue((byte)xiRespawnSetting.Direction);
            break;
          }

          lShet.UnusedSteeringImages.Remove(lSteeringId);
        }
      }

      // Use 4 instead of 0 for orientation North, because orientation 0 
      // reverses the directions...
      mSubject.TexMetaData[x][y][(byte)eTexMetaDataEntries.Orientation] =
       xiRespawnSetting.Orientation == eOrientation.North ? (byte)4 : (byte)xiRespawnSetting.Orientation;
      mSubject.TexMetaData[x][y][(byte)eTexMetaDataEntries.Steering] = (byte)lSteeringId;
      
      // Rotate back to north to set position (layer seven)
      xiRespawnSetting.RotateTo(eOrientation.North);
      byte lRespawnValue = PointToRespawnPos(new Point(xiRespawnSetting.X, xiRespawnSetting.Y));
      mSubject.TexMetaData[x][y][(byte)eTexMetaDataEntries.RespawnPos] = lRespawnValue;
    }

    private byte PointToRespawnPos(Point xiPoint)
    {
      return (byte)(xiPoint.X * 16 + xiPoint.Y);
    }

    private int FindSteeringImageMatchingDirection(ref RespawnSetting xbRespawnSetting)
    {
      SHETChunk lShet = mMainForm.CurrentLevel.SHET;

      for (int i = 0; i < lShet.SteeringImages.mChildren.Length; i++)
      {
        if (!(lShet.SteeringImages.mChildren[i] is SteeringImageChunk))
        {
          continue;
        }

        SteeringImageChunk lSteeringImage = (SteeringImageChunk)lShet.SteeringImages.mChildren[i];

        if (xbRespawnSetting.MatchToSteeringImage(lSteeringImage))
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
          mCurrentWaypoint = GetDefaultWaypoint();
        }

        if ((mSubject == null || mSubject.TexMetaData == null)
         && (value == eViewMode.EditBumpPixels
          || value == eViewMode.EditBumpSquares
          || value == eViewMode.EditSteeringPixels
          || value == eViewMode.EditSteeringSquares
          || value == eViewMode.EditCameras
          || value == eViewMode.EditRespawns
          || value == eViewMode.EditWaypoints
          || value == eViewMode.EditBehaviours
          || value == eViewMode.EditMetadata
          || value == eViewMode.FillMetadata))
        {
          value = eViewMode.ViewOnly; //reject change!
        }

        if (mViewMode != value)
        {
          mViewMode = value;

          // update the editing palette
          //
          mMainForm.GridViewPalettePanel.SuspendLayout();
          mMainForm.GridViewPalettePanel.Controls.Clear();
          if (ViewMode == eViewMode.EditTextures
            || ViewMode == eViewMode.EditBumpSquares
            || ViewMode == eViewMode.EditBumpPixels
            || ViewMode == eViewMode.EditSteeringSquares
            || ViewMode == eViewMode.EditSteeringPixels
            || ViewMode == eViewMode.EditCameras
            || ViewMode == eViewMode.EditRespawns
            || ViewMode == eViewMode.EditWaypoints
            || ViewMode == eViewMode.EditBehaviours)
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
            for (int i = 0; i < (ViewMode == eViewMode.EditSteeringSquares ? 256 * 5 : 256); i++)
            {
              Image im = null;
              BrushData lBrush = null;

              //fetch or create the appropriate palette image for the byte value "i"
              if (ViewMode == eViewMode.EditTextures)
              {
                TIMChunk c = mMainForm.CurrentLevel.GetTileById(i);
                if (c != null) im = c.ToImage();
                if (im.Width != this.mSubjectTileNativeWidth
                 || im.Height != this.mSubjectTileNativeHeight)
                {
                  im = null;
                }

                lBrush = new BrushData((byte)i, "TIM " + i.ToString());
              }
              else if (ViewMode == eViewMode.EditBumpSquares)
              {
                BumpImageChunk bim = mMainForm.CurrentLevel.GetBumpById(i);
                if (bim != null)
                {
                  im = bim.ToImage();
                  lBrush = new BrushData((byte)i, "Bump " + i.ToString());
                }
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
                  lBrush = new BrushData((byte)i, "Bump type " + bti.Name + ": " + bti.Description);
                }
              }
              else if (ViewMode == eViewMode.EditSteeringSquares)
              {
                SteeringImageChunk lSteeringChunk = mMainForm.CurrentLevel.GetSteeringImageById((int)(i / 5));
                if (lSteeringChunk != null)
                {
                  im = lSteeringChunk.ToImage((byte)(i % 5), Pens.Black);
                  lBrush = new BrushData((byte)(i / 5), (byte)(i % 5), "Steering image " + ((byte)(i / 5)).ToString() + "; Orientation " + (i % 5).ToString());
                }
              }
              else if (ViewMode == eViewMode.EditSteeringPixels)
              {
                im = GetSteeringPixelBrush(i, out lBrush);
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
                if (cpc != null)
                {
                  im = cpc.ToImage();
                  lBrush = new BrushData((byte)i, "Camera pos " + i.ToString());
                }
              }
              else if (ViewMode == eViewMode.EditRespawns)
              {
                im = GetRespawnBrush(i, out lBrush);
              }
              else if (ViewMode == eViewMode.EditWaypoints)
              {
                im = GetWaypointBrush(i, out lBrush);
              }
              else if (ViewMode == eViewMode.EditBehaviours)
              {
                im = GetBehaviourBrush(i, out lBrush);
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
                lPalPB.BorderStyle = BorderStyle.FixedSingle; //qq:MWR
                lPalPB.Tag = lBrush;
                lPalPB.MouseClick += new MouseEventHandler(PaletteImageMouseClick);
                lPalPB.MouseMove += new MouseEventHandler(PaletteImageMouseMove);
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

          if (ViewMode == eViewMode.EditBumpPixels)
          {
            //=================================================================
            // Make a list of how many times each bump tex is used, to be used
            // in editing the bump map, pixel by pixel
            //=================================================================
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
          else if (ViewMode == eViewMode.EditSteeringPixels)
          {
            //=================================================================
            // Make a list of how many times each steering tex is used, to be 
            // used in editing the steering map, pixel by pixel
            //=================================================================
            mSteeringImageUsageCountArray = new int[mMainForm.CurrentLevel.SHET.SteeringImages.mChildren.Length];
            int lSteeringIdx = (int)eTexMetaDataEntries.Steering;
            foreach (FlatChunk flat in mMainForm.CurrentLevel.SHET.Flats)
            {
              if (flat.TexMetaData != null)
              {
                foreach (byte[][] row in flat.TexMetaData)
                {
                  foreach (byte[] entry in row)
                  {
                    int lSteeringId = entry[lSteeringIdx];
                    if (lSteeringId < mSteeringImageUsageCountArray.Length)
                    {
                      mSteeringImageUsageCountArray[lSteeringId]++;
                    }
                    else
                    {
                      Console.Error.WriteLine("Tex square in Flat {0} references non-existant steering id {1}",
                        flat.Name,
                        lSteeringId);
                    }
                  }
                }
              }
            }
          }

          //===================================================================
          // Make sure that we can view bumps, if we're editing them
          //===================================================================
          if ((ViewMode == eViewMode.EditBumpPixels ||
            ViewMode == eViewMode.EditBumpSquares) &&
            mMainForm.GridViewTransparencySlider.Value == mMainForm.GridViewTransparencySlider.Minimum)
          {
            mMainForm.GridViewTransparencySlider.Value =
              (mMainForm.GridViewTransparencySlider.Maximum - mMainForm.GridViewTransparencySlider.Minimum) / 2;
          }
        }

        if (OnViewModeChanged != null) OnViewModeChanged(this, null);
        InvalidateGridDisplay();
      }
    }

    void PaletteImageMouseMove(object sender, MouseEventArgs e)
    {
      string lStatusText = BrushData.FromPictureBox(sender).Caption;
      mMainForm.GridViewerStatusLabel.Text = lStatusText;
    }

    private Image GetRespawnBrush(int i, out BrushData xoBrush)
    {
      Bitmap lRet = new Bitmap(4 * SCALE, 4 * SCALE);
      Graphics g = Graphics.FromImage(lRet);
      Pen p = new Pen(Color.Red, 1);
      Point lMidpoint = new Point(2 * SCALE, 2 * SCALE);

      if (i == 0)
      {
        Utils.DrawCross(g, p, lMidpoint, (int)(mSubjectTileScaledWidth * 0.8));
        xoBrush = new BrushData((byte)i, "No respawn (Warning: Corrupts Behaviours)");
        return lRet;
      }
      else if (i <= FlatChunk.STEERING_HIGHESTDIRECTION + 1)
      {
        g.DrawRectangle(p, 0, 0, 63, 63);
        Utils.DrawArrow(g, p, lMidpoint, 4096 - ((i - 1) * 256), (int)(mSubjectTileScaledWidth * 0.4), 1, true);
        xoBrush = new BrushData((byte)i, "Respawn direction " + i.ToString() + " (Warning: Corrupts Steering)");
        return lRet;
      }
      else
      {
        xoBrush = null;
        return null;
      }
    }

    ///========================================================================
    /// Method : GetSteeringPixelBrush
    /// 
    /// <summary>
    /// 	Get the brush for pixel-by-pixel editing of steering controls
    /// </summary>
    /// <param name="i"></param>
    /// <returns></returns>
    ///========================================================================
    private Image GetSteeringPixelBrush(int i, out BrushData xoBrush)
    {
      Bitmap lRet = new Bitmap(4 * SCALE, 4 * SCALE);
      Graphics g = Graphics.FromImage(lRet);
      Pen p = new Pen(Color.Black, 1);
      Point lMidpoint = new Point(2 * SCALE, 2 * SCALE);

      if (i <= FlatChunk.STEERING_HIGHESTDIRECTION)
      {
        g.DrawRectangle(p, 0, 0, 63, 63);
        Utils.DrawArrow(g, p, lMidpoint, 4096 - (i * 256), (int)(mSubjectTileScaledWidth * 0.4), 1, true);
        xoBrush = new BrushData((byte)i, "Steering direction " + SteeringImageChunk.GetDirectionName((byte)i));
        return lRet;
      }
      else
      {
        xoBrush = null;
        return null;
      }
    }

    private Image GetBehaviourBrush(int i, out BrushData xoBrush)
    {
      if (i >= FlatChunk.Behaviours.Length)
      {
        xoBrush = null;
        return null;
      }

      xoBrush = new BrushData((byte)i, FlatChunk.Behaviours[i].BehaviourTypes.ToString());
      return FlatChunk.Behaviours[i].ToBitmap();
    }

    private Image GetWaypointBrush(int i, out BrushData xoBrush)
    {
      if (i > (int)eWaypointAction.Restart)
      {
        xoBrush = null;
        return null;
      }

      string lLabel = "";

      switch (i)
      {
        case (int)eWaypointAction.AddPath:
          lLabel = "Add path";
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

      xoBrush = new BrushData((byte)i, lLabel);

      return lRet;
    }

    public event EventHandler OnViewModeChanged;

    #endregion

    private int[] mBumpImageUsageCountArray;
    private int[] mSteeringImageUsageCountArray;

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

    #region OverlaySteeringColor property

    private Color mOverlaySteeringColor = Color.Transparent;
    public Color OverlaySteeringColor
    {
      get { return mOverlaySteeringColor; }
      set
      {
        mOverlaySteeringColor = value;
        if (OnOverlaySteeringColorChanged != null) OnOverlaySteeringColorChanged(this, null);
        InvalidateGridDisplay();
      }
    }
    public event EventHandler OnOverlaySteeringColorChanged;

    #endregion

    #region OverlayBehaviourColor property

    private Color mOverlayBehaviourColor = Color.Transparent;
    public Color OverlayBehaviourColor
    {
      get { return mOverlayBehaviourColor; }
      set
      {
        mOverlayBehaviourColor = value;
        if (OnOverlayBehaviourColorChanged != null) OnOverlayBehaviourColorChanged(this, null);
        InvalidateGridDisplay();
      }
    }
    public event EventHandler OnOverlayBehaviourColorChanged;

    #endregion

    #region Enums

    public enum eViewMode
    {
      ViewOnly,
      EditTextures,
      EditBumpSquares,
      EditBumpPixels,
      EditSteeringSquares,
      EditSteeringPixels,
      EditCameras,
      EditRespawns,
      EditWaypoints,
      EditBehaviours,
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
      AddPath = 0,
      Add = 1,
      Duplicate = 2,
      Decrement = 3,
      Eyedropper = 4,
      Erase = 5,
      Waypoints = 6,
      Restart = 7
    }

    #endregion

    #region Helper classes

    ///========================================================================
    /// Class : BrushData
    /// 
    /// <summary>
    /// 	Class to hold data associated with a brush
    /// </summary>
    ///========================================================================
    private class BrushData
    {
      public byte Value;
      public byte Orientation; // Only applicable to EditSteeringSquares
      public string Caption;

      public BrushData(byte xiValue, byte xiOrientation, string xiCaption)
      {
        Value = xiValue;
        Orientation = xiOrientation;
        Caption = xiCaption;
      }

      public BrushData(byte xiValue, string xiCaption)
        : this(xiValue, 0, xiCaption)
      {
      }

      public static BrushData FromPictureBox(object xiPictureBox)
      {
        return xiPictureBox == null ? null : (BrushData)((PictureBox)xiPictureBox).Tag;
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

    private class RespawnSetting
    {
      public RespawnSetting(int x, int y, int xiNewDirectionValue)
      {
        X = x;
        Y = y;
        Orientation = eOrientation.North;

        if (xiNewDirectionValue > FlatChunk.STEERING_HIGHESTDIRECTION + 1)
        {
          throw new Exception("Tried to set respawn direction to a value higher than the maximum");
        }

        // The direction value passed in is based on the brush numbering and so needs to be reduced by one.
        Direction = (eDirection)(xiNewDirectionValue - 1);
      }

      public bool MatchToSteeringImage(SteeringImageChunk xiSteeringImage)
      {
        eOrientation lOriginalOrientation = Orientation;

        for (int lOrientation = (int)eOrientation.North; (int)lOrientation <= (int)eOrientation.West; lOrientation++)
        {
          RotateTo((eOrientation)lOrientation);

          // y needs to be flipped.
          if (xiSteeringImage.GetPixelType(X, (FlatChunk.STEERINGDIMENSION - 1) - Y) == (byte)Direction)
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
        int lMaxValue = FlatChunk.STEERINGDIMENSION - 1;

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

