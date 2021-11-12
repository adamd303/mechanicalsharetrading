// Copyright (c) 2012, Adam Duval Kangaroo Point South 4169, Australia.
// All Rights Reserved.
using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Adam.Trading.Mechanical.Model.Tests
{
    [TestClass]
    public class SupportResistanceCalculatorTests
    {
        const string test_method_datafile = @"GC----C.csv";
        
        [DeploymentItem(test_method_datafile), TestMethod]
        public void Seykota140_20Test()
        {
            var deploymentFolder = System.IO.Path.GetDirectoryName(this.GetType().Assembly.Location);
            var deployedTestData = System.IO.Path.Combine(deploymentFolder, test_method_datafile);

            SupportResistanceCalculator supportResistanceCalculator = new SupportResistanceCalculator(140, 20, deployedTestData, 1000000.0M, 0.05M, 0.5M, 0.07M, false);
            supportResistanceCalculator.CalculateTradingSystem();
            bool result = true;
            if (Math.Abs(supportResistanceCalculator.EndingEquity - 2585500.0M) > 1.0M)
            {
                result = false;
            }
            if (Math.Abs(supportResistanceCalculator.ICAGR - 0.03090) > 0.00001)
            {
                result = false;
            }
            if (Math.Abs(supportResistanceCalculator.DD - 40.77) > 0.01)
            {
                result = false;
            }
            
            Assert.AreEqual(result, true);
        }

        [DeploymentItem(test_method_datafile), TestMethod]
        public void Seykota120_45Test()
        {
            var deploymentFolder = System.IO.Path.GetDirectoryName(this.GetType().Assembly.Location);
            var deployedTestData = System.IO.Path.Combine(deploymentFolder, test_method_datafile);

            SupportResistanceCalculator supportResistanceCalculator = new SupportResistanceCalculator(120, 45, deployedTestData, 1000000.0M, 0.05M, 0.5M, 0.07M, false);
            supportResistanceCalculator.CalculateTradingSystem();
            bool result = true;
            if (Math.Abs(supportResistanceCalculator.EndingEquity - 2333280.0M) > 1.0M)
            {
                result = false;
            }
            // Don't have an ICAGR for this one. It is based on Equity ratio and years.
            // So already tested above anyway as years are the same, just different
            // equity ratios for the two tests
            if (Math.Abs(supportResistanceCalculator.DD - 36.89) > 0.01)
            {
                result = false;
            }

            Assert.AreEqual(result, true);
        }
    }
}
