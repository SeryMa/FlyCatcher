namespace FlyCatcher
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
            this.mainMenuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.localVideoCaptureDeviceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openVideofileusingDirectShowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.fpsLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.mainPanel = new System.Windows.Forms.Panel();
            this.maskGroupBox = new System.Windows.Forms.GroupBox();
            this.maskControlContainer = new System.Windows.Forms.ListBox();
            this.videoGroupBox = new System.Windows.Forms.GroupBox();
            this.endingBias = new System.Windows.Forms.CheckBox();
            this.beginingBias = new System.Windows.Forms.CheckBox();
            this.runAnalysisTo = new System.Windows.Forms.NumericUpDown();
            this.runAnalysisFrom = new System.Windows.Forms.NumericUpDown();
            this.actualIndex = new System.Windows.Forms.NumericUpDown();
            this.videoSlider = new System.Windows.Forms.TrackBar();
            this.StopStartButton = new System.Windows.Forms.Button();
            this.VideoBox_processedPicture = new System.Windows.Forms.PictureBox();
            this.MaskContextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.applyMaskToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.MaskCIrcle = new System.Windows.Forms.ToolStripMenuItem();
            this.MaskRectangle = new System.Windows.Forms.ToolStripMenuItem();
            this.drawCurveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.VideoBox_staticPicture = new System.Windows.Forms.PictureBox();
            this.controlGroupBox = new System.Windows.Forms.GroupBox();
            this.upperBound = new System.Windows.Forms.NumericUpDown();
            this.lowerBound = new System.Windows.Forms.NumericUpDown();
            this.invertCheckBox = new System.Windows.Forms.CheckBox();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.maskTagBox = new System.Windows.Forms.TextBox();
            this.mainMenuStrip.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.mainPanel.SuspendLayout();
            this.maskGroupBox.SuspendLayout();
            this.videoGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.runAnalysisTo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.runAnalysisFrom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.actualIndex)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.videoSlider)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.VideoBox_processedPicture)).BeginInit();
            this.MaskContextMenuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.VideoBox_staticPicture)).BeginInit();
            this.controlGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.upperBound)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lowerBound)).BeginInit();
            this.SuspendLayout();
            // 
            // mainMenuStrip
            // 
            this.mainMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.mainMenuStrip.Location = new System.Drawing.Point(0, 0);
            this.mainMenuStrip.Name = "mainMenuStrip";
            this.mainMenuStrip.Size = new System.Drawing.Size(928, 24);
            this.mainMenuStrip.TabIndex = 0;
            this.mainMenuStrip.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.localVideoCaptureDeviceToolStripMenuItem,
            this.openVideofileusingDirectShowToolStripMenuItem,
            this.toolStripMenuItem1,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // localVideoCaptureDeviceToolStripMenuItem
            // 
            this.localVideoCaptureDeviceToolStripMenuItem.Name = "localVideoCaptureDeviceToolStripMenuItem";
            this.localVideoCaptureDeviceToolStripMenuItem.Size = new System.Drawing.Size(257, 22);
            this.localVideoCaptureDeviceToolStripMenuItem.Text = "Local &Video Capture Device";
            // 
            // openVideofileusingDirectShowToolStripMenuItem
            // 
            this.openVideofileusingDirectShowToolStripMenuItem.Name = "openVideofileusingDirectShowToolStripMenuItem";
            this.openVideofileusingDirectShowToolStripMenuItem.Size = new System.Drawing.Size(257, 22);
            this.openVideofileusingDirectShowToolStripMenuItem.Text = "Open video &file (using DirectShow)";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(254, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(257, 22);
            this.exitToolStripMenuItem.Text = "E&xit";
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fpsLabel});
            this.statusStrip.Location = new System.Drawing.Point(0, 508);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(928, 22);
            this.statusStrip.TabIndex = 1;
            this.statusStrip.Text = "statusStrip1";
            // 
            // fpsLabel
            // 
            this.fpsLabel.Name = "fpsLabel";
            this.fpsLabel.Size = new System.Drawing.Size(913, 17);
            this.fpsLabel.Spring = true;
            this.fpsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // mainPanel
            // 
            this.mainPanel.Controls.Add(this.maskGroupBox);
            this.mainPanel.Controls.Add(this.videoGroupBox);
            this.mainPanel.Controls.Add(this.controlGroupBox);
            this.mainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainPanel.Location = new System.Drawing.Point(0, 24);
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.Size = new System.Drawing.Size(928, 484);
            this.mainPanel.TabIndex = 2;
            // 
            // maskGroupBox
            // 
            this.maskGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.maskGroupBox.Controls.Add(this.maskTagBox);
            this.maskGroupBox.Controls.Add(this.maskControlContainer);
            this.maskGroupBox.Location = new System.Drawing.Point(764, 13);
            this.maskGroupBox.Name = "maskGroupBox";
            this.maskGroupBox.Size = new System.Drawing.Size(152, 461);
            this.maskGroupBox.TabIndex = 8;
            this.maskGroupBox.TabStop = false;
            this.maskGroupBox.Text = "Masks";
            // 
            // maskControlContainer
            // 
            this.maskControlContainer.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.maskControlContainer.FormattingEnabled = true;
            this.maskControlContainer.Location = new System.Drawing.Point(3, 38);
            this.maskControlContainer.Name = "maskControlContainer";
            this.maskControlContainer.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.maskControlContainer.Size = new System.Drawing.Size(146, 420);
            this.maskControlContainer.TabIndex = 0;
            this.maskControlContainer.SelectedIndexChanged += new System.EventHandler(this.displayMask);
            // 
            // videoGroupBox
            // 
            this.videoGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.videoGroupBox.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.videoGroupBox.Controls.Add(this.endingBias);
            this.videoGroupBox.Controls.Add(this.beginingBias);
            this.videoGroupBox.Controls.Add(this.runAnalysisTo);
            this.videoGroupBox.Controls.Add(this.runAnalysisFrom);
            this.videoGroupBox.Controls.Add(this.actualIndex);
            this.videoGroupBox.Controls.Add(this.videoSlider);
            this.videoGroupBox.Controls.Add(this.StopStartButton);
            this.videoGroupBox.Controls.Add(this.VideoBox_processedPicture);
            this.videoGroupBox.Controls.Add(this.VideoBox_staticPicture);
            this.videoGroupBox.Location = new System.Drawing.Point(110, 13);
            this.videoGroupBox.Name = "videoGroupBox";
            this.videoGroupBox.Size = new System.Drawing.Size(648, 461);
            this.videoGroupBox.TabIndex = 3;
            this.videoGroupBox.TabStop = false;
            this.videoGroupBox.Text = "Video";
            // 
            // endingBias
            // 
            this.endingBias.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.endingBias.AutoSize = true;
            this.endingBias.Checked = true;
            this.endingBias.CheckState = System.Windows.Forms.CheckState.Checked;
            this.endingBias.Location = new System.Drawing.Point(315, 436);
            this.endingBias.Name = "endingBias";
            this.endingBias.Size = new System.Drawing.Size(79, 17);
            this.endingBias.TabIndex = 13;
            this.endingBias.Text = "Run till end";
            this.endingBias.UseVisualStyleBackColor = true;
            this.endingBias.CheckedChanged += new System.EventHandler(this.endingBias_CheckedChanged);
            // 
            // beginingBias
            // 
            this.beginingBias.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.beginingBias.AutoSize = true;
            this.beginingBias.Checked = true;
            this.beginingBias.CheckState = System.Windows.Forms.CheckState.Checked;
            this.beginingBias.Location = new System.Drawing.Point(151, 436);
            this.beginingBias.Name = "beginingBias";
            this.beginingBias.Size = new System.Drawing.Size(112, 17);
            this.beginingBias.TabIndex = 12;
            this.beginingBias.Text = "Run from begining";
            this.beginingBias.UseVisualStyleBackColor = true;
            this.beginingBias.CheckedChanged += new System.EventHandler(this.beginingBias_CheckedChanged);
            // 
            // runAnalysisTo
            // 
            this.runAnalysisTo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.runAnalysisTo.Enabled = false;
            this.runAnalysisTo.Location = new System.Drawing.Point(269, 435);
            this.runAnalysisTo.Maximum = new decimal(new int[] {
            100000000,
            0,
            0,
            0});
            this.runAnalysisTo.Name = "runAnalysisTo";
            this.runAnalysisTo.Size = new System.Drawing.Size(40, 20);
            this.runAnalysisTo.TabIndex = 11;
            this.runAnalysisTo.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.runAnalysisTo.ThousandsSeparator = true;
            this.runAnalysisTo.ValueChanged += new System.EventHandler(this.runAnalysisTo_ValueChanged);
            // 
            // runAnalysisFrom
            // 
            this.runAnalysisFrom.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.runAnalysisFrom.Enabled = false;
            this.runAnalysisFrom.Location = new System.Drawing.Point(105, 435);
            this.runAnalysisFrom.Maximum = new decimal(new int[] {
            100000000,
            0,
            0,
            0});
            this.runAnalysisFrom.Name = "runAnalysisFrom";
            this.runAnalysisFrom.Size = new System.Drawing.Size(40, 20);
            this.runAnalysisFrom.TabIndex = 10;
            this.runAnalysisFrom.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.runAnalysisFrom.ThousandsSeparator = true;
            this.runAnalysisFrom.ValueChanged += new System.EventHandler(this.runAnalysisFrom_ValueChanged);
            // 
            // actualIndex
            // 
            this.actualIndex.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.actualIndex.Location = new System.Drawing.Point(59, 435);
            this.actualIndex.Maximum = new decimal(new int[] {
            100000000,
            0,
            0,
            0});
            this.actualIndex.Name = "actualIndex";
            this.actualIndex.Size = new System.Drawing.Size(40, 20);
            this.actualIndex.TabIndex = 9;
            this.actualIndex.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.actualIndex.ThousandsSeparator = true;
            this.actualIndex.ValueChanged += new System.EventHandler(this.pictureSelected);
            // 
            // videoSlider
            // 
            this.videoSlider.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.videoSlider.Location = new System.Drawing.Point(6, 384);
            this.videoSlider.Maximum = 100;
            this.videoSlider.Name = "videoSlider";
            this.videoSlider.Size = new System.Drawing.Size(637, 45);
            this.videoSlider.TabIndex = 8;
            this.videoSlider.Scroll += new System.EventHandler(this.videoSlider_Scroll);
            // 
            // StopStartButton
            // 
            this.StopStartButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.StopStartButton.Location = new System.Drawing.Point(6, 435);
            this.StopStartButton.Name = "StopStartButton";
            this.StopStartButton.Size = new System.Drawing.Size(47, 20);
            this.StopStartButton.TabIndex = 6;
            this.StopStartButton.Text = "Start";
            this.StopStartButton.UseVisualStyleBackColor = true;
            this.StopStartButton.Click += new System.EventHandler(this.StopStartButton_Click);
            // 
            // VideoBox_processedPicture
            // 
            this.VideoBox_processedPicture.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.VideoBox_processedPicture.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.VideoBox_processedPicture.ContextMenuStrip = this.MaskContextMenuStrip1;
            this.VideoBox_processedPicture.Location = new System.Drawing.Point(341, 19);
            this.VideoBox_processedPicture.Name = "VideoBox_processedPicture";
            this.VideoBox_processedPicture.Size = new System.Drawing.Size(300, 302);
            this.VideoBox_processedPicture.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.VideoBox_processedPicture.TabIndex = 1;
            this.VideoBox_processedPicture.TabStop = false;
            this.VideoBox_processedPicture.Paint += new System.Windows.Forms.PaintEventHandler(this.VideoBox_Paint);
            this.VideoBox_processedPicture.MouseClick += new System.Windows.Forms.MouseEventHandler(this.applyMask);
            this.VideoBox_processedPicture.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Draw);
            this.VideoBox_processedPicture.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.ChangeDimensions);
            // 
            // MaskContextMenuStrip1
            // 
            this.MaskContextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.applyMaskToolStripMenuItem,
            this.MaskCIrcle,
            this.MaskRectangle,
            this.drawCurveToolStripMenuItem});
            this.MaskContextMenuStrip1.Name = "contextMenuStrip1";
            this.MaskContextMenuStrip1.Size = new System.Drawing.Size(154, 92);
            this.MaskContextMenuStrip1.Text = "Set mask";
            // 
            // applyMaskToolStripMenuItem
            // 
            this.applyMaskToolStripMenuItem.Name = "applyMaskToolStripMenuItem";
            this.applyMaskToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.applyMaskToolStripMenuItem.Text = "Stop masking";
            this.applyMaskToolStripMenuItem.Click += new System.EventHandler(this.StopMasking);
            // 
            // MaskCIrcle
            // 
            this.MaskCIrcle.Name = "MaskCIrcle";
            this.MaskCIrcle.Size = new System.Drawing.Size(153, 22);
            this.MaskCIrcle.Text = "Draw circle";
            this.MaskCIrcle.Click += new System.EventHandler(this.StartDrawEllipse);
            // 
            // MaskRectangle
            // 
            this.MaskRectangle.Name = "MaskRectangle";
            this.MaskRectangle.Size = new System.Drawing.Size(153, 22);
            this.MaskRectangle.Text = "Draw rectangle";
            this.MaskRectangle.Click += new System.EventHandler(this.StartDrawRectangle);
            // 
            // drawCurveToolStripMenuItem
            // 
            this.drawCurveToolStripMenuItem.Enabled = false;
            this.drawCurveToolStripMenuItem.Name = "drawCurveToolStripMenuItem";
            this.drawCurveToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.drawCurveToolStripMenuItem.Text = "Draw curve";
            this.drawCurveToolStripMenuItem.Click += new System.EventHandler(this.StartDrawCurve);
            // 
            // VideoBox_staticPicture
            // 
            this.VideoBox_staticPicture.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.VideoBox_staticPicture.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.VideoBox_staticPicture.ContextMenuStrip = this.MaskContextMenuStrip1;
            this.VideoBox_staticPicture.Location = new System.Drawing.Point(6, 19);
            this.VideoBox_staticPicture.Name = "VideoBox_staticPicture";
            this.VideoBox_staticPicture.Size = new System.Drawing.Size(300, 302);
            this.VideoBox_staticPicture.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.VideoBox_staticPicture.TabIndex = 8;
            this.VideoBox_staticPicture.TabStop = false;
            this.VideoBox_staticPicture.Paint += new System.Windows.Forms.PaintEventHandler(this.VideoBox_Paint);
            this.VideoBox_staticPicture.MouseClick += new System.Windows.Forms.MouseEventHandler(this.applyMask);
            this.VideoBox_staticPicture.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Draw);
            this.VideoBox_staticPicture.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.ChangeDimensions);
            // 
            // controlGroupBox
            // 
            this.controlGroupBox.Controls.Add(this.upperBound);
            this.controlGroupBox.Controls.Add(this.lowerBound);
            this.controlGroupBox.Controls.Add(this.invertCheckBox);
            this.controlGroupBox.Location = new System.Drawing.Point(12, 13);
            this.controlGroupBox.Name = "controlGroupBox";
            this.controlGroupBox.Size = new System.Drawing.Size(92, 118);
            this.controlGroupBox.TabIndex = 7;
            this.controlGroupBox.TabStop = false;
            this.controlGroupBox.Text = "Controls";
            // 
            // upperBound
            // 
            this.upperBound.Location = new System.Drawing.Point(6, 42);
            this.upperBound.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.upperBound.Name = "upperBound";
            this.upperBound.Size = new System.Drawing.Size(41, 20);
            this.upperBound.TabIndex = 2;
            this.upperBound.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.upperBound.ValueChanged += new System.EventHandler(this.refreshActualFrame);
            // 
            // lowerBound
            // 
            this.lowerBound.Location = new System.Drawing.Point(6, 68);
            this.lowerBound.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.lowerBound.Name = "lowerBound";
            this.lowerBound.Size = new System.Drawing.Size(41, 20);
            this.lowerBound.TabIndex = 3;
            this.lowerBound.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.lowerBound.ValueChanged += new System.EventHandler(this.refreshActualFrame);
            // 
            // invertCheckBox
            // 
            this.invertCheckBox.AutoSize = true;
            this.invertCheckBox.Location = new System.Drawing.Point(6, 19);
            this.invertCheckBox.Name = "invertCheckBox";
            this.invertCheckBox.Size = new System.Drawing.Size(84, 17);
            this.invertCheckBox.TabIndex = 5;
            this.invertCheckBox.Text = "Invert colors";
            this.invertCheckBox.UseVisualStyleBackColor = true;
            this.invertCheckBox.CheckedChanged += new System.EventHandler(this.refreshActualFrame);
            // 
            // openFileDialog
            // 
            this.openFileDialog.Filter = "AVI files (*.avi)|*.avi|All files (*.*)|*.*";
            this.openFileDialog.Title = "Opem movie";
            // 
            // maskTagBox
            // 
            this.maskTagBox.AllowDrop = true;
            this.maskTagBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.maskTagBox.Location = new System.Drawing.Point(3, 16);
            this.maskTagBox.MaxLength = 255;
            this.maskTagBox.Name = "maskTagBox";
            this.maskTagBox.Size = new System.Drawing.Size(146, 20);
            this.maskTagBox.TabIndex = 1;
            this.maskTagBox.Text = "mask";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(928, 530);
            this.Controls.Add(this.mainPanel);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.mainMenuStrip);
            this.MainMenuStrip = this.mainMenuStrip;
            this.Name = "MainForm";
            this.Text = "Simple Player";
            this.mainMenuStrip.ResumeLayout(false);
            this.mainMenuStrip.PerformLayout();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.mainPanel.ResumeLayout(false);
            this.maskGroupBox.ResumeLayout(false);
            this.maskGroupBox.PerformLayout();
            this.videoGroupBox.ResumeLayout(false);
            this.videoGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.runAnalysisTo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.runAnalysisFrom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.actualIndex)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.videoSlider)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.VideoBox_processedPicture)).EndInit();
            this.MaskContextMenuStrip1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.VideoBox_staticPicture)).EndInit();
            this.controlGroupBox.ResumeLayout(false);
            this.controlGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.upperBound)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lowerBound)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip mainMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.Panel mainPanel;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem localVideoCaptureDeviceToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem openVideofileusingDirectShowToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.PictureBox VideoBox_processedPicture;
        private System.Windows.Forms.ToolStripStatusLabel fpsLabel;
        private System.Windows.Forms.GroupBox controlGroupBox;
        private System.Windows.Forms.Button StopStartButton;
        private System.Windows.Forms.PictureBox VideoBox_staticPicture;
        private System.Windows.Forms.GroupBox videoGroupBox;
        private System.Windows.Forms.NumericUpDown actualIndex;
        private System.Windows.Forms.TrackBar videoSlider;
        private System.Windows.Forms.CheckBox invertCheckBox;
        private System.Windows.Forms.NumericUpDown lowerBound;
        private System.Windows.Forms.NumericUpDown upperBound;
        private System.Windows.Forms.CheckBox endingBias;
        private System.Windows.Forms.CheckBox beginingBias;
        private System.Windows.Forms.NumericUpDown runAnalysisTo;
        private System.Windows.Forms.NumericUpDown runAnalysisFrom;
        private System.Windows.Forms.ContextMenuStrip MaskContextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem MaskCIrcle;
        private System.Windows.Forms.ToolStripMenuItem MaskRectangle;
        private System.Windows.Forms.ToolStripMenuItem drawCurveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem applyMaskToolStripMenuItem;
        private System.Windows.Forms.GroupBox maskGroupBox;
        private System.Windows.Forms.ListBox maskControlContainer;
        private System.Windows.Forms.TextBox maskTagBox;
    }
}

