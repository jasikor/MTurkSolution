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

        public Task StartNewSession(SessionModel s)
        {
            string sql = @"insert into dbo.Sessions (WorkerId, Time)
                            values (@WorkerId, @Time)";

            return _db.SaveData(sql, new { WorkerId = s.WorkerId, Time = DateTime.UtcNow });
        }

        //public Task<GameModel> StartNewGameAsync(int sessionId)
        //{

        //    GameModel g = new GameModel()
        //    {
        //        SessionId = sessionId,
        //        StartTime = DateTime.UtcNow,
        //        TurksDisValue = 4,
        //        MachineDisValue = 8
        //    };

        //    return g;
        //}

        public Task<List<SessionModel>> GetAllSessionsAsync()
        {
            string sql = @"select * from dbo.Sessions order by Id desc";
            return _db.LoadData<SessionModel, dynamic>(sql, new { });

        }
    }
}
