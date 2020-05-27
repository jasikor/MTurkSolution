using NeuralNetworkNET.APIs.Interfaces;
using NeuralNetworkNET.APIs.Interfaces.Data;
using NeuralNetworkNET.APIs.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MTurk.AI
{
    interface IAIManager
    {
        INeuralNetwork GetNetwork();
        Task<TrainingSessionResult> TrainAsync(ITrainingDataset data, ITestDataset testData);
    }
}
