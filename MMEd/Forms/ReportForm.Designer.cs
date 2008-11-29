namespace MMEd.Forms
{
  partial class ReportForm
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
      this.ReportBox = new System.Windows.Forms.TextBox();
      this.CloseButton = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // ReportBox
      // 
      this.ReportBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.ReportBox.Location = new System.Drawing.Point(13, 13);
      this.ReportBox.Multiline = true;
      this.ReportBox.Name = "ReportBox";
      this.ReportBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
      this.ReportBox.Size = new System.Drawing.Size(402, 353);
      this.ReportBox.TabIndex = 0;
      // 
      // CloseButton
      // 
      this.CloseButton.Location = new System.Drawing.Point(340, 372);
      this.CloseButton.Name = "CloseButton";
      this.CloseButton.Size = new System.Drawing.Size(75, 23);
      this.CloseButton.TabIndex = 1;
      this.CloseButton.Text = "Close";
      this.CloseButton.UseVisualStyleBackColor = true;
      this.CloseButton.Click += new System.EventHandler(this.CloseButton_Click);
      // 
      // ReportForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(427, 407);
      this.Controls.Add(this.CloseButton);
      this.Controls.Add(this.ReportBox);
      this.Name = "ReportForm";
      this.Text = "ReportForm";
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.TextBox ReportBox;
    private System.Windows.Forms.Button CloseButton;
  }
}