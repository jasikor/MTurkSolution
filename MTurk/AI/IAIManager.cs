using NeuralNetworkNET.APIs.Interfaces;
using NeuralNetworkNET.APIs.Interfaces.Data;
using NeuralNetworkNET.APIs.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MTurk.AI
{
    public interface IAIManager
    {
        INeuralNetwork GetNetwork();
        TrainingSessionResult Train(ITrainingDataset data, ITestDataset testData);
    }
}
