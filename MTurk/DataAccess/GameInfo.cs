using MTurk.Algo;
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



        internal float[] GetSubHistory(int i) => SubHistory.GetSubHistory(i, Game.MachineDisValue, Game.ShowMachinesDisValue ? Game.TurksDisValue : -1, Game.MachineStarts, MovesToFloat());

        public float[] MovesToFloat()
        {
            var res = new float[Moves.Count];
            for (int i = 0; i < Moves.Count; i++)
                res[i] = Moves[i].ProposedAmount;
            return res;
        }

    }
}