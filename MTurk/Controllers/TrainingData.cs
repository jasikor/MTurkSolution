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
                if (!row.IsValid())
                    continue;
                res.Append($"MDis:{row.Game.MachineDisValue} ");
                res.Append($"MStarts:{(row.Game.MachineStarts ? 1 : 0)} ");
                res.Append($"NOfMoves:{row.Moves.Count} ");

                for (int i = 0; i < row.Moves.Count; i++)
                {
                    var move = row.Moves[i];
                    res.Append($"{move.MoveBy[0]}:{move.ProposedAmount} ");
                }
                res.AppendLine();
            }
            return res.ToString();
        }

        public TrainingData(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }
    }
}
