using System.Collections.Generic;

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
            for(int i =0; i < Moves.Count -1; i++)
            {
                if (Moves[i].MoveBy == Moves[i + 1].MoveBy)
                    return false;
            }
            return true;
        }
    }
}