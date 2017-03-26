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
        private ICounter<Bitmap, Blob> blobCounter;
        private List<IKeeper<Blob, double, double>> blobKeepers;
        //private IKeeper<Blob, double, double> blobKeeper;

        internal bool shouldInvert { get { return invertCheckBox.Checked; } }
        internal int upperBoundValue { get { return (int)blobUpperBound.Value; } }
        internal int lowerBoundValue { get { return (int)blobLowerBound.Value; } }

        private Pen blobHighliter = new Pen(Color.Coral, 5);
        private Pen maskHighliter = new Pen(Color.Red, 4);

        private void debugInit()
        {
            pictureGiver = new SeparatePhotoGiver("Untitled.avi_", "D:\\Users\\Martin_Sery\\Documents\\Work\\Natočená videa", "jpg", 5, 600);
            pictureProcessor = new BitmapPreProcessor(this);
            blobCounter = new PictureBlobCounter(this);
            blobKeepers = new List<IKeeper<Blob, double, double>>();

            refreshActualFrame();
        }

        public MainForm()
        {
            InitializeComponent();

            //TODO: make init
            debugInit();
        }

        void refreshActualFrame()
        {
            proccesFrame(pictureGiver[actualPicture], Extensions.RefreshBlobKeeper);
        }

        void proccesFrame(Bitmap image, Action<IKeeper<Blob, double, double>, IEnumerable<Blob>> action)
        {
            Bitmap rawImage = image;
            Bitmap processedImage = processImage(rawImage);

            IEnumerable<Tuple<string, Blob>> blobsWithTags = blobCounter.CountItems(processedImage);

            blobKeepers.ForEach(blobKeeper => action(blobKeeper, from blobWithTag in blobsWithTags
                                                                 where blobKeeper.Tag == blobWithTag.Item1
                                                                 select blobWithTag.Item2));

            highlightFlies(VideoBox_processedPicture, processedImage);
            highlightFlies(VideoBox_staticPicture, rawImage);
        }

        Bitmap processImage(Bitmap image)
        {
            return pictureProcessor.processImage(image);
        }

        void highlightFlies(PictureBox pictureBox, Bitmap image)
        {
            pictureBox.BackgroundImage = image;
            pictureBox.crossThreadOperation(() => pictureBox.Image = new Bitmap(image.Width, image.Height));
            //pictureBox.Image = new Bitmap(image.Width, image.Height);
            Graphics gr = Graphics.FromImage(pictureBox.Image);

            foreach (var blobKeeper in blobKeepers)
                foreach (var blob in blobKeeper.ItemsData)
                    blob.Draw(gr);

            pictureBox.crossThreadOperation(() => Refresh());
        }

        bool running = false;
        private void StopStartButton_Click(object sender, EventArgs e)
        {
            if (running)
            {
                Console.WriteLine("Stoping");
                StopStartButton.Text = "Start";
            }
            else
            {
                Console.WriteLine("Starting");
                StopStartButton.Text = "Stop";
                RunAnalysis();
                Console.WriteLine("Running");
            }

            running = !running;
        }

        private void RunAnalysis()
        {
            new Task(() =>
            {
                foreach (Bitmap picture in pictureGiver)
                {
                    if (running) proccesFrame(picture, Extensions.ActualizeBlobKeeper);
                    else break;
                }

                Console.WriteLine("Done");
            }).Start();
        }

        private void refreshActualFrame(object sender, EventArgs e)
        {
            Console.WriteLine("Refreshing...");
            refreshActualFrame();
        }

        int actualPicture;
        private void videoSlider_Scroll(object sender, EventArgs e)
        {
            actualPicture = MathFunctions.getActual(pictureGiver.RunFrom, pictureGiver.RunTo, videoSlider.Value);
            actualIndex.Value = actualPicture;

            adjustSliders();
            refreshActualFrame();
        }

        private void pictureSelected(object sender, EventArgs e)
        {
            actualPicture = (int)actualIndex.Value;

            if (actualPicture > pictureGiver.RunTo)
            {
                actualPicture = pictureGiver.RunTo;
                actualIndex.Value = actualPicture;
            }

            adjustSliders();
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

        private void maskingEventHandler(drawEvent ev)
        {
            if (isOkayToDraw())
            {
                ev();
                
                Refresh();
            }
        }

        private void Draw(object sender, MouseEventArgs e)
        {
            maskingEventHandler(() => drawRectangle = MathFunctions.getRectangleFromRadius(e.Location, halfWidth, halfHeight));
        }

        private void ChangeDimensions(object sender, MouseEventArgs e)
        {
            maskingEventHandler(() =>
            {
                halfWidth += halfWidth * e.Delta / wheelCorrection;

                halfWidth =
                    Math.Min(
                        Math.Max(
                            halfWidth, 1),
                        VideoBox_processedPicture.Width / 2);

                halfHeight += halfHeight * e.Delta / wheelCorrection;
                
                halfHeight =
                    Math.Min(
                        Math.Max(
                            halfHeight, 1),
                        VideoBox_processedPicture.Height / 2);
            });
        }

        private void applyMask(object sender, MouseEventArgs e)
        {
            maskingEventHandler(() =>
            {
                CurveMask mask = null;
                Rectangle boundaryRectangle = MathFunctions.recalculateRectangle(drawRectangle, VideoBox_staticPicture.Width, VideoBox_staticPicture.Height, VideoBox_staticPicture.Image.Width, VideoBox_staticPicture.Image.Height);

                //Debug: rectangle draw around mask that has just been addeded
                Graphics gr = Graphics.FromImage(VideoBox_processedPicture.Image);
                gr.DrawRectangle(Pens.BurlyWood, boundaryRectangle);
                gr.Dispose();
                Refresh();

                switch (actualStyle)
                {
                    case DrawStyle.Curve:
                        //TODO: implement curve function
                        //containFunction = ???
                        break;
                    case DrawStyle.Ellipse:
                        mask = new EllipMask(boundaryRectangle, drawRectangle, maskTag);
                        break;
                    case DrawStyle.Rectangle:
                        mask = new RectMask(boundaryRectangle, drawRectangle, maskTag);
                        break;
                    //because of 'isOkayToDraw' testing, this part of switch ought to never occur,
                    //therefore 'containFunction' should be assigned properly
                    case DrawStyle.None:
                    default:
                        break;
                }

                blobCounter.Masks.Add(mask);
                maskControlContainer.Items.Add(mask);

                Bitmap rawImage = pictureGiver[actualPicture];
                Bitmap processedImage = processImage(rawImage);

                IEnumerable<Tuple<string, Blob>> blobsWithTags = blobCounter.CountItems(processedImage);

                blobKeepers.Add(
                    new BlobKeeper(
                        from blobWithTag in blobsWithTags where maskTag == blobWithTag.Item1 select blobWithTag.Item2,
                        maskTag,
                        mask.isIn));

                refreshActualFrame();
            });

        }

        private void VideoBox_Paint(object sender, PaintEventArgs e)
        {
            foreach (var mask in maskControlContainer.SelectedItems)
                ((CurveMask)mask).drawMask(e.Graphics);

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

        private void StartDrawEllipse(object sender, EventArgs e)
        {
            actualStyle = DrawStyle.Ellipse;
        }

        private void StartDrawRectangle(object sender, EventArgs e)
        {
            actualStyle = DrawStyle.Rectangle;
        }

        //TODO: curve
        private void StartDrawCurve(object sender, EventArgs e)
        {
            actualStyle = DrawStyle.Curve;
        }

        private void displayMask(object sender, EventArgs e)
        {
            Refresh();            
        }

        #endregion
    }
}
