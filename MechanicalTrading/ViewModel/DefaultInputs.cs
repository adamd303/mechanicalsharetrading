// Copyright (c) 2012, Adam Duval Kangaroo Point South 4169, Australia.
// All Rights Reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam.Trading.Mechanical.ViewModel
{
    /// <summary>
    /// A Class to contain all the default inputs
    /// </summary>
    internal static class DefaultInputs
    {
        #region Support Resistance
        /// <summary>
        /// The default for Long Term Support/Resistance Days
        /// </summary>
        public const int SupportResistanceLONGTERMDAYSDEFAULT = 140;

        /// <summary>
        /// The default for Short Term Support/Resistance Days
        /// </summary>
        public const int SupportResistanceSHORTTERMDAYSDEFAULT = 20;

        /// <summary>
        /// The message for the user to select an input file
        /// </summary>
        public const string SupportResistanceINPUTFILEMESSAGE = "Please select a .csv file";

        /// <summary>
        /// The default for the Starting Equity
        /// </summary>
        public const decimal SupportResistanceSTARTINGEQUITYDEFAULT = 1000000.0M;

        /// <summary>
        /// The default for the Percent of Capital to risk for each trade
        /// Actually a fraction rather than a percent
        /// </summary>
        public const decimal SupportResistancePERCENTRISKDEFAULT = 0.05M;

        /// <summary>
        /// The default for the Skid Fraction to determine entry/exit prices
        /// </summary>
        public const decimal SupportResistanceSKIDFRACTIONDEFAULT = 0.5M;

        /// <summary>
        /// The default for the Benchmark return for Sharpe Ratio calculation
        /// </summary>
        public const decimal SupportResistanceBENCHMARKDEFAULT = 0.07M;

        /// <summary>
        /// The number of initial Calendar days to display on the Chart
        /// </summary>
        public const int SupportResistanceCALENDARDAYSTODISPLAY = 30;
        #endregion

        #region Krause Gann
        /// <summary>
        /// The entry stop for Retracement for the Krause Basic System
        /// </summary>
        public const decimal KrauseENTRYSTOPRETRACEMENT = 0.0625M;

        /// <summary>
        /// The entry stop for Exceed Hi Lo for the Krause Basic System
        /// </summary>
        public const decimal KrauseENTRYSTOPEXCEEDHILO = 0.00M;

        /// <summary>
        /// The retracement fraction for the Krause Basic System
        /// </summary>
        public const decimal KrauseRETRACEMENTFRACTION = 0.382M;

        /// <summary>
        /// The message for the user to select an input file
        /// </summary>
        public const string KrauseINPUTFILEMESSAGE = "Please select a .csv file";

        /// <summary>
        /// The default for the Dollars Per Point of Price Movement
        /// </summary>
        public const decimal KrauseDOLLARSPERPOINT = 31.25M;

        /// <summary>
        /// The default for the Skid Points for Buy/Sell orders
        /// </summary>
        public const decimal KrauseSKIDPOINTS = 0.00M;

        /// <summary>
        /// The default for the Starting Equity
        /// </summary>
        public const decimal KrauseSTARTINGEQUITYDEFAULT = 30000.0M;

        /// <summary>
        /// The number of contracts per $10000 of equity for the Krause Gann system
        /// </summary>
        public const decimal KrauseNOOFCONTRACTS = 1.000M;

        /// <summary>
        /// The tick size for the contract being traded
        /// </summary>
        public const decimal KrauseTICKSIZE = 0.03125M;

        /// <summary>
        /// The default for the Benchmark return for Sharpe Ratio calculation
        /// </summary>
        public const decimal KrauseBENCHMARKDEFAULT = 0.07M;

        /// <summary>
        /// The number of initial Calendar days to display on the Chart
        /// </summary>
        public const int KrauseCALENDARDAYSTODISPLAY = 180;
        #endregion

        #region Gann 2 Day Swing Charts
        /// <summary>
        /// The entry stop for Retracement for the Gann 2 Day Swing chart
        /// </summary>
        public const decimal Gann2DayENTRYSTOPRETRACEMENT = 0.25M;

        /// <summary>
        /// The entry stop for Exceed Hi Lo for the Gann 2 Day Swing chart
        /// </summary>
        public const decimal Gann2DayENTRYSTOPEXCEEDHILO = 1.25M;

        /// <summary>
        /// The exit stop for the Gann 2 Day Swing chart
        /// </summary>
        public const decimal Gann2DayEXITSTOP = 5.0M;

        /// <summary>
        /// The message for the user to select an input file
        /// </summary>
        public const string Gann2DayINPUTFILEMESSAGE = "Please select a .csv file";

        /// <summary>
        /// The default for the Dollars Per Point of Price Movement
        /// </summary>
        public const decimal Gann2DayDOLLARSPERPOINT = 1.0M;

        /// <summary>
        /// The default for the Skid Points for Buy/Sell orders
        /// </summary>
        public const decimal Gann2DaySKIDPOINTS = 0.25M;

        /// <summary>
        /// The default for the Starting Equity
        /// </summary>
        public const decimal Gann2DaySTARTINGEQUITYDEFAULT = 1000000.0M;

        /// <summary>
        /// The default for the Percent of Capital to risk for each trade
        /// Actually a fraction rather than a percent
        /// </summary>
        public const decimal Gann2DayPERCENTRISKDEFAULT = 0.05M;

        /// <summary>
        /// The default for the Benchmark return for Sharpe Ratio calculation
        /// </summary>
        public const decimal Gann2DayBENCHMARKDEFAULT = 0.07M;

        /// <summary>
        /// The number of initial Calendar days to display on the Chart
        /// </summary>
        public const int Gann2DayCALENDARDAYSTODISPLAY = 180;
        #endregion
    }
}
