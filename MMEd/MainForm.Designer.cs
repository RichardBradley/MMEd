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
          this.components = new System.ComponentModel.Container();
          System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
          this.mMenuStrip = new System.Windows.Forms.MenuStrip();
          this.MnuFile = new System.Windows.Forms.ToolStripMenuItem();
          this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
          this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
          this.MnuiFileOpenLevel = new System.Windows.Forms.ToolStripMenuItem();
          this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
          this.saveToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
          this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
          this.publishToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
          this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
          this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
          this.MainSplitter = new System.Windows.Forms.SplitContainer();
          this.ChunkTreeView = new System.Windows.Forms.TreeView();
          this.ViewerTabControl = new System.Windows.Forms.TabControl();
          this.ViewTabActions = new System.Windows.Forms.TabPage();
          this.groupBox6 = new System.Windows.Forms.GroupBox();
          this.label20 = new System.Windows.Forms.Label();
          this.label19 = new System.Windows.Forms.Label();
          this.ActionsTabImportTMDButton = new System.Windows.Forms.Button();
          this.ActionsTabExportTMDButton = new System.Windows.Forms.Button();
          this.groupBox5 = new System.Windows.Forms.GroupBox();
          this.label9 = new System.Windows.Forms.Label();
          this.ActionsTabTIMTextureUsageButton = new System.Windows.Forms.Button();
          this.ActionsTabImportTIMButton = new System.Windows.Forms.Button();
          this.label11 = new System.Windows.Forms.Label();
          this.ActionsTabExportTIMButton = new System.Windows.Forms.Button();
          this.label10 = new System.Windows.Forms.Label();
          this.groupBox3 = new System.Windows.Forms.GroupBox();
          this.groupBox7 = new System.Windows.Forms.GroupBox();
          this.label25 = new System.Windows.Forms.Label();
          this.OptimiseCameraCompactCheckbox = new System.Windows.Forms.CheckBox();
          this.OptimiseCameraReindexCheckbox = new System.Windows.Forms.CheckBox();
          this.label1 = new System.Windows.Forms.Label();
          this.OptimiseSteeringCompactCheckbox = new System.Windows.Forms.CheckBox();
          this.OptimiseSteeringReindexCheckbox = new System.Windows.Forms.CheckBox();
          this.label24 = new System.Windows.Forms.Label();
          this.OptimiseBumpCompactCheckbox = new System.Windows.Forms.CheckBox();
          this.OptimiseBumpReindexCheckbox = new System.Windows.Forms.CheckBox();
          this.label23 = new System.Windows.Forms.Label();
          this.ActionsTabOptimiseButton = new System.Windows.Forms.Button();
          this.ActionsTabImportFlatImagesButton = new System.Windows.Forms.Button();
          this.label22 = new System.Windows.Forms.Label();
          this.ActionsTabExportFlatImagesButton = new System.Windows.Forms.Button();
          this.label21 = new System.Windows.Forms.Label();
          this.ViewTabXML = new System.Windows.Forms.TabPage();
          this.XMLViewerCommitBtn = new System.Windows.Forms.Button();
          this.XMLTextBox = new System.Windows.Forms.TextBox();
          this.ViewTabFlat = new System.Windows.Forms.TabPage();
          this.FlatViewerDeleteButton = new System.Windows.Forms.Button();
          this.FlatViewerCloneButton = new System.Windows.Forms.Button();
          this.FlatViewerCommitBtn = new System.Windows.Forms.Button();
          this.FlatPanel = new MMEd.Util.FlatEditorPanel();
          this.ViewTabImg = new System.Windows.Forms.TabPage();
          this.ImgPictureBox = new System.Windows.Forms.PictureBox();
          this.ViewTabGrid = new System.Windows.Forms.TabPage();
          this.GridViewSelImageGroupBox = new System.Windows.Forms.GroupBox();
          this.GridViewSelPanelHolder = new MMEd.Util.SmoothScrollingPanel();
          this.GridViewSelPanel = new System.Windows.Forms.Panel();
          this.statusStrip1 = new System.Windows.Forms.StatusStrip();
          this.GridViewerStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
          this.groupBox2 = new System.Windows.Forms.GroupBox();
          this.smoothScrollingPanel1 = new MMEd.Util.SmoothScrollingPanel();
          this.GridViewPalettePanel = new System.Windows.Forms.Panel();
          this.WorldCoordLabel = new System.Windows.Forms.Label();
          this.label4 = new System.Windows.Forms.Label();
          this.FlatCoordLabel = new System.Windows.Forms.Label();
          this.TexSquareLabel = new System.Windows.Forms.Label();
          this.label3 = new System.Windows.Forms.Label();
          this.label2 = new System.Windows.Forms.Label();
          this.groupBox1 = new System.Windows.Forms.GroupBox();
          this.OverlaySelectorBehaviour = new MMEd.Util.OverlaySelector();
          this.OverlaySelectorSteering = new MMEd.Util.OverlaySelector();
          this.GridViewShowWaypointsCheck = new System.Windows.Forms.CheckBox();
          this.OverlaySelectorRespawn = new MMEd.Util.OverlaySelector();
          this.OverlaySelectorCamera = new MMEd.Util.OverlaySelector();
          this.OverlaySelectorGrid = new MMEd.Util.OverlaySelector();
          this.GridViewShowObjectsCheck = new System.Windows.Forms.CheckBox();
          this.GridViewZoomSlider = new MMEd.Util.LabelledSlider();
          this.GridViewTransparencySlider = new MMEd.Util.LabelledSlider();
          this.GridViewViewModeCombo = new System.Windows.Forms.ComboBox();
          this.GridViewMetaTypeCombo = new System.Windows.Forms.ComboBox();
          this.GridDisplayPanelHolder = new MMEd.Util.SmoothScrollingPanel();
          this.GridDisplayPanel = new MMEd.Util.UserPaintDoubleBufferedPanel();
          this.ViewTab3D = new System.Windows.Forms.TabPage();
          this.Viewer3DRenderingSurface = new GLTK.RenderingSurface();
          this.ViewTabBump = new System.Windows.Forms.TabPage();
          this.label18 = new System.Windows.Forms.Label();
          this.label17 = new System.Windows.Forms.Label();
          this.label16 = new System.Windows.Forms.Label();
          this.label15 = new System.Windows.Forms.Label();
          this.BumpEditFillButton = new System.Windows.Forms.Button();
          this.BumpTypeLabel = new System.Windows.Forms.Label();
          this.BumpViewPictureBox = new System.Windows.Forms.PictureBox();
          this.BumpCombo = new System.Windows.Forms.ComboBox();
          this.BumpEditPictureBox = new System.Windows.Forms.PictureBox();
          this.ViewTabSteering = new System.Windows.Forms.TabPage();
          this.SteeringEditFillButton = new System.Windows.Forms.Button();
          this.label8 = new System.Windows.Forms.Label();
          this.label7 = new System.Windows.Forms.Label();
          this.label6 = new System.Windows.Forms.Label();
          this.label5 = new System.Windows.Forms.Label();
          this.SteeringTypeLabel = new System.Windows.Forms.Label();
          this.SteeringViewPictureBox = new System.Windows.Forms.PictureBox();
          this.SteeringCombo = new System.Windows.Forms.ComboBox();
          this.SteeringEditPictureBox = new System.Windows.Forms.PictureBox();
          this.ViewTabCamera = new System.Windows.Forms.TabPage();
          this.CameraRenderingSurface = new GLTK.RenderingSurface();
          this.PanelCameraImage = new System.Windows.Forms.Panel();
          this.SliderElevation = new System.Windows.Forms.TrackBar();
          this.TextElevation = new System.Windows.Forms.TextBox();
          this.label14 = new System.Windows.Forms.Label();
          this.SliderDistance = new System.Windows.Forms.TrackBar();
          this.TextDistance = new System.Windows.Forms.TextBox();
          this.label13 = new System.Windows.Forms.Label();
          this.SliderDirection = new System.Windows.Forms.TrackBar();
          this.TextDirection = new System.Windows.Forms.TextBox();
          this.label12 = new System.Windows.Forms.Label();
          this.ViewTabVRAM = new System.Windows.Forms.TabPage();
          this.VRAMScrollPanel = new MMEd.Util.SmoothScrollingPanel();
          this.VRAMPictureBox = new System.Windows.Forms.PictureBox();
          this.VRAMStatusStrip = new System.Windows.Forms.StatusStrip();
          this.ViewTab3dEditor = new System.Windows.Forms.TabPage();
          this.panel2 = new System.Windows.Forms.Panel();
          this.ThreeDeeEditorStatusLabel = new System.Windows.Forms.Label();
          this.LeftRightSplit = new System.Windows.Forms.SplitContainer();
          this.LeftHandSplit = new System.Windows.Forms.SplitContainer();
          this.Viewer3DRenderingSurfaceTopLeft = new GLTK.RenderingSurface();
          this.Viewer3DRenderingSurfaceBottomLeft = new GLTK.RenderingSurface();
          this.RightHandSplit = new System.Windows.Forms.SplitContainer();
          this.Viewer3DRenderingSurfaceTopRight = new GLTK.RenderingSurface();
          this.Viewer3DRenderingSurfaceBottomRight = new GLTK.RenderingSurface();
          this.ViewTabHistory = new System.Windows.Forms.TabPage();
          this.RevertButton = new System.Windows.Forms.Button();
          this.VersionChangesTextBox = new System.Windows.Forms.TextBox();
          this.label27 = new System.Windows.Forms.Label();
          this.VersionCreatedLabel = new System.Windows.Forms.Label();
          this.label26 = new System.Windows.Forms.Label();
          this.ToolTip = new System.Windows.Forms.ToolTip(this.components);
          this.SaveFileDialog = new System.Windows.Forms.SaveFileDialog();
          this.OpenFileDialog = new System.Windows.Forms.OpenFileDialog();
          this.mMenuStrip.SuspendLayout();
          this.MainSplitter.Panel1.SuspendLayout();
          this.MainSplitter.Panel2.SuspendLayout();
          this.MainSplitter.SuspendLayout();
          this.ViewerTabControl.SuspendLayout();
          this.ViewTabActions.SuspendLayout();
          this.groupBox6.SuspendLayout();
          this.groupBox5.SuspendLayout();
          this.groupBox3.SuspendLayout();
          this.groupBox7.SuspendLayout();
          this.ViewTabXML.SuspendLayout();
          this.ViewTabFlat.SuspendLayout();
          this.ViewTabImg.SuspendLayout();
          ((System.ComponentModel.ISupportInitialize)(this.ImgPictureBox)).BeginInit();
          this.ViewTabGrid.SuspendLayout();
          this.GridViewSelImageGroupBox.SuspendLayout();
          this.GridViewSelPanelHolder.SuspendLayout();
          this.statusStrip1.SuspendLayout();
          this.groupBox2.SuspendLayout();
          this.smoothScrollingPanel1.SuspendLayout();
          this.groupBox1.SuspendLayout();
          this.GridDisplayPanelHolder.SuspendLayout();
          this.ViewTab3D.SuspendLayout();
          this.ViewTabBump.SuspendLayout();
          ((System.ComponentModel.ISupportInitialize)(this.BumpViewPictureBox)).BeginInit();
          ((System.ComponentModel.ISupportInitialize)(this.BumpEditPictureBox)).BeginInit();
          this.ViewTabSteering.SuspendLayout();
          ((System.ComponentModel.ISupportInitialize)(this.SteeringViewPictureBox)).BeginInit();
          ((System.ComponentModel.ISupportInitialize)(this.SteeringEditPictureBox)).BeginInit();
          this.ViewTabCamera.SuspendLayout();
          ((System.ComponentModel.ISupportInitialize)(this.SliderElevation)).BeginInit();
          ((System.ComponentModel.ISupportInitialize)(this.SliderDistance)).BeginInit();
          ((System.ComponentModel.ISupportInitialize)(this.SliderDirection)).BeginInit();
          this.ViewTabVRAM.SuspendLayout();
          this.VRAMScrollPanel.SuspendLayout();
          ((System.ComponentModel.ISupportInitialize)(this.VRAMPictureBox)).BeginInit();
          this.ViewTab3dEditor.SuspendLayout();
          this.panel2.SuspendLayout();
          this.LeftRightSplit.Panel1.SuspendLayout();
          this.LeftRightSplit.Panel2.SuspendLayout();
          this.LeftRightSplit.SuspendLayout();
          this.LeftHandSplit.Panel1.SuspendLayout();
          this.LeftHandSplit.Panel2.SuspendLayout();
          this.LeftHandSplit.SuspendLayout();
          this.RightHandSplit.Panel1.SuspendLayout();
          this.RightHandSplit.Panel2.SuspendLayout();
          this.RightHandSplit.SuspendLayout();
          this.ViewTabHistory.SuspendLayout();
          this.SuspendLayout();
          // 
          // mMenuStrip
          // 
          this.mMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MnuFile});
          this.mMenuStrip.Location = new System.Drawing.Point(0, 0);
          this.mMenuStrip.Name = "mMenuStrip";
          this.mMenuStrip.Size = new System.Drawing.Size(858, 24);
          this.mMenuStrip.TabIndex = 0;
          this.mMenuStrip.Text = "menuStrip1";
          // 
          // MnuFile
          // 
          this.MnuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.openToolStripMenuItem,
            this.MnuiFileOpenLevel,
            this.toolStripSeparator1,
            this.saveToolStripMenuItem1,
            this.saveAsToolStripMenuItem,
            this.publishToolStripMenuItem,
            this.toolStripSeparator2,
            this.exitToolStripMenuItem});
          this.MnuFile.Name = "MnuFile";
          this.MnuFile.Size = new System.Drawing.Size(35, 20);
          this.MnuFile.Text = "&File";
          // 
          // newToolStripMenuItem
          // 
          this.newToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("newToolStripMenuItem.Image")));
          this.newToolStripMenuItem.Name = "newToolStripMenuItem";
          this.newToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
          this.newToolStripMenuItem.Size = new System.Drawing.Size(203, 22);
          this.newToolStripMenuItem.Text = "&New...";
          this.newToolStripMenuItem.ToolTipText = "Start making a new edit to an MMs level";
          this.newToolStripMenuItem.Click += new System.EventHandler(this.newToolStripMenuItem_Click);
          // 
          // openToolStripMenuItem
          // 
          this.openToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("openToolStripMenuItem.Image")));
          this.openToolStripMenuItem.Name = "openToolStripMenuItem";
          this.openToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
          this.openToolStripMenuItem.Size = new System.Drawing.Size(203, 22);
          this.openToolStripMenuItem.Text = "&Open...";
          this.openToolStripMenuItem.ToolTipText = "Open an existing MMs level, or a level extracted from the CD";
          this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
          // 
          // MnuiFileOpenLevel
          // 
          this.MnuiFileOpenLevel.ImageTransparentColor = System.Drawing.Color.Magenta;
          this.MnuiFileOpenLevel.Name = "MnuiFileOpenLevel";
          this.MnuiFileOpenLevel.Size = new System.Drawing.Size(203, 22);
          this.MnuiFileOpenLevel.Text = "Open Unknown &Binary...";
          this.MnuiFileOpenLevel.ToolTipText = "Opens any old binary file, and searches through it for recognisable MMv3 datastru" +
              "ctures";
          this.MnuiFileOpenLevel.Click += new System.EventHandler(this.OpenUnknownBinaryClick);
          // 
          // toolStripSeparator1
          // 
          this.toolStripSeparator1.Name = "toolStripSeparator1";
          this.toolStripSeparator1.Size = new System.Drawing.Size(200, 6);
          // 
          // saveToolStripMenuItem1
          // 
          this.saveToolStripMenuItem1.Image = ((System.Drawing.Image)(resources.GetObject("saveToolStripMenuItem1.Image")));
          this.saveToolStripMenuItem1.Name = "saveToolStripMenuItem1";
          this.saveToolStripMenuItem1.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
          this.saveToolStripMenuItem1.Size = new System.Drawing.Size(203, 22);
          this.saveToolStripMenuItem1.Text = "&Save";
          this.saveToolStripMenuItem1.ToolTipText = "Save the current level";
          this.saveToolStripMenuItem1.Click += new System.EventHandler(this.saveToolStripMenuItem1_Click);
          // 
          // saveAsToolStripMenuItem
          // 
          this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
          this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(203, 22);
          this.saveAsToolStripMenuItem.Text = "Save &As...";
          this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
          // 
          // publishToolStripMenuItem
          // 
          this.publishToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("publishToolStripMenuItem.Image")));
          this.publishToolStripMenuItem.Name = "publishToolStripMenuItem";
          this.publishToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P)));
          this.publishToolStripMenuItem.Size = new System.Drawing.Size(203, 22);
          this.publishToolStripMenuItem.Text = "&Publish";
          this.publishToolStripMenuItem.ToolTipText = "Export the current level to binary format, and optionally update your CD image";
          this.publishToolStripMenuItem.Click += new System.EventHandler(this.publishToolStripMenuItem_Click);
          // 
          // toolStripSeparator2
          // 
          this.toolStripSeparator2.Name = "toolStripSeparator2";
          this.toolStripSeparator2.Size = new System.Drawing.Size(200, 6);
          // 
          // exitToolStripMenuItem
          // 
          this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
          this.exitToolStripMenuItem.Size = new System.Drawing.Size(203, 22);
          this.exitToolStripMenuItem.Text = "E&xit";
          this.exitToolStripMenuItem.ToolTipText = "Exit MMed";
          this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
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
          this.MainSplitter.Size = new System.Drawing.Size(858, 626);
          this.MainSplitter.SplitterDistance = 160;
          this.MainSplitter.TabIndex = 2;
          // 
          // ChunkTreeView
          // 
          this.ChunkTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
          this.ChunkTreeView.Location = new System.Drawing.Point(0, 0);
          this.ChunkTreeView.Name = "ChunkTreeView";
          this.ChunkTreeView.Size = new System.Drawing.Size(160, 626);
          this.ChunkTreeView.TabIndex = 6;
          this.ChunkTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.ChunkTreeView_AfterSelect);
          // 
          // ViewerTabControl
          // 
          this.ViewerTabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                      | System.Windows.Forms.AnchorStyles.Left)
                      | System.Windows.Forms.AnchorStyles.Right)));
          this.ViewerTabControl.Controls.Add(this.ViewTabActions);
          this.ViewerTabControl.Controls.Add(this.ViewTabXML);
          this.ViewerTabControl.Controls.Add(this.ViewTabFlat);
          this.ViewerTabControl.Controls.Add(this.ViewTabImg);
          this.ViewerTabControl.Controls.Add(this.ViewTabGrid);
          this.ViewerTabControl.Controls.Add(this.ViewTab3D);
          this.ViewerTabControl.Controls.Add(this.ViewTabBump);
          this.ViewerTabControl.Controls.Add(this.ViewTabSteering);
          this.ViewerTabControl.Controls.Add(this.ViewTabCamera);
          this.ViewerTabControl.Controls.Add(this.ViewTabVRAM);
          this.ViewerTabControl.Controls.Add(this.ViewTab3dEditor);
          this.ViewerTabControl.Controls.Add(this.ViewTabHistory);
          this.ViewerTabControl.Location = new System.Drawing.Point(3, 3);
          this.ViewerTabControl.Name = "ViewerTabControl";
          this.ViewerTabControl.SelectedIndex = 0;
          this.ViewerTabControl.Size = new System.Drawing.Size(691, 620);
          this.ViewerTabControl.TabIndex = 0;
          this.ViewerTabControl.Selected += new System.Windows.Forms.TabControlEventHandler(this.ViewerTabControl_Selected);
          this.ViewerTabControl.SelectedIndexChanged += new System.EventHandler(this.ViewerTabControl_SelectedIndexChanged);
          // 
          // ViewTabActions
          // 
          this.ViewTabActions.Controls.Add(this.groupBox6);
          this.ViewTabActions.Controls.Add(this.groupBox5);
          this.ViewTabActions.Controls.Add(this.groupBox3);
          this.ViewTabActions.Location = new System.Drawing.Point(4, 22);
          this.ViewTabActions.Name = "ViewTabActions";
          this.ViewTabActions.Padding = new System.Windows.Forms.Padding(3);
          this.ViewTabActions.Size = new System.Drawing.Size(683, 594);
          this.ViewTabActions.TabIndex = 0;
          this.ViewTabActions.Text = "Actions";
          this.ViewTabActions.UseVisualStyleBackColor = true;
          // 
          // groupBox6
          // 
          this.groupBox6.Controls.Add(this.label20);
          this.groupBox6.Controls.Add(this.label19);
          this.groupBox6.Controls.Add(this.ActionsTabImportTMDButton);
          this.groupBox6.Controls.Add(this.ActionsTabExportTMDButton);
          this.groupBox6.Location = new System.Drawing.Point(0, 370);
          this.groupBox6.Name = "groupBox6";
          this.groupBox6.Size = new System.Drawing.Size(626, 83);
          this.groupBox6.TabIndex = 7;
          this.groupBox6.TabStop = false;
          this.groupBox6.Text = "TMD Objects";
          // 
          // label20
          // 
          this.label20.AutoSize = true;
          this.label20.Location = new System.Drawing.Point(157, 48);
          this.label20.Name = "label20";
          this.label20.Size = new System.Drawing.Size(212, 13);
          this.label20.TabIndex = 8;
          this.label20.Text = "Overwrite the selected TMD from a 3DS file";
          // 
          // label19
          // 
          this.label19.AutoSize = true;
          this.label19.Location = new System.Drawing.Point(157, 19);
          this.label19.Name = "label19";
          this.label19.Size = new System.Drawing.Size(181, 13);
          this.label19.TabIndex = 6;
          this.label19.Text = "Save the selected TMD to a 3DS file";
          // 
          // ActionsTabImportTMDButton
          // 
          this.ActionsTabImportTMDButton.Location = new System.Drawing.Point(9, 48);
          this.ActionsTabImportTMDButton.Name = "ActionsTabImportTMDButton";
          this.ActionsTabImportTMDButton.Size = new System.Drawing.Size(142, 23);
          this.ActionsTabImportTMDButton.TabIndex = 7;
          this.ActionsTabImportTMDButton.Text = "Import TMD from 3DS";
          this.ActionsTabImportTMDButton.UseVisualStyleBackColor = true;
          // 
          // ActionsTabExportTMDButton
          // 
          this.ActionsTabExportTMDButton.Location = new System.Drawing.Point(9, 19);
          this.ActionsTabExportTMDButton.Name = "ActionsTabExportTMDButton";
          this.ActionsTabExportTMDButton.Size = new System.Drawing.Size(142, 23);
          this.ActionsTabExportTMDButton.TabIndex = 6;
          this.ActionsTabExportTMDButton.Text = "Export TMD to 3DS";
          this.ActionsTabExportTMDButton.UseVisualStyleBackColor = true;
          // 
          // groupBox5
          // 
          this.groupBox5.Controls.Add(this.label9);
          this.groupBox5.Controls.Add(this.ActionsTabTIMTextureUsageButton);
          this.groupBox5.Controls.Add(this.ActionsTabImportTIMButton);
          this.groupBox5.Controls.Add(this.label11);
          this.groupBox5.Controls.Add(this.ActionsTabExportTIMButton);
          this.groupBox5.Controls.Add(this.label10);
          this.groupBox5.Location = new System.Drawing.Point(1, 223);
          this.groupBox5.Name = "groupBox5";
          this.groupBox5.Size = new System.Drawing.Size(627, 141);
          this.groupBox5.TabIndex = 6;
          this.groupBox5.TabStop = false;
          this.groupBox5.Text = "TIM Images";
          // 
          // label9
          // 
          this.label9.AutoSize = true;
          this.label9.Location = new System.Drawing.Point(157, 107);
          this.label9.Name = "label9";
          this.label9.Size = new System.Drawing.Size(344, 13);
          this.label9.TabIndex = 7;
          this.label9.Text = "Find unused or rarely used TIMs that look like they might be flat textures";
          // 
          // ActionsTabTIMTextureUsageButton
          // 
          this.ActionsTabTIMTextureUsageButton.Location = new System.Drawing.Point(9, 107);
          this.ActionsTabTIMTextureUsageButton.Name = "ActionsTabTIMTextureUsageButton";
          this.ActionsTabTIMTextureUsageButton.Size = new System.Drawing.Size(142, 23);
          this.ActionsTabTIMTextureUsageButton.TabIndex = 6;
          this.ActionsTabTIMTextureUsageButton.Text = "TIM Texture Usage Report";
          this.ActionsTabTIMTextureUsageButton.UseVisualStyleBackColor = true;
          // 
          // ActionsTabImportTIMButton
          // 
          this.ActionsTabImportTIMButton.Location = new System.Drawing.Point(9, 57);
          this.ActionsTabImportTIMButton.Name = "ActionsTabImportTIMButton";
          this.ActionsTabImportTIMButton.Size = new System.Drawing.Size(142, 23);
          this.ActionsTabImportTIMButton.TabIndex = 4;
          this.ActionsTabImportTIMButton.Text = "Import TIM from BMP";
          this.ActionsTabImportTIMButton.UseVisualStyleBackColor = true;
          // 
          // label11
          // 
          this.label11.AutoSize = true;
          this.label11.Location = new System.Drawing.Point(157, 57);
          this.label11.Name = "label11";
          this.label11.Size = new System.Drawing.Size(301, 39);
          this.label11.TabIndex = 5;
          this.label11.Text = "Overwrite the selected TIM image from a paletted BMP file.\r\nThe BMP must be the s" +
              "ame size, and use the same palette as \r\nthe TIM which you are replacing.";
          // 
          // ActionsTabExportTIMButton
          // 
          this.ActionsTabExportTIMButton.Location = new System.Drawing.Point(9, 19);
          this.ActionsTabExportTIMButton.Name = "ActionsTabExportTIMButton";
          this.ActionsTabExportTIMButton.Size = new System.Drawing.Size(142, 23);
          this.ActionsTabExportTIMButton.TabIndex = 2;
          this.ActionsTabExportTIMButton.Text = "Export TIM to BMP";
          this.ActionsTabExportTIMButton.UseVisualStyleBackColor = true;
          // 
          // label10
          // 
          this.label10.AutoSize = true;
          this.label10.Location = new System.Drawing.Point(157, 19);
          this.label10.Name = "label10";
          this.label10.Size = new System.Drawing.Size(250, 13);
          this.label10.TabIndex = 3;
          this.label10.Text = "Save the selected TIM image to a paletted BMP file";
          // 
          // groupBox3
          // 
          this.groupBox3.Controls.Add(this.groupBox7);
          this.groupBox3.Controls.Add(this.ActionsTabOptimiseButton);
          this.groupBox3.Controls.Add(this.ActionsTabImportFlatImagesButton);
          this.groupBox3.Controls.Add(this.label22);
          this.groupBox3.Controls.Add(this.ActionsTabExportFlatImagesButton);
          this.groupBox3.Controls.Add(this.label21);
          this.groupBox3.Location = new System.Drawing.Point(1, 6);
          this.groupBox3.Name = "groupBox3";
          this.groupBox3.Size = new System.Drawing.Size(627, 211);
          this.groupBox3.TabIndex = 4;
          this.groupBox3.TabStop = false;
          this.groupBox3.Text = "Level";
          // 
          // groupBox7
          // 
          this.groupBox7.Controls.Add(this.label25);
          this.groupBox7.Controls.Add(this.OptimiseCameraCompactCheckbox);
          this.groupBox7.Controls.Add(this.OptimiseCameraReindexCheckbox);
          this.groupBox7.Controls.Add(this.label1);
          this.groupBox7.Controls.Add(this.OptimiseSteeringCompactCheckbox);
          this.groupBox7.Controls.Add(this.OptimiseSteeringReindexCheckbox);
          this.groupBox7.Controls.Add(this.label24);
          this.groupBox7.Controls.Add(this.OptimiseBumpCompactCheckbox);
          this.groupBox7.Controls.Add(this.OptimiseBumpReindexCheckbox);
          this.groupBox7.Controls.Add(this.label23);
          this.groupBox7.Location = new System.Drawing.Point(160, 19);
          this.groupBox7.Name = "groupBox7";
          this.groupBox7.Size = new System.Drawing.Size(461, 94);
          this.groupBox7.TabIndex = 7;
          this.groupBox7.TabStop = false;
          this.groupBox7.Text = "Optimisation Settings";
          // 
          // label25
          // 
          this.label25.AutoSize = true;
          this.label25.Location = new System.Drawing.Point(241, 66);
          this.label25.Name = "label25";
          this.label25.Size = new System.Drawing.Size(206, 13);
          this.label25.TabIndex = 11;
          this.label25.Text = "! Reindex corrupts multi-player camera pos";
          // 
          // OptimiseCameraCompactCheckbox
          // 
          this.OptimiseCameraCompactCheckbox.AutoSize = true;
          this.OptimiseCameraCompactCheckbox.Location = new System.Drawing.Point(166, 66);
          this.OptimiseCameraCompactCheckbox.Name = "OptimiseCameraCompactCheckbox";
          this.OptimiseCameraCompactCheckbox.Size = new System.Drawing.Size(68, 17);
          this.OptimiseCameraCompactCheckbox.TabIndex = 10;
          this.OptimiseCameraCompactCheckbox.Text = "Compact";
          this.OptimiseCameraCompactCheckbox.UseVisualStyleBackColor = true;
          // 
          // OptimiseCameraReindexCheckbox
          // 
          this.OptimiseCameraReindexCheckbox.AutoSize = true;
          this.OptimiseCameraReindexCheckbox.Location = new System.Drawing.Point(74, 66);
          this.OptimiseCameraReindexCheckbox.Name = "OptimiseCameraReindexCheckbox";
          this.OptimiseCameraReindexCheckbox.Size = new System.Drawing.Size(65, 17);
          this.OptimiseCameraReindexCheckbox.TabIndex = 9;
          this.OptimiseCameraReindexCheckbox.Text = "Reindex";
          this.OptimiseCameraReindexCheckbox.UseVisualStyleBackColor = true;
          // 
          // label1
          // 
          this.label1.AutoSize = true;
          this.label1.Location = new System.Drawing.Point(7, 66);
          this.label1.Name = "label1";
          this.label1.Size = new System.Drawing.Size(67, 13);
          this.label1.TabIndex = 8;
          this.label1.Text = "Camera Pos:";
          // 
          // OptimiseSteeringCompactCheckbox
          // 
          this.OptimiseSteeringCompactCheckbox.AutoSize = true;
          this.OptimiseSteeringCompactCheckbox.Location = new System.Drawing.Point(166, 43);
          this.OptimiseSteeringCompactCheckbox.Name = "OptimiseSteeringCompactCheckbox";
          this.OptimiseSteeringCompactCheckbox.Size = new System.Drawing.Size(68, 17);
          this.OptimiseSteeringCompactCheckbox.TabIndex = 7;
          this.OptimiseSteeringCompactCheckbox.Text = "Compact";
          this.OptimiseSteeringCompactCheckbox.UseVisualStyleBackColor = true;
          // 
          // OptimiseSteeringReindexCheckbox
          // 
          this.OptimiseSteeringReindexCheckbox.AutoSize = true;
          this.OptimiseSteeringReindexCheckbox.Location = new System.Drawing.Point(74, 43);
          this.OptimiseSteeringReindexCheckbox.Name = "OptimiseSteeringReindexCheckbox";
          this.OptimiseSteeringReindexCheckbox.Size = new System.Drawing.Size(65, 17);
          this.OptimiseSteeringReindexCheckbox.TabIndex = 6;
          this.OptimiseSteeringReindexCheckbox.Text = "Reindex";
          this.OptimiseSteeringReindexCheckbox.UseVisualStyleBackColor = true;
          // 
          // label24
          // 
          this.label24.AutoSize = true;
          this.label24.Location = new System.Drawing.Point(7, 43);
          this.label24.Name = "label24";
          this.label24.Size = new System.Drawing.Size(49, 13);
          this.label24.TabIndex = 5;
          this.label24.Text = "Steering:";
          // 
          // OptimiseBumpCompactCheckbox
          // 
          this.OptimiseBumpCompactCheckbox.AutoSize = true;
          this.OptimiseBumpCompactCheckbox.Location = new System.Drawing.Point(166, 20);
          this.OptimiseBumpCompactCheckbox.Name = "OptimiseBumpCompactCheckbox";
          this.OptimiseBumpCompactCheckbox.Size = new System.Drawing.Size(68, 17);
          this.OptimiseBumpCompactCheckbox.TabIndex = 2;
          this.OptimiseBumpCompactCheckbox.Text = "Compact";
          this.OptimiseBumpCompactCheckbox.UseVisualStyleBackColor = true;
          // 
          // OptimiseBumpReindexCheckbox
          // 
          this.OptimiseBumpReindexCheckbox.AutoSize = true;
          this.OptimiseBumpReindexCheckbox.Location = new System.Drawing.Point(74, 20);
          this.OptimiseBumpReindexCheckbox.Name = "OptimiseBumpReindexCheckbox";
          this.OptimiseBumpReindexCheckbox.Size = new System.Drawing.Size(65, 17);
          this.OptimiseBumpReindexCheckbox.TabIndex = 1;
          this.OptimiseBumpReindexCheckbox.Text = "Reindex";
          this.OptimiseBumpReindexCheckbox.UseVisualStyleBackColor = true;
          // 
          // label23
          // 
          this.label23.AutoSize = true;
          this.label23.Location = new System.Drawing.Point(7, 20);
          this.label23.Name = "label23";
          this.label23.Size = new System.Drawing.Size(60, 13);
          this.label23.TabIndex = 0;
          this.label23.Text = "Bump map:";
          // 
          // ActionsTabOptimiseButton
          // 
          this.ActionsTabOptimiseButton.Location = new System.Drawing.Point(9, 19);
          this.ActionsTabOptimiseButton.Name = "ActionsTabOptimiseButton";
          this.ActionsTabOptimiseButton.Size = new System.Drawing.Size(142, 23);
          this.ActionsTabOptimiseButton.TabIndex = 6;
          this.ActionsTabOptimiseButton.Text = "Optimise Level";
          this.ActionsTabOptimiseButton.UseVisualStyleBackColor = true;
          // 
          // ActionsTabImportFlatImagesButton
          // 
          this.ActionsTabImportFlatImagesButton.Location = new System.Drawing.Point(9, 161);
          this.ActionsTabImportFlatImagesButton.Name = "ActionsTabImportFlatImagesButton";
          this.ActionsTabImportFlatImagesButton.Size = new System.Drawing.Size(142, 23);
          this.ActionsTabImportFlatImagesButton.TabIndex = 4;
          this.ActionsTabImportFlatImagesButton.Text = "Import flats from images";
          this.ActionsTabImportFlatImagesButton.UseVisualStyleBackColor = true;
          // 
          // label22
          // 
          this.label22.AutoSize = true;
          this.label22.Location = new System.Drawing.Point(157, 161);
          this.label22.Name = "label22";
          this.label22.Size = new System.Drawing.Size(333, 39);
          this.label22.TabIndex = 5;
          this.label22.Text = "Looks in the given directory for images of the the flats in the\r\nlevel, then atte" +
              "mpts to retexture the flats accordingly.\r\nAny flats which do not have correspond" +
              "ing images will be unchanged";
          // 
          // ActionsTabExportFlatImagesButton
          // 
          this.ActionsTabExportFlatImagesButton.Location = new System.Drawing.Point(9, 123);
          this.ActionsTabExportFlatImagesButton.Name = "ActionsTabExportFlatImagesButton";
          this.ActionsTabExportFlatImagesButton.Size = new System.Drawing.Size(142, 23);
          this.ActionsTabExportFlatImagesButton.TabIndex = 2;
          this.ActionsTabExportFlatImagesButton.Text = "Export flats to images";
          this.ActionsTabExportFlatImagesButton.UseVisualStyleBackColor = true;
          // 
          // label21
          // 
          this.label21.AutoSize = true;
          this.label21.Location = new System.Drawing.Point(157, 123);
          this.label21.Name = "label21";
          this.label21.Size = new System.Drawing.Size(304, 26);
          this.label21.TabIndex = 3;
          this.label21.Text = "Choose a directory to save images of the textures of all the flats\r\nin the level";
          // 
          // ViewTabXML
          // 
          this.ViewTabXML.Controls.Add(this.XMLViewerCommitBtn);
          this.ViewTabXML.Controls.Add(this.XMLTextBox);
          this.ViewTabXML.Location = new System.Drawing.Point(4, 22);
          this.ViewTabXML.Name = "ViewTabXML";
          this.ViewTabXML.Padding = new System.Windows.Forms.Padding(3);
          this.ViewTabXML.Size = new System.Drawing.Size(683, 594);
          this.ViewTabXML.TabIndex = 1;
          this.ViewTabXML.Text = "XML";
          this.ViewTabXML.UseVisualStyleBackColor = true;
          // 
          // XMLViewerCommitBtn
          // 
          this.XMLViewerCommitBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
          this.XMLViewerCommitBtn.Location = new System.Drawing.Point(609, 535);
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
          this.XMLTextBox.Size = new System.Drawing.Size(688, 529);
          this.XMLTextBox.TabIndex = 0;
          // 
          // ViewTabFlat
          // 
          this.ViewTabFlat.Controls.Add(this.FlatViewerDeleteButton);
          this.ViewTabFlat.Controls.Add(this.FlatViewerCloneButton);
          this.ViewTabFlat.Controls.Add(this.FlatViewerCommitBtn);
          this.ViewTabFlat.Controls.Add(this.FlatPanel);
          this.ViewTabFlat.Location = new System.Drawing.Point(4, 22);
          this.ViewTabFlat.Name = "ViewTabFlat";
          this.ViewTabFlat.Padding = new System.Windows.Forms.Padding(3);
          this.ViewTabFlat.Size = new System.Drawing.Size(683, 594);
          this.ViewTabFlat.TabIndex = 1;
          this.ViewTabFlat.Text = "Flat";
          this.ViewTabFlat.UseVisualStyleBackColor = true;
          // 
          // FlatViewerDeleteButton
          // 
          this.FlatViewerDeleteButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
          this.FlatViewerDeleteButton.Location = new System.Drawing.Point(87, 557);
          this.FlatViewerDeleteButton.Name = "FlatViewerDeleteButton";
          this.FlatViewerDeleteButton.Size = new System.Drawing.Size(75, 31);
          this.FlatViewerDeleteButton.TabIndex = 3;
          this.FlatViewerDeleteButton.Text = "Delete Flat";
          this.FlatViewerDeleteButton.UseVisualStyleBackColor = true;
          // 
          // FlatViewerCloneButton
          // 
          this.FlatViewerCloneButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
          this.FlatViewerCloneButton.Location = new System.Drawing.Point(6, 557);
          this.FlatViewerCloneButton.Name = "FlatViewerCloneButton";
          this.FlatViewerCloneButton.Size = new System.Drawing.Size(75, 31);
          this.FlatViewerCloneButton.TabIndex = 2;
          this.FlatViewerCloneButton.Text = "Clone Flat";
          this.FlatViewerCloneButton.UseVisualStyleBackColor = true;
          // 
          // FlatViewerCommitBtn
          // 
          this.FlatViewerCommitBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
          this.FlatViewerCommitBtn.Location = new System.Drawing.Point(600, 557);
          this.FlatViewerCommitBtn.Name = "FlatViewerCommitBtn";
          this.FlatViewerCommitBtn.Size = new System.Drawing.Size(75, 31);
          this.FlatViewerCommitBtn.TabIndex = 1;
          this.FlatViewerCommitBtn.Text = "Commit";
          this.FlatViewerCommitBtn.UseVisualStyleBackColor = true;
          // 
          // FlatPanel
          // 
          this.FlatPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                      | System.Windows.Forms.AnchorStyles.Left)
                      | System.Windows.Forms.AnchorStyles.Right)));
          this.FlatPanel.AutoScroll = true;
          this.FlatPanel.Location = new System.Drawing.Point(0, 0);
          this.FlatPanel.Name = "FlatPanel";
          this.FlatPanel.Size = new System.Drawing.Size(688, 564);
          this.FlatPanel.TabIndex = 0;
          // 
          // ViewTabImg
          // 
          this.ViewTabImg.Controls.Add(this.ImgPictureBox);
          this.ViewTabImg.Location = new System.Drawing.Point(4, 22);
          this.ViewTabImg.Name = "ViewTabImg";
          this.ViewTabImg.Padding = new System.Windows.Forms.Padding(3);
          this.ViewTabImg.Size = new System.Drawing.Size(683, 594);
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
          this.ViewTabGrid.Controls.Add(this.GridViewSelImageGroupBox);
          this.ViewTabGrid.Controls.Add(this.statusStrip1);
          this.ViewTabGrid.Controls.Add(this.groupBox2);
          this.ViewTabGrid.Controls.Add(this.groupBox1);
          this.ViewTabGrid.Controls.Add(this.GridDisplayPanelHolder);
          this.ViewTabGrid.Location = new System.Drawing.Point(4, 22);
          this.ViewTabGrid.Name = "ViewTabGrid";
          this.ViewTabGrid.Padding = new System.Windows.Forms.Padding(3);
          this.ViewTabGrid.Size = new System.Drawing.Size(683, 594);
          this.ViewTabGrid.TabIndex = 3;
          this.ViewTabGrid.Text = "Grid";
          this.ViewTabGrid.UseVisualStyleBackColor = true;
          // 
          // GridViewSelImageGroupBox
          // 
          this.GridViewSelImageGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                      | System.Windows.Forms.AnchorStyles.Right)));
          this.GridViewSelImageGroupBox.Controls.Add(this.GridViewSelPanelHolder);
          this.GridViewSelImageGroupBox.Location = new System.Drawing.Point(555, 291);
          this.GridViewSelImageGroupBox.Name = "GridViewSelImageGroupBox";
          this.GridViewSelImageGroupBox.Size = new System.Drawing.Size(133, 137);
          this.GridViewSelImageGroupBox.TabIndex = 4;
          this.GridViewSelImageGroupBox.TabStop = false;
          this.GridViewSelImageGroupBox.Text = "Selected Images";
          // 
          // GridViewSelPanelHolder
          // 
          this.GridViewSelPanelHolder.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                      | System.Windows.Forms.AnchorStyles.Left)
                      | System.Windows.Forms.AnchorStyles.Right)));
          this.GridViewSelPanelHolder.AutoScroll = true;
          this.GridViewSelPanelHolder.Controls.Add(this.GridViewSelPanel);
          this.GridViewSelPanelHolder.Location = new System.Drawing.Point(6, 19);
          this.GridViewSelPanelHolder.Name = "GridViewSelPanelHolder";
          this.GridViewSelPanelHolder.Size = new System.Drawing.Size(121, 112);
          this.GridViewSelPanelHolder.TabIndex = 0;
          // 
          // GridViewSelPanel
          // 
          this.GridViewSelPanel.Location = new System.Drawing.Point(0, 0);
          this.GridViewSelPanel.Name = "GridViewSelPanel";
          this.GridViewSelPanel.Size = new System.Drawing.Size(90, 68);
          this.GridViewSelPanel.TabIndex = 0;
          // 
          // statusStrip1
          // 
          this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.GridViewerStatusLabel});
          this.statusStrip1.Location = new System.Drawing.Point(3, 569);
          this.statusStrip1.Name = "statusStrip1";
          this.statusStrip1.Size = new System.Drawing.Size(677, 22);
          this.statusStrip1.TabIndex = 3;
          this.statusStrip1.Text = "statusStrip1";
          // 
          // GridViewerStatusLabel
          // 
          this.GridViewerStatusLabel.Name = "GridViewerStatusLabel";
          this.GridViewerStatusLabel.Size = new System.Drawing.Size(45, 17);
          this.GridViewerStatusLabel.Text = "(status)";
          // 
          // groupBox2
          // 
          this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                      | System.Windows.Forms.AnchorStyles.Right)));
          this.groupBox2.Controls.Add(this.smoothScrollingPanel1);
          this.groupBox2.Controls.Add(this.WorldCoordLabel);
          this.groupBox2.Controls.Add(this.label4);
          this.groupBox2.Controls.Add(this.FlatCoordLabel);
          this.groupBox2.Controls.Add(this.TexSquareLabel);
          this.groupBox2.Controls.Add(this.label3);
          this.groupBox2.Controls.Add(this.label2);
          this.groupBox2.Location = new System.Drawing.Point(0, 434);
          this.groupBox2.Name = "groupBox2";
          this.groupBox2.Size = new System.Drawing.Size(687, 130);
          this.groupBox2.TabIndex = 2;
          this.groupBox2.TabStop = false;
          this.groupBox2.Text = "Image Palette";
          // 
          // smoothScrollingPanel1
          // 
          this.smoothScrollingPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                      | System.Windows.Forms.AnchorStyles.Left)
                      | System.Windows.Forms.AnchorStyles.Right)));
          this.smoothScrollingPanel1.AutoScroll = true;
          this.smoothScrollingPanel1.Controls.Add(this.GridViewPalettePanel);
          this.smoothScrollingPanel1.Location = new System.Drawing.Point(3, 19);
          this.smoothScrollingPanel1.Name = "smoothScrollingPanel1";
          this.smoothScrollingPanel1.Size = new System.Drawing.Size(678, 105);
          this.smoothScrollingPanel1.TabIndex = 6;
          // 
          // GridViewPalettePanel
          // 
          this.GridViewPalettePanel.Location = new System.Drawing.Point(0, 0);
          this.GridViewPalettePanel.Name = "GridViewPalettePanel";
          this.GridViewPalettePanel.Size = new System.Drawing.Size(232, 59);
          this.GridViewPalettePanel.TabIndex = 0;
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
          // groupBox1
          // 
          this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
          this.groupBox1.Controls.Add(this.OverlaySelectorBehaviour);
          this.groupBox1.Controls.Add(this.OverlaySelectorSteering);
          this.groupBox1.Controls.Add(this.GridViewShowWaypointsCheck);
          this.groupBox1.Controls.Add(this.OverlaySelectorRespawn);
          this.groupBox1.Controls.Add(this.OverlaySelectorCamera);
          this.groupBox1.Controls.Add(this.OverlaySelectorGrid);
          this.groupBox1.Controls.Add(this.GridViewShowObjectsCheck);
          this.groupBox1.Controls.Add(this.GridViewZoomSlider);
          this.groupBox1.Controls.Add(this.GridViewTransparencySlider);
          this.groupBox1.Controls.Add(this.GridViewViewModeCombo);
          this.groupBox1.Controls.Add(this.GridViewMetaTypeCombo);
          this.groupBox1.Location = new System.Drawing.Point(555, 0);
          this.groupBox1.Name = "groupBox1";
          this.groupBox1.Size = new System.Drawing.Size(133, 285);
          this.groupBox1.TabIndex = 0;
          this.groupBox1.TabStop = false;
          this.groupBox1.Text = "View Options";
          // 
          // OverlaySelectorBehaviour
          // 
          this.OverlaySelectorBehaviour.Checked = false;
          this.OverlaySelectorBehaviour.CurrentColor = System.Drawing.Color.Transparent;
          this.OverlaySelectorBehaviour.DefaultColor = System.Drawing.Color.Black;
          this.OverlaySelectorBehaviour.Label = "Behaviour";
          this.OverlaySelectorBehaviour.Location = new System.Drawing.Point(6, 262);
          this.OverlaySelectorBehaviour.Name = "OverlaySelectorBehaviour";
          this.OverlaySelectorBehaviour.Size = new System.Drawing.Size(121, 20);
          this.OverlaySelectorBehaviour.TabIndex = 15;
          // 
          // OverlaySelectorSteering
          // 
          this.OverlaySelectorSteering.Checked = false;
          this.OverlaySelectorSteering.CurrentColor = System.Drawing.Color.Transparent;
          this.OverlaySelectorSteering.DefaultColor = System.Drawing.Color.Black;
          this.OverlaySelectorSteering.Label = "Steering";
          this.OverlaySelectorSteering.Location = new System.Drawing.Point(6, 244);
          this.OverlaySelectorSteering.Name = "OverlaySelectorSteering";
          this.OverlaySelectorSteering.Size = new System.Drawing.Size(121, 20);
          this.OverlaySelectorSteering.TabIndex = 14;
          // 
          // GridViewShowWaypointsCheck
          // 
          this.GridViewShowWaypointsCheck.AutoSize = true;
          this.GridViewShowWaypointsCheck.Location = new System.Drawing.Point(6, 172);
          this.GridViewShowWaypointsCheck.Name = "GridViewShowWaypointsCheck";
          this.GridViewShowWaypointsCheck.Size = new System.Drawing.Size(103, 17);
          this.GridViewShowWaypointsCheck.TabIndex = 13;
          this.GridViewShowWaypointsCheck.Text = "Show waypoints";
          this.GridViewShowWaypointsCheck.UseVisualStyleBackColor = true;
          // 
          // OverlaySelectorRespawn
          // 
          this.OverlaySelectorRespawn.Checked = false;
          this.OverlaySelectorRespawn.CurrentColor = System.Drawing.Color.Transparent;
          this.OverlaySelectorRespawn.DefaultColor = System.Drawing.Color.Red;
          this.OverlaySelectorRespawn.Label = "Respawn";
          this.OverlaySelectorRespawn.Location = new System.Drawing.Point(6, 226);
          this.OverlaySelectorRespawn.Name = "OverlaySelectorRespawn";
          this.OverlaySelectorRespawn.Size = new System.Drawing.Size(121, 20);
          this.OverlaySelectorRespawn.TabIndex = 12;
          // 
          // OverlaySelectorCamera
          // 
          this.OverlaySelectorCamera.Checked = false;
          this.OverlaySelectorCamera.CurrentColor = System.Drawing.Color.Transparent;
          this.OverlaySelectorCamera.DefaultColor = System.Drawing.Color.White;
          this.OverlaySelectorCamera.Label = "Camera";
          this.OverlaySelectorCamera.Location = new System.Drawing.Point(6, 208);
          this.OverlaySelectorCamera.Name = "OverlaySelectorCamera";
          this.OverlaySelectorCamera.Size = new System.Drawing.Size(121, 20);
          this.OverlaySelectorCamera.TabIndex = 11;
          // 
          // OverlaySelectorGrid
          // 
          this.OverlaySelectorGrid.Checked = false;
          this.OverlaySelectorGrid.CurrentColor = System.Drawing.Color.Transparent;
          this.OverlaySelectorGrid.DefaultColor = System.Drawing.Color.Gray;
          this.OverlaySelectorGrid.Label = "Grid";
          this.OverlaySelectorGrid.Location = new System.Drawing.Point(6, 190);
          this.OverlaySelectorGrid.Name = "OverlaySelectorGrid";
          this.OverlaySelectorGrid.Size = new System.Drawing.Size(121, 20);
          this.OverlaySelectorGrid.TabIndex = 10;
          // 
          // GridViewShowObjectsCheck
          // 
          this.GridViewShowObjectsCheck.AutoSize = true;
          this.GridViewShowObjectsCheck.Location = new System.Drawing.Point(6, 154);
          this.GridViewShowObjectsCheck.Name = "GridViewShowObjectsCheck";
          this.GridViewShowObjectsCheck.Size = new System.Drawing.Size(90, 17);
          this.GridViewShowObjectsCheck.TabIndex = 5;
          this.GridViewShowObjectsCheck.Text = "Show objects";
          this.GridViewShowObjectsCheck.UseVisualStyleBackColor = true;
          // 
          // GridViewZoomSlider
          // 
          this.GridViewZoomSlider.LargeChange = 10;
          this.GridViewZoomSlider.Location = new System.Drawing.Point(5, 112);
          this.GridViewZoomSlider.Maximum = 35;
          this.GridViewZoomSlider.MaximumLabel = "Zoom In";
          this.GridViewZoomSlider.Minimum = 5;
          this.GridViewZoomSlider.MinimumLabel = "Zoom Out";
          this.GridViewZoomSlider.Name = "GridViewZoomSlider";
          this.GridViewZoomSlider.Size = new System.Drawing.Size(122, 45);
          this.GridViewZoomSlider.SmallChange = 5;
          this.GridViewZoomSlider.TabIndex = 6;
          this.ToolTip.SetToolTip(this.GridViewZoomSlider, "Set the zoom level");
          this.GridViewZoomSlider.Value = 20;
          // 
          // GridViewTransparencySlider
          // 
          this.GridViewTransparencySlider.LargeChange = 5;
          this.GridViewTransparencySlider.Location = new System.Drawing.Point(5, 72);
          this.GridViewTransparencySlider.Maximum = 10;
          this.GridViewTransparencySlider.MaximumLabel = "Bump";
          this.GridViewTransparencySlider.Minimum = 0;
          this.GridViewTransparencySlider.MinimumLabel = "Textures";
          this.GridViewTransparencySlider.Name = "GridViewTransparencySlider";
          this.GridViewTransparencySlider.Size = new System.Drawing.Size(122, 39);
          this.GridViewTransparencySlider.SmallChange = 1;
          this.GridViewTransparencySlider.TabIndex = 4;
          this.ToolTip.SetToolTip(this.GridViewTransparencySlider, "Switch between texture and bump view");
          this.GridViewTransparencySlider.Value = 0;
          // 
          // GridViewViewModeCombo
          // 
          this.GridViewViewModeCombo.FormattingEnabled = true;
          this.GridViewViewModeCombo.Location = new System.Drawing.Point(5, 19);
          this.GridViewViewModeCombo.Name = "GridViewViewModeCombo";
          this.GridViewViewModeCombo.Size = new System.Drawing.Size(121, 21);
          this.GridViewViewModeCombo.TabIndex = 3;
          this.ToolTip.SetToolTip(this.GridViewViewModeCombo, "Change the primary view / edit mode of the grid");
          // 
          // GridViewMetaTypeCombo
          // 
          this.GridViewMetaTypeCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
          this.GridViewMetaTypeCombo.FormattingEnabled = true;
          this.GridViewMetaTypeCombo.Location = new System.Drawing.Point(5, 46);
          this.GridViewMetaTypeCombo.Name = "GridViewMetaTypeCombo";
          this.GridViewMetaTypeCombo.Size = new System.Drawing.Size(121, 21);
          this.GridViewMetaTypeCombo.TabIndex = 2;
          this.ToolTip.SetToolTip(this.GridViewMetaTypeCombo, "Change the selected meta type, for use when viewing or changing numeric meta data" +
                  "");
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
          this.GridDisplayPanelHolder.Size = new System.Drawing.Size(543, 422);
          this.GridDisplayPanelHolder.TabIndex = 1;
          // 
          // GridDisplayPanel
          // 
          this.GridDisplayPanel.Location = new System.Drawing.Point(0, 0);
          this.GridDisplayPanel.Name = "GridDisplayPanel";
          this.GridDisplayPanel.Size = new System.Drawing.Size(200, 192);
          this.GridDisplayPanel.TabIndex = 1;
          // 
          // ViewTab3D
          // 
          this.ViewTab3D.Controls.Add(this.Viewer3DRenderingSurface);
          this.ViewTab3D.Location = new System.Drawing.Point(4, 22);
          this.ViewTab3D.Name = "ViewTab3D";
          this.ViewTab3D.Padding = new System.Windows.Forms.Padding(3);
          this.ViewTab3D.Size = new System.Drawing.Size(683, 594);
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
          this.Viewer3DRenderingSurface.Size = new System.Drawing.Size(691, 570);
          this.Viewer3DRenderingSurface.TabIndex = 0;
          this.Viewer3DRenderingSurface.Text = "renderingSurface1";
          // 
          // ViewTabBump
          // 
          this.ViewTabBump.Controls.Add(this.label18);
          this.ViewTabBump.Controls.Add(this.label17);
          this.ViewTabBump.Controls.Add(this.label16);
          this.ViewTabBump.Controls.Add(this.label15);
          this.ViewTabBump.Controls.Add(this.BumpEditFillButton);
          this.ViewTabBump.Controls.Add(this.BumpTypeLabel);
          this.ViewTabBump.Controls.Add(this.BumpViewPictureBox);
          this.ViewTabBump.Controls.Add(this.BumpCombo);
          this.ViewTabBump.Controls.Add(this.BumpEditPictureBox);
          this.ViewTabBump.Location = new System.Drawing.Point(4, 22);
          this.ViewTabBump.Name = "ViewTabBump";
          this.ViewTabBump.Padding = new System.Windows.Forms.Padding(3);
          this.ViewTabBump.Size = new System.Drawing.Size(683, 594);
          this.ViewTabBump.TabIndex = 5;
          this.ViewTabBump.Text = "Bump";
          this.ViewTabBump.UseVisualStyleBackColor = true;
          // 
          // label18
          // 
          this.label18.AutoSize = true;
          this.label18.Location = new System.Drawing.Point(134, 284);
          this.label18.Name = "label18";
          this.label18.Size = new System.Drawing.Size(165, 13);
          this.label18.TabIndex = 12;
          this.label18.Text = "Click a pixel to view its bump type";
          // 
          // label17
          // 
          this.label17.AutoSize = true;
          this.label17.Location = new System.Drawing.Point(131, 80);
          this.label17.Name = "label17";
          this.label17.Size = new System.Drawing.Size(325, 13);
          this.label17.TabIndex = 11;
          this.label17.Text = "Select a bump type then click the pixels you want to set to that type";
          // 
          // label16
          // 
          this.label16.AutoSize = true;
          this.label16.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
          this.label16.Location = new System.Drawing.Point(3, 237);
          this.label16.Name = "label16";
          this.label16.Size = new System.Drawing.Size(47, 20);
          this.label16.TabIndex = 10;
          this.label16.Text = "View";
          // 
          // label15
          // 
          this.label15.AutoSize = true;
          this.label15.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
          this.label15.Location = new System.Drawing.Point(3, 15);
          this.label15.Name = "label15";
          this.label15.Size = new System.Drawing.Size(41, 20);
          this.label15.TabIndex = 9;
          this.label15.Text = "Edit";
          // 
          // BumpEditFillButton
          // 
          this.BumpEditFillButton.Location = new System.Drawing.Point(261, 44);
          this.BumpEditFillButton.Name = "BumpEditFillButton";
          this.BumpEditFillButton.Size = new System.Drawing.Size(75, 23);
          this.BumpEditFillButton.TabIndex = 8;
          this.BumpEditFillButton.Text = "Fill";
          this.BumpEditFillButton.UseVisualStyleBackColor = true;
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
          // ViewTabSteering
          // 
          this.ViewTabSteering.Controls.Add(this.SteeringEditFillButton);
          this.ViewTabSteering.Controls.Add(this.label8);
          this.ViewTabSteering.Controls.Add(this.label7);
          this.ViewTabSteering.Controls.Add(this.label6);
          this.ViewTabSteering.Controls.Add(this.label5);
          this.ViewTabSteering.Controls.Add(this.SteeringTypeLabel);
          this.ViewTabSteering.Controls.Add(this.SteeringViewPictureBox);
          this.ViewTabSteering.Controls.Add(this.SteeringCombo);
          this.ViewTabSteering.Controls.Add(this.SteeringEditPictureBox);
          this.ViewTabSteering.Location = new System.Drawing.Point(4, 22);
          this.ViewTabSteering.Name = "ViewTabSteering";
          this.ViewTabSteering.Padding = new System.Windows.Forms.Padding(3);
          this.ViewTabSteering.Size = new System.Drawing.Size(683, 594);
          this.ViewTabSteering.TabIndex = 5;
          this.ViewTabSteering.Text = "Steering";
          this.ViewTabSteering.UseVisualStyleBackColor = true;
          // 
          // SteeringEditFillButton
          // 
          this.SteeringEditFillButton.Location = new System.Drawing.Point(261, 44);
          this.SteeringEditFillButton.Name = "SteeringEditFillButton";
          this.SteeringEditFillButton.Size = new System.Drawing.Size(75, 23);
          this.SteeringEditFillButton.TabIndex = 8;
          this.SteeringEditFillButton.Text = "Fill";
          this.SteeringEditFillButton.UseVisualStyleBackColor = true;
          // 
          // label8
          // 
          this.label8.AutoSize = true;
          this.label8.Location = new System.Drawing.Point(134, 284);
          this.label8.Name = "label8";
          this.label8.Size = new System.Drawing.Size(156, 13);
          this.label8.TabIndex = 7;
          this.label8.Text = "Click a pixel to view its direction";
          // 
          // label7
          // 
          this.label7.AutoSize = true;
          this.label7.Location = new System.Drawing.Point(131, 80);
          this.label7.Name = "label7";
          this.label7.Size = new System.Drawing.Size(376, 13);
          this.label7.TabIndex = 6;
          this.label7.Text = "Select a steering direction then click the pixels you want to set to that directi" +
              "on";
          // 
          // label6
          // 
          this.label6.AutoSize = true;
          this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
          this.label6.Location = new System.Drawing.Point(3, 237);
          this.label6.Name = "label6";
          this.label6.Size = new System.Drawing.Size(47, 20);
          this.label6.TabIndex = 5;
          this.label6.Text = "View";
          // 
          // label5
          // 
          this.label5.AutoSize = true;
          this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
          this.label5.Location = new System.Drawing.Point(3, 15);
          this.label5.Name = "label5";
          this.label5.Size = new System.Drawing.Size(41, 20);
          this.label5.TabIndex = 4;
          this.label5.Text = "Edit";
          // 
          // SteeringTypeLabel
          // 
          this.SteeringTypeLabel.AutoSize = true;
          this.SteeringTypeLabel.Location = new System.Drawing.Point(134, 260);
          this.SteeringTypeLabel.Name = "SteeringTypeLabel";
          this.SteeringTypeLabel.Size = new System.Drawing.Size(96, 13);
          this.SteeringTypeLabel.TabIndex = 3;
          this.SteeringTypeLabel.Text = "SteeringTypeLabel";
          // 
          // SteeringViewPictureBox
          // 
          this.SteeringViewPictureBox.BackColor = System.Drawing.Color.White;
          this.SteeringViewPictureBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
          this.SteeringViewPictureBox.InitialImage = null;
          this.SteeringViewPictureBox.Location = new System.Drawing.Point(0, 260);
          this.SteeringViewPictureBox.Name = "SteeringViewPictureBox";
          this.SteeringViewPictureBox.Size = new System.Drawing.Size(128, 128);
          this.SteeringViewPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
          this.SteeringViewPictureBox.TabIndex = 2;
          this.SteeringViewPictureBox.TabStop = false;
          // 
          // SteeringCombo
          // 
          this.SteeringCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
          this.SteeringCombo.FormattingEnabled = true;
          this.SteeringCombo.Location = new System.Drawing.Point(134, 46);
          this.SteeringCombo.Name = "SteeringCombo";
          this.SteeringCombo.Size = new System.Drawing.Size(121, 21);
          this.SteeringCombo.TabIndex = 1;
          // 
          // SteeringEditPictureBox
          // 
          this.SteeringEditPictureBox.BackColor = System.Drawing.Color.White;
          this.SteeringEditPictureBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
          this.SteeringEditPictureBox.InitialImage = null;
          this.SteeringEditPictureBox.Location = new System.Drawing.Point(0, 43);
          this.SteeringEditPictureBox.Name = "SteeringEditPictureBox";
          this.SteeringEditPictureBox.Size = new System.Drawing.Size(128, 128);
          this.SteeringEditPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
          this.SteeringEditPictureBox.TabIndex = 0;
          this.SteeringEditPictureBox.TabStop = false;
          // 
          // ViewTabCamera
          // 
          this.ViewTabCamera.Controls.Add(this.CameraRenderingSurface);
          this.ViewTabCamera.Controls.Add(this.PanelCameraImage);
          this.ViewTabCamera.Controls.Add(this.SliderElevation);
          this.ViewTabCamera.Controls.Add(this.TextElevation);
          this.ViewTabCamera.Controls.Add(this.label14);
          this.ViewTabCamera.Controls.Add(this.SliderDistance);
          this.ViewTabCamera.Controls.Add(this.TextDistance);
          this.ViewTabCamera.Controls.Add(this.label13);
          this.ViewTabCamera.Controls.Add(this.SliderDirection);
          this.ViewTabCamera.Controls.Add(this.TextDirection);
          this.ViewTabCamera.Controls.Add(this.label12);
          this.ViewTabCamera.Location = new System.Drawing.Point(4, 22);
          this.ViewTabCamera.Name = "ViewTabCamera";
          this.ViewTabCamera.Padding = new System.Windows.Forms.Padding(3);
          this.ViewTabCamera.Size = new System.Drawing.Size(683, 594);
          this.ViewTabCamera.TabIndex = 5;
          this.ViewTabCamera.Text = "Camera";
          this.ViewTabCamera.UseVisualStyleBackColor = true;
          // 
          // CameraRenderingSurface
          // 
          this.CameraRenderingSurface.Location = new System.Drawing.Point(6, 181);
          this.CameraRenderingSurface.Name = "CameraRenderingSurface";
          this.CameraRenderingSurface.Size = new System.Drawing.Size(446, 300);
          this.CameraRenderingSurface.TabIndex = 13;
          this.CameraRenderingSurface.Text = "renderingSurface1";
          // 
          // PanelCameraImage
          // 
          this.PanelCameraImage.Location = new System.Drawing.Point(324, 19);
          this.PanelCameraImage.Name = "PanelCameraImage";
          this.PanelCameraImage.Size = new System.Drawing.Size(128, 128);
          this.PanelCameraImage.TabIndex = 12;
          // 
          // SliderElevation
          // 
          this.SliderElevation.Location = new System.Drawing.Point(68, 127);
          this.SliderElevation.Name = "SliderElevation";
          this.SliderElevation.Size = new System.Drawing.Size(250, 45);
          this.SliderElevation.TabIndex = 11;
          this.ToolTip.SetToolTip(this.SliderElevation, "Height of the camera from the vehicles");
          // 
          // TextElevation
          // 
          this.TextElevation.Location = new System.Drawing.Point(6, 127);
          this.TextElevation.Name = "TextElevation";
          this.TextElevation.Size = new System.Drawing.Size(56, 20);
          this.TextElevation.TabIndex = 10;
          // 
          // label14
          // 
          this.label14.AutoSize = true;
          this.label14.Location = new System.Drawing.Point(3, 111);
          this.label14.Name = "label14";
          this.label14.Size = new System.Drawing.Size(51, 13);
          this.label14.TabIndex = 9;
          this.label14.Text = "Elevation";
          // 
          // SliderDistance
          // 
          this.SliderDistance.Location = new System.Drawing.Point(68, 73);
          this.SliderDistance.Name = "SliderDistance";
          this.SliderDistance.Size = new System.Drawing.Size(250, 45);
          this.SliderDistance.TabIndex = 8;
          this.ToolTip.SetToolTip(this.SliderDistance, "Horizontal distance of the camera from the vehicles");
          // 
          // TextDistance
          // 
          this.TextDistance.Location = new System.Drawing.Point(6, 73);
          this.TextDistance.Name = "TextDistance";
          this.TextDistance.Size = new System.Drawing.Size(56, 20);
          this.TextDistance.TabIndex = 7;
          // 
          // label13
          // 
          this.label13.AutoSize = true;
          this.label13.Location = new System.Drawing.Point(3, 57);
          this.label13.Name = "label13";
          this.label13.Size = new System.Drawing.Size(49, 13);
          this.label13.TabIndex = 6;
          this.label13.Text = "Distance";
          // 
          // SliderDirection
          // 
          this.SliderDirection.LargeChange = 4;
          this.SliderDirection.Location = new System.Drawing.Point(68, 19);
          this.SliderDirection.Maximum = 8;
          this.SliderDirection.Minimum = -8;
          this.SliderDirection.Name = "SliderDirection";
          this.SliderDirection.Size = new System.Drawing.Size(250, 45);
          this.SliderDirection.TabIndex = 5;
          this.SliderDirection.TickFrequency = 2;
          this.ToolTip.SetToolTip(this.SliderDirection, "Angle of the camera to the vehicles");
          // 
          // TextDirection
          // 
          this.TextDirection.Location = new System.Drawing.Point(6, 19);
          this.TextDirection.Name = "TextDirection";
          this.TextDirection.Size = new System.Drawing.Size(56, 20);
          this.TextDirection.TabIndex = 1;
          // 
          // label12
          // 
          this.label12.AutoSize = true;
          this.label12.Location = new System.Drawing.Point(3, 3);
          this.label12.Name = "label12";
          this.label12.Size = new System.Drawing.Size(49, 13);
          this.label12.TabIndex = 0;
          this.label12.Text = "Direction";
          // 
          // ViewTabVRAM
          // 
          this.ViewTabVRAM.Controls.Add(this.VRAMScrollPanel);
          this.ViewTabVRAM.Controls.Add(this.VRAMStatusStrip);
          this.ViewTabVRAM.Location = new System.Drawing.Point(4, 22);
          this.ViewTabVRAM.Name = "ViewTabVRAM";
          this.ViewTabVRAM.Size = new System.Drawing.Size(683, 594);
          this.ViewTabVRAM.TabIndex = 6;
          this.ViewTabVRAM.Text = "VRAM";
          // 
          // VRAMScrollPanel
          // 
          this.VRAMScrollPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                      | System.Windows.Forms.AnchorStyles.Left)
                      | System.Windows.Forms.AnchorStyles.Right)));
          this.VRAMScrollPanel.AutoScroll = true;
          this.VRAMScrollPanel.Controls.Add(this.VRAMPictureBox);
          this.VRAMScrollPanel.Location = new System.Drawing.Point(0, 0);
          this.VRAMScrollPanel.Name = "VRAMScrollPanel";
          this.VRAMScrollPanel.Size = new System.Drawing.Size(688, 568);
          this.VRAMScrollPanel.TabIndex = 2;
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
          this.VRAMStatusStrip.Location = new System.Drawing.Point(0, 572);
          this.VRAMStatusStrip.Name = "VRAMStatusStrip";
          this.VRAMStatusStrip.Size = new System.Drawing.Size(683, 22);
          this.VRAMStatusStrip.TabIndex = 1;
          this.VRAMStatusStrip.Text = "statusStrip1";
          // 
          // ViewTab3dEditor
          // 
          this.ViewTab3dEditor.Controls.Add(this.panel2);
          this.ViewTab3dEditor.Controls.Add(this.LeftRightSplit);
          this.ViewTab3dEditor.Location = new System.Drawing.Point(4, 22);
          this.ViewTab3dEditor.Name = "ViewTab3dEditor";
          this.ViewTab3dEditor.Padding = new System.Windows.Forms.Padding(3);
          this.ViewTab3dEditor.Size = new System.Drawing.Size(683, 594);
          this.ViewTab3dEditor.TabIndex = 7;
          this.ViewTab3dEditor.Text = "3D Editor";
          this.ViewTab3dEditor.UseVisualStyleBackColor = true;
          // 
          // panel2
          // 
          this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                      | System.Windows.Forms.AnchorStyles.Right)));
          this.panel2.Controls.Add(this.ThreeDeeEditorStatusLabel);
          this.panel2.Location = new System.Drawing.Point(1, 561);
          this.panel2.Name = "panel2";
          this.panel2.Size = new System.Drawing.Size(678, 32);
          this.panel2.TabIndex = 1;
          // 
          // ThreeDeeEditorStatusLabel
          // 
          this.ThreeDeeEditorStatusLabel.AutoSize = true;
          this.ThreeDeeEditorStatusLabel.Location = new System.Drawing.Point(6, 6);
          this.ThreeDeeEditorStatusLabel.Name = "ThreeDeeEditorStatusLabel";
          this.ThreeDeeEditorStatusLabel.Size = new System.Drawing.Size(0, 13);
          this.ThreeDeeEditorStatusLabel.TabIndex = 0;
          // 
          // LeftRightSplit
          // 
          this.LeftRightSplit.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                      | System.Windows.Forms.AnchorStyles.Left)
                      | System.Windows.Forms.AnchorStyles.Right)));
          this.LeftRightSplit.Location = new System.Drawing.Point(3, 3);
          this.LeftRightSplit.Name = "LeftRightSplit";
          // 
          // LeftRightSplit.Panel1
          // 
          this.LeftRightSplit.Panel1.Controls.Add(this.LeftHandSplit);
          // 
          // LeftRightSplit.Panel2
          // 
          this.LeftRightSplit.Panel2.Controls.Add(this.RightHandSplit);
          this.LeftRightSplit.Size = new System.Drawing.Size(677, 552);
          this.LeftRightSplit.SplitterDistance = 307;
          this.LeftRightSplit.TabIndex = 0;
          // 
          // LeftHandSplit
          // 
          this.LeftHandSplit.Dock = System.Windows.Forms.DockStyle.Fill;
          this.LeftHandSplit.Location = new System.Drawing.Point(0, 0);
          this.LeftHandSplit.Name = "LeftHandSplit";
          this.LeftHandSplit.Orientation = System.Windows.Forms.Orientation.Horizontal;
          // 
          // LeftHandSplit.Panel1
          // 
          this.LeftHandSplit.Panel1.Controls.Add(this.Viewer3DRenderingSurfaceTopLeft);
          // 
          // LeftHandSplit.Panel2
          // 
          this.LeftHandSplit.Panel2.Controls.Add(this.Viewer3DRenderingSurfaceBottomLeft);
          this.LeftHandSplit.Size = new System.Drawing.Size(307, 552);
          this.LeftHandSplit.SplitterDistance = 251;
          this.LeftHandSplit.TabIndex = 0;
          // 
          // Viewer3DRenderingSurfaceTopLeft
          // 
          this.Viewer3DRenderingSurfaceTopLeft.Dock = System.Windows.Forms.DockStyle.Fill;
          this.Viewer3DRenderingSurfaceTopLeft.Location = new System.Drawing.Point(0, 0);
          this.Viewer3DRenderingSurfaceTopLeft.Name = "Viewer3DRenderingSurfaceTopLeft";
          this.Viewer3DRenderingSurfaceTopLeft.Size = new System.Drawing.Size(307, 251);
          this.Viewer3DRenderingSurfaceTopLeft.TabIndex = 0;
          this.Viewer3DRenderingSurfaceTopLeft.Text = "renderingSurface1";
          // 
          // Viewer3DRenderingSurfaceBottomLeft
          // 
          this.Viewer3DRenderingSurfaceBottomLeft.Dock = System.Windows.Forms.DockStyle.Fill;
          this.Viewer3DRenderingSurfaceBottomLeft.Location = new System.Drawing.Point(0, 0);
          this.Viewer3DRenderingSurfaceBottomLeft.Name = "Viewer3DRenderingSurfaceBottomLeft";
          this.Viewer3DRenderingSurfaceBottomLeft.Size = new System.Drawing.Size(307, 297);
          this.Viewer3DRenderingSurfaceBottomLeft.TabIndex = 0;
          this.Viewer3DRenderingSurfaceBottomLeft.Text = "renderingSurface3";
          // 
          // RightHandSplit
          // 
          this.RightHandSplit.Dock = System.Windows.Forms.DockStyle.Fill;
          this.RightHandSplit.Location = new System.Drawing.Point(0, 0);
          this.RightHandSplit.Name = "RightHandSplit";
          this.RightHandSplit.Orientation = System.Windows.Forms.Orientation.Horizontal;
          // 
          // RightHandSplit.Panel1
          // 
          this.RightHandSplit.Panel1.Controls.Add(this.Viewer3DRenderingSurfaceTopRight);
          // 
          // RightHandSplit.Panel2
          // 
          this.RightHandSplit.Panel2.Controls.Add(this.Viewer3DRenderingSurfaceBottomRight);
          this.RightHandSplit.Size = new System.Drawing.Size(366, 552);
          this.RightHandSplit.SplitterDistance = 253;
          this.RightHandSplit.TabIndex = 0;
          // 
          // Viewer3DRenderingSurfaceTopRight
          // 
          this.Viewer3DRenderingSurfaceTopRight.Dock = System.Windows.Forms.DockStyle.Fill;
          this.Viewer3DRenderingSurfaceTopRight.Location = new System.Drawing.Point(0, 0);
          this.Viewer3DRenderingSurfaceTopRight.Name = "Viewer3DRenderingSurfaceTopRight";
          this.Viewer3DRenderingSurfaceTopRight.Size = new System.Drawing.Size(366, 253);
          this.Viewer3DRenderingSurfaceTopRight.TabIndex = 0;
          this.Viewer3DRenderingSurfaceTopRight.Text = "renderingSurface2";
          // 
          // Viewer3DRenderingSurfaceBottomRight
          // 
          this.Viewer3DRenderingSurfaceBottomRight.Dock = System.Windows.Forms.DockStyle.Fill;
          this.Viewer3DRenderingSurfaceBottomRight.Location = new System.Drawing.Point(0, 0);
          this.Viewer3DRenderingSurfaceBottomRight.Name = "Viewer3DRenderingSurfaceBottomRight";
          this.Viewer3DRenderingSurfaceBottomRight.Size = new System.Drawing.Size(366, 295);
          this.Viewer3DRenderingSurfaceBottomRight.TabIndex = 0;
          this.Viewer3DRenderingSurfaceBottomRight.Text = "renderingSurface4";
          // 
          // ViewTabHistory
          // 
          this.ViewTabHistory.Controls.Add(this.RevertButton);
          this.ViewTabHistory.Controls.Add(this.VersionChangesTextBox);
          this.ViewTabHistory.Controls.Add(this.label27);
          this.ViewTabHistory.Controls.Add(this.VersionCreatedLabel);
          this.ViewTabHistory.Controls.Add(this.label26);
          this.ViewTabHistory.Location = new System.Drawing.Point(4, 22);
          this.ViewTabHistory.Name = "ViewTabHistory";
          this.ViewTabHistory.Padding = new System.Windows.Forms.Padding(3);
          this.ViewTabHistory.Size = new System.Drawing.Size(683, 594);
          this.ViewTabHistory.TabIndex = 8;
          this.ViewTabHistory.Text = "History";
          this.ViewTabHistory.UseVisualStyleBackColor = true;
          // 
          // RevertButton
          // 
          this.RevertButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
          this.RevertButton.Location = new System.Drawing.Point(482, 565);
          this.RevertButton.Name = "RevertButton";
          this.RevertButton.Size = new System.Drawing.Size(193, 23);
          this.RevertButton.TabIndex = 4;
          this.RevertButton.Text = "Revert to this version";
          this.RevertButton.UseVisualStyleBackColor = true;
          // 
          // VersionChangesTextBox
          // 
          this.VersionChangesTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                      | System.Windows.Forms.AnchorStyles.Left)
                      | System.Windows.Forms.AnchorStyles.Right)));
          this.VersionChangesTextBox.Location = new System.Drawing.Point(66, 24);
          this.VersionChangesTextBox.Multiline = true;
          this.VersionChangesTextBox.Name = "VersionChangesTextBox";
          this.VersionChangesTextBox.ReadOnly = true;
          this.VersionChangesTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
          this.VersionChangesTextBox.Size = new System.Drawing.Size(611, 535);
          this.VersionChangesTextBox.TabIndex = 3;
          // 
          // label27
          // 
          this.label27.AutoSize = true;
          this.label27.Location = new System.Drawing.Point(7, 24);
          this.label27.Name = "label27";
          this.label27.Size = new System.Drawing.Size(52, 13);
          this.label27.TabIndex = 2;
          this.label27.Text = "Changes:";
          // 
          // VersionCreatedLabel
          // 
          this.VersionCreatedLabel.AutoSize = true;
          this.VersionCreatedLabel.Location = new System.Drawing.Point(98, 7);
          this.VersionCreatedLabel.Name = "VersionCreatedLabel";
          this.VersionCreatedLabel.Size = new System.Drawing.Size(16, 13);
          this.VersionCreatedLabel.TabIndex = 1;
          this.VersionCreatedLabel.Text = "---";
          // 
          // label26
          // 
          this.label26.AutoSize = true;
          this.label26.Location = new System.Drawing.Point(8, 7);
          this.label26.Name = "label26";
          this.label26.Size = new System.Drawing.Size(84, 13);
          this.label26.TabIndex = 0;
          this.label26.Text = "Version created:";
          // 
          // OpenFileDialog
          // 
          this.OpenFileDialog.FileName = "openFileDialog1";
          // 
          // MainForm
          // 
          this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
          this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
          this.ClientSize = new System.Drawing.Size(858, 652);
          this.Controls.Add(this.MainSplitter);
          this.Controls.Add(this.mMenuStrip);
          this.Name = "MainForm";
          this.Text = "MMed";
          this.Load += new System.EventHandler(this.MainForm_Load);
          this.ResizeEnd += new System.EventHandler(this.OnResizeEnd);
          this.mMenuStrip.ResumeLayout(false);
          this.mMenuStrip.PerformLayout();
          this.MainSplitter.Panel1.ResumeLayout(false);
          this.MainSplitter.Panel2.ResumeLayout(false);
          this.MainSplitter.ResumeLayout(false);
          this.ViewerTabControl.ResumeLayout(false);
          this.ViewTabActions.ResumeLayout(false);
          this.groupBox6.ResumeLayout(false);
          this.groupBox6.PerformLayout();
          this.groupBox5.ResumeLayout(false);
          this.groupBox5.PerformLayout();
          this.groupBox3.ResumeLayout(false);
          this.groupBox3.PerformLayout();
          this.groupBox7.ResumeLayout(false);
          this.groupBox7.PerformLayout();
          this.ViewTabXML.ResumeLayout(false);
          this.ViewTabXML.PerformLayout();
          this.ViewTabFlat.ResumeLayout(false);
          this.ViewTabImg.ResumeLayout(false);
          this.ViewTabImg.PerformLayout();
          ((System.ComponentModel.ISupportInitialize)(this.ImgPictureBox)).EndInit();
          this.ViewTabGrid.ResumeLayout(false);
          this.ViewTabGrid.PerformLayout();
          this.GridViewSelImageGroupBox.ResumeLayout(false);
          this.GridViewSelPanelHolder.ResumeLayout(false);
          this.statusStrip1.ResumeLayout(false);
          this.statusStrip1.PerformLayout();
          this.groupBox2.ResumeLayout(false);
          this.groupBox2.PerformLayout();
          this.smoothScrollingPanel1.ResumeLayout(false);
          this.groupBox1.ResumeLayout(false);
          this.groupBox1.PerformLayout();
          this.GridDisplayPanelHolder.ResumeLayout(false);
          this.ViewTab3D.ResumeLayout(false);
          this.ViewTabBump.ResumeLayout(false);
          this.ViewTabBump.PerformLayout();
          ((System.ComponentModel.ISupportInitialize)(this.BumpViewPictureBox)).EndInit();
          ((System.ComponentModel.ISupportInitialize)(this.BumpEditPictureBox)).EndInit();
          this.ViewTabSteering.ResumeLayout(false);
          this.ViewTabSteering.PerformLayout();
          ((System.ComponentModel.ISupportInitialize)(this.SteeringViewPictureBox)).EndInit();
          ((System.ComponentModel.ISupportInitialize)(this.SteeringEditPictureBox)).EndInit();
          this.ViewTabCamera.ResumeLayout(false);
          this.ViewTabCamera.PerformLayout();
          ((System.ComponentModel.ISupportInitialize)(this.SliderElevation)).EndInit();
          ((System.ComponentModel.ISupportInitialize)(this.SliderDistance)).EndInit();
          ((System.ComponentModel.ISupportInitialize)(this.SliderDirection)).EndInit();
          this.ViewTabVRAM.ResumeLayout(false);
          this.ViewTabVRAM.PerformLayout();
          this.VRAMScrollPanel.ResumeLayout(false);
          this.VRAMScrollPanel.PerformLayout();
          ((System.ComponentModel.ISupportInitialize)(this.VRAMPictureBox)).EndInit();
          this.ViewTab3dEditor.ResumeLayout(false);
          this.panel2.ResumeLayout(false);
          this.panel2.PerformLayout();
          this.LeftRightSplit.Panel1.ResumeLayout(false);
          this.LeftRightSplit.Panel2.ResumeLayout(false);
          this.LeftRightSplit.ResumeLayout(false);
          this.LeftHandSplit.Panel1.ResumeLayout(false);
          this.LeftHandSplit.Panel2.ResumeLayout(false);
          this.LeftHandSplit.ResumeLayout(false);
          this.RightHandSplit.Panel1.ResumeLayout(false);
          this.RightHandSplit.Panel2.ResumeLayout(false);
          this.RightHandSplit.ResumeLayout(false);
          this.ViewTabHistory.ResumeLayout(false);
          this.ViewTabHistory.PerformLayout();
          this.ResumeLayout(false);
          this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStripMenuItem MnuFile;
        private System.Windows.Forms.SplitContainer MainSplitter;
        public System.Windows.Forms.TreeView ChunkTreeView;
      private System.Windows.Forms.ToolStripMenuItem MnuiFileOpenLevel;
        public System.Windows.Forms.MenuStrip mMenuStrip;
      public System.Windows.Forms.TabControl ViewerTabControl;
        public System.Windows.Forms.TabPage ViewTabXML;
        public System.Windows.Forms.Button XMLViewerCommitBtn;
        public System.Windows.Forms.TextBox XMLTextBox;
      public System.Windows.Forms.TabPage ViewTabFlat;
      public System.Windows.Forms.Button FlatViewerCommitBtn;
      public Util.FlatEditorPanel FlatPanel;
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
        private System.Windows.Forms.GroupBox groupBox1;
        public System.Windows.Forms.ComboBox GridViewMetaTypeCombo;
        public System.Windows.Forms.TabPage ViewTab3D;
        public GLTK.RenderingSurface Viewer3DRenderingSurface;
        public System.Windows.Forms.TabPage ViewTabBump;
        public System.Windows.Forms.TabPage ViewTabSteering;
        public System.Windows.Forms.TabPage ViewTabCamera;
        public System.Windows.Forms.PictureBox BumpEditPictureBox;
        public System.Windows.Forms.PictureBox SteeringEditPictureBox;
      public System.Windows.Forms.TabPage ViewTabVRAM;
        public System.Windows.Forms.PictureBox VRAMPictureBox;
        public System.Windows.Forms.StatusStrip VRAMStatusStrip;
        public System.Windows.Forms.ComboBox BumpCombo;
        public System.Windows.Forms.ComboBox SteeringCombo;
        public System.Windows.Forms.PictureBox BumpViewPictureBox;
        public System.Windows.Forms.PictureBox SteeringViewPictureBox;
        public System.Windows.Forms.Label BumpTypeLabel;
        public System.Windows.Forms.Label SteeringTypeLabel;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        public MMEd.Util.SmoothScrollingPanel GridDisplayPanelHolder;
        public MMEd.Util.UserPaintDoubleBufferedPanel GridDisplayPanel;
        private System.Windows.Forms.SplitContainer LeftRightSplit;
        private System.Windows.Forms.SplitContainer LeftHandSplit;
        private System.Windows.Forms.SplitContainer RightHandSplit;
        internal GLTK.RenderingSurface Viewer3DRenderingSurfaceTopLeft;
        internal GLTK.RenderingSurface Viewer3DRenderingSurfaceBottomLeft;
        internal GLTK.RenderingSurface Viewer3DRenderingSurfaceTopRight;
        internal GLTK.RenderingSurface Viewer3DRenderingSurfaceBottomRight;
        internal System.Windows.Forms.TabPage ViewTab3dEditor;
      private System.Windows.Forms.StatusStrip statusStrip1;
        public System.Windows.Forms.ToolStripStatusLabel GridViewerStatusLabel;
        private MMEd.Util.SmoothScrollingPanel smoothScrollingPanel1;
        public System.Windows.Forms.Panel GridViewPalettePanel;
        public System.Windows.Forms.Panel GridViewSelPanel;
        public MMEd.Util.SmoothScrollingPanel GridViewSelPanelHolder;
      public System.Windows.Forms.GroupBox GridViewSelImageGroupBox;
      public System.Windows.Forms.ComboBox GridViewViewModeCombo;
      public MMEd.Util.LabelledSlider GridViewTransparencySlider;
      public System.Windows.Forms.Button BumpEditFillButton;
      public System.Windows.Forms.Button SteeringEditFillButton;
      private System.Windows.Forms.Panel panel2;
      internal System.Windows.Forms.Label ThreeDeeEditorStatusLabel;
      public System.Windows.Forms.ToolTip ToolTip;
      public System.Windows.Forms.TabPage ViewTabActions;
      public System.Windows.Forms.CheckBox GridViewShowObjectsCheck;
      private System.Windows.Forms.GroupBox groupBox5;
      public System.Windows.Forms.Button ActionsTabImportTIMButton;
      private System.Windows.Forms.Label label11;
      public System.Windows.Forms.Button ActionsTabExportTIMButton;
      private System.Windows.Forms.Label label10;
      private System.Windows.Forms.GroupBox groupBox3;
      public MMEd.Util.LabelledSlider GridViewZoomSlider;
      private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
      public System.Windows.Forms.TrackBar SliderDistance;
      public System.Windows.Forms.TextBox TextDistance;
      private System.Windows.Forms.Label label13;
      public System.Windows.Forms.TrackBar SliderDirection;
      public System.Windows.Forms.TextBox TextDirection;
      private System.Windows.Forms.Label label12;
      public System.Windows.Forms.TrackBar SliderElevation;
      public System.Windows.Forms.TextBox TextElevation;
      private System.Windows.Forms.Label label14;
      private System.Windows.Forms.Label label18;
      private System.Windows.Forms.Label label17;
      private System.Windows.Forms.Label label16;
      private System.Windows.Forms.Label label15;
      public MMEd.Util.OverlaySelector OverlaySelectorGrid;
      public MMEd.Util.OverlaySelector OverlaySelectorRespawn;
      public MMEd.Util.OverlaySelector OverlaySelectorCamera;
      public System.Windows.Forms.Panel PanelCameraImage;
      private System.Windows.Forms.GroupBox groupBox6;
      private System.Windows.Forms.Label label20;
      private System.Windows.Forms.Label label19;
      public System.Windows.Forms.Button ActionsTabImportTMDButton;
      public System.Windows.Forms.Button ActionsTabExportTMDButton;
      public System.Windows.Forms.CheckBox GridViewShowWaypointsCheck;
      public GLTK.RenderingSurface CameraRenderingSurface;
      private MMEd.Util.SmoothScrollingPanel VRAMScrollPanel;
      public System.Windows.Forms.Button ActionsTabImportFlatImagesButton;
      private System.Windows.Forms.Label label22;
      public System.Windows.Forms.Button ActionsTabExportFlatImagesButton;
      private System.Windows.Forms.Label label21;
      public System.Windows.Forms.Button ActionsTabOptimiseButton;
      private System.Windows.Forms.GroupBox groupBox7;
      private System.Windows.Forms.Label label23;
      public System.Windows.Forms.CheckBox OptimiseBumpCompactCheckbox;
      public System.Windows.Forms.CheckBox OptimiseBumpReindexCheckbox;
      public System.Windows.Forms.CheckBox OptimiseSteeringCompactCheckbox;
      public System.Windows.Forms.CheckBox OptimiseSteeringReindexCheckbox;
      private System.Windows.Forms.Label label24;
      public System.Windows.Forms.CheckBox OptimiseCameraCompactCheckbox;
      public System.Windows.Forms.CheckBox OptimiseCameraReindexCheckbox;
      private System.Windows.Forms.Label label1;
      private System.Windows.Forms.Label label25;
      private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
      private System.Windows.Forms.ToolStripMenuItem publishToolStripMenuItem;
      private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
      private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
      private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
      private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem1;
      private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
      private System.Windows.Forms.Label label26;
      internal System.Windows.Forms.Label VersionCreatedLabel;
      internal System.Windows.Forms.TextBox VersionChangesTextBox;
      private System.Windows.Forms.Label label27;
      internal System.Windows.Forms.Button RevertButton;
      private System.Windows.Forms.SaveFileDialog SaveFileDialog;
      private System.Windows.Forms.OpenFileDialog OpenFileDialog;
      internal System.Windows.Forms.TabPage ViewTabHistory;
      public System.Windows.Forms.Button FlatViewerDeleteButton;
      public System.Windows.Forms.Button FlatViewerCloneButton;
      private System.Windows.Forms.Label label9;
      public System.Windows.Forms.Button ActionsTabTIMTextureUsageButton;
      public MMEd.Util.OverlaySelector OverlaySelectorSteering;
      public MMEd.Util.OverlaySelector OverlaySelectorBehaviour;
    }
}

