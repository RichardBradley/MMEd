using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using MMEd.Chunks;

namespace MMEd.Viewers
{
  class HistoryViewer : Viewer
  {
    private HistoryViewer(MainForm xiMainForm)
      : base(xiMainForm)
    {
      xiMainForm.RevertButton.Click += new EventHandler(RevertButton_Click);
    }

    void RevertButton_Click(object sender, EventArgs e)
    {
      if (MessageBox.Show("Are you sure? This will lose any unsaved changes in your current version of the level.", "Revert Level", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
      {
        VersionList lVersionList = (VersionList)mMainForm.RootChunk;
        lVersionList.CurrentLevel = lVersionList.GetLevel(mVersion);
        mMainForm.RootChunk = mMainForm.RootChunk;
      }
    }

    public override bool CanViewChunk(Chunk xiChunk)
    {
      return xiChunk is MMEd.Chunks.Version;
    }

    private MMEd.Chunks.Version mVersion = null;

    public override void SetSubject(Chunk xiChunk)
    {
      if (xiChunk == null)
      {
        return;
      }

      mVersion = (MMEd.Chunks.Version)xiChunk;
      mMainForm.VersionCreatedLabel.Text = mVersion.CreationDate.ToString("yyyy/MM/dd HH:mm");
      mMainForm.VersionChangesTextBox.Text = mVersion.Changes;
    }

    public override System.Windows.Forms.TabPage Tab
    {
      get { return mMainForm.ViewTabHistory; }
    }

    // Create an instance of the viewer manager class
    public static Viewer InitialiseViewer(MainForm xiMainForm)
    {
      return new HistoryViewer(xiMainForm);
    }
  }
}
