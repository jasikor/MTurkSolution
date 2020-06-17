using MTurk.AI;
using MTurk.Data;
using NeuralNetworkNET.APIs.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.XPath;

namespace MTurk.Algo
{
    public class NeuralNetworkMoveEngine : IMoveEngine
    {
        private readonly IAIManager _manager;

        public NeuralNetworkMoveEngine(IAIManager manager)
        {
            _manager = manager;
        }
        public int GetMachinesOffer(GameInfo g)
        {
            INeuralNetwork net = _manager.GetNetwork();
            if (net is null)
                return 0;
            var moves1 = GetMoves1(g.MovesToFloat());

            float[] expectedPayoffs = new float[IMoveEngine.Payoffs];
            for (int i = 0; i < expectedPayoffs.Length; i++)
            {
                moves1[moves1.Length - 1] = i;
                float[] X = SubHistory.GetSubHistory(moves1.Length - 1, g.Game.MachineDisValue, g.Game.ShowMachinesDisValue ? g.Game.TurksDisValue : -1, g.Game.MachineStarts, moves1);
                float[] Y = net.Forward(X);
                expectedPayoffs[i] = ExpectedPayoff(Y);

            }
            float sum = 0.0f;
            double lambda = 0.5;
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
        private static float ExpectedPayoff(float[] y)
        {
            float s = 0f;
            for (int i = 0; i < y.Length; i++)
            {
                s += y[i] * i;
            }
            return s;
        }

        private static int Max(float[] v)
        {
            var max = v.Max();
            return v.ToList().IndexOf(max);
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
