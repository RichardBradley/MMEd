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
            this.mMenuStrip = new System.Windows.Forms.MenuStrip();
            this.MnuFile = new System.Windows.Forms.ToolStripMenuItem();
            this.MnuiFileOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
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
            this.ViewTabGrid = new System.Windows.Forms.TabPage();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.WorldCoordLabel = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.FlatCoordLabel = new System.Windows.Forms.Label();
            this.TexSquareLabel = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.GridDisplayPanelHolder = new MMEd.Util.SmoothScrollingPanel();
            this.GridDisplayPanel = new MMEd.Util.UserPaintDoubleBufferedPanel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.GridViewMetaTypeCombo = new System.Windows.Forms.ComboBox();
            this.GridViewRadioImgNum = new System.Windows.Forms.RadioButton();
            this.GridViewRadioImages = new System.Windows.Forms.RadioButton();
            this.ViewTab3D = new System.Windows.Forms.TabPage();
            this.Viewer3DRenderingSurface = new GLTK.RenderingSurface();
            this.ViewTabBump = new System.Windows.Forms.TabPage();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.BumpTypeLabel = new System.Windows.Forms.Label();
            this.BumpViewPictureBox = new System.Windows.Forms.PictureBox();
            this.BumpCombo = new System.Windows.Forms.ComboBox();
            this.BumpEditPictureBox = new System.Windows.Forms.PictureBox();
            this.ViewTabVRAM = new System.Windows.Forms.TabPage();
            this.panel1 = new System.Windows.Forms.Panel();
            this.VRAMPictureBox = new System.Windows.Forms.PictureBox();
            this.VRAMStatusStrip = new System.Windows.Forms.StatusStrip();
            this.mMenuStrip.SuspendLayout();
            this.MainSplitter.Panel1.SuspendLayout();
            this.MainSplitter.Panel2.SuspendLayout();
            this.MainSplitter.SuspendLayout();
            this.ViewerTabControl.SuspendLayout();
            this.ViewTabSummary.SuspendLayout();
            this.ViewTabXML.SuspendLayout();
            this.ViewTabImg.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ImgPictureBox)).BeginInit();
            this.ViewTabGrid.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.GridDisplayPanelHolder.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.ViewTab3D.SuspendLayout();
            this.ViewTabBump.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.BumpViewPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.BumpEditPictureBox)).BeginInit();
            this.ViewTabVRAM.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.VRAMPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // mMenuStrip
            // 
            this.mMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MnuFile});
            this.mMenuStrip.Location = new System.Drawing.Point(0, 0);
            this.mMenuStrip.Name = "mMenuStrip";
            this.mMenuStrip.Size = new System.Drawing.Size(801, 24);
            this.mMenuStrip.TabIndex = 0;
            this.mMenuStrip.Text = "menuStrip1";
            // 
            // MnuFile
            // 
            this.MnuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MnuiFileOpen,
            this.saveToolStripMenuItem});
            this.MnuFile.Name = "MnuFile";
            this.MnuFile.Size = new System.Drawing.Size(35, 20);
            this.MnuFile.Text = "&File";
            // 
            // MnuiFileOpen
            // 
            this.MnuiFileOpen.Image = ((System.Drawing.Image)(resources.GetObject("MnuiFileOpen.Image")));
            this.MnuiFileOpen.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.MnuiFileOpen.Name = "MnuiFileOpen";
            this.MnuiFileOpen.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.MnuiFileOpen.Size = new System.Drawing.Size(174, 22);
            this.MnuiFileOpen.Text = "&Open";
            this.MnuiFileOpen.Click += new System.EventHandler(this.MnuiFileOpen_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("saveToolStripMenuItem.Image")));
            this.saveToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(174, 22);
            this.saveToolStripMenuItem.Text = "&Save As...";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
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
            this.MainSplitter.SplitterDistance = 150;
            this.MainSplitter.TabIndex = 2;
            // 
            // ChunkTreeView
            // 
            this.ChunkTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ChunkTreeView.Location = new System.Drawing.Point(0, 0);
            this.ChunkTreeView.Name = "ChunkTreeView";
            this.ChunkTreeView.Size = new System.Drawing.Size(150, 567);
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
            this.ViewerTabControl.Controls.Add(this.ViewTabGrid);
            this.ViewerTabControl.Controls.Add(this.ViewTab3D);
            this.ViewerTabControl.Controls.Add(this.ViewTabBump);
            this.ViewerTabControl.Controls.Add(this.ViewTabVRAM);
            this.ViewerTabControl.Location = new System.Drawing.Point(3, 3);
            this.ViewerTabControl.Name = "ViewerTabControl";
            this.ViewerTabControl.SelectedIndex = 0;
            this.ViewerTabControl.Size = new System.Drawing.Size(644, 561);
            this.ViewerTabControl.TabIndex = 0;
            this.ViewerTabControl.Selected += new System.Windows.Forms.TabControlEventHandler(this.ViewerTabControl_Selected);
            this.ViewerTabControl.SelectedIndexChanged += new System.EventHandler(this.ViewerTabControl_SelectedIndexChanged);
            // 
            // ViewTabSummary
            // 
            this.ViewTabSummary.Controls.Add(this.label1);
            this.ViewTabSummary.Location = new System.Drawing.Point(4, 22);
            this.ViewTabSummary.Name = "ViewTabSummary";
            this.ViewTabSummary.Padding = new System.Windows.Forms.Padding(3);
            this.ViewTabSummary.Size = new System.Drawing.Size(636, 535);
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
            this.ViewTabXML.Size = new System.Drawing.Size(636, 535);
            this.ViewTabXML.TabIndex = 1;
            this.ViewTabXML.Text = "XML";
            this.ViewTabXML.UseVisualStyleBackColor = true;
            // 
            // XMLViewerCommitBtn
            // 
            this.XMLViewerCommitBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.XMLViewerCommitBtn.Location = new System.Drawing.Point(558, 477);
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
            this.XMLTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.XMLTextBox.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.XMLTextBox.Location = new System.Drawing.Point(0, 0);
            this.XMLTextBox.MaxLength = 0;
            this.XMLTextBox.Multiline = true;
            this.XMLTextBox.Name = "XMLTextBox";
            this.XMLTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.XMLTextBox.Size = new System.Drawing.Size(637, 471);
            this.XMLTextBox.TabIndex = 0;
            // 
            // ViewTabImg
            // 
            this.ViewTabImg.Controls.Add(this.ImgPictureBox);
            this.ViewTabImg.Location = new System.Drawing.Point(4, 22);
            this.ViewTabImg.Name = "ViewTabImg";
            this.ViewTabImg.Padding = new System.Windows.Forms.Padding(3);
            this.ViewTabImg.Size = new System.Drawing.Size(636, 535);
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
            // ViewTabGrid
            // 
            this.ViewTabGrid.Controls.Add(this.groupBox2);
            this.ViewTabGrid.Controls.Add(this.GridDisplayPanelHolder);
            this.ViewTabGrid.Controls.Add(this.groupBox1);
            this.ViewTabGrid.Location = new System.Drawing.Point(4, 22);
            this.ViewTabGrid.Name = "ViewTabGrid";
            this.ViewTabGrid.Padding = new System.Windows.Forms.Padding(3);
            this.ViewTabGrid.Size = new System.Drawing.Size(636, 535);
            this.ViewTabGrid.TabIndex = 3;
            this.ViewTabGrid.Text = "Grid";
            this.ViewTabGrid.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.WorldCoordLabel);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.FlatCoordLabel);
            this.groupBox2.Controls.Add(this.TexSquareLabel);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Location = new System.Drawing.Point(0, 415);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(636, 100);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Data";
            // 
            // WorldCoordLabel
            // 
            this.WorldCoordLabel.AutoSize = true;
            this.WorldCoordLabel.Location = new System.Drawing.Point(98, 65);
            this.WorldCoordLabel.Name = "WorldCoordLabel";
            this.WorldCoordLabel.Size = new System.Drawing.Size(29, 13);
            this.WorldCoordLabel.TabIndex = 5;
            this.WorldCoordLabel.Text = "TBD";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(7, 65);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(96, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "World coordinates:";
            // 
            // FlatCoordLabel
            // 
            this.FlatCoordLabel.AutoSize = true;
            this.FlatCoordLabel.Location = new System.Drawing.Point(98, 42);
            this.FlatCoordLabel.Name = "FlatCoordLabel";
            this.FlatCoordLabel.Size = new System.Drawing.Size(29, 13);
            this.FlatCoordLabel.TabIndex = 3;
            this.FlatCoordLabel.Text = "TBD";
            // 
            // TexSquareLabel
            // 
            this.TexSquareLabel.AutoSize = true;
            this.TexSquareLabel.Location = new System.Drawing.Point(98, 20);
            this.TexSquareLabel.Name = "TexSquareLabel";
            this.TexSquareLabel.Size = new System.Drawing.Size(29, 13);
            this.TexSquareLabel.TabIndex = 2;
            this.TexSquareLabel.Text = "TBD";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 42);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(85, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "Flat coordinates:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 20);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(63, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Tex square:";
            // 
            // GridDisplayPanelHolder
            // 
            this.GridDisplayPanelHolder.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.GridDisplayPanelHolder.AutoScroll = true;
            this.GridDisplayPanelHolder.Controls.Add(this.GridDisplayPanel);
            this.GridDisplayPanelHolder.Location = new System.Drawing.Point(6, 6);
            this.GridDisplayPanelHolder.Name = "GridDisplayPanelHolder";
            this.GridDisplayPanelHolder.Size = new System.Drawing.Size(492, 364);
            this.GridDisplayPanelHolder.TabIndex = 1;
            // 
            // GridDisplayPanel
            // 
            this.GridDisplayPanel.Location = new System.Drawing.Point(0, 0);
            this.GridDisplayPanel.Name = "GridDisplayPanel";
            this.GridDisplayPanel.Size = new System.Drawing.Size(200, 100);
            this.GridDisplayPanel.TabIndex = 1;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.GridViewMetaTypeCombo);
            this.groupBox1.Controls.Add(this.GridViewRadioImgNum);
            this.groupBox1.Controls.Add(this.GridViewRadioImages);
            this.groupBox1.Location = new System.Drawing.Point(504, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(132, 220);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "ViewMode";
            // 
            // GridViewMetaTypeCombo
            // 
            this.GridViewMetaTypeCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.GridViewMetaTypeCombo.FormattingEnabled = true;
            this.GridViewMetaTypeCombo.Location = new System.Drawing.Point(6, 65);
            this.GridViewMetaTypeCombo.Name = "GridViewMetaTypeCombo";
            this.GridViewMetaTypeCombo.Size = new System.Drawing.Size(118, 21);
            this.GridViewMetaTypeCombo.TabIndex = 2;
            // 
            // GridViewRadioImgNum
            // 
            this.GridViewRadioImgNum.AutoSize = true;
            this.GridViewRadioImgNum.Location = new System.Drawing.Point(6, 42);
            this.GridViewRadioImgNum.Name = "GridViewRadioImgNum";
            this.GridViewRadioImgNum.Size = new System.Drawing.Size(109, 17);
            this.GridViewRadioImgNum.TabIndex = 1;
            this.GridViewRadioImgNum.Text = "Images + number:";
            this.GridViewRadioImgNum.UseVisualStyleBackColor = true;
            // 
            // GridViewRadioImages
            // 
            this.GridViewRadioImages.AutoSize = true;
            this.GridViewRadioImages.Checked = true;
            this.GridViewRadioImages.Location = new System.Drawing.Point(6, 19);
            this.GridViewRadioImages.Name = "GridViewRadioImages";
            this.GridViewRadioImages.Size = new System.Drawing.Size(81, 17);
            this.GridViewRadioImages.TabIndex = 0;
            this.GridViewRadioImages.TabStop = true;
            this.GridViewRadioImages.Text = "Images only";
            this.GridViewRadioImages.UseVisualStyleBackColor = true;
            // 
            // ViewTab3D
            // 
            this.ViewTab3D.Controls.Add(this.Viewer3DRenderingSurface);
            this.ViewTab3D.Location = new System.Drawing.Point(4, 22);
            this.ViewTab3D.Name = "ViewTab3D";
            this.ViewTab3D.Padding = new System.Windows.Forms.Padding(3);
            this.ViewTab3D.Size = new System.Drawing.Size(636, 535);
            this.ViewTab3D.TabIndex = 4;
            this.ViewTab3D.Text = "3D";
            this.ViewTab3D.UseVisualStyleBackColor = true;
            // 
            // Viewer3DRenderingSurface
            // 
            this.Viewer3DRenderingSurface.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.Viewer3DRenderingSurface.Location = new System.Drawing.Point(-4, 0);
            this.Viewer3DRenderingSurface.Name = "Viewer3DRenderingSurface";
            this.Viewer3DRenderingSurface.Size = new System.Drawing.Size(640, 512);
            this.Viewer3DRenderingSurface.TabIndex = 0;
            this.Viewer3DRenderingSurface.Text = "renderingSurface1";
            // 
            // ViewTabBump
            // 
            this.ViewTabBump.Controls.Add(this.label8);
            this.ViewTabBump.Controls.Add(this.label7);
            this.ViewTabBump.Controls.Add(this.label6);
            this.ViewTabBump.Controls.Add(this.label5);
            this.ViewTabBump.Controls.Add(this.BumpTypeLabel);
            this.ViewTabBump.Controls.Add(this.BumpViewPictureBox);
            this.ViewTabBump.Controls.Add(this.BumpCombo);
            this.ViewTabBump.Controls.Add(this.BumpEditPictureBox);
            this.ViewTabBump.Location = new System.Drawing.Point(4, 22);
            this.ViewTabBump.Name = "ViewTabBump";
            this.ViewTabBump.Padding = new System.Windows.Forms.Padding(3);
            this.ViewTabBump.Size = new System.Drawing.Size(636, 535);
            this.ViewTabBump.TabIndex = 5;
            this.ViewTabBump.Text = "Bump";
            this.ViewTabBump.UseVisualStyleBackColor = true;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(134, 284);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(165, 13);
            this.label8.TabIndex = 7;
            this.label8.Text = "Click a pixel to view its bump type";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(131, 80);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(325, 13);
            this.label7.TabIndex = 6;
            this.label7.Text = "Select a bump type then click the pixels you want to set to that type";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(3, 237);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(148, 20);
            this.label6.TabIndex = 5;
            this.label6.Text = "View (Does work)";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(3, 15);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(191, 20);
            this.label5.TabIndex = 4;
            this.label5.Text = "Edit (Doesn\'t work yet)";
            // 
            // BumpTypeLabel
            // 
            this.BumpTypeLabel.AutoSize = true;
            this.BumpTypeLabel.Location = new System.Drawing.Point(134, 260);
            this.BumpTypeLabel.Name = "BumpTypeLabel";
            this.BumpTypeLabel.Size = new System.Drawing.Size(84, 13);
            this.BumpTypeLabel.TabIndex = 3;
            this.BumpTypeLabel.Text = "BumpTypeLabel";
            // 
            // BumpViewPictureBox
            // 
            this.BumpViewPictureBox.BackColor = System.Drawing.Color.White;
            this.BumpViewPictureBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.BumpViewPictureBox.InitialImage = null;
            this.BumpViewPictureBox.Location = new System.Drawing.Point(0, 260);
            this.BumpViewPictureBox.Name = "BumpViewPictureBox";
            this.BumpViewPictureBox.Size = new System.Drawing.Size(128, 128);
            this.BumpViewPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.BumpViewPictureBox.TabIndex = 2;
            this.BumpViewPictureBox.TabStop = false;
            // 
            // BumpCombo
            // 
            this.BumpCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.BumpCombo.FormattingEnabled = true;
            this.BumpCombo.Location = new System.Drawing.Point(134, 46);
            this.BumpCombo.Name = "BumpCombo";
            this.BumpCombo.Size = new System.Drawing.Size(121, 21);
            this.BumpCombo.TabIndex = 1;
            // 
            // BumpEditPictureBox
            // 
            this.BumpEditPictureBox.BackColor = System.Drawing.Color.White;
            this.BumpEditPictureBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.BumpEditPictureBox.InitialImage = null;
            this.BumpEditPictureBox.Location = new System.Drawing.Point(0, 43);
            this.BumpEditPictureBox.Name = "BumpEditPictureBox";
            this.BumpEditPictureBox.Size = new System.Drawing.Size(128, 128);
            this.BumpEditPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.BumpEditPictureBox.TabIndex = 0;
            this.BumpEditPictureBox.TabStop = false;
            // 
            // ViewTabVRAM
            // 
            this.ViewTabVRAM.Controls.Add(this.panel1);
            this.ViewTabVRAM.Controls.Add(this.VRAMStatusStrip);
            this.ViewTabVRAM.Location = new System.Drawing.Point(4, 22);
            this.ViewTabVRAM.Name = "ViewTabVRAM";
            this.ViewTabVRAM.Size = new System.Drawing.Size(636, 535);
            this.ViewTabVRAM.TabIndex = 6;
            this.ViewTabVRAM.Text = "VRAM";
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.AutoScroll = true;
            this.panel1.Controls.Add(this.VRAMPictureBox);
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(637, 510);
            this.panel1.TabIndex = 2;
            // 
            // VRAMPictureBox
            // 
            this.VRAMPictureBox.BackColor = System.Drawing.Color.White;
            this.VRAMPictureBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.VRAMPictureBox.Location = new System.Drawing.Point(0, 0);
            this.VRAMPictureBox.Name = "VRAMPictureBox";
            this.VRAMPictureBox.Size = new System.Drawing.Size(304, 254);
            this.VRAMPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.VRAMPictureBox.TabIndex = 1;
            this.VRAMPictureBox.TabStop = false;
            // 
            // VRAMStatusStrip
            // 
            this.VRAMStatusStrip.Location = new System.Drawing.Point(0, 513);
            this.VRAMStatusStrip.Name = "VRAMStatusStrip";
            this.VRAMStatusStrip.Size = new System.Drawing.Size(636, 22);
            this.VRAMStatusStrip.TabIndex = 1;
            this.VRAMStatusStrip.Text = "statusStrip1";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(801, 593);
            this.Controls.Add(this.MainSplitter);
            this.Controls.Add(this.mMenuStrip);
            this.Name = "MainForm";
            this.Text = "MMed";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.mMenuStrip.ResumeLayout(false);
            this.mMenuStrip.PerformLayout();
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
            this.ViewTabGrid.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.GridDisplayPanelHolder.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ViewTab3D.ResumeLayout(false);
            this.ViewTabBump.ResumeLayout(false);
            this.ViewTabBump.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.BumpViewPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.BumpEditPictureBox)).EndInit();
            this.ViewTabVRAM.ResumeLayout(false);
            this.ViewTabVRAM.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.VRAMPictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStripMenuItem MnuFile;
      private System.Windows.Forms.SplitContainer MainSplitter;
      public System.Windows.Forms.TreeView ChunkTreeView;
        private System.Windows.Forms.ToolStripMenuItem MnuiFileOpen;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
      public System.Windows.Forms.MenuStrip mMenuStrip;
      public System.Windows.Forms.TabControl ViewerTabControl;
      private System.Windows.Forms.TabPage ViewTabSummary;
      private System.Windows.Forms.Label label1;
      public System.Windows.Forms.TabPage ViewTabXML;
      public System.Windows.Forms.Button XMLViewerCommitBtn;
      public System.Windows.Forms.TextBox XMLTextBox;
      public System.Windows.Forms.TabPage ViewTabImg;
      public System.Windows.Forms.PictureBox ImgPictureBox;
      public System.Windows.Forms.TabPage ViewTabGrid;
      private System.Windows.Forms.GroupBox groupBox2;
      public System.Windows.Forms.Label WorldCoordLabel;
      private System.Windows.Forms.Label label4;
      public System.Windows.Forms.Label FlatCoordLabel;
      public System.Windows.Forms.Label TexSquareLabel;
      private System.Windows.Forms.Label label3;
      private System.Windows.Forms.Label label2;
      public System.Windows.Forms.Panel GridDisplayPanelHolder;
      public System.Windows.Forms.Panel GridDisplayPanel;
      private System.Windows.Forms.GroupBox groupBox1;
      public System.Windows.Forms.ComboBox GridViewMetaTypeCombo;
      public System.Windows.Forms.RadioButton GridViewRadioImgNum;
      public System.Windows.Forms.RadioButton GridViewRadioImages;
      public System.Windows.Forms.TabPage ViewTab3D;
      public GLTK.RenderingSurface Viewer3DRenderingSurface;
      public System.Windows.Forms.TabPage ViewTabBump;
      public System.Windows.Forms.PictureBox BumpEditPictureBox;
      public System.Windows.Forms.TabPage ViewTabVRAM;
      private System.Windows.Forms.Panel panel1;
      public System.Windows.Forms.PictureBox VRAMPictureBox;
      public System.Windows.Forms.StatusStrip VRAMStatusStrip;
      public System.Windows.Forms.ComboBox BumpCombo;
      public System.Windows.Forms.PictureBox BumpViewPictureBox;
      public System.Windows.Forms.Label BumpTypeLabel;
      private System.Windows.Forms.Label label5;
      private System.Windows.Forms.Label label6;
      private System.Windows.Forms.Label label8;
      private System.Windows.Forms.Label label7;
    }
}

