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
    /// Fetches and manipulates the realtime data dump.
    /// </summary>
    class FileHandler
    {

        private string fileName;

        private ReadContentsCallback callback;
        private ConfigurationLoadedCallback confCallback;

        /// <summary>
        /// Constructor with normal file reading configuration.
        /// </summary>
        /// <param name="fileName">the file which should be opened.</param>
        /// <param name="callback">the callback which will be used after async loading of the file is complete</param>
        public FileHandler(string fileName, ReadContentsCallback callback)
        {
            this.fileName = fileName;
            this.callback = callback;
        }

        /// <summary>
        /// Constructor with callback for parameters reading.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="confCallback"></param>
        public FileHandler(string fileName, ConfigurationLoadedCallback confCallback)
        {
            this.fileName = fileName;
            this.confCallback = confCallback;
        }

        /// <summary>
        /// Get the parameters from the selected file.
        /// </summary>
        public void FetchParameters()
        {
            var config = new Dictionary<string, string>();
            bool fileExists = File.Exists(@fileName);

            if (!fileExists)
            {
                // file doesn't exist, no point in continuing 
                confCallback(config);
            }

            bool startAppending = false;
            string line;
            StreamReader file = new StreamReader(fileName);
            try
            {
                while ((line = file.ReadLine()) != null)
                {
                    if (!startAppending)
                    {
                        startAppending = line.Equals("[Params]") ? true : false;
                        continue;
                    }

                    if (string.IsNullOrEmpty(line) || string.IsNullOrWhiteSpace(line))
                    {

                        // finished loading configuration
                        break;
                    }

                    if (startAppending)
                    {
                        string[] temp = line.Split('=');
                        config.Add(temp[0], temp[1]);
                    }
                }
            }
            catch (IndexOutOfRangeException)
            {

            }
            finally
            {
                if (file != null)
                {
                    file.Close();
                }

                confCallback(config);
            }
        }

        /// <summary>
        /// Get realtime data dumped from the monitor
        /// </summary>
        public void FetchHrmData()
        {
            List<string[]> result = new List<string[]>();
            bool fileExists = File.Exists(@fileName);

            if (!fileExists)
            {
                // file doesn't exist, no point in continuing 
                callback(result);
            }

            bool startAppending = false;
            string line;
            StreamReader file = new StreamReader(fileName);
            try
            {
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
                MainForm.newData = true;
            }
            catch (IndexOutOfRangeException)
            {

            }
            finally
            {
                if (file != null)
                {
                    file.Close();
                }

                callback(result);
            }
        }
    }
}
