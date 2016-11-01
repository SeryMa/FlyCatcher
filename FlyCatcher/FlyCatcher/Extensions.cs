using AForge.Imaging.Filters;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Imaging;
using AForge.Imaging;

namespace FlyCatcher
{
    static class MathFunctions
    {
        public static double getPercent(double min, double max, double actual)
        { return 100*( (actual - min) / (max - min)); }

        public static int getActual(double min, double max, double percent)
        { return (int)Math.Floor(min + ((percent/100) * (max - min))); }


    }

    static class Extensions
    {
        public delegate void operation();
        public static void crossThreadOperation(this System.Windows.Forms.Control control, operation fun)
        {
            //if (!control.IsDisposed)           
            if (control.InvokeRequired)
                control.Invoke(new System.Windows.Forms.MethodInvoker(delegate () { fun(); }));
            else
                fun();
        }

        public static void ApplyInPlace(this Grayscale scale, Bitmap image)
        {
            image = scale.Apply(image);
        }

        public static Bitmap convertPixelFormat(this Bitmap image, PixelFormat format)
        {
            //var bmp = new Bitmap(image.Width, image.Height, format);
            //using (var gr = Graphics.FromImage(bmp))
            //    gr.DrawImage(image, new Rectangle(0, 0, image.Width, image.Height));
            //return bmp;

            return AForge.Imaging.Image.Clone(image, format);
        }
    }
}
