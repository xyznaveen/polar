using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HeartBeatMonitor
{
    public class PowerCalc
    {

        /// <summary>
        /// Generates and gives Normalized Power (NP) value.
        /// </summary>
        /// <param name="dataSet">the list which contains the array of data</param>
        /// <param name="indexOfPower"></param>
        /// <returns></returns>
        public static double GetNormalizedPower(List<string[]> dataSet, int indexOfPower)
        {
            // assume our default column index for power is 4
            // this is the data read from file
            // just the list of power can be passed here instead of list of array of strings
            if (indexOfPower == -1) indexOfPower = 4;

            double normalizedPower = 0.0;

            // default interval of 30 second is required to calculate normalized power
            int intervalSec = 30; 

            double rollingAverage = 0.0;

            List<double> rollingAverageList = new List<double>();

            for (int i = 0; i < dataSet.Count; i++)
            {
                double currentPower = AddDecimalToValue(dataSet[i][indexOfPower]);
                rollingAverage += currentPower;

                if (i % intervalSec == 0)
                {
                    rollingAverage = rollingAverage / intervalSec; // get the average power during the interval
                    rollingAverage = Math.Pow(rollingAverage, 4); // raise the average's power to the fourth
                    rollingAverage *= 0.5; // multiply by the time
                    rollingAverageList.Add(currentPower);
                    rollingAverage = 0.0; // reset values else calculation will not be right
                }
            }

            double fourthPowerAverage = GetFourthPowerAverageValue(rollingAverageList);

            normalizedPower = GetFourthRoot(fourthPowerAverage);

            return normalizedPower;
        }

        /// <summary>
        /// Calculate and returns the fourth root of a given number.
        /// </summary>
        /// <param name="fourthPowerAverage">the number whose fourth root must be calculated.</param>
        /// <returns>the fourth root of the number specified.</returns>
        public static double GetFourthRoot(double fourthPowerAverage)
        {
                return Math.Pow(fourthPowerAverage, 1.0 / 4);
        }

        /// <summary>
        /// Calculate the aerage for the rolling average of fourth powers
        /// </summary>
        /// <param name="rollingAverageList">the list of rolling average</param>
        /// <returns></returns>
        public static double GetFourthPowerAverageValue(List<double> rollingAverageList)
        {
            double fourthPowerAverage = 0.0;
            for (int rollingAvgCounter = 0; rollingAvgCounter < rollingAverageList.Count; rollingAvgCounter++)
            {
                fourthPowerAverage += rollingAverageList[rollingAvgCounter];
            }
            fourthPowerAverage = fourthPowerAverage / rollingAverageList.Count;

            return fourthPowerAverage;
        }

        /// <summary>
        /// Separate the last digit with decimal because the polar documentation says the values are separated by decimal.
        /// </summary>
        /// <param name="inputValue">the string to be converted to actual value</param>
        /// <returns></returns>
        public static double AddDecimalToValue(string inputValue)
        {
            if (inputValue.Length > 1)
            {
                inputValue = inputValue.Insert(inputValue.Length - 1, ".");
            }

            double output = 0.0;
            double.TryParse(inputValue, out output);

            return output;
        }

        /// <summary>
        /// Get the Functional Threshhold Power for the given power average.
        /// </summary>
        /// <param name="dataSet">the list of data which contains the values</param>
        /// <param name="indexOfPower">the array index of the power value</param>
        /// <returns>functional threshhold power of the provided data</returns>
        public static double GetFtp(List<string[]> dataSet, int indexOfPower)
        {
            double result = 0.0;
            if (indexOfPower <= -1) indexOfPower = 4;

            foreach(string[] data in dataSet)
            {
                // double power = AddDecimalToValue(data[indexOfPower]);
                double power = 0;
                double.TryParse(data[indexOfPower], out power);
                result += power;
            }


            // calculate average of the result
            result /= dataSet.Count;

            // 95% of the power is the functional threshhold power
            result *= 0.95;

            return result;
        }

        /// <summary>
        /// Calculate the intensity factor for the given Normalized Power and Functional Threshhold Power
        /// </summary>
        /// <param name="np">the normalized power calculated earlier.</param>
        /// <param name="ftp">the functional threshhold power calculated earlier.</param>
        /// <returns></returns>
        public static double GetIf(double np, double ftp)
        {
            return LimitTo2(np / ftp);
        }

        /// <summary>
        /// Round the decimal with many decimal places to 2 places.
        /// </summary>
        /// <param name="value">the number for rounding</param>
        /// <returns>resulting value with 2 decimal places</returns>
        public static double LimitTo2(double value)
        {
            return System.Math.Round(value, 2);
        }

        /// <summary>
        /// Calculate TSS for the given values
        /// </summary>
        /// <param name="seconds">total training time in seconds</param>
        /// <param name="np">the normalized power</param>
        /// <param name="intf">the intensity factor</param>
        /// <param name="ftp">the functional</param>
        /// <returns></returns>
        public static double GetTss(double seconds, double np, double intf, double ftp)
        {
            double tss = 0.0;

            tss = (seconds * np * intf) / (ftp * 3600) * 100;
            return LimitTo2(tss);
        }

    }
}
