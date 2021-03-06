﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeartBeatMonitor
{
    class Helper
    {

        /// <summary>
        /// Helps identify the version of device from the provided length of SMode.
        /// </summary>
        /// <param name="value"> value extracted from the SMode field of configuration</param>
        /// <returns> version number based on the provided SMode value</returns>
        public static int IdentifySmodeType(string value)
        {
            int length = value.Length;
            if (length == 3)
            {
                return 105;
            } else if(length > 3 && length <= 8)
            {
                return 106;
            } else if (length == 9)
            {
                return 107;
            }
            return 105;
        }

        /// <summary>
        /// Get device's actual name based on the Monitor code saved in the configuration file.
        /// </summary>
        /// <param name="identifier"> model code from the configuration</param>
        /// <returns> name of the device used to dump the realtime data</returns>
        public static string GetDeviceName(int identifier)
        {
            string result = null;
            switch (identifier)
            {
                case 1:
                    {
                        result = "Polar Sport Tester / Vantage XL";
                        break;
                    }
                case 2:
                    {
                        result = "Polar Vantage NV (VNV)";
                        break;
                    }
                case 3:
                    {
                        result = "Polar Accurex Plus";
                        break;
                    }
                case 4:
                    {
                        result = "Polar XTrainer Plus";
                        break;
                    }
                case 6:
                    {
                        result = "Polar S520";
                        break;
                    }
                case 7:
                    {
                        result = "Polar Coach";
                        break;
                    }
                case 8:
                    {
                        result = "Polar S210";
                        break;
                    }
                case 9:
                    {
                        result = "Polar S410";
                        break;
                    }
                case 10:
                    {
                        result = "Polar S510";
                        break;
                    }
                case 11:
                    {
                        result = "Polar S610 / S610i";
                        break;
                    }
                case 12:
                    {
                        result = "Polar S710 / S710i / S720i";
                        break;
                    }
                case 13:
                    {
                        result = "Polar S810 / S810i";
                        break;
                    }
                case 15:
                    {
                        result = "Polar E600";
                        break;
                    }
                case 20:
                    {
                        result = "Polar AXN500";
                        break;
                    }
                case 21:
                    {
                        result = "Polar AXN700";
                        break;
                    }
                case 22:
                    {
                        result = "Polar S625X / S725X";
                        break;
                    }
                case 23:
                    {
                        result = "Polar S725";
                        break;
                    }
                case 33:
                    {
                        result = "Polar CS400";
                        break;
                    }
                case 34:
                    {
                        result = "Polar CS600X";
                        break;
                    }
                case 35:
                    {
                        result = "Polar CS600";
                        break;
                    }
                case 36:
                    {
                        result = "Polar RS400";
                        break;
                    }
                case 37:
                    {
                        result = "Polar RS800";
                        break;
                    }
                case 38:
                    {
                        result = "Polar RS800X";
                        break;
                    }
                default:
                    break;
            }
            return result;
        }

    }
}
