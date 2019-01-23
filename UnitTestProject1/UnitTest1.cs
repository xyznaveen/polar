using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        /// <summary>
        /// This method tests the calculation of the normalized power for the given input of power.
        /// </summary>
        [TestMethod]
        public void GetNormalizedPowerTest()
        {
            List<string[]> inputData = new List<string[]>();

            for (int i = 0; i < 100; i++)
            {
                inputData.Add(new string[] {"10", "10", "10", "10", "10"});
            }

            double normalizedPower = HeartBeatMonitor.PowerCalc.GetNormalizedPower(inputData, -1);

            Assert.AreEqual(1, normalizedPower);
        }

        /// <summary>
        /// This method tests for the validity of the functional threshhold power of the array of data provided.
        /// </summary>
        [TestMethod]
        public void GetFtpTest()
        {
            List<string[]> inputData = new List<string[]>();

            for (int i = 0; i < 100; i++)
            {
                inputData.Add(new string[] { "5", "10", "20", "25", "30" });
            }

            double normalizedPower = HeartBeatMonitor.PowerCalc.GetFtp(inputData, -1);

            Assert.AreEqual(28.5, normalizedPower);
        }

        /// <summary>
        /// This method checks if the double value passed is rounded off to 2 decimal places.
        /// </summary>
        [TestMethod]
        public void LimitTo2Test()
        {
            double output = HeartBeatMonitor.PowerCalc.LimitTo2(1.546);

            Assert.AreEqual(1.55, output);
        }

        /// <summary>
        /// This method verifies the fourth root for any given number.
        /// For example the fourth root of 81 is 3.
        /// </summary>
        [TestMethod]
        public void GetFourthRootTest()
        {
            double output = HeartBeatMonitor.PowerCalc.GetFourthRoot(81);

            Assert.AreEqual(3, output);
        }

        /// <summary>
        /// This method gets the average of the list of power to the four raised values.
        /// </summary>
        [TestMethod]
        public void GetFourthPowerAverageValueTest()
        {
            List<double> inputValues = new List<double>();
            for (int i = 0; i < 100; i++)
            {
                inputValues.Add(i);
            }

            double output = HeartBeatMonitor.PowerCalc.GetFourthPowerAverageValue(inputValues);

            Assert.AreEqual(49.5, output);
        }

        /// <summary>
        /// This method checks if the original method adds decimal to the ending of the provided value or not.
        /// </summary>
        [TestMethod]
        public void AddDecimalToValueTest()
        {
            string input = "101";

            double output = HeartBeatMonitor.PowerCalc.AddDecimalToValue(input);
            Assert.AreEqual(10.1, output);

            input = "231";
            output = HeartBeatMonitor.PowerCalc.AddDecimalToValue(input);
            Assert.AreEqual(23.1, output);
        }

        /// <summary>
        /// This method tests the maximum value returned by the method GetMax.
        /// An array of string is passed as list as input
        /// </summary>
        [TestMethod]
        public void GetMaxTest()
        {
            List<string[]> inputData = new List<string[]>();

            for (int i = 0; i < 100; i++)
            {
                inputData.Add(new string[] { Convert.ToString(i), Convert.ToString(i), Convert.ToString(i), Convert.ToString(i), Convert.ToString(i) });
            }

            double result = HeartBeatMonitor.Calculator.GetMax(inputData, 1);

            Assert.AreEqual(99, result);
        }

        /// <summary>
        /// This method tests the minimum value returned by the method GetMin.
        /// An array of string is passed as list as input
        /// </summary>
        [TestMethod]
        public void GetMinTest()
        {
            List<string[]> inputData = new List<string[]>();

            for (int i = 0; i < 100; i++)
            {
                inputData.Add(new string[] { Convert.ToString(i), Convert.ToString(i), Convert.ToString(i), Convert.ToString(i), Convert.ToString(i) });
            }

            double result = HeartBeatMonitor.Calculator.GetMin(inputData, 1);

            Assert.AreEqual(0, result);
        }

        /// <summary>
        /// This method tests the output returned by the GetAverage method.
        /// </summary>
        [TestMethod]
        public void GetAverageTest()
        {
            List<string[]> inputData = new List<string[]>();

            for (int i = 0; i < 100; i++)
            {
                inputData.Add(new string[] { Convert.ToString(i), Convert.ToString(i), Convert.ToString(i), Convert.ToString(i), Convert.ToString(i) });
            }

            double result = HeartBeatMonitor.Calculator.GetAverage(inputData, 1);
            Assert.AreEqual(5.355, result);
        }

        /// <summary>
        /// This method checks the output returned by the GetSpeed method.
        /// </summary>
        [TestMethod]
        public void GetSpeedTest()
        {
            double result = HeartBeatMonitor.Calculator.GetSpeed(1, 300, 65);
            Assert.AreEqual(5.42, result);

            result = HeartBeatMonitor.Calculator.GetSpeed(2, 300, 65);
            Assert.AreEqual(10.83, result);
        }

    }
}
