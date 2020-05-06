﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MTurk.Models
{
    public class QueryRows
    {
        public int Id;
        public DateTime StartTime;
        public DateTime EndTime;
        public int Surplus;
        public int TurksDisValue;
        public int MachineDisValue;
        public int TimeOut;
        public double Stubborn;
        public bool MachineStarts;
        public string WorkerId;
        public int ProposedAmount;
        public override string ToString()
        {
            return $"{Id} {WorkerId} {StartTime} {EndTime} {Surplus} {TurksDisValue} {MachineDisValue} {TimeOut} {Stubborn} {(MachineStarts ? 1 : 0)} ";
        }
    }
}
