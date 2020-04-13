using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MTurk.Data
{
    public class SessionService : ISessionService
    {
        private Session[] sessions = new Session[] { new Session(), new Session() };
        public Task<Session[]> GetAllSessionsAsync()
        {
            return Task.FromResult(sessions);
        }
    }
}
