using NeuralNetworkNET.APIs;
using NeuralNetworkNET.APIs.Enums;
using NeuralNetworkNET.APIs.Interfaces;
using NeuralNetworkNET.APIs.Interfaces.Data;
using NeuralNetworkNET.APIs.Results;
using NeuralNetworkNET.APIs.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MTurk.AI
{
    public class AIManager : IAIManager
    {
        private readonly INetworkStorage _storage;
        private INeuralNetwork _network;
        public AIManager(INetworkStorage storage)
        {
            _storage = storage;
        }
        public INeuralNetwork GetNetwork()
        {
            if (_network is null)
                _network = _storage.Load();
            if (_network is null)
                return null;
            return _network;
        }

        public TrainingSessionResult Train(ITrainingDataset data, ITestDataset testData)
        {
            INeuralNetwork net = NetworkManager.NewSequential(TensorInfo.Linear(11),
                NetworkLayers.FullyConnected(11, ActivationType.LeCunTanh),
                NetworkLayers.Softmax(21));
            TrainingSessionResult result = NetworkManager.TrainNetwork(net,
                data,
                TrainingAlgorithms.AdaDelta(),
                100, 0.0f,
                null,
                testDataset: testData);
            if (result.StopReason == TrainingStopReason.EpochsCompleted)
            {
                _storage.Save(net);
                _network = net;
            }
            return result;
        }
    }
}
