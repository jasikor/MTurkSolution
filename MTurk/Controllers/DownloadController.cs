using Microsoft.AspNetCore.Mvc;
using MTurk.Data;
using MTurk.DataAccess;
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

    [ApiController, Route("dwn/")]
    public class DownloadController : ControllerBase
    {
        private readonly IHistoricalGamesService _gs;

        [HttpGet]
        public ActionResult Get()
        {

            var content =  GetContent();
            var stream = GenerateStreamFromString(content);

            var result = new FileStreamResult(stream, "text/plain");
            result.FileDownloadName = $"TurkSessions {DateTime.Now}.txt";
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

        private string GetContent()
        {
            List<MovesWithGames> rows = _gs.GetMovesWithGames();
            if (rows.Count == 0)
                return "Nothing to see here, there were no finished games";
            StringBuilder res = new StringBuilder();
            int currentGame = 0;
            while (currentGame < rows.Count - 1)
            {
                res.Append(rows[currentGame].ToString());
                for (int i = currentGame; ; i++)
                {
                    if (i >= rows.Count)
                        return res.ToString();
                    if (rows[currentGame].Id == rows[i].Id)
                        res.Append($"{{{rows[i].MoveBy[0]},{rows[i].ProposedAmount}}} ");
                    else
                    {
                        res.AppendLine();
                        currentGame = i;
                        break;
                    }
                }
            }
            return res.ToString();
        }

        public DownloadController(IHistoricalGamesService gs)
        {
            _gs = gs;
        }

    }
}
