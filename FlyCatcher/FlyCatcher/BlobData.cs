using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using AForge.Imaging;
using AForge.Imaging.Filters;
using System.Drawing;
using AForge;

namespace FlyCatcher
{
    interface IData<T, out MeasureType, out DistanceType> where MeasureType : IComparable
    {
        ICollection<T> Items { get; }
        DistanceType averageSpeed { get; }
        DistanceType averageArea { get; }

        AForge.Point Direction { get; }
        AForge.Point PredictNext { get; }

        T First { get; }

        string Tag { get; }

        bool valid { get; }
        void makeInvalid();

        MeasureType GetMatch(T item);
        MeasureType AddItem(T item);

        void PrintOut(StreamWriter writer, Constants.OutputFormat format);
        void PrintOutHeader(StreamWriter writer, Constants.OutputFormat format);
        void PrintOutTag(StreamWriter writer, Constants.OutputFormat format);
        void Draw(Graphics gr, Constants.HighlightFormat format);
    }

    class MergeData : IData<Blob, double, double>
    {
        public double averageArea { get { return merges.Average(x => x.averageArea); } }

        public double averageSpeed { get { return merges.Average(x => x.averageSpeed); } }


        public void PrintOut(StreamWriter writer, Constants.OutputFormat format)
        {
            foreach (var blobData in merges)
                blobData.PrintOut(writer, format);
        }

        /// <summary>
        /// MergeData doesn't support Items.
        /// </summary>
        public ICollection<Blob> Items { get { throw new NotImplementedException(); } }

        public string Tag { get { return (from x in merges select x.Tag).Aggregate((str1, str2) => str1 + str2); } }

        public AForge.Point Direction { get { return (from x in merges select x.Direction).Aggregate((A, B) => MathFunctions.Mean(A, B)); } }

        public AForge.Point PredictNext { get { return (from x in merges select x.PredictNext).Aggregate((A, B) => MathFunctions.Mean(A, B)); } }

        /// <summary>
        /// MergeData doesn't support First.
        /// </summary>
        public Blob First { get { throw new NotImplementedException(); } }

        public bool valid { get { return merges.Any(b => b.valid); } }

        private IData<Blob, double, double>[] merges;

        public MergeData(IData<Blob, double, double> first, IData<Blob, double, double> second)
        {
            merges = new IData<Blob, double, double>[2] { first, second };
        }
        public MergeData(IEnumerable<IData<Blob, double, double>> data)
        {
            merges = data.ToArray();
        }

        public double AddItem(Blob item) => (merges.Average(x => x.AddItem(item)));

        public void Draw(Graphics gr, Constants.HighlightFormat format)
        {
            foreach (var merge in merges)
                merge.Draw(gr, format);
        }

        public double GetMatch(Blob item) => (from x in merges select x.GetMatch(item)).Average();

        public void makeInvalid()
        {
            foreach (var item in merges)
                item.makeInvalid();
        }

        public void PrintOutHeader(StreamWriter writer, Constants.OutputFormat format)
        {
            foreach (var blobData in merges)
                blobData.PrintOutHeader(writer, format);
        }
        public void PrintOutTag(StreamWriter writer, Constants.OutputFormat format)
        {
            foreach (var blobData in merges)
                blobData.PrintOutTag(writer, format);
        }
    }

    class BlobData : IData<Blob, double, double>
    {
        protected static Font drawFont = new Font("Arial", 15);
        protected static SolidBrush drawBrush = new SolidBrush(Color.GreenYellow);
        private static Pen blobHighliter = new Pen(Color.Coral, 1);

        LinkedList<Blob> items;
        public ICollection<Blob> Items { get { return items; } }

        public Blob First { get { return items.First.Value; } }

        //TODO: get direction from shape of object

        LinkedList<double> distances;

        int maxCount;
        double sumAll;
        public double averageSpeed { get { return sumAll / distances.Count; } }
        public double averageArea { get { return (from x in items select x.Area).Average(); } }

        public AForge.Point Direction
        {
            get
            {
                if (items.First.Next == null)
                    return new AForge.Point(0, 0);
                else
                    return (First.CenterOfGravity - items.First.Next.Value.CenterOfGravity).Normalize();
            }
        }
        public AForge.Point PredictNext { get { return Direction.Multiply(averageSpeed) + First.CenterOfGravity; } }

        public string Tag { get; set; }

        public bool valid { get; private set; }

        private void removeLast()
        {
            sumAll -= distances.Last.Value;

            items.RemoveLast();
            distances.RemoveLast();
        }

        private double addFirst(Blob blob)
        {
            double distance = GetMatch(blob);

            sumAll += distance;

            distances.AddFirst(distance);
            items.AddFirst(blob);

            return distance;
        }

        public BlobData(int historyCount, Blob blob, string tag)
        {
            maxCount = historyCount;

            valid = true;

            Tag = tag + Extensions.GenerateString(1);

            sumAll = 0;

            items = new LinkedList<Blob>();
            distances = new LinkedList<double>();

            items.AddFirst(blob);
        }

        //TODO: get better matching function
        public double GetMatch(Blob blob) => blob.CenterOfGravity.DistanceTo(First.CenterOfGravity);

        public double AddItem(Blob blob)
        {
            while (maxCount <= items.Count) removeLast();

            return addFirst(blob);
        }
        
        public void PrintOut(StreamWriter writer, Constants.OutputFormat format)
        {
            if (valid)
            {
                if (format.HasFlag(Constants.OutputFormat.ImmadiateArea))
                    writer.Write($"{items.First.Value.Area}{Constants.Delimeter}");

                if (format.HasFlag(Constants.OutputFormat.AverageArea))
                    writer.Write($"{averageArea}{Constants.Delimeter}");

                if (format.HasFlag(Constants.OutputFormat.Position))
                    writer.Write($"{items.First.Value.CenterOfGravity.X}{Constants.Delimeter}{items.First.Value.CenterOfGravity.Y}{Constants.Delimeter}");

                if (format.HasFlag(Constants.OutputFormat.Prediction))
                    writer.Write($"{PredictNext.X}{Constants.Delimeter}{PredictNext.Y}{Constants.Delimeter}");

                if (format.HasFlag(Constants.OutputFormat.AverageSpeed))
                    writer.Write($"{averageSpeed}{Constants.Delimeter}");

                if (format.HasFlag(Constants.OutputFormat.ImmediateSpeed))
                    writer.Write($"{distances.First.Value}{Constants.Delimeter}");
            }
            else
            {
                if (format.HasFlag(Constants.OutputFormat.ImmadiateArea))
                    writer.Write($"{Constants.Blank}{Constants.Delimeter}");

                if (format.HasFlag(Constants.OutputFormat.AverageArea))
                    writer.Write($"{Constants.Blank}{Constants.Delimeter}");

                if (format.HasFlag(Constants.OutputFormat.Position))
                    writer.Write($"{Constants.Blank}{Constants.Delimeter}{Constants.Blank}{Constants.Delimeter}");

                if (format.HasFlag(Constants.OutputFormat.Prediction))
                    writer.Write($"{Constants.Blank}{Constants.Delimeter}{Constants.Blank}{Constants.Delimeter}");

                if (format.HasFlag(Constants.OutputFormat.AverageSpeed))
                    writer.Write($"{Constants.Blank}{Constants.Delimeter}");

                if (format.HasFlag(Constants.OutputFormat.ImmediateSpeed))
                    writer.Write($"{Constants.Blank}{Constants.Delimeter}");
            }
        }

        public void PrintOutHeader(StreamWriter writer, Constants.OutputFormat format)
        {
            if (format.HasFlag(Constants.OutputFormat.ImmadiateArea))
                writer.Write($"Area{Constants.Delimeter}");

            if (format.HasFlag(Constants.OutputFormat.Position))
                writer.Write($"Pos X{Constants.Delimeter}Pos Y{Constants.Delimeter}");

            if (format.HasFlag(Constants.OutputFormat.Prediction))
                writer.Write($"Pred X{Constants.Delimeter}Pred Y{Constants.Delimeter}");

            if (format.HasFlag(Constants.OutputFormat.AverageSpeed))
                writer.Write($"Speed{Constants.Delimeter}");
        }

        public void PrintOutTag(StreamWriter writer, Constants.OutputFormat format)
        {
            writer.Write(Tag);

            if (format.HasFlag(Constants.OutputFormat.ImmadiateArea))
                writer.Write($"{Constants.Delimeter}");

            if (format.HasFlag(Constants.OutputFormat.Position))
                writer.Write($"{Constants.Delimeter}{Constants.Delimeter}");

            if (format.HasFlag(Constants.OutputFormat.Prediction))
                writer.Write($"{Constants.Delimeter}{Constants.Delimeter}");

            if (format.HasFlag(Constants.OutputFormat.AverageSpeed))
                writer.Write($"{Constants.Delimeter}");
        }

        public void Draw(Graphics gr, Constants.HighlightFormat format)
        {
            if (format.HasFlag(Constants.HighlightFormat.Object))
                gr.DrawEllipse(blobHighliter, First.Rectangle);

            if (format.HasFlag(Constants.HighlightFormat.Trace))
                //TODO: out of memory exception is being thrown
                if (items.Count > 1)
                    gr.DrawCurve(blobHighliter, (items.ForEach(blob => new PointF(blob.CenterOfGravity.X, blob.CenterOfGravity.Y))).ToArray());

            if (format.HasFlag(Constants.HighlightFormat.Tag))
                gr.DrawString(Tag, drawFont, drawBrush, First.Rectangle.Location);

            if (format.HasFlag(Constants.HighlightFormat.Prediction))
                gr.DrawEllipse(new Pen(drawBrush), MathFunctions.getRectangleFromRadius(PredictNext.ConverseToPoint(), 5, 5));

            //TODO: direction displayng doesn't work well
            //if (format.HasFlag(Extensions.HighlightFormat.Direction))
            //    gr.DrawLine(blobHighliter, First.CenterOfGravity.ConverseToPointF(), (PredictNext + First.CenterOfGravity.Multiply(3)).ConverseToPointF());
        }

        public void makeInvalid() => valid = false;
        //TODO: maybe more logic behind this...
    }
}
