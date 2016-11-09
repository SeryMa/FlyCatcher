﻿using AForge.Imaging;
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
    interface IPicturePreProcessor<T>
    {
        void processImageInPlace(T image);
        T processImage(T image);
    }

    class BitmapPreProcessor : IPicturePreProcessor<Bitmap>
    {
        private MainForm owner;

        private Invert inv;
        
        private OtsuThreshold thresholding;

        private ApplyMask masking;

        public BitmapPreProcessor(MainForm owner)
        {
            this.owner = owner;

            inv = new Invert();
            thresholding = new OtsuThreshold();
            masking = new ApplyMask(new byte[0,0]);        
        }

        bool shouldInvert { get { return owner.shouldInvert; } }
        
        public Bitmap processImage(Bitmap image)
        {
            Bitmap pictureToReturn = AForge.Imaging.Image.Clone(image);

            if (shouldInvert) inv.ApplyInPlace(pictureToReturn);

            masking.Mask = owner.mask;
            masking.ApplyInPlace(pictureToReturn);            

            return thresholding.Apply(Grayscale.CommonAlgorithms.Y.Apply(pictureToReturn));            
        }

        public void processImageInPlace(Bitmap image)
        {
            image = processImage(image);
        }
    }
}
