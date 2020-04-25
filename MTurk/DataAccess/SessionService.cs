using MTurk.SQLDataAccess;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            TurksDisValue = 5,
            MachineDisValue = 5,
            TimeOut = 5444,
            Stubborn = 0.6,
            MachineStarts = false
        };
        public async Task<GameInfo> GetCurrentGame(string workerId)
        {
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

        private int GetLastMachineMove(string workerId)
        {
            int r;
            var res = ret.Moves.FindLast((x) => x.MoveBy == "MACH");
            if (res is null)
                r = 1;
            else
                r = res.ProposedAmount;
            return r;
        }
        public async Task<GameInfo> TurksMove(string workerId, MoveModel move)
        {
            await SaveMove(workerId, move);
            if (move.MoveBy == "TURK" && !move.OfferAccepted)
            {
                int machinesOffer = MachinesOffer(ret.Surplus, ret.Stubborn, ret.MachineDisValue,
                    move.ProposedAmount, GetLastMachineMove(workerId));

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

        private static Random rnd = new Random();
        private int RandomInteger(int minInclusive, int maxInclusive)
        {
            if (minInclusive > maxInclusive)
                return minInclusive;
            Debug.Assert(minInclusive <= maxInclusive + 1);
            return rnd.Next(minInclusive, maxInclusive + 1); // rnd.Next(min, max), min inclusive, max exclusive
        }

        private int MachinesOffer(int surplus, double stubborn, int machineDisValue, int workerLastDemand, int machineLastOffer)
        {
            int aIOffer = Math.Min(machineLastOffer, surplus - machineDisValue);
            if (workerLastDemand <= aIOffer)
                return workerLastDemand;
            if (rnd.NextDouble() < stubborn)
                aIOffer = RandomInteger(aIOffer - 1, aIOffer + 1);
            else
                aIOffer = RandomInteger(aIOffer, workerLastDemand);

            var res = Math.Clamp(aIOffer, 0, surplus - machineDisValue);
            Debug.WriteLine($"   MachinesOffer() = {res}, before clamping = {aIOffer}");
            return res;

        }
    }
}
