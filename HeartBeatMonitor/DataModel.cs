using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeartBeatMonitor
{
    class DataModel
    {
        public long HeartRate { get; set; }
        public long Speed { get; set; }
        public long Cadence { get; set; }
        public long Altitude { get; set; }
        public long Power { get; set; }
        public long PowerBalance { get; set; }
    }
}
