using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MTurk.Models
{
    public class MovesWithGames
    {
        public int Id;
        public int SessionId;
        public DateTime StartTime;
        public DateTime EndTime;
        public int Surplus;
        public int TurksDisValue;
        public int MachineDisValue;
        public int TimeOut;
        public double Stubborn;
        public bool MachineStarts;
        public int? TurksProfit;
        public bool ShowMachinesDisValue;
        public string WorkerId;
        public int ProposedAmount;
        public string MoveBy;

        public override string ToString()
        {
            return $"{Id} {WorkerId} {StartTime} {EndTime} {Surplus} {TurksDisValue} {MachineDisValue} {TimeOut} {Stubborn} {(MachineStarts ? 1 : 0)} {(ShowMachinesDisValue ? 1 : 0)} ";
        }
    }
}
