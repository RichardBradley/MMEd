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

    ///========================================================================
    /// Method : ReindexImages
    /// 
    /// <summary>
    /// 	Reindex images of the given type - remove duplicates and renumber
    ///   to the top of the image array.
    /// </summary>
    /// <param name="xiImageType"></param>
    ///========================================================================
    public void ReindexImages(eTexMetaDataEntries xiImageType)
    {
      Level lLevel = mMainForm.CurrentLevel;
      if (lLevel == null)
      {
        MessageBox.Show("Must have a level open for this action");
        return;
      }

      Chunk[] lChunkArray;
      if (xiImageType == eTexMetaDataEntries.Bumpmap)
      {
        lChunkArray = lLevel.SHET.BumpImages.mChildren;
      }
      else if (xiImageType == eTexMetaDataEntries.Odds)
      {
        lChunkArray = lLevel.SHET.OddImages.mChildren;
      }
      else if (xiImageType == eTexMetaDataEntries.CameraPos)
      {
        lChunkArray = lLevel.SHET.CameraPositions.mChildren;
      }
      else
      {
        throw new Exception("Internal error: Cannot reindex images of type " + xiImageType.ToString());
      }

      // count how many times each image is currently in use
      int[] lUseCount = new int[lChunkArray.Length];
      int lTexMetaIndex = (int)xiImageType;
      foreach (FlatChunk flat in lLevel.SHET.Flats)
      {
        if (flat.TexMetaData != null)
        {
          foreach (byte[][] row in flat.TexMetaData)
          {
            foreach (byte[] entry in row)
            {
              lUseCount[entry[lTexMetaIndex]]++;
            }
          }
        }
      }

      //get a mapping from each equivalence class of chunks to the lowest index in that class
      SortedDictionary<IReindexableChunk, int> lImagesToCanonicalId = new SortedDictionary<IReindexableChunk, int>(new ReindexableChunkComparer());
      for (int i=lChunkArray.Length-1; i>=0; i--)
      {
        if (lUseCount[i] == 0)
        {
          // Ignore any unused images - otherwise we might try to remap used bumps
          // to unused ones, which would cause errors when we remove all the unused
          // bumps later on.
          continue;
        }

        lImagesToCanonicalId[(IReindexableChunk)lChunkArray[i]] = i;
      }

      int[] lOldToNewIndexMap = new int[lUseCount.Length];

      //determine where each image should map to:
      int lNextUnusedId = 0;
      for (int lOldIdx=0; lOldIdx<lOldToNewIndexMap.Length; lOldIdx++)
      {
        if (lUseCount[lOldIdx] > 0)
        {
          //we need to map this to somewhere.
          //can it be coalesced with other, identical bumps?
          int lCanonicalId = (int)lImagesToCanonicalId[(IReindexableChunk)lChunkArray[lOldIdx]];
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

      //take a copy of each canonical image, so we don't lose any information
      Dictionary<int,byte[]> lCanonicalImages = new Dictionary<int,byte[]>();
      foreach (KeyValuePair<IReindexableChunk,int> lEntry in lImagesToCanonicalId)
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
            ((IReindexableChunk)lChunkArray[lNew]).Data = lCanonicalImages[lOldIdx];
          }
        }
      }
      for (int i=lHighestUsed+1; i<lChunkArray.Length; i++)
      {
        ((IReindexableChunk)lChunkArray[i]).Clear();
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
              int lOldIdx = entry[lTexMetaIndex];
              int lNewIdx = lOldToNewIndexMap[lOldIdx];
              if (lNewIdx < 0) throw new Exception("Internal error: this image should be unused");
              entry[lTexMetaIndex] = (byte)lNewIdx;
            }
          }
        }
      }

      MessageBox.Show(string.Format(
        "Successfully reindexed. There are {0} {2} tiles in use, and {1} free",
        lCanonicalImages.Count,
        lChunkArray.Length - lCanonicalImages.Count,
        xiImageType.ToString().ToLower()));
    }

    ///========================================================================
    /// Method : CompactImages
    /// 
    /// <summary>
    /// 	Compact images fo the given type - remove any unused images from the
    ///   end of the array.
    /// </summary>
    /// <param name="xiImageType"></param>
    /// <remarks>
    ///   Note that no attempt is made to deduplicate - it is assumed that
    ///   duplicates will be removed via ReindexImages if required.
    /// </remarks>
    ///========================================================================
    public void CompactImages(eTexMetaDataEntries xiImageType)
    {
      Level lLevel = mMainForm.CurrentLevel;
      if (lLevel == null)
      {
        MessageBox.Show("Must have a level open for this action");
        return;
      }

      //=======================================================================
      // Work out which array of images to work with
      //=======================================================================
      Chunk[] lChunkArray;
      if (xiImageType == eTexMetaDataEntries.Bumpmap)
      {
        lChunkArray = lLevel.SHET.BumpImages.mChildren;
      }
      else if (xiImageType == eTexMetaDataEntries.Odds)
      {
        lChunkArray = lLevel.SHET.OddImages.mChildren;
      }
      else if (xiImageType == eTexMetaDataEntries.CameraPos)
      {
        lChunkArray = lLevel.SHET.CameraPositions.mChildren;
      }
      else
      {
        throw new Exception("Internal error: Cannot reindex images of type " + xiImageType.ToString());
      }

      //=======================================================================
      // Find the highest numbered image that's in use - we'll remove all 
      // higher-numbered images
      //=======================================================================
      int lMaxUsedIndex = -1;
      int lTexMetaIndex = (int)xiImageType;

      foreach (FlatChunk lFlat in lLevel.SHET.Flats)
      {
        if (lFlat.TexMetaData != null)
        {
          foreach (byte[][] lRow in lFlat.TexMetaData)
          {
            foreach (byte[] lEntry in lRow)
            {
              if (lMaxUsedIndex < lEntry[lTexMetaIndex])
              {
                lMaxUsedIndex = lEntry[lTexMetaIndex];
              }
            }
          }
        }
      }

      if (lMaxUsedIndex == -1)
      {
        MessageBox.Show(string.Format("No compaction required - there are no {0} tiles!",
          xiImageType.ToString().ToLower()));
        return;
      }

      //=======================================================================
      // Create a new image array and adjust zero padding
      //=======================================================================
      Chunk[] lNewChunkArray = new Chunk[lMaxUsedIndex + 1];
      Array.Copy(lChunkArray, lNewChunkArray, lMaxUsedIndex + 1);
      
      lLevel.SHET.TrailingZeroByteCount += (lChunkArray.Length - (lMaxUsedIndex + 1)) * ((IReindexableChunk)lChunkArray[0]).Data.Length;
      
      if (xiImageType == eTexMetaDataEntries.Bumpmap)
      {
        lLevel.SHET.BumpImages.mChildren = lNewChunkArray;
      }
      else if (xiImageType == eTexMetaDataEntries.Odds)
      {
        lLevel.SHET.OddImages.mChildren = lNewChunkArray;
      }
      else if (xiImageType == eTexMetaDataEntries.CameraPos)
      {
        lLevel.SHET.CameraPositions.mChildren = lNewChunkArray;
      }

      MessageBox.Show(string.Format("Successfully compacted. There are {0} {1} tiles remaining",
        lMaxUsedIndex + 1,
        xiImageType.ToString().ToLower()));
    }

    private class ReindexableChunkComparer : Comparer<IReindexableChunk>
    {
      public override int Compare(IReindexableChunk a, IReindexableChunk b)
      {
        return Util.ByteArrayComparer.CompareStatic(a.Data, b.Data);
      }
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

    #region Export flats to images

    private string GetImageNameForFlat(string xiDirname, FlatChunk xiFlat)
    {
      string lName = string.Format("Flat-{0}-{1}.png", xiFlat.DeclaredIdx, xiFlat.DeclaredName);
      lName.Replace('\\', '_');
      lName.Replace('/', '_');
      lName.Replace(':', '_');
      lName.Replace('*', '_');
      lName.Replace('?', '_');
      lName.Replace('"', '_');
      lName.Replace('<', '_');
      lName.Replace('>', '_');
      lName.Replace('|', '_');
      return string.Format("{0}{1}{2}",
        xiDirname,
        Path.DirectorySeparatorChar,
        lName);
    }

    void ActionsTabExportFlatImagesButton_Click(object sender, EventArgs e)
    {
      if (mSubject as Level != null)
      {
        Level lLevel = (Level)mSubject;
        FolderBrowserDialog fbd = new FolderBrowserDialog();
        fbd.SelectedPath = mMainForm.LocalSettings.LastFlatImagesDir;
        if (fbd.ShowDialog(mMainForm) == DialogResult.OK)
        {
          string lExceptionWhen = "opening the directory";
          try
          {
            foreach (FlatChunk lFlat in lLevel.SHET.Flats)
            {
              lExceptionWhen = string.Format("creating image for flat {0}", lFlat.DeclaredName);
              TIMChunk lTopLeftTile = lLevel.GetTileById(lFlat.TextureIds[0][0]);
              if (lTopLeftTile == null) throw new Exception("top left tile was null!");
              int lTileWidth = lTopLeftTile.ImageWidth;
              int lTileHeight = lTopLeftTile.ImageHeight;
              if (lTileHeight != 64 || lTileWidth != 64)
              {
                MessageBox.Show(string.Format("Warning: Skipping tile {0} because only 64x64 tiles are supported for this action.", lFlat.DeclaredName));
              }
              Bitmap lBmp = new Bitmap(lFlat.Width * lTileWidth, lFlat.Height * lTileHeight);
              Graphics g = Graphics.FromImage(lBmp);
              g.Clear(Color.Black);
              for (int x = 0; x < lFlat.Width; x++)
              {
                for (int y = 0; y < lFlat.Height; y++)
                {
                  g.DrawImageUnscaled(
                    lLevel.GetTileById(lFlat.TextureIds[x][y]).ToBitmap(),
                    x * lTileWidth,
                    y * lTileHeight);
                }
              }

              lExceptionWhen = string.Format("saving image for flat {0}", lFlat.DeclaredName);
              using (FileStream fs = File.Create(GetImageNameForFlat(fbd.SelectedPath, lFlat)))
              {
                lBmp.Save(fs, System.Drawing.Imaging.ImageFormat.Png);
              }
            }
          }
          catch (Exception err)
          {
            MessageBox.Show(string.Format("Exception occurred while {0}: {1}", lExceptionWhen, err.Message), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
          }
        }
        mMainForm.LocalSettings.LastFlatImagesDir = fbd.SelectedPath;
      }
      else
      {
        MessageBox.Show("Error: mSubject is null!");
      }
    }

    #endregion

    #region Import flat textures from images 
    
    //this is going to be complex. Should be in its own class?

    void ActionsTabImportFlatImagesButton_Click(object sender, EventArgs e)
    {
      if (mSubject as Level != null)
      {
        Level lLevel = (Level)mSubject;
         MessageBox.Show("Sorry, not yet implemented");
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

    #region Import TMD Action

    void ActionsTabImportTMDButton_Click(object sender, EventArgs e)
    {
      TMDChunk lTmd = mSubject as TMDChunk;
      if (lTmd == null)
      {
        MessageBox.Show("Error: mSubject is null!");
        return;
      }

      OpenFileDialog ofd = new OpenFileDialog();
      ofd.FileName = mMainForm.LocalSettings.LastTMDFile;
      DialogResult res = ofd.ShowDialog(mMainForm);
      if (res == DialogResult.OK)
      {
        string lExceptionWhen = "opening the file";
        try
        {
          using (FileStream fs = File.OpenRead(ofd.FileName))
          {
            lTmd.DeserialiseFrom3dsStream(fs);
          }
        }
        catch (Exception err)
        {
          MessageBox.Show(string.Format("Exception occurred while {0}: {1}", lExceptionWhen, err.Message), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
          return;
        }
      }
      mMainForm.LocalSettings.LastTMDFile = ofd.FileName;
    }

    #endregion

    #region Export TMD Action

    void ActionsTabExportTMDButton_Click(object sender, EventArgs e)
    {
      TMDChunk lTmd = mSubject as TMDChunk;
      if (lTmd == null)
      {
        MessageBox.Show("Error: mSubject is null!");
        return;
      }

      SaveFileDialog sfd = new SaveFileDialog();
      sfd.FileName = mMainForm.LocalSettings.LastTMDFile;
      DialogResult res = sfd.ShowDialog(mMainForm);
      if (res == DialogResult.OK)
      {
        string lExceptionWhen = "opening the file";
        try
        {
          using (FileStream fs = File.Create(sfd.FileName))
          {
            lExceptionWhen = "writing the file";
            lTmd.SerialiseTo3dsStream(fs);
          }
        }
        catch (Exception err)
        {
          MessageBox.Show(string.Format("Exception occurred while {0}: {1}", lExceptionWhen, err.Message), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
          return;
        }
      }
      mMainForm.LocalSettings.LastTMDFile = sfd.FileName;
    }

    #endregion

    private ActionsViewer(MainForm xiMainForm)
      : base(xiMainForm) 
    {
      mMainForm.ActionsTabOptimiseButton.Click += new EventHandler(ActionsTabOptimiseButton_Click);
      mMainForm.ActionsTabExportFlatImagesButton.Click += new EventHandler(ActionsTabExportFlatImagesButton_Click);
      mMainForm.ActionsTabImportFlatImagesButton.Click += new EventHandler(ActionsTabImportFlatImagesButton_Click);
      mMainForm.ActionsTabExportTIMButton.Click += new EventHandler(ActionsTabExportTIMButton_Click);
      mMainForm.ActionsTabImportTIMButton.Click += new EventHandler(ActionsTabImportTIMButton_Click);
      mMainForm.ActionsTabExportTMDButton.Click += new EventHandler(ActionsTabExportTMDButton_Click);
      mMainForm.ActionsTabImportTMDButton.Click += new EventHandler(ActionsTabImportTMDButton_Click);
      SetSubject(null);
    }

    void ActionsTabOptimiseButton_Click(object sender, EventArgs e)
    {
      if (mMainForm.OptimiseBumpReindexCheckbox.Checked)
      {
        ReindexImages(eTexMetaDataEntries.Bumpmap);
      }

      if (mMainForm.OptimiseBumpCompactCheckbox.Checked)
      {
        CompactImages(eTexMetaDataEntries.Bumpmap);
      }

      if (mMainForm.OptimiseOddsReindexCheckbox.Checked)
      {
        ReindexImages(eTexMetaDataEntries.Odds);
      }

      if (mMainForm.OptimiseOddsCompactCheckbox.Checked)
      {
        CompactImages(eTexMetaDataEntries.Odds);
      }

      if (mMainForm.OptimiseCameraReindexCheckbox.Checked)
      {
        ReindexImages(eTexMetaDataEntries.CameraPos);
      }

      if (mMainForm.OptimiseCameraCompactCheckbox.Checked)
      {
        CompactImages(eTexMetaDataEntries.CameraPos);
      }

      //=======================================================================
      // Refresh the tree view
      //=======================================================================
      mMainForm.RootChunk = mMainForm.RootChunk;
    }

    public override bool CanViewChunk(Chunk xiChunk)
    {
      //always leave this tab available, as it's fast to render - unless this is 
      //a Version chunk, in which case there's really nothing useful it can do
      return !(xiChunk is MMEd.Chunks.Version);
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
      mMainForm.ActionsTabOptimiseButton.Enabled = (mSubject is Level);
      mMainForm.ActionsTabExportFlatImagesButton.Enabled = (mSubject is Level);
      mMainForm.ActionsTabImportFlatImagesButton.Enabled = (mSubject is Level);
      mMainForm.ActionsTabImportTIMButton.Enabled = (mSubject is TIMChunk);
      mMainForm.ActionsTabExportTIMButton.Enabled = (mSubject is TIMChunk);
      mMainForm.ActionsTabImportTMDButton.Enabled = (mSubject is TMDChunk);
      mMainForm.ActionsTabExportTMDButton.Enabled = (mSubject is TMDChunk);
    }

    public override System.Windows.Forms.TabPage Tab
    {
      get { return mMainForm.ViewTabActions; }
    }
  }
}
