﻿using MTurk.DataAccess;
using MTurk.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MTurk.Data
{
    public interface ISessionService
    {
        List<SessionInfo> GetAHandfullOfLastSessions();
        Task<SessionModel> StartNewSession(string workerId);
        Task<GameInfo> GetCurrentGame(string workerId, string algoVersion);
        Task<GameInfo> StartNewGame(string workerId, string algoVersion);
        Task SaveMove(MoveModel move);
        Task FinishGame(GameModel game);
    }
}