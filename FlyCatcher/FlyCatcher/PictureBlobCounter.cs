using System;
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
        IEnumerable<Tuple<string, T2>> CountItems(T1 image);
        ICollection<CurveMask> Masks { get;  set; }
    }

    class PictureBlobCounter : ICounter<Bitmap, Blob>
    {
        private BlobCounter blobCounter;
        public ICollection<CurveMask> Masks { get; set; }
        private MainForm owner;        
    
        int upperBound { get { return owner.upperBoundValue; } }
        int lowerBound { get { return owner.lowerBoundValue; } }
        
        public PictureBlobCounter(MainForm owner)
        {
            this.owner = owner;

            blobCounter = new BlobCounter();
            Masks = new List<CurveMask>();
            //In coupled filtering mode, objects are filtered out in the case if their width is smaller than MinWidth and height is smaller than MinHeight.
            blobCounter.CoupledSizeFiltering = true;
            blobCounter.FilterBlobs = true;
            blobCounter.BackgroundThreshold = Color.Gray;
        }

        public IEnumerable<Tuple<string, Blob>> CountItems(Bitmap image)
        {
            blobCounter.MaxHeight = blobCounter.MaxWidth = upperBound;
            blobCounter.MinHeight = blobCounter.MinWidth = lowerBound;

            blobCounter.ProcessImage(image);

            return from blob in blobCounter.GetObjects(image, true)
                   from mask in Masks
                   where mask.isIn(blob.CenterOfGravity)
                   select new Tuple<string, Blob>(mask.tag, blob);
        }
    }
}
