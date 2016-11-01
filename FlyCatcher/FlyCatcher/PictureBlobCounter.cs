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
        ICollection<T2> CountItems(T1 image);
    }

    class PictureBlobCounter :ICounter<Bitmap, Blob>
    {
        private BlobCounter blobCounter;
        private MainForm owner;

        int upperBound { get { return owner.upperBoundValue; } }
        int lowerBound { get { return owner.lowerBoundValue; } }

        Color backGroundThreshold
        {
            get
            {
                return Color.FromArgb(
                       (owner.backgroundColor.A + owner.flyColor.A) / 2,
                       (owner.backgroundColor.R + owner.flyColor.R) / 2,
                       (owner.backgroundColor.G + owner.flyColor.G) / 2,
                       (owner.backgroundColor.B + owner.flyColor.B) / 2
                       );
            }
        }

        public PictureBlobCounter(MainForm owner)
        {
            this.owner = owner;

            blobCounter = new BlobCounter();
            //blobCounter.BlobsFilter
            blobCounter.CoupledSizeFiltering = true;//TODO: do not know what this do
            blobCounter.FilterBlobs = true;
        }

        public ICollection<Blob> CountItems(Bitmap image)
        {
            blobCounter.MaxHeight = blobCounter.MaxWidth = upperBound;
            blobCounter.MinHeight = blobCounter.MinWidth = lowerBound;            

            blobCounter.BackgroundThreshold = backGroundThreshold;

            blobCounter.ProcessImage(image);

            return blobCounter.GetObjects(image, true);
        }
    }
}
