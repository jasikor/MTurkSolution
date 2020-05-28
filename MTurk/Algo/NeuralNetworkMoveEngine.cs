using MTurk.AI;
using MTurk.Data;
using NeuralNetworkNET.APIs.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MTurk.Algo
{
    public class NeuralNetworkMoveEngine : IMoveEngine
    {
        private readonly IAIManager _manager;

        public NeuralNetworkMoveEngine(IAIManager manager)
        {
            _manager = manager;
        }
        private Random rnd = new Random();
        public int GetMachinesOffer(GameInfo g)
        {
            INeuralNetwork net = _manager.GetNetwork();
            if (net is null)
                return 0;
            float[] X; //  = g.GetSubHistory(g.Moves.Count-1);
            X = new float[11] {
                (float)rnd.NextDouble(),
                (float)rnd.NextDouble(),
                (float)rnd.NextDouble(),
                (float)rnd.NextDouble(),
                (float)rnd.NextDouble(),
                (float)rnd.NextDouble(),
                (float)rnd.NextDouble(),
                (float)rnd.NextDouble(),
                (float)rnd.NextDouble(),
                (float)rnd.NextDouble(),
                (float)rnd.NextDouble(),
                };
            float[] Y = net.Forward(X);
            var max = Y.Max();
            return Y.ToList().IndexOf(max);
        }
    }
}
