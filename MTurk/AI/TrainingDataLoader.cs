﻿using JetBrains.Annotations;
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
        List<(float[], float[])> data = new List<(float[], float[])>();

        const int testingPercentage = 80;

        public TrainingDataLoader(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }
        public Task<ITestDataset> GetTestDatasetAsync([CanBeNull] Action<TrainingProgressEventArgs> progress = null, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }

        public async Task<ITrainingDataset> GetTrainingDatasetAsync(int size)
        {
            await LoadData();

            float[,] X = new float[data.Count, 11];
            float[,] Y = new float[data.Count, 21];
            for (int i = 0; i < data.Count; i++)
            {
                for (int j = 0; j < 11; j++)
                    X[i, j] = data[i].Item1[j];
                for (int j = 0; j < 21; j++)
                    Y[i, j] = data[i].Item2[j];
            }
            (float[,] X, float[,] Y) d = (X, Y);
            int batchSize = 512;
            return d.X == null || d.Y == null
                ? null
                : DatasetLoader.Training(d, batchSize);
        }

        private async Task LoadData()
        {
            if (data.Count == 0)
            {
                var rows = await _sessionService.GetGameInfosAsync(100000);
                if (rows.Count == 0)
                    return;
                foreach (var row in rows)
                {
                    row.TrimMoves();
                    if (!row.IsValid())
                        continue;


                    int i;
                    Debug.Assert(row.Moves.Count > 0);
                    float[] y = new float[21];
                    int machProfit = row.AreLastTwoMovesEqual() ?
                                        row.Game.Surplus - (int)row.Game.TurksProfit :
                                        row.Game.MachineDisValue;
                    Debug.Assert(machProfit >= 0 && machProfit <= 20, $"machProfit is incorrect in row:{row.Game.Id}");
                    y[machProfit] = 1f;
                    i = row.Game.MachineStarts ? 0 : 1;
                    float[] x = new float[11];
                    for (; i < row.Moves.Count; i += 2)
                    {
                        x = new float[11] {
                                row.Game.MachineDisValue / 20f,
                                row.Game.MachineStarts ? 1 : 0,
                                i / row.Moves.Count,
                                (row.TurksLastConcession(i)+1) / row.Moves.Count,
                                (row.MachinesLastConcession(i)+1) / row.Moves.Count,
                                (row.TurksFirst()+1)/21,
                                (row.MachinesFirst()+1)/ 21,
                                (row.TurksLast1(i) + 1)/ 21,
                                (row.MachinesLast1(i) + 1)/ 21,
                                (row.TurksLast(i) + 1)/ 21,
                                (row.MachinesLast(i) + 1)/ 21,
                         };
                        data.Add((x, y));
                    }


                }
            }

        }
    }
}
