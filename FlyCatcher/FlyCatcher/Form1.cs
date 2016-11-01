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

        internal Color backgroundColor { get; private set; }
        internal Color flyColor { get; private set; }
        internal bool shouldInvert { get { return invertCheckBox.Checked; } }
        internal int upperBoundValue { get { return (int)upperBound.Value; } }
        internal int lowerBoundValue { get { return (int)lowerBound.Value; } }

        private Pen pen = new Pen(Color.Coral, 5);

        //internal int width { get { return VideoBox_staticPicture.Width; } }
        //internal int height { get { return VideoBox_staticPicture.Height; } }

        private void debugInit()
        {
            pictureGiver = new SeparatePhotoGiver("Untitled.avi_", "D:\\Users\\Martin_Sery\\Documents\\Work\\Natočená videa", "jpg", 5, 600);
            pictureProcessor = new BitmapPreProcessor(this);
            blobCounter = new PictureBlobCounter(this);

            refreshActualFrame();
        }

        public MainForm()
        {
            InitializeComponent();

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
                gr.DrawEllipse(pen, rect.Rectangle);

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

        private void VideoBox_MouseClick(object sender, MouseEventArgs e)
        {
            Console.WriteLine($"Mouse position: {e.Location.ToString()}");
            Color col = ((Bitmap)((PictureBox)sender).Image).GetPixel(e.X, e.Y);

            switch (e.Button)
            {
                case MouseButtons.Left:
                    Console.WriteLine($"Setting background color to: {col.ToString()}");
                    backgroundColor = col;
                    break;
                case MouseButtons.Right:
                    Console.WriteLine($"Setting fly color to: {col.ToString()}");
                    flyColor = col;
                    break;

                case MouseButtons.None:
                case MouseButtons.Middle:
                case MouseButtons.XButton1:
                case MouseButtons.XButton2:
                default:
                    break;
            }
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
            { actualPicture = pictureGiver.RunTo;
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
    }
}
