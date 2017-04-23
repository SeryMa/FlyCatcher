using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using System.IO;
using System.Threading.Tasks;

using AForge.Video;

using AForge.Imaging;


namespace FlyCatcher
{
    public partial class MainForm : Form
    {
        private IGiver<Bitmap> pictureGiver;
        private IPreProcessor<Bitmap> pictureProcessor;
        private ICounter<Bitmap, Blob, AForge.Point> blobCounter;
        private List<IKeeper<Blob, double, double, AForge.Point>> blobKeepers;

        private static OpenFileDialog openDialog = new OpenFileDialog();

        internal IEnumerable<IMask<AForge.Point>> masks { get { return maskControlContainer.Items.OfType<IMask<AForge.Point>>(); } }

        internal bool shouldInvert { get { return invertCheckBox.Checked; } }
        internal bool filterByArea { get { return filterStyleCheckBox.Checked; } }
        internal int upperBoundValue { get { return (int)blobUpperBound.Value; } }
        internal int lowerBoundValue { get { return (int)blobLowerBound.Value; } }

        internal double redCoeficient { get { return (double)redCoeficientControl.Value; } }
        internal double greenCoeficient { get { return (double)greenCoeficientControl.Value; } }
        internal double blueCoeficient { get { return (double)blueCoeficientControl.Value; } }

        private int historyCount;
        private double penaltyValue;

        public MainForm()
        {
            InitializeComponent();

            videoGroupBox.MinimumSize = new Size(endingBias.Right + 1, controlGroupBox.Height);
            maskGroupBox.MinimumSize = new Size(maskGroupBox.Width, controlGroupBox.Height);
            controlGroupBox.MinimumSize = controlGroupBox.Size;

            //TODO: this could be calculated more preciselly
            MinimumSize =
                new Size(videoGroupBox.MinimumSize.Width + maskGroupBox.MinimumSize.Width + controlGroupBox.MinimumSize.Width + 4 * controlGroupBox.Left + 3,
                         controlGroupBox.Bottom + fpsLabel.Height + mainMenuStrip.Height + 50);

            openDialog.Filter = Constants.FileExtensions;
            openDialog.FilterIndex = 1;
            openDialog.RestoreDirectory = true;

            initParams(parameters: null);

            pictureProcessor = new BitmapPreProcessor(this);
            blobCounter = new PictureBlobCounter(this);
            blobKeepers = new List<IKeeper<Blob, double, double, AForge.Point>>();

            DisplayControl.SelectedIndex = 0;

            
            refreshActualFrame();
        }

        #region Init
        
        T getParameter<T>(string parameter, ILookup<string, string> parameters, T def, Extensions.Transform<T, string> transform)
            => parameters != null && parameters.Contains(parameter) ? transform(parameters[parameter].First()) : def;

        private ILookup<string, string> getParameters(string path) => (from line in File.ReadLines(path) where line.Contains("=") select line.Split('='))
                                                                      .ToLookup(parsedLine => parsedLine[0].Trim(), s => s[1].Trim());

        private void initPictureGiver(string path)
        {
            switch (Path.GetExtension(path))
            {
                case ".jpg":
                case ".png":
                case ".bmp":
                    initPictureGiverWithSeparatePhotoGiver(path);
                    break;
                case ".avi":
                    initPictureGiverWithAVIGiver(path);
                    break;
                case ".mpeg":
                    initPictureGiverWithMPEGGiver(path);
                    break;
                default:
                    break;
            }

            runAnalysisFrom.Minimum = actualIndex.Minimum = runAnalysisTo.Minimum = pictureGiver.RunFromMin;
            runAnalysisFrom.Maximum = actualIndex.Maximum = runAnalysisTo.Maximum = step.Maximum = pictureGiver.RunToMax;

            videoGroupBox.Text = $"Video - {pictureGiver.Tag}";
            fpsLabel.Text = $"{pictureGiver.Tag}";
            Text = $"FlyCatcher - {pictureGiver.Tag}";

            enableFlowControl();
            refreshActualFrame();
        }
        private void initPictureGiverWithSeparatePhotoGiver(string path) => pictureGiver = new SeparatePhotoGiver(path);
        private void initPictureGiverWithAVIGiver(string path) => pictureGiver = new VideoAVIGiver(path);
        private void initPictureGiverWithMPEGGiver(string path) => pictureGiver = new VideoMPEGGiver(path);

        private void initParams(ILookup<string, string> parameters)
        {
            historyCount = getParameter("history", parameters, 10, int.Parse);
            penaltyValue = getParameter("penalty", parameters, 10, int.Parse);

            blobUpperBound.Value = getParameter("max_size", parameters, 10, int.Parse);
            blobLowerBound.Value = getParameter("min_size", parameters, 5, int.Parse);

            maskHeight.Value = getParameter("height", parameters, 10, int.Parse);
            maskWidth.Value = getParameter("width", parameters, 10, int.Parse);

            invertCheckBox.Checked = getParameter("invert_colors", parameters, true, bool.Parse);

            initMasks(parameters);
            initOutputFormat(parameters);
            initHighlight(parameters);
        }
        private void initOutputFormat(ILookup<string, string> parameters)
        {
            outputFormat = Constants.OutputFormat.None;

            foreach (var tag in Constants.OutputTag)
                outputFormat |= (getParameter(tag.Value, parameters, true, bool.Parse) ? tag.Key : Constants.OutputFormat.None);
        }
        private void initHighlight(ILookup<string, string> parameters)
        {
            highlightFormat = Constants.HighlightFormat.None;

            foreach (var tag in Constants.HighlightTag)
                highlightFormat |= (getParameter(tag.Value, parameters, false, bool.Parse) ? tag.Key : Constants.HighlightFormat.None);

            //Special case which default value should be true, not false like with the others
            highlightFormat |= (getParameter(Constants.HighlightTag[Constants.HighlightFormat.Object], parameters, true, bool.Parse) ? Constants.HighlightFormat.Object : Constants.HighlightFormat.None);
        }
        private void initMasks(ILookup<string, string> parameters)
        {
            if (getParameter("clean", parameters, false, bool.Parse))
            {
                maskControlContainer.Items.Clear();
                blobKeepers.Clear();
            }

            if (parameters != null)
                foreach (var param in parameters)
                {
                    if (param.Key == "ellipse" || param.Key == "rectangle")
                    {
                        foreach (var mask in param)
                        {
                            var pars = mask.Split(' ');
                            RectangleF rect = MathFunctions.getRectangle(float.Parse(pars[1]), float.Parse(pars[2]), float.Parse(pars[3]), float.Parse(pars[4]));
                            //TODO: better parsing here...

                            switch (param.Key)
                            {
                                case "ellipse":
                                    addMask(DrawStyle.Ellipse, rect, pars[0]);
                                    break;
                                case "rectangle":
                                    addMask(DrawStyle.Rectangle, rect, pars[0]);
                                    break;
                                default:
                                    break;
                            }
                        }
                    }

                }
        }

        private void initOutput(string path)
        {
            if (Output != null)
                Output.Close();

            Output = new MyStream(path);
        }

        private void InitParams(string path)
        {
            path = path.ToLower();

            try
            {
                switch (Path.GetExtension(path))
                {
                    case ".config":
                        initParams(getParameters(path));
                        break;
                    case ".mask":
                        initMasks(getParameters(path));
                        break;
                    case ".output":
                        initOutputFormat(getParameters(path));
                        break;
                    case ".csv":
                        initOutput(path);
                        break;
                    case ".jpg":
                    case ".png":
                    case ".bmp":
                    case ".avi":
                    case ".mpeg":
                        initPictureGiver(path);
                        break;
                    default:
                        break;
                }
            }
            catch (VideoException ex)
            {
                MessageBox.Show(ex.Message, "Failed loading video", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (FormatException ex)
            {
                MessageBox.Show(ex.Message, "Wrong file name format", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DragDroped(object sender, DragEventArgs e)
        {
            string[] fileList = (string[])e.Data.GetData(DataFormats.FileDrop, false);

            foreach (var path in fileList)
                InitParams(path);

            refreshActualFrame();
        }

        private void promptToOpenFile()
        {
            if (openDialog.ShowDialog(this) == DialogResult.OK)
                foreach (var path in openDialog.FileNames)
                    InitParams(path);
        }

        private void openFileClick(object sender, EventArgs e) => promptToOpenFile();

        private void saveCurrentConfiguration(object sender, EventArgs e)
        {
            SaveFileDialog dial = new SaveFileDialog();

            dial.FilterIndex = 1;
            dial.RestoreDirectory = true;
            dial.AddExtension = true;
            dial.DefaultExt = ".config";
            dial.Filter = @"Config Files(*.config) | *.config | Masks (*.mask) | *.mask ";

            if (dial.ShowDialog(this) != DialogResult.OK) return;

            StreamWriter stream = new StreamWriter(dial.FileName);

            if (Path.GetExtension(dial.FileName) == ".config")
            {
                stream.WriteLine($"history = {historyCount}");
                stream.WriteLine($"penalty = {penaltyValue}");

                stream.WriteLine($"max_size = {Math.Round(blobUpperBound.Value)}");
                stream.WriteLine($"min_size = {Math.Round(blobLowerBound.Value)}");

                stream.WriteLine($"height = {Math.Round(maskHeight.Value)}");
                stream.WriteLine($"width = {Math.Round(maskWidth.Value)}");

                stream.WriteLine($"invert_colors = {invertCheckBox.Checked}");
            }
            else
                stream.WriteLine($"clean = true");

            foreach (var mask in masks)
                stream.WriteLine(mask.PrintOut());

            //TODO: output & highlight

            //initOutputFormat(parameters);
            //initHighlight(parameters);

            stream.Close();
        }

        private void MainForm_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Link;
        }
        #endregion

        #region Output
        private MyStream Output;

        private Constants.OutputFormat outputFormat;
        private Constants.HighlightFormat highlightFormat;

        void highlightBlobs(Bitmap image)
        {
            VideoBox.BackgroundImage = image;
            VideoBox.crossThreadOperation(() => VideoBox.Image = new Bitmap(image.Width, image.Height));

            Graphics gr = Graphics.FromImage(VideoBox.Image);

            foreach (var blobKeeper in blobKeepers)
                blobKeeper.Draw(gr, highlightFormat);

            VideoBox.crossThreadOperation(() => Refresh());
        }


        void printOut(int frame)
        {
            if (Output == null) return;

            Output.Write($"{frame}{Constants.Delimeter}");

            foreach (var blobKeeper in blobKeepers)
                blobKeeper.PrintOut(Output, outputFormat);

            Output.WriteLine();
        }

        private void printOutHeader()
        {
            if (Output == null) return;

            Output.StartAppending();

            Output.Write($"{Constants.Delimeter}");

            foreach (var blobKeeper in blobKeepers)
                blobKeeper.PrintOutTag(Output, outputFormat);

            Output.WriteLine();

            foreach (var blobKeeper in blobKeepers)
                blobKeeper.PrintOutHeader(Output, outputFormat);

            Output.WriteLine();

            Output.Flush();

            Output.StopAppending();
            Output.Close();

        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Output != null) Output.Close();
        }
        #endregion

        #region FlowControl
        void refreshActualFrame()
        {
            if (pictureGiver != null)            
                proccesFrame(pictureGiver[actualPicture], Extensions.RefreshBlobKeeper);            
            else
                disableFlowControl();
        }

        void proccesFrame(Bitmap image, Action<IKeeper<Blob, double, double, AForge.Point>, IEnumerable<Blob>> action)
        {
            Bitmap rawImage = image;
            Bitmap processedImage = processImage(rawImage);

            IEnumerable<Tuple<string, Blob>> blobsWithTags = blobCounter.CountItems(processedImage);

            blobKeepers.ForEach(blobKeeper => action(blobKeeper, from blobWithTag in blobsWithTags
                                                                 where blobKeeper.Tag == blobWithTag.Item1
                                                                 select blobWithTag.Item2));

            DisplayControl.crossThreadOperation(() =>
                {
                    switch ((string)DisplayControl.SelectedItem)
                    {
                        case "Raw image":
                            highlightBlobs(rawImage);
                            break;
                        case "Processed":
                            highlightBlobs(processedImage);
                            break;
                        case "None":
                        default:
                            break;
                    }
                });

        }

        Bitmap processImage(Bitmap image) => pictureProcessor.processItem(image);

        void enableAll() => switchEnability(true);
        void disableAll() => switchEnability(false);

        void switchEnability(bool enabled)
        {
            foreach (Control control in maskGroupBox.Controls)
                control.Enabled = enabled;

            foreach (Control control in controlGroupBox.Controls)
                control.Enabled = enabled;
            
            setFlowControl(enabled);

            StopButton.Enabled = true;
            StartPauseButton.Enabled = true;
        }

        private void Stop()
        {
            StartPauseButton.Text = "Start";
            actualState = state.stoped;            
            
            printOutHeader();

            //TODO: think about the consequences
            pictureGiver.Restart();
            adjustSliders();

            enableAll();
        }

        private void Pause()
        {
            StartPauseButton.Text = "Start";
            actualState = state.paused;

            if (Output == null) return;

            Output.Flush();
        }

        private void Start()
        {
            StartPauseButton.Text = "Pause";

            //When the state is Running nothing ought to happen
            //When the state is Paused then the Task is actively waiting to be running again
            if (actualState == state.stoped)
                RunAnalysis();

            actualState = state.runing;
            disableAll();
        }

        private enum state { stoped, runing, paused }
        state actualState;
        private void PauseStartButton_Click(object sender, EventArgs e)
        {
            switch (actualState)
            {
                case state.stoped:
                case state.paused:
                    Start();
                    break;
                case state.runing:
                    Pause();
                    break;
                default:
                    break;
            }
        }
        private void StopButton_Click(object sender, EventArgs e)
        {
            switch (actualState)
            {
                case state.runing:
                case state.paused:
                    Stop();
                    break;
                case state.stoped:
                default:
                    break;
            }
        }

        private void RunAnalysis()
        {
            actualState = state.runing;

            new Task(() =>
            {
                foreach (Bitmap picture in pictureGiver)
                {
                    if (actualState == state.stoped)
                        break;

                    //TODO: pausing handling - something better than active waiting for the switch of state.
                    //Althought there is only one Task with the code, so the active waiting in paused state is not so big overhead...
                    while (actualState == state.paused) ;

                    proccesFrame(picture, Extensions.ActualizeBlobKeeper);

                    printOut(pictureGiver.ActualIndex);
                    videoSlider.crossThreadOperation(() => adjustSliders());
                }

                this.crossThreadOperation(()=> Stop());
            }).Start();
        }
        #endregion

        #region VideoControl
        void enableFlowControl() => setFlowControl(true);
        void disableFlowControl() => setFlowControl(false);
        void setFlowControl(bool enabled)
        {
            foreach (Control control in videoGroupBox.Controls)
                control.Enabled = enabled;

            runAnalysisFrom.Enabled = !beginingBias.Checked;
            runAnalysisTo.Enabled = !endingBias.Checked;
        }
        
        int actualPicture
        {
            get { return pictureGiver.ActualIndex; }
            set
            {
                pictureGiver.ActualIndex = Math.Min(pictureGiver.RunTo, Math.Max(pictureGiver.RunFrom, value));

                actualIndex.Value = pictureGiver.ActualIndex;
                videoSlider.crossThreadOperation(() => adjustSliders());
            }
        }

        private void videoSlider_Scroll(object sender, EventArgs e)
            =>actualPicture = MathFunctions.PercentToValue(pictureGiver.RunFrom, pictureGiver.RunTo, videoSlider.Value);            
        

        private void pictureSelected(object sender, EventArgs e)=>actualPicture = (int)actualIndex.Value;                    

        private void beginingBias_CheckedChanged(object sender, EventArgs e)
        {
            runAnalysisFrom.Enabled = !beginingBias.Checked;            
            runAnalysisFrom.Value = runAnalysisFrom.Minimum;

            adjustSliders();
        }

        private void endingBias_CheckedChanged(object sender, EventArgs e)
        {
            runAnalysisTo.Enabled = !endingBias.Checked;
            runAnalysisTo.Value = runAnalysisTo.Maximum;

            adjustSliders();
        }

        private void runAnalysisFrom_ValueChanged(object sender, EventArgs e)
        {            
            pictureGiver.RunFrom = (int)runAnalysisFrom.Value;
            actualPicture = Math.Max(pictureGiver.RunFrom, actualPicture);            
        }

        private void runAnalysisTo_ValueChanged(object sender, EventArgs e)
        {
            pictureGiver.RunTo = (int)runAnalysisTo.Value;
            actualPicture = Math.Min(actualPicture, pictureGiver.RunTo);
        }

        private void adjustSliders()
        {
            checkForOverlap();

            videoSlider.Value = (int)Math.Floor(MathFunctions.ValueToPercent(pictureGiver.RunFrom, pictureGiver.RunTo, actualPicture));
            refreshActualFrame();
        }

        //Should be obsollete now
        private void checkForOverlap()
        {
            if ((int)runAnalysisTo.Value > pictureGiver.RunTo)
                runAnalysisTo.Value = pictureGiver.RunTo;

            if ((int)runAnalysisFrom.Value > pictureGiver.RunTo)
                runAnalysisFrom.Value = pictureGiver.RunTo;

            if (runAnalysisTo.Value < runAnalysisFrom.Value)
                runAnalysisTo.Value = runAnalysisFrom.Value;
        }


        private void step_ValueChanged(object sender, EventArgs e)
        {
            pictureGiver.Step = (int)step.Value; 
            actualIndex.Increment = step.Value;
        }

        private void parametrsValueChanged(object sender, EventArgs e) => refreshActualFrame();

        #endregion

        #region Masking

        private static Pen maskHighliter = new Pen(Color.Red, 4);

        enum DrawStyle { None, Ellipse, Rectangle, Curve };
        DrawStyle actualStyle = DrawStyle.None;

        string maskTag { get { return maskTagBox.Text; } }
        RectangleF drawRectangle;

        float halfWidth { get { return (float)maskWidth.Value; } set { maskWidth.Value = (decimal)value; } }
        float halfHeight { get { return (float)maskHeight.Value; } set { maskHeight.Value = (decimal)value; } }
        float wheelCorrection = 1200;

        delegate void drawEvent();

        bool isOkayToDraw() => pictureGiver != null && actualStyle != DrawStyle.None;

        private void addMask(DrawStyle style, RectangleF percentRectangle, string tag)
        {
            IMask<AForge.Point> mask = null;
            RectangleF realRectangle = MathFunctions.recalculateRectangle(percentRectangle, VideoBox.Image.Size);

            switch (style)
            {
                case DrawStyle.Curve:
                    //TODO: implement curve function
                    //containFunction = ???
                    break;
                case DrawStyle.Ellipse:
                    mask = new EllipMask(realRectangle, percentRectangle, tag);
                    break;
                case DrawStyle.Rectangle:
                    mask = new RectMask(realRectangle, percentRectangle, tag);
                    break;
                //because of 'isOkayToDraw' testing in 'maskingEventHanlder', this part of switch ought to never occur,
                //therefore 'containFunction' should be assigned properly
                case DrawStyle.None:
                default:
                    break;
            }

            maskControlContainer.Items.Add(mask);
        }

        private void maskingEventHandler(drawEvent ev)
        {
            if (isOkayToDraw())
            {
                ev();

                Refresh();
            }
        }

        private void Draw(object sender, MouseEventArgs e) => maskingEventHandler(() => drawRectangle = MathFunctions.getRectangleFromRadius(e.Location, halfWidth, halfHeight));

        private void ChangeDimensions(object sender, MouseEventArgs e)
        {
            maskingEventHandler(() =>
            {
                halfWidth += halfWidth * e.Delta / wheelCorrection;

                halfWidth =
                    Math.Min(
                        Math.Max(
                            halfWidth, 1),
                        VideoBox.Width / 2);

                halfHeight += halfHeight * e.Delta / wheelCorrection;

                halfHeight =
                    Math.Min(
                        Math.Max(
                            halfHeight, 1),
                        VideoBox.Height / 2);
            });
        }

        private void removeKeepers()
        {
            blobKeepers.RemoveAll(keeper =>
            {
                foreach (IMask<AForge.Point> mask in maskControlContainer.Items)
                    if (mask.Tag == keeper.Tag)
                        return false;

                return true;
            });

        }
        private void removeMasks(object sender, EventArgs e)
        {
            for (int i = maskControlContainer.SelectedItems.Count - 1; i >= 0; i--)
            {
                maskControlContainer.Items.Remove(maskControlContainer.SelectedItems[i]);

            }

            removeKeepers();

            refreshActualFrame();
        }

        private void applyMask(object sender, MouseEventArgs e)
        {
            maskingEventHandler(() =>
            {
                addMask(actualStyle, MathFunctions.getPercentRectangle(drawRectangle, VideoBox.Size), maskTag);

                if (!blobKeepers.Any(keeper => keeper.Tag == maskTag))
                    blobKeepers.Add(new BlobKeeperAssignment(maskTag, historyCount, penaltyValue));

                refreshActualFrame();
            });

        }

        private void VideoBox_Paint(object sender, PaintEventArgs e)
        {
            foreach (IMask<AForge.Point> mask in maskControlContainer.SelectedItems)
                mask.DrawMask(e.Graphics);

            switch (actualStyle)
            {
                case DrawStyle.Curve:
                    //TODO: curve doesn't work
                    //e.Graphics.DrawClosedCurve(maskHighliter, Points);
                    break;
                case DrawStyle.Ellipse:
                    e.Graphics.DrawEllipse(maskHighliter, drawRectangle);
                    break;
                case DrawStyle.Rectangle:
                    e.Graphics.DrawRectangle(maskHighliter, drawRectangle.Round());
                    break;
                case DrawStyle.None:
                default:
                    break;
            }
        }

        private void StopMasking(object sender, EventArgs e)
        {
            actualStyle = DrawStyle.None;
            Refresh();
        }

        private void StartDrawEllipse(object sender, EventArgs e) => actualStyle = DrawStyle.Ellipse;
        private void StartDrawRectangle(object sender, EventArgs e) => actualStyle = DrawStyle.Rectangle;
        //TODO: curve
        private void StartDrawCurve(object sender, EventArgs e) => actualStyle = DrawStyle.Curve;

        private void displayMask(object sender, EventArgs e) => Refresh();

        #endregion

    }
}