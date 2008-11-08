using System;
using System.Collections.Generic;
using System.Text;
using MMEd;
using MMEd.Chunks;
using System.Xml.Serialization;
using System.IO;
using System.Windows.Forms;

namespace MMEd.Viewers
{
  ///==========================================================================
  ///  Class : FlatViewer
  /// 
  /// <summary>
  /// 	Viewer for raw Flat information. Use Grid and 3D viewers to actually 
  ///   change the tiles on the Flat, but this viewer will let you edit the
  ///   flat's position and size, and add/remove objects and weapons
  /// </summary>
  ///==========================================================================
  class FlatViewer : Viewer
  {
    private FlatViewer(MainForm xiMainForm)
      : base(xiMainForm)
    {
      mMainForm.FlatViewerCommitBtn.Click += new System.EventHandler(this.CommitButton_Click);
      mMainForm.FlatViewerCloneButton.Click += new EventHandler(CloneButton_Click);
      mMainForm.FlatViewerDeleteButton.Click += new EventHandler(DeleteButton_Click);
      Panel.AddWeaponLink.Click += new EventHandler(AddWeaponLink_Click);
      Panel.AddObjectLink.Click += new EventHandler(AddObjectLink_Click);
    }

    void DeleteButton_Click(object sender, EventArgs e)
    {
      if (mSubject == null)
      {
        MessageBox.Show("Can't do that -- mSubject is null!");
        return;
      }

      if (MessageBox.Show("Are you sure you want to delete this flat?", "Delete Flat", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
      {
        return;
      }

      int lSizeDecrease = mMainForm.CurrentLevel.SHET.DeleteFlat(mSubject);
      mMainForm.CurrentLevel.SHET.TrailingZeroByteCount += lSizeDecrease;

      //=======================================================================
      // Refresh the tree view
      //=======================================================================
      mMainForm.RootChunk = mMainForm.RootChunk;

      MessageBox.Show("Flat deleted successfully.");
    }

    void CloneButton_Click(object sender, EventArgs e)
    {
      if (mSubject == null)
      {
        MessageBox.Show("Can't do that -- mSubject is null!");
        return;
      }

      //=======================================================================
      // Serialise the current Flat to XML, then deserialise - simple method
      // of creating a deep clone of the Flat.
      //=======================================================================
      XmlSerializer lSerializer = new XmlSerializer(mSubject.GetType());
      StringWriter lStringWriter = new StringWriter();
      lSerializer.Serialize(lStringWriter, mSubject);
      StringReader lStringReader = new StringReader(lStringWriter.ToString());
      FlatChunk lDest = (FlatChunk)lSerializer.Deserialize(lStringReader);

      //=======================================================================
      // Add the new Flat to the SHET
      //=======================================================================
      lDest.DeclaredName = "NewFlat1";
      Level lLevel = mMainForm.CurrentLevel;
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
    
    public void CommitButton_Click(object xiSender, EventArgs xiArgs)
    {
      if (mSubject == null)
      {
        MessageBox.Show("Can't do that -- mSubject is null!");
        return;
      }

      Level lLevel = mMainForm.CurrentLevel;

      //=======================================================================
      // Warn about changing flags
      //=======================================================================
      if (mSubject.HasMetaData != Panel.HasMetaDataCheckBox.Checked && !Panel.HasMetaDataCheckBox.Checked)
      {
        if (MessageBox.Show("You are about to remove all metadata from this flat, eg bump settings. Are you sure?", 
          "Removing Meta Data", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
        {
          return;
        }
      }

      if (mSubject.FlgB != Panel.FlagBCheckBox.Checked ||
        mSubject.FlgC != Panel.FlagCCheckBox.Checked ||
        mSubject.FlgE != Panel.FlagECheckBox.Checked)
      {
        if (MessageBox.Show("Changing flags may (or may not) be rather a dangerous thing to do. " +
          (mSubject.HasMetaData != Panel.HasMetaDataCheckBox.Checked && !Panel.HasMetaDataCheckBox.Checked ? "In particular, unsetting Flag A will delete all you tex meta data e.g. bump settings. " : "") +
          "Are you sure you want to do this?", "Changing Flat Flags",
          MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
        {
          return;
        }
      }

      //=======================================================================
      // Offer to be helpful if changing the origin (should really do this
      // for rotation too...)
      //=======================================================================
      short lNewX = short.Parse(Panel.OriginXTextBox.Text);
      short lNewY = short.Parse(Panel.OriginYTextBox.Text);
      short lNewZ = short.Parse(Panel.OriginZTextBox.Text);
      short lDeltaX = 0;
      short lDeltaY = 0;
      short lDeltaZ = 0;

      if (mSubject.OriginPosition.X != lNewX ||
        mSubject.OriginPosition.Y != lNewY ||
        mSubject.OriginPosition.Z != lNewZ)
      {
        if (MessageBox.Show("You are moving the flat. Do you want to move all the objects and weapons by the same amount?",
          "MMEd", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
        {
          lDeltaX = (short)(lNewX - mSubject.OriginPosition.X);
          lDeltaY = (short)(lNewY - mSubject.OriginPosition.Y);
          lDeltaZ = (short)(lNewZ - mSubject.OriginPosition.Z);
        }
      }

      //=======================================================================
      // Save simple values
      //=======================================================================
      mSubject.DeclaredName = Panel.NameTextBox.Text;
      mSubject.OriginPosition.X = lNewX;
      mSubject.OriginPosition.Y = lNewY;
      mSubject.OriginPosition.Z = lNewZ;
      mSubject.RotationVector.X = short.Parse(Panel.RotationXTextBox.Text);
      mSubject.RotationVector.Y = short.Parse(Panel.RotationYTextBox.Text);
      mSubject.RotationVector.Z = short.Parse(Panel.RotationZTextBox.Text);
      mSubject.ScaleX = short.Parse(Panel.ScaleXTextBox.Text);
      mSubject.ScaleY = short.Parse(Panel.ScaleYTextBox.Text);
      mSubject.FlgB = Panel.FlagBCheckBox.Checked;
      mSubject.FlgC = Panel.FlagCCheckBox.Checked;
      mSubject.Visible = Panel.VisibleCheckBox.Checked;
      mSubject.FlgE = Panel.FlagECheckBox.Checked;

      //=======================================================================
      // Change width, height and HasMetaData if appropriate
      //=======================================================================
      short lNewWidth = short.Parse(Panel.WidthTextBox.Text);
      short lNewHeight = short.Parse(Panel.HeightTextBox.Text);
      FlatChunk.eResizeOptions lResizeOptions = FlatChunk.eResizeOptions.Default;
      int lSizeIncrease;

      if (mSubject.Width != lNewWidth)
      {
        if (MessageBox.Show("You are changing the width of the flat. Do you want to fix the left-hand side of the flat? (Alternative is right-hand side)",
          "MMEd", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
        {
          lResizeOptions |= FlatChunk.eResizeOptions.KeepRight;
        }
      }

      if (mSubject.Height != lNewHeight)
      {
        if (MessageBox.Show("You are changing the height of the flat. Do you want to fix the top side of the flat? (Alternative is bottom side)",
          "MMEd", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
        {
          lResizeOptions |= FlatChunk.eResizeOptions.KeepBottom;
        }
      }

      if (mSubject.Width != lNewWidth || mSubject.Height != lNewHeight || mSubject.HasMetaData != Panel.HasMetaDataCheckBox.Checked)
      {
        lSizeIncrease = mSubject.Resize(Panel.HasMetaDataCheckBox.Checked, lNewWidth, lNewHeight, lResizeOptions);
        lLevel.SHET.TrailingZeroByteCount -= lSizeIncrease;
      }

      //=======================================================================
      // Save weapons
      //=======================================================================
      List<FlatChunk.WeaponEntry> lWeapons = new List<FlatChunk.WeaponEntry>();
      for (int ii = 6; ii < Panel.WeaponsTable.Controls.Count; ii += 6)
      {
        FlatChunk.WeaponEntry lWeapon = new FlatChunk.WeaponEntry();
        lWeapon.WeaponType = (eWeaponType)Enum.Parse(typeof(eWeaponType), ((ComboBox)Panel.WeaponsTable.Controls[ii]).Text);
        lWeapon.ShortUnknown = short.Parse(((TextBox)Panel.WeaponsTable.Controls[ii + 1]).Text);
        lWeapon.OriginPosition = new Short3Coord();
        lWeapon.OriginPosition.X = (short)(short.Parse(((TextBox)Panel.WeaponsTable.Controls[ii + 2]).Text) + lDeltaX);
        lWeapon.OriginPosition.Y = (short)(short.Parse(((TextBox)Panel.WeaponsTable.Controls[ii + 3]).Text) + lDeltaY);
        lWeapon.OriginPosition.Z = (short)(short.Parse(((TextBox)Panel.WeaponsTable.Controls[ii + 4]).Text) + lDeltaZ);
        lWeapons.Add(lWeapon);
      }

      lSizeIncrease = mSubject.ReplaceWeapons(lWeapons);
      lLevel.SHET.TrailingZeroByteCount -= lSizeIncrease;

      //=======================================================================
      // Save objects
      //=======================================================================
      List<FlatChunk.ObjectEntry> lObjects = new List<FlatChunk.ObjectEntry>();
      for (int ii = 11; ii < Panel.ObjectsTable.Controls.Count; ii += 11)
      {
        FlatChunk.ObjectEntry lObject = new FlatChunk.ObjectEntry();
        lObject.ObjtType = short.Parse(((TextBox)Panel.ObjectsTable.Controls[ii]).Text);
        lObject.OriginPosition = new Short3Coord();
        lObject.OriginPosition.X = (short)(short.Parse(((TextBox)Panel.ObjectsTable.Controls[ii + 1]).Text) + lDeltaX);
        lObject.OriginPosition.Y = (short)(short.Parse(((TextBox)Panel.ObjectsTable.Controls[ii + 2]).Text) + lDeltaY);
        lObject.OriginPosition.Z = (short)(short.Parse(((TextBox)Panel.ObjectsTable.Controls[ii + 3]).Text) + lDeltaZ);
        lObject.RotationVector = new Short3Coord();
        lObject.RotationVector.X = short.Parse(((TextBox)Panel.ObjectsTable.Controls[ii + 4]).Text);
        lObject.RotationVector.Y = short.Parse(((TextBox)Panel.ObjectsTable.Controls[ii + 5]).Text);
        lObject.RotationVector.Z = short.Parse(((TextBox)Panel.ObjectsTable.Controls[ii + 6]).Text);
        lObject.IsSolid = ((CheckBox)Panel.ObjectsTable.Controls[ii + 7]).Checked;
        lObject.FlagUnknown = ((CheckBox)Panel.ObjectsTable.Controls[ii + 8]).Checked;
        lObject.ShortUnknown = short.Parse(((TextBox)Panel.ObjectsTable.Controls[ii + 9]).Text);
        lObjects.Add(lObject);
      }

      lSizeIncrease = mSubject.ReplaceObjects(lObjects);
      lLevel.SHET.TrailingZeroByteCount -= lSizeIncrease;

      //=======================================================================
      // Warn if we've run out of space
      //=======================================================================
      if (lLevel.SHET.TrailingZeroByteCount < 0)
      {
        MessageBox.Show("WARNING: You do not currently have enough spare space at the end of your level file. " +
          string.Format("You will need to free up {0} bytes from the file before you can save to disk.", -lLevel.SHET.TrailingZeroByteCount),
          "MMEd", MessageBoxButtons.OK, MessageBoxIcon.Warning);
      }
    }

    void AddWeaponLink_Click(object xiSender, EventArgs xiArgs)
    {
      FlatChunk.WeaponEntry lWeapon = new FlatChunk.WeaponEntry();
      lWeapon.WeaponType = eWeaponType.Mines;
      lWeapon.ShortUnknown = 1;
      lWeapon.OriginPosition = new Short3Coord();
      lWeapon.OriginPosition.X = mSubject.OriginPosition.X;
      lWeapon.OriginPosition.Y = mSubject.OriginPosition.Y;
      lWeapon.OriginPosition.Z = mSubject.OriginPosition.Z;
      AddWeaponToTable(lWeapon);
    }

    void AddObjectLink_Click(object xiSender, EventArgs xiArgs)
    {
      FlatChunk.ObjectEntry lObject = new FlatChunk.ObjectEntry();
      lObject.ObjtType = 1;
      lObject.OriginPosition = new Short3Coord();
      lObject.OriginPosition.X = mSubject.OriginPosition.X;
      lObject.OriginPosition.Y = mSubject.OriginPosition.Y;
      lObject.OriginPosition.Z = mSubject.OriginPosition.Z;
      lObject.RotationVector = new Short3Coord();
      lObject.RotationVector.X = 0;
      lObject.RotationVector.Y = 0;
      lObject.RotationVector.Z = 0;
      lObject.IsSolid = true;
      lObject.FlagUnknown = true;
      lObject.ShortUnknown = 0;
      AddObjectToTable(lObject);
    }

    // FlatChunks can be viewed
    public override bool CanViewChunk(Chunk xiChunk)
    {
      return xiChunk is FlatChunk;
    }

    // Create an instance of the viewer manager class
    public static Viewer InitialiseViewer(MainForm xiMainForm)
    {
      return new FlatViewer(xiMainForm);
    }

    private FlatChunk mSubject = null;

    public override void SetSubject(Chunk xiChunk)
    {
      mSubject = xiChunk as FlatChunk;

      if (mSubject != null)
      {
        Level lLevel = mMainForm.CurrentLevel;

        Panel.SuspendLayout();
        Panel.WeaponsTable.SuspendLayout();
        Panel.ObjectsTable.SuspendLayout();

        //=====================================================================
        // Set the general Flat properties
        //=====================================================================
        Panel.IdTextBox.Text = mSubject.DeclaredIdx.ToString();
        Panel.NameTextBox.Text = mSubject.DeclaredName;
        Panel.OriginXTextBox.Text = mSubject.OriginPosition.X.ToString();
        Panel.OriginYTextBox.Text = mSubject.OriginPosition.Y.ToString();
        Panel.OriginZTextBox.Text = mSubject.OriginPosition.Z.ToString();
        Panel.RotationXTextBox.Text = mSubject.RotationVector.X.ToString();
        Panel.RotationYTextBox.Text = mSubject.RotationVector.Y.ToString();
        Panel.RotationZTextBox.Text = mSubject.RotationVector.Z.ToString();
        Panel.WidthTextBox.Text = mSubject.Width.ToString();
        Panel.HeightTextBox.Text = mSubject.Height.ToString();
        Panel.ScaleXTextBox.Text = mSubject.ScaleX.ToString();
        Panel.ScaleYTextBox.Text = mSubject.ScaleY.ToString();
        Panel.HasMetaDataCheckBox.Checked = mSubject.HasMetaData;
        Panel.FlagBCheckBox.Checked = mSubject.FlgB;
        Panel.FlagCCheckBox.Checked = mSubject.FlgC;
        Panel.VisibleCheckBox.Checked = mSubject.Visible;
        Panel.FlagECheckBox.Checked = mSubject.FlgE;
        Panel.NextNTextBox.Text = ArrayToString(mSubject.NextN);
        Panel.ByteSizeLabel.Text = string.Format("{0} bytes ({1} bytes free)", mSubject.ByteSize, lLevel.SHET.TrailingZeroByteCount);

        //=====================================================================
        // Set up the Weapons section
        //=====================================================================
        while (Panel.WeaponsTable.RowStyles.Count > 1)
        {
          Panel.WeaponsTable.RowStyles.RemoveAt(1);
          for (int ii = 0; ii < 6; ii++)
          {
            Panel.WeaponsTable.Controls.RemoveAt(6);
          }
        }

        if (mSubject.Weapons != null)
        {
          foreach (FlatChunk.WeaponEntry lWeapon in mSubject.Weapons)
          {
            AddWeaponToTable(lWeapon);
          }
        }

        //=====================================================================
        // Set up the Objects section
        //=====================================================================
        while (Panel.ObjectsTable.RowStyles.Count > 1)
        {
          Panel.ObjectsTable.RowStyles.RemoveAt(1);
          for (int ii = 0; ii < 11; ii++)
          {
            Panel.ObjectsTable.Controls.RemoveAt(11);
          }
        }

        if (mSubject.Objects != null)
        {
          foreach (FlatChunk.ObjectEntry lObject in mSubject.Objects)
          {
            AddObjectToTable(lObject);
          }
        }

        Panel.ObjectsTable.ResumeLayout();
        Panel.WeaponsTable.ResumeLayout();
        Panel.ResumeLayout();
      }
    }

    private void AddWeaponToTable(FlatChunk.WeaponEntry xiWeapon)
    {
      Panel.WeaponsTable.RowStyles.Add(new RowStyle(SizeType.AutoSize));

      ComboBox lWeaponTypeBox = new ComboBox();
      lWeaponTypeBox.Items.AddRange(Enum.GetNames(typeof(eWeaponType)));
      lWeaponTypeBox.Text = xiWeapon.WeaponType.ToString();
      lWeaponTypeBox.Width = 120;
      Panel.WeaponsTable.Controls.Add(lWeaponTypeBox);

      TextBox lUnknownBox = new TextBox();
      lUnknownBox.Text = xiWeapon.ShortUnknown.ToString();
      lUnknownBox.Width = 60;
      Panel.WeaponsTable.Controls.Add(lUnknownBox);

      TextBox lXBox = new TextBox();
      lXBox.Text = xiWeapon.OriginPosition.X.ToString();
      lXBox.Width = 60;
      Panel.WeaponsTable.Controls.Add(lXBox);

      TextBox lYBox = new TextBox();
      lYBox.Text = xiWeapon.OriginPosition.Y.ToString();
      lYBox.Width = 60;
      Panel.WeaponsTable.Controls.Add(lYBox);

      TextBox lZBox = new TextBox();
      lZBox.Text = xiWeapon.OriginPosition.Z.ToString();
      lZBox.Width = 60;
      Panel.WeaponsTable.Controls.Add(lZBox);

      LinkLabel lDeleteLink = new LinkLabel();
      lDeleteLink.Text = "Delete";
      lDeleteLink.Click += new EventHandler(delegate(object xiSender, System.EventArgs xiArgs)
      {
        Panel.WeaponsTable.SuspendLayout();
        Panel.WeaponsTable.RowStyles.RemoveAt(1);
        Panel.WeaponsTable.Controls.Remove(lWeaponTypeBox);
        Panel.WeaponsTable.Controls.Remove(lUnknownBox);
        Panel.WeaponsTable.Controls.Remove(lXBox);
        Panel.WeaponsTable.Controls.Remove(lYBox);
        Panel.WeaponsTable.Controls.Remove(lZBox);
        Panel.WeaponsTable.Controls.Remove(lDeleteLink);
        Panel.WeaponsTable.ResumeLayout();
      });
      Panel.WeaponsTable.Controls.Add(lDeleteLink);
    }

    private void AddObjectToTable(FlatChunk.ObjectEntry xiObject)
    {
      Panel.ObjectsTable.RowStyles.Add(new RowStyle(SizeType.AutoSize));

      TextBox lTypeBox = new TextBox();
      lTypeBox.Text = xiObject.ObjtType.ToString();
      lTypeBox.Width = 60;
      Panel.ObjectsTable.Controls.Add(lTypeBox);

      TextBox lXBox = new TextBox();
      lXBox.Text = xiObject.OriginPosition.X.ToString();
      lXBox.Width = 60;
      Panel.ObjectsTable.Controls.Add(lXBox);

      TextBox lYBox = new TextBox();
      lYBox.Text = xiObject.OriginPosition.Y.ToString();
      lYBox.Width = 60;
      Panel.ObjectsTable.Controls.Add(lYBox);

      TextBox lZBox = new TextBox();
      lZBox.Text = xiObject.OriginPosition.Z.ToString();
      lZBox.Width = 60;
      Panel.ObjectsTable.Controls.Add(lZBox);

      TextBox lRXBox = new TextBox();
      lRXBox.Text = xiObject.RotationVector.X.ToString();
      lRXBox.Width = 60;
      Panel.ObjectsTable.Controls.Add(lRXBox);

      TextBox lRYBox = new TextBox();
      lRYBox.Text = xiObject.RotationVector.Y.ToString();
      lRYBox.Width = 60;
      Panel.ObjectsTable.Controls.Add(lRYBox);

      TextBox lRZBox = new TextBox();
      lRZBox.Text = xiObject.RotationVector.Z.ToString();
      lRZBox.Width = 60;
      Panel.ObjectsTable.Controls.Add(lRZBox);

      CheckBox lSolidBox = new CheckBox();
      lSolidBox.Checked = xiObject.IsSolid;
      lSolidBox.Text = "";
      lSolidBox.AutoSize = true;
      Panel.ObjectsTable.Controls.Add(lSolidBox);

      CheckBox lUnkBox1 = new CheckBox();
      lUnkBox1.Checked = xiObject.FlagUnknown;
      lUnkBox1.Text = "";
      lUnkBox1.AutoSize = true;
      Panel.ObjectsTable.Controls.Add(lUnkBox1);

      TextBox lUnkBox2 = new TextBox();
      lUnkBox2.Text = xiObject.ShortUnknown.ToString();
      lUnkBox2.Width = 60;
      lUnkBox2.AutoSize = true;
      Panel.ObjectsTable.Controls.Add(lUnkBox2);

      LinkLabel lDeleteLink = new LinkLabel();
      lDeleteLink.Text = "Delete";
      lDeleteLink.Click += new EventHandler(delegate(object xiSender, System.EventArgs xiArgs)
      {
        Panel.ObjectsTable.SuspendLayout();
        Panel.ObjectsTable.RowStyles.RemoveAt(1);
        Panel.ObjectsTable.Controls.Remove(lTypeBox);
        Panel.ObjectsTable.Controls.Remove(lXBox);
        Panel.ObjectsTable.Controls.Remove(lYBox);
        Panel.ObjectsTable.Controls.Remove(lZBox);
        Panel.ObjectsTable.Controls.Remove(lRXBox);
        Panel.ObjectsTable.Controls.Remove(lRYBox);
        Panel.ObjectsTable.Controls.Remove(lRZBox);
        Panel.ObjectsTable.Controls.Remove(lSolidBox);
        Panel.ObjectsTable.Controls.Remove(lUnkBox1);
        Panel.ObjectsTable.Controls.Remove(lUnkBox2);
        Panel.ObjectsTable.Controls.Remove(lDeleteLink);
        Panel.ObjectsTable.ResumeLayout();
      });
      Panel.ObjectsTable.Controls.Add(lDeleteLink);
    }

    private string ArrayToString(byte[] xiArray)
    {
      StringBuilder lBuilder = new StringBuilder();
      foreach (byte lValue in xiArray)
      {
        if (lBuilder.Length > 0)
        {
          lBuilder.Append(", ");
        }
        lBuilder.Append(lValue.ToString());
      }
      return lBuilder.ToString();
    }

    public override System.Windows.Forms.TabPage Tab
    {
      get { return mMainForm.ViewTabFlat; }
    }

    protected Util.FlatEditorPanel Panel
    {
      get { return mMainForm.FlatPanel; }
    }
  }
}
