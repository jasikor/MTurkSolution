using MTurk.Data;
using MTurk.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MTurk.DataAccess
{
    public interface IHistoricalGamesService
    {
        List<MovesWithGames> GetMovesWithGames(DateTime startTime = default, DateTime endTime = default);
        IList<GameInfo> GetGameInfos(DateTime startTime = default, DateTime endTime = default);

    }
}
