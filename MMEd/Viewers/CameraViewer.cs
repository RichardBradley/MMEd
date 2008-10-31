using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;
using GLTK;
using MMEd;
using MMEd.Chunks;
using Point = System.Drawing.Point;

namespace MMEd.Viewers
{
  class CameraViewer : Viewer
  {
    private CameraViewer(MainForm xiMainForm)
      : base(xiMainForm)
    {
      xiMainForm.SliderDirection.ValueChanged += new EventHandler(SliderDirection_ValueChanged);
      xiMainForm.SliderDistance.ValueChanged += new EventHandler(SliderDistance_ValueChanged);
      xiMainForm.SliderElevation.ValueChanged += new EventHandler(SliderElevation_ValueChanged);
      xiMainForm.TextDirection.TextChanged += new EventHandler(TextDirection_TextChanged);
      xiMainForm.TextDistance.TextChanged += new EventHandler(TextDistance_TextChanged);
      xiMainForm.TextElevation.TextChanged += new EventHandler(TextElevation_TextChanged);
    }

    // Only CameraPosChunks can be viewed.
    public override bool CanViewChunk(Chunk xiChunk)
    {
      return xiChunk is CameraPosChunk;
    }

    // Create an instance of the viewer manager class
    public static Viewer InitialiseViewer(MainForm xiMainForm)
    {
      return new CameraViewer(xiMainForm);
    }

    private CameraPosChunk mSubject = null;

    public override void SetSubject(Chunk xiChunk)
    {
      if (mSubject == xiChunk || xiChunk == null) return;

      if (!(xiChunk is CameraPosChunk))
      {
        throw new InvalidOperationException("Tried to view chunk of type {0} in CameraViewer");
      }

      mMainForm.CameraRenderingSurface.Visible = true;
      mSubject = (CameraPosChunk)xiChunk;
      SetDirection(mSubject.Direction);
      mMainForm.TextDistance.Text = mSubject.Distance.ToString();
      mMainForm.TextElevation.Text = mSubject.Elevation.ToString();

      InitialiseThreeDeeView();
    }

    private void SetDirection(int xiValue)
    {
      if (mUpdating) return;

      mUpdating = true;

      int lSliderValue =
        (xiValue / (2048 / mMainForm.SliderDirection.Maximum)) %
        (mMainForm.SliderDirection.Maximum * 2);

      if (lSliderValue > mMainForm.SliderDirection.Maximum)
      {
        lSliderValue += (mMainForm.SliderDirection.Minimum * 2);
      }

      if (lSliderValue < mMainForm.SliderDirection.Minimum)
      {
        lSliderValue += (mMainForm.SliderDirection.Maximum * 2);
      }

      mMainForm.SliderDirection.Value = lSliderValue;
      mMainForm.TextDirection.Text = xiValue.ToString();
      mSubject.Direction = (short)xiValue;
      UpdateCameraImage();
      UpdateCameraThreeDee();
      mUpdating = false;
    }

    private void SetDistance(int xiValue)
    {
      if (mUpdating) return;

      mUpdating = true;
      int lSliderValue = (xiValue / (mArbitraryMaximum / mMainForm.SliderDistance.Maximum));
      lSliderValue = Math.Min(mMainForm.SliderDistance.Maximum, lSliderValue);
      lSliderValue = Math.Max(mMainForm.SliderDistance.Minimum, lSliderValue);
      mMainForm.SliderDistance.Value = lSliderValue;
      mMainForm.TextDistance.Text = xiValue.ToString();
      mSubject.Distance = (short)xiValue;
      UpdateCameraImage();
      UpdateCameraThreeDee();
      mUpdating = false;
    }

    private void SetElevation(int xiValue)
    {
      if (mUpdating) return;

      mUpdating = true;
      int lSliderValue = (xiValue / (mArbitraryMaximum / mMainForm.SliderElevation.Maximum));
      lSliderValue = Math.Min(mMainForm.SliderElevation.Maximum, lSliderValue);
      lSliderValue = Math.Max(mMainForm.SliderElevation.Minimum, lSliderValue);
      mMainForm.SliderElevation.Value = lSliderValue;
      mMainForm.TextElevation.Text = xiValue.ToString();
      mSubject.Elevation = (short)xiValue;
      UpdateCameraImage();
      UpdateCameraThreeDee();
      mUpdating = false;
    }

    private void UpdateCameraImage()
    {
      Panel lPanel = mMainForm.PanelCameraImage;
      Bitmap lImage = new Bitmap(lPanel.Width, lPanel.Height);
      Graphics g = Graphics.FromImage(lImage);
      g.FillRectangle(
        new SolidBrush(Color.White),
        0,
        0,
        lPanel.Width,
        lPanel.Height);
      mSubject.Draw(
        g,
        new Pen(Color.Black),
        new Point(lPanel.Width / 2, lPanel.Height / 2),
        lPanel.Width);
      lPanel.BackgroundImage = lImage;
    }

    private void InitialiseThreeDeeView()
    {
      if (mSubject == null)
      {
        return;
      }

      int SCALE = 256;
      int GRIDSIZE = 9; // Should be an odd number so the centre is the middle of a tile.
      mRenderer = new ImmediateModeRenderer();
      mRenderer.Attach(mMainForm.CameraRenderingSurface);

      mScene = new Scene();
      mCamera = new Camera(80, 0.1, 1e10);
      mView = new GLTK.View(mScene, mCamera, mRenderer);

      mScene.Clear();
      if (mMainForm.CurrentLevel != null)
      {
        // Create a surface and fill it with a 10 x 10 grid of squares
        MMEdEntity lSurface = new MMEdEntity(mSubject);

        for (int x = 0; x < GRIDSIZE; x++)
        {
          for (int y = 0; y < GRIDSIZE; y++)
          {
            Mesh lSquare = new OwnedMesh(mSubject, PolygonMode.Quads);
            lSquare.AddFace(
              new Vertex(new GLTK.Point(x, y, 0), 0, 0),
              new Vertex(new GLTK.Point(x + 1, y, 0), 1, 0),
              new Vertex(new GLTK.Point(x + 1, y + 1, 0), 1, 1),
              new Vertex(new GLTK.Point(x, y + 1, 0), 0, 1));
            lSquare.RenderMode = RenderMode.Wireframe;
            lSurface.Meshes.Add(lSquare);
          }
        }

        // Add it to the scene at the origin
        lSurface.Scale(SCALE, SCALE, 1.0);
        short lOffset = (short)(-SCALE * GRIDSIZE / 2);
        GLTK.Point lNewPos = ThreeDeeViewer.Short3CoordToPoint(new Short3Coord(lOffset, lOffset, 0));
        lSurface.Position = new GLTK.Point(lNewPos.x, lNewPos.y, -lNewPos.z);
        mScene.AddRange(new MMEdEntity[] { lSurface });

        // Use a random object from the level for now.
        Level lLevel = mMainForm.CurrentLevel;
        TMDChunk lObject = lLevel.GetObjtById(1);
        mScene.AddRange(lObject.GetEntities(
          mMainForm.CurrentLevel,
          MMEd.Viewers.ThreeDee.eTextureMode.NormalTextures,
          eTexMetaDataEntries.Odds));

        string lExceptionWhen = "opening file";
        try
        {
          ThreeDeeViewer.SceneHolder sh;
          string lFilename = string.Format("{0}{1}..{1}camera-editor-scene.xml",
            Path.GetDirectoryName(
              new Uri(System.Reflection.Assembly.GetExecutingAssembly().CodeBase).LocalPath),
            Path.DirectorySeparatorChar);

          using (FileStream fs = File.OpenRead(lFilename))
          {
            lExceptionWhen = "deserialising the scene";
            XmlSerializer xs = new XmlSerializer(typeof(ThreeDeeViewer.SceneHolder));
            sh = (ThreeDeeViewer.SceneHolder)xs.Deserialize(fs);
          }
          lExceptionWhen = "fixing texture ids";
          Dictionary<int, int> lSavedTexIdsToLiveTexIds = new Dictionary<int, int>();
          foreach (ThreeDeeViewer.TextureHolder th in sh.Textures)
          {
            if (th.Bitmap != null)
            {
              lSavedTexIdsToLiveTexIds[th.ID] = AbstractRenderer.ImageToTextureId(th.Bitmap);
            }
            else
            {
              lSavedTexIdsToLiveTexIds[th.ID] = 0;
            }
          }
          foreach (Entity ent in sh.Entities)
          {
            foreach (Mesh m in ent.Meshes)
            {
              if (m.RenderMode == RenderMode.Textured)
              {
                m.Texture = lSavedTexIdsToLiveTexIds[m.Texture];
              }
            }
          }
          mScene.Objects.Clear();
          mScene.AddRange(sh.Entities);
        }
        catch (Exception err)
        {
          System.Diagnostics.Trace.WriteLine(err);
          MessageBox.Show(string.Format("Exception occurred while {0}: {1}", lExceptionWhen, err.Message), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        UpdateCameraThreeDee();
      }
      else
      {
        System.Windows.Forms.MessageBox.Show("3D view is only available when editing a level");
        mMainForm.CameraRenderingSurface.Visible = false;
      }
    }

    private void UpdateCameraThreeDee()
    {
      if (mCamera == null)
      {
        return;
      }

      // TODO: The elevation isn't rending quite right yet...
      short x = (short)(Math.Sin(mSubject.Direction * Math.PI / 2048) * mSubject.Distance);
      short y = (short)(Math.Cos(mSubject.Direction * Math.PI / 2048) * mSubject.Distance);
      mCamera.Position = ThreeDeeViewer.Short3CoordToPoint(new Short3Coord(x, y, mSubject.Elevation));
      mCamera.LookAt(new GLTK.Point(0, 0, 0), new GLTK.Vector(0, 0, 1));
      mMainForm.CameraRenderingSurface.Invalidate();
    }

    public override System.Windows.Forms.TabPage Tab
    {
      get { return mMainForm.ViewTabCamera; }
    }

    private int ParseValue(string xiValue)
    {
      if (xiValue == null || xiValue == "")
      {
        return int.MinValue;
      }

      try
      {
        return int.Parse(xiValue);
      }
      catch
      {
        return int.MinValue;
      }
    }

    void TextDirection_TextChanged(object sender, EventArgs e)
    {
      int lNewValue = ParseValue(mMainForm.TextDirection.Text);

      if (lNewValue == int.MinValue)
      {
        return;
      }

      SetDirection(lNewValue);
    }

    void TextDistance_TextChanged(object sender, EventArgs e)
    {
      int lNewValue = ParseValue(mMainForm.TextDistance.Text);

      if (lNewValue == int.MinValue)
      {
        return;
      }

      SetDistance(lNewValue);
    }

    void TextElevation_TextChanged(object sender, EventArgs e)
    {
      int lNewValue = ParseValue(mMainForm.TextElevation.Text);

      if (lNewValue == int.MinValue)
      {
        return;
      }

      SetElevation(lNewValue);
    }

    void SliderDirection_ValueChanged(object sender, EventArgs e)
    {
      SetDirection(mMainForm.SliderDirection.Value * 2048 / mMainForm.SliderDirection.Maximum);
    }

    void SliderDistance_ValueChanged(object sender, EventArgs e)
    {
      SetDistance(mMainForm.SliderDistance.Value * (mArbitraryMaximum / mMainForm.SliderDistance.Maximum));
    }

    void SliderElevation_ValueChanged(object sender, EventArgs e)
    {
      SetElevation(mMainForm.SliderElevation.Value * (mArbitraryMaximum / mMainForm.SliderElevation.Maximum));
    }

    private bool mUpdating = false;
    private static int mArbitraryMaximum = 1000;

    Scene mScene;
    Camera mCamera;
    Light mLight;
    ImmediateModeRenderer mRenderer;
    GLTK.View mView;
  }
}
