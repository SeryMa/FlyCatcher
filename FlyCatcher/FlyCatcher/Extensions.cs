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
        { return 100 * ((actual - min) / (max - min)); }

        public static int getActual(double min, double max, double percent)
        { return (int)Math.Floor(min + ((percent / 100) * (max - min))); }

        public static Rectangle recalculateRectangle(Rectangle rect, double actualWidth, double actualHeight, double desiredWidth, double desiredHeight)
        {
            double biasWidth = desiredWidth /actualWidth;
            double biasHeight = desiredHeight / actualHeight;
            return new Rectangle((int)Math.Floor(rect.Left * biasWidth), (int)Math.Floor(rect.Top * biasHeight),(int)Math.Floor(rect.Width * biasWidth), (int)Math.Floor(rect.Height * biasHeight));
        }

        public static Rectangle getRectangleFromRadius(Point center, int width, int height)
        {
            return getRectangleFromRadius(center.X, center.Y, width, height);
        }

        public static Rectangle getRectangleFromRadius(int X, int Y, int width, int height)
        {
            return getRectangle(X + width, Y + height, X - width, Y - height);
        }

        public static Rectangle getRectangle(int x1, int y1, int x2, int y2)
        {
            return new Rectangle(Math.Min(x1, x2), Math.Min(y1, y2), Math.Abs(x1 - x2), Math.Abs(y1 - y2));
        }

        public static Rectangle getRectangle(Point a, Point b)
        {
            return getRectangle(a.X, a.Y, b.X, b.Y);
        }

        public static Rectangle getRectangle(Point p, int x, int y)
        {
            return getRectangle(p.X, p.Y, x, y);
        }

        public static bool isPointInRectangle(Point p, Rectangle rect)
        {
            return isPointInRectangle(p.X, p.Y, rect);
        }

        public static bool isPointInRectangle(int x, int y, Rectangle rect)
        {
            return ((y >= rect.Left && y <= rect.Right) &&
                    (x >= rect.Top && x <= rect.Bottom));
        }
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

        public static byte[,] convertToArray(this Bitmap inPicture)
        {
            Bitmap picture = Grayscale.CommonAlgorithms.BT709.Apply(inPicture);

            BitmapData data = picture.LockBits(new Rectangle(0, 0, picture.Width, picture.Height), ImageLockMode.ReadOnly, picture.PixelFormat);

            try
            {
                IntPtr ptr = data.Scan0;
                int stride = Math.Abs(data.Stride);
                int bytes = stride * picture.Height;

                byte[] rgbValues = new byte[bytes];
                System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);

                byte[,] outputValues = new byte[stride, picture.Height];
                for (int x = 0; x < picture.Height; x++)
                    for (int y = 0; y < stride; y++)
                        outputValues[x, y] = rgbValues[x * stride + y];

                return outputValues;
            }
            finally
            {
                picture.UnlockBits(data);
            }
        }

        public delegate bool isInCurve(AForge.Point point);
    }
}

