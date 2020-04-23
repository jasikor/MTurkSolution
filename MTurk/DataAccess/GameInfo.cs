using System.Collections.Generic;

namespace MTurk.Data
{
    public class GameInfo
    {
        public int Id;
        public List<MoveModel> Moves;
        public int Surplus;
        public int TurksDisValue;
        public int MachineDisValue;
        public int TimeOut;
        public double Stubborn;
        public bool MachineStarts;
    }
}