using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

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
        private MaskContainer maskContainer;

        internal bool shouldInvert { get { return invertCheckBox.Checked; } }
        internal int upperBoundValue { get { return (int)upperBound.Value; } }
        internal int lowerBoundValue { get { return (int)lowerBound.Value; } }
        internal byte[,] mask { get { return maskContainer.maskArray; } }
        
        private Pen blobHighliter = new Pen(Color.Coral, 5);
        private Pen maskHighliter = new Pen(Color.Red, 4);

        //internal int width { get { return VideoBox_staticPicture.Width; } }
        //internal int height { get { return VideoBox_staticPicture.Height; } }

        private void debugInit()
        {
            pictureGiver = new SeparatePhotoGiver("Untitled.avi_", "D:\\Users\\Martin_Sery\\Documents\\Work\\Natočená videa", "jpg", 5, 600);
            pictureProcessor = new BitmapPreProcessor(this);
            blobCounter = new PictureBlobCounter(this);
            maskContainer = new MaskContainer(pictureGiver.First.Width, pictureGiver.First.Height, 255);
            
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
            refreshFrame(pictureGiver[actualPicture]);
        }

        void refreshFrame(Bitmap image)
        {
            Bitmap rawIamge = image;
            Bitmap processedImage = processImage(rawIamge);

            ICollection<Blob> blobs = blobCounter.CountItems(processedImage);

            highlightFlies(VideoBox_processedPicture, processedImage, blobs);
            highlightFlies(VideoBox_staticPicture, rawIamge, blobs);

            VideoBox_processedPicture.crossThreadOperation(() => Refresh());
            VideoBox_staticPicture.crossThreadOperation(() => Refresh());
        }

        Bitmap processImage(Bitmap image)
        {
            return pictureProcessor.processImage(image);
        }

        void highlightFlies(PictureBox pictureBox, Bitmap image, ICollection<Blob> rects)
        {
            pictureBox.BackgroundImage = image;
            pictureBox.Image = new Bitmap(image.Width, image.Height);
            Graphics gr = Graphics.FromImage(pictureBox.Image);

            foreach (var rect in rects)
                gr.DrawEllipse(blobHighliter, rect.Rectangle);

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
                    if (running) refreshFrame(picture);
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


        enum DrawStyle { None, Ellipse, Rectangle, Curve };
        DrawStyle actualStyle = DrawStyle.None;
        bool validPoint = false;
        int tmpX, tmpY;
        Rectangle drawRectangle;

        private void applyMaskToolStripMenuItem_Click(object sender, EventArgs e)
        {
            switch (actualStyle)
            {
                case DrawStyle.Curve:
                    //TODO: curve doesn't work well
                    //e.Graphics.DrawClosedCurve(maskHighliter, Points);
                    break;
                case DrawStyle.Ellipse:
                    //Ellipse doesn't work with mask container
                    break;
                case DrawStyle.Rectangle:
                    maskContainer += drawRectangle;
                    break;
                case DrawStyle.None:
                default:
                    break;
            }

            drawRectangle = new Rectangle();
            refreshActualFrame();
        }

        private void Draw(object sender, MouseEventArgs e)
        {
            if (validPoint)
            {
                drawRectangle = MathFunctions.getRectangle(tmpX, tmpY, e.X, e.Y);
                Refresh();
            }
        }

        private void DrawNewPoint(object sender, MouseEventArgs e)
        {
            tmpX = e.X;
            tmpY = e.Y;
            validPoint = true;
        }

        private void DrawEnd(object sender, MouseEventArgs e)
        {
            validPoint = false;
            //points.Add(new System.Drawing.Point(e.X, e.Y));
        }

        private void VideoBox_Paint(object sender, PaintEventArgs e)
        {
            switch (actualStyle)
            {
                case DrawStyle.Curve:
                    //TODO: curve doesn't work well
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

        private void StartDrawEllipse(object sender, EventArgs e)
        {
            actualStyle = DrawStyle.Ellipse;
        }

        private void StartDrawRectangle(object sender, EventArgs e)
        {
            actualStyle = DrawStyle.Rectangle;
        }

        //Todo: curve
        private void StartDrawCurve(object sender, EventArgs e)
        {
            actualStyle = DrawStyle.Curve;
        }
    }
}
