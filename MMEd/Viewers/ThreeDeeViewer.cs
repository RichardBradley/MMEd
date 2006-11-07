using System;
using System.Collections.Generic;
using System.Text;
using MMEd;
using MMEd.Chunks;
using MMEd.Util;
using System.Drawing;
using System.Windows.Forms;
using GLTK;


namespace MMEd.Viewers
{
  public class ThreeDeeViewer : Viewer
  {
    private double MOVE_SCALE = 100;

    //indicates that an object can provide an enumeration of GLTK.Entity objects
    //to be redered as a ThreeDee scene
    public interface IEntityProvider
    {
      IEnumerable<Entity> GetEntities(AbstractRenderer xiRenderer, Level xiLevel, eTextureMode xiTextureMode, FlatChunk.TexMetaDataEntries xiSelectedMetadata);
    }

    private ThreeDeeViewer(MainForm xiMainForm)
      : base(xiMainForm)
    {
      mMainForm.KeyPreview = true;
      mMainForm.KeyPress += new KeyPressEventHandler(this.KeyPressHandle);
      mMainForm.FormClosing += new FormClosingEventHandler(mMainForm_FormClosing);
      mMainForm.Viewer3DRenderingSurface.MouseDown += new MouseEventHandler(Viewer3DRenderingSurface_MouseDown);
      mMainForm.Viewer3DRenderingSurface.MouseUp += new MouseEventHandler(Viewer3DRenderingSurface_MouseUp);
      mMainForm.Viewer3DRenderingSurface.MouseMove += new MouseEventHandler(Viewer3DRenderingSurface_MouseMove);
      mMainForm.ChunkTreeView.NodeMouseClick += new TreeNodeMouseClickEventHandler(ChunkTreeView_NodeMouseClick);

      mRenderer = new ImmediateModeRenderer();
      mRenderer.Attach(mMainForm.Viewer3DRenderingSurface);

      mScene = new Scene(mRenderer);
      mCamera = new Camera(80, 0.1, 1e10);
      mScene.Camera = mCamera;
      mLight = new Light();
      mLight.DiffuseIntensity = 0.1;
      mLight.SpecularIntensity = 0.02;

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
    Light mLight;
    void ChunkTreeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
    {
      RebuildScene();
      InvalidateViewer();
    }

    void Viewer3DRenderingSurface_MouseMove(object sender, MouseEventArgs e)
    {
      if (!mDraggingView || mSubject == null) return;
      System.Drawing.Point lNewMousePoint = e.Location;
      if (mDraggingButton == MouseButtons.Left)
      {
        bool lCameraIsUpsideDown = mCamera.YAxis.Dot(Vector.ZAxis) < 0;
        switch (MovementMode)
        {
          case eMovementMode.FlyMode:
            mCamera.Rotate(0.01 * (lNewMousePoint.X - mLastMouseDown.X), lCameraIsUpsideDown ? Vector.ZAxis : -Vector.ZAxis);
            mCamera.Rotate(0.01 * (lNewMousePoint.Y - mLastMouseDown.Y), mCamera.XAxis);
            break;
          case eMovementMode.InspectMode:
            mCamera.RotateAboutWorldOrigin(0.01 * (lNewMousePoint.X - mLastMouseDown.X), mCamera.YAxis);
            mCamera.RotateAboutWorldOrigin(0.01 * (lNewMousePoint.Y - mLastMouseDown.Y), mCamera.XAxis);
            break;
          default: throw new Exception("Unreachable case");
        }
      }
      else if (mDraggingButton == MouseButtons.Right)
      {
        switch (MovementMode)
        {
          case eMovementMode.FlyMode:
            mCamera.Move(-0.1 * MOVE_SCALE * (lNewMousePoint.X - mLastMouseDown.X) * mCamera.XAxis);
            mCamera.Move(0.1 * MOVE_SCALE * (lNewMousePoint.Y - mLastMouseDown.Y) * mCamera.ZAxis);
            break;
          case eMovementMode.InspectMode:
            Vector lStartPos = mCamera.Position.GetPositionVector();
            Vector lMoveVec = 0.1 * MOVE_SCALE * (lNewMousePoint.Y - mLastMouseDown.Y) * mCamera.ZAxis;
            //don't move through the origin
            if (lMoveVec.Dot(lStartPos) / lStartPos.LengthSquared > -1.0)
            {
              mCamera.Move(lMoveVec);
            }
            break;
          default: throw new Exception("Unreachable case");
        }
      }
      if (LightingMode == eLightingMode.Headlight)
      {
        //qq this replaces the matrix, rather than changes the values,
        // but that's OK for now
        mLight.Transform = mCamera.Transform;
      }
      mLastMouseDown = lNewMousePoint;
      InvalidateViewer();
    }

    void Viewer3DRenderingSurface_MouseUp(object sender, MouseEventArgs e)
    {
      mDraggingView = false;
      mMainForm.Viewer3DRenderingSurface.Capture = false;
    }

    bool mDraggingView = false;
    MouseButtons mDraggingButton;
    System.Drawing.Point mLastMouseDown;

    void Viewer3DRenderingSurface_MouseDown(object sender, MouseEventArgs e)
    {
      if (e.Button == MouseButtons.Middle)
      {
        Mesh lMesh = mRenderer.Pick(e.X, e.Y);
        if (lMesh != null)
        {
          if (lMesh is OwnedMesh && ((OwnedMesh)lMesh).Owner is Chunk)
          {
            OwnedMesh om = (OwnedMesh)lMesh;
            Chunk c = (Chunk)om.Owner;
            MessageBox.Show(string.Format("Clicked on {0} with name {1}", c.GetType().Name, c.Name));
          }
          else if (lMesh is OwnedMesh && ((OwnedMesh)lMesh).Owner is FlatChunk.ObjectEntry)
          {
            FlatChunk.ObjectEntry oe = (FlatChunk.ObjectEntry)((OwnedMesh)lMesh).Owner;
            MessageBox.Show(string.Format("Clicked on object type {0} at {1}, rotation {2}", oe.ObjtType, oe.OriginPosition, oe.RotationVector));
          }
          else
          {
            lMesh.RenderMode = lMesh.RenderMode == RenderMode.Filled ?
              RenderMode.Textured : RenderMode.Filled;
            InvalidateViewer();
          }
        }
      }
      else
      {
        mMainForm.Viewer3DRenderingSurface.Capture = true;
        mLastMouseDown = e.Location;
        mDraggingButton = e.Button;
        mDraggingView = true;
      }
    }

    void mMainForm_FormClosing(object sender, FormClosingEventArgs e)
    {
      mMainForm.Viewer3DRenderingSurface.Release();
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
      return new ThreeDeeViewer(xiMainForm);
    }

    private IEntityProvider mSubject = null;

    Scene mScene;
    Camera mCamera;
    ImmediateModeRenderer mRenderer;

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

      const double MOVE_SCALE = 100;

      Cursor prevCursor = mMainForm.Viewer3DRenderingSurface.Cursor;
      mMainForm.Viewer3DRenderingSurface.Cursor = Cursors.WaitCursor;
      RebuildScene();
      if (mSubject != null)
      {
        mCamera.Position = new GLTK.Point(-3 * MOVE_SCALE, -3 * MOVE_SCALE, 3 * MOVE_SCALE);
        mCamera.LookAt(new GLTK.Point(3 * MOVE_SCALE, 3 * MOVE_SCALE, 0), new GLTK.Vector(0, 0, 1));

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
          mLight.Transform = mCamera.Transform;
        }

        mMainForm.ChunkTreeView.CheckBoxes = (mSubject is Level);
      }
      else
      {
        mMainForm.ChunkTreeView.CheckBoxes = false;
      }
      mMainForm.Viewer3DRenderingSurface.Cursor = prevCursor;

      InvalidateViewer();
    }

    private void RebuildScene()
    {
      mScene.Clear();
      if (mSubject != null)
        mScene.AddRange(mSubject.GetEntities(mRenderer, mMainForm.Level, TextureMode, SelectedMetadata));
    }

    public static GLTK.Point Short3CoordToPoint(Short3Coord xiVal)
    {
      return new GLTK.Point(xiVal.X, xiVal.Y, xiVal.Z);
    }

    public override System.Windows.Forms.TabPage Tab
    {
      get { return mMainForm.ViewTab3D; }
    }

    #region MovementMode property

    public enum eMovementMode
    {
      FlyMode,
      InspectMode
    }

    private eMovementMode mMovementMode;
    public eMovementMode MovementMode
    {
      get { return mMovementMode; }
      set
      {
        mMovementMode = value;
        if (value == eMovementMode.InspectMode)
        {
          mCamera.LookAt(GLTK.Point.Origin, mCamera.YAxis);
        }
        if (OnMovementModeChange != null) OnMovementModeChange(this, null);
        InvalidateViewer();
      }
    }
    public event EventHandler OnMovementModeChange;

    #endregion

    #region DrawNormalsMode property

    public enum eDrawNormalsMode
    {
      DrawNormals,
      HideNormals
    }

    private eDrawNormalsMode mDrawNormalsMode;
    public eDrawNormalsMode DrawNormalsMode
    {
      get { return mDrawNormalsMode; }
      set
      {
        mDrawNormalsMode = value;
        if (value == eDrawNormalsMode.DrawNormals)
        {
          mRenderer.DebugNormalDrawLength = 10;
        }
        else
        {
          mRenderer.DebugNormalDrawLength = 0;
        }
        if (OnDrawNormalsModeChange != null) OnDrawNormalsModeChange(this, null);
        InvalidateViewer();
      }
    }
    public event EventHandler OnDrawNormalsModeChange;

    #endregion

    #region LightingMode property

    public enum eLightingMode
    {
      None,
      Headlight,
      OverheadLight
    }
    private eLightingMode mLightingMode;
    public eLightingMode LightingMode
    {
      get { return mLightingMode; }
      set
      {
        switch (value)
        {
          case eLightingMode.None:
            mRenderer.DisableLighting();
            break;
          case eLightingMode.Headlight:
            mRenderer.EnableLighting();
            mRenderer.ResetLights();
            mLight.Transform = mCamera.Transform;
            mScene.AddLight(mLight);
            break;
          case eLightingMode.OverheadLight:
            MessageBox.Show("OverheadLight mode not supported yet");
            return;
        }
        mLightingMode = value;
        if (OnLightingModeChange != null) OnLightingModeChange(this, null);
        InvalidateViewer();
      }
    }
    public event EventHandler OnLightingModeChange;

    #endregion

    #region TextureMode property

    public enum eTextureMode
    {
      NormalTextures,
      NormalTexturesWithMetadata,
      BumpmapTextures
    }
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
        InvalidateViewer();
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
        InvalidateViewer();
      }
    }

    public event EventHandler OnSelectedMetadataChange;

    #endregion

    private void InvalidateViewer()
    {
      mMainForm.Viewer3DRenderingSurface.Invalidate();
    }

    // a 2d move request, which will be turned into 3d camera
    // movement, in a manner dependent on
    private void MoveCamera(Vector xiMove)
    {
      if (MovementMode == eMovementMode.InspectMode)
      {
        //turn movement requests into rotation about origin

      }

      if (LightingMode == eLightingMode.Headlight)
      {
        //light moves with camera
        mLight.Move(xiMove);
      }
      mCamera.Move(xiMove);
    }

    private void KeyPressHandle(object sender, KeyPressEventArgs e)
    {
      if (mMainForm.ViewerTabControl.SelectedTab != Tab) return;

      switch (e.KeyChar)
      {
        case 'W':
        case 'w':
          mCamera.Move(-1.0 * mCamera.ZAxis * MOVE_SCALE);
          break;

        case 'S':
        case 's':
          mCamera.Move(1.0 * mCamera.ZAxis * MOVE_SCALE);
          break;

        case 'A':
        case 'a':
          mCamera.Move(-1.0 * mCamera.XAxis * MOVE_SCALE);
          break;

        case 'D':
        case 'd':
          mCamera.Move(1.0 * mCamera.XAxis * MOVE_SCALE);
          break;

        case 'Q':
        case 'q':
          mCamera.Move(-1.0 * mCamera.ZAxis * MOVE_SCALE);
          break;

        case 'E':
        case 'e':
          mCamera.Move(1.0 * mCamera.ZAxis * MOVE_SCALE);
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

      InvalidateViewer();
    }
  }
}
