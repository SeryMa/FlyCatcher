using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AForge.Imaging;
using AForge.Imaging.Filters;

namespace FlyCatcher
{
    interface IKeeper<ItemType, MeasureType, DistanceType> where MeasureType : IComparable where DistanceType : IComparable
    {
        ICollection<IData<ItemType, MeasureType, DistanceType>> ItemsData { get; }

        string Tag { get; }        

        void ActualizeData(IEnumerable<ItemType> items);
        //void ActualizeData(ItemType[] items);
        void Refresh(IEnumerable<ItemType> items);
        void Init(IEnumerable<ItemType> items, string tag, Extensions.isInCurve curveFunction);
    }

    class BlobKeeper : IKeeper<Blob, double, double>
    {
        /// <summary>
        /// Collection of stored Blobs data.
        /// </summary>
        public ICollection<IData<Blob, double, double>> ItemsData { get { return itemsData; }}
        private List<IData<Blob, double, double>> itemsData;

        //TODO: remove if remains unused
        private Extensions.isInCurve curveFunction;

        /// <summary>
        /// Tag of the Keeper. Usually the mask Keeper is handling.
        /// </summary>
        public string Tag { get; private set; }

        public BlobKeeper(IEnumerable<Blob> items, string tag, Extensions.isInCurve curveFunction)
        {
            Init(items, tag, curveFunction);
        }

        /// <summary>
        /// Inicialize ItemsData with new statring Enumeration.
        /// </summary>
        /// <param name="items">Enumeration is converted to IData<Blob> and inicialize ItemsData</param>
        /// <param name="tag">Tag that is assigned to Tag property</param>
        public void Init(IEnumerable<Blob> items, string tag, Extensions.isInCurve curveFunction)
        {
            Refresh(items);

            this.curveFunction = curveFunction;
            Tag = tag;
        }

        /// <summary>
        /// Inicialize ItemsData with new statring Enumeration.
        /// </summary>
        /// <param name="items">Enumeration is converted to IData<Blob> and inicialize ItemsData</param>
        public void Refresh(IEnumerable<Blob> items)
        {
            itemsData = new List<IData<Blob, double, double>>();

            foreach (var blob in items)
                itemsData.Add(new BlobData(10, blob, Tag));


            matrix = new SquareMatrix(10);
        }

        public void ActualizeData(IEnumerable<Blob> items) => ActualizeDataAssignmentTask(items.ToArray()); 

        /// <summary>
        /// Function that should update the collection ItemsData with new data.
        /// </summary>
        /// <param name="items">The items that should perform the updating</param>
        public void ActualizeDataClosestFirst(Blob[] items)
        {
            int index = 0;
            double distMatch = double.PositiveInfinity;

            //TODO: watch out for duplicates
            //when two flies occupy the same space I can lose info about one of them
            foreach (var BlobData in ItemsData)
            {
                if (items.Any(x => x != null))
                {
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
            }
        }

        private IEnumerable<Blob> DetectMerges(IEnumerable<Blob> items)
        {
            List<Blob> merges = new List<Blob>();
            foreach (var keeperA in itemsData)
                foreach (var keeperB in itemsData)
                    if (keeperA != keeperB &&
                        keeperA.PredictNext.DistanceTo(keeperB.PredictNext) < keeperA.First.Rectangle.getDiameter() + keeperB.First.Rectangle.getDiameter())
                        return null; //TODO    
                
            return items;
        }

        private IEnumerable<Blob> CatchRuners(IEnumerable<Blob> items)
        {
            return items.Concat(from dta in itemsData where curveFunction(dta.PredictNext) select dta.First);
        }

        private IEnumerable<Blob> FilterNoise(IEnumerable<Blob> items)
        {
            return items;

            //TODO: filter out the noise
            double averageSize = (from keeper in itemsData select keeper.averageSize).Average();
            return items.Where(blob => averageSize.isSameAs(blob.Area, 10));            
        }

        private SquareMatrix matrix = new SquareMatrix(10);
        private double getMatch(IData<Blob, double, double> agent, Blob blob)
        {
            return agent.GetMatch(blob);
        }
        public void ActualizeDataAssignmentTask(Blob[] items)
        {
            //Blob[] clearedItems = FilterNoise(DetectMerges(CatchRuners(items))).ToArray();
            Blob[] clearedItems = items;
            
            var assignment = matrix.GetPerfectAssignment(itemsData, clearedItems, getMatch);

            foreach (var properMatch in assignment[0])
                itemsData[properMatch.Item1].AddItem(clearedItems[properMatch.Item2]);

            foreach (var taskMatch in assignment[1])
                itemsData.Add(new BlobData(10, clearedItems[taskMatch.Item2], Tag));

            //TODO: this is wrong...
            foreach (var agentMatch in assignment[2])
                itemsData[agentMatch.Item1].AddItem(itemsData[agentMatch.Item1].First);

        }
    }
}
