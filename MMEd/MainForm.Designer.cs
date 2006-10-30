namespace MMEd
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.MnuFile = new System.Windows.Forms.ToolStripMenuItem();
            this.MnuiFileOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.MainSplitter = new System.Windows.Forms.SplitContainer();
            this.ChunkTreeView = new System.Windows.Forms.TreeView();
            this.ViewerTabControl = new System.Windows.Forms.TabControl();
            this.ViewTabSummary = new System.Windows.Forms.TabPage();
            this.label1 = new System.Windows.Forms.Label();
            this.ViewTabXML = new System.Windows.Forms.TabPage();
            this.XMLViewerCommitBtn = new System.Windows.Forms.Button();
            this.XMLTextBox = new System.Windows.Forms.TextBox();
            this.ViewTabImg = new System.Windows.Forms.TabPage();
            this.ImgPictureBox = new System.Windows.Forms.PictureBox();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.MainSplitter.Panel1.SuspendLayout();
            this.MainSplitter.Panel2.SuspendLayout();
            this.MainSplitter.SuspendLayout();
            this.ViewerTabControl.SuspendLayout();
            this.ViewTabSummary.SuspendLayout();
            this.ViewTabXML.SuspendLayout();
            this.ViewTabImg.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ImgPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MnuFile});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(801, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // MnuFile
            // 
            this.MnuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MnuiFileOpen,
            this.saveToolStripMenuItem});
            this.MnuFile.Name = "MnuFile";
            this.MnuFile.Size = new System.Drawing.Size(35, 20);
            this.MnuFile.Text = "File";
            // 
            // MnuiFileOpen
            // 
            this.MnuiFileOpen.Name = "MnuiFileOpen";
            this.MnuiFileOpen.Size = new System.Drawing.Size(123, 22);
            this.MnuiFileOpen.Text = "Open...";
            this.MnuiFileOpen.Click += new System.EventHandler(this.MnuiFileOpen_Click);
            // 
            // MainSplitter
            // 
            this.MainSplitter.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.MainSplitter.Location = new System.Drawing.Point(0, 27);
            this.MainSplitter.Name = "MainSplitter";
            // 
            // MainSplitter.Panel1
            // 
            this.MainSplitter.Panel1.Controls.Add(this.ChunkTreeView);
            // 
            // MainSplitter.Panel2
            // 
            this.MainSplitter.Panel2.Controls.Add(this.ViewerTabControl);
            this.MainSplitter.Size = new System.Drawing.Size(801, 567);
            this.MainSplitter.SplitterDistance = 267;
            this.MainSplitter.TabIndex = 2;
            // 
            // ChunkTreeView
            // 
            this.ChunkTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ChunkTreeView.Location = new System.Drawing.Point(0, 0);
            this.ChunkTreeView.Name = "ChunkTreeView";
            this.ChunkTreeView.Size = new System.Drawing.Size(267, 567);
            this.ChunkTreeView.TabIndex = 6;
            this.ChunkTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.ChunkTreeView_AfterSelect);
            // 
            // ViewerTabControl
            // 
            this.ViewerTabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.ViewerTabControl.Controls.Add(this.ViewTabSummary);
            this.ViewerTabControl.Controls.Add(this.ViewTabXML);
            this.ViewerTabControl.Controls.Add(this.ViewTabImg);
            this.ViewerTabControl.Location = new System.Drawing.Point(3, 3);
            this.ViewerTabControl.Name = "ViewerTabControl";
            this.ViewerTabControl.SelectedIndex = 0;
            this.ViewerTabControl.Size = new System.Drawing.Size(527, 564);
            this.ViewerTabControl.TabIndex = 0;
            this.ViewerTabControl.Selected += new System.Windows.Forms.TabControlEventHandler(this.ViewerTabControl_Selected);
            // 
            // ViewTabSummary
            // 
            this.ViewTabSummary.Controls.Add(this.label1);
            this.ViewTabSummary.Location = new System.Drawing.Point(4, 22);
            this.ViewTabSummary.Name = "ViewTabSummary";
            this.ViewTabSummary.Padding = new System.Windows.Forms.Padding(3);
            this.ViewTabSummary.Size = new System.Drawing.Size(519, 538);
            this.ViewTabSummary.TabIndex = 0;
            this.ViewTabSummary.Text = "Summary";
            this.ViewTabSummary.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(175, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Nothing here yet... (hint: open a file)";
            // 
            // ViewTabXML
            // 
            this.ViewTabXML.Controls.Add(this.XMLViewerCommitBtn);
            this.ViewTabXML.Controls.Add(this.XMLTextBox);
            this.ViewTabXML.Location = new System.Drawing.Point(4, 22);
            this.ViewTabXML.Name = "ViewTabXML";
            this.ViewTabXML.Padding = new System.Windows.Forms.Padding(3);
            this.ViewTabXML.Size = new System.Drawing.Size(519, 538);
            this.ViewTabXML.TabIndex = 1;
            this.ViewTabXML.Text = "XML";
            this.ViewTabXML.UseVisualStyleBackColor = true;
            // 
            // XMLViewerCommitBtn
            // 
            this.XMLViewerCommitBtn.Location = new System.Drawing.Point(441, 504);
            this.XMLViewerCommitBtn.Name = "XMLViewerCommitBtn";
            this.XMLViewerCommitBtn.Size = new System.Drawing.Size(75, 31);
            this.XMLViewerCommitBtn.TabIndex = 1;
            this.XMLViewerCommitBtn.Text = "Commit";
            this.XMLViewerCommitBtn.UseVisualStyleBackColor = true;
            // 
            // XMLTextBox
            // 
            this.XMLTextBox.AcceptsReturn = true;
            this.XMLTextBox.AcceptsTab = true;
            this.XMLTextBox.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.XMLTextBox.Location = new System.Drawing.Point(0, 0);
            this.XMLTextBox.MaxLength = 0;
            this.XMLTextBox.Multiline = true;
            this.XMLTextBox.Name = "XMLTextBox";
            this.XMLTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.XMLTextBox.Size = new System.Drawing.Size(520, 498);
            this.XMLTextBox.TabIndex = 0;
            // 
            // ViewTabImg
            // 
            this.ViewTabImg.Controls.Add(this.ImgPictureBox);
            this.ViewTabImg.Location = new System.Drawing.Point(4, 22);
            this.ViewTabImg.Name = "ViewTabImg";
            this.ViewTabImg.Padding = new System.Windows.Forms.Padding(3);
            this.ViewTabImg.Size = new System.Drawing.Size(519, 538);
            this.ViewTabImg.TabIndex = 2;
            this.ViewTabImg.Text = "Img";
            this.ViewTabImg.UseVisualStyleBackColor = true;
            // 
            // ImgPictureBox
            // 
            this.ImgPictureBox.BackColor = System.Drawing.Color.White;
            this.ImgPictureBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.ImgPictureBox.Image = ((System.Drawing.Image)(resources.GetObject("ImgPictureBox.Image")));
            this.ImgPictureBox.Location = new System.Drawing.Point(0, 0);
            this.ImgPictureBox.Name = "ImgPictureBox";
            this.ImgPictureBox.Size = new System.Drawing.Size(304, 254);
            this.ImgPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.ImgPictureBox.TabIndex = 0;
            this.ImgPictureBox.TabStop = false;
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.saveToolStripMenuItem.Text = "Save As...";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(801, 593);
            this.Controls.Add(this.MainSplitter);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.Text = "MMed";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.MainSplitter.Panel1.ResumeLayout(false);
            this.MainSplitter.Panel2.ResumeLayout(false);
            this.MainSplitter.ResumeLayout(false);
            this.ViewerTabControl.ResumeLayout(false);
            this.ViewTabSummary.ResumeLayout(false);
            this.ViewTabSummary.PerformLayout();
            this.ViewTabXML.ResumeLayout(false);
            this.ViewTabXML.PerformLayout();
            this.ViewTabImg.ResumeLayout(false);
            this.ViewTabImg.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ImgPictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem MnuFile;
        private System.Windows.Forms.ToolStripMenuItem MnuiFileOpen;
        private System.Windows.Forms.SplitContainer MainSplitter;
        private System.Windows.Forms.TabControl ViewerTabControl;
        private System.Windows.Forms.TabPage ViewTabSummary;
        public System.Windows.Forms.TabPage ViewTabXML;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.TextBox XMLTextBox;
        public System.Windows.Forms.PictureBox ImgPictureBox;
        public System.Windows.Forms.TabPage ViewTabImg;
        public System.Windows.Forms.Button XMLViewerCommitBtn;
        public System.Windows.Forms.TreeView ChunkTreeView;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
    }
}

