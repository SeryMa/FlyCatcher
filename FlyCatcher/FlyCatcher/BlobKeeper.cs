using System;
using System.Collections.Generic;
using System.Collections;
//using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using AForge;
using AForge.Imaging;
using AForge.Imaging.Filters;

namespace FlyCatcher
{
    interface IKeeper<T1, TKey>
    {
        void addCollection(ICollection<T1> coll);

        Dictionary<TKey, T1> ActualInfo { get; }

        //ICollection<T1> Items { get; }
        //int buffer { get; set; }
        //ICollection<T1>[] blobBuffer { get; set; }
    }

    class BlobKeeper : IKeeper<Blob, int>
    {
        private Queue<ICollection<Blob>> blobBuffer;
        private int bufferSize;

        public Dictionary<int, Blob> ActualInfo { get { return actualInfo; } }
        private Dictionary<int, Blob> actualInfo;

        public BlobKeeper(int bufferSize)
        {
            blobBuffer = new Queue<ICollection<Blob>>(bufferSize + 1);
            this.bufferSize = bufferSize;
        }



        public void addCollection(ICollection<Blob> coll)
        {
            blobBuffer.Enqueue(coll);

            //TODO: Upadte the dictionary

            if (blobBuffer.Count > bufferSize)
                blobBuffer.Dequeue();
        }
    }
}
