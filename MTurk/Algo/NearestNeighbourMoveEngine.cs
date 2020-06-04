using MTurk.AI;
using MTurk.Data;
using NeuralNetworkNET.APIs.Interfaces.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        private void CalcDistance(int i, float[] p, DistIndex[] distanceIndex)
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
            var moves1 = GetMoves1(g.MovesToFloat());

            float[] expectedPayoffs = new float[21];
            for (int i = 0; i < expectedPayoffs.Length; i++)
            {
                moves1[moves1.Length - 1] = i;
                float[] X = GameInfo.GetSubHistory(moves1.Length - 1, g.Game.MachineDisValue, g.Game.MachineStarts, moves1);
                Debug.WriteLine(Unnormalized(X, moves1.Length));
                float y = Nearest(X);
                expectedPayoffs[i] = y;

            }
            float sum = 0.0f;
            double lambda = 1.5;
            //double lambda = g.Game.Stubborn;
            for (int i = 0; i < expectedPayoffs.Length; i++)
            {
                expectedPayoffs[i] = (float)Math.Exp(lambda * expectedPayoffs[i]);
                sum += expectedPayoffs[i];
            }
            for (int i = 0; i < expectedPayoffs.Length; i++)
                expectedPayoffs[i] /= sum;

            var cumDist = new double[expectedPayoffs.Length];
            double prob = 0.0;
            for (int i = 0; i < cumDist.Length; i++)
            {
                prob += expectedPayoffs[i];
                cumDist[i] = prob;
                Debug.WriteLine($"cumDist[{i}] = {cumDist[i]}");
            }

            Debug.Assert(Math.Abs(cumDist[cumDist.Length - 1] - 1.0) < 0.00001);

            cumDist[cumDist.Length - 1] = 1.0;
            int aIOffer = CumulativeRandom(cumDist);
            return aIOffer;
        }

        private float Nearest(float[] p)
        {
            Parallel.For(0, Y.Length - 1,
                (i) => { CalcDistance(i, p, distanceIndex); }
                );
            Array.Sort<DistIndex>(distanceIndex,
                (x, y) => x.Distance.CompareTo(y.Distance));

            int[] votes = new int[21];
            int K = 5;
            for (int i = 0; i < K; i++)
                votes[
                (int)(Y[distanceIndex[i].Index] * 21f)
                ]++;
            return Max(votes) / 21f;
        }
        private static int Max(int[] v)
        {
            var max = v.Max();
            return v.ToList().IndexOf(max);
        }

        private static Random rnd = new Random();

        private static int CumulativeRandom(double[] cumDist)
        {
            var random = rnd.NextDouble();
            int i;
            for (i = 0; i < cumDist.Length; i++)
                if (random <= cumDist[i])
                    break;
            return i;
        }
        private static float[] GetMoves1(float[] moves)
        {
            var res = new float[moves.Length + 1];
            for (int i = 0; i < moves.Length; i++)
                res[i] = moves[i];
            return res;
        }
#if DEBUG
        private static float UnNormalizeMove(float x) => 21f * x - 1;
        private static float UnNormalizeDisValue(float x) => 20f * x;
        private static float UnNormalizeMoveNumber(float x, int l) => x * l;
        private static float UnNormalizeTime(float x, int i) => x * i - 1;
        private static string Unnormalized(float[] x, int i)
        {
            return
                $"MDis: {UnNormalizeDisValue(x[0])} MStarts:{x[1]} MNumber:{UnNormalizeMoveNumber(x[2], i)} TLCons:{UnNormalizeTime(x[3], i)} MLCons:{UnNormalizeTime(x[4], i)} T1stMove:{UnNormalizeMove(x[5])} M1stMove:{UnNormalizeMove(x[6])} T-1:{UnNormalizeMove(x[7])} M-1:{UnNormalizeMove(x[8])} TLast:{UnNormalizeMove(x[9])} MLast:{UnNormalizeMove(x[10])}";
        }

#endif
    }
}

