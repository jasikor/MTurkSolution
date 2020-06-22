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
        float[] Mean { get; }
        float[] StdVar { get; }
        int? LearningRangeStart { get; }
        int? LearningRangeEnd { get; }

        ITrainingDataset GetTrainingDataset(int size);
        (float[,], float[]) GetRawData();
        void Normalize(float[] x);
        void Normalize(float[,] x);
    }
}
