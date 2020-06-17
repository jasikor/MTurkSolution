using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace MTurk.Algo
{
    public class SubHistory
    {
        public const int SubHistoryLength = 11;
        public static float[] GetSubHistory(int i, int machineDisValue, bool machineStarts,  float[] moves)
        {
            var x = new float[SubHistoryLength];
            x[0] = machineDisValue;
            x[1] = machineStarts ? 1f : 0f;
            x[2] = i;
            x[3] = TurksLastConcession(i, moves);
            x[4] = MachinesLastConcession(i, moves);
            x[5] = TurksFirst(moves, machineStarts);
            x[6] = MachinesFirst(moves, machineStarts);
            x[7] = TurksLast1(i,moves);
            x[8] = MachinesLast1(i, moves) ;
            x[9] = TurksLast(i, moves, machineStarts) ;
            x[10] = MachinesLast(i, moves, machineStarts);
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
