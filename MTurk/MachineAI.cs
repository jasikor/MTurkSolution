﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Threading.Tasks;

namespace MTurk
{
    public static class MachineAI
    {
        private static Random rnd = new Random();
        private static int RandomInteger(int minInclusive, int maxInclusive)
        {
            if (minInclusive > maxInclusive)
                return minInclusive;
            return rnd.Next(minInclusive, maxInclusive + 1); // rnd.Next(min, max), min inclusive, max exclusive
        }

        public static int GetMachinesOffer(int surplus, double stubborn, int machineDisValue, int? workerLastDemand, int? machineLastOffer)
        {
            if (machineLastOffer is null)
                machineLastOffer = 1;
            if (workerLastDemand is null)
                workerLastDemand = surplus;
            int aIOffer = Math.Min((int)machineLastOffer, surplus - machineDisValue);
            if (workerLastDemand <= aIOffer)
                return (int)workerLastDemand;
            if (rnd.NextDouble() < stubborn)
                aIOffer = RandomInteger(aIOffer - 1, aIOffer + 1);
            else
                aIOffer = RandomInteger(aIOffer, (int)workerLastDemand - 1);

            var res = Math.Clamp(aIOffer, 0, surplus - machineDisValue);
            return res;

        }
    }
}
