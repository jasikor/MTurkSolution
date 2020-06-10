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
            ITrainingDataset trainingDataset = null; ; 
            await Task.Run(()=> trainingDataset = _trainingDataLoader.GetTrainingDataset(counter));
            TrainingSessionResult res = _aIManager.Train(trainingDataset, null);

            var content = GetContent(counter, FileFormat.TrainingVectorsNormalized);
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

        enum FileFormat
        {
            GamesAndMoves,
            TrainingVectors,
            TrainingVectorsNormalized
        }
        private string GetContent(int numberOfGames, FileFormat format)
        {
            var rows = _sessionService.GetGameInfos(numberOfGames);
            if (rows.Count == 0)
                return "Nothing to see here, there were no finished games";
            switch (format)
            {
                case FileFormat.TrainingVectorsNormalized:
                    return TrainingNormalized(rows);
                case FileFormat.TrainingVectors:
                    return TrainingInt(rows);
                default:
                    return GamesMoves(rows);
            }
        }
        private static string TrainingInt(IList<GameInfo> rows)
        {
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
                    var moves = row.MovesToFloat();
                    res.AppendFormat("{0,4}", i);
                    res.AppendFormat("{0,6}", GameInfo.TurksLastConcession(i, moves));
                    res.AppendFormat("{0,7}", GameInfo.MachinesLastConcession(i, moves));
                    res.AppendFormat("{0,7}", GameInfo.TurksFirst(moves, row.Game.MachineStarts));
                    res.AppendFormat("{0,7}", GameInfo.MachinesFirst(moves, row.Game.MachineStarts));
                    res.AppendFormat("{0,7}", GameInfo.TurksLast1(i, moves));
                    res.AppendFormat("{0,7}", GameInfo.MachinesLast1(i, moves));
                    res.AppendFormat("{0,7}", GameInfo.TurksLast(i, moves, row.Game.MachineStarts));
                    res.AppendFormat("{0,7}", GameInfo.MachinesLast(i, moves, row.Game.MachineStarts));

                    res.AppendLine();

                }
            }

            return res.ToString();

        }
        private static string TrainingNormalized(IList<GameInfo> rows)
        {
            StringBuilder res = new StringBuilder();
            foreach (var row in rows)
            {
                row.TrimMoves();
                if (!row.IsValid())
                    continue;
                int machProfit = row.AreLastTwoMovesEqual() ?
                                    row.Game.Surplus - (int)row.Game.TurksProfit :
                                    row.Game.MachineDisValue;
                int i;
                for (i = 0; i < row.Moves.Count; i++)
                {
                    var move = row.Moves[i];
                }
                Debug.Assert(row.Moves.Count > 0);
                i = row.Game.MachineStarts ? 0 : 1;
                for (; i < row.Moves.Count; i += 2)
                {
                    res.Append($"{row.Game.MachineDisValue} ");
                    res.Append($"{(row.Game.MachineStarts ? 1 : 0)} ");

                    var moves = row.MovesToFloat();
                    res.AppendFormat("{0} ", i);
                    res.AppendFormat("{0} ", GameInfo.TurksLastConcession(i, moves));
                    res.AppendFormat("{0} ", GameInfo.MachinesLastConcession(i, moves));
                    res.AppendFormat("{0} ", GameInfo.TurksFirst(moves, row.Game.MachineStarts));
                    res.AppendFormat("{0} ", GameInfo.MachinesFirst(moves, row.Game.MachineStarts));
                    res.AppendFormat("{0} ", GameInfo.TurksLast1(i, moves));
                    res.AppendFormat("{0} ", GameInfo.MachinesLast1(i, moves));
                    res.AppendFormat("{0} ", GameInfo.TurksLast(i, moves, row.Game.MachineStarts));
                    res.AppendFormat("{0} ", GameInfo.MachinesLast(i, moves, row.Game.MachineStarts));
                    res.AppendFormat("{0} ", machProfit);
                    res.AppendLine();

                }
            }

            return res.ToString();
        }
        private static string GamesMoves(IList<GameInfo> rows)
        {
            StringBuilder res = new StringBuilder();
            foreach (var row in rows)
            {
                row.TrimMoves();
                if (!row.IsValid())
                    continue;
                res.Append($"{row.Game.Id} AAAAAAAAAAAA {row.Game.StartTime} {row.Game.EndTime} {row.Game.Surplus} {row.Game.TurksDisValue} {row.Game.MachineDisValue} {row.Game.TimeOut} {row.Game.Stubborn} {(row.Game.MachineStarts ? 1 : 0)} ");
                int machProfit = row.AreLastTwoMovesEqual() ?
                                    row.Game.Surplus - (int)row.Game.TurksProfit :
                                    row.Game.MachineDisValue;
                int i;
                for (i = 0; i < row.Moves.Count; i++)
                {
                        res.Append($"{{{row.Moves[i].MoveBy[0]},{row.Moves[i].ProposedAmount}}} ");

                }
                res.AppendLine();
            }

            return res.ToString();
        }


    }
}