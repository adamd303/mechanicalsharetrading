// Copyright (c) 2012, Adam Duval Kangaroo Point South 4169, Australia.
// All Rights Reserved.
using System;
using System.Text;

namespace Adam.Trading.Mechanical.Model
{
    /// <summary>
    /// A class to keep track of the output data for a
    /// system optimisation
    /// </summary>
    public class OutputData
    {
        #region Accessor Methods
        /// <summary>
        /// Get the Long Term Support/Resistance Days
        /// </summary>
        public int LongTermDays
        {
            get;
            set;
        }
        /// <summary>
        /// Get the Short Term Support/Resistance Days
        /// </summary>
        public int ShortTermDays
        {
            get;
            set;
        }
        /// <summary>
        /// Get the Instantaneously Compounding Annualised Growth Rate (ICAGR)
        /// </summary>
        public double ICAGR
        {
            get;
            set;
        }
        /// <summary>
        /// Get the Maximum Draw Down
        /// </summary>
        public double DD
        {
            get;
            set;
        }
        /// <summary>
        /// Get the Bliss
        /// </summary>
        public double Bliss
        {
            get;
            set;
        }
        /// <summary>
        /// Get the Sharpe Ratio
        /// </summary>
        public double SharpeRatio
        {
            get;
            set;
        }
        #endregion

        public OutputData(int longTermDays, int shortTermDays, double icagr, double dd, double bliss, double sharpeRatio)
        {
            LongTermDays = longTermDays;
            ShortTermDays = shortTermDays;
            ICAGR = icagr;
            DD = dd;
            Bliss = bliss;
            SharpeRatio = sharpeRatio;
        }
    }
}

