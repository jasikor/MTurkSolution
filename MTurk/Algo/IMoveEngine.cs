using MTurk.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MTurk.Algo
{
    public interface IMoveEngine
    {
        public int GetMachinesOffer(GameInfo g);
    }
}
