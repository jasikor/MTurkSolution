using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MTurk.Data
{
    public class MoveModel
    {
        public int Id { get; set; }
        public DateTime Time { get; set; }
        public int GameId { get; set; }
        public string MoveBy { get; set; }
        public int ProposedAmount { get; set; }
        public bool OfferAccepted { get; set; }
        public override string ToString()
        {
            return $"{MoveBy[0]}:{ProposedAmount}";
        }
    }
}
