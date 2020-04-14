using System.Threading.Tasks;

namespace MTurk.Data
{
    public interface ISessionService
    {
        Task<Session[]> GetAllSessionsAsync();
        Task StartNewSession(string workerId);
    }
}