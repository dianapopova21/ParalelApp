using System;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;
using MPI;

class Program
{
    // Функція для рахування ваги елемента в рядку або стовпці
    static double ComputeWeight(double[] arr)
    {
        double mean = arr.Average();
        return arr.Select(x => Math.Abs(x - mean)).Sum();
    }

    // Функція для обрахування ваг у підматриці
    static Tuple<double, Tuple<int, int>[]> ComputeWeightSubM(Matrix<double> submatrix)
    {
        double maxWeight = 0;
        Tuple<int, int>[] maxIndices = new Tuple<int, int>[0];
        int m = submatrix.RowCount;
        int n = submatrix.ColumnCount;
        for (int i = 0; i < m; i++)
        {
            double weight = ComputeWeight(submatrix.Row(i).ToArray());
            if (weight > maxWeight)
            {
                maxWeight = weight;
                maxIndices = Enumerable.Range(0, n).Select(j => Tuple.Create(i, j)).ToArray();
            }
            else if (weight == maxWeight)
            {
                maxIndices = maxIndices.Concat(Enumerable.Range(0, n).Select(j => Tuple.Create(i, j))).ToArray();
            }
        }
        for (int j = 0; j < n; j++)
        {
            double weight = ComputeWeight(submatrix.Column(j).ToArray());
            if (weight > maxWeight)
            {
                maxWeight = weight;
                maxIndices = Enumerable.Range(0, m).Select(i => Tuple.Create(i, j)).ToArray();
            }
            else if (weight == maxWeight)
            {
                maxIndices = maxIndices.Concat(Enumerable.Range(0, m).Select(i => Tuple.Create(i, j))).ToArray();
            }
        }
        return Tuple.Create(maxWeight, maxIndices);
    }

    // Функція для знаходження максимальної ваги та відповідних елементів
    static Tuple<double, Tuple<int, int>[]> FindMaxWeight(Tuple<double, Tuple<int, int>[]>[] results)
    {
        double maxWeight = 0;
        Tuple<int, int>[] maxIndices = new Tuple<int, int>[0];
        foreach (var result in results)
        {
            if (result.Item1 > maxWeight)
            {
                maxWeight = result.Item1;
                maxIndices = result.Item2;
            }
            else if (result.Item1 == maxWeight)
            {
                maxIndices = maxIndices.Concat(result.Item2).ToArray();
            }
        }
        return Tuple.Create(maxWeight, maxIndices);
    }

    // Головна функція програми
    static void Main(string[] args)
    {
        using (new MPI.Environment(ref args))
        {
            int rank = Communicator.world.Rank;
            int size = Communicator.world.Size;
            Matrix<double> A = Matrix<double>.Build.Random(1000, 1000); // Генерація матриці
            int chunkSize = int.Parse(A.ToMatrixString());
        }
    }
}

