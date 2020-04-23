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

        public async Task DeleteGameParameters(int id)
        {
            string sql =
                @"DELETE GameParameters WHERE Id = @Id";

            await _db.SaveData<dynamic>(sql, new { Id = id });
        }

        public Task<List<GameParametersModel>> GetAllParametersAsync()
        {
            string sql = @"select * from dbo.GameParameters order by Id desc";
            return _db.LoadData<GameParametersModel, dynamic>(sql, new { });
        }

        /*
         * 
        public int Id;
        public int Surplus;
        public int TurksDisValue;
        public int MachineDisValue;
        public int TimeOut;
        public double Stubborn;
        public bool MachineStarts;
         */
        public async Task SaveGameParameters(GameParametersModel gp)
        {
            string sql;
            if (gp.Id != 0)
                sql = @"UPDATE [dbo].[GameParameters] 
                        SET 
                            Surplus = @Surplus,
                            TurksDisValue = @TurksDisValue,
                            MachineDisValue = @MachineDisValue,
                            TimeOut = @TimeOut,
                            Stubborn = @Stubborn,
                            MachineStarts = @MachineStarts
                        WHERE Id = @Id";
            else
                sql = @"INSERT INTO [dbo].[GameParameters](
                            Surplus,
                            TurksDisValue,
                            MachineDisValue,
                            TimeOut,
                            Stubborn,
                            MachineStarts
                            ) 
                        VALUES (
                            @Surplus,
                            @TurksDisValue,
                            @MachineDisValue,
                            @TimeOut,
                            @Stubborn,
                            @MachineStarts
                        )";

            await _db.SaveData<GameParametersModel>(sql, gp);
        }

    }
}
