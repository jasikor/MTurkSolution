using MTurk.DataAccess;
using MTurk.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MTurk.Data
{
    public interface ISessionService
    {
        List<SessionInfo> GetAHandfullOfLastSessions();
        IList<GameInfo> GetGameInfos(DateTime startTime = default, DateTime endTime = default);
        Task<SessionModel> StartNewSession(string workerId);
        Task<GameInfo> GetCurrentGame(string workerId);
        Task<GameInfo> StartNewGame(string workerId);
        Task SaveMove(MoveModel move);
        Task FinishGame(GameModel game);
        List<MovesWithGames> GetMovesWithGames(DateTime startTime = default, DateTime endTime = default);
    }
}