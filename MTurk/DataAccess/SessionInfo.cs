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
        /// <summary>
        ///  zarobki = Max( Nr_skonczonych_gier * 3 * x, zebrane_bars * x)
        ///  X = $/bar(do ustalenia, narazie zakladam x = $0.05)
        /// </summary>
        public double TurksPayment { get => Math.Max(GamesPlayed * 3 * DollarsPerBar, TotalProfit * DollarsPerBar); }
    }
}
