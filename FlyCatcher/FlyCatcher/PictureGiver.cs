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
    interface IPictureGiver<out T> : IEnumerable<T>
    {
        T First { get; }
        T Actual { get; }
        T Last { get; }

        T this[int index] { get; }

        bool isValidIndex(int index);

        int RunFrom { get; set; }
        int RunTo { get; set; }
        int RunToMax { get; }
        int Step { set; }
    }

    class SeparatePhotoGiver : IPictureGiver<Bitmap>
    {        
        string photoMask;
        string folder;
        string fileSuffix;
        int numbers;

        int actualIndex;
        int lastIndex;        //TODO: problem with undefined values
        int fromIndex;
        int toIndex;
        int step;

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
        { set { step = value; } }

        public bool isValidIndex(int index)
        {
            return index >= 0 && index <= lastIndex;
        }


        public SeparatePhotoGiver(string photoMask, string folder, string fileSuffix, int numbers, int last)
        {
            actualIndex = 0;
            this.photoMask = photoMask;
            this.numbers = numbers;
            this.fileSuffix = fileSuffix;

            this.folder = folder;

            lastIndex = last;
            fromIndex = 0;
            toIndex = lastIndex;
            step = 1;
        }

        private string photoName(int index)
        {
            //StringBuilder num = new StringBuilder(index.ToString($"D{numbers}");
            //while (num.Length < numbers)
            //    num.Append(0);

            string num = index.ToString($"D{numbers}");

            Console.WriteLine($"{folder}\\{photoMask}{num}.{fileSuffix}");

            return $"{folder}\\{photoMask}{num}.{fileSuffix}";
        }

        private bool validPhoto
        { get { return File.Exists(photoName(actualIndex)); } }

        public IEnumerator<Bitmap> GetEnumerator()
        {
            int tmpActual = fromIndex;
            while (validPhoto && tmpActual <= toIndex)
            {
                yield return this[tmpActual];
                tmpActual += step;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
