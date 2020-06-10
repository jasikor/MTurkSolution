﻿using MTurk.DataAccess;
using MTurk.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MTurk.Data
{
    public interface ISessionService
    {
        List<SessionInfo> GetAHandfullOfLastSessions();
        List<MovesWithGames> GetMovesWithGames(int numberOfGames, int firstRow = 0);
        IList<GameInfo> GetGameInfos(int numberOfGames, int firstGame = 0);
        Task<SessionModel> StartNewSession(string workerId);
        Task<GameInfo> GetCurrentGame(string workerId);
        Task<GameInfo> StartNewGame(string workerId);
        Task SaveMove(MoveModel move);
        Task FinishGame(GameModel game);
    }
}