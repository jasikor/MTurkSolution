using MTurk.SQLDataAccess;
using MTurk;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;
using MTurk.Models;

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
            SessionModel sm = new SessionModel() { WorkerId = workerId.ToUpper(), Time = utcNow };
            return await _db.SaveData<SessionModel, SessionModel>(sql, sm);
        }

        public Task<List<SessionModel>> GetAllSessionsAsync()
        {
            string sql = @"select * from dbo.Sessions order by Id desc";
            return _db.LoadData<SessionModel, dynamic>(sql, new { });
        }

        private GameInfo ret = new GameInfo()
        {
            Id = 10,
            Moves = new List<MoveModel>(),
            Surplus = 20,
            TurksDisValue = 5,
            MachineDisValue = 5,
            TimeOut = 60,
            Stubborn = 0.6,
            MachineStarts = false
        };

        /// <summary>
        /// Gets unfinished or new game in session.
        /// </summary>
        /// <param name="workerId"></param>
        /// <returns>null if all games have been played, new game if all games so far are finished, or unfinished game</returns>
        public async Task<GameInfo> GetCurrentGame(string workerId)
        {
            return ret;
        }

        /// <summary>
        /// Starts new game using first not used GameParameter
        /// </summary>
        /// <param name="workerId"></param>
        /// <returns>new game or null if there are no more unused GameParameters</returns>
        public async Task<GameInfo> StartNewGame(string workerId)
        {
            string sql = @"select top 1  * from GameParameters
                            left join Games 
                            on GameParameters.Id = Games.GameParameterId
                            where Games.SessionId is null";


            sql = @"select Top 1 gp.* from GameParameters gp
                    left join (
                        select Games.Id, Games.GameParameterId, Games.SessionId 
                        from Games
                        left join Sessions
                        on Sessions.Id = Games.SessionId
                        where Sessions.WorkerId = @WorkerId) AS G
                    on gp.Id = G.GameParameterId
                    where G.SessionId is null;";


            GameParametersModel gameParameter = null;
            try
            {
                gameParameter = await _db.LoadData<dynamic, GameParametersModel>(sql, new { WorkerId = workerId });
            }
            catch (SqlException e)
            {
                Debug.WriteLine(e);
                throw;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                throw;
            }

            if (gameParameter is null)
                return null;

            var g = new
            {
                WorkerId = workerId,
                GameParameterId = gameParameter.Id,
                StartTime = DateTime.UtcNow,
                Surplus = gameParameter.Surplus,
                TurksDisValue = gameParameter.TurksDisValue,
                MachineDisValue = gameParameter.MachineDisValue,
                TimeOut = gameParameter.TimeOut,
                Stubborn = gameParameter.Stubborn,
                MachineStarts = gameParameter.MachineStarts,
                Finished = false,
            };
           sql = @"insert into dbo.Games 
                              (SessionId,
                               GameParameterId, StartTime, Surplus, TurksDisValue, MachineDisValue, TimeOut, Stubborn, MachineStarts, Finished)
                           output inserted.*
                           values 
                              ((select Id from Sessions where WorkerId = @WorkerId), 
                               @GameParameterId, @StartTime, @Surplus, @TurksDisValue, @MachineDisValue, @TimeOut, @Stubborn, @MachineStarts, @Finished)";
            try
            {
               var res = await _db.SaveData<dynamic, GameModel>(sql, g);
                return new GameInfo()
                {
                    Id = res.Id,
                    Moves = new List<MoveModel>(),
                    Surplus = res.Surplus,
                    TurksDisValue = res.MachineDisValue,
                    MachineDisValue = res.MachineDisValue,
                    TimeOut = res.TimeOut,
                    Stubborn = res.Stubborn,
                    MachineStarts = res.MachineStarts
                };
            }
            catch (SqlException) 
            {
               return null;
            }
        }

        private async Task SaveMove(MoveModel move)
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
            await SaveMove(move);
            MoveModel machinesMove = new MoveModel();
            if (move.MoveBy == "TURK" && !move.OfferAccepted)
            {
                int machinesOffer = MachineAI.MachinesOffer(ret.Surplus, ret.Stubborn, ret.MachineDisValue,
                    move.ProposedAmount, GetLastMachineMove(workerId));

                machinesMove = new MoveModel()
                {
                    MoveBy = "MACH",
                    ProposedAmount = machinesOffer,
                    OfferAccepted = move.MoveBy == "TURK" && move.ProposedAmount == machinesOffer,
                    GameId = move.GameId,
                };

                await SaveMove(machinesMove);
            }
            var res = await GetCurrentGame(workerId);
            res.PartnersAgreed = move.OfferAccepted || machinesMove.OfferAccepted;
            return res;
        }

        

    }
}
