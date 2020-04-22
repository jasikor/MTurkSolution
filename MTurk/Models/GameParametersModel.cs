using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MTurk.Models
{
    public class GameParametersModel
    {
        public int Id;
        public int Surplus;
        public int TurksDisValue;
        public int MachineDisValue;
        public int TimeOut;
        public double Stubborn;
        public bool MachineStarts;
    }
}
