using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Linq;

using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Threading;

using AForge.Video;

using AForge.Imaging;
using AForge.Imaging.Filters;
using AForge.Math.Geometry;

using AForge;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;


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

            //TODO: make init
            debugInit();
        }

        #region Init

        private void debugInit()
        {
            pictureGiver = new SeparatePhotoGiver("Untitled.avi_", "D:\\Users\\Martin_Sery\\Documents\\Work\\Natočená videa", ".jpg", 5, 600);
            pictureProcessor = new BitmapPreProcessor(this);
            blobCounter = new PictureBlobCounter(this);
            blobKeepers = new List<IKeeper<Blob, double, double, AForge.Point>>();

            openDialog.InitialDirectory = "c:\\";
            openDialog.Filter = Constants.FileExtensions;
            openDialog.FilterIndex = 2;
            openDialog.RestoreDirectory = true;

            initParams(parameters: null);

            refreshActualFrame();
        }

        T getParameter<T>(string parameter, ILookup<string, string> parameters, T def, Extensions.Transform<T, string> transform)
            => parameters != null && parameters.Contains(parameter) ? transform(parameters[parameter].First()) : def;

        private ILookup<string, string> getParameters(string path) => (from line in File.ReadLines(path) where line.Contains("=") select line.Split('='))
                                                                      .ToLookup(parsedLine => parsedLine[0].Trim(), s => s[1].Trim());

        private void initPictureGiverWithSeparatePhotoGiver(string path) => pictureGiver = new SeparatePhotoGiver(path);
        private void initPictureGiverWithAVIGiver(string path) => pictureGiver = new VideoAVIGiver(path);
        private void initPictureGiverWithMPEGGiver(string path) => pictureGiver = new VideoMPEGGiver(path);

        private void initParams(string path) => initParams(getParameters(path));
        private void initParams(ILookup<string, string> parameters)
        {
            historyCount = getParameter("history", parameters, 10, int.Parse);
            penaltyValue = getParameter("penalty", parameters, 10, int.Parse);

            blobUpperBound.Value = getParameter("max_size", parameters, 10, int.Parse);
            blobLowerBound.Value = getParameter("min_size", parameters, 5, int.Parse);

            maskHeight.Value = getParameter("height", parameters, 10, int.Parse);
            maskWidth.Value = getParameter("width", parameters, 10, int.Parse);

            invertCheckBox.Checked = getParameter("invert_colors", parameters, true, bool.Parse);

            initMask(parameters);
            initOutputFormat(parameters);
            initHighlight(parameters);
        }

        private void initOutputFormat(string path) => initOutputFormat(getParameters(path));
        private void initOutputFormat(ILookup<string, string> parameters)
        {
            outputFormat = Constants.OutputFormat.None;

            foreach (var tag in Constants.OutputTag)
                outputFormat |= (getParameter(tag.Value, parameters, true, bool.Parse) ? tag.Key : Constants.OutputFormat.None);
        }

        private void initHighlight(string path) => initHighlight(getParameters(path));
        private void initHighlight(ILookup<string, string> parameters)
        {
            highlightFormat = Constants.HighlightFormat.None;

            foreach (var tag in Constants.HighlightTag)
                highlightFormat |= (getParameter(tag.Value, parameters, false, bool.Parse) ? tag.Key : Constants.HighlightFormat.None);

            //Special case which default value should be true, not false like with the others
            highlightFormat |= (getParameter(Constants.HighlightTag[Constants.HighlightFormat.Object], parameters, true, bool.Parse) ? Constants.HighlightFormat.Object : Constants.HighlightFormat.None);
        }

        private void initMasks(string path) => initMask(getParameters(path));
        private void initMask(ILookup<string, string> parameters)
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

            Output = new StreamWriter(path);
        }

        private void initState(string path)
        {
            if (ActualState != null)
                ActualState.Close();

            //TODO: copy previous state
            ActualState = new StreamReader(path);
        }

        private void InitParams(string path)
        {
            try
            {
                switch (Path.GetExtension(path))
                {
                    case ".config":
                        initParams(path);
                        break;
                    case ".mask":
                        initMasks(path);
                        break;
                    case ".output":
                        initOutputFormat(path);
                        break;
                    case ".csv":
                        initOutput(path);
                        break;
                    case ".state":
                        initState(path);
                        break;
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
            }
            catch (VideoException ex)
            {
                MessageBox.Show(ex.Message, "Failed loading video", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DragDroped(object sender, DragEventArgs e)
        {
            string[] fileList = (string[])e.Data.GetData(DataFormats.FileDrop, false);

            foreach (var path in fileList)
                InitParams(path);

            refreshActualFrame();
        }

        private void openFile(object sender, EventArgs e)
        {
            if (MainForm.openDialog.ShowDialog(this) == DialogResult.OK)
                foreach (var path in openDialog.FileNames)
                    InitParams(path);

        }

        private void saveState(object sender, EventArgs e)
        {

        }

        private void MainForm_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Link;
        }
        #endregion

        #region Output
        private StreamReader ActualState;

        private StreamWriter Output;
        private FileStream tmpFile;

        private Constants.OutputFormat outputFormat;
        private Constants.HighlightFormat highlightFormat;

        void highlightBlobs(Bitmap image)
        {
            VideoBox.BackgroundImage = image;
            VideoBox.crossThreadOperation(() => VideoBox.Image = new Bitmap(image.Width, image.Height));

            Graphics gr = Graphics.FromImage(VideoBox.Image);

            foreach (var blobKeeper in blobKeepers)
                blobKeeper.Draw(gr, highlightFormat);

            //TODO: implement more carefully
            if (blobKeepers.Count > 0 && blobKeepers.First().ItemsData.Count > 0)
                highlightBlobPictureBox.Image = blobKeepers.First().ItemsData.First().First.Image.ToManagedImage();

            highlightBlobPictureBox.crossThreadOperation(() => Refresh());
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

            Output.Write($"{Constants.Delimeter}");

            foreach (var blobKeeper in blobKeepers)
                blobKeeper.PrintOutTag(Output, outputFormat);

            Output.WriteLine();

            foreach (var blobKeeper in blobKeepers)
                blobKeeper.PrintOutHeader(Output, outputFormat);

            Output.WriteLine();
        }
        #endregion

        #region FlowControl
        void refreshActualFrame() => proccesFrame(pictureGiver[actualPicture], Extensions.RefreshBlobKeeper);

        void proccesFrame(Bitmap image, Action<IKeeper<Blob, double, double, AForge.Point>, IEnumerable<Blob>> action)
        {
            Bitmap rawImage = image;
            Bitmap processedImage = processImage(rawImage);

            IEnumerable<Tuple<string, Blob>> blobsWithTags = blobCounter.CountItems(processedImage);

            blobKeepers.ForEach(blobKeeper => action(blobKeeper, from blobWithTag in blobsWithTags
                                                                 where blobKeeper.Tag == blobWithTag.Item1
                                                                 select blobWithTag.Item2));

            //TOOD: make only one videobox, where proccessed & raw image & introduce switching between them

            if (DisplayOriginal.Checked)
                highlightBlobs(rawImage);
            else
                highlightBlobs(processedImage);
        }

        Bitmap processImage(Bitmap image) => pictureProcessor.processItem(image);

        private void Stop()
        {
            Console.WriteLine("Stoping");
            StartPauseButton.Text = "Start";
            actualState = state.stoped;
            actualPicture = pictureGiver.RunFrom;

            printOutHeader();

            if (Output == null) return;

            Output.Flush();
            Output.Close();

            refreshActualFrame();
        }

        private void Pause()
        {
            Console.WriteLine("Stoping");
            StartPauseButton.Text = "Start";
            actualState = state.paused;

            if (Output == null) return;

            Output.Flush();
        }

        private void Start()
        {
            Console.WriteLine("Starting");
            StartPauseButton.Text = "Pause";
            if (actualState == state.stoped)
                RunAnalysis();

            actualState = state.runing;
            Console.WriteLine("Running");
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
                    switch (actualState)
                    {
                        case state.runing:
                            proccesFrame(picture, Extensions.ActualizeBlobKeeper);

                            printOut(pictureGiver.ActualIndex);
                            videoSlider.crossThreadOperation(() => adjustSliders());

                            break;
                        case state.stoped:
                        case state.paused:
                        //TODO: pausing handling - something better than active waiting for the switch of state.
                        //Althought there is only one Task with the code, so the active waiting in paused state is not so big overhead...
                        default:
                            break;
                    }

                    if (actualState == state.stoped)
                        break;
                }

                Console.WriteLine("Done");
            }).Start();
        }

        private void refreshActualFrame(object sender, EventArgs e)
        {
            Console.WriteLine("Refreshing...");
            refreshActualFrame();
        }
        #endregion

        #region VideoControl
        int actualPicture
        {
            get { return pictureGiver.ActualIndex; }
            set
            {
                if (value > pictureGiver.RunTo)
                {
                    pictureGiver.ActualIndex = pictureGiver.RunTo;
                    actualIndex.Value = actualPicture;
                }
                else
                    pictureGiver.ActualIndex = value;

                videoSlider.crossThreadOperation(() => adjustSliders());
            }
        }
        private void videoSlider_Scroll(object sender, EventArgs e)
        {
            actualPicture = MathFunctions.PercentToValue(pictureGiver.RunFrom, pictureGiver.RunTo, videoSlider.Value);
            actualIndex.Value = actualPicture;

            refreshActualFrame();
        }

        private void pictureSelected(object sender, EventArgs e)
        {
            actualPicture = (int)actualIndex.Value;
            refreshActualFrame();
        }

        private void beginingBias_CheckedChanged(object sender, EventArgs e)
        {
            runAnalysisFrom.Enabled = !beginingBias.Checked;
            runAnalysisFrom.Value = 0;

            adjustSliders();
        }

        private void endingBias_CheckedChanged(object sender, EventArgs e)
        {
            runAnalysisTo.Enabled = !endingBias.Checked;
            runAnalysisTo.Value = pictureGiver.RunToMax;

            adjustSliders();
        }

        private void runAnalysisFrom_ValueChanged(object sender, EventArgs e)
        {
            pictureGiver.RunFrom = (int)runAnalysisFrom.Value;
            adjustSliders();
        }

        private void runAnalysisTo_ValueChanged(object sender, EventArgs e)
        {
            pictureGiver.RunTo = (int)runAnalysisTo.Value;
            adjustSliders();
        }

        private void adjustSliders()
        {
            videoSlider.Value = (int)Math.Floor(MathFunctions.ValueToPercent(pictureGiver.RunFrom, pictureGiver.RunTo, actualPicture));

            checkForOverlap();
        }

        private void checkForOverlap()
        {
            if ((int)runAnalysisTo.Value > pictureGiver.RunTo)
                runAnalysisTo.Value = pictureGiver.RunTo;

            if ((int)runAnalysisFrom.Value > pictureGiver.RunTo)
                runAnalysisFrom.Value = pictureGiver.RunTo;

            if (runAnalysisTo.Value < runAnalysisFrom.Value)
                runAnalysisTo.Value = runAnalysisFrom.Value;
        }

        private void parametrsValueChanged(object sender, EventArgs e)
        {
            refreshActualFrame();
        }
        #endregion

        #region Masking

        private static Pen maskHighliter = new Pen(Color.Red, 4);
        enum DrawStyle { None, Ellipse, Rectangle, Curve };

        DrawStyle actualStyle = DrawStyle.None;
        bool isOkayToDraw() { return actualStyle != DrawStyle.None; }
        string maskTag { get { return maskTagBox.Text; } }
        RectangleF drawRectangle;

        float halfWidth { get { return (float)maskWidth.Value; } set { maskWidth.Value = (decimal)value; } }
        float halfHeight { get { return (float)maskHeight.Value; } set { maskHeight.Value = (decimal)value; } }
        float wheelCorrection = 1200;

        delegate void drawEvent();

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

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (ActualState != null) ActualState.Close();
            if (Output != null) Output.Close();
        }
    }
}