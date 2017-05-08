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
 
        #region Debug
        private void print()
        {
            for (int row = 0; row < Dimension; row++)
            {
                for (int column = 0; column < Dimension; column++)
                    switch (tags[row][column])
                    {
                        case Tag.Free:
                            Console.Write(". ");
                            break;
                        case Tag.Circled:
                            Console.Write("O ");
                            break;
                        case Tag.Taged:
                            Console.Write("* ");
                            break;
                        case Tag.Vertical:
                            Console.Write("| ");
                            break;
                        case Tag.Horizontal:
                            Console.Write("- ");
                            break;
                        case Tag.DoubleAssigned:
                            Console.Write("# ");
                            break;
                        default:
                            break;
                    }

                Console.WriteLine();
            }
        }
        private void printOut()
        { Console.WriteLine(ToString());}

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
                        case Tag.Taged:
                            str.AppendFormat("\t {0,6}'", Math.Round(matrix[row][column], 3));
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
        #endregion

        #region Init       
        private void initTags()
        {
            tags = new Tag[Dimension][];
            for (int i = 0; i < Dimension; i++)
                tags[i] = new Tag[Dimension];
        }
        private void removeTags()
        {
            for (int row = 0; row < Dimension; row++)
                for (int column = 0; column < Dimension; column++)
                    if (tags[row][column] != Tag.Circled)
                        tags[row][column]  = Tag.Free;
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
        #endregion

        #region Ctors        
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
        #endregion

        #region Preparation
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
        #endregion

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

        private enum Tag { Free, Circled, Taged, Vertical, Horizontal, DoubleAssigned }
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
                    case Tag.Taged:
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
                    case Tag.Taged:
                        //Nothing happens to already circled zeros
                        break;
                    case Tag.DoubleAssigned:
                    case Tag.Vertical:
                    default:
                        //Other options should not happen
                        throw new Exception("This column has already been assigned to.");
                }
        }

        private void unAssignRow(int row)
        {
            for (int column = 0; column < Dimension; column++)
                switch (tags[row][column])
                {
                    case Tag.Horizontal:
                        tags[row][column] = Tag.Free;
                        break;
                    case Tag.DoubleAssigned:
                        tags[row][column] = Tag.Vertical;
                        break;
                    case Tag.Circled:
                    case Tag.Taged:
                        //Nothing happens to already circled zeros
                        break;
                    case Tag.Vertical:
                    case Tag.Free:
                    default:
                        //Other options should not happen
                        throw new Exception("This row is not assigned.");
                }
        }
        private void unAssignColumn(int column)
        {
            for (int row = 0; row < Dimension; row++)
                switch (tags[row][column])
                {
                    case Tag.Vertical:
                        tags[row][column] = Tag.Free;
                        break;
                    case Tag.DoubleAssigned:
                        tags[row][column] = Tag.Horizontal;
                        break;
                    case Tag.Circled:
                    case Tag.Taged:
                        //Nothing happens to already circled zeros
                        break;

                    case Tag.Horizontal:
                    case Tag.Free:
                    default:
                        //Other options should not happen
                        throw new Exception("This row is not assigned.");
                }
        }

        //private bool CircleAssignedZeros()
        //{
        //    bool circledAny = false;

        //    for (int row = 0; row < Dimension; row++)
        //        for (int column = 0; column < Dimension; column++)
        //            if (isCircleableZero(row, column))
        //                if (circleableZerosInRow(row) == 1 || circleableZerosInColumn(column) == 1)
        //                {
        //                    tags[row][column] = Tag.Circled;
        //                    circledAny = true;
        //                }

        //    for (int row = 0; row < Dimension; row++)
        //        for (int column = 0; column < Dimension; column++)
        //            if (isCircleableZero(row, column))
        //            {
        //                tags[row][column] = Tag.Circled;
        //                return true;
        //            }

        //    return circledAny;
        //}

        //private void Assign()
        //{
        //    //Position of zero in the matrix
        //    Point zeroPosition;

        //    while (unassignedZero(out zeroPosition))
        //        if (freeZerosInRow(zeroPosition.X) >= freeZerosInColumn(zeroPosition.Y))
        //            assignRow(zeroPosition.X);
        //        else
        //            assignColumn(zeroPosition.Y);

        //    //TODO: fix this method - see problem.csv
        //    while (CircleAssignedZeros());            
        //}

        //private bool CircleAssignedZeros()
        //{
        //    bool circledAny = false;

        //    for (int row = 0; row < Dimension; row++)
        //        for (int column = 0; column < Dimension; column++)
        //            if (isCircleableZero(row, column))
        //                if (circleableZerosInRow(row) == 1 || circleableZerosInColumn(column) == 1)
        //                {
        //                    tags[row][column] = Tag.Circled;
        //                    circledAny = true;
        //                }

        //    for (int row = 0; row < Dimension; row++)
        //        for (int column = 0; column < Dimension; column++)
        //            if (isCircleableZero(row, column))
        //            {
        //                tags[row][column] = Tag.Circled;
        //                return true;
        //            }

        //    return circledAny;
        //}

        //private void Assign()
        //{
        //    SortedSet<int> markRows = new SortedSet<int>();
        //    SortedSet<int> markColumns = new SortedSet<int>();

        //    //SortedSet<int> assignedRows = new SortedSet<int>();
        //    //SortedSet<int> assignedColumns = new SortedSet<int>();


        //    for (int row = 0; row < Dimension; row++)
        //        if (freeZerosInRow(row) == 0)
        //            markRows.Add(row);
        //        else
        //        {
        //            for (int column = 0; column < Dimension; column++)
        //                if (isUnassignedZero(row, column))
        //                { assignColumn(column); break; }

        //            assignRow(row);
        //        }


        //    for (int column = 0; column < Dimension; column++)
        //        if (!markColumns.Contains(column))
        //            foreach (var row in markRows)
        //                if (matrix[row][column].isZero())
        //                    markColumns.Add(column);

        //    for (int row = 0; row < Dimension; row++)
        //        if (!markRows.Contains(row))
        //            foreach (var column in markColumns)
        //                if (matrix[row][column].isZero() && tags[row][column] != Tag.Free)
        //                    markRows.Add(row);

        //    initTags();

        //    for (int row = 0; row < Dimension; row++)
        //        if (!markRows.Contains(row))
        //            assignRow(row);

        //    foreach (var column in markColumns)
        //        assignColumn(column);

        //    while (CircleAssignedZeros()) ;
        //}
        //private void Augment()
        //{
        //    double min = double.PositiveInfinity;

        //    for (int row = 0; row < Dimension; row++)
        //        for (int column = 0; column < Dimension; column++)
        //            if (tags[row][column] == Tag.Free && matrix[row][column] < min)
        //                min = matrix[row][column];

        //    for (int row = 0; row < Dimension; row++)
        //        for (int column = 0; column < Dimension; column++)
        //            switch (tags[row][column])
        //            {
        //                case Tag.Free:
        //                    matrix[row][column] -= min;
        //                    break;
        //                case Tag.DoubleAssigned:
        //                    matrix[row][column] += min;
        //                    break;
        //                default:
        //                    break;
        //            }
        //}

        private void CircleZero(int row, int column)
        {
            assignColumn(column);
            tags[row][column] = Tag.Circled;            
        }

        private bool FindCircledInColumn(int column, out Point circeldZero)
        {
            circeldZero = nullPoint;

            for (int row = 0; row < Dimension; row++)
                if (tags[row][column] == Tag.Circled)
                {
                    circeldZero = new Point(row, column);
                    return true;
                }

            return false;
        }

        private Point FindTagedInRow(int row)
        {
            for (int column = 0; column < Dimension; column++)
                if (tags[row][column] == Tag.Taged)                
                    return new Point(row, column);

            return nullPoint;
        }

        private void CircleNew(Point zero)
        {
            Stack<Point> zeros = new Stack<Point>(Dimension);
            zeros.Push(zero);

            while (FindCircledInColumn(zeros.Peek().Y, out zero))
            {
                zeros.Push(zero);
                zeros.Push(FindTagedInRow(zero.X));
            }
            
            removeTags();

            zero = zeros.Pop();
            CircleZero(zero.X, zero.Y);
            
            while (zeros.Count > 0)
            {                
                zero = zeros.Pop();
                tags[zero.X][zero.Y] = Tag.Free;

                zero = zeros.Pop();
                CircleZero(zero.X, zero.Y);
            }

            for (int column = 0; column < Dimension; column++)
                for (int row = 0; row < Dimension; row++)
                    if (tags[row][column] == Tag.Circled && !columnAssigned(column))
                    { assignColumn(column); break; }
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

        private bool Assign(out Point zero)
        {
            while (true)
            {
                while (unassignedZero(out zero))
                {
                    tags[zero.X][zero.Y] = Tag.Taged;

                    for (int column = 0; column < Dimension; column++)
                        if (tags[zero.X][column] == Tag.Circled)
                        {
                            unAssignColumn(column);
                            assignRow(zero.X);
                            return true;
                        }

                    return false;
                }

                Augment();
            }            
        }

        private List<Tuple<int, int>> GetActualAssignment()
        {
            List<Tuple<int, int>> assignment = new List<Tuple<int, int>>(Dimension);

            for (int row = 0; row < Dimension; row++)
                for (int column = 0; column < Dimension; column++)
                    if (tags[row][column] == Tag.Circled)
                        assignment.Add(new Tuple<int, int>(row, column));

            return assignment;
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

            for (int row = 0; row < Dimension; row++)
                for (int column = 0; column < Dimension; column++)
                    if (isCircleableZero(row, column))
                        CircleZero(row, column);

            List<Tuple<int, int>> assignment = GetActualAssignment();
            Point zero;

            while (assignment.Count != Dimension)
            {
                while (Assign(out zero)) ;           
                    
                
                CircleNew(zero);
                
                assignment = GetActualAssignment();
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
