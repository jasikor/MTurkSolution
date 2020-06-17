using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace MTurk.Algo
{
    public class SubHistory
    {
        public const int SubHistoryLength = 12;

        public const int MachineDisValueIndex = 0;
        public const int MachineStartsIndex  = 1;
        public const int NoOfMovesIndex  = 2;
        public const int TurksLastConcessionIndex  = 3;
        public const int MachinesLastConcessionIndex  = 4;
        public const int TurksFirstOfferIndex  = 5;
        public const int MachinesFirstOfferIndex  = 6;
        public const int TurksLast1OfferIndex  = 7;
        public const int MachinesLast1OfferIndex  = 8;
        public const int TurksLastOfferIndex  = 9;
        public const int MachinesLastOfferIndex  = 10;
        public const int TurksDisValueIndex  = 11;
        public static float[] GetSubHistory(int i, int machineDisValue, int turksDisValue, bool machineStarts,  float[] moves)
        {
            var x = new float[SubHistoryLength];
            x[MachineDisValueIndex] = machineDisValue;
            x[MachineStartsIndex] = machineStarts ? 1f : 0f;
            x[NoOfMovesIndex] = i;
            x[TurksLastConcessionIndex] = TurksLastConcession(i, moves);
            x[MachinesLastConcessionIndex] = MachinesLastConcession(i, moves);
            x[TurksFirstOfferIndex] = TurksFirst(moves, machineStarts);
            x[MachinesFirstOfferIndex] = MachinesFirst(moves, machineStarts);
            x[TurksLast1OfferIndex] = TurksLast1(i,moves);
            x[MachinesLast1OfferIndex] = MachinesLast1(i, moves) ;
            x[TurksLastOfferIndex] = TurksLast(i, moves, machineStarts) ;
            x[MachinesLastOfferIndex] = MachinesLast(i, moves, machineStarts);
            x[TurksDisValueIndex] = turksDisValue;
            return x;
        }

        public static int TurksLastConcession(int i, float[] moves)
        {
            i--;
            int res = -1;
            for (; i - 2 >= 0; i -= 2)
            {
                if (moves[i] < moves[i - 2])
                {
                    res = i;
                    break;
                }
            }
            return res;
        }

        public static int MachinesLastConcession(int i, float[] moves)
        {
            int res = -1;
            for (; i - 2 >= 0; i -= 2)
            {
                if (moves[i] > moves[i - 2])
                {
                    res = i;
                    break;
                }
            }
            return res;
        }

        public static float TurksFirst(float[] moves, bool machineStarts)
        {
            if (moves.Length == 1)
                return -1;
            Debug.Assert(moves.Length > (machineStarts ? 0 : 1));
            return moves[machineStarts ? 1 : 0];
        }

        public static float MachinesFirst(float[] moves, bool machineStarts)
        {
            Debug.Assert(moves.Length > (machineStarts ? 0 : 1));
            return moves[machineStarts ? 0 : 1];
        }
        public static float MachinesLast(int i, float[] moves, bool machineStarts)
        {
            Debug.Assert(machineStarts ? i % 2 == 0 : i % 2 == 1);
            return moves[i];
        }

        public static float TurksLast(int i, float[] moves, bool machineStarts)
        {
            if (machineStarts && i == 0)
                return -1;
            else
                return moves[i - 1];
        }

        public static float MachinesLast1(int i, float[] moves)
        {
            if (i < 2)
                return -1;
            else
                return moves[i - 2];
        }

        public static float TurksLast1(int i, float[] moves)
        {
            if (i < 3)
                return -1;
            else
                return moves[i - 3];
        }


    }
}
