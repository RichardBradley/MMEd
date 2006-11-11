namespace MMEd.Util
{
  partial class FlatEditorPanel
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

    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.label1 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.label3 = new System.Windows.Forms.Label();
      this.label4 = new System.Windows.Forms.Label();
      this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
      this.RotationXTextBox = new System.Windows.Forms.TextBox();
      this.NameTextBox = new System.Windows.Forms.TextBox();
      this.label6 = new System.Windows.Forms.Label();
      this.label5 = new System.Windows.Forms.Label();
      this.IdTextBox = new System.Windows.Forms.TextBox();
      this.OriginXTextBox = new System.Windows.Forms.TextBox();
      this.OriginYTextBox = new System.Windows.Forms.TextBox();
      this.OriginZTextBox = new System.Windows.Forms.TextBox();
      this.RotationYTextBox = new System.Windows.Forms.TextBox();
      this.RotationZTextBox = new System.Windows.Forms.TextBox();
      this.WidthTextBox = new System.Windows.Forms.TextBox();
      this.HeightTextBox = new System.Windows.Forms.TextBox();
      this.ScaleXTextBox = new System.Windows.Forms.TextBox();
      this.ScaleYTextBox = new System.Windows.Forms.TextBox();
      this.tableLayoutPanel1.SuspendLayout();
      this.SuspendLayout();
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(3, 0);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(21, 13);
      this.label1.TabIndex = 0;
      this.label1.Text = "ID:";
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(3, 26);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(38, 13);
      this.label2.TabIndex = 1;
      this.label2.Text = "Name:";
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(3, 52);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(37, 13);
      this.label3.TabIndex = 2;
      this.label3.Text = "Origin:";
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.Location = new System.Drawing.Point(3, 78);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(50, 13);
      this.label4.TabIndex = 3;
      this.label4.Text = "Rotation:";
      // 
      // tableLayoutPanel1
      // 
      this.tableLayoutPanel1.ColumnCount = 4;
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel1.Controls.Add(this.NameTextBox, 1, 1);
      this.tableLayoutPanel1.Controls.Add(this.label6, 0, 5);
      this.tableLayoutPanel1.Controls.Add(this.label5, 0, 4);
      this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
      this.tableLayoutPanel1.Controls.Add(this.label4, 0, 3);
      this.tableLayoutPanel1.Controls.Add(this.label2, 0, 1);
      this.tableLayoutPanel1.Controls.Add(this.label3, 0, 2);
      this.tableLayoutPanel1.Controls.Add(this.IdTextBox, 1, 0);
      this.tableLayoutPanel1.Controls.Add(this.RotationXTextBox, 1, 3);
      this.tableLayoutPanel1.Controls.Add(this.OriginXTextBox, 1, 2);
      this.tableLayoutPanel1.Controls.Add(this.OriginYTextBox, 2, 2);
      this.tableLayoutPanel1.Controls.Add(this.OriginZTextBox, 3, 2);
      this.tableLayoutPanel1.Controls.Add(this.RotationYTextBox, 2, 3);
      this.tableLayoutPanel1.Controls.Add(this.RotationZTextBox, 3, 3);
      this.tableLayoutPanel1.Controls.Add(this.WidthTextBox, 1, 4);
      this.tableLayoutPanel1.Controls.Add(this.HeightTextBox, 2, 4);
      this.tableLayoutPanel1.Controls.Add(this.ScaleXTextBox, 1, 5);
      this.tableLayoutPanel1.Controls.Add(this.ScaleYTextBox, 2, 5);
      this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 3);
      this.tableLayoutPanel1.Name = "tableLayoutPanel1";
      this.tableLayoutPanel1.RowCount = 6;
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel1.Size = new System.Drawing.Size(287, 160);
      this.tableLayoutPanel1.TabIndex = 4;
      // 
      // RotationXTextBox
      // 
      this.RotationXTextBox.Location = new System.Drawing.Point(59, 81);
      this.RotationXTextBox.Name = "RotationXTextBox";
      this.RotationXTextBox.Size = new System.Drawing.Size(42, 20);
      this.RotationXTextBox.TabIndex = 8;
      this.RotationXTextBox.TextChanged += new System.EventHandler(this.textBox3_TextChanged);
      // 
      // NameTextBox
      // 
      this.tableLayoutPanel1.SetColumnSpan(this.NameTextBox, 3);
      this.NameTextBox.Location = new System.Drawing.Point(59, 29);
      this.NameTextBox.MaxLength = 8;
      this.NameTextBox.Name = "NameTextBox";
      this.NameTextBox.Size = new System.Drawing.Size(216, 20);
      this.NameTextBox.TabIndex = 7;
      // 
      // label6
      // 
      this.label6.AutoSize = true;
      this.label6.Location = new System.Drawing.Point(3, 130);
      this.label6.Name = "label6";
      this.label6.Size = new System.Drawing.Size(37, 13);
      this.label6.TabIndex = 5;
      this.label6.Text = "Scale:";
      // 
      // label5
      // 
      this.label5.AutoSize = true;
      this.label5.Location = new System.Drawing.Point(3, 104);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(30, 13);
      this.label5.TabIndex = 5;
      this.label5.Text = "Size:";
      // 
      // IdTextBox
      // 
      this.IdTextBox.Location = new System.Drawing.Point(59, 3);
      this.IdTextBox.Name = "IdTextBox";
      this.IdTextBox.ReadOnly = true;
      this.IdTextBox.Size = new System.Drawing.Size(42, 20);
      this.IdTextBox.TabIndex = 6;
      // 
      // OriginXTextBox
      // 
      this.OriginXTextBox.Location = new System.Drawing.Point(59, 55);
      this.OriginXTextBox.Name = "OriginXTextBox";
      this.OriginXTextBox.Size = new System.Drawing.Size(42, 20);
      this.OriginXTextBox.TabIndex = 9;
      // 
      // OriginYTextBox
      // 
      this.OriginYTextBox.Location = new System.Drawing.Point(107, 55);
      this.OriginYTextBox.Name = "OriginYTextBox";
      this.OriginYTextBox.Size = new System.Drawing.Size(42, 20);
      this.OriginYTextBox.TabIndex = 10;
      // 
      // OriginZTextBox
      // 
      this.OriginZTextBox.Location = new System.Drawing.Point(155, 55);
      this.OriginZTextBox.Name = "OriginZTextBox";
      this.OriginZTextBox.Size = new System.Drawing.Size(42, 20);
      this.OriginZTextBox.TabIndex = 11;
      // 
      // RotationYTextBox
      // 
      this.RotationYTextBox.Location = new System.Drawing.Point(107, 81);
      this.RotationYTextBox.Name = "RotationYTextBox";
      this.RotationYTextBox.Size = new System.Drawing.Size(42, 20);
      this.RotationYTextBox.TabIndex = 12;
      // 
      // RotationZTextBox
      // 
      this.RotationZTextBox.Location = new System.Drawing.Point(155, 81);
      this.RotationZTextBox.Name = "RotationZTextBox";
      this.RotationZTextBox.Size = new System.Drawing.Size(42, 20);
      this.RotationZTextBox.TabIndex = 13;
      // 
      // WidthTextBox
      // 
      this.WidthTextBox.Location = new System.Drawing.Point(59, 107);
      this.WidthTextBox.Name = "WidthTextBox";
      this.WidthTextBox.Size = new System.Drawing.Size(42, 20);
      this.WidthTextBox.TabIndex = 14;
      // 
      // HeightTextBox
      // 
      this.HeightTextBox.Location = new System.Drawing.Point(107, 107);
      this.HeightTextBox.Name = "HeightTextBox";
      this.HeightTextBox.Size = new System.Drawing.Size(42, 20);
      this.HeightTextBox.TabIndex = 15;
      // 
      // ScaleXTextBox
      // 
      this.ScaleXTextBox.Location = new System.Drawing.Point(59, 133);
      this.ScaleXTextBox.Name = "ScaleXTextBox";
      this.ScaleXTextBox.Size = new System.Drawing.Size(42, 20);
      this.ScaleXTextBox.TabIndex = 16;
      // 
      // ScaleYTextBox
      // 
      this.ScaleYTextBox.Location = new System.Drawing.Point(107, 133);
      this.ScaleYTextBox.Name = "ScaleYTextBox";
      this.ScaleYTextBox.Size = new System.Drawing.Size(42, 20);
      this.ScaleYTextBox.TabIndex = 17;
      // 
      // FlatEditorPanel
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.tableLayoutPanel1);
      this.Name = "FlatEditorPanel";
      this.Size = new System.Drawing.Size(405, 450);
      this.tableLayoutPanel1.ResumeLayout(false);
      this.tableLayoutPanel1.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    private System.Windows.Forms.Label label6;
    private System.Windows.Forms.Label label5;
    internal System.Windows.Forms.TextBox OriginXTextBox;
    internal System.Windows.Forms.TextBox OriginYTextBox;
    internal System.Windows.Forms.TextBox OriginZTextBox;
    internal System.Windows.Forms.TextBox RotationYTextBox;
    internal System.Windows.Forms.TextBox RotationZTextBox;
    internal System.Windows.Forms.TextBox WidthTextBox;
    internal System.Windows.Forms.TextBox HeightTextBox;
    internal System.Windows.Forms.TextBox ScaleXTextBox;
    internal System.Windows.Forms.TextBox ScaleYTextBox;
    internal System.Windows.Forms.TextBox RotationXTextBox;
    internal System.Windows.Forms.TextBox NameTextBox;
    internal System.Windows.Forms.TextBox IdTextBox;
  }
}
