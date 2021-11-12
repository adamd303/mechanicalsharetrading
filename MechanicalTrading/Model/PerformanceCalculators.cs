// Copyright (c) 2012, Adam Duval Kangaroo Point South 4169, Australia.
// All Rights Reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam.Trading.Mechanical.Model
{
    public static class PerformanceCalculators
    {
        /// <summary>
        /// A function to calulate the Instantaneously Compounding Annual Growth Rate (ICAGR),
        /// given an input Equity List
        /// </summary>
        /// <param name="equityList">The Input Equity List</param>
        /// <returns>The ICAGR as a double.</returns>
        public static double CalculateICAGR(List<Equity> equityList)
        {
            if (equityList.Count >= 2)
            {
                if (equityList[0].CurrentEquity > 0.0000000001M)
                {
                    double Ratio = double.Parse((equityList[equityList.Count - 1].CurrentEquity).ToString()) /
                                   double.Parse((equityList[0].CurrentEquity).ToString());
                    DateTime startTime = equityList[0].Date;
                    DateTime endTime = equityList[equityList.Count - 1].Date;
                    TimeSpan span = endTime.Subtract(startTime);
                    double DateRangeInYears = span.TotalDays / 365.25;
                    double icagr = Math.Log(double.Parse(Ratio.ToString())) / DateRangeInYears;
                    return icagr;
                }
                else
                {
                    // May not be the correct value to return. Investigate.
                    return 0.0;
                }
            }
            else
            {
                // May not be the correct value to return. Investigate.
                return 0.0;
            }
        }

        /// <summary>
        /// A function to calculate the maximum percentage Draw Down and the date it occurred on
        /// </summary>
        /// <param name="equityList">The input Equity List.</param>
        /// <param name="maxDD">The output max. Draw Down.</param>
        /// <param name="maxDDDate">The output date the max. Draw Down occured on.</param>
        public static void CalculateMaxDD(List<Equity> equityList, out double maxDD, out DateTime maxDDDate)
        {
            maxDD = 0.0;
            maxDDDate = equityList[0].Date;
            if (equityList.Count >= 3)
            {
                double currMax = double.Parse(equityList[0].CurrentEquity.ToString());
                for (int counter = 1; counter < equityList.Count; counter++)
                {
                    double currPoint = double.Parse(equityList[counter].CurrentEquity.ToString());
                    if ((currPoint < currMax) && (currPoint > 0.0))
                    {
                        double percDD = Math.Abs(currPoint - currMax) / currMax * 100.0;
                        if (percDD > maxDD)
                        {
                            maxDD = percDD;
                            maxDDDate = equityList[counter].Date;
                        }
                    }
                    if (currPoint > currMax)
                    {
                        currMax = currPoint;
                    }
                }
            }
        }

        /// <summary>
        /// A function to calulate the Projected Equity, 
        /// given a Benchmark Return, an Initial Equity and an Equity List
        /// </summary>
        /// <param name="benchMarkReturn">The Benchmark Return</param>
        /// <param name="initialEquity">The Starting (Original) Equity</param>
        /// <param name="equityList">The Input Equity List</param>
        /// <returns>The Projected Equity as a double.</returns>
        public static double CalculateProjectedEquity(decimal benchMarkReturn, decimal initialEquity, List<Equity> equityList)
        {
            if (equityList.Count >= 2)
            {
                DateTime startTime = equityList[0].Date;
                DateTime endTime =  equityList[equityList.Count - 1].Date;
                TimeSpan span = endTime.Subtract(startTime);
                double DateRangeInYears = span.TotalDays / 365.25;
                double projectedEquity = double.Parse(initialEquity.ToString()) * Math.Exp(DateRangeInYears * double.Parse(benchMarkReturn.ToString()));
                return projectedEquity;
            }
            else
            {
                return double.Parse(initialEquity.ToString());
            }
        }
    }
}
