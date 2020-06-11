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
        private const int deviation = 3;

        public int GetMachinesOffer(GameInfo g)
        {
            float[] moves = g.MovesToFloat();
            var moves1 = GetMoves1(moves);

            float[] expectedPayoffs = new float[IMoveEngine.Payoffs];
            int lastMove = rnd.Next(1, 10+1);
            if (moves.Length > 1)
                lastMove = (int)moves[moves.Length - 2];
            int max = Math.Clamp(20 - g.Game.MachineDisValue - 1, 0, 21);
            int first = Math.Clamp(lastMove - deviation, 0, max);
            int last = Math.Clamp(lastMove + deviation, 0, max);
            Debug.Assert(last - first <= 2 * deviation + 1);
            for (int i = first; i <= last; i++)
            {
                moves1[moves1.Length - 1] = i;
                float[] X = GameInfo.GetSubHistory(moves1.Length - 1, g.Game.MachineDisValue, g.Game.MachineStarts, moves1);
                X[10] = i;
                _dataLoader.Normalize(X);
                float y = Nearest(X);
                expectedPayoffs[i] = y;

            }
            float sum = 0.0f;
            double lambda = g.Game.Stubborn;
            
            for (int i = first; i <= last; i++)
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

        const int K = 5;
        private float Nearest(float[] p)
        {
            Parallel.For(0, Y.Length - 1,
                (i) => { CalcDistance(i, p, distanceIndex); }
                );
            Array.Sort<DistIndex>(distanceIndex,
                (x, y) => x.Distance.CompareTo(y.Distance));

            int[] votes = new int[IMoveEngine.Payoffs];
            for (int i = 0; i < K; i++)
                votes[
                (int)(Y[distanceIndex[i].Index])
                ]++;
            //return Max(votes);
            return Average(votes);
        }
        private static float Average(int[] votes)
        {
            float res = 0.0f;
            for (int i = 0; i < votes.Length; i++)
                res += votes[i] * i ;
            return res / K;
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
    }
}

