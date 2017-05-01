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
        protected IData<Blob, double, double, AForge.Point>[] validData
        {
            get
            {
                return (from dta in itemsData where dta.Valid select dta).ToArray();
            }
        }

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
                itemsData.Add(new BlobData(historyCount, blob, $"{Tag}-{ItemsData.Count}"));
        }

        /// <summary>
        /// Function that should update the collection ItemsData with new data.
        /// </summary>
        /// <param name="items">The items that should perform the updating</param>
        public abstract void ActualizeData(IEnumerable<Blob> items);

        #region Output
        public void PrintOut(StreamWriter writer, Constants.OutputFormat format)
        {
            if (format.HasFlag(Constants.OutputFormat.Objects))
                writer.Write($"{itemsData.Where(dta => dta.Valid).Count()}{Constants.Delimeter}");

            foreach (var blobData in itemsData)
                if (blobData.Valid)
                    blobData.PrintOut(writer, format);
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
            if (format.HasFlag(Constants.OutputFormat.Objects))
                writer.Write($"Object count{Constants.Delimeter}");

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
        #endregion
    }

    class BlobKeeperValidOnlyAssignment : BlobKeeper
    {
        private SquareMatrix matrix;

        private double getMatch(IData<Blob, double, double, AForge.Point> agent, Blob blob) => agent.GetMatch(blob);

        public void ActualizeData(Blob[] items)
        {
            var valids = validData;

            var assignment = matrix.GetPerfectAssignment(valids, items, getMatch);

            foreach (var properMatch in assignment[0])
                valids[properMatch.Item1].AddItem(items[properMatch.Item2]);

            foreach (var taskMatch in assignment[1])
                itemsData.Add(new BlobData(historyCount, items[taskMatch.Item2], $"{Tag}-{ItemsData.Count}"));

            foreach (var agentMatch in assignment[2])
                valids[agentMatch.Item1].MakeInvalid();
        }

        public override void ActualizeData(IEnumerable<Blob> items) => ActualizeData(items.ToArray());

        #region Ctors
        public BlobKeeperValidOnlyAssignment(IEnumerable<Blob> items, string tag, int historyCount, double penalty) : base(items, tag, historyCount)
        {
            matrix = new SquareMatrix(penalty);
        }

        public BlobKeeperValidOnlyAssignment(string tag, int historyCount, double penalty) : base(tag, historyCount)
        {
            matrix = new SquareMatrix(penalty);
        }
        #endregion
    }

    class BlobKeeperValidReplaceAssignment : BlobKeeper
    {
        private SquareMatrix matrix;

        private double getMatch(IData<Blob, double, double, AForge.Point> agent, Blob blob) => agent.GetMatch(blob) + (agent.Valid ? 0 : matrix.Penalty);

        public void ActualizeData(Blob[] items)
        {
            var assignment = matrix.GetPerfectAssignment(itemsData, items, getMatch);

            foreach (var properMatch in assignment[0])
                if (itemsData[properMatch.Item1].Valid)                
                    itemsData[properMatch.Item1].AddItem(items[properMatch.Item2]);
                else
                    itemsData.Add(new BlobData(historyCount, items[properMatch.Item2], $"{Tag}-{ItemsData.Count}"));

            foreach (var taskMatch in assignment[1])
                itemsData.Add(new BlobData(historyCount, items[taskMatch.Item2], $"{Tag}-{ItemsData.Count}"));

            foreach (var agentMatch in assignment[2])
                itemsData[agentMatch.Item1].MakeInvalid();
        }

        public override void ActualizeData(IEnumerable<Blob> items) => ActualizeData(items.ToArray());

        #region Ctors
        public BlobKeeperValidReplaceAssignment(IEnumerable<Blob> items, string tag, int historyCount, double penalty) : base(items, tag, historyCount)
        {
            matrix = new SquareMatrix(penalty);
        }

        public BlobKeeperValidReplaceAssignment(string tag, int historyCount, double penalty) : base(tag, historyCount)
        {
            matrix = new SquareMatrix(penalty);
        }
        #endregion
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