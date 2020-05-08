using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MTurk.DataAccess
{
    public class SessionInfo
    {
        public const double DollarsPerBar = 0.05;
        public string WorkerId;
        public DateTime Time;
        public int TotalProfit;
        public int GamesPlayed;
        public double TurksPayment { get => Math.Max(GamesPlayed * 2 * DollarsPerBar, TotalProfit * DollarsPerBar); }
    }
}
