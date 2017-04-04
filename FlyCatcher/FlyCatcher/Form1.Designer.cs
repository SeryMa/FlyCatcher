﻿namespace FlyCatcher
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
            this.HighlightBlob = new System.Windows.Forms.GroupBox();
            this.highlightBlobPictureBox = new System.Windows.Forms.PictureBox();
            this.maskGroupBox = new System.Windows.Forms.GroupBox();
            this.maskTagBox = new System.Windows.Forms.TextBox();
            this.maskControlContainer = new System.Windows.Forms.ListBox();
            this.videoGroupBox = new System.Windows.Forms.GroupBox();
            this.endingBias = new System.Windows.Forms.CheckBox();
            this.beginingBias = new System.Windows.Forms.CheckBox();
            this.runAnalysisTo = new System.Windows.Forms.NumericUpDown();
            this.runAnalysisFrom = new System.Windows.Forms.NumericUpDown();
            this.actualIndex = new System.Windows.Forms.NumericUpDown();
            this.videoSlider = new System.Windows.Forms.TrackBar();
            this.StartPauseButton = new System.Windows.Forms.Button();
            this.VideoBox = new System.Windows.Forms.PictureBox();
            this.MaskContextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.applyMaskToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.MaskCIrcle = new System.Windows.Forms.ToolStripMenuItem();
            this.MaskRectangle = new System.Windows.Forms.ToolStripMenuItem();
            this.drawCurveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.controlGroupBox = new System.Windows.Forms.GroupBox();
            this.DisplayOriginal = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.maskHeight = new System.Windows.Forms.NumericUpDown();
            this.maskWidth = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.blobUpperBound = new System.Windows.Forms.NumericUpDown();
            this.blobLowerBound = new System.Windows.Forms.NumericUpDown();
            this.invertCheckBox = new System.Windows.Forms.CheckBox();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.StopButton = new System.Windows.Forms.Button();
            this.mainMenuStrip.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.mainPanel.SuspendLayout();
            this.HighlightBlob.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.highlightBlobPictureBox)).BeginInit();
            this.maskGroupBox.SuspendLayout();
            this.videoGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.runAnalysisTo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.runAnalysisFrom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.actualIndex)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.videoSlider)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.VideoBox)).BeginInit();
            this.MaskContextMenuStrip1.SuspendLayout();
            this.controlGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.maskHeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.maskWidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.blobUpperBound)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.blobLowerBound)).BeginInit();
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
            this.mainPanel.Controls.Add(this.HighlightBlob);
            this.mainPanel.Controls.Add(this.maskGroupBox);
            this.mainPanel.Controls.Add(this.videoGroupBox);
            this.mainPanel.Controls.Add(this.controlGroupBox);
            this.mainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainPanel.Location = new System.Drawing.Point(0, 24);
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.Size = new System.Drawing.Size(928, 484);
            this.mainPanel.TabIndex = 2;
            // 
            // HighlightBlob
            // 
            this.HighlightBlob.Controls.Add(this.highlightBlobPictureBox);
            this.HighlightBlob.Location = new System.Drawing.Point(12, 239);
            this.HighlightBlob.Name = "HighlightBlob";
            this.HighlightBlob.Size = new System.Drawing.Size(92, 220);
            this.HighlightBlob.TabIndex = 10;
            this.HighlightBlob.TabStop = false;
            this.HighlightBlob.Text = "Highlight";
            // 
            // highlightBlobPictureBox
            // 
            this.highlightBlobPictureBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.highlightBlobPictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.highlightBlobPictureBox.Location = new System.Drawing.Point(3, 16);
            this.highlightBlobPictureBox.Name = "highlightBlobPictureBox";
            this.highlightBlobPictureBox.Size = new System.Drawing.Size(86, 201);
            this.highlightBlobPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.highlightBlobPictureBox.TabIndex = 14;
            this.highlightBlobPictureBox.TabStop = false;
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
            this.videoGroupBox.Controls.Add(this.StopButton);
            this.videoGroupBox.Controls.Add(this.endingBias);
            this.videoGroupBox.Controls.Add(this.beginingBias);
            this.videoGroupBox.Controls.Add(this.runAnalysisTo);
            this.videoGroupBox.Controls.Add(this.runAnalysisFrom);
            this.videoGroupBox.Controls.Add(this.actualIndex);
            this.videoGroupBox.Controls.Add(this.videoSlider);
            this.videoGroupBox.Controls.Add(this.StartPauseButton);
            this.videoGroupBox.Controls.Add(this.VideoBox);
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
            this.endingBias.Location = new System.Drawing.Point(393, 436);
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
            this.beginingBias.Location = new System.Drawing.Point(229, 436);
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
            this.runAnalysisTo.Location = new System.Drawing.Point(347, 435);
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
            this.runAnalysisFrom.Location = new System.Drawing.Point(183, 435);
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
            this.actualIndex.Location = new System.Drawing.Point(137, 435);
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
            // StartPauseButton
            // 
            this.StartPauseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.StartPauseButton.Location = new System.Drawing.Point(6, 435);
            this.StartPauseButton.Name = "StartPauseButton";
            this.StartPauseButton.Size = new System.Drawing.Size(47, 20);
            this.StartPauseButton.TabIndex = 6;
            this.StartPauseButton.Text = "Start";
            this.StartPauseButton.UseVisualStyleBackColor = true;
            this.StartPauseButton.Click += new System.EventHandler(this.PauseStartButton_Click);
            // 
            // VideoBox
            // 
            this.VideoBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.VideoBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.VideoBox.ContextMenuStrip = this.MaskContextMenuStrip1;
            this.VideoBox.Location = new System.Drawing.Point(6, 19);
            this.VideoBox.Name = "VideoBox";
            this.VideoBox.Size = new System.Drawing.Size(635, 362);
            this.VideoBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.VideoBox.TabIndex = 1;
            this.VideoBox.TabStop = false;
            this.VideoBox.Paint += new System.Windows.Forms.PaintEventHandler(this.VideoBox_Paint);
            this.VideoBox.MouseClick += new System.Windows.Forms.MouseEventHandler(this.applyMask);
            this.VideoBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Draw);
            this.VideoBox.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.ChangeDimensions);
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
            // controlGroupBox
            // 
            this.controlGroupBox.Controls.Add(this.DisplayOriginal);
            this.controlGroupBox.Controls.Add(this.label2);
            this.controlGroupBox.Controls.Add(this.maskHeight);
            this.controlGroupBox.Controls.Add(this.maskWidth);
            this.controlGroupBox.Controls.Add(this.label1);
            this.controlGroupBox.Controls.Add(this.blobUpperBound);
            this.controlGroupBox.Controls.Add(this.blobLowerBound);
            this.controlGroupBox.Controls.Add(this.invertCheckBox);
            this.controlGroupBox.Location = new System.Drawing.Point(12, 13);
            this.controlGroupBox.Name = "controlGroupBox";
            this.controlGroupBox.Size = new System.Drawing.Size(92, 220);
            this.controlGroupBox.TabIndex = 7;
            this.controlGroupBox.TabStop = false;
            this.controlGroupBox.Text = "Controls";
            // 
            // DisplayOriginal
            // 
            this.DisplayOriginal.AutoSize = true;
            this.DisplayOriginal.Location = new System.Drawing.Point(6, 172);
            this.DisplayOriginal.Name = "DisplayOriginal";
            this.DisplayOriginal.Size = new System.Drawing.Size(79, 17);
            this.DisplayOriginal.TabIndex = 10;
            this.DisplayOriginal.Text = "Raw image";
            this.DisplayOriginal.UseVisualStyleBackColor = true;
            this.DisplayOriginal.CheckedChanged += new System.EventHandler(this.DisplayOriginal_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 104);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(57, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "Mask size:";
            // 
            // maskHeight
            // 
            this.maskHeight.Location = new System.Drawing.Point(6, 120);
            this.maskHeight.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.maskHeight.Name = "maskHeight";
            this.maskHeight.Size = new System.Drawing.Size(41, 20);
            this.maskHeight.TabIndex = 7;
            this.maskHeight.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // maskWidth
            // 
            this.maskWidth.Location = new System.Drawing.Point(6, 146);
            this.maskWidth.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.maskWidth.Name = "maskWidth";
            this.maskWidth.Size = new System.Drawing.Size(41, 20);
            this.maskWidth.TabIndex = 8;
            this.maskWidth.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 39);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(52, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Blob size:";
            // 
            // blobUpperBound
            // 
            this.blobUpperBound.Location = new System.Drawing.Point(6, 55);
            this.blobUpperBound.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.blobUpperBound.Name = "blobUpperBound";
            this.blobUpperBound.Size = new System.Drawing.Size(41, 20);
            this.blobUpperBound.TabIndex = 2;
            this.blobUpperBound.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.blobUpperBound.ValueChanged += new System.EventHandler(this.refreshActualFrame);
            // 
            // blobLowerBound
            // 
            this.blobLowerBound.Location = new System.Drawing.Point(6, 81);
            this.blobLowerBound.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.blobLowerBound.Name = "blobLowerBound";
            this.blobLowerBound.Size = new System.Drawing.Size(41, 20);
            this.blobLowerBound.TabIndex = 3;
            this.blobLowerBound.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.blobLowerBound.ValueChanged += new System.EventHandler(this.refreshActualFrame);
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
            // StopButton
            // 
            this.StopButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.StopButton.Location = new System.Drawing.Point(59, 435);
            this.StopButton.Name = "StopButton";
            this.StopButton.Size = new System.Drawing.Size(47, 20);
            this.StopButton.TabIndex = 14;
            this.StopButton.Text = "Stop";
            this.StopButton.UseVisualStyleBackColor = true;
            this.StopButton.Click += new System.EventHandler(this.StopButton_Click);
            // 
            // MainForm
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(928, 530);
            this.Controls.Add(this.mainPanel);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.mainMenuStrip);
            this.MainMenuStrip = this.mainMenuStrip;
            this.Name = "MainForm";
            this.Text = "Simple Player";
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.InitParams);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.MainForm_DragEnter);
            this.mainMenuStrip.ResumeLayout(false);
            this.mainMenuStrip.PerformLayout();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.mainPanel.ResumeLayout(false);
            this.HighlightBlob.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.highlightBlobPictureBox)).EndInit();
            this.maskGroupBox.ResumeLayout(false);
            this.maskGroupBox.PerformLayout();
            this.videoGroupBox.ResumeLayout(false);
            this.videoGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.runAnalysisTo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.runAnalysisFrom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.actualIndex)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.videoSlider)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.VideoBox)).EndInit();
            this.MaskContextMenuStrip1.ResumeLayout(false);
            this.controlGroupBox.ResumeLayout(false);
            this.controlGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.maskHeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.maskWidth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.blobUpperBound)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.blobLowerBound)).EndInit();
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
        private System.Windows.Forms.PictureBox VideoBox;
        private System.Windows.Forms.ToolStripStatusLabel fpsLabel;
        private System.Windows.Forms.GroupBox controlGroupBox;
        private System.Windows.Forms.Button StartPauseButton;
        private System.Windows.Forms.GroupBox videoGroupBox;
        private System.Windows.Forms.NumericUpDown actualIndex;
        private System.Windows.Forms.TrackBar videoSlider;
        private System.Windows.Forms.CheckBox invertCheckBox;
        private System.Windows.Forms.NumericUpDown blobLowerBound;
        private System.Windows.Forms.NumericUpDown blobUpperBound;
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
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown maskHeight;
        private System.Windows.Forms.NumericUpDown maskWidth;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox HighlightBlob;
        private System.Windows.Forms.PictureBox highlightBlobPictureBox;
        private System.Windows.Forms.CheckBox DisplayOriginal;
        private System.Windows.Forms.Button StopButton;
    }
}

