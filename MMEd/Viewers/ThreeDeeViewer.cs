using System;
using System.Collections.Generic;
using System.Text;
using MMEd;
using MMEd.Chunks;
using System.Drawing;
using System.Windows.Forms;
using GLTK;


namespace MMEd.Viewers
{
  public class ThreeDeeViewer : Viewer
  {
    const double MOVE_SCALE = 100;

    //indicates that an object can provide an enumeration of GLTK.Entity objects
    //to be redered as a ThreeDee scene
    public interface IEntityProvider
    {
      IEnumerable<Entity> GetEntities(AbstractRenderer xiRenderer, Level xiLevel);
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
      mCamera = new Camera(80, 0.1, 1e5);
      mScene.Camera = mCamera;
    }

    void ChunkTreeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
    {
      RebuildScene();
      mMainForm.Viewer3DRenderingSurface.Invalidate();
    }

    void Viewer3DRenderingSurface_MouseMove(object sender, MouseEventArgs e)
    {
      if (!mDraggingView || mSubject == null) return;
      System.Drawing.Point lNewMousePoint = e.Location;
      if (mDraggingButton == MouseButtons.Left)
      {
        mCamera.Rotate2(-0.01 * (lNewMousePoint.X - mLastMouseDown.X), Vector.ZAxis);
        mCamera.Rotate2(0.01 * (lNewMousePoint.Y - mLastMouseDown.Y), mCamera.XAxis);
      }
      else if (mDraggingButton == MouseButtons.Right)
      {
        mCamera.Move(0.1 * MOVE_SCALE * (lNewMousePoint.X - mLastMouseDown.X) * mCamera.XAxis);
        mCamera.Move(0.1 * MOVE_SCALE * (lNewMousePoint.Y - mLastMouseDown.Y) * mCamera.ZAxis);
      }
      mLastMouseDown = lNewMousePoint;
      mMainForm.Viewer3DRenderingSurface.Invalidate();
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
          lMesh.RenderMode = lMesh.RenderMode == RenderMode.Filled ?
            RenderMode.Textured : RenderMode.Filled;
          mMainForm.Viewer3DRenderingSurface.Invalidate();
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
    AbstractRenderer mRenderer;

    public override void SetSubject(Chunk xiChunk)
    {
      if (!(xiChunk is IEntityProvider)) xiChunk = null;
      if (mSubject == xiChunk) return;
      mSubject = (IEntityProvider)xiChunk;

      Cursor prevCursor = mMainForm.Viewer3DRenderingSurface.Cursor;
      mMainForm.Viewer3DRenderingSurface.Cursor = Cursors.WaitCursor;
      RebuildScene();
      if (mSubject != null)
      {
        mCamera.Position = new GLTK.Point(-3 * MOVE_SCALE, -3 * MOVE_SCALE, -3 * MOVE_SCALE);
        mCamera.LookAt(new GLTK.Point(3 * MOVE_SCALE, 3 * MOVE_SCALE, 0), new GLTK.Vector(0, 0, -1));
        mMainForm.ChunkTreeView.CheckBoxes = (mSubject is Level);
      }
      else
      {
        mMainForm.ChunkTreeView.CheckBoxes = false;
      }
      mMainForm.Viewer3DRenderingSurface.Cursor = prevCursor;

      mMainForm.Viewer3DRenderingSurface.Invalidate();
    }

    private void RebuildScene()
    {
      mScene.Clear();
      if (mSubject != null)
        mScene.AddRange(mSubject.GetEntities(mRenderer, mMainForm.Level));
    }

    public static GLTK.Point Short3CoordToPoint(Short3Coord xiVal)
    {
      return new GLTK.Point(xiVal.X, xiVal.Y, xiVal.Z);
    }

    public override System.Windows.Forms.TabPage Tab
    {
      get { return mMainForm.ViewTab3D; }
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
          mCamera.Rotate2(-0.1, mCamera.XAxis);
          break;

        case 'K':
        case 'k':
          mCamera.Rotate2(0.1, mCamera.XAxis);
          break;

        case 'J':
        case 'j':
          mCamera.Rotate2(-0.1, mCamera.YAxis);
          break;

        case 'L':
        case 'l':
          mCamera.Rotate2(0.1, mCamera.YAxis);
          break;

        case 'U':
        case 'u':
          mCamera.Rotate2(-0.1, mCamera.ZAxis);
          break;

        case 'O':
        case 'o':
          mCamera.Rotate2(0.1, mCamera.ZAxis);
          break;
      }

      mMainForm.Viewer3DRenderingSurface.Invalidate();
    }
  }
}
