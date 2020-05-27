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
        Task<ITrainingDataset> GetTrainingDatasetAsync(int size);
        Task<ITestDataset> GetTestDatasetAsync([CanBeNull] Action<TrainingProgressEventArgs> progress = null, CancellationToken token = default);


    }
}
