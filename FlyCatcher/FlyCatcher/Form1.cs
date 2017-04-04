﻿using System;
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
using AForge.Video.DirectShow;

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
        private IPictureGiver<Bitmap> pictureGiver;
        private IPicturePreProcessor<Bitmap> pictureProcessor;
        private ICounter<Bitmap, Blob, AForge.Point> blobCounter;
        private List<IKeeper<Blob, double, double>> blobKeepers;

        private StreamReader ActualState;

        private StreamWriter Output;
        private FileStream tmpFile;


        internal bool shouldInvert { get { return invertCheckBox.Checked; } }
        internal int upperBoundValue { get { return (int)blobUpperBound.Value; } }
        internal int lowerBoundValue { get { return (int)blobLowerBound.Value; } }

        private Pen blobHighliter = new Pen(Color.Coral, 5);
        private Pen maskHighliter = new Pen(Color.Red, 4);

        private int historyCount;

        private Constants.OutputFormat outputFormat;
        private Constants.HighlightFormat highlightFormat;

        private void debugInit()
        {
            pictureGiver = new SeparatePhotoGiver("Untitled.avi_", "D:\\Users\\Martin_Sery\\Documents\\Work\\Natočená videa", ".jpg", 5, 600);
            pictureProcessor = new BitmapPreProcessor(this);
            blobCounter = new PictureBlobCounter(this);
            blobKeepers = new List<IKeeper<Blob, double, double>>();

            initParams(parameters: null);

            refreshActualFrame();
        }

        T getParameter<T>(string parameter, ILookup<string, string> parameters, T def, Extensions.Transform<T, string> transform)
            => parameters != null && parameters.Contains(parameter) ? transform(parameters[parameter].First()) : def;

        private ILookup<string, string> getParameters(string path) => (from line in File.ReadLines(path) where line.Contains("=") select line.Split('='))
                                                                      .ToLookup(parsedLine => parsedLine[0].Trim(), s => s[1].Trim());

        private void initPictureGiverWithSeparatePhotoGiver(string path) => pictureGiver = new SeparatePhotoGiver(path);

        private void initParams(string path) => initParams(getParameters(path));
        private void initParams(ILookup<string, string> parameters)
        {
            historyCount = getParameter("history", parameters, 10, int.Parse);

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
                highlightFormat |= (getParameter(tag.Value, parameters, false, bool.Parse) ? tag.Key: Constants.HighlightFormat.None);

            //Special case which default value should be true, not false like with the others
            highlightFormat |= (getParameter(Constants.HighlightTag[Constants.HighlightFormat.Object], parameters, true, bool.Parse) ? Constants.HighlightFormat.Object : Constants.HighlightFormat.None);            
        }

        private void initMasks(string path) => initMask(getParameters(path));
        private void initMask(ILookup<string, string> parameters)
        {
            if (getParameter("clean", parameters, false, bool.Parse))
            {
                blobCounter.Masks.Clear();
                maskControlContainer.Items.Clear();
            }

            if (parameters != null)
                foreach (var param in parameters)
                {
                    if (param.Key == "ellipse" || param.Key == "rectangle")
                    {
                        foreach (var mask in param)
                        {
                            var pars = mask.Split(' ');
                            Rectangle rect = MathFunctions.getRectangle(int.Parse(pars[1]), int.Parse(pars[2]), int.Parse(pars[3]), int.Parse(pars[4]));
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
            if (ActualState!= null)            
                ActualState.Close();
            
            //TODO: copy previous state
            ActualState = new StreamReader(path);
        }

        public MainForm()
        {
            InitializeComponent();

            //TODO: make init
            debugInit();
        }

        void refreshActualFrame() => proccesFrame(pictureGiver[actualPicture], Extensions.RefreshBlobKeeper);

        void proccesFrame(Bitmap image, Action<IKeeper<Blob, double, double>, IEnumerable<Blob>> action)
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

        Bitmap processImage(Bitmap image) => pictureProcessor.processImage(image);

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

        private enum state { stoped, runing, paused}
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

                            printOut(pictureGiver.actualIndex);
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

        int actualPicture
        {
            get { return pictureGiver.actualIndex; }
            set {
                if (value > pictureGiver.RunTo)
                {
                    pictureGiver.actualIndex = pictureGiver.RunTo;
                    actualIndex.Value = actualPicture;
                }
                else
                    pictureGiver.actualIndex = value;

                videoSlider.crossThreadOperation(() => adjustSliders());
            }
        }
        private void videoSlider_Scroll(object sender, EventArgs e)
        {
            actualPicture = MathFunctions.getActual(pictureGiver.RunFrom, pictureGiver.RunTo, videoSlider.Value);
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
            videoSlider.Value = (int)Math.Floor(MathFunctions.getPercent(pictureGiver.RunFrom, pictureGiver.RunTo, actualPicture));

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

        #region Masking

        enum DrawStyle { None, Ellipse, Rectangle, Curve };
        DrawStyle actualStyle = DrawStyle.None;
        bool isOkayToDraw() { return actualStyle != DrawStyle.None; }
        string maskTag { get { return maskTagBox.Text; } }
        Rectangle drawRectangle;
        //TOOD: get radius or dimensionsfrom user
        float halfWidth { get { return (float)maskWidth.Value; } set { maskWidth.Value = (decimal)value; } }
        float halfHeight { get { return (float)maskHeight.Value; } set { maskHeight.Value = (decimal)value; } }
        float wheelCorrection = 1200;
        delegate void drawEvent();

        private void addMask(DrawStyle style, Rectangle rect, string tag)
        {
            IMask<AForge.Point> mask = null;
            Rectangle realRectangle = MathFunctions.recalculateRectangle(rect, VideoBox.Width, VideoBox.Height, VideoBox.Image.Width, VideoBox.Image.Height);

            switch (style)
            {
                case DrawStyle.Curve:
                    //TODO: implement curve function
                    //containFunction = ???
                    break;
                case DrawStyle.Ellipse:
                    mask = new EllipMask(realRectangle, rect, tag);
                    break;
                case DrawStyle.Rectangle:
                    mask = new RectMask(realRectangle, rect, tag);
                    break;
                //because of 'isOkayToDraw' testing in 'maskingEventHanlder', this part of switch ought to never occur,
                //therefore 'containFunction' should be assigned properly
                case DrawStyle.None:
                default:
                    break;
            }

            blobCounter.Masks.Add(mask);
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

        private void applyMask(object sender, MouseEventArgs e)
        {
            maskingEventHandler(() =>
            {
                addMask(actualStyle, drawRectangle, maskTag);
                
                //TOOD: high code duality with 'processActualFrame'
                Bitmap rawImage = pictureGiver[actualPicture];
                Bitmap processedImage = processImage(rawImage);

                IEnumerable<Tuple<string, Blob>> blobsWithTags = blobCounter.CountItems(processedImage);

                blobKeepers.Add(
                    new BlobKeeper(
                        from blobWithTag in blobsWithTags where maskTag == blobWithTag.Item1 select blobWithTag.Item2,
                        maskTag, historyCount));

                refreshActualFrame();
            });

        }

        private void VideoBox_Paint(object sender, PaintEventArgs e)
        {
            foreach (var mask in maskControlContainer.SelectedItems)
                ((IMask<AForge.Point>)mask).drawMask(e.Graphics);

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
                    e.Graphics.DrawRectangle(maskHighliter, drawRectangle);
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

        private void InitParams(object sender, DragEventArgs e)
        {
            string[] fileList = (string[])e.Data.GetData(DataFormats.FileDrop, false);

            foreach (var item in fileList)
            {
                switch (Path.GetExtension(item))
                {
                    case ".config":
                        initParams(item);
                        break;
                    case ".mask":
                        initMasks(item);
                        break;
                    case ".output":
                        initOutputFormat(item);
                        break;
                    case ".csv":
                        initOutput(item);
                        break;
                    case ".state":
                        initState(item);
                        break;
                    case ".jpg":
                    case ".png":
                    case ".bmp":
                        initPictureGiverWithSeparatePhotoGiver(item);
                        break;
                    default:
                        break;
                }

            }

            refreshActualFrame();
        }

        private void MainForm_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Link;
        }

        private void DisplayOriginal_CheckedChanged(object sender, EventArgs e)
        {
            refreshActualFrame();
        }

        //TODO: resize mask on videobox resize
    }
}