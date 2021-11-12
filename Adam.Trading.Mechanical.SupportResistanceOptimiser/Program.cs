// Copyright (c) 2012, Adam Duval Kangaroo Point South 4169, Australia.
// All Rights Reserved.
using System;
using System.Collections.Generic;
using System.Text;
using Adam.Trading.Mechanical.Model;

namespace Adam.Trading.Mechanical.SupportResistanceOptimiser
{
    /// <summary>
    /// A program to calculate the trading system for various combinations 
    /// of long and short days
    /// </summary>
    public class Program
    {
        public static void Main(string[] args)
        {
            List<OutputData> data = new List<OutputData>();  // The list of output data for a particular value of Long Term Support/Resistance Days
            // Long Term Support/Resistance Days Loop
            for (int i=20; i <= 420; i+=20)
            {
                // Short Term Support/Resistance Days Loop
                for (int j=5; j <= 105; j+=5)
                {
                    try
                    {
                        SupportResistanceCalculator supportResistanceCalculator =
                            new SupportResistanceCalculator(i,
                                                            j,
                                                            ".\\Data\\GC----C.csv",
                                                            1000000.0M,
                                                            0.05M,
                                                            0.5M,
                                                            0.0499M,
                                                            false);
                        supportResistanceCalculator.CalculateTradingSystem();
                        data.Add(new OutputData(supportResistanceCalculator.LongDays,
                                                supportResistanceCalculator.ShortDays,
                                                supportResistanceCalculator.ICAGR,
                                                supportResistanceCalculator.DD,
                                                supportResistanceCalculator.Bliss,
                                                supportResistanceCalculator.SharpeRatio));
                    }
                    catch(Exception ex)
                    {
                        System.Console.WriteLine("Short Term Days:," + j.ToString());
                        System.Console.WriteLine("An error occured:," + ex.ToString());
                    }
                }
            }
            if (data.Count > 0)
            {
                System.Console.WriteLine("Long Term Days, Short Term Days, ICAGR, Max. Draw Down, Bliss,  Sharpe Ratio");
                // Output the data for the LongDays = i in a row, separated by commas
                foreach (OutputData dataRow in data)
                {
                    System.Console.WriteLine(dataRow.LongTermDays.ToString() + "," +
                                             dataRow.ShortTermDays.ToString() + "," +
                                             dataRow.ICAGR.ToString() + "," +
                                             dataRow.DD.ToString() + "," +
                                             dataRow.Bliss.ToString() + "," +
                                             dataRow.SharpeRatio.ToString());
                }
            }
            else
            {
                System.Console.WriteLine("No data calculated!");
            }
        }
    }
}

