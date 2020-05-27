﻿using Microsoft.AspNetCore.Mvc;
using MTurk.AI;
using MTurk.Data;
using MTurk.Models;
using NeuralNetworkNET.APIs.Interfaces.Data;
using NeuralNetworkNET.APIs.Results;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTurk.Controllers
{
    [ApiController, Route("training/{counter}")]
    public class TrainingData : ControllerBase
    {
        private readonly ISessionService _sessionService;
        private readonly ITrainingDataLoader _trainingDataLoader;
        private readonly IAIManager _aIManager;

        public TrainingData(ISessionService sessionService, ITrainingDataLoader trainingDataLoader, IAIManager aIManager)
        {
            _sessionService = sessionService;
            _trainingDataLoader = trainingDataLoader;
            _aIManager = aIManager;
        }

        [HttpGet]
        public async Task<ActionResult> Get(int counter)
        {
            ITrainingDataset trainingDataset = await _trainingDataLoader.GetTrainingDatasetAsync(counter);
            TrainingSessionResult res = await _aIManager.TrainAsync(trainingDataset, null);

            var content = await GetContent(counter);
            var stream = GenerateStreamFromString(content);

            var result = new FileStreamResult(stream, "text/plain");
            result.FileDownloadName = $"TrainingData {DateTime.Now}.txt";
            return result;
        }

        private static Stream GenerateStreamFromString(string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        private async Task<string> GetContent(int numberOfGames)
        {
            var rows = await _sessionService.GetGameInfosAsync(numberOfGames);
            if (rows.Count == 0)
                return "Nothing to see here, there were no finished games";
            StringBuilder res = new StringBuilder();
            foreach (var row in rows)
            {
                row.TrimMoves();
                if (!row.IsValid())
                    continue;
                res.Append($"GameId:{row.Game.Id} ");
                int machProfit = row.AreLastTwoMovesEqual() ?
                                    row.Game.Surplus - (int)row.Game.TurksProfit :
                                    row.Game.MachineDisValue;
                res.Append($"MProfit:{machProfit} ");

                res.Append($"MDis:{row.Game.MachineDisValue} ");
                res.Append($"MStarts:{(row.Game.MachineStarts ? 1 : 0)} ");
                int i;
                for (i = 0; i < row.Moves.Count; i++)
                {
                    var move = row.Moves[i];
                    res.Append($"{move.MoveBy[0]}:{move.ProposedAmount} ");
                }
                res.AppendLine();
                res.AppendLine("Moves TConc  MConc  TFirst  MFirst TLast1 MLast1 TLast  MLast");
                Debug.Assert(row.Moves.Count > 0);
                i = row.Game.MachineStarts ? 0 : 1;
                for (; i < row.Moves.Count; i += 2)
                {
                    res.AppendFormat("{0,4}", i);
                    res.AppendFormat("{0,6}", row.TurksLastConcession(i));
                    res.AppendFormat("{0,7}", row.MachinesLastConcession(i));
                    res.AppendFormat("{0,7}", row.TurksFirst());
                    res.AppendFormat("{0,7}", row.MachinesFirst());
                    res.AppendFormat("{0,7}", row.TurksLast1(i));
                    res.AppendFormat("{0,7}", row.MachinesLast1(i));
                    res.AppendFormat("{0,7}", row.TurksLast(i));
                    res.AppendFormat("{0,7}", row.MachinesLast(i));
                    res.AppendLine();

                }
            }
            return res.ToString();
        }

        
    }
}
