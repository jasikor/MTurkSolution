using MTurk.Data;
using MTurk.Models;
using MTurk.SQLDataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MTurk.DataAccess
{
    public class HistoricalGamesService : IHistoricalGamesService
    {
        private readonly ISqlDataAccess _db;

        public HistoricalGamesService(ISqlDataAccess db)
        {
            _db = db;
        }
        public List<MovesWithGames> GetMovesWithGames(DateTime startTime, DateTime endTime)
        {
            FixDefaults(ref startTime, ref endTime);
            string sql = @"Select g.*, s.WorkerId, m.ProposedAmount, m.MoveBy  
                                from Sessions s
                                join games g on g.SessionId = s.Id
                                join moves m on m.GameId = g.Id
                                where WorkerId like 'A%' and 
                                    g.StartTime >= @StartTime and 
                                    g.EndTime <= @EndTime
                                order by g.Id desc, m.Id";


            return _db.LoadDataList<MovesWithGames, dynamic>(sql, new { EndTime = endTime, StartTime = startTime });
        }

        private static void FixDefaults(ref DateTime startTime, ref DateTime endTime)
        {
            if (startTime == default(DateTime))
                startTime = new DateTime(2020, 1, 1);
            if (endTime == default(DateTime))
                endTime = DateTime.UtcNow.AddYears(100);
        }

        public IList<GameInfo> GetGameInfos(DateTime startTime, DateTime endTime)
        {
            FixDefaults(ref startTime, ref endTime);
            var res = new List<GameInfo>();
            var movesWithGames = GetMovesWithGames(startTime, endTime);
            if (movesWithGames.Count == 0)
                return res;
            int currentGame = 0;
            while (currentGame < movesWithGames.Count - 1)
            {
                var row = movesWithGames[currentGame];
                res.Add(new GameInfo()
                {
                    WorkerId = row.WorkerId,
                    Game = new GameModel()
                    {
                        Id = row.Id,
                        SessionId = row.SessionId,
                        StartTime = row.StartTime,
                        EndTime = row.EndTime,
                        Surplus = row.Surplus,
                        TurksDisValue = row.TurksDisValue,
                        MachineDisValue = row.MachineDisValue,
                        TimeOut = row.TimeOut,
                        Stubborn = row.Stubborn,
                        MachineStarts = row.MachineStarts,
                        TurksProfit = row.TurksProfit,
                        ShowMachinesDisValue = row.ShowMachinesDisValue,

                    },
                    Moves = new List<MoveModel>(),
                });

                for (int i = currentGame; ; i++)
                {
                    if (i >= movesWithGames.Count)
                        return res;
                    if (movesWithGames[currentGame].Id == movesWithGames[i].Id)
                        res[res.Count - 1].Moves.Add(
                            new MoveModel()
                            {
                                MoveBy = movesWithGames[i].MoveBy,
                                ProposedAmount = movesWithGames[i].ProposedAmount,
                            }
                            );
                    else
                    {
                        currentGame = i;
                        break;
                    }
                }
            }
            return res;
        }

    }
}
