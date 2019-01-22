using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
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

        [TestMethod]
        public void LimitTo2Test()
        {
            double output = HeartBeatMonitor.PowerCalc.LimitTo2(1.546);

            Assert.AreEqual(1.55, output);
        }

        [TestMethod]
        public void GetFourthRootTest()
        {
            double output = HeartBeatMonitor.PowerCalc.GetFourthRoot(81);

            Assert.AreEqual(3, output);
        }

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

    }
}
