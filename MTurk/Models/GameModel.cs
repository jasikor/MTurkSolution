using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MTurk.Data
{
    public class GameModel
    {
        public int Id { get; set; }
        public int SessionId { get; set; }
        public int GameParameterId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int Surplus { get; set; }
        public int TurksDisValue { get; set; }
        public int MachineDisValue { get; set; }
        public int TimeOut { get; set; }
        public double Stubborn { get; set; }
        public bool MachineStarts { get; set; }
        public int? TurksProfit { get; set; }
    }
}
