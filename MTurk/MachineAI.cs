using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            Debug.Assert(machineLastOffer >= 0 && machineLastOffer <= surplus);
            Debug.Assert(workerLastDemand >= 0 && workerLastDemand <= surplus);

            if (workerLastDemand <= machineLastOffer)
                return (int)workerLastDemand;

            var first = Math.Min((int)machineLastOffer, surplus - machineDisValue);
            var last = Math.Min((int)workerLastDemand, surplus - machineDisValue);

            var dist = new double[last - first + 1];

            for (int i = 0; i < dist.Length; i++)
            {
                var val = Math.Exp(-stubborn * (machineDisValue + first + last) * (i + first) / last);
                Debug.WriteLine($"val = {val}");
                dist[i] = val;
            }

            var sum = dist.Sum();
            for (int i = 0; i < dist.Length; i++)
                dist[i] = dist[i] / sum;

            Debug.Assert(Math.Abs(dist.Sum() - 1.0) < 0.00000001);

            var cumDist = new double[dist.Length];
            double prob = 0.0;
            for (int i = 0; i < cumDist.Length; i++)
            {
                prob += dist[i];
                cumDist[i] = prob;
                Debug.WriteLine($"cumDist[{i}] = {cumDist[i]}");

            }

            Debug.Assert(Math.Abs(cumDist[cumDist.Length - 1] - 1.0) < 0.00000001);

            cumDist[cumDist.Length - 1] = 1.0;

            int aIOffer = CumulativeRandom(cumDist) + first;

            Debug.Assert(aIOffer >= 0 && aIOffer <= surplus);

            if (rnd.NextDouble() < 0.2 && machineLastOffer >= 1)
                aIOffer = (int)machineLastOffer - 1;

            return aIOffer;
        }
        private static int CumulativeRandom(double[] cumDist)
        {
            var random = rnd.NextDouble();
            int i;
            for (i = 0; i < cumDist.Length; i++)
                if (random <= cumDist[i])
                    break;
            return i;
        }
    }
}
