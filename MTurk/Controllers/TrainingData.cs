using Microsoft.AspNetCore.Mvc;
using MTurk.Data;
using MTurk.Models;
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

        [HttpGet]
        public async Task<ActionResult> Get(int counter)
        {

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
                res.Append($"NOfMoves:{row.Moves.Count} ");
                int i;
                for (i = 0; i < row.Moves.Count; i++)
                {
                    var move = row.Moves[i];
                    res.Append($"{move.MoveBy[0]}:{move.ProposedAmount} ");
                }
                res.AppendLine();
                res.AppendLine("TConc    MConc  TFirst  MFirst TLast1 MLast1 TLast  MLast");
                Debug.Assert(row.Moves.Count > 0);
                i = row.Game.MachineStarts ? 0 : 1;
                for (; i < row.Moves.Count; i += 2)
                {
                    res.AppendFormat("{0,7}", row.TurksLastConcession(i));
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

        public TrainingData(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }
    }
}
