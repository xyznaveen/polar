using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeartBeatMonitor
{
    class Delegates
    {
        /// <summary>
        /// Called when the thread finishes loading contents from <code>FileHandler</code>.
        /// </summary>
        /// <param name="results"> list of data fetched from the file</param>
        public delegate void ReadContentsCallback(List<string[]> results);
    }
}
