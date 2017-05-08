using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;

using AForge.Imaging;

namespace FlyCatcher
{
    interface ICounter<in T1, T2, T3>
    {
        IEnumerable<Tuple<string, T2>> CountItems(T1 image);
        IEnumerable<IMask<T3>> Masks { get; }
    }

    class PictureBlobCounter : ICounter<Bitmap, Blob, AForge.Point>
    {
        private BlobCounter blobCounter;
        public IEnumerable<IMask<AForge.Point>> Masks { get { return owner.masks; } }
        private MainForm owner;        
        private BlobFilter filter;       
    
        int upperBound { get { return owner.upperBoundValue; } }
        int lowerBound { get { return owner.lowerBoundValue; } }
        bool filterByArea { get { return owner.filterByArea; } }
        
        public PictureBlobCounter(MainForm owner)
        {
            this.owner = owner;

            blobCounter = new BlobCounter();
            //Masks = new List<IMask<AForge.Point>>();                       

            //In coupled filtering mode, objects are filtered out in the case if their width is smaller than MinWidth and height is smaller than MinHeight.
            blobCounter.CoupledSizeFiltering = true;
            blobCounter.FilterBlobs = true;
            blobCounter.BackgroundThreshold = Color.Gray;

            filter = new BlobFilter();
            blobCounter.BlobsFilter = filter;
        }

        public IEnumerable<Tuple<string, Blob>> CountItems(Bitmap image)
        {
            if (filterByArea)
                blobCounter.BlobsFilter = filter;
            else
                blobCounter.BlobsFilter = null;

            blobCounter.MaxHeight = blobCounter.MaxWidth = upperBound;
            blobCounter.MinHeight = blobCounter.MinWidth = lowerBound;

            filter.max = upperBound;
            filter.min = lowerBound;

            blobCounter.ProcessImage(image);


            List<Tuple<string, Blob>> output = new List<Tuple<string, Blob>>();

            foreach (var blob in blobCounter.GetObjectsInformation())
                foreach (var mask in Masks)
                    if (mask.IsIn(blob.CenterOfGravity) &&
                        !output.Any(tpl => tpl.Item1 == mask.Tag && tpl.Item2 == blob))
                        output.Add(new Tuple<string, Blob>(mask.Tag, blob));

            return output;

            //return from blob in blobCounter.GetObjects(image, false)
            //       from mask in Masks
            //       where mask.IsIn(blob.CenterOfGravity)
            //       select new Tuple<string, Blob>(mask.Tag, blob);
        }
    }
}
