using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Drawing;

using AForge.Imaging;

namespace FlyCatcher
{
    interface IKeeper<ItemType, MeasureType, DistanceType, PositionType> where MeasureType : IComparable where DistanceType : IComparable
    {
        ICollection<IData<ItemType, MeasureType, DistanceType, PositionType>> ItemsData { get; }

        string Tag { get; }

        void ActualizeData(IEnumerable<ItemType> items);
        void Refresh(IEnumerable<ItemType> items);

        void PrintOut(StreamWriter writer, Constants.OutputFormat format);
        void PrintOutHeader(StreamWriter writer, Constants.OutputFormat format);
        void PrintOutTag(StreamWriter writer, Constants.OutputFormat format);
        void Draw(Graphics gr, Constants.HighlightFormat format);
    }

    abstract class BlobKeeper : IKeeper<Blob, double, double, AForge.Point>
    {
        /// <summary>
        /// Collection of stored Blobs data.
        /// </summary>
        public ICollection<IData<Blob, double, double, AForge.Point>> ItemsData { get { return itemsData; } }

        protected List<IData<Blob, double, double, AForge.Point>> itemsData;
        protected int historyCount;

        /// <summary>
        /// Tag of the Keeper. Usually the mask Keeper is handling.
        /// </summary>
        public string Tag { get; private set; }

        private void init(string tag, int historyCount)
        {
            Tag = tag;
            this.historyCount = historyCount;
        }

        public BlobKeeper(IEnumerable<Blob> items, string tag, int historyCount)
        {
            init(tag, historyCount);
            Refresh(items);
        }

        public BlobKeeper(string tag, int historyCount)
        {
            init(tag, historyCount);
        }

        /// <summary>
        /// Inicialize ItemsData with new statring Enumeration.
        /// </summary>
        /// <param name="items">Enumeration is converted to IData<Blob> and inicialize ItemsData</param>
        public void Refresh(IEnumerable<Blob> items)
        {
            itemsData = new List<IData<Blob, double, double, AForge.Point>>();

            foreach (var blob in items)
                itemsData.Add(new BlobData(historyCount, blob, Tag));
        }

        /// <summary>
        /// Function that should update the collection ItemsData with new data.
        /// </summary>
        /// <param name="items">The items that should perform the updating</param>
        public abstract void ActualizeData(IEnumerable<Blob> items);// => ActualizeDataAssignmentTask(items.ToArray());       

        public void PrintOut(StreamWriter writer, Constants.OutputFormat format)
        {
            if (format.HasFlag(Constants.OutputFormat.Objects))
                writer.Write($"{itemsData.Where(dta => dta.Valid).Count()}{Constants.Delimeter}");

            foreach (var blobData in itemsData)
                blobData.PrintOut(writer, format);
        }
        public void PrintOutHeader(StreamWriter writer, Constants.OutputFormat format)
        {
            if (format.HasFlag(Constants.OutputFormat.Objects))
                writer.Write($"{Constants.Delimeter}");

            foreach (var blobData in itemsData)
                blobData.PrintOutHeader(writer, format);
        }
        public void PrintOutTag(StreamWriter writer, Constants.OutputFormat format)
        {
            if (format.HasFlag(Constants.OutputFormat.Objects))
                writer.Write($"{Constants.Delimeter}");

            foreach (var blobData in itemsData)
                blobData.PrintOutTag(writer, format);
        }
        public void Draw(Graphics gr, Constants.HighlightFormat format)
        {
            foreach (var blobData in itemsData.Where(dta => dta.Valid))
                blobData.Draw(gr, format);
        }
    }

    class BlobKeeperAssignment : BlobKeeper
    {
        private SquareMatrix matrix;

        private double getMatch(IData<Blob, double, double, AForge.Point> agent, Blob blob) => agent.GetMatch(blob);

        public void ActualizeData(Blob[] items)
        {
            var assignment = matrix.GetPerfectAssignment(itemsData, items, getMatch);

            foreach (var properMatch in assignment[0])
                itemsData[properMatch.Item1].AddItem(items[properMatch.Item2]);

            foreach (var taskMatch in assignment[1])
                itemsData.Add(new BlobData(historyCount, items[taskMatch.Item2], Tag));

            foreach (var agentMatch in assignment[2])
                itemsData[agentMatch.Item1].MakeInvalid();
        }

        public override void ActualizeData(IEnumerable<Blob> items) => ActualizeData(items.ToArray());

        public BlobKeeperAssignment(IEnumerable<Blob> items, string tag, int historyCount, double penalty) : base(items, tag, historyCount)
        {
            matrix = new SquareMatrix(penalty);
        }

        public BlobKeeperAssignment(string tag, int historyCount, double penalty) : base(tag, historyCount)
        {
            matrix = new SquareMatrix(penalty);
        }
    }

    class BlobKeeperClosestFirst : BlobKeeper
    {
        private SquareMatrix matrix = new SquareMatrix(10);
        private double getMatch(IData<Blob, double, double, AForge.Point> agent, Blob blob) => agent.GetMatch(blob);
        public void ActualizeData(Blob[] items)
        {
            int index = 0;
            double distMatch = double.PositiveInfinity;

            //TODO: watch out for duplicates
            //when two flies occupy the same space I can lose info about one of them
            foreach (var BlobData in ItemsData)
                if (items.Any(x => x != null))
                    for (int i = 0; i < items.Length; i++)
                    {
                        if (items[i] != null)
                        {
                            double dist = BlobData.GetMatch(items[i]);

                            if (dist <= distMatch)
                            {
                                index = i;
                                distMatch = dist;
                            }
                        }

                        BlobData.AddItem(items[index]);

                        items[index] = null;
                        distMatch = double.PositiveInfinity;
                    }
        }

        public override void ActualizeData(IEnumerable<Blob> items) => ActualizeData(items.ToArray());

        public BlobKeeperClosestFirst(IEnumerable<Blob> items, string tag, int historyCount) : base(items, tag, historyCount) { }
        public BlobKeeperClosestFirst(string tag, int historyCount) : base(tag, historyCount) { }
    }
}
