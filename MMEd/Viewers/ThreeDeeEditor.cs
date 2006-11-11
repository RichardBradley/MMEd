using System;
using System.Collections.Generic;
using System.Text;
using MMEd;
using MMEd.Chunks;
using MMEd.Util;
using System.Drawing;
using System.Windows.Forms;
using GLTK;
using MMEd.Viewers.ThreeDee;


namespace MMEd.Viewers
{
  public class ThreeDeeEditor : Viewer
  {
    private double MoveScale = 100;

    private ThreeDeeEditor(MainForm xiMainForm)
      : base(xiMainForm)
    {
      mMainForm.KeyPreview = true;
      mMainForm.DialogKey +=new KeyEventHandler(MainForm_KeyDown);
      mMainForm.FormClosing += new FormClosingEventHandler(mMainForm_FormClosing);

      mMainForm.ChunkTreeView.NodeMouseClick += new TreeNodeMouseClickEventHandler(ChunkTreeView_NodeMouseClick);

      mScene = new MMEdScene();

      Camera lCameraBottomLeft = new Camera(80, 0.1, 1e10);
      lCameraBottomLeft.ProjectionMode = eProjectionMode.Orthographic;
      CreateView(mMainForm.Viewer3DRenderingSurfaceBottomLeft, mScene, lCameraBottomLeft, RenderMode.Wireframe);

      Camera lCameraBottomRight = new Camera(80, 0.1, 1e10);
      lCameraBottomRight.ProjectionMode = eProjectionMode.Orthographic;
      CreateView(mMainForm.Viewer3DRenderingSurfaceBottomRight, mScene, lCameraBottomRight, RenderMode.Wireframe);

      CreateView(mMainForm.Viewer3DRenderingSurfaceTopRight, mScene, new Camera(80, 0.1, 1e10), RenderMode.Undefined);

      Camera lCameraTopLeft = new Camera(80, 0.1, 1e10);
      lCameraTopLeft.ProjectionMode = eProjectionMode.Orthographic;
      CreateView(mMainForm.Viewer3DRenderingSurfaceTopLeft, mScene, lCameraTopLeft, RenderMode.Wireframe);

      //add view mode menus:
      mOptionsMenu = new ToolStripMenuItem("3D");
      //
      PropertyController lMoveCtrl = new PropertyController(this, "MovementMode", "OnMovementModeChange");
      mOptionsMenu.DropDownItems.AddRange(lMoveCtrl.CreateMenuItems());
      mOptionsMenu.DropDownItems.Add(new ToolStripSeparator());
      //
      PropertyController lNormCtrl = new PropertyController(this, "DrawNormalsMode", "OnDrawNormalsModeChange");
      mOptionsMenu.DropDownItems.AddRange(lNormCtrl.CreateMenuItems());
      mOptionsMenu.DropDownItems.Add(new ToolStripSeparator());
      //
      PropertyController lTexModeCtrl = new PropertyController(this, "TextureMode", "OnTextureModeChange");
      mOptionsMenu.DropDownItems.AddRange(lTexModeCtrl.CreateMenuItems());
      mOptionsMenu.DropDownItems.Add(new ToolStripSeparator());
      //
      PropertyController lSelMetaCtrl = new PropertyController(this, "SelectedMetadata", "OnSelectedMetadataChange");
      mOptionsMenu.DropDownItems.Add(lSelMetaCtrl.CreateToolStripComboBox());
      mOptionsMenu.DropDownItems.Add(new ToolStripSeparator());
      //
      mOptionsMenu.DropDownItems.Add(new ToolStripMenuItem("Hide all Flats without FlgD", null, new EventHandler(this.HideAllFlatsWithoutFlgDClicked)));
      mMainForm.mMenuStrip.Items.Add(mOptionsMenu);
    }

    void CreateView(RenderingSurface xiSurface, Scene xiScene, Camera xiCamera, RenderMode xiFixedRenderMode)
    {
      xiSurface.MouseDown += new MouseEventHandler(Viewer3DRenderingSurface_MouseDown);
      xiSurface.MouseUp += new MouseEventHandler(Viewer3DRenderingSurface_MouseUp);
      xiSurface.MouseMove += new MouseEventHandler(Viewer3DRenderingSurface_MouseMove);
      xiSurface.MouseWheel += new MouseEventHandler(Viewer3DRenderingSurface_MouseWheel);
      xiSurface.GotFocus += new EventHandler(Viewer3DRenderingSurface_GotFocus);

      ImmediateModeRenderer lRenderer = new ImmediateModeRenderer();
      lRenderer.FixedRenderMode = xiFixedRenderMode;
      lRenderer.Attach(xiSurface);

      MMEdView lView = new MMEdView(this, xiScene, xiCamera, lRenderer);

      mViews.Add(xiSurface, lView);
    }

    void Viewer3DRenderingSurface_GotFocus(object sender, EventArgs e)
    {
      mActiveSurface = sender as RenderingSurface;
    }

    RenderingSurface mActiveSurface = null;

    void ChunkTreeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
    {
      RebuildScene();
      InvalidateAllViewers();
    }

    void Viewer3DRenderingSurface_MouseWheel(object sender, MouseEventArgs e)
    {
      Camera lCamera = mViews[(RenderingSurface)sender].Camera;
      bool lCameraIsUpsideDown = lCamera.YAxis.Dot(Vector.ZAxis) < 0;
      double lDelta = -0.02 * (double)e.Delta;
      Vector lStartPos = lCamera.Position.GetPositionVector();
      Vector lMoveVec = MoveScale * lDelta * lCamera.ZAxis;
      //don't move through the origin
      if (lMoveVec.Dot(lStartPos) / lStartPos.LengthSquared > -1.0)
      {
        lCamera.Move(lMoveVec);
        ((RenderingSurface)sender).Invalidate();
      }
    }

    void Viewer3DRenderingSurface_MouseMove(object sender, MouseEventArgs e)
    {
      if (!mDraggingView || mSubject == null) return;
      System.Drawing.Point lNewMousePoint = e.Location;

      RenderingSurface lSender = sender as RenderingSurface;
      Camera lCamera = mViews[lSender].Camera;

      bool lCameraIsUpsideDown = lCamera.YAxis.Dot(Vector.ZAxis) < 0;
      if (mDraggingButton == MouseButtons.Left)
      {
        if (Control.ModifierKeys == Keys.Alt && lCamera.ProjectionMode == eProjectionMode.Orthographic)
        {
          FlatChunk.ObjectEntry lObject = ActiveObject as FlatChunk.ObjectEntry;
          if (lObject != null)
          {
            double lDeltaX = (lNewMousePoint.X - mLastMouseDown.X);
            // Origin is at bottom-left, but mouse position is measured from orign at top-left (or vice versa)
            double lDeltaY = -(lNewMousePoint.Y - mLastMouseDown.Y);
            double lScale = (lCamera.Position.GetPositionVector() * lCamera.ZAxis) / lSender.Width;
            Vector lDelta = ((lDeltaX * lCamera.XAxis) + (lDeltaY * lCamera.YAxis)) * lScale;
            lObject.OriginPosition.X += (short)(lDelta.x);
            lObject.OriginPosition.Y += (short)(lDelta.y);
            // MMs has a left handed coordinate system, so the z-axis is pointing the "wrong" way
            lObject.OriginPosition.Z -= (short)(lDelta.z);
            RebuildObject(lObject);
            InvalidateAllViewers();
          }
        }
        else if (Control.ModifierKeys == Keys.Shift)
        {
        }
        else if (lCamera.ProjectionMode == eProjectionMode.Perspective)
        {
          switch (MovementMode)
          {
            case eMovementMode.FlyMode:
              lCamera.Rotate(0.01 * (lNewMousePoint.X - mLastMouseDown.X), lCameraIsUpsideDown ? Vector.ZAxis : -Vector.ZAxis);
              lCamera.Rotate(0.01 * (lNewMousePoint.Y - mLastMouseDown.Y), lCamera.XAxis);
              break;
            case eMovementMode.InspectMode:
              lCamera.RotateAboutWorldOrigin(0.01 * (lNewMousePoint.X - mLastMouseDown.X), lCamera.YAxis);
              lCamera.RotateAboutWorldOrigin(0.01 * (lNewMousePoint.Y - mLastMouseDown.Y), lCamera.XAxis);
              break;
            default: throw new Exception("Unreachable case");
          }
        }
      }
      if (mDraggingButton == MouseButtons.Right)
      {
        if (Control.ModifierKeys == Keys.Alt)
        {
        }
        else if (Control.ModifierKeys == Keys.Shift)
        {
        }
        else
        {
          if (lCamera.ProjectionMode == eProjectionMode.Orthographic)
          {
            lCamera.Move(-0.1 * MoveScale * (lNewMousePoint.X - mLastMouseDown.X) * lCamera.XAxis);
            lCamera.Move(0.1 * MoveScale * (lNewMousePoint.Y - mLastMouseDown.Y) * lCamera.YAxis);
          }
          else
          {
            if (lCamera.ProjectionMode == eProjectionMode.Perspective && MovementMode == eMovementMode.FlyMode)
            {
              lCamera.Move(-0.1 * MoveScale * (lNewMousePoint.X - mLastMouseDown.X) * lCamera.XAxis);
              lCamera.Move(-0.1 * MoveScale * (lNewMousePoint.Y - mLastMouseDown.Y) * lCamera.YAxis);
            }
          }
        }
      }
      mLastMouseDown = lNewMousePoint;
      lSender.Invalidate();
    }

    void Viewer3DRenderingSurface_MouseUp(object sender, MouseEventArgs e)
    {
      mDraggingView = false;
      ((RenderingSurface)sender).Capture = false;
    }

    bool mDraggingView = false;
    MouseButtons mDraggingButton;
    System.Drawing.Point mLastMouseDown;

    void Viewer3DRenderingSurface_MouseDown(object sender, MouseEventArgs e)
    {
      Camera lCamera = mViews[(RenderingSurface)sender].Camera;

      if (Control.ModifierKeys != Keys.Control)
      {
        ((RenderingSurface)sender).Capture = true;
        mLastMouseDown = e.Location;
        mDraggingButton = e.Button;
        mDraggingView = true;
      }
      else if (e.Button == MouseButtons.Left)
      {
        OwnedMesh lMesh = mViews[(RenderingSurface)sender].Renderer.Pick(e.X, e.Y) as OwnedMesh;
        if (lMesh != null)
        {
          ActiveObject = lMesh.Owner;
        }
      }

      ((RenderingSurface)sender).Focus();
    }

    void MainForm_KeyDown(object sender, KeyEventArgs e)
    {
      FlatChunk.ObjectEntry lObject = ActiveObject as FlatChunk.ObjectEntry;
      if (lObject == null)
      {
        return;
      }

      if (mActiveSurface == null)
      {
        return;
      }

      Camera lCamera = mViews[mActiveSurface].Camera;

      Vector lDelta;
      switch (e.KeyCode)
      {
        case Keys.Up:
          lDelta = lCamera.YAxis;
          break;

        case Keys.Down:
          lDelta = -lCamera.YAxis;
          break;

        case Keys.Right:
          lDelta = lCamera.XAxis;
          break;

        case Keys.Left:
          lDelta = -lCamera.XAxis;
          break;

        default:
          return;
      }

      if (Control.ModifierKeys != Keys.Control)
      {
        lDelta *= 5;
      }

      lObject.OriginPosition.X += (short)(lDelta.x);
      lObject.OriginPosition.Y += (short)(lDelta.y);
      lObject.OriginPosition.Z -= (short)(lDelta.z);

      RebuildObject(lObject);
      InvalidateAllViewers();
    }

    void mMainForm_FormClosing(object sender, FormClosingEventArgs e)
    {
      mMainForm.Viewer3DRenderingSurfaceTopRight.Release();
      mMainForm.Viewer3DRenderingSurfaceTopLeft.Release();
      mMainForm.Viewer3DRenderingSurfaceBottomLeft.Release();
      mMainForm.Viewer3DRenderingSurfaceBottomRight.Release();
    }

    public void HideAllFlatsWithoutFlgDClicked(object xiSender, EventArgs xiArgs)
    {
      if (!(mSubject is Level))
      {
        MessageBox.Show("This action only applicable when viewing Levels");
        return;
      }
      foreach (FlatChunk f in ((Level)mSubject).SHET.Flats)
      {
        f.TreeNode.Checked = !f.FlgD;
      }
      ChunkTreeView_NodeMouseClick(null, null);
    }


    public override bool CanViewChunk(Chunk xiChunk)
    {
      return xiChunk is IEntityProvider;
    }

    // Create an instance of the viewer manager class
    public static Viewer InitialiseViewer(MainForm xiMainForm)
    {
      return new ThreeDeeEditor(xiMainForm);
    }

    private IEntityProvider mSubject = null;

    MMEdScene mScene;
    Dictionary<RenderingSurface, MMEdView> mViews = new Dictionary<RenderingSurface, MMEdView>();

    ToolStripMenuItem mOptionsMenu;

    public override void SetSubject(Chunk xiChunk)
    {
      if (!(xiChunk is IEntityProvider)) xiChunk = null;
      mOptionsMenu.Visible = (xiChunk != null);
      if (mSubject == xiChunk) return;
      bool lResetViewMode = true;
      if (xiChunk != null && mSubject != null && xiChunk.GetType() == mSubject.GetType())
        lResetViewMode = false;
      mSubject = (IEntityProvider)xiChunk;

      MoveScale = xiChunk is Level ? 100 : 100;

      Cursor prevCursor = mMainForm.Viewer3DRenderingSurfaceTopRight.Cursor;
      mMainForm.Viewer3DRenderingSurfaceTopRight.Cursor = Cursors.WaitCursor;
      RebuildScene();
      if (mSubject != null)
      {
        MMEdView lTR = mViews[mMainForm.Viewer3DRenderingSurfaceTopRight];
        lTR.Camera.Position = new GLTK.Point(-3 * MoveScale, -3 * MoveScale, 3 * MoveScale);
        lTR.Camera.LookAt(new GLTK.Point(3 * MoveScale, 3 * MoveScale, 0), new GLTK.Vector(0, 0, 1));

        MMEdView lTL = mViews[mMainForm.Viewer3DRenderingSurfaceTopLeft];
        lTL.Camera.Position = new GLTK.Point(0, 0, -5000);
        lTL.Camera.LookAt(new GLTK.Point(0, 0, 0), GLTK.Vector.XAxis);

        MMEdView lBL = mViews[mMainForm.Viewer3DRenderingSurfaceBottomLeft];
        lBL.Camera.Position = new GLTK.Point(-5000, 0, 0);
        lBL.Camera.LookAt(new GLTK.Point(0, 0, 0), GLTK.Vector.ZAxis);

        MMEdView lBR = mViews[mMainForm.Viewer3DRenderingSurfaceBottomRight];
        lBR.Camera.Position = new GLTK.Point(0, -5000, 0);
        lBR.Camera.LookAt(new GLTK.Point(0, 0, 0), GLTK.Vector.ZAxis);

        //set defaults
        if (lResetViewMode)
        {
          if (mSubject is TMDChunk)
          {
            MovementMode = eMovementMode.InspectMode;
            DrawNormalsMode = eDrawNormalsMode.HideNormals;
            TextureMode = eTextureMode.NormalTextures;
            SelectedMetadata = eTexMetaDataEntries.Waypoint;
          }
          else
          {
            MovementMode = eMovementMode.FlyMode;
            DrawNormalsMode = eDrawNormalsMode.HideNormals;
            TextureMode = eTextureMode.NormalTextures;
            SelectedMetadata = eTexMetaDataEntries.Waypoint;
          }
        }

        mMainForm.ChunkTreeView.CheckBoxes = (mSubject is Level);
      }
      else
      {
        mMainForm.ChunkTreeView.CheckBoxes = false;
      }
      mMainForm.Viewer3DRenderingSurfaceTopRight.Cursor = prevCursor;

      InvalidateAllViewers();
    }

    private void RebuildScene()
    {
      mScene.Clear();
      if (mSubject != null)
        mScene.AddRange(mSubject.GetEntities(mMainForm.Level, TextureMode, SelectedMetadata));
    }

    private void RebuildObject(FlatChunk.ObjectEntry xiObject)
    {
      if (mSubject is Level)
      {
        mScene.RemoveMMEdObject(xiObject);
        mScene.AddObject(xiObject.GetEntity(mSubject as Level, TextureMode, SelectedMetadata));
        UpdateActiveObjectStatus(xiObject);
      }
    }

    public static GLTK.Point Short3CoordToPoint(Short3Coord xiVal)
    {
      return new GLTK.Point(xiVal.X, xiVal.Y, xiVal.Z);
    }

    public override System.Windows.Forms.TabPage Tab
    {
      get { return mMainForm.ViewTab3dEditor; }
    }

    #region MovementMode property

    private eMovementMode mMovementMode;
    public eMovementMode MovementMode
    {
      get { return mMovementMode; }
      set
      {
        mMovementMode = value;
        if (value == eMovementMode.InspectMode)
        {
          foreach (GLTK.View lView in mViews.Values)
          {
            if (lView.Camera.ProjectionMode != eProjectionMode.Orthographic)
            {
              lView.Camera.LookAt(GLTK.Point.Origin, lView.Camera.YAxis);
            }
          }
        }
        if (OnMovementModeChange != null) OnMovementModeChange(this, null);
        InvalidateAllViewers();
      }
    }
    public event EventHandler OnMovementModeChange;

    #endregion

    #region DrawNormalsMode property

    private eDrawNormalsMode mDrawNormalsMode;
    public eDrawNormalsMode DrawNormalsMode
    {
      get { return mDrawNormalsMode; }
      set
      {
        mDrawNormalsMode = value;
        if (OnDrawNormalsModeChange != null) OnDrawNormalsModeChange(this, null);
        InvalidateAllViewers();
      }
    }
    public event EventHandler OnDrawNormalsModeChange;

    #endregion

    #region TextureMode property

    private eTextureMode mTextureMode;
    public eTextureMode TextureMode
    {
      get { return mTextureMode; }
      set
      {
        if (value == eTextureMode.BumpmapTextures)
        {
          MessageBox.Show("Not implemented yet");
          return;
        }
        mTextureMode = value;
        if (OnTextureModeChange != null) OnTextureModeChange(this, null);
        RebuildScene();
        InvalidateAllViewers();
      }
    }
    public event EventHandler OnTextureModeChange;

    #endregion

    #region SelectedMetadata property

    private eTexMetaDataEntries mSelectedMetadata;

    public eTexMetaDataEntries SelectedMetadata
    {
      get { return mSelectedMetadata; }
      set
      {
        mSelectedMetadata = value;
        if (OnSelectedMetadataChange != null) OnSelectedMetadataChange(this, null);
        RebuildScene();
        InvalidateAllViewers();
      }
    }

    public event EventHandler OnSelectedMetadataChange;

    #endregion

    private void InvalidateAllViewers()
    {
      foreach (MMEdView lView in mViews.Values)
      {
        lView.Renderer.RendereringSurface.Invalidate();
      }
    }

    public object ActiveObject
    {
      get
      {
        return mActiveObject;
      }
      set
      {
        if (mActiveObject != value)
        {
          mActiveObject = value;
          InvalidateAllViewers();
        }

        UpdateActiveObjectStatus(mActiveObject);
      }
    }

    private void UpdateActiveObjectStatus(object xiObject)
    {
      FlatChunk.ObjectEntry lObject = xiObject as FlatChunk.ObjectEntry;

      string lStatus = null;
      if (lObject != null)
      {
        lStatus = string.Format("Pos: ({0},{1},{2}) Rot: ({3},{4},{5})",
          lObject.OriginPosition.X,
          lObject.OriginPosition.Y,
          lObject.OriginPosition.Z,
          lObject.RotationVector.X,
          lObject.RotationVector.Y,
          lObject.RotationVector.Z);
      }

      mMainForm.ThreeDeeEditorStatusLabel.Text = lStatus;
    }

    private object mActiveObject = null;

  }
}
