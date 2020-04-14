using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MTurk.Data
{
    public class Session
    {
        public int Id { get; set; }
        public string WorkerId { get; set; }
        public DateTime Time { get; set; }
        public Session()
        {
            WorkerId = "<UNKNOWN>";
            Time = DateTime.UtcNow;
        }
    }
}
