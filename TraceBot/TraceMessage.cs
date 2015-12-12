using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TraceBot
{
    public class TraceMessage
    {
        public TraceMessage()
        {
            Time = DateTime.UtcNow;
        }
        public string Message { get; set; }
        public TraceType TraceType { get; set; }
        public DateTime Time { get; }
    }

    public enum TraceType
    {
        Information,
        Error
    }
}

