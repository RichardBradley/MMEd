using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace MMEd.Forms
{
  public partial class NewForm : Form
  {
    private MainForm mMainForm;

    public NewForm(MainForm xiMainForm)
    {
      mMainForm = xiMainForm;
      
      InitializeComponent();

      CourseDropDown.Items.AddRange(Util.MMCD.Courses.ToArray());
      CDImageTextBox.Text = mMainForm.LocalSettings.LastCleanCDImage;
    }

    private void CDImageSelect_Click(object sender, EventArgs e)
    {
      SelectFileDialog.Title = "Extract From CD Image";
      SelectFileDialog.DefaultExt = ".bin";
      SelectFileDialog.CheckFileExists = true;
      SelectFileDialog.Filter = "CD Images (*.bin)|*.bin|All Files (*.*)|*.*";
      SelectFileDialog.FileName = CDImageTextBox.Text;

      if (SelectFileDialog.ShowDialog() == DialogResult.OK)
      {
        CDImageTextBox.Text = SelectFileDialog.FileName;
      }
    }

    private void OkButton_Click(object sender, EventArgs e)
    {
      mMainForm.LocalSettings.LastCleanCDImage = CDImageTextBox.Text;
    }

    private void CDImageTextBox_Validating(object sender, CancelEventArgs e)
    {
      if (CDImageTextBox.Text == null || CDImageTextBox.Text == "")
      {
        MessageBox.Show("Please select a CD image to load your new level from.", "New Course", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        e.Cancel = true;
      }
    }

    private void CourseDropDown_Validating(object sender, CancelEventArgs e)
    {
      if (CourseDropDown.SelectedItem == null)
      {
        MessageBox.Show("Please select which course you'd like to edit.", "New Course", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        e.Cancel = true;
      }
    }
  }
}