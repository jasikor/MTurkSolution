using JetBrains.Annotations;
using NeuralNetworkNET.APIs.Interfaces.Data;
using NeuralNetworkNET.SupervisedLearning.Progress;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MTurk.AI
{
    public interface ITrainingDataLoader
    {
        ITrainingDataset GetTrainingDataset(int size);
        ITestDataset GetTestDataset([CanBeNull] Action<TrainingProgressEventArgs> progress = null, CancellationToken token = default);


    }
}
