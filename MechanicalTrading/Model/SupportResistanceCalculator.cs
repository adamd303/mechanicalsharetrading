// Copyright (c) 2011, Adam Duval Kangaroo Point South 4169, Australia.
// All Rights Reserved.
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Adam.Trading.Mechanical.Model
{
    public class SupportResistanceCalculator
    {
        #region Class private variables
        private const int UP = 1;         // An uptrend based on the slow metric
        private const int DOWN = -1;      // A downtrend based on the slow metric
        private const int UNKNOWN = 0;    // Uncertain trend
        private const int LONG = 1;       // A long trade is in progress
        private const int SHORT = -1;     // A short trade is in progress
        private const int NOTRADE = 0;    // No trade in progress
        private decimal _profitLoss;
        private decimal _closingBalance;
        private decimal _openProfit;
        private decimal _currentEquity;
        private List<Equity> _currEquityList;
        private decimal _initialEquity;     // The initial (starting) equity
        private double _projectedEquity;    // The projected Equity
        private double _icagr;              // The current icagr
        private double _sigmaX;             // The sum of the _icagr
        private double _sigmaXSq;           // The sum of the _icagr * _icagr
        private int _count;                 // The count of the _icagr
        private decimal _equity;
        private decimal _percentRisk;
        private decimal _skidFraction;
        private decimal _benchmarkReturn; 
        private List<decimal> _longSupport = new List<decimal>();            // The list of low prices for the last LongDays
        private List<decimal> _shortSupport = new List<decimal>();           // The list of low prces for the last ShortDays
        private List<decimal> _longResistance = new List<decimal>();         // The list of high prices for the last LongDays
        private List<decimal> _shortResistance = new List<decimal>();        // The list of high prices for the last ShortDays    	
        private decimal _minLongSupport;                                     // The minimum of the low prices for the last LongDays
        private decimal _minShortSupport;                                    // The minimum of the low prices for the last ShortDays
        private decimal _maxLongResistance;                                  // The maximum of the high prices for the last LongDays
        private decimal _maxShortResistance;                                 // The maximum of the high prices for the last ShortDays
        private int _trend = UNKNOWN;                                        // The trend based on the slow metric 
        private StringBuilder _tradeLogMessage;                              // The message to write to the trade log file
        private StreamWriter _tradeLog;                                      // The trade log file
        private StreamWriter _equityLog;                                     // The equity log file
        private StreamWriter _metricsLog;                                    // The metrics log file
        private List<LineData> _longSupportList = new List<LineData>();      // The list of long support values for each day
        private List<LineData> _shortSupportList = new List<LineData>();     // The list of short support values for each day
        private List<LineData> _longResistanceList = new List<LineData>();   // The list of long resistance values for each day
        private List<LineData> _shortResistanceList = new List<LineData>();  // The list of short resistance values for each day
        private bool _writeLogFile = true;                                   // Write to a log file or not
        private List<BarData> _ohlc = new List<BarData>();                   // The Open, High, Low, Close data from the input file
        private const char delimiterChar = ',';                              // The delimeter char to write csv files only at the moment
        #endregion

        #region Accessor Methods
        /// <summary>
        /// The Long Term Support/Resistance Days
        /// </summary>
        public int LongDays { get; private set; }

        /// <summary>
        /// The Short Term Support/Resistance Days
        /// </summary>
        public int ShortDays { get; private set; }

        /// <summary>
        /// The Data File name
        /// </summary>
        public string DataFile { get; private set; }

        /// <summary>
        /// Instantaneously Compounding Annual Growth Rate
        /// </summary>
        public double ICAGR { get; private set; }

        /// <summary>
        /// The Bliss
        /// </summary>
        public double Bliss { get; private set; }

        /// <summary>
        /// The maximum percent drawn down
        /// </summary>
        public double DD { get; private set; }

        /// <summary>
        /// The Sharpe Ratio
        /// </summary>
        public double SharpeRatio { get; private set; }

        /// <summary>
        /// The Ending Equity
        /// </summary>
        public decimal EndingEquity { get; private set; }

        public List<BarData> OHLC
        {
            get { return _ohlc;  }
        }

        public List<LineData> LongSupportList
        {
            get { return _longSupportList; }
        }

        public List<LineData> ShortSupportList
        {
            get { return _shortSupportList; }
        }

        public List<LineData> LongResistanceList
        {
            get { return _longResistanceList; }
        }

        public List<LineData> ShortResistanceList
        {
            get { return _shortResistanceList; }
        }
        #endregion

        #region The Contructor
        /// <summary>
        /// The constructor
        /// </summary>
        /// <param name="longTermDays">The Long Term Support/Resistance Days.</param>
        /// <param name="shortTermDays">The Short Term Support/Resistance Days.</param>
        /// <param name="dataFile">The Data File name.</param>
        /// <param name="startingEquity">The Starting Equity.</param>
        /// <param name="percentRisk">The Percent risked per trade.</param>
        /// <param name="skidFraction">The Skid Fraction to determine entry and exit prices</param>
        /// <param name="benchmarkReturn">The Benchmark return to calculate Sharpe ratio</param>
        /// <param name="writeLogFile">True to output to the metrics log.</param>
        public SupportResistanceCalculator(int longTermDays,
                                           int shortTermDays,
                                           string dataFile,
                                           decimal startingEquity,
                                           decimal percentRisk,
                                           decimal skidFraction,
                                           decimal benchmarkReturn,
                                           bool writeLogFile)
        {
            LongDays = longTermDays;
            ShortDays = shortTermDays;
            DataFile = dataFile;
            _profitLoss = 0.0M;
            _closingBalance = startingEquity;
            _openProfit = 0.0M;
            _equity = startingEquity;
            _percentRisk = percentRisk;
            _skidFraction = skidFraction;
            _benchmarkReturn = benchmarkReturn;
            _currentEquity = 0.0M;
            _currEquityList = new List<Equity>();
            _initialEquity = startingEquity;
            _projectedEquity = double.Parse(startingEquity.ToString());
            _count = 0;
            _sigmaX = 0.0;
            _sigmaXSq = 0.0;
            _writeLogFile = writeLogFile;
            _ohlc = new List<BarData>();
        #endregion
        }

        #region
        public void CalculateTradingSystem()
        {       
            if (_writeLogFile)
            {
                // Open the trade log, metrics log and equity log.
                _tradeLog = File.CreateText("TradeLog.csv");
                _equityLog = File.CreateText("EquityLog.csv");
                _metricsLog = File.CreateText("MetricsLog.csv");
                // Initialsise string to write to the trade log
                _tradeLogMessage = new StringBuilder();
            }
            // Read the input data from the input file
            try
            {
                BarDataReader barDataReader = new BarDataReader(DataFile);
                if (barDataReader.OHLC.Count > 0)
                {
                    // Initialise the data
                    DateTime date = barDataReader.OHLC[0].Date;
                    decimal open = barDataReader.OHLC[0].Open;
                    decimal high = barDataReader.OHLC[0].High;
                    decimal low = barDataReader.OHLC[0].Low;
                    decimal close = barDataReader.OHLC[0].Close;
                    _ohlc.Add(barDataReader.OHLC[0]);
                    if (_writeLogFile)
                    {
                        // Write the initial values to the Metrics Log
                        _metricsLog.WriteLine("[Metrics] " +
                                              date +
                                              " OHLC:[" +
                                              open.ToString() + " " +
                                              high.ToString() + " " +
                                              low.ToString() + " " +
                                              close.ToString() +
                                              "] [Slow:" +
                                              high.ToString() +
                                              "/" +
                                              low.ToString() +
                                              " Fast:" +
                                              high.ToString() +
                                              "/" +
                                              low.ToString() +
                                              " T: " +
                                              _trend.ToString() +
                                              "]");
                        // Write headers for the equity log
                        _equityLog.WriteLine("Date" + delimiterChar +
                                             "Closing Balance" + delimiterChar +
                                             "Open Profit" + delimiterChar +
                                             "Current Equity" + delimiterChar +
                                             "Projected Equity Curve" + delimiterChar +
                                             "ICAGR");
                        // Write headers to the trade log
                        _tradeLog.WriteLine("Trade Entry Date" + delimiterChar +
                                            "Long/Short" + delimiterChar +
                                            "Position Size" + delimiterChar +
                                            "Open Trade Price" + delimiterChar +
                                            "Trade Exit Date" + delimiterChar +
                                            "Close Trade Price" + delimiterChar +
                                            "Profit/Loss");
                    }
                    // Initialise values
                    // A flag if we are in the trade
                    int trade = NOTRADE;
                    // The position size
                    decimal positionSize = 0.0M;
                    // The awarded trade price at start and finish of trade
                    decimal openTradePrice = 0.0M;
                    decimal closeTradePrice = 0.0M;
                    // Calculate the true range
                    // which is the maximum of 
                    // High-Low, High-Yesterday Close, Yesterday Close-Low
                    decimal trueRange = high - low;
                    // Initialise the lists
                    _longSupport.Clear();
                    _longSupport.Add(low);
                    _longSupportList.Clear();
                    _longSupportList.Add(new LineData(barDataReader.OHLC[0].Date, low));
                    _shortSupport.Clear();
                    _shortSupport.Add(low);
                    _shortSupportList.Clear();
                    _shortSupportList.Add(new LineData(barDataReader.OHLC[0].Date, low));
                    _longResistance.Clear();
                    _longResistance.Add(high);
                    _longResistanceList.Clear();
                    _longResistanceList.Add(new LineData(barDataReader.OHLC[0].Date, high));
                    _shortResistance.Clear();
                    _shortResistance.Add(high);
                    _shortResistanceList.Clear();
                    _shortResistanceList.Add(new LineData(barDataReader.OHLC[0].Date, high));
                    // Declare and initialise the relevant data for the next trading day
                    DateTime nextDate = new DateTime();
                    decimal nextOpen = 0.0M;
                    decimal nextHigh = 0.0M;
                    decimal nextLow = 0.0M;
                    decimal nextClose = 0.0M;
                    // Read the rest of the data 
                    if (barDataReader.OHLC.Count >= 1)
                    {
                        for (int i = 1; i < barDataReader.OHLC.Count; i++)
                        {
                            // Write the data to the equity log
                            _currentEquity = _closingBalance + _openProfit;
                            _currEquityList.Add(new Equity(date, _currentEquity));
                            // Projected Equity
                            _projectedEquity = PerformanceCalculators.CalculateProjectedEquity(_benchmarkReturn, _initialEquity, _currEquityList);
                            // Instantaneously Compounding Annual Growth Rate
                            _icagr = PerformanceCalculators.CalculateICAGR(_currEquityList);
                            // Update the values to calculate the standard deviation of the fund return
                            _count++;
                            _sigmaX += _icagr;
                            _sigmaXSq += _icagr * _icagr;
                            if (_writeLogFile)
                            {
                                _equityLog.WriteLine(date.ToShortDateString() + delimiterChar +
                                                     _closingBalance.ToString() + delimiterChar +
                                                     _openProfit.ToString() + delimiterChar +
                                                     _currentEquity.ToString() + delimiterChar +
                                                     _projectedEquity.ToString() + delimiterChar +
                                                     _icagr.ToString());
                            }
                            // Find the support and resistance
                            _minLongSupport = FindMin(_longSupport);
                            _minShortSupport = FindMin(_shortSupport);
                            _maxLongResistance = FindMax(_longResistance);
                            _maxShortResistance = FindMax(_shortResistance);
                            _longSupportList.Add(new LineData(barDataReader.OHLC[i].Date, _minLongSupport));
                            _shortSupportList.Add(new LineData(barDataReader.OHLC[i].Date, _minShortSupport));
                            _longResistanceList.Add(new LineData(barDataReader.OHLC[i].Date, _maxLongResistance));
                            _shortResistanceList.Add(new LineData(barDataReader.OHLC[i].Date, _maxShortResistance));

                            // Get the next buffer data
                            nextDate = barDataReader.OHLC[i].Date;
                            nextOpen = barDataReader.OHLC[i].Open;
                            nextHigh = barDataReader.OHLC[i].High;
                            nextLow = barDataReader.OHLC[i].Low;
                            nextClose = barDataReader.OHLC[i].Close;
                            _ohlc.Add(barDataReader.OHLC[i]);

                            // Check if we are in a trade or not
                            if (trade == LONG)
                            {
                                if (nextLow < _minShortSupport)
                                {
                                    if (nextOpen < _minShortSupport)
                                    {
                                        // The market opens at a gap below the stop
                                        // So we award a trade based on 50% of the difference 
                                        // between the open and the low
                                        closeTradePrice = nextOpen - (nextOpen - nextLow) * _skidFraction;
                                    }
                                    else
                                    {
                                        // The system awards trades with 50% skid. 
                                        // It executes sell orders at a price half-way 
                                        // between the best price and the low of the day.
                                        // The best price is the minimum of the open, 
                                        // the stop price and the high of the day.                                	
                                        decimal stopPrice = _minShortSupport;
                                        decimal bestPrice = Math.Min(nextOpen, nextHigh);
                                        bestPrice = Math.Min(bestPrice, stopPrice);
                                        closeTradePrice = (nextLow - bestPrice) * _skidFraction + bestPrice;
                                    }
                                    _profitLoss += (closeTradePrice - openTradePrice) * positionSize;
                                    _equity += (closeTradePrice - openTradePrice) * positionSize;
                                    _closingBalance = _equity;
                                    _openProfit = 0.0M;
                                    if (_writeLogFile)
                                    {
                                        _tradeLogMessage.Append(nextDate);
                                        _tradeLogMessage.Append(delimiterChar);
                                        _tradeLogMessage.Append(closeTradePrice.ToString());
                                        _tradeLogMessage.Append(delimiterChar);
                                        _tradeLogMessage.Append(((closeTradePrice - openTradePrice) * positionSize).ToString());
                                        _tradeLog.WriteLine(_tradeLogMessage);
                                        // Re-set the message
                                        _tradeLogMessage = new StringBuilder();
                                    }
                                    trade = NOTRADE;
                                    // Update the current equity
                                    _currentEquity = _closingBalance + _openProfit;
                                }
                                else
                                {
                                    // Update the current equity
                                    _openProfit = (nextClose - openTradePrice) * positionSize;
                                    _currentEquity = _closingBalance + _openProfit;
                                }
                            }
                            else if (trade == SHORT)
                            {
                                if (nextHigh > _maxShortResistance)
                                {
                                    if (nextOpen >= _maxShortResistance)
                                    {
                                        // The market opens at a gap above the stop
                                        // So we award a trade based on 50% of the difference 
                                        // between the open and the high
                                        closeTradePrice = (nextHigh - nextOpen) * _skidFraction + nextOpen;
                                    }
                                    else
                                    {
                                        // The system awards trades with 50% skid. 
                                        // That is, it executes buy orders at a price half-way 
                                        // between best price and the high of the day.  
                                        // The best price is the highest of the open, 
                                        // the stop price or the low of the day.
                                        decimal stopPrice = _maxShortResistance;
                                        decimal bestPrice = Math.Max(nextOpen, nextLow);
                                        bestPrice = Math.Max(bestPrice, stopPrice);
                                        closeTradePrice = (nextHigh - bestPrice) * _skidFraction + bestPrice;
                                    }
                                    _profitLoss += (openTradePrice - closeTradePrice) * positionSize;
                                    _equity += (openTradePrice - closeTradePrice) * positionSize;
                                    _closingBalance = _equity;
                                    _openProfit = 0.0M;
                                    if (_writeLogFile)
                                    {
                                        _tradeLogMessage.Append(nextDate);
                                        _tradeLogMessage.Append(delimiterChar);
                                        _tradeLogMessage.Append(closeTradePrice.ToString());
                                        _tradeLogMessage.Append(delimiterChar);
                                        _tradeLogMessage.Append(((openTradePrice - closeTradePrice) * positionSize).ToString());
                                        _tradeLog.WriteLine(_tradeLogMessage);
                                        // Re-set the message
                                        _tradeLogMessage = new StringBuilder();
                                    }
                                    trade = NOTRADE;
                                    // Update the current equity
                                    _currentEquity = _closingBalance + _openProfit;
                                }
                                else
                                {
                                    // Update the current equity
                                    _openProfit = (openTradePrice - nextClose) * positionSize;
                                    _currentEquity = _closingBalance + _openProfit;
                                }
                            }
                            else
                            {
                                // No trade is in progress. So check if one can be started
                                if (_trend == UP)
                                {
                                    if ((nextHigh > _maxShortResistance) && (_shortResistance.Count == ShortDays))
                                    {
                                        // A trade has been entered on the long side
                                        decimal riskPerLot = _maxShortResistance - _minShortSupport;
                                        positionSize = _closingBalance * _percentRisk / riskPerLot;
                                        // Round to nearest 100 Shares
                                        int intPositionSize = System.Convert.ToInt32((positionSize + 0.5M) / 100) * 100;
                                        positionSize = System.Convert.ToDecimal(intPositionSize);
                                        if (nextOpen >= _maxShortResistance)
                                        {
                                            // The market opens at a gap above the stop
                                            // So we award a trade based on 50% of the difference 
                                            // between the open and the high
                                            openTradePrice = (nextHigh - nextOpen) * _skidFraction + nextOpen;
                                        }
                                        else
                                        {
                                            // The system awards trades with 50% skid. 
                                            // That is, it executes buy orders at a price half-way 
                                            // between best price and the high of the day.  
                                            // The best price is the highest of the open, 
                                            // the stop price or the low of the day.
                                            decimal stopPrice = _maxShortResistance;
                                            decimal bestPrice = Math.Max(nextOpen, nextLow);
                                            bestPrice = Math.Max(bestPrice, stopPrice);
                                            openTradePrice = (nextHigh - bestPrice) * _skidFraction + bestPrice;
                                        }
                                        if (_writeLogFile)
                                        {
                                            // Prepare a string to write to the trade log
                                            _tradeLogMessage.Append(nextDate);
                                            _tradeLogMessage.Append(delimiterChar);
                                            _tradeLogMessage.Append("Long");
                                            _tradeLogMessage.Append(delimiterChar);
                                            _tradeLogMessage.Append(positionSize.ToString());
                                            _tradeLogMessage.Append(delimiterChar);
                                            _tradeLogMessage.Append(openTradePrice.ToString());
                                            _tradeLogMessage.Append(delimiterChar);
                                        }
                                        trade = LONG;
                                        _openProfit = (nextClose - openTradePrice) * positionSize;
                                        // Update the current equity
                                        _currentEquity = _closingBalance + _openProfit;
                                    }
                                    else
                                    {
                                        // Update the current equity
                                        _openProfit = 0.0M;
                                        _currentEquity = _closingBalance + _openProfit;
                                    }
                                }
                                else if (_trend == DOWN)
                                {
                                    if ((nextLow < _minShortSupport) && (_shortSupport.Count == ShortDays))
                                    {
                                        // A trade has been entered on the short side
                                        decimal riskPerLot = _maxShortResistance - _minShortSupport;
                                        positionSize = _closingBalance * _percentRisk / riskPerLot;
                                        // Round to nearest 100 Shares
                                        int intPositionSize = System.Convert.ToInt32((positionSize + 0.5M) / 100) * 100;
                                        positionSize = System.Convert.ToDecimal(intPositionSize);
                                        if (nextOpen < _minShortSupport)
                                        {
                                            // The market opens at a gap below the stop
                                            // So we award a trade based on 50% of the difference 
                                            // between the open and the low
                                            openTradePrice = nextOpen - (nextOpen - nextLow) * _skidFraction;
                                        }
                                        else
                                        {
                                            // The system awards trades with 50% skid. 
                                            // It executes sell orders at a price half-way 
                                            // between the best price and the low of the day.
                                            // The best price is the minimum of the open, 
                                            // the stop price and the high of the day.                                	
                                            decimal stopPrice = _minShortSupport;
                                            decimal bestPrice = Math.Min(nextOpen, nextHigh);
                                            bestPrice = Math.Min(bestPrice, stopPrice);
                                            openTradePrice = (nextLow - bestPrice) * _skidFraction + bestPrice;
                                        }
                                        if (_writeLogFile)
                                        {
                                            // Prepare a string to write to the trade log
                                            _tradeLogMessage.Append(nextDate);
                                            _tradeLogMessage.Append(delimiterChar);
                                            _tradeLogMessage.Append("Short");
                                            _tradeLogMessage.Append(delimiterChar);
                                            _tradeLogMessage.Append(positionSize.ToString());
                                            _tradeLogMessage.Append(delimiterChar);
                                            _tradeLogMessage.Append(openTradePrice.ToString());
                                            _tradeLogMessage.Append(delimiterChar);
                                        }
                                        trade = SHORT;
                                        _openProfit = (openTradePrice - nextClose) * positionSize;
                                        // Update the current equity
                                        _currentEquity = _closingBalance + _openProfit;
                                    }
                                    else
                                    {
                                        // Update the current equity
                                        _openProfit = 0.0M;
                                        _currentEquity = _closingBalance + _openProfit;
                                    }
                                }
                                else
                                {
                                    // Uncertain trend so no trade is possible
                                    // This should only happen on the first pass
                                    // Update the current equity
                                    _openProfit = 0.0M;
                                    _currentEquity = _closingBalance + _openProfit;
                                }
                            }
                            // Recalculate the trend based on the long metric
                            if (_trend == UP)
                            {
                                if (nextLow < _minLongSupport)
                                {
                                    _trend = DOWN;
                                }
                            }
                            else if (_trend == DOWN)
                            {
                                if (nextHigh > _maxLongResistance)
                                {
                                    _trend = UP;
                                }
                            }
                            else if (_trend == UNKNOWN)
                            {
                                if (nextHigh > _maxLongResistance)
                                {
                                    _trend = UP;
                                }
                                else if (nextLow < _minLongSupport)
                                {
                                    _trend = DOWN;
                                }
                            }
                            // Update the values for the next pass through
                            date = nextDate;
                            open = nextOpen;
                            high = nextHigh;
                            low = nextLow;
                            close = nextClose;
                            // Update the lists
                            if (_longSupport.Count >= LongDays)
                            {
                                _longSupport.RemoveAt(0);
                            }
                            _longSupport.Add(low);
                            if (_shortSupport.Count >= ShortDays)
                            {
                                _shortSupport.RemoveAt(0);
                            }
                            _shortSupport.Add(low);
                            if (_longResistance.Count >= LongDays)
                            {
                                _longResistance.RemoveAt(0);
                            }
                            _longResistance.Add(high);
                            if (_shortResistance.Count >= ShortDays)
                            {
                                _shortResistance.RemoveAt(0);
                            }
                            _shortResistance.Add(high);
                            // Write relevant data to the metrics log
                            if (_writeLogFile)
                            {
                                _metricsLog.WriteLine("[Metrics] " +
                                                      date +
                                                      " OHLC:[" +
                                                      open.ToString() + " " +
                                                      high.ToString() + " " +
                                                      low.ToString() + " " +
                                                      close.ToString() +
                                                      "] [Slow:" +
                                                      _maxLongResistance.ToString() +
                                                      "/" +
                                                      _minLongSupport.ToString() +
                                                      " Fast:" +
                                                     _maxShortResistance.ToString() +
                                                      "/" +
                                                      _minShortSupport.ToString() +
                                                      " T: " +
                                                      _trend.ToString() +
                                                      "]");
                            }
                        }
                    }
                    // We have reached the end of the loop as there is no more data.
                    // If we are in a trade we need to close it.
                    if ((trade == LONG) || (trade == SHORT))
                    {
                        // The exit is different.
                        // The system exits the final trade at the average of 
                        // the final closing price and the worst price of the day
                        if (trade == LONG)
                        {
                            closeTradePrice = (nextClose + nextLow) * 0.5M;
                        }
                        else
                        {
                            closeTradePrice = (nextClose + nextHigh) * 0.5M;
                        }
                        _profitLoss += (closeTradePrice - openTradePrice) * positionSize;
                        _equity += (closeTradePrice - openTradePrice) * positionSize;
                        _closingBalance = _equity;
                        // Update the current equity
                        _currentEquity = _closingBalance;
                        _currEquityList.Add(new Equity(nextDate, _currentEquity));
                        // Projected Equity
                        _projectedEquity = PerformanceCalculators.CalculateProjectedEquity(_benchmarkReturn, _initialEquity, _currEquityList);
                        // Instantaneously Compounding Annual Growth Rate
                        _icagr = PerformanceCalculators.CalculateICAGR(_currEquityList);
                        // Update the values to calculate the standard deviation of the fund return
                        _count++;
                        _sigmaX += _icagr;
                        _sigmaXSq += _icagr * _icagr;
                        trade = NOTRADE;
                        // We have closed out the trade so there is no open profit
                        _openProfit = 0.0M;
                        if (_writeLogFile)
                        {
                            // Write the string to the trade log
                            _tradeLogMessage.Append(nextDate);
                            _tradeLogMessage.Append(delimiterChar);
                            _tradeLogMessage.Append(closeTradePrice.ToString());
                            _tradeLogMessage.Append(delimiterChar);
                            _tradeLogMessage.Append(((closeTradePrice - openTradePrice) * positionSize).ToString());
                            _tradeLog.WriteLine(_tradeLogMessage);
                            // Re-set the message
                            _tradeLogMessage = new StringBuilder();
                            _tradeLogMessage.Append("Total Profit and Loss = " + _profitLoss.ToString());
                            _tradeLog.WriteLine(_tradeLogMessage);
                            // Empty the message
                            _tradeLogMessage = null;
                            _equityLog.WriteLine(date.ToShortDateString() + delimiterChar +
                                                 _closingBalance.ToString() + delimiterChar +
                                                 _openProfit.ToString() + delimiterChar +
                                                 _currentEquity.ToString() + delimiterChar +
                                                 _projectedEquity.ToString() + delimiterChar +
                                                 _icagr.ToString());
                        }
                    }
                    else
                    {
                        // We are not in a trade
                        _openProfit = 0.0M;
                        _closingBalance = _equity;
                        // Update the current equity
                        _currentEquity = _closingBalance + _openProfit;
                        _currEquityList.Add(new Equity(nextDate, _currentEquity));
                        // Projected Equity
                        _projectedEquity = PerformanceCalculators.CalculateProjectedEquity(_benchmarkReturn, _initialEquity, _currEquityList);
                        // Instantaneously Compounding Annual Growth Rate
                        _icagr = PerformanceCalculators.CalculateICAGR(_currEquityList);
                        // Update the values to calculate the standard deviation of the fund return
                        _count++;
                        _sigmaX += _icagr;
                        _sigmaXSq += _icagr * _icagr;
                        if (_writeLogFile)
                        {
                            _tradeLogMessage.Append("Total Profit and Loss = " + _profitLoss.ToString());
                            _tradeLog.WriteLine(_tradeLogMessage);
                            // Empty the message
                            _tradeLogMessage = null;
                            _equityLog.WriteLine(date.ToShortDateString() + delimiterChar +
                                                 _closingBalance.ToString() + delimiterChar +
                                                 _openProfit.ToString() + delimiterChar +
                                                 _currentEquity.ToString() + delimiterChar +
                                                 _projectedEquity.ToString() + delimiterChar +
                                                 _icagr.ToString());
                        }
                    }
                }
                // Instantaneously Compounding Annual Growth Rate
                ICAGR = PerformanceCalculators.CalculateICAGR(_currEquityList);
                if (_writeLogFile)
                {
                    _equityLog.WriteLine("The ICAGR = " + ICAGR.ToString());
                }
                // Find the Maximum percentage draw down
                DateTime DDDate;
                double dd = 0.0;
                PerformanceCalculators.CalculateMaxDD(_currEquityList, out dd, out DDDate);
                DD = dd;
                // The Bliss 
                Bliss = ICAGR / DD * 100.0;
                // The Sharpe Ratio
                double doubleCount = double.Parse(_count.ToString());
                double variance = (_sigmaXSq - _sigmaX * _sigmaX / doubleCount) / doubleCount;
                double stdev = Math.Sqrt(variance);
                SharpeRatio = (ICAGR - double.Parse((_benchmarkReturn).ToString())) / stdev;
                EndingEquity = _currEquityList[_currEquityList.Count - 1].CurrentEquity;
                if (_writeLogFile)
                {
                    _equityLog.WriteLine("The ending equity was:" + EndingEquity.ToString());
                    _equityLog.WriteLine("The Largest Percent Draw Down was: " + DD.ToString() + " on " + DDDate.ToShortDateString());
                    _equityLog.WriteLine("The Bliss is: " + Bliss.ToString());
                    _equityLog.WriteLine("The Sharpe Ratio is: " + SharpeRatio.ToString());
                    // Close all the log files.
                    _tradeLog.Close();
                    _equityLog.Close();
                    _metricsLog.Close();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Find the minimum value in a list
        /// Assumes that the list is not null and has one or more elements
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private decimal FindMin(List<decimal> list)
        {
            if (list.Count == 1)
            {
                return list[0];
            }
            else
            {
                decimal min = list[0];
                foreach (decimal value in list)
                {
                    min = Math.Min(value, min);
                }
                return min;
            }
        }

        /// <summary>
        /// Find the maximum value in a list
        /// Assumes that the list is not null and has one or more elements
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private decimal FindMax(List<decimal> list)
        {
            if (list.Count == 1)
            {
                return list[0];
            }
            else
            {
                decimal max = list[0];
                foreach (decimal value in list)
                {
                    max = Math.Max(value, max);
                }
                return max;
            }
        }
        #endregion
    }
}
