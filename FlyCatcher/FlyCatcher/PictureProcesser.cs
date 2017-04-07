using AForge.Imaging;
using AForge.Imaging.Filters;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Imaging;

namespace FlyCatcher
{
    interface IPreProcessor<T>
    {
        T processItem(T item);
    }

    class BitmapPreProcessor : IPreProcessor<Bitmap>
    {
        private MainForm owner;
        private Invert inv;
        private BaseInPlacePartialFilter thresholding;

        private Grayscale gr;
        private Grayscale grayscale
        {
            get
            {
                if (gr == null ||
                    gr.RedCoefficient != owner.redCoeficient ||
                    gr.GreenCoefficient != owner.greenCoeficient ||
                    gr.BlueCoefficient != owner.blueCoeficient)
                    gr = new Grayscale(owner.redCoeficient, owner.greenCoeficient, owner.blueCoeficient);

                return gr;
            }
        }


        public BitmapPreProcessor(MainForm owner)
        {
            this.owner = owner;

            inv = new Invert();

            thresholding = new OtsuThreshold();
            //thresholding = new Threshold();
            //thresholding = new SISThreshold();
        }

        bool shouldInvert { get { return owner.shouldInvert; } }

        public Bitmap processItem(Bitmap image) => thresholding.Apply(grayscale.Apply(shouldInvert ? inv.Apply(image) : image));
    }
}
