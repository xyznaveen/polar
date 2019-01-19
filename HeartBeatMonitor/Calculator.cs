using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeartBeatMonitor
{
    class Calculator
    {

        List<string[]> dataList = new List<string[]>();

        public Calculator(List<string[]> dataList)
        {
            this.dataList = dataList;
        }

        public static double Str2Double(string str)
        {
            if (str.Length > 1)
            {
                str = str.Insert(str.Length - 1, ".");
            }
            return double.Parse(str);
        }

        public static double GetAverage(List<string[]> dataList, int index)
        {
            double total = 0;
            foreach (var items in dataList)
            {
                string str = items[index];

                if (str.Length > 1)
                {
                    str = str.Insert(str.Length - 1, ".");
                }

                total += double.Parse(str);
            }
            return total / dataList.Count;
        }

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

        public static double Limit2Two(double value)
        {
            return Math.Round(value, 2, MidpointRounding.AwayFromZero);
        }

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

        public static double GetSpeed(int interval, int rowCount, double averageSpeed)
        {
            var timeSpan = TimeSpan.FromSeconds(interval * rowCount);
            return Calculator.Limit2Two(timeSpan.TotalHours * averageSpeed);
        }

        public static double Mile2Km(double inputValue)
        {

            return Math.Round(inputValue * 1.60934, 2, MidpointRounding.AwayFromZero);
        }

        public static double Km2Mile(double inputValue)
        {
            return Math.Round(inputValue / 1.60934, 2, MidpointRounding.AwayFromZero);
        }

    }
}
