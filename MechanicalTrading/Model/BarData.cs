// Copyright (c) 2011, Adam Duval Kangaroo Point South 4169, Australia.
// All Rights Reserved.
using System;
using System.Collections.Generic;

namespace Adam.Trading.Mechanical.Model
{
    /// <summary>
    /// A class to keep track of the data for an Open, High, Low, Close bar
    /// </summary>
    public class BarData
    {
        private DateTime _date;    // The date
        private decimal _open;     // The opening price
        private decimal _high;     // The high price
        private decimal _low;      // The low price
        private decimal _close;    // The closing price
        private decimal? _volume;  // The volume
        private bool _contractRollover = false;  // True if the contract rollover information exits and there is a contract rollover on this day
        private TradingEnumerations.Trend _minorTrend = TradingEnumerations.Trend.Unknown;  // The minor trend value at this datapoint
        private TradingEnumerations.Trend _majorTrend = TradingEnumerations.Trend.Unknown;  // The major trend value at this datapoint
        
        #region Accessor Methods
        /// <summary>
        /// Get the Date
        /// </summary>
        public DateTime Date
        {
            get { return _date; }
        }

        /// <summary>
        /// Get the Open Price
        /// </summary>
        public decimal Open
        {
            get { return _open; }
        }

        /// <summary>
        /// Get the High Price
        /// </summary>
        public decimal High
        {
            get { return _high; }
        }

        /// <summary>
        /// Get the Low Price
        /// </summary>
        public decimal Low
        {
            get { return _low;}
        }

        /// <summary>
        /// Get the Close High Price
        /// </summary>
        public decimal Close
        {
            get { return _close;}
        }

        /// <summary>
        /// Get the Volume
        /// </summary>
        public decimal? Volume
        {
            get { return _volume; }
        }

        public TradingEnumerations.Trend MajorTrend
        {
            get { return _majorTrend; }
            set { _majorTrend = value; }
        }

        public TradingEnumerations.Trend MinorTrend
        {
            get { return _minorTrend; }
            set { _minorTrend = value; }
        }

        /// <summary>
        /// Get Contract rollover information
        /// </summary>
        public bool ContractRollover
        {
            get { return _contractRollover; }
        }
        #endregion

        #region The Constructor
        /// <summary>
        /// A constructor that accepts volume
        /// </summary>
        /// <param name="date">The date</param>
        /// <param name="open">The opening price</param>
        /// <param name="high">The high price</param>
        /// <param name="low">The low price</param>
        /// <param name="close">The closing price</param>
        /// <param name="volume">The volume</param>
        public BarData(DateTime date, decimal open, decimal high, decimal low, decimal close, decimal volume)
        {
            _date = date;
            _open = open;
            _high = high;
            _low = low;
            _close = close;
            _volume = volume;
        }
        /// <summary>
        /// A constructor that doesn't require volume
        /// </summary>
        /// <param name="date">The date</param>
        /// <param name="open">The opening price</param>
        /// <param name="high">The high price</param>
        /// <param name="low">The low price</param>
        /// <param name="close">The closing price</param>
        public BarData(DateTime date, decimal open, decimal high, decimal low, decimal close)
        {
            _date = date;
            _open = open;
            _high = high;
            _low = low;
            _close = close;
            _volume = null;
        }

        /// <summary>
        /// A constructor that accepts volume and contract rollover and all inputs are strings apart from the contract rollover flag
        /// </summary>
        /// <param name="date">The date</param>
        /// <param name="open">The opening price</param>
        /// <param name="high">The high price</param>
        /// <param name="low">The low price</param>
        /// <param name="close">The closing price</param>
        /// <param name="volume">The volume</param>
        /// <param name="contractRollover">The contract rollover flag</param>
        public BarData(string stringDate, string stringOpen, string stringHigh, string stringLow, string stringClose, string stringVolume, bool contractRollover)
        {
            bool validDate = false;
            bool validPriceData = false;
            DateTime date;
            _contractRollover = contractRollover;
            if (DateTime.TryParse(stringDate, out date))
            {
                validDate = true;
            }
            else if (stringDate.Length >= 8)
            {
                // Dates in the format YYYYMMDD
                char[] dateArray = stringDate.ToCharArray();
                string yearString = dateArray[0].ToString() + dateArray[1].ToString() + dateArray[2].ToString() + dateArray[3].ToString();
                string monthString = dateArray[4].ToString() + dateArray[5].ToString();
                string dayString = dateArray[6].ToString() + dateArray[7].ToString();
                int year;
                int month;
                int day;
                if ((int.TryParse(yearString, out year)) && (int.TryParse(monthString, out month)) && (int.TryParse(dayString, out day))) 
                {
                    try
                    {
                        date = new DateTime(year, month, day);
                        validDate = true;
                    }
                    catch
                    {
                        validDate = false;
                    }
                }
                if (validDate)
                {
                    decimal open;
                    decimal high;
                    decimal low;
                    decimal close;
                    decimal volume;
                    if (decimal.TryParse(stringOpen, out open))
                    {
                        if (decimal.TryParse(stringHigh, out high))
                        {
                            if (decimal.TryParse(stringLow, out low))
                            {
                                if (decimal.TryParse(stringClose, out close))
                                {
                                    if (decimal.TryParse(stringVolume, out volume))
                                    {
                                        validPriceData = true;
                                        _date = date;
                                        _open = open;
                                        _high = high;
                                        _low = low;
                                        _close = close;
                                        _volume = volume;
                                    }
                                    else
                                    {
                                        validPriceData = false;
                                    }
                                }
                                else
                                {
                                    validPriceData = false;
                                }
                            }
                            else
                            {
                                validPriceData = false;
                            }
                        }
                        else
                        {
                            validPriceData = false;
                        }
                    }
                    else
                    {
                        validPriceData = false;
                    }
                    if (!validPriceData)
                    {
                        throw new ArgumentException("Invalid price data.");
                    }
                }
                else
                {
                    throw new ArgumentException("Invalid input date.");
                }
            }
            else
            {
                throw new ArgumentException("Unsupported input data format.");
            }
        }

        /// <summary>
        /// A constructor that doesn't require volume and all inputs are strings
        /// </summary>
        /// <param name="date">The date</param>
        /// <param name="open">The opening price</param>
        /// <param name="high">The high price</param>
        /// <param name="low">The low price</param>
        /// <param name="close">The closing price</param>
        public BarData(string stringDate, string stringOpen, string stringHigh, string stringLow, string stringClose)
        {
            bool validDate = false;
            bool validPriceData = false;
            DateTime date;
            if (DateTime.TryParse(stringDate, out date))
            {
                validDate = true;
            }
            else if (stringDate.Length >= 8)
            {
                // Dates in the format YYYYMMDD
                char[] dateArray = stringDate.ToCharArray();
                string yearString = dateArray[0].ToString() + dateArray[1].ToString() + dateArray[2].ToString() + dateArray[3].ToString();
                string monthString = dateArray[4].ToString() + dateArray[5].ToString();
                string dayString = dateArray[6].ToString() + dateArray[7].ToString();
                int year;
                int month;
                int day;
                if ((int.TryParse(yearString, out year)) && (int.TryParse(monthString, out month)) && (int.TryParse(dayString, out day)))
                {
                    try
                    {
                        date = new DateTime(year, month, day);
                        validDate = true;
                    }
                    catch
                    {
                        validDate = false;
                    }
                }
                if (validDate)
                {
                    decimal open;
                    decimal high;
                    decimal low;
                    decimal close;
                    if (decimal.TryParse(stringOpen, out open))
                    {
                        if (decimal.TryParse(stringHigh, out high))
                        {
                            if (decimal.TryParse(stringLow, out low))
                            {
                                if (decimal.TryParse(stringClose, out close))
                                {
                                    validPriceData = true;
                                    _date = date;
                                    _open = open;
                                    _high = high;
                                    _low = low;
                                    _close = close;
                                    _volume = null;
                                }
                                else
                                {
                                    validPriceData = false;
                                }
                            }
                            else
                            {
                                validPriceData = false;
                            }
                        }
                        else
                        {
                            validPriceData = false;
                        }
                    }
                    else
                    {
                        validPriceData = false;
                    }
                    if (!validPriceData)
                    {
                        throw new ArgumentException("Invalid price data.");
                    }
                }
                else
                {
                    throw new ArgumentException("Invalid input date.");
                }
            }
            else
            {
                throw new ArgumentException("Unsupported input data format.");
            }
        }
        #endregion
    }
}
