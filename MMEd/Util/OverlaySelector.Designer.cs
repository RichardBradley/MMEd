namespace MMEd.Util
{
  partial class OverlaySelector
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
      this.LinkRed = new System.Windows.Forms.LinkLabel();
      this.LinkWhite = new System.Windows.Forms.LinkLabel();
      this.Checkbox = new System.Windows.Forms.CheckBox();
      this.LinkBlack = new System.Windows.Forms.LinkLabel();
      this.LinkGray = new System.Windows.Forms.LinkLabel();
      this.SuspendLayout();
      // 
      // LinkRed
      // 
      this.LinkRed.AutoSize = true;
      this.LinkRed.LinkColor = System.Drawing.Color.Red;
      this.LinkRed.Location = new System.Drawing.Point(68, 4);
      this.LinkRed.Name = "LinkRed";
      this.LinkRed.Size = new System.Drawing.Size(15, 13);
      this.LinkRed.TabIndex = 14;
      this.LinkRed.TabStop = true;
      this.LinkRed.Text = "R";
      this.LinkRed.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkRed_LinkClicked);
      // 
      // LinkWhite
      // 
      this.LinkWhite.ActiveLinkColor = System.Drawing.Color.White;
      this.LinkWhite.AutoSize = true;
      this.LinkWhite.LinkColor = System.Drawing.Color.White;
      this.LinkWhite.Location = new System.Drawing.Point(81, 4);
      this.LinkWhite.Name = "LinkWhite";
      this.LinkWhite.Size = new System.Drawing.Size(18, 13);
      this.LinkWhite.TabIndex = 13;
      this.LinkWhite.TabStop = true;
      this.LinkWhite.Text = "W";
      this.LinkWhite.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkWhite_LinkClicked);
      // 
      // Checkbox
      // 
      this.Checkbox.AutoSize = true;
      this.Checkbox.Location = new System.Drawing.Point(0, 3);
      this.Checkbox.Name = "Checkbox";
      this.Checkbox.Size = new System.Drawing.Size(15, 14);
      this.Checkbox.TabIndex = 12;
      this.Checkbox.UseVisualStyleBackColor = true;
      // 
      // LinkBlack
      // 
      this.LinkBlack.ActiveLinkColor = System.Drawing.Color.Black;
      this.LinkBlack.AutoSize = true;
      this.LinkBlack.LinkColor = System.Drawing.Color.Black;
      this.LinkBlack.Location = new System.Drawing.Point(109, 4);
      this.LinkBlack.Name = "LinkBlack";
      this.LinkBlack.Size = new System.Drawing.Size(14, 13);
      this.LinkBlack.TabIndex = 17;
      this.LinkBlack.TabStop = true;
      this.LinkBlack.Text = "B";
      this.LinkBlack.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkBlack_LinkClicked);
      // 
      // LinkGray
      // 
      this.LinkGray.ActiveLinkColor = System.Drawing.Color.Gray;
      this.LinkGray.AutoSize = true;
      this.LinkGray.LinkColor = System.Drawing.Color.Gray;
      this.LinkGray.Location = new System.Drawing.Point(96, 4);
      this.LinkGray.Name = "LinkGray";
      this.LinkGray.Size = new System.Drawing.Size(15, 13);
      this.LinkGray.TabIndex = 18;
      this.LinkGray.TabStop = true;
      this.LinkGray.Text = "G";
      this.LinkGray.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkGray_LinkClicked);
      // 
      // OverlaySelector
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.LinkGray);
      this.Controls.Add(this.LinkBlack);
      this.Controls.Add(this.LinkRed);
      this.Controls.Add(this.LinkWhite);
      this.Controls.Add(this.Checkbox);
      this.Name = "OverlaySelector";
      this.Size = new System.Drawing.Size(121, 20);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.LinkLabel LinkRed;
    private System.Windows.Forms.LinkLabel LinkWhite;
    public System.Windows.Forms.CheckBox Checkbox;
    private System.Windows.Forms.LinkLabel LinkBlack;
    private System.Windows.Forms.LinkLabel LinkGray;
  }
}
