using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HeartBeatMonitor
{
    public class Calculator
    {

        List<string[]> dataList = new List<string[]>();

        public Calculator(List<string[]> dataList)
        {
            this.dataList = dataList;
        }

        /// <summary>
        /// Convert string to double.
        /// </summary>
        /// <param name="str">the string representation of the string</param>
        /// <returns>the double value of the string</returns>
        public static double Str2Double(string str)
        {
            if (str.Length > 1)
            {
                str = str.Insert(str.Length - 1, ".");
            }
            return double.Parse(str);
        }

        /// <summary>
        /// Get the average from the provided list of the strings.
        /// </summary>
        /// <param name="psdData">the list of the data read from the file</param>
        /// <param name="index">the default index of the number to be read</param>
        /// <returns>the aerage value of number at the given index</returns>
        public static double GetAverage(List<string[]> psdData, int index)
        {
            double total = 0;
            foreach (var items in psdData)
            {
                string str = items[index];

                if (str.Length > 1)
                {
                    str = str.Insert(str.Length - 1, ".");
                }

                total += double.Parse(str);
            }

            total = total / psdData.Count;

            return total;
        }

        /// <summary>
        /// Gets the average speed of the given index of data
        /// </summary>
        /// <param name="index">the index of the array which contains the speed</param>
        /// <returns>the average value of the speed</returns>
        public double GetAverageSpeed(int index)
        {
            index = index < 0 ? 0 : index;
            double total = 0;
            try
            {
                foreach (var items in dataList)
                {
                    string str = items[index];

                    if (str.Length > 1)
                    {
                        str = str.Insert(str.Length - 1, ".");
                    }

                    total += double.Parse(str);
                }
            } catch(Exception ex)
            {

            }

            return total / dataList.Count;
        }

        /// <summary>
        /// Gets the maximum value among the passed list of array of strings at any index specified.
        /// </summary>
        /// <param name="dataList">the list of data which must be passed.</param>
        /// <param name="index">the index of the data</param>
        /// <returns>the maximum value among the passed string</returns>
        public static double GetMax(List<string[]> dataList, int index)
        {
            double max = 0.0D;
            foreach (var items in dataList)
            {
                string str = items[index];

                max = double.Parse(str) > max ? double.Parse(str) : max;
            }
            return max;
        }


        /// <summary>
        /// Gets the minimum value among the passed list of array of strings at any index specified.
        /// </summary>
        /// <param name="dataList">the data which contains array of strings as list</param>
        /// <param name="index">the index of array whose minimum value must be calculated</param>
        /// <returns>the minimum value in the specified index</returns>
        public static double GetMin(List<string[]> dataList, int index)
        {
            double max = double.Parse(dataList[index][0]);
            foreach (var items in dataList)
            {
                string str = items[index];

                max = double.Parse(str) < max ? double.Parse(str) : max;
            }
            return max;
        }

        /// <summary>
        /// Round off the value of passed double to 2 decimal places
        /// </summary>
        /// <param name="value">the input value which requires rounding</param>
        /// <returns>the rounded off value of the string</returns>
        public static double Limit2Two(double value)
        {
            return Math.Round(value, 2, MidpointRounding.AwayFromZero);
        }

        /// <summary>
        /// Get the maximum speed by providing index of the speed.
        /// </summary>
        /// <param name="index">the index of the speed in array</param>
        /// <returns>the maximum speed as double</returns>
        public double GetMaxSpeed(int index)
        {
            index = index < 0 ? 0 : index;
            double max = 0.0D;
            foreach (var items in dataList)
            {
                string str = items[index];

                if (str.Length > 1)
                {
                    str = str.Insert(str.Length - 1, ".");
                }

                max = double.Parse(str) > max ? double.Parse(str) : max;
            }

            return max;
        }

        /// <summary>
        /// Get the speed from the durations.
        /// </summary>
        /// <param name="interval">continious data interval</param>
        /// <param name="rowCount">total number of rows</param>
        /// <param name="averageSpeed">the average speed of the dataset</param>
        /// <returns></returns>
        public static double GetSpeed(int interval, int rowCount, double averageSpeed)
        {
            var timeSpan = TimeSpan.FromSeconds(interval * rowCount);
            return Calculator.Limit2Two(timeSpan.TotalHours * averageSpeed);
        }

        /// <summary>
        /// Convert the mile to kilometer.
        /// </summary>
        /// <param name="inputValue">miles</param>
        /// <returns>converted kilometer value</returns>
        public static double Mile2Km(double inputValue)
        {
            return Math.Round(inputValue * 1.60934, 2, MidpointRounding.AwayFromZero);
        }

        /// <summary>
        /// Kilometer to miles
        /// </summary>
        /// <param name="inputValue">kilometer</param>
        /// <returns>the converted miles value</returns>
        public static double Km2Mile(double inputValue)
        {
            return Math.Round(inputValue / 1.60934, 2, MidpointRounding.AwayFromZero);
        }

    }
}
