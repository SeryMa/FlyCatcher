﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using AForge;
using AForge.Imaging;
using AForge.Imaging.Filters;

namespace FlyCatcher
{
    interface ICounter<in T1, T2>
    {
        ICollection<T2> CountItems(T1 image);
    }

    class PictureBlobCounter :ICounter<Bitmap, Blob>
    {
        private BlobCounter blobCounter;
        private MainForm owner;

        int upperBound { get { return owner.upperBoundValue; } }
        int lowerBound { get { return owner.lowerBoundValue; } }

        public PictureBlobCounter(MainForm owner)
        {
            this.owner = owner;

            blobCounter = new BlobCounter();
            //blobCounter.BlobsFilter
            blobCounter.CoupledSizeFiltering = true;//TODO: do not know what this do
            blobCounter.FilterBlobs = true;
            blobCounter.BackgroundThreshold = Color.Gray;
        }

        public ICollection<Blob> CountItems(Bitmap image)
        {
            blobCounter.MaxHeight = blobCounter.MaxWidth = upperBound;
            blobCounter.MinHeight = blobCounter.MinWidth = lowerBound;            

            

            blobCounter.ProcessImage(image);

            return blobCounter.GetObjects(image, true);
        }
    }
}
