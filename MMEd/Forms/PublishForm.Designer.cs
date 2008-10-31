namespace MMEd
{
  partial class PublishForm
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
      this.label1 = new System.Windows.Forms.Label();
      this.BinaryFileTextBox = new System.Windows.Forms.TextBox();
      this.label4 = new System.Windows.Forms.Label();
      this.CourseDropDown = new System.Windows.Forms.ComboBox();
      this.OkButton = new System.Windows.Forms.Button();
      this.label3 = new System.Windows.Forms.Label();
      this.NameTextBox = new System.Windows.Forms.TextBox();
      this.BinaryFileSelect = new System.Windows.Forms.Button();
      this.CDImageGroupBox = new System.Windows.Forms.GroupBox();
      this.CDImageSelect = new System.Windows.Forms.Button();
      this.CDImageTextBox = new System.Windows.Forms.TextBox();
      this.label2 = new System.Windows.Forms.Label();
      this.UpdateCDImageCheckBox = new System.Windows.Forms.CheckBox();
      this.AbortButton = new System.Windows.Forms.Button();
      this.SelectFileDialog = new System.Windows.Forms.SaveFileDialog();
      this.BackupsCheckBox = new System.Windows.Forms.CheckBox();
      this.BackupCountUpDown = new System.Windows.Forms.NumericUpDown();
      this.CDImageGroupBox.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.BackupCountUpDown)).BeginInit();
      this.SuspendLayout();
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(12, 9);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(87, 13);
      this.label1.TabIndex = 0;
      this.label1.Text = "Export binary file:";
      // 
      // BinaryFileTextBox
      // 
      this.BinaryFileTextBox.CausesValidation = false;
      this.BinaryFileTextBox.Location = new System.Drawing.Point(105, 9);
      this.BinaryFileTextBox.Name = "BinaryFileTextBox";
      this.BinaryFileTextBox.Size = new System.Drawing.Size(260, 20);
      this.BinaryFileTextBox.TabIndex = 1;
      this.BinaryFileTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.BinaryFileTextBox_Validating);
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.Location = new System.Drawing.Point(11, 48);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(43, 13);
      this.label4.TabIndex = 3;
      this.label4.Text = "Course:";
      // 
      // CourseDropDown
      // 
      this.CourseDropDown.CausesValidation = false;
      this.CourseDropDown.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.CourseDropDown.FormattingEnabled = true;
      this.CourseDropDown.Location = new System.Drawing.Point(74, 48);
      this.CourseDropDown.Name = "CourseDropDown";
      this.CourseDropDown.Size = new System.Drawing.Size(299, 21);
      this.CourseDropDown.TabIndex = 4;
      this.CourseDropDown.Validating += new System.ComponentModel.CancelEventHandler(this.CourseDropDown_Validating);
      this.CourseDropDown.SelectedIndexChanged += new System.EventHandler(this.CourseDropDown_SelectedIndexChanged);
      // 
      // OkButton
      // 
      this.OkButton.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.OkButton.Location = new System.Drawing.Point(238, 196);
      this.OkButton.Name = "OkButton";
      this.OkButton.Size = new System.Drawing.Size(75, 23);
      this.OkButton.TabIndex = 7;
      this.OkButton.Text = "OK";
      this.OkButton.UseVisualStyleBackColor = true;
      this.OkButton.Click += new System.EventHandler(this.OkButton_Click);
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(11, 75);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(38, 13);
      this.label3.TabIndex = 5;
      this.label3.Text = "Name:";
      // 
      // NameTextBox
      // 
      this.NameTextBox.CausesValidation = false;
      this.NameTextBox.Location = new System.Drawing.Point(74, 75);
      this.NameTextBox.Name = "NameTextBox";
      this.NameTextBox.Size = new System.Drawing.Size(179, 20);
      this.NameTextBox.TabIndex = 6;
      this.NameTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.NameTextBox_Validating);
      // 
      // BinaryFileSelect
      // 
      this.BinaryFileSelect.CausesValidation = false;
      this.BinaryFileSelect.Location = new System.Drawing.Point(372, 9);
      this.BinaryFileSelect.Name = "BinaryFileSelect";
      this.BinaryFileSelect.Size = new System.Drawing.Size(23, 23);
      this.BinaryFileSelect.TabIndex = 2;
      this.BinaryFileSelect.Text = "...";
      this.BinaryFileSelect.UseVisualStyleBackColor = true;
      this.BinaryFileSelect.Click += new System.EventHandler(this.BinaryFileSelect_Click);
      // 
      // CDImageGroupBox
      // 
      this.CDImageGroupBox.Controls.Add(this.CDImageSelect);
      this.CDImageGroupBox.Controls.Add(this.CDImageTextBox);
      this.CDImageGroupBox.Controls.Add(this.label2);
      this.CDImageGroupBox.Controls.Add(this.CourseDropDown);
      this.CDImageGroupBox.Controls.Add(this.label4);
      this.CDImageGroupBox.Controls.Add(this.NameTextBox);
      this.CDImageGroupBox.Controls.Add(this.label3);
      this.CDImageGroupBox.Enabled = false;
      this.CDImageGroupBox.Location = new System.Drawing.Point(15, 82);
      this.CDImageGroupBox.Name = "CDImageGroupBox";
      this.CDImageGroupBox.Size = new System.Drawing.Size(379, 108);
      this.CDImageGroupBox.TabIndex = 6;
      this.CDImageGroupBox.TabStop = false;
      this.CDImageGroupBox.Text = "Update CD Image";
      // 
      // CDImageSelect
      // 
      this.CDImageSelect.CausesValidation = false;
      this.CDImageSelect.Location = new System.Drawing.Point(350, 19);
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
      this.CDImageTextBox.Location = new System.Drawing.Point(74, 19);
      this.CDImageTextBox.Name = "CDImageTextBox";
      this.CDImageTextBox.Size = new System.Drawing.Size(270, 20);
      this.CDImageTextBox.TabIndex = 1;
      this.CDImageTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.CDImageTextBox_Validating);
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(11, 19);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(57, 13);
      this.label2.TabIndex = 0;
      this.label2.Text = "CD Image:";
      // 
      // UpdateCDImageCheckBox
      // 
      this.UpdateCDImageCheckBox.AutoSize = true;
      this.UpdateCDImageCheckBox.CausesValidation = false;
      this.UpdateCDImageCheckBox.Location = new System.Drawing.Point(105, 59);
      this.UpdateCDImageCheckBox.Name = "UpdateCDImageCheckBox";
      this.UpdateCDImageCheckBox.Size = new System.Drawing.Size(110, 17);
      this.UpdateCDImageCheckBox.TabIndex = 5;
      this.UpdateCDImageCheckBox.Text = "Update CD image";
      this.UpdateCDImageCheckBox.UseVisualStyleBackColor = true;
      this.UpdateCDImageCheckBox.CheckedChanged += new System.EventHandler(this.UpdateCDImageCheckBox_CheckedChanged);
      // 
      // AbortButton
      // 
      this.AbortButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.AbortButton.Location = new System.Drawing.Point(319, 196);
      this.AbortButton.Name = "AbortButton";
      this.AbortButton.Size = new System.Drawing.Size(75, 23);
      this.AbortButton.TabIndex = 8;
      this.AbortButton.Text = "Cancel";
      this.AbortButton.UseVisualStyleBackColor = true;
      // 
      // BackupsCheckBox
      // 
      this.BackupsCheckBox.AutoSize = true;
      this.BackupsCheckBox.Location = new System.Drawing.Point(105, 36);
      this.BackupsCheckBox.Name = "BackupsCheckBox";
      this.BackupsCheckBox.Size = new System.Drawing.Size(98, 17);
      this.BackupsCheckBox.TabIndex = 3;
      this.BackupsCheckBox.Text = "Keep backups:";
      this.BackupsCheckBox.UseVisualStyleBackColor = true;
      // 
      // BackupCountUpDown
      // 
      this.BackupCountUpDown.Location = new System.Drawing.Point(210, 36);
      this.BackupCountUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
      this.BackupCountUpDown.Name = "BackupCountUpDown";
      this.BackupCountUpDown.Size = new System.Drawing.Size(41, 20);
      this.BackupCountUpDown.TabIndex = 4;
      this.BackupCountUpDown.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
      // 
      // PublishForm
      // 
      this.AcceptButton = this.OkButton;
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.CancelButton = this.AbortButton;
      this.ClientSize = new System.Drawing.Size(406, 228);
      this.Controls.Add(this.BackupCountUpDown);
      this.Controls.Add(this.BackupsCheckBox);
      this.Controls.Add(this.AbortButton);
      this.Controls.Add(this.UpdateCDImageCheckBox);
      this.Controls.Add(this.CDImageGroupBox);
      this.Controls.Add(this.BinaryFileSelect);
      this.Controls.Add(this.OkButton);
      this.Controls.Add(this.BinaryFileTextBox);
      this.Controls.Add(this.label1);
      this.Name = "PublishForm";
      this.Text = "Publish Course";
      this.CDImageGroupBox.ResumeLayout(false);
      this.CDImageGroupBox.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.BackupCountUpDown)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.Button OkButton;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.Button BinaryFileSelect;
    private System.Windows.Forms.GroupBox CDImageGroupBox;
    private System.Windows.Forms.Button AbortButton;
    private System.Windows.Forms.Button CDImageSelect;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.SaveFileDialog SelectFileDialog;
    internal System.Windows.Forms.TextBox BinaryFileTextBox;
    internal System.Windows.Forms.ComboBox CourseDropDown;
    internal System.Windows.Forms.TextBox NameTextBox;
    internal System.Windows.Forms.CheckBox UpdateCDImageCheckBox;
    internal System.Windows.Forms.TextBox CDImageTextBox;
    internal System.Windows.Forms.CheckBox BackupsCheckBox;
    internal System.Windows.Forms.NumericUpDown BackupCountUpDown;
  }
}