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
      mMainForm.KeyPress += new KeyPressEventHandler(this.KeyPressHandle);
      mMainForm.FormClosing += new FormClosingEventHandler(mMainForm_FormClosing);

      mMainForm.ChunkTreeView.NodeMouseClick += new TreeNodeMouseClickEventHandler(ChunkTreeView_NodeMouseClick);

      mScene = new Scene();

      mLight = new Light();
      mLight.DiffuseIntensity = 0.1;
      mLight.SpecularIntensity = 0.02;

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
      PropertyController lLightCtrl = new PropertyController(this, "LightingMode", "OnLightingModeChange");
      mOptionsMenu.DropDownItems.AddRange(lLightCtrl.CreateMenuItems());
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
      xiSurface.GotFocus += new EventHandler(Viewer3DRenderingSurface_GotFocus);
      xiSurface.MouseWheel += new MouseEventHandler(Viewer3DRenderingSurface_MouseWheel);

      ImmediateModeRenderer lRenderer = new ImmediateModeRenderer();
      lRenderer.FixedRenderMode = xiFixedRenderMode;
      lRenderer.Attach(xiSurface);

      MMEdView lView = new MMEdView(this, xiScene, xiCamera, lRenderer);

      mViews.Add(xiSurface, lView);
    }

    void Viewer3DRenderingSurface_GotFocus(object sender, EventArgs e)
    {
      RenderingSurface lSurface = sender as RenderingSurface;

      if (mActiveSurface != null)
      {
        mActiveSurface.BackColor = Color.FromKnownColor(KnownColor.Control);
      }

      mActiveSurface = lSurface;
      lSurface.Parent.BackColor = Color.Red;
    }

    private RenderingSurface mActiveSurface = null;

    Light mLight;
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
      if (lCamera.ProjectionMode == eProjectionMode.Orthographic)
      {
        lCamera.Move(MoveScale * lDelta * lCamera.ZAxis);
        ((RenderingSurface)sender).Invalidate();
      }
      else if (MovementMode == eMovementMode.InspectMode)
      {
        Vector lStartPos = lCamera.Position.GetPositionVector();
        Vector lMoveVec = MoveScale * lDelta * lCamera.ZAxis;
        //don't move through the origin
        if (lMoveVec.Dot(lStartPos) / lStartPos.LengthSquared > -1.0)
        {
          lCamera.Move(lMoveVec);
          ((RenderingSurface)sender).Invalidate();
        }
      }
    }

    void Viewer3DRenderingSurface_MouseMove(object sender, MouseEventArgs e)
    {
      if (!mDraggingView || mSubject == null) return;
      System.Drawing.Point lNewMousePoint = e.Location;

      Camera lCamera = mViews[(RenderingSurface)sender].Camera;

      bool lCameraIsUpsideDown = lCamera.YAxis.Dot(Vector.ZAxis) < 0;
      if (mDraggingButton == MouseButtons.Left)
      {
        if (lCamera.ProjectionMode == eProjectionMode.Orthographic)
        {
          lCamera.Move(-0.1 * MoveScale * (lNewMousePoint.X - mLastMouseDown.X) * lCamera.XAxis);
          lCamera.Move(0.1 * MoveScale * (lNewMousePoint.Y - mLastMouseDown.Y) * lCamera.YAxis);
        }
        else
        {
          switch (MovementMode)
          {
            case eMovementMode.FlyMode:
              lCamera.Move(-0.1 * MoveScale * (lNewMousePoint.X - mLastMouseDown.X) * lCamera.XAxis);
              lCamera.Move(0.1 * MoveScale * (lNewMousePoint.Y - mLastMouseDown.Y) * lCamera.ZAxis);
              break;
            default: throw new Exception("Unreachable case");
          }
        }
      }
      if (mDraggingButton == MouseButtons.Right)
      {
        if (lCamera.ProjectionMode == eProjectionMode.Orthographic)
        {
          lCamera.Move(0.1 * MoveScale * (lNewMousePoint.Y - mLastMouseDown.Y) * lCamera.ZAxis);
        }
        else
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
      if (LightingMode == eLightingMode.Headlight)
      {
        //qq this replaces the matrix, rather than changes the values,
        // but that's OK for now
        mLight.Transform = lCamera.Transform;
      }
      mLastMouseDown = lNewMousePoint;
      ((RenderingSurface)sender).Invalidate();
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

      bool lDragging = false;
      if (lCamera.ProjectionMode == eProjectionMode.Orthographic)
      {
        lDragging = e.Button == MouseButtons.Right;
      }
      else if (MovementMode == eMovementMode.InspectMode)
      {
        lDragging = e.Button == MouseButtons.Right;
      }
      else
      {
        lDragging = (e.Button == MouseButtons.Left || e.Button == MouseButtons.Right);
      }

      if (lDragging)
      {
        ((RenderingSurface)sender).Capture = true;
        mLastMouseDown = e.Location;
        mDraggingButton = e.Button;
        mDraggingView = true;
      }
      else if (e.Button == MouseButtons.Left && sender == mActiveSurface)
      {
        Mesh lMesh = mViews[(RenderingSurface)sender].Renderer.Pick(e.X, e.Y);
        if (lMesh != null)
        {
          if (lMesh is OwnedMesh && ((OwnedMesh)lMesh).Owner is Chunk)
          {
            OwnedMesh om = (OwnedMesh)lMesh;
            Chunk c = (Chunk)om.Owner;
            //            MessageBox.Show(c.Name);
          }
          ActiveMesh = lMesh;
        }
      }

      ((RenderingSurface)sender).Focus();
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

    Scene mScene;
    Dictionary<RenderingSurface, MMEdView> mViews = new Dictionary<RenderingSurface, MMEdView>();

    ToolStripMenuItem mOptionsMenu;

    public override void SetSubject(Chunk xiChunk)
    {
      if (!(xiChunk is IEntityProvider)) xiChunk = null;
      if (mSubject == xiChunk) return;
      bool lResetViewMode = true;
      if (xiChunk != null && mSubject != null && xiChunk.GetType() == mSubject.GetType())
        lResetViewMode = false;
      mSubject = (IEntityProvider)xiChunk;

      mOptionsMenu.Visible = (mSubject != null);
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
        lTL.Camera.Position = new GLTK.Point(0, 0, 5000);
        lTL.Camera.LookAt(new GLTK.Point(0, 0, 0), GLTK.Vector.XAxis);

        MMEdView lBL = mViews[mMainForm.Viewer3DRenderingSurfaceBottomLeft];
        lBL.Camera.Position = new GLTK.Point(-5000, 0, 0);
        lBL.Camera.LookAt(new GLTK.Point(0, 0, 0), GLTK.Vector.ZAxis);

        MMEdView lBR = mViews[mMainForm.Viewer3DRenderingSurfaceBottomRight];
        lBR.Camera.Position = new GLTK.Point(0, 5000, 0);
        lBR.Camera.LookAt(new GLTK.Point(0, 0, 0), GLTK.Vector.ZAxis);

        //set defaults
        if (lResetViewMode)
        {
          if (mSubject is TMDChunk)
          {
            LightingMode = eLightingMode.None; //qq
            MovementMode = eMovementMode.InspectMode;
            DrawNormalsMode = eDrawNormalsMode.HideNormals;
            TextureMode = eTextureMode.NormalTextures;
            SelectedMetadata = FlatChunk.TexMetaDataEntries.Waypoint;
          }
          else
          {
            LightingMode = eLightingMode.None;
            MovementMode = eMovementMode.FlyMode;
            DrawNormalsMode = eDrawNormalsMode.HideNormals;
            TextureMode = eTextureMode.NormalTextures;
            SelectedMetadata = FlatChunk.TexMetaDataEntries.Waypoint;
          }
        }

        if (MovementMode == eMovementMode.InspectMode)
        {
          //qqAIT mLight.Transform = mCamera.Transform;
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

    #region LightingMode property

    private eLightingMode mLightingMode;
    public eLightingMode LightingMode
    {
      get { return mLightingMode; }
      set
      {/* qqAIT
        switch (value)
        {
          case eLightingMode.None:
            mRendererTopRight.DisableLighting();
            break;
          case eLightingMode.Headlight:
            mRendererTopRight.EnableLighting();
            mRendererTopRight.ResetLights();
            mLight.Transform = mCamera.Transform;
            mScene.AddLight(mLight);
            break;
          case eLightingMode.OverheadLight:
            MessageBox.Show("OverheadLight mode not supported yet");
            return;
        }
        mLightingMode = value;
        if (OnLightingModeChange != null) OnLightingModeChange(this, null);
        InvalidateViewer();*/
      }
    }
    public event EventHandler OnLightingModeChange;

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

    private FlatChunk.TexMetaDataEntries mSelectedMetadata;

    public FlatChunk.TexMetaDataEntries SelectedMetadata
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

    public Mesh ActiveMesh
    {
      get { return mActiveMesh; }
      set
      {
        if (value != mActiveMesh)
        {
          mActiveMesh = value;
          if (OnActiveMeshChange != null)
          {
            OnActiveMeshChange(value, EventArgs.Empty);
          }

          InvalidateAllViewers();
        }
      }
    }

    public event EventHandler OnActiveMeshChange;

    private Mesh mActiveMesh = null;

    // a 2d move request, which will be turned into 3d camera
    // movement, in a manner dependent on

    private void KeyPressHandle(object sender, KeyPressEventArgs e)
    {
      if (mMainForm.ViewerTabControl.SelectedTab != Tab) return;
      /* qqAIT
      switch (e.KeyChar)
      {
        case 'W':
        case 'w':
          mCamera.Move(-1.0 * mCamera.ZAxis * MoveScale);
          break;

        case 'S':
        case 's':
          mCamera.Move(1.0 * mCamera.ZAxis * MoveScale);
          break;

        case 'A':
        case 'a':
          mCamera.Move(-1.0 * mCamera.XAxis * MoveScale);
          break;

        case 'D':
        case 'd':
          mCamera.Move(1.0 * mCamera.XAxis * MoveScale);
          break;

        case 'Q':
        case 'q':
          mCamera.Move(-1.0 * mCamera.ZAxis * MoveScale);
          break;

        case 'E':
        case 'e':
          mCamera.Move(1.0 * mCamera.ZAxis * MoveScale);
          break;

        case 'I':
        case 'i':
          mCamera.Rotate(-0.1, mCamera.XAxis);
          break;

        case 'K':
        case 'k':
          mCamera.Rotate(0.1, mCamera.XAxis);
          break;

        case 'J':
        case 'j':
          mCamera.Rotate(-0.1, mCamera.YAxis);
          break;

        case 'L':
        case 'l':
          mCamera.Rotate(0.1, mCamera.YAxis);
          break;

        case 'U':
        case 'u':
          mCamera.Rotate(-0.1, mCamera.ZAxis);
          break;

        case 'O':
        case 'o':
          mCamera.Rotate(0.1, mCamera.ZAxis);
          break;
      }

      InvalidateViewer();*/
    }
  }
}
