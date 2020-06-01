using MTurk.AI;
using MTurk.Data;
using NeuralNetworkNET.APIs.Interfaces.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MTurk.Algo
{
    public class NearestNeighbourMoveEngine : IMoveEngine
    {
        private readonly ITrainingDataLoader _dataLoader;
        private (float[,],float[]) _data;

        public NearestNeighbourMoveEngine(ITrainingDataLoader dataLoader)
        {
            _dataLoader = dataLoader;
            LoadData();


        }

        private void LoadData()
        {
            _data = _dataLoader.GetRawData();
        }

        public int GetMachinesOffer(GameInfo g)
        {
            return 3;
        }
    }
}
