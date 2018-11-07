using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HeartBeatMonitor.Delegates;

namespace HeartBeatMonitor
{

    /// <summary>
    /// Get data dump with thread
    /// </summary>
    class FileHandler
    {

        private string fileName;

        private ReadContentsCallback callback;

        public FileHandler(string fileName, ReadContentsCallback callback)
        {
            this.fileName = fileName;
            this.callback = callback;
        }

        /// <summary>
        /// 
        /// </summary>
        public void FetchHrmData()
        {
            List<string[]> result = new List<string[]>();
            bool fileExists = File.Exists(fileName);

            if (fileExists)
            {
                bool startAppending = false;
                string line = " ";
                try
                {
                    Console.WriteLine("Fetching started from file.");
                    StreamReader file = new StreamReader(fileName);
                    while ((line = file.ReadLine()) != null)
                    {

                        if (!startAppending)
                        {

                            startAppending = line.Equals("[HRData]") ? true : false;
                            continue;
                        }

                        if (startAppending)
                        {
                            string[] temp = line.Split('\t');
                            result.Add(temp);
                        }
                    }
                    file.Close();
                    callback(result);
                    Console.WriteLine("Finished fetching data from file.");
                }
                catch (IndexOutOfRangeException)
                {
                    callback(result);
                }
            }
        }

    }
}
