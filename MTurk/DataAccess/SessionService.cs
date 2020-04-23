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

        private GameInfo ret = new GameInfo()
        {
            Id = 0,
            Moves = new List<MoveModel>(),
            Surplus = 20,
            TurksDisValue = 15,
            MachineDisValue = 10,
            TimeOut = 5444,
            Stubborn = 0.6,
            MachineStarts = false
        };
        public async Task<GameInfo> GetCurrentGame(string workerId)
        {
            List<MoveModel> m = new List<MoveModel>();
            m.Add(new MoveModel() { MoveBy = "TURK", ProposedAmount = 10 });
            m.Add(new MoveModel() { MoveBy = "MACH", ProposedAmount = 9 });
            m.Add(new MoveModel() { MoveBy = "TURK", ProposedAmount = 10 });
            m.Add(new MoveModel() { MoveBy = "MACH", ProposedAmount = 8 });
            //m.Add(new MoveModel() { MoveBy = "SYST" });
            //m.Add(new MoveModel() { MoveBy = "TURK", ProposedAmount = 9 , OfferAccepted = true});
            //m.Add(new MoveModel() { MoveBy = "MACH", ProposedAmount = 9, OfferAccepted = true });


            return ret;
        }

        private async Task SaveMove(string workerId, MoveModel move)
        {
            move.Time = DateTime.UtcNow;
            string sql = @"insert into dbo.Moves 
                            (Time, GameId, MoveBy, ProposedAmount, OfferAccepted)
                           values 
                            (@Time, @GameId, @MoveBy, @ProposedAmount, @OfferAccepted)";

            if (move.Time == DateTime.MinValue)
                await _db.SaveData<MoveModel>(sql, move);
            else
                ret.Moves.Add(move);

        }

        public async Task<GameInfo> TurksMove(string workerId, MoveModel move)
        {
            await SaveMove(workerId, move);
            if (move.MoveBy == "TURK" && !move.OfferAccepted)
            {
                int machinesOffer = MachinesOffer(ret, move);
                MoveModel machinesMove = new MoveModel()
                {
                    MoveBy = "MACH",
                    ProposedAmount = machinesOffer,
                    OfferAccepted = move.MoveBy == "TURK" && move.ProposedAmount == machinesOffer,
                    GameId = move.GameId,
                };

                await SaveMove(workerId, machinesMove);
            }
            return await GetCurrentGame(workerId);
        }

        private int MachinesOffer(GameInfo ret, MoveModel move)
        {
            return 3;
        }
    }
}
