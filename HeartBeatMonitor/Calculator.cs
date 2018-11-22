using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeartBeatMonitor
{
    class Calculator
    {

        List<string[]> dataList;

        public Calculator(List<string[]> dataList)
        {
            this.dataList = dataList;
        }

        public double GetAverageSpeed(int index)
        {
            index = index < 0 ? 0 : index;
            double total = 0;
            foreach (var items in dataList)
            {
                string str = items[index];

                if(str.Length > 1)
                {
                    str = str.Insert(str.Length - 1, ".");
                }

                total += double.Parse(str);
            }

            return total / dataList.Count;
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
    }
}
