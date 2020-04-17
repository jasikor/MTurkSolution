using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MTurk.Data
{
    public class Game
    {
        public int Id { get; set; }
        public int SessionId { get; set; }
        public DateTime StartTime { get; set; }
        public int TurksDisValue { get; set; }
        public int MachineDisValue { get; set; }
        // TODO: Kto zaczyna

    }
}
