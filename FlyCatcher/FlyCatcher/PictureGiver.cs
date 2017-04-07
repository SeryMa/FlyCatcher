using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Collections;
using System.Diagnostics;
using System.IO;

namespace FlyCatcher
{
    interface IGiver<out T> : IEnumerable<T>
    {
        T First { get; }
        T Actual { get; }
        T Last { get; }

        T this[int index] { get; }

        bool isValidIndex(int index);

        int actualIndex { get; set; }

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

        public int actualIndex { get; set; }
        int lastIndex;        //TODO: problem with undefined values
        int fromIndex;
        int toIndex;

        public Bitmap Actual
        { get { return this[actualIndex]; } }

        public Bitmap First
        { get { return this[0]; } }

        public Bitmap Last
        { get { return this[lastIndex]; } }

        public Bitmap this[int index]
        { get { return new Bitmap(photoName(index)); } }//TODO: argument out of range exeption

        public int RunFrom
        { get { return fromIndex; } set { fromIndex = value; } }

        public int RunTo
        { get { return toIndex; } set { toIndex = value; } }

        public int RunToMax
        { get { return lastIndex; } }

        public int Step
        { get; set; }

        public bool isValidIndex(int index) => index >= 0 && index <= lastIndex;

        private void init(string photoMask, string folder, string fileSuffix, int numbers, int last)
        {
            actualIndex = 0;
            this.photoMask = photoMask;
            this.digitCount = numbers;
            this.fileSuffix = fileSuffix;

            this.folder = folder;

            lastIndex = last;
            fromIndex = 0;
            toIndex = lastIndex;
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
        { get { return File.Exists(photoName(actualIndex)); } }

        public IEnumerator<Bitmap> GetEnumerator()
        {
            actualIndex = fromIndex;
            while (validPhoto && actualIndex <= toIndex)
            {
                yield return this[actualIndex];
                actualIndex += Step;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
