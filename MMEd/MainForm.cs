using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Xml.Serialization;
using MMEd.Chunks;
using MMEd.Viewers;
using MMEd.Util;
using MMEd.Forms;

namespace MMEd
{
  public partial class MainForm : Form
  {
    protected override void OnClosing(CancelEventArgs e)
    {
      try
      {
        mLocalSettings.Save();
      }
      catch
      {
        //if unable to save local settings, just drop them
      }
      base.OnClosing(e);
    }

    public MainForm()
    {
      InitializeComponent();
      // Load global settings
      this.mGlobalSettings = new GlobalSettings();
      this.mLocalSettings = LocalSettings.GetInstance();

      // Restore the size
      Size = mLocalSettings.m_size;
      if ((Size.Height < 100) || (Size.Width < 200))
      {
          Size lsize = new Size();
          lsize.Height = 500;
          lsize.Width = 800;

          this.Size = lsize;
      }

      //register the viewer manager classes
      //qq there must be a better way of doing this, it seems a bit
      //   odd. Could use reflection. Might be a little slow or have
      //   unexpected side-effects. It's nice to be able to control
      //   the order the tabs appear in, too.
      ViewTabActions.Tag = ActionsViewer.InitialiseViewer(this);
      ViewTabXML.Tag = XMLViewer.InitialiseViewer(this);
      ViewTabFlat.Tag = FlatViewer.InitialiseViewer(this);
      ViewTabImg.Tag = ImageViewer.InitialiseViewer(this);
      ViewTabBump.Tag = BumpViewer.InitialiseViewer(this);
      ViewTabSteering.Tag = SteeringViewer.InitialiseViewer(this);
      ViewTabCamera.Tag = CameraViewer.InitialiseViewer(this);
      ViewTabGrid.Tag = GridViewer.InitialiseViewer(this);
      ViewTab3D.Tag = ThreeDeeViewer.InitialiseViewer(this);
      ViewTabVRAM.Tag = VRAMViewer.InitialiseViewer(this);
      ViewTab3dEditor.Tag = ThreeDeeEditor.InitialiseViewer(this);
      ViewTabHistory.Tag = HistoryViewer.InitialiseViewer(this);

      //auto-load last level
      if (mLocalSettings.LastOpenedFile != null)
      {
        LoadInternal(mLocalSettings.LastOpenedType, mLocalSettings.LastOpenedFile);
      }
    }

    #region load routines

    public enum eOpenType
    {
      // Values index into OPEN_FILTER, except UnknownBinary which doesn't have an index
      Mmv = 1,
      LevelBinary = 2,
      Xml = 3,
      UnknownBinary = 99
    }
    private static string OPEN_FILTER = "MMEd Save File (*.mmv)|*.mmv|Binary Level File (*.dat)|*.dat|XML Level File (*.xml)|*.xml";

    private void openToolStripMenuItem_Click(object sender, EventArgs e)
    {
      //=======================================================================
      // Prompt for file
      //=======================================================================
      OpenFileDialog.FileName = mLocalSettings.LastOpenedFile;
      OpenFileDialog.Filter = OPEN_FILTER;
      OpenFileDialog.FilterIndex = (int)mLocalSettings.LastOpenedType;

      if (OpenFileDialog.ShowDialog(this) == DialogResult.OK)
      {
        //=====================================================================
        // Open the file
        //=====================================================================
        string lFileToOpen = OpenFileDialog.FileName;
        eOpenType lOpenType = (eOpenType)OpenFileDialog.FilterIndex;
        LoadInternal(lOpenType, lFileToOpen);
      }
    }

    private void OpenUnknownBinaryClick(object sender, EventArgs e)
    {
      //=======================================================================
      // Prompt for file
      //=======================================================================
      OpenFileDialog.FileName = mLocalSettings.LastOpenedFile;
      OpenFileDialog.Filter = "All files (*.*)|*.*";

      if (OpenFileDialog.ShowDialog(this) == DialogResult.OK)
      {
        //=====================================================================
        // Open the file
        //=====================================================================
        string lFileToOpen = OpenFileDialog.FileName;
        LoadInternal(eOpenType.UnknownBinary, lFileToOpen);
      }
    }

    private void LoadInternal(eOpenType xiOpenType, string xiFilename)
    {
      Chunk lNewRootChunk = null;
      string lExceptionWhen = "opening file";
      try
      {
        using (FileStream fs = File.OpenRead(xiFilename))
        {
          lExceptionWhen = "deserialising the file";
          switch (xiOpenType)
          {
            case eOpenType.LevelBinary:
              lNewRootChunk = new Level(fs);
              break;
            case eOpenType.UnknownBinary:
              lNewRootChunk = new FileChunk(fs);
              break;
            case eOpenType.Mmv:
              lNewRootChunk = new VersionList(fs);
              break;
            case eOpenType.Xml:
              XmlSerializer xs = new XmlSerializer(typeof(Chunk));
              lNewRootChunk = (Chunk)xs.Deserialize(fs);
              break;
            default: throw new Exception("unreachable case");
          }

          if (fs.Length != fs.Position)
          {
            //check the whole file has been read
            throw new DeserialisationException(string.Format("Deserialisation terminated early at byte {0} of {1}", fs.Position, fs.Length));
          }
        }
      }
      catch (Exception err)
      {
        Trace.WriteLine(err);
        MessageBox.Show(string.Format("Exception occurred while {0}: {1}", lExceptionWhen, err.Message), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }
      // level loaded OK, now fill tree:
      RootChunk = lNewRootChunk;
      mLocalSettings.LastOpenedFile = xiFilename;
      mLocalSettings.LastOpenedType = xiOpenType;
      mCurrentFile = xiFilename;
      mCurrentFileMode = xiOpenType == eOpenType.Mmv ? eSaveMode.Mmv : xiOpenType == eOpenType.Xml ? eSaveMode.Xml : eSaveMode.Binary;
    }

    #endregion

    #region save routines

    public enum eSaveMode
    {
      // Values index into SAVE_FILTER
      Mmv = 1,
      Binary = 2,
      Xml = 3
    }
    private static string SAVE_FILTER = "MMEd Save File (*.mmv)|*.mmv|Binary Level File (*.dat)|*.dat|XML Level File (*.xml)|*.xml";

    private void SaveInternal(eSaveMode xiSaveMode, string xiFilename)
    {
      if (RootChunk == null)
      {
        MessageBox.Show("Can't save: no file is open");
        return;
      }

      string lExceptionWhen = "saving file";
      try
      {
        long lPreviousSize = -1;

        if (xiSaveMode == eSaveMode.Binary && File.Exists(xiFilename))
        {
          lPreviousSize = new FileInfo(xiFilename).Length;
        }

        using (FileStream fs = File.Create(xiFilename))
        {
          lExceptionWhen = "serialising the file";
          if (xiSaveMode == eSaveMode.Binary)
          {
            if (RootChunk is VersionList)
            {
              CurrentLevel.Serialise(fs);
            }
            else
            {
              RootChunk.Serialise(fs);
            }
          }
          else if (xiSaveMode == eSaveMode.Xml)
          {
            XmlSerializer xs = new XmlSerializer(typeof(Chunk));

            if (RootChunk is VersionList)
            {
              xs.Serialize(fs, CurrentLevel);
            }
            else
            {
              xs.Serialize(fs, RootChunk);
            }
          }
          else if (xiSaveMode == eSaveMode.Mmv)
          {
            if (RootChunk is VersionList)
            {
              VersionList lVersionList = (VersionList)RootChunk;
              lVersionList.AddLevel(CurrentLevel);

              RecursiveAddChunkNode(ChunkTreeView.Nodes[0].Nodes, 1, lVersionList.GetLastVersion());
            }
            else if (RootChunk is Level)
            {
              VersionList lVersionList = new VersionList(
                (Level)RootChunk,
                Path.GetFileNameWithoutExtension(xiFilename),
                null);
              RootChunk = lVersionList;
            }
            RootChunk.Serialise(fs);
          }
        }

        if (lPreviousSize != -1 && lPreviousSize != new FileInfo(xiFilename).Length)
        {
          MessageBox.Show("WARNING: The size of your level has changed. Please check it's not too large, and check MMEd for bugs that have allowed the size to change.",
            "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
      }
      catch (Exception err)
      {
        Trace.WriteLine(err);
        MessageBox.Show(string.Format("Exception occurred while {0}: {1}", lExceptionWhen, err.Message), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }

      mLocalSettings.LastSavedFile = xiFilename;
      mLocalSettings.LastSavedMode = xiSaveMode;
      mCurrentFile = xiFilename;
      mCurrentFileMode = xiSaveMode;
    }

    private void saveToolStripMenuItem1_Click(object sender, EventArgs e)
    {
      if (RootChunk == null)
      {
        MessageBox.Show("Can't save: no file is open");
        return;
      }

      if (mCurrentFile == null)
      {
        saveAsToolStripMenuItem_Click(sender, e);
        return;
      }

      SaveInternal(mCurrentFileMode, mCurrentFile);
    }

    private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
    {
      //=======================================================================
      // Prompt for filename
      //=======================================================================
      SaveFileDialog.FileName = mLocalSettings.LastSavedFile;
      SaveFileDialog.Filter = SAVE_FILTER;
      SaveFileDialog.FilterIndex = (int)mLocalSettings.LastSavedMode;
      SaveFileDialog.AddExtension = true;
      SaveFileDialog.CheckPathExists = true;

      if (SaveFileDialog.ShowDialog(this) == DialogResult.OK)
      {
        string lFileToSave = SaveFileDialog.FileName;
        eSaveMode lSaveMode = (eSaveMode)SaveFileDialog.FilterIndex;

        SaveInternal(lSaveMode, lFileToSave);
      }
    }

    #endregion

    public Chunk RootChunk
    {
      set
      {
        mRootChunk = value;
        // Suppress repainting the TreeView until all the objects have been created.
        ChunkTreeView.SelectedNode = null;
        ChunkTreeView.BeginUpdate();
        ChunkTreeView.Nodes.Clear();
        RecursiveAddChunkNode(ChunkTreeView.Nodes, value);
        ChunkTreeView.Nodes[0].Expand();
        ChunkTreeView.EndUpdate();
        ChunkTreeView_AfterSelect(null, null);
        Text = string.Format("MMEd [{0}]", mRootChunk.Name);
      }
      get
      {
        return mRootChunk;
      }
    }

    ///========================================================================
    /// Property : CurrentLevel
    /// 
    /// <summary>
    /// 	The level currently being edited
    /// </summary>
    ///========================================================================
    public Level CurrentLevel
    {
      get
      {
        if (mRootChunk is Level)
        {
          return (Level)mRootChunk;
        }
        else if (mRootChunk is VersionList)
        {
          return ((VersionList)mRootChunk).CurrentLevel;
        }
        else
        {
          return null;
        }
      }
    }

    private Chunk mRootChunk;
    private string mCurrentFile;
    private eSaveMode mCurrentFileMode;

    private void RecursiveAddChunkNode(TreeNodeCollection xiNodes, Chunk xiChunk)
    {
      TreeNode lNode = xiNodes.Add(xiChunk.Name);
      //record the mapping on the two objects, for easy reference later
      //no doubt, this will play merry hell with the GC if you load lots
      //of different levels...
      lNode.Tag = xiChunk;
      xiChunk.TreeNode = lNode;

      //recurse
      foreach (Chunk child in xiChunk.GetChildren())
      {
        RecursiveAddChunkNode(lNode.Nodes, child);
      }
    }

    private void RecursiveAddChunkNode(TreeNodeCollection xiNodes, int xiIndex, Chunk xiChunk)
    {
      TreeNode lNode = xiNodes.Insert(xiIndex, xiChunk.Name);
      //record the mapping on the two objects, for easy reference later
      //no doubt, this will play merry hell with the GC if you load lots
      //of different levels...
      lNode.Tag = xiChunk;
      xiChunk.TreeNode = lNode;

      //recurse
      foreach (Chunk child in xiChunk.GetChildren())
      {
        RecursiveAddChunkNode(lNode.Nodes, child);
      }
    }

    public LocalSettings LocalSettings
    {
      get { return mLocalSettings; }
    }


    GlobalSettings mGlobalSettings;
    LocalSettings mLocalSettings;

    //sender can be null!
    private void ChunkTreeView_AfterSelect(object sender, TreeViewEventArgs e)
    {
      TreeNode lActiveNode = e == null
       ? ChunkTreeView.SelectedNode
       : e.Node;
      Chunk lActiveChunk = lActiveNode == null
          ? null
          : (Chunk)lActiveNode.Tag;
      TabPage lActiveViewerPage = ViewerTabControl.SelectedTab;

      // do we need to switch to a different page?
      if (lActiveViewerPage != null && lActiveViewerPage.Tag != null)
      {
        Viewer lViewer = (Viewer)lActiveViewerPage.Tag;
        if (!lViewer.CanViewChunk(lActiveChunk))
        {
          ViewerTabControl.SelectedTab = ViewTabActions;
        }
      }

      // set the enabled state of all the tabs
      foreach (Viewer lViewer in Viewer.GetViewers())
      {
        if (lViewer.CanViewChunk(lActiveChunk))
        {
          if (!ViewerTabControl.TabPages.Contains(lViewer.Tab))
          {
            ViewerTabControl.TabPages.Add(lViewer.Tab);
          }
        }
        else
        {
          if (ViewerTabControl.TabPages.Contains(lViewer.Tab))
          {
            ViewerTabControl.TabPages.Remove(lViewer.Tab);
          }
        }
      }

      // update subjects
      ViewerTabControl_SelectedIndexChanged(null, null);
    }

    private void MainForm_Load(object sender, EventArgs e)
    {

    }

    private void ViewerTabControl_Selected(object sender, TabControlEventArgs e)
    {
    }

    private void ViewerTabControl_SelectedIndexChanged(object sender, EventArgs e)
    {
      TabPage lActiveViewerPage = ViewerTabControl.SelectedTab;
      TreeNode lActiveNode = ChunkTreeView.SelectedNode;
      Chunk lActiveChunk = lActiveNode == null ? null : (Chunk)lActiveNode.Tag;
      foreach (Viewer lViewer in Viewer.GetViewers())
      {
        if (lViewer.Tab == lActiveViewerPage)
        {
          lViewer.SetSubject(lActiveChunk);
        }
        else
        {
          lViewer.SetSubject(null);
        }
      }
    }

    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
      if (DialogKey != null)
      {
        DialogKey(this, new KeyEventArgs(keyData));
      }
      return base.ProcessCmdKey(ref msg, keyData);
    }

    public event KeyEventHandler DialogKey;

    private void OnResizeEnd(object sender, EventArgs e)
    {
      mLocalSettings.m_size = Size;
    }

    ///========================================================================
    /// Method : publishToolStripMenuItem_Click
    /// 
    /// <summary>
    /// 	The Publish operation
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    ///========================================================================
    private void publishToolStripMenuItem_Click(object sender, EventArgs e)
    {
      PublishForm lForm = new PublishForm(this);

      if (RootChunk is VersionList)
      {
        //=====================================================================
        // Use the properties saved on the VersionList as defaults
        //=====================================================================
        VersionList lVersionList = (VersionList)RootChunk;

        if (lVersionList.CDFilename != null && lVersionList.CDFilename != "")
        {
          lForm.CourseDropDown.SelectedItem = MMCD.Courses.Find(new Predicate<MMCD.Course>(
            delegate (MMCD.Course xiCourse) { return xiCourse.FileName == lVersionList.CDFilename; }));
        }

        if (lVersionList.BinaryFilename != null && lVersionList.BinaryFilename != "")
        {
          lForm.BinaryFileTextBox.Text = lVersionList.BinaryFilename;
        }

        if (lVersionList.CourseName != null && lVersionList.CourseName != "")
        {
          lForm.NameTextBox.Text = lVersionList.CourseName;
        }
      }

      if (lForm.ShowDialog() == DialogResult.OK)
      {
        FileInfo lBinaryFile = new FileInfo(lForm.BinaryFileTextBox.Text);

        //=====================================================================
        // If we want to keep backups, make one now
        //=====================================================================
        if (lForm.BackupsCheckBox.Checked && lBinaryFile.Exists)
        {
          string lBackupExtension = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss.bak");
          string lBackupPath = Path.Combine(Path.GetDirectoryName(lForm.BinaryFileTextBox.Text), "Backups");

          if (!Directory.Exists(lBackupPath))
          {
            Directory.CreateDirectory(lBackupPath);
          }

          string lBackupFile = Path.Combine(lBackupPath,
            Path.GetFileNameWithoutExtension(lForm.BinaryFileTextBox.Text) + "." + lBackupExtension);
          lBinaryFile.CopyTo(lBackupFile);

          //===================================================================
          // Remove any outdated backups
          //===================================================================
          FileInfo[] lAllBackups = Array.ConvertAll<string, FileInfo>(Directory.GetFiles(lBackupPath,
            Path.GetFileNameWithoutExtension(lForm.BinaryFileTextBox.Text) + ".*.bak"),
            new Converter<string, FileInfo>(delegate(string xiFilename)
            {
              return new FileInfo(xiFilename);
            }));
          Array.Sort<FileInfo>(lAllBackups, new Comparison<FileInfo>(delegate(FileInfo xiBackup1, FileInfo xiBackup2)
            {
              return xiBackup2.CreationTime.CompareTo(xiBackup1.CreationTime);
            }));

          for (int ii = (int)lForm.BackupCountUpDown.Value; ii < lAllBackups.Length; ii++)
          {
            lAllBackups[ii].Delete();
          }
        }

        //=====================================================================
        // Save the binary file
        //=====================================================================
        using (FileStream lFileStream = lBinaryFile.Create())
        {
          CurrentLevel.Serialise(lFileStream);
        }

        if (lForm.UpdateCDImageCheckBox.Checked)
        {
          //===================================================================
          // Update the CD image
          //===================================================================
          FileInfo lCDFile = new FileInfo(lForm.CDImageTextBox.Text);
          CDImage lImage = new CDImage(lCDFile);
          MMCD.Course lCourse = (MMCD.Course)lForm.CourseDropDown.SelectedItem;
          byte[] lBinaryData = new byte[lCourse.CDLength];

          lBinaryFile.Refresh();
          if (lBinaryFile.Length != lCourse.CDLength)
          {
            MessageBox.Show("File is the wrong length! It will be padded with zeros or truncated to fit on the CD.", "Publish Course", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
          }

          using (FileStream lFileStream = lBinaryFile.OpenRead())
          {
            lFileStream.Read(lBinaryData, 0, (int)Math.Min(lBinaryData.Length, lBinaryFile.Length));
          }

          lImage.Replace(lBinaryData, lCourse.CDOffset);
          lImage.WriteFile(lCDFile);

          byte[] lCourseNameForCD = Encoding.ASCII.GetBytes(lCourse.GetCDCourseName(lForm.NameTextBox.Text));

          foreach (long lNameOffset in lCourse.NameOffsets)
          {
            lImage.Replace(lCourseNameForCD, lNameOffset);
            lImage.WriteFile(lCDFile);
          }
        }

        if (RootChunk is VersionList)
        {
          //===================================================================
          // Update the properties saved on the VersionList
          //===================================================================
          VersionList lVersionList = (VersionList)RootChunk;
          lVersionList.BinaryFilename = lForm.BinaryFileTextBox.Text;

          if (lForm.UpdateCDImageCheckBox.Checked)
          {
            lVersionList.CDFilename = ((MMCD.Course)lForm.CourseDropDown.SelectedItem).FileName;
            lVersionList.CourseName = lForm.NameTextBox.Text;
          }

          //===================================================================
          // Save the VersionList
          //===================================================================
          if (mCurrentFile != null)
          {
            SaveInternal(mCurrentFileMode, mCurrentFile);
          }
        }
      }
    }

    private void exitToolStripMenuItem_Click(object sender, EventArgs e)
    {
      this.Close();
    }

    private void newToolStripMenuItem_Click(object sender, EventArgs e)
    {
      NewForm lForm = new NewForm(this);

      if (lForm.ShowDialog() == DialogResult.OK)
      {
        //=====================================================================
        // Load the file from the CD image
        //=====================================================================
        CDImage lCDImage = new CDImage(new FileInfo(lForm.CDImageTextBox.Text));
        MMCD.Course lCourse = (MMCD.Course)lForm.CourseDropDown.SelectedItem;
        byte[] lLevelBinary = lCDImage.Extract(lCourse.CDOffset, lCourse.CDLength);
        MemoryStream lLevelStream = new MemoryStream(lLevelBinary);

        Level lNewLevel = new Level(lLevelStream);

        //=====================================================================
        // Check that the whole file has been read
        //=====================================================================
        if (lLevelStream.Length != lLevelStream.Position)
        {
          throw new DeserialisationException(string.Format("Deserialisation terminated early at byte {0} of {1}", lLevelStream.Position, lLevelStream.Length));
        }

        //=====================================================================
        // Create a new VersionList based on this level, and set it up
        //=====================================================================
        VersionList lVersionList = new VersionList(lNewLevel, lCourse.CourseName, lCourse.FileName);
        RootChunk = lVersionList;
        mCurrentFile = null;
      }
    }


  }
}