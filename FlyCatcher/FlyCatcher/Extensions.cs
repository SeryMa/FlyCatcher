﻿using AForge.Imaging.Filters;
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
        public static double getPercent(double min, double max, double actual) => 100 * ((actual - min) / (max - min));

        public static int getActual(double min, double max, double percent) => (int)Math.Floor(min + ((percent / 100) * (max - min)));

        public static Rectangle recalculateRectangle(Rectangle rect, double actualWidth, double actualHeight, double desiredWidth, double desiredHeight)
        {
            double biasWidth = desiredWidth / actualWidth;
            double biasHeight = desiredHeight / actualHeight;
            return new Rectangle((int)Math.Floor(rect.Left * biasWidth), (int)Math.Floor(rect.Top * biasHeight), (int)Math.Floor(rect.Width * biasWidth), (int)Math.Floor(rect.Height * biasHeight));
        }

        public static Rectangle getRectangleFromRadius(Point center, double halfWidth, double halfHeight) => getRectangleFromRadius(center.X, center.Y, halfWidth, halfHeight);
        public static Rectangle getRectangleFromRadius(double X, double Y, double halfWidth, double halfHeight) => getRectangle(X + halfWidth, Y + halfHeight, X - halfWidth, Y - halfHeight);
        public static Rectangle getRectangle(double x1, double y1, double x2, double y2) => new Rectangle((int)Math.Min(x1, x2), (int)Math.Min(y1, y2), (int)Math.Abs(x1 - x2), (int)Math.Abs(y1 - y2));
        public static Rectangle getRectangle(Point a, Point b) => getRectangle(a.X, a.Y, b.X, b.Y);
        public static Rectangle getRectangle(Point p, double x, double y) => getRectangle(p.X, p.Y, x, y);

        public static Point getCenter(this Rectangle rect) => new Point(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);
        public static double getDiameter(this Rectangle rect) => (rect.Height + rect.Width) / 4.0;

        public static bool isPointInEllipse(Point p, Rectangle rect) => isPointInEllipse(p.X, p.Y, rect);
        public static bool isPointInEllipse(AForge.Point p, Rectangle rect) => isPointInEllipse(p.X, p.Y, rect);
        public static bool isPointInEllipse(double x, double y, Rectangle rect)
        {
            Point center = rect.getCenter();
            return (Math.Pow((x - center.X) / (rect.Width / 2), 2) + Math.Pow((y - center.Y) / (rect.Height / 2), 2)) <= 1;
        }

        public static bool isPointInRectangle(Point p, Rectangle rect) => isPointInRectangle(p.X, p.Y, rect);
        public static bool isPointInRectangle(AForge.Point p, Rectangle rect) => isPointInRectangle(p.X, p.Y, rect);
        public static bool isPointInRectangle(double x, double y, Rectangle rect) => ((y >= rect.Left && y <= rect.Right) && (x >= rect.Top && x <= rect.Bottom));

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

        public static AForge.Point Mean(AForge.Point A, AForge.Point B)
        {
            return (A - B).Multiply(0.5);
        }
    }

    static class Extensions
    {
        public delegate void operation();
        public static void crossThreadOperation(this System.Windows.Forms.Control control, operation fun)
        {
            if (control.InvokeRequired)
                control.Invoke(new System.Windows.Forms.MethodInvoker(delegate () { fun(); }));
            else
                fun();
        }

        public static void ApplyInPlace(this Grayscale scale, Bitmap image)
        {
            image = scale.Apply(image);
        }

        public static Bitmap convertPixelFormat(this Bitmap image, PixelFormat format) => AForge.Imaging.Image.Clone(image, format);

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

        public delegate T Alter<T>(T item);
        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> source, Alter<T> action) => from item in source select action(item);

        public delegate T Transform<out T, in U>(U item);
        public static IEnumerable<T> ForEach<T, U>(this IEnumerable<U> source, Transform<T, U> action) => from item in source select action(item);

        public delegate bool isInCurve(AForge.Point point);

        public static PointF ConverseToPointF(this AForge.Point point) => new PointF(point.X, point.Y);
        public static Point ConverseToPoint(this AForge.Point point) => new Point((int)point.X, (int)point.Y);
        public static AForge.Point Converse(this PointF point) => new AForge.Point(point.X, point.Y);
        public static AForge.Point Converse(this Point point) => new AForge.Point(point.X, point.Y);

        public static void ActualizeBlobKeeper(IKeeper<Blob, double, double> blobKeeper, IEnumerable<Blob> blobs)
        {
            blobKeeper.ActualizeData(blobs);
        }
        public static void RefreshBlobKeeper(IKeeper<Blob, double, double> blobKeeper, IEnumerable<Blob> blobs)
        {
            blobKeeper.Refresh(blobs);
        }

        static Random rand = new Random();

        public const string Alphabet =
        "abcdefghijklmnopqrstuvwyxzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        public static string GenerateString(int size)
        {
            char[] chars = new char[size];

            for (int i = 0; i < size; i++)
                chars[i] = Alphabet[rand.Next(Alphabet.Length)];

            return new string(chars);
        }

        public delegate double DistanceGetter<T, U>(T agent, U task);

        public static void iterateOverColumn<T>(this T[][] matrix, int columnIndex, Func<T, T> fun)
        {
            for (int row = 0; row < matrix.Length; row++)
                matrix[row][columnIndex] = fun(matrix[row][columnIndex]);
        }
        public static void iterateOverRow<T>(this T[][] matrix, int rowIndex, Func<T, T> fun)
        {
            for (int column = 0; column < matrix[0].Length; column++)
                matrix[rowIndex][column] = fun(matrix[rowIndex][column]);
        }

        public static void ForEach<T>(this T[][] matrix, Alter<T> fun)
        {
            for (int row = 0; row < matrix.Length; row++)
                for (int column = 0; column < matrix[0].Length; column++)
                    matrix[row][column] = fun(matrix[row][column]);
        }

        public static void SetAll<T>(this T[][] matrix, T value) => matrix.ForEach(x => value);


        public static int Columns<T>(this T[][] matrix) => matrix[0].Length;
        public static int Rows<T>(this T[][] matrix) => matrix.Length;

        public static List<int> thingsInRow<T>(this T[][] matrix, int row, T thing)
        {
            int columns = matrix.Columns();
            List<int> counter = new List<int>(columns);
            for (int j = 0; j < columns; j++)
                if (matrix[row][j].Equals(thing))
                    counter.Add(j);

            return counter;
        }
        public static List<int> thingsInColumn<T>(this T[][] matrix, int column, T thing)
        {
            int rows = matrix.Rows();
            List<int> counter = new List<int>(rows);
            for (int i = 0; i < rows; i++)
                if (matrix[i][column].Equals(thing))
                    counter.Add(i);

            return counter;
        }

        public static IEnumerable<T> GetRow<T>(this T[][] matrix, int row) => matrix[row];
        public static IEnumerable<T> GetColumn<T>(this T[][] matrix, int column)
        {
            for (int row = 0; row < matrix.Length; row++)
                yield return matrix[row][column];
        }

        public static bool isZero(this double num) => (num.isSameAs(0));
        public static bool isSameAs(this double numA, double numB) => numA.isSameAs(numB, 0.0000000001);
        public static bool isSameAs(this double numA, double numB, double precision) => (Math.Abs(numA - numB) < precision);
    }

}

