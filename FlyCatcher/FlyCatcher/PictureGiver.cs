using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Collections;
using System.IO;

using AForge.Video.FFMPEG;
using AForge.Video.VFW;
using AForge.Video;

namespace FlyCatcher
{
    interface IGiver<out T> : IEnumerable<T>, IDisposable
    {
        T First { get; }
        T Actual { get; }
        T Last { get; }

        T this[int index] { get; }

        bool isValidIndex(int index);

        void Restart();

        int ActualIndex { get; set; }

        int RunFrom { get; set; }
        int RunTo { get; set; }

        int RunFromMin { get; }
        int RunToMax { get; }
        int Step { get; set; }

        string Path { get; }
        string Tag { get; }
    }

    class SeparatePhotoGiver : IGiver<Bitmap>
    {
        string photoMask;
        string folder;
        string fileSuffix;
        int digitCount;

        public int ActualIndex { get; set; }
        public string Path { get; private set; }
        public string Tag { get { return photoMask; } }

        public Bitmap Actual
        { get { return this[ActualIndex]; } }

        public Bitmap First
        { get { return this[0]; } }

        public Bitmap Last
        { get { return this[RunToMax]; } }

        public Bitmap this[int index]
        { get { return new Bitmap(photoName(index)); } }//TODO: argument out of range exeption

        public int RunFrom
        { get; set; }

        public int RunTo
        { get; set; }

        //TODO: problem with undefined values
        public int RunToMax
        { get; private set; }
        public int RunFromMin
        { get; private set; }

        public int Step
        { get; set; }

        public bool isValidIndex(int index) => index >= 0 && index <= RunToMax;

        public void Restart() => ActualIndex = RunFromMin;

        private void init(string photoMask, string folder, string fileSuffix, int numbers, int last)
        {
            ActualIndex = 0;
            this.photoMask = photoMask;
            this.digitCount = numbers;
            this.fileSuffix = fileSuffix;

            this.folder = folder;

            RunToMax = last;
            RunFromMin = 0;
            RunFrom = 0;
            RunTo = RunToMax;
            Step = 1;
        }

        private string getMask(string file) => file.TrimEnd(Constants.Digits.ToCharArray());
        private string getLast(string file) => new string(file.Reverse().TakeWhile(char.IsDigit).Reverse().ToArray());

        /// <summary>
        /// Constructor for photo giver, that iterates over images in a folder.
        /// </summary>
        public SeparatePhotoGiver(string path)
        {
            Path = path;

            string str = getLast(System.IO.Path.GetFileNameWithoutExtension(path));

            init(getMask(System.IO.Path.GetFileNameWithoutExtension(path)),
                 System.IO.Path.GetDirectoryName(path),
                 System.IO.Path.GetExtension(path),
                 str.Length,
                 int.Parse(str)
                );
        }

        /// <summary>
        /// Constructor for photo giver, that iterates over images in a folder.
        /// </summary>
        /// <param name="photoMask"></param>
        /// <param name="folder"></param>
        /// <param name="fileSuffix"></param>
        /// <param name="digitCount"></param>
        /// <param name="last"></param>
        public SeparatePhotoGiver(string photoMask, string folder, string fileSuffix, int digitCount, int last)
        {
            Path = photoName(last);
            init(photoMask, folder, fileSuffix, digitCount, last);
        }

        private string photoName(int index) => $"{folder}\\{photoMask}{index.ToString($"D{digitCount}")}{fileSuffix}";

        private bool validPhoto
        { get { return File.Exists(photoName(ActualIndex)); } }

        public IEnumerator<Bitmap> GetEnumerator()
        {
            ActualIndex = RunFrom;
            while (validPhoto && ActualIndex <= RunTo)
            {
                yield return this[ActualIndex];
                ActualIndex += Step;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Dispose() { }
    }

    class VideoMPEGGiver : IGiver<Bitmap>
    {
        private VideoFileReader reader;
        private Bitmap preview;

        public string Path { get; private set; }
        public string Tag { get { return System.IO.Path.GetFileNameWithoutExtension(Path); } }

        public VideoMPEGGiver(string path)
        {
            Path = path;

            reader = new VideoFileReader();
            reader.Open(path);
            preview = reader.ReadVideoFrame();

            //This is used to reset the reader to the default position and not loosing the first frame
            Restart();

            Step = 1;
        }

        public bool isValidIndex(int index) => true;

        public void Restart()
        {
            reader.Close();
            reader.Open(Path);
        }

        public Bitmap this[int index]
        { get { return preview; } }

        public Bitmap Actual
        { get { return preview; } }

        public int ActualIndex
        { get { return 0; } set { } }

        public Bitmap First
        { get { return preview; } }

        public Bitmap Last
        { get { return preview; } }

        public int RunFrom
        { get { return 0; } set { } }

        public int RunTo
        { get { return 0; } set { } }

        public int RunToMax
        { get { return 0; } }
        public int RunFromMin
        { get { return 0; } }

        public int Step
        { get; set; }

        public IEnumerator<Bitmap> GetEnumerator()
        {
            while (reader.IsOpen)
            {
                yield return reader.ReadVideoFrame();

                for (int i = Step; i > 1; i--)
                    reader.ReadVideoFrame();
            }

            //Restart();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Dispose() => reader.Dispose();
    }

    class VideoAVIGiver : IGiver<Bitmap>
    {
        private AVIReader reader;

        public string Path { get; private set; }
        public string Tag { get { return System.IO.Path.GetFileNameWithoutExtension(Path); } }

        private Bitmap getFrame(int position)
        {
            int tmp = reader.Position;
            reader.Position = position;

            Bitmap frame = reader.GetNextFrame();

            this.reader.Position = tmp;

            return frame;
        }

        public bool isValidIndex(int index) => index >= 0 && index <= RunToMax;

        public void Restart()
        {
            ActualIndex = RunFrom;
        }

        public VideoAVIGiver(string path)
        {
            reader = new AVIReader();
            reader.Open(path);

            Path = path;

            RunFrom = 0;
            RunTo = RunToMax;
            Step = 1;
        }

        public Bitmap this[int index] { get { return getFrame(index); } }

        public Bitmap Actual { get { return getFrame(ActualIndex); } }

        public int ActualIndex { get { return reader.Position; } set { reader.Position = value; } }

        public Bitmap First { get { return getFrame(0); } }

        public Bitmap Last { get { return getFrame(reader.Length); } }

        public int RunFrom
        { get; set; }

        public int RunTo
        { get; set; }

        public int RunToMax
        { get { return reader.Length; } }
        public int RunFromMin
        { get { return 0; } }

        public int Step
        { get; set; }

        public IEnumerator<Bitmap> GetEnumerator()
        {
            int frameCounter;

            reader.Position = frameCounter = RunFrom;

            do
            {
                reader.Position = frameCounter;
                yield return reader.GetNextFrame();

                for (int i = Step; i >= 1; i--)
                    frameCounter++;

            } while (frameCounter < RunTo);

            //Restart();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Dispose() => reader.Dispose();
    }
}
