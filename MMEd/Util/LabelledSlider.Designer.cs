namespace MMEd.Util
{
  partial class LabelledSlider
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
      this.TrackBar = new System.Windows.Forms.TrackBar();
      this.LabelMin = new System.Windows.Forms.Label();
      this.LabelMax = new System.Windows.Forms.Label();
      ((System.ComponentModel.ISupportInitialize)(this.TrackBar)).BeginInit();
      this.SuspendLayout();
      // 
      // TrackBar
      // 
      this.TrackBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.TrackBar.Location = new System.Drawing.Point(0, 0);
      this.TrackBar.Name = "TrackBar";
      this.TrackBar.Size = new System.Drawing.Size(144, 45);
      this.TrackBar.TabIndex = 0;
      this.TrackBar.TickStyle = System.Windows.Forms.TickStyle.None;
      // 
      // LabelMin
      // 
      this.LabelMin.Location = new System.Drawing.Point(3, 26);
      this.LabelMin.Name = "LabelMin";
      this.LabelMin.Size = new System.Drawing.Size(62, 13);
      this.LabelMin.TabIndex = 1;
      this.LabelMin.Text = "Min";
      // 
      // LabelMax
      // 
      this.LabelMax.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.LabelMax.Location = new System.Drawing.Point(82, 26);
      this.LabelMax.Name = "LabelMax";
      this.LabelMax.Size = new System.Drawing.Size(59, 13);
      this.LabelMax.TabIndex = 2;
      this.LabelMax.Text = "Max";
      this.LabelMax.TextAlign = System.Drawing.ContentAlignment.TopRight;
      // 
      // LabelledSlider
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.LabelMax);
      this.Controls.Add(this.LabelMin);
      this.Controls.Add(this.TrackBar);
      this.Name = "LabelledSlider";
      this.Size = new System.Drawing.Size(147, 45);
      ((System.ComponentModel.ISupportInitialize)(this.TrackBar)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.TrackBar TrackBar;
    private System.Windows.Forms.Label LabelMin;
    private System.Windows.Forms.Label LabelMax;
  }
}
