using JetBrains.Annotations;
using MTurk.Algo;
using MTurk.Data;
using MTurk.DataAccess;
using NeuralNetworkNET.APIs;
using NeuralNetworkNET.APIs.Interfaces.Data;
using NeuralNetworkNET.SupervisedLearning.Progress;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MTurk.AI
{
    public class TrainingDataLoader : ITrainingDataLoader
    {
        private readonly List<float[]> inputData = new List<float[]>();
        private readonly List<float> resultData = new List<float>();
        private IHistoricalGamesService _gs;

        public TrainingDataLoader(IHistoricalGamesService gs)
        {
            _gs = gs;
        }

        public ITrainingDataset GetTrainingDataset(int size)
        {
            LoadData(inputData, resultData);
            float[,] X = new float[inputData.Count, SubHistory.SubHistoryLength];
            float[,] Y = new float[inputData.Count, IMoveEngine.Payoffs];
            for (int i = 0; i < inputData.Count; i++)
            {
                for (int j = 0; j < SubHistory.SubHistoryLength; j++)
                    X[i, j] = inputData[i][j];
                var data = new float[IMoveEngine.Payoffs];

                data[Math.Clamp((int)resultData[i], 0, 20)] = 1f;
                for (int j = 0; j < IMoveEngine.Payoffs; j++)
                    Y[i, j] = data[j];
            }
            Normalize(X);
            (float[,] X, float[,] Y) d = (X, Y);
            int batchSize = 512;
            return d.X == null || d.Y == null
                ? null
                : DatasetLoader.Training(d, batchSize);
        }
        public float[] Mean { get; private set; }
        public float[] StdVar { get; private set; }
        public int? LearningRangeStart { get; private set; }
        public int? LearningRangeEnd { get; private set; }

        public void Normalize(float[,] x)
        {
            float[] sum = new float[x.GetLength(1)];
            for (int i = 0; i < x.GetLength(0); i++)
            {
                for (int j = 0; j < x.GetLength(1); j++)
                    sum[j] += x[i, j];
            }
            Mean = new float[x.GetLength(1)];
            for (int j = 0; j < x.GetLength(1); j++)
                Mean[j] = sum[j] / x.GetLength(0);
            float[] sumDist = new float[x.GetLength(1)];
            for (int i = 0; i < x.GetLength(0); i++)
            {
                for (int j = 0; j < x.GetLength(1); j++)
                {
                    var dist = x[i, j] - Mean[j];
                    sumDist[j] += dist * dist;
                }
            }
            StdVar = new float[x.GetLength(1)];
            for (int j = 0; j < x.GetLength(1); j++)
                StdVar[j] = (float)Math.Sqrt(sumDist[j] / x.GetLength(0)) + 0.01f;
            for (int i = 0; i < x.GetLength(0); i++)
            {
                for (int j = 0; j < x.GetLength(1); j++)
                {
                    x[i, j] = (x[i, j] - Mean[j]) / StdVar[j];
                }
            }

        }
        public void Normalize(float[] x)
        {
            for (int i = 0; i < x.Length; i++)
                x[i] = (x[i] - Mean[i]) / StdVar[i];
        }

        private void LoadData(List<float[]> X, List<float> Y)
        {
            LearningRangeStart = null;
            LearningRangeEnd = null;
            if (X.Count == 0)
            {
                var rows = _gs.GetGameInfos();
                if (rows.Count == 0)
                    return;
                
                foreach (var row in rows)
                {
                    row.TrimMoves();
                    if (!row.IsValid())
                        continue;
                    if (LearningRangeEnd is null)
                        LearningRangeEnd = row.Game.Id;
                    LearningRangeStart = row.Game.Id;

                    int i;
                    Debug.Assert(row.Moves.Count > 0);
                    int machProfit = row.AreLastTwoMovesEqual() ?
                                        row.Game.Surplus - (int)row.Game.TurksProfit :
                                        row.Game.MachineDisValue;
                    Debug.Assert(machProfit >= 0 && machProfit <= 20, $"machProfit is incorrect in row:{row.Game.Id}");
                    i = row.Game.MachineStarts ? 0 : 1;
                    for (; i < row.Moves.Count; i += 2)
                    {
                        var x = row.GetSubHistory(i);
                        X.Add(x);
                        Y.Add(machProfit);
                    }
                }
            }

        }

        public (float[,], float[]) GetRawData()
        {
            LoadData(inputData, resultData);
            float[,] X = new float[inputData.Count, SubHistory.SubHistoryLength];
            float[] Y = new float[inputData.Count];
            for (int i = 0; i < inputData.Count; i++)
            {
                for (int j = 0; j < SubHistory.SubHistoryLength; j++)
                    X[i, j] = inputData[i][j];
                Y[i] = resultData[i];
            }
            Normalize(X);
            return (X, Y);
        }
    }
}
