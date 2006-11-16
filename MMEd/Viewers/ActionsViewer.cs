using System;
using System.Collections.Generic;
using System.Text;
using MMEd;
using MMEd.Util;
using MMEd.Chunks;
using System.Drawing;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.IO;

// a viewer/editor which performs file wide actions

namespace MMEd.Viewers
{
  public class ActionsViewer : Viewer
  {
    #region Reindex Bump Action

    public void ReindexBumpImages()
    {
      Level lLevel = mMainForm.RootChunk as Level;
      if (lLevel == null)
      {
        MessageBox.Show("Must have a level open for this action");
        return;
      }

      // count how many times each bump image is currently in use
      int[] lUseCount = new int[lLevel.SHET.BumpImages.mChildren.Length];
      const int lBumpIdx = (int)eTexMetaDataEntries.Bumpmap;
      foreach (FlatChunk flat in lLevel.SHET.Flats)
      {
        if (flat.TexMetaData != null)
        {
          foreach (byte[][] row in flat.TexMetaData)
          {
            foreach (byte[] entry in row)
            {
              lUseCount[entry[lBumpIdx]]++;
            }
          }
        }
      }

      Chunk[] lChunkArray = lLevel.SHET.BumpImages.mChildren;
      BumpImageChunk[] lBumpArray = new BumpImageChunk[lChunkArray.Length];
      Array.Copy(lChunkArray, lBumpArray, lChunkArray.Length);

      //get a mapping from each equivalence class of BumpImageChunks (under a
      //deep '==' operator on their data) to the lowest index in that class
      SortedDictionary<BumpImageChunk, int> lBumpsToCanonicalId = new SortedDictionary<BumpImageChunk, int>(new BumpImageComparer());
      for (int i=lBumpArray.Length-1; i>=0; i--)
      {
        lBumpsToCanonicalId[lBumpArray[i]] = i;
      }

      int[] lOldToNewIndexMap = new int[lUseCount.Length];

      //determine where each bump should map to:
      int lNextUnusedId = 0;
      for (int lOldIdx=0; lOldIdx<lOldToNewIndexMap.Length; lOldIdx++)
      {
        if (lUseCount[lOldIdx] > 0)
        {
          //we need to map this to somewhere.
          //can it be coalesced with other, identical bumps?
          int lCanonicalId = (int)lBumpsToCanonicalId[lBumpArray[lOldIdx]];
          if (lCanonicalId < lOldIdx)
          {
            lOldToNewIndexMap[lOldIdx] = lOldToNewIndexMap[lCanonicalId];
          }
          else if (lCanonicalId == lOldIdx)
          {
            lOldToNewIndexMap[lOldIdx] = lNextUnusedId;
            lNextUnusedId++;
          }
          else
          {
            throw new Exception("Internal error: lCanonicalId can't be greater than lOldIdx");
          }
        }
        else
        {
          lOldToNewIndexMap[lOldIdx] = -1;
        }
      }

      //take a copy of each canonical bump, so we don't lose any information
      Dictionary<int,byte[]> lCanonicalImages = new Dictionary<int,byte[]>();
      foreach (KeyValuePair<BumpImageChunk,int> lEntry in lBumpsToCanonicalId)
      {
        lCanonicalImages.Add(lEntry.Value, (byte[])lEntry.Key.Data.Clone());
      }

      //fill in all the bump images with their new values:
      int lHighestUsed = -1;
      for (int lOldIdx=0; lOldIdx<lOldToNewIndexMap.Length; lOldIdx++)
      {
        int lNew = lOldToNewIndexMap[lOldIdx];
        if (lNew != -1)
        {
          if (lCanonicalImages.ContainsKey(lOldIdx))
          {
            lHighestUsed = lNew;
            lBumpArray[lNew].Data = lCanonicalImages[lOldIdx];
          }
        }
      }
      for (int i=lHighestUsed+1; i<lBumpArray.Length; i++)
      {
        lBumpArray[i].Clear();
      }

      //now update all the bumps in the Flats
      foreach (FlatChunk flat in lLevel.SHET.Flats)
      {
        if (flat.TexMetaData != null)
        {
          foreach (byte[][] row in flat.TexMetaData)
          {
            foreach (byte[] entry in row)
            {
              int lOldIdx = entry[lBumpIdx];
              int lNewIdx = lOldToNewIndexMap[lOldIdx];
              if (lNewIdx < 0) throw new Exception("Internal error: this bump should be unused");
              entry[lBumpIdx] = (byte)lNewIdx;
            }
          }
        }
      }

      MessageBox.Show(string.Format(
        "Sucessfully re-indexed. There are {0} bump tiles in use, and {0} free",
        lCanonicalImages.Count,
        lBumpArray.Length - lCanonicalImages.Count));
    }

    private class BumpImageComparer : Comparer<BumpImageChunk>
    {
      public override int Compare(BumpImageChunk a, BumpImageChunk b)
      {
        return Util.ByteArrayComparer.CompareStatic(a.Data, b.Data);
      }
    }

    #endregion 

    #region Clone Flat Action

    public void CloneFlat()
    {
      Level lLevel = mMainForm.RootChunk as Level;
      if (lLevel == null)
      {
        MessageBox.Show("Must have a level open for this action");
        return;
      }

      FlatChunk lSource = mSubject as FlatChunk;
      if (lSource == null) return;

      //=======================================================================
      // Serialise the current Flat to XML, then deserialise - simple method
      // of creating a deep clone of the Flat.
      //=======================================================================
      XmlSerializer lSerializer = new XmlSerializer(lSource.GetType());
      StringWriter lStringWriter = new StringWriter();
      lSerializer.Serialize(lStringWriter, lSource);
      StringReader lStringReader = new StringReader(lStringWriter.ToString());
      FlatChunk lDest = (FlatChunk)lSerializer.Deserialize(lStringReader);

      //=======================================================================
      // Add the new Flat to the SHET
      //=======================================================================
      lDest.DeclaredName = "NewFlat1";
      short lMaxIdx = 0;
      foreach (FlatChunk lExistingFlat in lLevel.SHET.Flats)
      {
        if (lExistingFlat.DeclaredIdx > lMaxIdx)
        {
          lMaxIdx = lExistingFlat.DeclaredIdx;
        }
      }
      lDest.DeclaredIdx = (short)(1 + lMaxIdx);
      int lSizeIncrease = lLevel.SHET.AddFlat(lDest);
      lLevel.SHET.TrailingZeroByteCount -= lSizeIncrease;

      //=======================================================================
      // Refresh the tree view
      //=======================================================================
      mMainForm.RootChunk = mMainForm.RootChunk;

      MessageBox.Show("Flat cloned successfully. " +
        (lLevel.SHET.TrailingZeroByteCount < 0 ? 
        string.Format("Note that you have run out of space in your level file - you will need to free up {0} bytes before you can save your changes.", -lLevel.SHET.TrailingZeroByteCount) : 
        ""));
    }

    #endregion

    #region Export TIM Action

    void ActionsTabExportTIMButton_Click(object sender, EventArgs e)
    {
      if (mSubject as TIMChunk != null)
      {
        SaveFileDialog sfd = new SaveFileDialog();
        sfd.FileName = mMainForm.LocalSettings.LastTIMFile;
        DialogResult res = sfd.ShowDialog(mMainForm);
        if (res == DialogResult.OK)
        {
          string lExceptionWhen = "opening the file";
          try
          {
            using (FileStream fs = File.Create(sfd.FileName))
            {
              lExceptionWhen = "writing the file";
              StreamUtils.Pipe(((TIMChunk)mSubject).CreateBMPStream(), fs);
            }
          }
          catch (Exception err)
          {
            MessageBox.Show(string.Format("Exception occurred while {0}: {1}", lExceptionWhen, err.Message), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
          }
        }
        mMainForm.LocalSettings.LastTIMFile = sfd.FileName;
      }
      else
      {
        MessageBox.Show("Error: mSubject is null!");
      }
    }

    #endregion

    #region Import TIM Action 

    void ActionsTabImportTIMButton_Click(object sender, EventArgs e)
    {
      if (mSubject as TIMChunk != null)
      {
        OpenFileDialog ofd = new OpenFileDialog();
        ofd.FileName = mMainForm.LocalSettings.LastTIMFile;
        DialogResult res = ofd.ShowDialog(mMainForm);
        if (res == DialogResult.OK)
        {
          string lExceptionWhen = "opening the file";
          try
          {
            using (FileStream fs = File.OpenRead(ofd.FileName))
            {
              lExceptionWhen = "loading a bitmap from the file";
              Bitmap lBmp = new Bitmap(fs);
              lExceptionWhen = "turning the bitmap into a TIM";
              ((TIMChunk)mSubject).FillDataFromBitmap(lBmp);
            }
          }
          catch (Exception err)
          {
            MessageBox.Show(string.Format("Exception occurred while {0}: {1}", lExceptionWhen, err.Message), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
          }
        }
        mMainForm.LocalSettings.LastTIMFile = ofd.FileName;
      }
      else
      {
        MessageBox.Show("Error: mSubject is null!");
      }
    }

    #endregion

    private ActionsViewer(MainForm xiMainForm)
      : base(xiMainForm) 
    {
      mMainForm.ActionsTabReindexBumpButton.Click += new EventHandler(ActionsTabReindexBumpButton_Click);
      mMainForm.ActionsTabCloneFlatButton.Click += new EventHandler(ActionsTabCloneFlatButton_Click);
      mMainForm.ActionsTabExportTIMButton.Click += new EventHandler(ActionsTabExportTIMButton_Click);
      mMainForm.ActionsTabImportTIMButton.Click += new EventHandler(ActionsTabImportTIMButton_Click);
      SetSubject(null);
    }

    void ActionsTabCloneFlatButton_Click(object sender, EventArgs e)
    {
      CloneFlat();
    }

    void ActionsTabReindexBumpButton_Click(object sender, EventArgs e)
    {
      ReindexBumpImages();
    }

    public override bool CanViewChunk(Chunk xiChunk)
    {
      //always leave this tab available, as it's fast to render
      return true;
    }

    // Create an instance of the viewer manager class
    public static Viewer InitialiseViewer(MainForm xiMainForm)
    {
      return new ActionsViewer(xiMainForm);
    }

    private Chunk mSubject = null;

    public override void SetSubject(Chunk xiChunk)
    {
      if (mSubject == xiChunk) return;
      mSubject = xiChunk;
      mMainForm.ActionsTabReindexBumpButton.Enabled = (mSubject is Level);
      mMainForm.ActionsTabCloneFlatButton.Enabled = (mSubject is FlatChunk);
      mMainForm.ActionsTabImportTIMButton.Enabled = (mSubject is TIMChunk);
      mMainForm.ActionsTabExportTIMButton.Enabled = (mSubject is TIMChunk);
    }

    public override System.Windows.Forms.TabPage Tab
    {
      get { return mMainForm.ViewTabActions; }
    }
  }
}
