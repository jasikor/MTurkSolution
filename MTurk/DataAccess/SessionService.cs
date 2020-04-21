using MTurk.SQLDataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MTurk.Data
{
    public class SessionService : ISessionService
    {
        public SessionService(ISqlDataAccess db)
        {
            _db = db;
        }

        private readonly ISqlDataAccess _db;

        public async Task<SessionModel> StartNewSession(string workerId)
        {
            string sql = @"insert into dbo.Sessions (WorkerId, Time)
                           output inserted.*
                           values (@WorkerId, @Time)";

            DateTime utcNow = DateTime.UtcNow;
            SessionModel sm = new SessionModel() { WorkerId = workerId, Time = utcNow };
            return await _db.SaveData<SessionModel, SessionModel>(sql, sm);
        }

        public Task<List<SessionModel>> GetAllSessionsAsync()
        {
            string sql = @"select * from dbo.Sessions order by Id desc";
            return _db.LoadData<SessionModel, dynamic>(sql, new { });
        }

        public Task<GameInfo> GetCurrentGame(string workerId)
        {
            List<MoveModel> m = new List<MoveModel>();
            m.Add(new MoveModel() { MoveBy = "TURK", ProposedAmount = 10 });
            m.Add(new MoveModel() { MoveBy = "MACH", ProposedAmount = 9 });
            m.Add(new MoveModel() { MoveBy = "TURK", ProposedAmount = 10 });
            m.Add(new MoveModel() { MoveBy = "MACH", ProposedAmount = 8 });
            m.Add(new MoveModel() { MoveBy = "TURK", ProposedAmount = 9 , OfferAccepted = true});
            //m.Add(new MoveModel() { MoveBy = "MACH", ProposedAmount = 9, OfferAccepted = true });

            var ret = new GameInfo()
            {
                Moves = m
            };
            return Task.FromResult<GameInfo>(ret);
        }
    }
}
