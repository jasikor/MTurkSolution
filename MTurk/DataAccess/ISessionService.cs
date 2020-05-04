using System.Collections.Generic;
using System.Threading.Tasks;

namespace MTurk.Data
{
    public interface ISessionService
    {
        Task<List<SessionModel>> GetAllSessionsAsync();
        Task<SessionModel> StartNewSession(string workerId);
        Task<GameInfo> GetCurrentGame(string workerId);
        Task<GameInfo> StartNewGame(string workerId);
        Task SaveMove(MoveModel move);
        Task FinishGame(GameModel game);


    }
}