using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Collections;

namespace FlyCatcher
{
    class SquareMatrix : IEnumerable<double>
    {
        public double this[Point pt] { get { return matrix[pt.X][pt.Y]; } set { matrix[pt.X][pt.Y] = value; } }
        private double[][] matrix;
        private Tag[][] tags;
        public int Dimension { get; private set; }

        public double Penalty { get; set; }

        public int ValidTasks { get; private set; }
        public int ValidAgents { get; private set; }

        private static Point nullPoint = new Point(-1, -1);

        public override string ToString()
        {
            StringBuilder str = new StringBuilder();
            for (int row = 0; row < Dimension; row++)
            {
                for (int column = 0; column < Dimension; column++)
                {
                    switch (tags[row][column])
                    {
                        case Tag.Free:
                            str.AppendFormat("\t*{0,6}*", Math.Round(matrix[row][column], 3));
                            break;
                        case Tag.Circled:
                            str.AppendFormat("\t({0,6})", Math.Round(matrix[row][column], 3));
                            break;
                        case Tag.Vertical:
                            str.AppendFormat("\t|{0,6}|", Math.Round(matrix[row][column], 3));
                            break;
                        case Tag.Horizontal:
                            str.AppendFormat("\t-{0,6}-", Math.Round(matrix[row][column], 3));
                            break;
                        case Tag.DoubleAssigned:
                            str.AppendFormat("\t#{0,6}#", Math.Round(matrix[row][column], 3));
                            break;
                    }
                }
                str.AppendLine();
            }
            str.AppendLine();
            return str.ToString();
        }

        private void initTags()
        {
            tags = new Tag[Dimension][];
            for (int i = 0; i < Dimension; i++)
                tags[i] = new Tag[Dimension];
        }
        private void initMatrix()
        {
            matrix = new double[Dimension][];
            for (int i = 0; i < Dimension; i++)
                matrix[i] = new double[Dimension];

            matrix.SetAll(Penalty);
        }
        public void initMatrix<AgentType, TaskType>(IEnumerable<AgentType> Agents, IEnumerable<TaskType> Tasks, Extensions.DistanceGetter<AgentType, TaskType> getDistance)
        {
            //if (Agents.Count() > Dimension || Tasks.Count() > Dimension)
            //    throw new ArgumentException("The matrix has to be squared.");

            ValidAgents = Agents.Count();
            ValidTasks = Tasks.Count();

            Dimension = 2 * Math.Max(ValidTasks, ValidAgents);

            initMatrix();
            initTags();

            int row = 0, column = 0;
            foreach (var Agent in Agents)
            {
                foreach (var Task in Tasks)
                {
                    matrix[row][column] = getDistance(Agent, Task);
                    column++;
                }
                column = 0;
                row++;
            }
        }

        public void initMatrix(double[][] distances)
        {
            //if (Agents.Count() > Dimension || Tasks.Count() > Dimension)
            //    throw new ArgumentException("The matrix has to be squared.");

            ValidAgents = distances.Count();
            ValidTasks = distances[0].Count();

            Dimension = 2 * Math.Max(ValidTasks, ValidAgents);

            initMatrix();
            initTags();

            for (int row = 0; row < ValidAgents; row++)
                for (int column = 0; column < ValidTasks; column++)
                    matrix[row][column] = distances[row][column];

        }

        public SquareMatrix(double penalty)
        {
            Penalty = penalty;
        }
        public SquareMatrix(double[][] matrix, double penalty)
        {
            Dimension = Math.Max(matrix.Length, matrix[0].Length);

            Penalty = penalty;

            this.matrix = matrix;
            initTags();
        }
        public SquareMatrix(double[,] matrix, double penalty)
        {
            Dimension = Math.Max(matrix.GetUpperBound(0) + 1, matrix.GetUpperBound(1) + 1);

            Penalty = penalty;

            initTags();
            initMatrix();

            for (int row = 0; row < Dimension; row++)
                for (int column = 0; column < Dimension; column++)
                    this.matrix[row][column] = matrix[row, column];
        }

        private double MinRow(int row) => matrix.GetRow(row).Min();
        private void MinimizeRow(int rowIndex) => MinimizeRowBy(rowIndex, MinRow(rowIndex));
        private void MinimizeRowBy(int rowIndex, double by) => matrix.iterateOverRow(rowIndex, val => val - by);

        private double MinColumn(int column) => matrix.GetColumn(column).Min();
        private void MinimizeColumn(int columnIndex) => MinimizeColumnBy(columnIndex, MinColumn(columnIndex));
        private void MinimizeColumnBy(int columnIndex, double by) => matrix.iterateOverColumn(columnIndex, val => val - by);

        private void Minimize()
        {
            //Could be written in two more readable for's - one for rows one for columns
            //but this does the same job -> thx to #Rows == #Columns == Dimension.
            for (int index = 0; index < Dimension; index++)
            {
                MinimizeRow(index);
                MinimizeColumn(index);
            }

        }

        private delegate bool Predicate(int row, int column);
        private int predictateCounterInRow(int row, Predicate pred)
        {
            int counter = 0;
            for (int column = 0; column < Dimension; column++)
                if (pred(row, column))
                    counter++;

            return counter;
        }
        private int predictateCounterInColumn(int column, Predicate pred)
        {
            int counter = 0;
            for (int row = 0; row < Dimension; row++)
                if (pred(row, column))
                    counter++;

            return counter;
        }

        private enum Tag { Free, Circled, Vertical, Horizontal, DoubleAssigned }
        private bool isUnassignedZero(int row, int column) => (matrix[row][column].isZero() &&
                                                               tags[row][column] == Tag.Free);

        private bool unassignedZero(out Point pt)
        {
            pt = nullPoint;

            for (int row = 0; row < Dimension; row++)
                for (int column = 0; column < Dimension; column++)
                    if (isUnassignedZero(row, column))
                    {
                        pt = new Point(row, column);
                        return true;
                    }

            return false;
        }

        private int freeZerosInRow(int row) => predictateCounterInRow(row, isUnassignedZero);
        private int freeZerosInColumn(int column) => predictateCounterInColumn(column, isUnassignedZero);

        private bool isCircleableZero(int row, int column) => (matrix[row][column].isZero() &&
                                                       !tags.GetRow(row).Any(tag => tag == Tag.Circled) &&
                                                       !tags.GetColumn(column).Any(tag => tag == Tag.Circled));

        private int circleableZerosInRow(int row) => predictateCounterInRow(row, isCircleableZero);
        private int circleableZerosInColumn(int column) => predictateCounterInColumn(column, isCircleableZero);

        private bool rowAssigned(int row) => tags.GetRow(row).Any(tag => tag == Tag.Horizontal || tag == Tag.DoubleAssigned);
        private bool columnAssigned(int column) => tags.GetColumn(column).Any(tag => tag == Tag.Vertical || tag == Tag.DoubleAssigned);

        private void assignRow(int row)
        {
            for (int column = 0; column < Dimension; column++)
                switch (tags[row][column])
                {
                    case Tag.Free:
                        tags[row][column] = Tag.Horizontal;
                        break;
                    case Tag.Vertical:
                        tags[row][column] = Tag.DoubleAssigned;
                        break;
                    case Tag.Circled:
                        //Nothing happens to already circled zeros
                        break;
                    case Tag.DoubleAssigned:
                    case Tag.Horizontal:
                    default:
                        //Other options should not happen
                        throw new Exception("This row has already been assigned to.");
                }
        }
        private void assignColumn(int column)
        {
            for (int i = 0; i < Dimension; i++)
                switch (tags[i][column])
                {
                    case Tag.Free:
                        tags[i][column] = Tag.Vertical;
                        break;
                    case Tag.Horizontal:
                        tags[i][column] = Tag.DoubleAssigned;
                        break;
                    case Tag.Circled:
                        //Nothing happens to already circled zeros
                        break;
                    case Tag.DoubleAssigned:
                    case Tag.Vertical:
                    default:
                        //Other options should not happen
                        throw new Exception("This column has already been assigned to.");
                }
        }

        private bool CircleAssignedZeros()
        {
            bool circledAny = false;

            for (int row = 0; row < Dimension; row++)
                for (int column = 0; column < Dimension; column++)
                    if (isCircleableZero(row, column))
                        if (circleableZerosInRow(row) == 1 || circleableZerosInColumn(column) == 1)
                        {
                            tags[row][column] = Tag.Circled;
                            circledAny = true;
                        }

            for (int row = 0; row < Dimension; row++)
                for (int column = 0; column < Dimension; column++)
                    if (isCircleableZero(row, column))
                    {
                        tags[row][column] = Tag.Circled;
                        return true;
                    }

            return circledAny;
        }

        private void Assign()
        {
            //Position of zero in the matrix
            Point zeroPosition;

            while (unassignedZero(out zeroPosition))
                if (freeZerosInRow(zeroPosition.X) >= freeZerosInColumn(zeroPosition.Y))
                    assignRow(zeroPosition.X);
                else
                    assignColumn(zeroPosition.Y);

            //Console.WriteLine(ToString());
            while (CircleAssignedZeros());            
        }

        //private bool isFreeZero(int row, int column) => matrix[row][column].isZero() &&
        //                                                !tags.GetColumn(column).Any(tag => tag == Tag.Circled);
        //private int countFreeZeros(int row, out Point pt)
        //{
        //    pt = nullPoint;
        //    int counter = 0;

        //    for (int column = 0; column < Dimension; column++)
        //        if (isFreeZero(row, column))
        //        {
        //            pt = new Point(row, column);
        //            counter++;
        //        }

        //    return counter;
        //}

        //private int countZeros(int column)
        //{
        //    int counter = 0;

        //    for (int row = 0; row < Dimension; row++)
        //        if (matrix[row][column].isZero())
        //            counter++;

        //    return counter;
        //}

        //private List<int> supp(List<int> lst)
        //{
        //    List<int> ret = new List<int>();

        //    lst.Sort();
        //    int i = 0;

        //    foreach (int item in lst)
        //    {
        //        while (item != i && i < Dimension)
        //        {
        //            ret.Add(i);
        //            i++;
        //        }

        //        i++;
        //    }

        //    while (i < Dimension)
        //    {
        //        ret.Add(i);
        //        i++;
        //    }

        //    return ret;
        //}

        //private void Assign()
        //{
        //    Point zero;
        //    List<int> rowMarks = new List<int>();

        //    for (int row = 0; row < Dimension; row++)
        //        switch (countFreeZeros(row, out zero))
        //        {
        //            case 0:
        //                rowMarks.Add(row);
        //                break;
        //            case 1:
        //            default:
        //                tags[zero.X][zero.Y] = Tag.Circled;
        //                break;
        //        }

        //    List<int> columnMarks = new List<int>();

        //    foreach (var row in rowMarks)
        //        for (int column = 0; column < Dimension; column++)
        //            if (matrix[row][column].isZero())
        //            {
        //                columnMarks.Add(column);
        //                break;
        //            }

        //    foreach (var column in columnMarks)
        //        for (int row = 0; row < Dimension; row++)
        //            if (tags[row][column] == Tag.Circled && !rowMarks.Contains(row))
        //            {
        //                rowMarks.Add(row);
        //                break;
        //            }

        //    foreach (var column in columnMarks)
        //    {       Console.WriteLine($"Column mark: {column}");
        //    assignColumn(column);
        //}

        //    foreach (var row in rowMarks)
        //        Console.WriteLine($"Row mark: {row}");

        //    foreach (var row in supp(rowMarks))
        //    {
        //        Console.WriteLine($"Row un-mark: {row}");
        //        assignRow(row);
        //    }
        //}

        private List<Tuple<int, int>> GetAssignment()
        {
            Assign();

            //Console.WriteLine(ToString());

            List<Tuple<int, int>> assignment = new List<Tuple<int, int>>(Dimension);

            for (int row = 0; row < Dimension; row++)
                for (int column = 0; column < Dimension; column++)
                    if (tags[row][column] == Tag.Circled)
                        assignment.Add(new Tuple<int, int>(row, column));

            return assignment;
        }

        private void Augment()
        {
            double min = double.PositiveInfinity;

            for (int row = 0; row < Dimension; row++)
                for (int column = 0; column < Dimension; column++)
                    if (tags[row][column] == Tag.Free && matrix[row][column] < min)
                        min = matrix[row][column];

            for (int row = 0; row < Dimension; row++)
                for (int column = 0; column < Dimension; column++)
                    switch (tags[row][column])
                    {
                        case Tag.Free:
                            matrix[row][column] -= min;
                            break;
                        case Tag.DoubleAssigned:
                            matrix[row][column] += min;
                            break;
                        default:
                            break;
                    }
        }

        /// <summary>
        /// Solves the assignment task for inited matrix.
        /// </summary>
        /// <returns>Returns array of matches:
        /// 0 ~ proper match
        /// 1 ~ tasks which did not match any agent
        /// 2 ~ agents that did not match any task
        /// </returns>
        public IEnumerable<Tuple<int, int>>[] GetPerfectAssignment()
        {
            Minimize();

            List<Tuple<int, int>> assignment = GetAssignment();

            while (assignment.Count != Dimension)
            {
                Augment();
                initTags();

                assignment = GetAssignment();
            }

            return FilterOutput(assignment);
        }

        /// <summary>
        /// Solves the assignment task for given data.
        /// </summary>
        /// <returns>Returns array of matches:
        /// 0 ~ proper match
        /// 1 ~ tasks which did not match any agent
        /// 2 ~ agents that did not match any task
        /// </returns>
        public IEnumerable<Tuple<int, int>>[] GetPerfectAssignment<AgentType, TaskType>(IEnumerable<AgentType> Agents, IEnumerable<TaskType> Tasks, Extensions.DistanceGetter<AgentType, TaskType> getDistance)
        {
            initMatrix(Agents, Tasks, getDistance);
            return GetPerfectAssignment();
        }

        public IEnumerable<Tuple<int, int>>[] GetPerfectAssignment(double[][] distances)
        {
            initMatrix(distances);
            return GetPerfectAssignment();
        }

        private IEnumerable<Tuple<int, int>>[] FilterOutput(List<Tuple<int, int>> assignment)
        {
            IEnumerable<Tuple<int, int>>[] output = new IEnumerable<Tuple<int, int>>[3];

            //Selects all validly assigned pair Agent - Task
            output[0] = from x in assignment
                        where x.Item1 < ValidAgents && x.Item2 < ValidTasks
                        select x;

            //Selects all tasks with no match in agents
            output[1] = from x in assignment
                        where x.Item1 >= ValidAgents && x.Item2 < ValidTasks
                        select x;

            //Selects all agents with no match in tasks
            output[2] = from x in assignment
                        where x.Item1 < ValidAgents && x.Item2 >= ValidTasks
                        select x;

            return output;
        }

        public IEnumerator<double> GetEnumerator()
        {
            for (int row = 0; row < Dimension; row++)
                for (int column = 0; column < Dimension; column++)
                    yield return matrix[row][column];
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
