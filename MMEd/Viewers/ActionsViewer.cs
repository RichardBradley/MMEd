using System;
using System.Collections.Generic;
using System.Text;
using MMEd;
using MMEd.Chunks;
using System.Drawing;
using System.Windows.Forms;

// a viewer/editor which performs file wide actions

namespace MMEd.Viewers
{
  public class ActionsViewer : Viewer
  {
    #region actions

    public void ReindexBumpImages()
    {
      // count how many times each bump image is currently in use
      int[] lUseCount = new int[mMainForm.Level.SHET.BumpImages.mChildren.Length];
      const int lBumpIdx = (int)eTexMetaDataEntries.Bumpmap;
      foreach (FlatChunk flat in mMainForm.Level.SHET.Flats)
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

      Chunk[] lChunkArray = mMainForm.Level.SHET.BumpImages.mChildren;
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
      foreach (FlatChunk flat in mMainForm.Level.SHET.Flats)
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

    private ActionsViewer(MainForm xiMainForm)
      : base(xiMainForm) 
    {
      mMainForm.ActionsTabReindexBumpButton.Click += new EventHandler(ActionsTabReindexBumpButton_Click);
      SetSubject(null);
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
    }

    public override System.Windows.Forms.TabPage Tab
    {
      get { return mMainForm.ViewTabActions; }
    }
  }
}
