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
      MoveScale = 100;

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
      PropertyController lMoveCtrl = new PropertyController(this, "MovementMode");
      mOptionsMenu.DropDownItems.AddRange(lMoveCtrl.CreateMenuItems());
      mOptionsMenu.DropDownItems.Add(new ToolStripSeparator());
      //
      PropertyController lNormCtrl = new PropertyController(this, "DrawNormalsMode");
      mOptionsMenu.DropDownItems.AddRange(lNormCtrl.CreateMenuItems());
      mOptionsMenu.DropDownItems.Add(new ToolStripSeparator());
      //
      PropertyController lTexModeCtrl = new PropertyController(this, "TextureMode");
      mOptionsMenu.DropDownItems.AddRange(lTexModeCtrl.CreateMenuItems());
      mOptionsMenu.DropDownItems.Add(new ToolStripSeparator());
      //
      PropertyController lSelMetaCtrl = new PropertyController(this, "SelectedMetadata");
      mOptionsMenu.DropDownItems.Add(lSelMetaCtrl.CreateToolStripComboBox());
      mOptionsMenu.DropDownItems.Add(new ToolStripSeparator());
      //
      mOptionsMenu.DropDownItems.Add(new ToolStripMenuItem("Hide all invisible Flats", null, new EventHandler(this.HideAllInvisibleFlats)));
      mOptionsMenu.DropDownItems.Add(new ToolStripMenuItem("Refresh View", null, new EventHandler(this.RefreshView)));
      mMainForm.mMenuStrip.Items.Add(mOptionsMenu);

      ResetCamera();

      MovementMode = eMovementMode.FlyMode;
      DrawNormalsMode = eDrawNormalsMode.HideNormals;
      TextureMode = eTextureMode.NormalTextures;
      SelectedMetadata = eTexMetaDataEntries.Waypoint;
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

      MMEdEditorView lView = new MMEdEditorView(this, xiScene, xiCamera, lRenderer);

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

      if (mDraggingButton == MouseButtons.Left)
      {
        if (Control.ModifierKeys == Keys.Alt && lCamera.ProjectionMode == eProjectionMode.Orthographic)
        {
          IPositionable lObject = ActiveObject as IPositionable;
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
          IRotatable lObject = ActiveObject as IRotatable;
          if (lObject != null)
          {
            double lDelta = (double)(lNewMousePoint.Y - mLastMouseDown.Y);
            lObject.RotationVector = Utils.RotationMatrixToShort3Coord(
              Matrix.Rotation(lDelta / 100, lCamera.ZAxis) * Utils.Short3CoordToRotationMatrix(lObject.RotationVector));
            RebuildObject(lObject);
            InvalidateAllViewers();
          }
        }
        else if (lCamera.ProjectionMode == eProjectionMode.Perspective)
        {
          switch (MovementMode)
          {
            case eMovementMode.FlyMode:
              bool lCameraIsUpsideDown = lCamera.YAxis.Dot(Vector.ZAxis) < 0;
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
            double lScale = (lCamera.Position.GetPositionVector() * lCamera.ZAxis) / lSender.Width;
            lCamera.Move(-lScale * (lNewMousePoint.X - mLastMouseDown.X) * lCamera.XAxis);
            lCamera.Move(lScale * (lNewMousePoint.Y - mLastMouseDown.Y) * lCamera.YAxis);
          }
          else
          {
            if (lCamera.ProjectionMode == eProjectionMode.Perspective && MovementMode == eMovementMode.FlyMode)
            {
              lCamera.Move(0.1 * MoveScale * (lNewMousePoint.X - mLastMouseDown.X) * lCamera.XAxis);
              lCamera.Move(0.1 * MoveScale * (lNewMousePoint.Y - mLastMouseDown.Y) * lCamera.YAxis);
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
      if (mActiveSurface == null)
      {
        return;
      }

      Camera lCamera = mViews[mActiveSurface].Camera;

      if (lCamera.ProjectionMode != eProjectionMode.Orthographic)
      {
        return;
      }

      if (Control.ModifierKeys == Keys.Shift)
      {
        IRotatable lObject = ActiveObject as IRotatable;
        if (lObject == null)
        {
          return;
        }

        double lDelta;
        switch (e.KeyCode)
        {
          case Keys.Up:
            lDelta = 0.1;
            break;

          case Keys.Down:
            lDelta = -0.1;
            break;

          case Keys.Right:
            lDelta = 0.1;
            break;

          case Keys.Left:
            lDelta = -0.1;
            break;

          default:
            return;
        }

        if (Control.ModifierKeys != Keys.Control)
        {
          lDelta /= 5;
        }

        lObject.RotationVector = Utils.RotationMatrixToShort3Coord(
          Matrix.Rotation(lDelta, lCamera.ZAxis) * Utils.Short3CoordToRotationMatrix(lObject.RotationVector));
        RebuildObject(lObject);
        InvalidateAllViewers();
      }
      else
      {
        IPositionable lObject = ActiveObject as IPositionable;
        if (lObject == null)
        {
          return;
        }

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
      }

      RebuildObject(ActiveObject);
      InvalidateAllViewers();
    }

    void mMainForm_FormClosing(object sender, FormClosingEventArgs e)
    {
      mMainForm.Viewer3DRenderingSurfaceTopRight.Release();
      mMainForm.Viewer3DRenderingSurfaceTopLeft.Release();
      mMainForm.Viewer3DRenderingSurfaceBottomLeft.Release();
      mMainForm.Viewer3DRenderingSurfaceBottomRight.Release();
    }

    public void HideAllInvisibleFlats(object xiSender, EventArgs xiArgs)
    {
      foreach (FlatChunk f in mSubject.SHET.Flats)
      {
        f.TreeNode.Checked = !f.Visible;
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

    private Level mSubject = null;

    MMEdScene mScene;
    Dictionary<RenderingSurface, MMEdEditorView> mViews = new Dictionary<RenderingSurface, MMEdEditorView>();

    ToolStripMenuItem mOptionsMenu;

    public override void SetSubject(Chunk xiChunk)
    {
      mOptionsMenu.Visible = (mSubject != null);

      if (xiChunk == null)
      {
        if (mMainForm.ViewerTabControl.SelectedTab == null
          || !(mMainForm.ViewerTabControl.SelectedTab.Tag is ThreeDeeViewer))
        {
          // the view has switched to another tab - reset the tree
          mMainForm.ChunkTreeView.CheckBoxes = false;
        }
        return;
      }

      if (mSubject != mMainForm.CurrentLevel)
      {
        mSubject = mMainForm.CurrentLevel;
        RebuildScene();
        ResetCamera();
      }

      mMainForm.ChunkTreeView.CheckBoxes = (mSubject != null);

      ActiveObject = xiChunk;
    }

    private void ResetCamera()
    {
      MMEdEditorView lTR = mViews[mMainForm.Viewer3DRenderingSurfaceTopRight];
      lTR.Camera.Position = new GLTK.Point(-3 * MoveScale, -3 * MoveScale, 3 * MoveScale);
      lTR.Camera.LookAt(new GLTK.Point(3 * MoveScale, 3 * MoveScale, 0), new GLTK.Vector(0, 0, 1));

      MMEdEditorView lTL = mViews[mMainForm.Viewer3DRenderingSurfaceTopLeft];
      lTL.Camera.Position = new GLTK.Point(0, 0, -5000);
      lTL.Camera.LookAt(new GLTK.Point(0, 0, 0), GLTK.Vector.XAxis);

      MMEdEditorView lBL = mViews[mMainForm.Viewer3DRenderingSurfaceBottomLeft];
      lBL.Camera.Position = new GLTK.Point(-5000, 0, 0);
      lBL.Camera.LookAt(new GLTK.Point(0, 0, 0), GLTK.Vector.ZAxis);

      MMEdEditorView lBR = mViews[mMainForm.Viewer3DRenderingSurfaceBottomRight];
      lBR.Camera.Position = new GLTK.Point(0, -5000, 0);
      lBR.Camera.LookAt(new GLTK.Point(0, 0, 0), GLTK.Vector.ZAxis);
    }

    private void RefreshView(object xiSender, System.EventArgs xiArgs)
    {
      RebuildScene();
    }

    private void RebuildScene()
    {
      mScene.Clear();
      if (mSubject != null)
        mScene.AddRange(mSubject.GetEntities(mSubject, TextureMode, SelectedMetadata));
    }

    private void RebuildObject(object xiObject)
    {
      IEntityProvider lObject = xiObject as IEntityProvider;
      if (lObject != null)
      {
        IEnumerable<Entity> lNew = lObject.GetEntities(mSubject, TextureMode, SelectedMetadata);
        //qq this hacky
        foreach (Entity lNewEntity in lNew)
        {
          mScene.RemoveMMEdObject(xiObject);
          mScene.AddObject(lNewEntity);
          UpdateActiveObjectStatus(xiObject);
          return;
        }

        RebuildScene();
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
        if (OnMovementModeChanged != null) OnMovementModeChanged(this, null);
        InvalidateAllViewers();
      }
    }
    public event EventHandler OnMovementModeChanged;

    #endregion

    #region DrawNormalsMode property

    private eDrawNormalsMode mDrawNormalsMode;
    public eDrawNormalsMode DrawNormalsMode
    {
      get { return mDrawNormalsMode; }
      set
      {
        mDrawNormalsMode = value;
        if (OnDrawNormalsModeChanged != null) OnDrawNormalsModeChanged(this, null);
        InvalidateAllViewers();
      }
    }
    public event EventHandler OnDrawNormalsModeChanged;

    #endregion

    #region TextureMode property

    private eTextureMode mTextureMode;
    public eTextureMode TextureMode
    {
      get { return mTextureMode; }
      set
      {
        mTextureMode = value;
        if (OnTextureModeChanged != null) OnTextureModeChanged(this, null);
        RebuildScene();
        InvalidateAllViewers();
      }
    }
    public event EventHandler OnTextureModeChanged;

    #endregion

    #region SelectedMetadata property

    private eTexMetaDataEntries mSelectedMetadata;

    public eTexMetaDataEntries SelectedMetadata
    {
      get { return mSelectedMetadata; }
      set
      {
        mSelectedMetadata = value;
        if (OnSelectedMetadataChanged != null) OnSelectedMetadataChanged(this, null);
        RebuildScene();
        InvalidateAllViewers();
      }
    }

    public event EventHandler OnSelectedMetadataChanged;

    #endregion

    private void InvalidateAllViewers()
    {
      foreach (MMEdEditorView lView in mViews.Values)
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
      string lStatus = "";

      Chunk lChunk = xiObject as Chunk;
      if (lChunk != null)
      {
        lStatus += lChunk.Name + " - ";
      }

      IPositionable lPos = xiObject as IPositionable;
      if (lPos != null)
      {
        lStatus += string.Format(" Pos: ({0},{1},{2})",
          lPos.OriginPosition.X,
          lPos.OriginPosition.Y,
          lPos.OriginPosition.Z);
      }

      IRotatable lRot = xiObject as IRotatable;
      if (lRot != null)
      {
        lStatus += string.Format(" Rot: ({0},{1},{2})",
          lRot.RotationVector.X,
          lRot.RotationVector.Y,
          lRot.RotationVector.Z);
      }

      mMainForm.ThreeDeeEditorStatusLabel.Text = lStatus;
    }

    private object mActiveObject = null;

  }
}
