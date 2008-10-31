namespace MMEd.Forms
{
  partial class NewForm
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.CDImageSelect = new System.Windows.Forms.Button();
      this.CDImageTextBox = new System.Windows.Forms.TextBox();
      this.label2 = new System.Windows.Forms.Label();
      this.CourseDropDown = new System.Windows.Forms.ComboBox();
      this.label4 = new System.Windows.Forms.Label();
      this.AbortButton = new System.Windows.Forms.Button();
      this.OkButton = new System.Windows.Forms.Button();
      this.SelectFileDialog = new System.Windows.Forms.OpenFileDialog();
      this.SuspendLayout();
      // 
      // CDImageSelect
      // 
      this.CDImageSelect.CausesValidation = false;
      this.CDImageSelect.Location = new System.Drawing.Point(348, 12);
      this.CDImageSelect.Name = "CDImageSelect";
      this.CDImageSelect.Size = new System.Drawing.Size(23, 23);
      this.CDImageSelect.TabIndex = 2;
      this.CDImageSelect.Text = "...";
      this.CDImageSelect.UseVisualStyleBackColor = true;
      this.CDImageSelect.Click += new System.EventHandler(this.CDImageSelect_Click);
      // 
      // CDImageTextBox
      // 
      this.CDImageTextBox.CausesValidation = false;
      this.CDImageTextBox.Location = new System.Drawing.Point(72, 12);
      this.CDImageTextBox.Name = "CDImageTextBox";
      this.CDImageTextBox.Size = new System.Drawing.Size(270, 20);
      this.CDImageTextBox.TabIndex = 1;
      this.CDImageTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.CDImageTextBox_Validating);
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(9, 12);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(57, 13);
      this.label2.TabIndex = 0;
      this.label2.Text = "CD Image:";
      // 
      // CourseDropDown
      // 
      this.CourseDropDown.CausesValidation = false;
      this.CourseDropDown.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.CourseDropDown.FormattingEnabled = true;
      this.CourseDropDown.Location = new System.Drawing.Point(72, 41);
      this.CourseDropDown.Name = "CourseDropDown";
      this.CourseDropDown.Size = new System.Drawing.Size(299, 21);
      this.CourseDropDown.TabIndex = 4;
      this.CourseDropDown.Validating += new System.ComponentModel.CancelEventHandler(this.CourseDropDown_Validating);
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.Location = new System.Drawing.Point(9, 41);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(43, 13);
      this.label4.TabIndex = 3;
      this.label4.Text = "Course:";
      // 
      // AbortButton
      // 
      this.AbortButton.CausesValidation = false;
      this.AbortButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.AbortButton.Location = new System.Drawing.Point(297, 68);
      this.AbortButton.Name = "AbortButton";
      this.AbortButton.Size = new System.Drawing.Size(75, 23);
      this.AbortButton.TabIndex = 6;
      this.AbortButton.Text = "Cancel";
      this.AbortButton.UseVisualStyleBackColor = true;
      // 
      // OkButton
      // 
      this.OkButton.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.OkButton.Location = new System.Drawing.Point(216, 68);
      this.OkButton.Name = "OkButton";
      this.OkButton.Size = new System.Drawing.Size(75, 23);
      this.OkButton.TabIndex = 5;
      this.OkButton.Text = "OK";
      this.OkButton.UseVisualStyleBackColor = true;
      this.OkButton.Click += new System.EventHandler(this.OkButton_Click);
      // 
      // NewForm
      // 
      this.AcceptButton = this.OkButton;
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.CancelButton = this.AbortButton;
      this.ClientSize = new System.Drawing.Size(383, 102);
      this.Controls.Add(this.AbortButton);
      this.Controls.Add(this.OkButton);
      this.Controls.Add(this.CDImageSelect);
      this.Controls.Add(this.CDImageTextBox);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.CourseDropDown);
      this.Controls.Add(this.label4);
      this.Name = "NewForm";
      this.Text = "New Course";
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Button CDImageSelect;
    internal System.Windows.Forms.TextBox CDImageTextBox;
    private System.Windows.Forms.Label label2;
    internal System.Windows.Forms.ComboBox CourseDropDown;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.Button AbortButton;
    private System.Windows.Forms.Button OkButton;
    private System.Windows.Forms.OpenFileDialog SelectFileDialog;
  }
}