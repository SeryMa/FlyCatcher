using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Imaging;

using AForge.Imaging;
using AForge.Imaging.Filters;

namespace FlyCatcher
{
    static class MathFunctions
    {
        public static double ValueToPercent(double min, double max, double actual) => 100 * ((actual - min) / (max - min));
        public static double ValueToPercent(double max, double actual) => ValueToPercent(0, max, actual);
        public static int PercentToValue(double min, double max, double percent) => (int)Math.Floor(min + ((percent / 100) * (max - min)));
        public static int PercentToValue(double max, double percent) => PercentToValue(0, max, percent);

        public static RectangleF recalculateRectangle(RectangleF rect, double actualWidth, double actualHeight, double desiredWidth, double desiredHeight)
        {
            double biasWidth = desiredWidth / actualWidth;
            double biasHeight = desiredHeight / actualHeight;
            return new Rectangle((int)Math.Floor(rect.Left * biasWidth), (int)Math.Floor(rect.Top * biasHeight), (int)Math.Floor(rect.Width * biasWidth), (int)Math.Floor(rect.Height * biasHeight));
        }

        public static RectangleF recalculateRectangle(RectangleF percentRectangle, RectangleF boundingRectangle) => recalculateRectangle(percentRectangle, boundingRectangle.Size);
        public static RectangleF recalculateRectangle(RectangleF percentRectangle, SizeF boundingRectangle)
            => new RectangleF(
                (percentRectangle.Left / 100 * boundingRectangle.Width),
                (percentRectangle.Top / 100 * boundingRectangle.Height),
                (percentRectangle.Width / 100 * boundingRectangle.Width),
                (percentRectangle.Height / 100 * boundingRectangle.Height));

        public static RectangleF getPercentRectangle(RectangleF original, RectangleF desiredSize) => getPercentRectangle(original, desiredSize.Size);
        public static RectangleF getPercentRectangle(RectangleF original, SizeF desiredSize)
            => new RectangleF(
                100*original.X / desiredSize.Width, 100*original.Y / desiredSize.Height,
                100*original.Width / desiredSize.Width, 100*original.Height / desiredSize.Height);

        public static Rectangle Round(this RectangleF rect) => new Rectangle((int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height);

        public static RectangleF getRectangleFromRadius(PointF center, double halfWidth, double halfHeight) => getRectangleFromRadius(center.X, center.Y, halfWidth, halfHeight);
        public static RectangleF getRectangleFromRadius(PointF center, double halfSize) => getRectangleFromRadius(center.X, center.Y, halfSize, halfSize);
        public static RectangleF getRectangleFromRadius(double X, double Y, double halfWidth, double halfHeight) => getRectangle(X + halfWidth, Y + halfHeight, X - halfWidth, Y - halfHeight);
        public static RectangleF getRectangle(double x1, double y1, double x2, double y2) => new RectangleF((float)Math.Min(x1, x2), (float)Math.Min(y1, y2), (float)Math.Abs(x1 - x2), (float)Math.Abs(y1 - y2));
        public static RectangleF getRectangle(PointF a, PointF b) => getRectangle(a.X, a.Y, b.X, b.Y);
        public static RectangleF getRectangle(PointF p, double x, double y) => getRectangle(p.X, p.Y, x, y);

        public static PointF getCenter(this RectangleF rect) => new PointF(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);
        public static double getDiameter(this RectangleF rect) => (rect.Height + rect.Width) / 4.0;
        public static double getDiameter(this Rectangle rect) => ((RectangleF)rect).getDiameter();

        public static bool isPointInEllipse(PointF p, RectangleF rect) => isPointInEllipse(p.X, p.Y, rect);
        public static bool isPointInEllipse(AForge.DoublePoint p, RectangleF rect) => isPointInEllipse(p.X, p.Y, rect);
        public static bool isPointInEllipse(double x, double y, RectangleF rect)
        {
            PointF center = rect.getCenter();
            return (Math.Pow((x - center.X) / (rect.Width / 2), 2) + Math.Pow((y - center.Y) / (rect.Height / 2), 2)) <= 1;
        }

        public static bool isPointInRectangle(PointF p, RectangleF rect) => isPointInRectangle(p.X, p.Y, rect);
        public static bool isPointInRectangle(AForge.DoublePoint p, RectangleF rect) => isPointInRectangle(p.X, p.Y, rect);
        public static bool isPointInRectangle(double x, double y, RectangleF rect) => ((x >= rect.Left && x <= rect.Right) && (y >= rect.Top && y <= rect.Bottom));

        public static double getDistanceTo(this Point pointA, Point pointB)
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

        public static void ApplyInPlace(this Grayscale scale, Bitmap image) => image = scale.Apply(image);

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

        public static void ActualizeBlobKeeper(IKeeper<Blob, double, double, AForge.Point> blobKeeper, IEnumerable<Blob> blobs)
        {
            blobKeeper.ActualizeData(blobs);
        }
        public static void RefreshBlobKeeper(IKeeper<Blob, double, double, AForge.Point> blobKeeper, IEnumerable<Blob> blobs)
        {
            blobKeeper.Refresh(blobs);
        }

        static Random rand = new Random();
        public static string GenerateString(int size)
        {
            char[] chars = new char[size];

            for (int i = 0; i < size; i++)
                chars[i] = Constants.AlfaNumerics[rand.Next(Constants.AlfaNumerics.Length)];

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

        public static int ParseBool(this bool pred) => pred ? 1 : 0;
    }

    static class Constants
    {
        public const string Digits = "0123456789";
        public const string Alphabet = "abcdefghijklmnopqrstuvwyxzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        public const string AlfaNumerics =
        "abcdefghijklmnopqrstuvwyxzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        public const char Delimeter = ';';
        public const char Blank = '-';

        public const string JPGFileExtension = ".jpg";
        public const string PNGFileExtension = ".png";
        public const string BMPFileExtension = ".bmp";
        public const string ConfigFileExtension = ".init";
        public const string MaskFileExtension = ".mask";
        public const string OutputFileExtension = ".output";
        public const string CSVFileExtension = ".csv";
        public const string StateFileExtension = ".state";

        [Flags]
        public enum OutputFormat { None = 0, Objects = 1, AverageSpeed = 2, ImmediateSpeed = 4, Position = 8, Prediction = 16, ImmadiateArea = 32, AverageArea = 64 }
        public static Dictionary<OutputFormat, string> OutputTag = new Dictionary<OutputFormat, string>()
        {
            { OutputFormat.None, "" },
            { OutputFormat.Objects, "objects" },
            { OutputFormat.AverageSpeed, "avg_speed" },
            { OutputFormat.ImmediateSpeed, "imm_speed" },
            { OutputFormat.Position, "positinon" },
            { OutputFormat.Prediction, "prediction" },
            { OutputFormat.ImmadiateArea, "imm_area" },
            { OutputFormat.AverageArea, "avg_area" }
        };

        [Flags]
        public enum HighlightFormat { None = 0, Object = 1, Prediction = 2, Trace = 4, Tag = 8, Direction = 16 }
        public static Dictionary<HighlightFormat, string> HighlightTag = new Dictionary<HighlightFormat, string>()
        {
            {HighlightFormat.None, "" },
            {HighlightFormat.Object, "mark_object" },
            {HighlightFormat.Prediction, "mark_prediction" },
            {HighlightFormat.Trace, "mark_trace" },
            {HighlightFormat.Tag, "mark_tag" },
            {HighlightFormat.Direction, "mark_direction" },
        };
    }
}

