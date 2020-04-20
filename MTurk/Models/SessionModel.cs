using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MTurk.Data
{
    public class SessionModel
    {
        public int Id { get; set; }
        public string WorkerId { get; set; }
        public DateTime Time { get; set; }
        public SessionModel()
        {
            WorkerId = "<UNKNOWN>";
            Time = DateTime.UtcNow;
        }
    }
}
