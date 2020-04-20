using System.Collections.Generic;
using System.Threading.Tasks;

namespace MTurk.Data
{
    public interface ISessionService
    {
        Task<List<SessionModel>> GetAllSessionsAsync();
        Task StartNewSession(SessionModel s);
    }
}