using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;

using AForge.Imaging;
using AForge.Imaging.Filters;

namespace FlyCatcher
{
    interface IKeeper<ItemType, MeasureType, DistanceType, PositionType> where MeasureType : IComparable where DistanceType : IComparable
    {
        ICollection<IData<ItemType, MeasureType, DistanceType, PositionType>> ItemsData { get; }

        string Tag { get; }

        void ActualizeData(IEnumerable<ItemType> items);
        void Refresh(IEnumerable<ItemType> items);
        void Init(IEnumerable<ItemType> items, string tag);

        void PrintOut(StreamWriter writer, Constants.OutputFormat format);
        void PrintOutHeader(StreamWriter writer, Constants.OutputFormat format);
        void PrintOutTag(StreamWriter writer, Constants.OutputFormat format);
        void Draw(Graphics gr, Constants.HighlightFormat format);
    }

    class BlobKeeper : IKeeper<Blob, double, double, AForge.Point>
    {
        /// <summary>
        /// Collection of stored Blobs data.
        /// </summary>
        public ICollection<IData<Blob, double, double, AForge.Point>> ItemsData { get { return itemsData; } }
        private List<IData<Blob, double, double, AForge.Point>> itemsData;

        private int historyCount;

        /// <summary>
        /// Tag of the Keeper. Usually the mask Keeper is handling.
        /// </summary>
        public string Tag { get; private set; }

        public BlobKeeper(IEnumerable<Blob> items, string tag, int historyCount)
        {
            Init(items, tag);

            this.historyCount = historyCount;
        }

        public BlobKeeper(string tag, int historyCount)
        {
            Init(tag);

            this.historyCount = historyCount;
        }

        /// <summary>
        /// Inicialize ItemsData with new tag.
        /// </summary>       
        /// <param name="tag">Tag that is assigned to Tag property</param>
        public void Init(string tag)
        {
            Tag = tag;
        }

        /// <summary>
        /// Inicialize ItemsData with new statring Enumeration.
        /// </summary>
        /// <param name="items">Enumeration is converted to IData<Blob> and inicialize ItemsData</param>
        /// <param name="tag">Tag that is assigned to Tag property</param>
        public void Init(IEnumerable<Blob> items, string tag)
        {
            Refresh(items);

            Tag = tag;
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

            matrix = new SquareMatrix(10);
        }

        /// <summary>
        /// Function that should update the collection ItemsData with new data.
        /// </summary>
        /// <param name="items">The items that should perform the updating</param>
        public void ActualizeData(IEnumerable<Blob> items) => ActualizeDataAssignmentTask(items.ToArray());

        public void ActualizeDataClosestFirst(Blob[] items)
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

        private SquareMatrix matrix = new SquareMatrix(10);
        private double getMatch(IData<Blob, double, double, AForge.Point> agent, Blob blob) => agent.GetMatch(blob);
        public void ActualizeDataAssignmentTask(Blob[] items)
        {
            var assignment = matrix.GetPerfectAssignment(itemsData, items, getMatch);

            foreach (var properMatch in assignment[0])
                itemsData[properMatch.Item1].AddItem(items[properMatch.Item2]);

            foreach (var taskMatch in assignment[1])
                itemsData.Add(new BlobData(historyCount, items[taskMatch.Item2], Tag));

            //TODO: this is not so wrong...
            foreach (var agentMatch in assignment[2])
                itemsData[agentMatch.Item1].MakeInvalid();
        }

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
}
