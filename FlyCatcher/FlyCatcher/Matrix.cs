using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        
        public double Penalty { private get; set; }

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
                            str.AppendFormat("\t{0}", Math.Round(matrix[row][column], 3));
                            break;
                        case Tag.Circled:
                            str.AppendFormat("\t({0})", Math.Round(matrix[row][column], 3));
                            break;
                        case Tag.Vertical:
                            str.AppendFormat("\t|{0}|", Math.Round(matrix[row][column], 3));
                            break;
                        case Tag.Horizontal:
                            str.AppendFormat("\t-{0}-", Math.Round(matrix[row][column], 3));
                            break;
                        case Tag.DoubleAssigned:
                            str.AppendFormat("\t#{0}#", Math.Round(matrix[row][column], 3));
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
            Dimension = Math.Max(matrix.GetUpperBound(0) + 1, matrix.GetUpperBound(1)+1);

            Penalty = penalty;

            initTags();
            initMatrix();

            for (int row = 0; row < Dimension; row++)
                for (int column = 0; column < Dimension; column++)
                    this.matrix[row][column] = matrix[row, column];
        }

        private double MinRow(int row)
        {
            return matrix.GetRow(row).Min();
        }
        private double MinColumn(int column)
        {
            return matrix.GetColumn(column).Min();
        }

        private void MinimizeRow(int rowIndex)
        {
            double min = MinRow(rowIndex);
            matrix.iterateOverRow(rowIndex, val => val - min);
        }
        private void MinimizeColumn(int columnIndex)
        {
            double min = MinColumn(columnIndex);
            matrix.iterateOverColumn(columnIndex, val => val - min);
        }

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

        private enum Tag { Free, Circled, Vertical, Horizontal, DoubleAssigned }
        private bool unassignedZeros()
        {
            for (int row = 0; row < Dimension; row++)
                for (int column = 0; column < Dimension; column++)
                    if (matrix[row][column].isZero() && tags[row][column] == Tag.Free)
                        return true;

            return false;
        }

        private int freeZerosInRow(int row)
        {
            int counter = 0;
            for (int column = 0; column < Dimension; column++)
                if (matrix[row][column].isZero() && tags[row][column] == Tag.Free)
                    counter++;

            return counter;
        }
        private int freeZerosInColumn(int column)
        {
            int counter = 0;
            for (int row = 0; row < Dimension; row++)
                if (matrix[row][column].isZero() && tags[row][column] == Tag.Free)
                    counter++;

            return counter;
        }

        private bool rowAssigned(int row) => tags.GetRow(row).Any(tag => tag == Tag.Horizontal || tag == Tag.DoubleAssigned);
        private bool columnAssigned(int column)=>tags.GetColumn(column).Any(tag => tag == Tag.Vertical || tag == Tag.DoubleAssigned);
        
        private Point circleZero()
        {
            Point pt = nullPoint;

            for (int row = 0; row < Dimension; row++)
            {
                if (rowAssigned(row))
                    continue;

                for (int column = 0; column < Dimension; column++)
                {
                    if (columnAssigned(column))
                        continue;

                    if (matrix[row][column].isZero() &&
                        tags[row][column] == Tag.Free &&
                        !tags.GetRow(row).Any(tag => tag == Tag.Circled) &&
                        !tags.GetColumn(column).Any(tag => tag == Tag.Circled))
                    {
                        pt = new Point(row, column);

                        if (freeZerosInRow(row) == 1 || freeZerosInColumn(column) == 1)
                        {
                            tags[pt.X][pt.Y] = Tag.Circled;
                            return pt;
                        }

                    }
                }
            }

            if (pt != nullPoint)
                tags[pt.X][pt.Y] = Tag.Circled;

            return pt;
        }

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

        private void Assign()
        {
            //Position of zero in the matrix
            Point zeroPosition;

            while (unassignedZeros())
            {
                zeroPosition = circleZero();

                if (freeZerosInRow(zeroPosition.X) >= freeZerosInColumn(zeroPosition.Y))
                    assignRow(zeroPosition.X);
                else
                    assignColumn(zeroPosition.Y);
            }
        }

        private List<Tuple<int, int>> GetAssignment()
        {
            Assign();

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
