using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MTurk.Models
{
    public class GameParametersModel
    {
        public int Id { get; set; }
        public int Surplus { get; set; }
        public int TurksDisValue { get; set; }
        public int MachineDisValue { get; set; }
        public int TimeOut { get; set; }
        public double Stubborn { get; set; }
        public bool MachineStarts { get; set; }
        public bool ShowMachinesDisValue{ get; set; }
    }
}
