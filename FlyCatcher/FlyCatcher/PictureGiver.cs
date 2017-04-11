using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Collections;
using System.Diagnostics;
using System.IO;

using AForge.Video.FFMPEG;
using AForge.Video.VFW;

namespace FlyCatcher
{
    interface IGiver<out T> : IEnumerable<T>
    {
        T First { get; }
        T Actual { get; }
        T Last { get; }

        T this[int index] { get; }

        bool isValidIndex(int index);

        int ActualIndex { get; set; }

        int RunFrom { get; set; }
        int RunTo { get; set; }
        int RunToMax { get; }
        int Step { get; set; }
    }

    class SeparatePhotoGiver : IGiver<Bitmap>
    {
        string photoMask;
        string folder;
        string fileSuffix;
        int digitCount;

        public int ActualIndex { get; set; }

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

        public int Step
        { get; set; }

        public bool isValidIndex(int index) => index >= 0 && index <= RunToMax;

        private void init(string photoMask, string folder, string fileSuffix, int numbers, int last)
        {
            ActualIndex = 0;
            this.photoMask = photoMask;
            this.digitCount = numbers;
            this.fileSuffix = fileSuffix;

            this.folder = folder;

            RunToMax = last;
            RunFrom = 0;
            RunTo = RunToMax;
            Step = 1;
        }

        private string getMask(string file) => file.TrimEnd(Constants.Digits.ToCharArray());
        private string getLast(string file) => new string(file.Reverse().TakeWhile(char.IsDigit).Reverse().ToArray());


        /// <summary>
        /// Constructor for photo giver, that iterates over images in a folder.
        /// </summary>
        /// <param name="photoMask"></param>
        /// <param name="folder"></param>
        /// <param name="fileSuffix"></param>
        /// <param name="numbers"></param>
        /// <param name="last"></param>
        public SeparatePhotoGiver(string path)
        {
            string str = getLast(Path.GetFileNameWithoutExtension(path));

            init(getMask(Path.GetFileNameWithoutExtension(path)),
                 Path.GetDirectoryName(path),
                 Path.GetExtension(path),
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
            init(photoMask, folder, fileSuffix, digitCount, last);
        }

        private string photoName(int index)
        {
            string num = index.ToString($"D{digitCount}");

            Console.WriteLine($"{folder}\\{photoMask}{num}{fileSuffix}");

            return $"{folder}\\{photoMask}{num}{fileSuffix}";
        }

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
    }

    class VideoMPEGGiver : IGiver<Bitmap>
    {
        private VideoFileReader reader;
        private Bitmap preview;
        
        public VideoMPEGGiver(string path)
        {
            reader = new VideoFileReader();
            reader.Open(path);
            preview = reader.ReadVideoFrame();

            //This is used to reset the reader to the default position and not loosing the first frame
            reader.Close();
            reader.Open(path);            
    }

        public bool isValidIndex(int index) => true;

        public Bitmap this[int index]
        { get { return preview; } }

        public Bitmap Actual
        { get { return preview; } }

        public int ActualIndex
        { get; set; }

        public Bitmap First
        { get { return preview; } }

        public Bitmap Last
        { get { return preview; } }

        public int RunFrom
        { get; set; }

        public int RunTo
        { get; set; }

        public int RunToMax
        { get; private set; }

        public int Step
        { get; set; }

        public IEnumerator<Bitmap> GetEnumerator()
        {
            yield return reader.ReadVideoFrame();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    class VideoAVIGiver : IGiver<Bitmap>
    {
        private AVIReader reader;

        private Bitmap getFrame(int position)
        {
            int tmp = reader.Position;
            reader.Position = position;

            Bitmap frame = reader.GetNextFrame();

            this.reader.Position = tmp;

            return frame;
        }

        public bool isValidIndex(int index) => index >= 0 && index <= RunToMax;

        public VideoAVIGiver(string path)
        {
            reader = new AVIReader();
            reader.Open(path);

            RunToMax = reader.Length;
            RunFrom = 0;
            RunTo = RunToMax;
            Step = 1;
        }

        public Bitmap this[int index] { get { return getFrame(index); } }

        public Bitmap Actual { get { return getFrame(ActualIndex); } }

        public int ActualIndex { get; set; }

        public Bitmap First { get { return getFrame(0); } }

        public Bitmap Last { get { return getFrame(reader.Length); } }

        public int RunFrom
        { get; set; }

        public int RunTo
        { get; set; }

        public int RunToMax
        { get; private set; }

        public int Step
        { get; set; }

        public IEnumerator<Bitmap> GetEnumerator()
        {
            yield return reader.GetNextFrame();

            for (int i = Step; i > 1; i--)
                reader.Position++;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
