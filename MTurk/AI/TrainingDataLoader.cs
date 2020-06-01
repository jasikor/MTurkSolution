using JetBrains.Annotations;
using MTurk.Data;
using NeuralNetworkNET.APIs;
using NeuralNetworkNET.APIs.Interfaces.Data;
using NeuralNetworkNET.SupervisedLearning.Progress;
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
        private readonly ISessionService _sessionService;
        List<(float[], float[])> dataa = new List<(float[], float[])>();
        List<float[]> inputData = new List<float[]>();
        List<float> resultData = new List<float>();

        const int testingPercentage = 80;

        public TrainingDataLoader(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }
        public ITestDataset GetTestDataset([CanBeNull] Action<TrainingProgressEventArgs> progress = null, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }

        public ITrainingDataset GetTrainingDataset(int size)
        {
            LoadData();
            float[,] X = new float[inputData.Count, 11];
            float[,] Y = new float[inputData.Count, 21];
            try
            {
                for (int i = 0; i < inputData.Count; i++)
                {
                    for (int j = 0; j < 11; j++)
                        X[i, j] = inputData[i][j];
                    var data = new float[21];

                    data[Math.Clamp((int)resultData[i], 0, 20)] = 1f;
                    for (int j = 0; j < 21; j++)
                        Y[i, j] = data[j];
                }
            }
            catch (Exception e)
            {

            }
            (float[,] X, float[,] Y) d = (X, Y);
            int batchSize = 512;
            return d.X == null || d.Y == null
                ? null
                : DatasetLoader.Training(d, batchSize);
        }

        private void LoadData()
        {
            if (inputData.Count == 0)
            {
                var rows = _sessionService.GetGameInfos(100000);
                if (rows.Count == 0)
                    return;
                foreach (var row in rows)
                {
                    row.TrimMoves();
                    if (!row.IsValid())
                        continue;


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
                        inputData.Add(x);
                        resultData.Add(machProfit);
                    }
                }
            }

        }
    }
}
