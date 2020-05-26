using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace MTurk.Data
{
    public class GameInfo
    {
        public List<MoveModel> Moves;
        public GameModel Game;
        public bool PartnersAgreed;
        public bool IsValid()
        {
            if (Game.TurksProfit is null)
                return false;
            if (Moves.Count == 0)
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
        public int TurksLastConcession(int upToMove) => -3;
        public int MachinesLastConcession(int upToMove) => -5;

        public int TurksFirst()
        {
            if (Moves.Count == 1)
                return -1;
            Debug.Assert(Moves.Count > (Game.MachineStarts ? 0 : 1));
            return Moves[Game.MachineStarts ? 1 : 0].ProposedAmount;
        }

        public int MachinesFirst()
        {
            Debug.Assert(Moves.Count > (Game.MachineStarts ? 0 : 1));
            return Moves[Game.MachineStarts ? 0 : 1].ProposedAmount;
        }

        public int MachinesLast(int i)
        {
            Debug.Assert(Game.MachineStarts ? i % 2 == 0 : i % 2 == 1);
            return Moves[i].ProposedAmount;
        }

        public int TurksLast(int i)
        {
            if (Game.MachineStarts && i == 0)
                return -1;
            else
                return Moves[i - 1].ProposedAmount;
        }

        public int MachinesLast1(int i)
        {
            if (i < 2)
                return -1;
            else
                return Moves[i - 2].ProposedAmount;
        }

        public int TurksLast1(int i)
        {
            if (i < 3)
                return -1;
            else
                return Moves[i - 3].ProposedAmount;
        }
    }
}