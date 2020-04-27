﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace MTurk.Data
{
    public interface ISessionService
    {
        Task<List<SessionModel>> GetAllSessionsAsync();
        Task<SessionModel> StartNewSession(string workerId);
        Task<GameInfo> GetCurrentGame(string workerId);
        Task<GameInfo> GetCurrentGame(int gameId);
        Task<GameInfo> TurksMove(string workerId, MoveModel move);

    }
}