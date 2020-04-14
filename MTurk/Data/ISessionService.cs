using System.Collections.Generic;
using System.Threading.Tasks;

namespace MTurk.Data
{
    public interface ISessionService
    {
        Task<List<Session>> GetAllSessionsAsync();
        Task StartNewSession(Session s);
    }
}