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

        public Task StartNewSession(Session s)
        {
            string sql = @"insert into dbo.Sessions (WorkerId, Time)
                            values (@WorkerId, @Time)";

            return _db.SaveData(sql, s);
        }

        public Task<List<Session>> GetAllSessionsAsync()
        {
            string sql = @"select * from dbo.Sessions order by Id desc";
            return _db.LoadData<Session, dynamic>(sql, new { });

        }
    }
}
