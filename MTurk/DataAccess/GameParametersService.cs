using MTurk.Models;
using MTurk.SQLDataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MTurk.DataAccess
{
    public class GameParametersService : IGameParametersService
    {
        private readonly ISqlDataAccess _db;

        public GameParametersService(ISqlDataAccess db)
        {
            _db = db;
        }
        public Task<List<GameParametersModel>> GetAllParametersAsync()
        {
            string sql = @"select * from dbo.GameParameters order by Id desc";
            return _db.LoadData<GameParametersModel, dynamic>(sql, new { });
        }

    }
}
