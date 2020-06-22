using Microsoft.AspNetCore.Mvc;
using MTurk.AI;
using MTurk.Algo;
using MTurk.Data;
using MTurk.DataAccess;
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
    [ApiController, Route("training")]
    public class TrainingData : ControllerBase
    {
        private readonly IHistoricalGamesService _gs;
        private readonly IAIManager _aIManager;

        public TrainingData(ISessionService sessionService, IAIManager aIManager, IHistoricalGamesService gs)
        {
            _gs = gs;
            _aIManager = aIManager;
        }

        [HttpGet]
        public ActionResult Get()
        {
            var content = GetContent(FileFormat.TrainingVectorsNormalized);
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
        private string GetContent(FileFormat format)
        {
            var rows = _gs.GetGameInfos();
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
                res.AppendLine("Moves TConc  MConc  TFirst  MFirst TLast1 MLast1 TLast  MLast TDisV");
                Debug.Assert(row.Moves.Count > 0);
                i = row.Game.MachineStarts ? 0 : 1;
                for (; i < row.Moves.Count; i += 2)
                {
                    var moves = row.MovesToFloat();
                    var s = SubHistory.GetSubHistory(i, row.Game.MachineDisValue, row.Game.ShowMachinesDisValue ? row.Game.TurksDisValue : -1, row.Game.MachineStarts, moves);
                    res.AppendFormat("{0,4}", i);
                    res.AppendFormat("{0,6}", s[SubHistory.TurksLastConcessionIndex]);
                    res.AppendFormat("{0,7}", s[SubHistory.MachinesLastConcessionIndex]);
                    res.AppendFormat("{0,7}", s[SubHistory.TurksFirstOfferIndex]);
                    res.AppendFormat("{0,7}", s[SubHistory.MachinesFirstOfferIndex]);
                    res.AppendFormat("{0,7}", s[SubHistory.TurksLastOfferIndex]);
                    res.AppendFormat("{0,7}", s[SubHistory.MachinesLast1OfferIndex]);
                    res.AppendFormat("{0,7}", s[SubHistory.TurksLastOfferIndex]);
                    res.AppendFormat("{0,7}", s[SubHistory.MachinesLastOfferIndex]);
                    res.AppendFormat("{0,7}", s[SubHistory.TurksDisValueIndex]);

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
                    var s = SubHistory.GetSubHistory(i, row.Game.MachineDisValue, row.Game.ShowMachinesDisValue ? row.Game.TurksDisValue : -1, row.Game.MachineStarts, moves);
                    res.AppendFormat("{0} ", i);
                    res.AppendFormat("{0} ", s[SubHistory.TurksLastConcessionIndex]);
                    res.AppendFormat("{0} ", s[SubHistory.MachinesLastConcessionIndex]);
                    res.AppendFormat("{0} ", s[SubHistory.TurksFirstOfferIndex]);
                    res.AppendFormat("{0} ", s[SubHistory.MachinesFirstOfferIndex]);
                    res.AppendFormat("{0} ", s[SubHistory.TurksLast1OfferIndex]);
                    res.AppendFormat("{0} ", s[SubHistory.MachinesLast1OfferIndex]);
                    res.AppendFormat("{0} ", s[SubHistory.TurksLastOfferIndex]);
                    res.AppendFormat("{0} ", s[SubHistory.MachinesLastOfferIndex]);
                    res.AppendFormat("{0} ", s[SubHistory.TurksDisValueIndex]);
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
