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
            double biasWidth = desiredWidth / actualWidth;
            double biasHeight = desiredHeight / actualHeight;
            return new Rectangle((int)Math.Floor(rect.Left * biasWidth), (int)Math.Floor(rect.Top * biasHeight), (int)Math.Floor(rect.Width * biasWidth), (int)Math.Floor(rect.Height * biasHeight));
        }

        public static Rectangle getRectangleFromRadius(Point center, double halfWidth, double halfHeight)
        {
            return getRectangleFromRadius(center.X, center.Y, halfWidth, halfHeight);
        }

        public static Rectangle getRectangleFromRadius(double X, double Y, double halfWidth, double halfHeight)
        {
            return getRectangle(X + halfWidth, Y + halfHeight, X - halfWidth, Y - halfHeight);
        }

        public static Rectangle getRectangle(double x1, double y1, double x2, double y2)
        {
            return new Rectangle((int)Math.Min(x1, x2), (int)Math.Min(y1, y2), (int)Math.Abs(x1 - x2), (int)Math.Abs(y1 - y2));
        }

        public static Rectangle getRectangle(Point a, Point b)
        {
            return getRectangle(a.X, a.Y, b.X, b.Y);
        }

        public static Rectangle getRectangle(Point p, double x, double y)
        {
            return getRectangle(p.X, p.Y, x, y);
        }

        public static bool isPointInEllipse(Point p, Rectangle rect)
        {
            return isPointInEllipse(p.X, p.Y, rect);
        }

        public static bool isPointInEllipse(AForge.Point p, Rectangle rect)
        {
            return isPointInEllipse(p.X, p.Y, rect);
        }

        public static bool isPointInEllipse(double x, double y, Rectangle rect)
        {
            Point center = rect.getCenter();
            return (Math.Pow((x - center.X) / (rect.Width / 2), 2) + Math.Pow((y - center.Y) / (rect.Height / 2), 2)) <= 1;
        }

        public static Point getCenter(this Rectangle rect)
        {
            return new Point(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);
        }

        public static bool isPointInRectangle(Point p, Rectangle rect)
        {
            return isPointInRectangle(p.X, p.Y, rect);
        }

        public static bool isPointInRectangle(AForge.Point p, Rectangle rect)
        {
            return isPointInRectangle(p.X, p.Y, rect);
        }

        public static bool isPointInRectangle(double x, double y, Rectangle rect)
        {
            return ((y >= rect.Left && y <= rect.Right) &&
                    (x >= rect.Top && x <= rect.Bottom));
        }

        public static double getDistance(this Point pointA, Point pointB)
        {
            return Math.Sqrt(
                        Math.Pow(pointA.X - pointB.X, 2) +
                        Math.Pow(pointA.Y - pointB.Y, 2)
                     );
        }

        public static AForge.Point Normalize(this AForge.Point point)
        {
            AForge.Point newPoint = point;
            newPoint.NormalizeInPlace();
            return newPoint;
        }

        public static void NormalizeInPlace(this AForge.Point point)
        {
            float dist = point.EuclideanNorm();

            point.X /= dist;
            point.Y /= dist;
        }

        public static AForge.Point Multiply(this AForge.Point point, double coef)
        {
            AForge.Point newPoint = point;
            newPoint.MultiplyInPlace(coef);
            return newPoint;
        }

        public static void MultiplyInPlace(this AForge.Point point, float coef)
        {
            point.X *= coef;
            point.Y *= coef;
        }

        public static void MultiplyInPlace(this AForge.Point point, double coef)
        {
            point.MultiplyInPlace((float)coef);
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

        public delegate T alter<T>(T item);
        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> source, alter<T> action)
        {
            return from item in source select action(item);            
        }

        public delegate T transform<out T, in U>(U item);
        public static IEnumerable<T> ForEach<T, U>(this IEnumerable<U> souce, transform<T,U> action)
        {
            return from item in souce select action(item);
        }

        public delegate bool isInCurve(AForge.Point point);

        public static PointF Converse(this AForge.Point point)
        {
            return new PointF(point.X, point.Y);
        }

        public static AForge.Point Converse(this PointF point)
        {
            return new AForge.Point(point.X, point.Y);
        }

        public static AForge.Point Converse(this Point point)
        {
            return new AForge.Point(point.X, point.Y);
        }

    }
}

