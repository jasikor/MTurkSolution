using MTurk.AI;
using MTurk.Data;
using NeuralNetworkNET.APIs.Interfaces.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace MTurk.Algo
{
    public class NearestNeighbourMoveEngine : IMoveEngine
    {
        private readonly ITrainingDataLoader _dataLoader;
        private float[,] X;
        private float[] Y;

        public NearestNeighbourMoveEngine(ITrainingDataLoader dataLoader)
        {
            _dataLoader = dataLoader;
            LoadData();


        }

        private void LoadData()
        {
            (X, Y) = _dataLoader.GetRawData();
            distanceIndex = new DistIndex[Y.Length];
        }

        private void CalcDistance(int i, float[] p)
        {
            double sum = 0.0;
            for (int j = 0; j < 11; j++)
                sum += (X[i, j] - p[j]) * (X[i, j] - p[j]);
            distanceIndex[i].Distance = Math.Sqrt(sum);
            distanceIndex[i].Index = i;
        }
        private struct DistIndex
        {
            public double Distance;
            public int Index;
            public override string ToString()
            {
                return $"{Index} -> {Distance}";
            }
        }

        private DistIndex[] distanceIndex;
        public int GetMachinesOffer(GameInfo g)
        {
            double s = 0.0;
            float[] p = new float[11];
            Parallel.For(0, Y.Length - 1,
                (i) => { CalcDistance(i, p); }
                );
            Array.Sort<DistIndex>(distanceIndex,
                (x, y) => x.Distance.CompareTo(y.Distance));

            int[] votes = new int[21];
            int K = 13;
            for (int i = 0; i < K; i++)
                votes[
                (int)(Y[distanceIndex[i].Index] * 21f)
                ]++;

            return 3;
        }
    }
}

