using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using MMEd.Util;
using System.IO;

namespace MMEd
{
  public partial class PublishForm : Form
  {
    private MainForm mMainForm;

    public PublishForm(MainForm xiMainForm)
    {
      mMainForm = xiMainForm;

      InitializeComponent();

      CourseDropDown.Items.AddRange(Util.MMCD.Courses.ToArray());
      BinaryFileTextBox.Text = Path.ChangeExtension(mMainForm.LocalSettings.LastOpenedFile, ".dat");
      UpdateCDImageCheckBox.Checked = mMainForm.LocalSettings.LastPublishUpdatedCDImage;
      CDImageTextBox.Text = mMainForm.LocalSettings.LastPublishedCDImage;
      CourseDropDown.SelectedItem = mMainForm.LocalSettings.LastPublishedCourse;
      NameTextBox.Text = mMainForm.LocalSettings.LastPublishedCourseName;
      BackupsCheckBox.Checked = mMainForm.LocalSettings.LastPublishedKeepBackups;
      BackupCountUpDown.Value = mMainForm.LocalSettings.LastPublishedBackupCount;
    }

    private void UpdateCDImageCheckBox_CheckedChanged(object sender, EventArgs e)
    {
      CDImageGroupBox.Enabled = UpdateCDImageCheckBox.Checked;
    }

    private void BinaryFileSelect_Click(object sender, EventArgs e)
    {
      SelectFileDialog.Title = "Export Binary File";
      SelectFileDialog.DefaultExt = ".dat";
      SelectFileDialog.CheckFileExists = false;
      SelectFileDialog.OverwritePrompt = true;
      SelectFileDialog.Filter = "Binary Courses (*.dat)|*.dat|All Files (*.*)|*.*";
      SelectFileDialog.InitialDirectory = mMainForm.LocalSettings.LastPublishedBinaryFilePath;
      SelectFileDialog.FileName = BinaryFileTextBox.Text;

      if (SelectFileDialog.ShowDialog() == DialogResult.OK)
      {
        BinaryFileTextBox.Text = SelectFileDialog.FileName;
      }
    }

    private void CDImageSelect_Click(object sender, EventArgs e)
    {
      SelectFileDialog.Title = "Update CD Image";
      SelectFileDialog.DefaultExt = ".bin";
      SelectFileDialog.CheckFileExists = true;
      SelectFileDialog.OverwritePrompt = false;
      SelectFileDialog.Filter = "CD Images (*.bin)|*.bin|All Files (*.*)|*.*";
      SelectFileDialog.FileName = CDImageTextBox.Text;

      if (SelectFileDialog.ShowDialog() == DialogResult.OK)
      {
        CDImageTextBox.Text = SelectFileDialog.FileName;
      }
    }

    private void BinaryFileTextBox_Validating(object sender, CancelEventArgs e)
    {
      if (BinaryFileTextBox.Text == null || BinaryFileTextBox.Text == "")
      {
        MessageBox.Show("Please select a binary file to export to.", "Publish Course", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        e.Cancel = true;
      }
    }

    private void CDImageTextBox_Validating(object sender, CancelEventArgs e)
    {
      if (!UpdateCDImageCheckBox.Checked)
      {
        return;
      }

      if (CDImageTextBox.Text == null || CDImageTextBox.Text == "")
      {
        MessageBox.Show("Please select a CD image to update.", "Publish Course", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        e.Cancel = true;
      }
    }

    private void CourseDropDown_Validating(object sender, CancelEventArgs e)
    {
      if (!UpdateCDImageCheckBox.Checked)
      {
        return;
      }

      if (CourseDropDown.SelectedItem == null)
      {
        MessageBox.Show("Please select which course you're editing.", "Publish Course", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        e.Cancel = true;
      }
    }

    private void CourseDropDown_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (CourseDropDown.SelectedItem == null)
      {
        return;
      }
      
      NameTextBox.Text = ((MMCD.Course)CourseDropDown.SelectedItem).CourseName;
      NameTextBox.MaxLength = NameTextBox.Text.Length;
    }

    private void NameTextBox_Validating(object sender, CancelEventArgs e)
    {
      if (!UpdateCDImageCheckBox.Checked)
      {
        return;
      }

      if (NameTextBox.Text == null || NameTextBox.Text == "")
      {
        MessageBox.Show("Please enter the name of your course.", "Publish Course", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        e.Cancel = true;
      }
    }

    private void OkButton_Click(object sender, EventArgs e)
    {
      mMainForm.LocalSettings.LastPublishedBinaryFilePath = new FileInfo(BinaryFileTextBox.Text).Directory.FullName;
      mMainForm.LocalSettings.LastPublishedKeepBackups = BackupsCheckBox.Checked;
      mMainForm.LocalSettings.LastPublishUpdatedCDImage = UpdateCDImageCheckBox.Checked;

      if (BackupsCheckBox.Checked)
      {
        mMainForm.LocalSettings.LastPublishedBackupCount = (int)BackupCountUpDown.Value;
      }

      if (UpdateCDImageCheckBox.Checked)
      {
        mMainForm.LocalSettings.LastPublishedCDImage = CDImageTextBox.Text;
        mMainForm.LocalSettings.LastPublishedCourse = ((MMCD.Course)CourseDropDown.SelectedItem).FileName;
        mMainForm.LocalSettings.LastPublishedCourseName = NameTextBox.Text;
      }
    }
  }
}