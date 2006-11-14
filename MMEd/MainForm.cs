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
using MMEd.Chunks;
using MMEd.Viewers;

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

            //register the viewer manager classes
            //qq there must be a better way of doing this, it seems a bit
            //   odd
            ViewTabActions.Tag = ActionsViewer.InitialiseViewer(this);
            ViewTabXML.Tag = XMLViewer.InitialiseViewer(this);
            ViewTabFlat.Tag = FlatViewer.InitialiseViewer(this);
            ViewTabImg.Tag = ImageViewer.InitialiseViewer(this);
            ViewTabBump.Tag = BumpViewer.InitialiseViewer(this);
            ViewTabGrid.Tag = GridViewer.InitialiseViewer(this);
            ViewTab3D.Tag = ThreeDeeViewer.InitialiseViewer(this);
            ViewTabVRAM.Tag = VRAMViewer.InitialiseViewer(this);
            ViewTab3dEditor.Tag = ThreeDeeEditor.InitialiseViewer(this);

            //auto-load last level
            if (mLocalSettings.LastOpenedFile != null)
            {
                LoadLevelFromFile(mLocalSettings.LastOpenedFile);
            }
        }

        private void MnuiFileOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.FileName = mLocalSettings.LastOpenedFile;
            DialogResult res = ofd.ShowDialog(this);
            if (res == DialogResult.OK)
            {
                LoadLevelFromFile(ofd.FileName);
            }
        }

        private void LoadLevelFromFile(string xiFilename)
        {
            Level lNewLevel = new Level();
            string lExceptionWhen = "opening file";
            try
            {
                using (FileStream fs = File.OpenRead(xiFilename))
                {
                    lExceptionWhen = "deserialising the level";
                    lNewLevel.Deserialise(fs);
                    if (fs.Length != fs.Position)
                    {
                        //This won't ever actually happen, because SHET expects
                        //trailing zeros to EOF
                        throw new DeserialisationException(string.Format("Deserialisatoin terminated early at byte {0} of {1}", fs.Position, fs.Length));
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
            Level = lNewLevel;
            mLocalSettings.LastOpenedFile = xiFilename;
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.FileName = mLocalSettings.LastSavedFile;
            DialogResult res = sfd.ShowDialog(this);
            if (res == DialogResult.OK)
            {
                string lExceptionWhen = "opening file";
                try
                {
                    using (FileStream fs = File.Create(sfd.FileName))
                    {
                        lExceptionWhen = "serialising the level";
                        Level.Serialise(fs);
                    }
                }
                catch (Exception err)
                {
                    Trace.WriteLine(err);
                    MessageBox.Show(string.Format("Exception occurred while {0}: {1}", lExceptionWhen, err.Message), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            mLocalSettings.LastSavedFile = sfd.FileName;
        }

        public Level Level
        {
            set
            {
                mLevel = value;
                // Suppress repainting the TreeView until all the objects have been created.
                ChunkTreeView.SelectedNode = null;
                ChunkTreeView.BeginUpdate();
                ChunkTreeView.Nodes.Clear();
                RecursiveAddChunkNode(ChunkTreeView.Nodes, value);
                ChunkTreeView.Nodes[0].Expand();
                ChunkTreeView.EndUpdate();
                ChunkTreeView_AfterSelect(null, null);
                Text = string.Format("MMEd [{0}]", mLevel.Name);
            }
            get
            {
                return mLevel;
            }
        }

        private Level mLevel;

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
            if (lActiveViewerPage.Tag != null)
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

      protected override bool  ProcessCmdKey(ref Message msg, Keys keyData)
      {
        if (DialogKey != null)
        {
          DialogKey(this, new KeyEventArgs(keyData));
        }
        return base.ProcessCmdKey(ref msg, keyData);
      }

      public event KeyEventHandler DialogKey;

    }
}