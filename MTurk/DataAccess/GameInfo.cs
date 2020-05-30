using MTurk.UIModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace MTurk.Data
{
    public class GameInfo
    {
        public string WorkerId;
        public List<MoveModel> Moves;
        public GameModel Game;
        public bool PartnersAgreed;
        public bool AreLastTwoMovesEqual()
        {
            if (Moves.Count < 2)
                return false;
            return Moves[Moves.Count - 1].ProposedAmount == Moves[Moves.Count - 2].ProposedAmount;
        }
        public bool IsValid()
        {
            if (Game.TurksProfit is null)
                return false;
            if (Moves.Count == 0)
                return false;
            if (Moves.Count < 9)
                return false;
            if (Moves.Count == 1 && !Game.MachineStarts)
                return false;
            if (Game.TurksProfit is null)
                return false;
            if (WorkerId[0] != 'A')
                return false;
            for (int i = 0; i < Moves.Count - 1; i++)
            {
                if (Moves[i].MoveBy == Moves[i + 1].MoveBy)
                    return false;
            }

            return true;
        }
        public void TrimMoves()
        {
            int index = Moves.FindIndex(t => t.ProposedAmount < 0);
            if (index >= 0)
                Moves.RemoveRange(index, Moves.Count - index);
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

        public static float MachinesFirst(float[]moves, bool machineStarts)
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

        internal float[] GetSubHistory(int i) => GetSubHistory(i, Game.MachineDisValue, Game.MachineStarts, MovesToFloat());
        internal static float[] GetSubHistory(int i, int machineDisValue, bool machineStarts,  float[] moves)
        {
            var x = new float[11];
            x[0] = machineDisValue / 20f;
            x[1] = machineStarts ? 1f : 0f;
            x[2] = (float)i / moves.Length;
            x[3] = (float)(TurksLastConcession(i, moves) + 1) / moves.Length;
            x[4] = (float)(MachinesLastConcession(i, moves) + 1) / moves.Length;
            x[5] = (float)(TurksFirst(moves, machineStarts) + 1) / 21f;
            x[6] = (float)(MachinesFirst(moves, machineStarts) + 1) / 21f;
            x[7] = (float)(TurksLast1(i,moves) + 1) / 21f;
            x[8] = (float)(MachinesLast1(i, moves) + 1) / 21f;
            x[9] = (float)(TurksLast(i, moves, machineStarts) + 1) / 21f;
            x[10] = (float)(MachinesLast(i, moves, machineStarts) + 1) / 21f;
            return x;
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
        public float[] MovesToFloat()
        {
            var res = new float[Moves.Count];
            for (int i = 0; i < Moves.Count; i++)
                res[i] = Moves[i].ProposedAmount;
            return res;
        }
    }
}