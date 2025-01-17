﻿using MTurk.SQLDataAccess;
using MTurk;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;
using MTurk.Models;
using MTurk.Pages;
using MTurk.DataAccess;
using System.Data.SqlTypes;
using MTurk.AI;
using MTurk.Algo;

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
            string sql = @"select * from Sessions where WorkerId = @WorkerId";
            SessionModel sm = await _db.LoadDataSingleAsync<dynamic, SessionModel>(sql, new { WorkerId = workerId });
            if (sm != null)
                return sm;
            var dollarsPerBar = await GetDollarsPerBar();

            sql = @"insert into dbo.Sessions (WorkerId, Time, DollarsPerBar)
                           output inserted.*
                           values (@WorkerId, @Time, @DollarsPerBar)";
            DateTime utcNow = DateTime.UtcNow;
            sm = new SessionModel() { WorkerId = workerId.ToUpper(), Time = utcNow, DollarsPerBar = dollarsPerBar };
            return await _db.SaveData<SessionModel, SessionModel>(sql, sm);
        }

        public List<SessionInfo> GetAHandfullOfLastSessions()
        {
            string sql =
                  @"select s.[Time], s.WorkerId, s.DollarsPerBar, g.totalProfit, g.gamesPlayed from sessions s
                    inner join 
                        (select Games.SessionId, sum(Games.TurksProfit) as totalProfit, count(Id) as gamesPlayed 
                        from games 
                        where games.TurksProfit is not null  
                        group by Games.SessionId) g
                    on s.Id = g.SessionId
                    order by s.Id desc";
            return _db.LoadDataList<SessionInfo, dynamic>(sql, new { });
        }

        /// <summary>
        /// Gets unfinished or new game in session.
        /// </summary>
        /// <param name="workerId"></param>
        /// <returns>null if all games have been played, new game if all games so far are finished, or unfinished game</returns>
        public async Task<GameInfo> GetCurrentGame(string workerId, string algoVersion)
        {
            string sql = @"select Top 1 g.* 
                           from Games g, Sessions s
                           where g.EndTime is null 
                                and s.WorkerId = @WorkerId 
                                and g.SessionId = s.Id";
            GameModel gm = null;
            try
            {
                gm = await _db.LoadDataSingleAsync<dynamic, GameModel>(sql, new { WorkerId = workerId });
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

            if (gm is null)
                return await StartNewGame(workerId, algoVersion);
            else
                return new GameInfo()
                {
                    Game = gm,
                    Moves = new List<MoveModel>(), // TODO: wczytać liste ruchów z bazy
                    PartnersAgreed = false,
                };
        }

        /// <summary>
        /// Starts new game using first not used GameParameter
        /// </summary>
        /// <param name="workerId"></param>
        /// <returns>new game or null if there are no more unused GameParameters</returns>
        public async Task<GameInfo> StartNewGame(string workerId, string algoVersion)
        {
            string sql =
                @"select Top 1 gp.* from GameParameters gp
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
                gameParameter = await _db.LoadDataSingleAsync<dynamic, GameParametersModel>(sql, new { WorkerId = workerId });
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
                ShowMachinesDisValue = gameParameter.ShowMachinesDisValue,
                AlgoVersion = algoVersion,
            };
            sql = @"insert into dbo.Games 
                              (SessionId,
                               GameParameterId, StartTime, Surplus, TurksDisValue, MachineDisValue, TimeOut, Stubborn, MachineStarts, ShowMachinesDisValue, AlgoVersion)
                           output inserted.*
                           values 
                              ((select Id from Sessions where WorkerId = @WorkerId), 
                               @GameParameterId, @StartTime, @Surplus, @TurksDisValue, @MachineDisValue, @TimeOut, @Stubborn, @MachineStarts, @ShowMachinesDisValue, @AlgoVersion)";
            try
            {
                var res = await _db.SaveData<dynamic, GameModel>(sql, g);
                return new GameInfo()
                {
                    Game = res,
                    Moves = new List<MoveModel>(),
                };
            }
            catch (SqlException)
            {
                return null;
            }
        }

        public async Task SaveMove(MoveModel move)
        {
            move.Time = DateTime.UtcNow;
            string sql = @"insert into dbo.Moves 
                            (Time, GameId, MoveBy, ProposedAmount, OfferAccepted)
                           values 
                            (@Time, @GameId, @MoveBy, @ProposedAmount, @OfferAccepted)";

            await _db.SaveDataAsync<MoveModel>(sql, move);
        }

        public async Task FinishGame(GameModel game)
        {
            var endTime = DateTime.UtcNow;
            string sql = @"update Games
                           set EndTime = @EndTime, TurksProfit = @TurksProfit
                           where Id = @GameId";

            await _db.SaveDataAsync<dynamic>(sql, new { EndTime = endTime, GameId = game.Id, TurksProfit = game.TurksProfit });
        }

        

        

        
        private async Task<double> GetDollarsPerBar()
        {
            string sql = @"select * from Settings where [Key] = 'DollarsPerBar'";
            var dollarsPerBar = await _db.LoadDataSingleAsync<dynamic, SettingModel>(sql, new { });
            double res;
            if (Double.TryParse(dollarsPerBar.Value, out res))
                return res;
            else
                return 0.05;

        }

    }

}
