// Copyright (c) 2012, Adam Duval Kangaroo Point South 4169, Australia.
// All Rights Reserved.
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Adam.Trading.Mechanical.Model
{
    public class Gann2DayCalculator
    {
        #region Constants
        private const int ATRDAYS = 20;  // The average true range (atr) is calculated over the last 20 trading days
        #endregion
        #region Class private variables
        private List<Gann2DayTradeData> _currentTrades;                        // The list of current trades
        private List<LineData> _majorUpBuyTrades = new List<LineData>();       // The list of all Major trend change to Up buy trades
        private List<LineData> _majorDownSellTrades = new List<LineData>();    // The list of all Major trend change to Down sell trades
        private int _majorTrendChangeToUp;                                     // -1 or Top/Bottom Index if major trend has changed to Up
        private int _majorTrendChangeToDown;                                   // -1 or Top/Bottom Index if major trend has changed to Down
        private bool _majorTrendJustchanged;                                   // True if we have had a major trend change but no minor trend change yet
        private decimal _trendChangePrice;                                     // The price at which the major trend (above) occurred.
        private List<LineData> _stoppedOutTrades = new List<LineData>();       // The list of all stopped out trades
        private List<LineData> _pyramidBuyTrades = new List<LineData>();       // The list of all Major trend Up retracement buy trades
        private List<LineData> _pyramidSellTrades = new List<LineData>();      // The list of all Major trend Down retracement sell trades
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
        private decimal _benchmarkReturn; 
        private TradingEnumerations.Trend _majorTrend = TradingEnumerations.Trend.Unknown;          // The Major Trend based on the Gann 2 Day Swing Chart
        private TradingEnumerations.Trend _minorTrend = TradingEnumerations.Trend.Unknown;          // The Minor Trend based on the Gann 2 Day Swing chart
        private StringBuilder _tradeLogMessage;                                                     // The message to write to the trade log file
        private StreamWriter _tradeLog;                                                             // The trade log file
        private StreamWriter _equityLog;                                                            // The equity log file
        private StreamWriter _metricsLog;                                                           // The metrics log file
        private List<LineData> _gann2DaySwingList = new List<LineData>();                           // The list of Gann 2Day swing points
        private bool _writeLogFile = true;                                                          // Write to a log file or not
        private List<BarData> _ohlc = new List<BarData>();                                          // The Open, High, Low, Close data from the input file
        private bool _majorTrendInitialised = false;                                                // The major trend has been initialised
        private uint _identifier;                                                                   // The unique trade identifier. Incremented when a trade is opened
        private enum PyramidTradeType
        {
            None,
            Retracement,
            Trend,
            HiloTakenOut,
        }
        private decimal _trueRange;         // The current true range
        private decimal _atr;               // The average true range (ATR)
        private decimal _prevAtr;           // The previous avreage true range
        private decimal _atrLagNormalised;  // The lag for ATR calculation = (System.Convert.ToInt32(atrLag + 1)) * 0.5M;
        #endregion

        #region Accessor Methods
        /// <summary>
        /// The Entry Stop for Retracement
        /// </summary>
        public decimal EntryStopRetracement { get; private set; }

        /// <summary>
        /// The Entry Stop for Retracement is in ATRs
        /// </summary>
        public bool EntryStopRetracementATR { get; private set; }

        /// <summary>
        /// The Entry Stop for Exceed Hi Lo
        /// </summary>
        public decimal EntryStopExceedHiLo { get; private set; }

        /// <summary>
        /// The Entry Stop for Exceed Hi Lo is in ATRs
        /// </summary>
        public bool EntryStopExceedHiLoATR { get; private set; }

        /// <summary>
        /// The Exit Stop
        /// </summary>
        public decimal ExitStop { get; private set; }

        /// <summary>
        /// The Exit Stop is in ATRs
        /// </summary>
        public bool ExitStopATR { get; private set; }

        /// <summary>
        /// Use Current Equity for number of contracts (risk) calculation
        /// </summary>
        public bool UseCurrentEquity { get; private set; }

        /// <summary>
        /// The Data File name
        /// </summary>
        public string DataFile { get; private set; }

        /// <summary>
        /// The Dollars Per Point
        /// </summary>
        public decimal DollarsPerPoint { get; private set; }

        /// <summary>
        /// The Skid Points for Buy/Sell orders
        /// </summary>
        public decimal SkidPoints { get; private set; }

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

        public List<LineData> Gann2DaySwingList
        {
            get { return _gann2DaySwingList; }
        }

        public List<LineData> MajorUpBuyTrades
        {
            get { return _majorUpBuyTrades; }
        }

        public List<LineData> MajorDownSellTrades
        {
            get { return _majorDownSellTrades; }
        }

        public List<LineData> StoppedOutTrades
        {
            get { return _stoppedOutTrades; }
        }

        public List<LineData> PyramidBuyTrades
        {
            get { return _pyramidBuyTrades; }
        }

        public List<LineData> PyramidSellTrades
        {
            get { return _pyramidSellTrades; }
        }
        #endregion

        #region The Contructor
        /// <summary>
        /// The constructor
        /// </summary>
        /// <param name="entryStopRetracement">The Entry Stop for Retracement.</param>
        /// <param name="entryStopRetracementATR">The Entry Stop for Retracement is in ATRs.</param>
        /// <param name="entryStopExceedHiLo">The Entry Stop for Exceed Hi Lo.</param>
        /// <param name="entryStopExceedHiLoATR">The Entry Stop for Exceed Hi Lo is in ATRs.</param>
        /// <param name="exitStop">The Exit Stop.</param>
        /// <param name="exitStopATR">The Exit Stop is in ATRs.</param>
        /// <param name="useCurrentEquity">Use Current Equity for the number of contracts (risk) calc.</param>
        /// <param name="dataFile">The Data File name.</param>
        /// <param name="dollarsPerPoint">The Dollars Per Point.</param>
        /// <param name="skidPoints">The Skid Points for Buy/Sell orders.</param>
        /// <param name="startingEquity">The Starting Equity.</param>
        /// <param name="percentRisk">The Percent risked per trade.</param>
        /// <param name="benchmarkReturn">The Benchmark return to calculate Sharpe ratio</param>
        /// <param name="writeLogFile">True to output to the metrics log.</param>
        public Gann2DayCalculator(decimal entryStopRetracement,
                                  bool entryStopRetracementATR,
                                  decimal entryStopExceedHiLo,
                                  bool entryStopExceedHiLoATR,
                                  decimal exitStop,
                                  bool exitStopATR,
                                  bool useCurrentEquity,
                                  string dataFile,
                                  decimal dollarsPerPoint,
                                  decimal skidPoints,
                                  decimal startingEquity,
                                  decimal percentRisk,
                                  decimal benchmarkReturn,
                                  bool writeLogFile)
        {
            EntryStopRetracement = entryStopRetracement;
            EntryStopRetracementATR = entryStopRetracementATR;
            EntryStopExceedHiLo = entryStopExceedHiLo;
            EntryStopExceedHiLoATR = entryStopExceedHiLoATR;
            ExitStop = exitStop;
            ExitStopATR = exitStopATR;
            UseCurrentEquity = useCurrentEquity;
            DataFile = dataFile;
            DollarsPerPoint = dollarsPerPoint;
            SkidPoints = skidPoints;
            _profitLoss = 0.0M;
            _closingBalance = startingEquity;
            _openProfit = 0.0M;
            _equity = startingEquity;
            _percentRisk = percentRisk;
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
            _identifier = 0;
            _majorTrendChangeToUp = -1;
            _majorTrendChangeToDown = -1;
            _majorTrendJustchanged = false;
            _atrLagNormalised = (decimal)(ATRDAYS + 1) * 0.5M;
        }
        #endregion

        #region The Main Calculation Routine
        public void CalculateTradingSystem()
        {       
            _profitLoss = 0.0M;
            _closingBalance = _initialEquity;
            _openProfit = 0.0M;
            _equity = _initialEquity;
            _currentEquity = 0.0M;
            _currEquityList = new List<Equity>();
            _count = 0;
            _sigmaX = 0.0;
            _sigmaXSq = 0.0;
            _ohlc = new List<BarData>();
            _gann2DaySwingList = new List<LineData>();
            _majorUpBuyTrades = new List<LineData>();
            _majorDownSellTrades = new List<LineData>();
            _stoppedOutTrades = new List<LineData>();
            _pyramidBuyTrades = new List<LineData>();
            _pyramidSellTrades = new List<LineData>();
            _identifier = 0;
            _majorTrendInitialised = false;
            _majorTrendChangeToUp = -1;
            _majorTrendChangeToDown = -1;
            _majorTrendJustchanged = false;
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
                if (barDataReader.OHLC.Count >= 2)
                {
                    // Initialise the previous day's data
                    DateTime prevDate = barDataReader.OHLC[0].Date;
                    decimal prevOpen = barDataReader.OHLC[0].Open;
                    decimal prevHigh = barDataReader.OHLC[0].High;
                    decimal prevLow = barDataReader.OHLC[0].Low;
                    decimal prevClose = barDataReader.OHLC[0].Close;
                    _ohlc.Add(barDataReader.OHLC[0]);
                    // Initialise the ATR 
                    _trueRange = prevHigh - prevLow;
                    _atr = _trueRange;
                    // Initialise values
                    // The minor trend 
                    _minorTrend = TradingEnumerations.Trend.Unknown;
                    // The major trend
                    _majorTrend = TradingEnumerations.Trend.Unknown;
                    // Initialise the list of trades
                    _currentTrades = new List<Gann2DayTradeData>();
                    // The awarded trade price at start and finish of trade
                    decimal tradePrice = 0.0M;
                    // The highest high since a minor trend change and lowest low since a minor trend change
                    decimal lowestLowSinceMinorTrendChange = 0.0M;
                    decimal highestHighSinceMinorTrendChange = 0.0M;
                    decimal highAtMinorTrendChangeToUp = 0.0M;
                    decimal lowAtMinorTrendChangeToDown = 0.0M;
                    // Write the data to the equity log
                    _currentEquity = _closingBalance;  // There is no open profit
                    _currEquityList.Add(new Equity(prevDate, _currentEquity));
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
                        // Write the headers to the metrics log
                        _metricsLog.WriteLine("Date, Open, High, Low, Close, Minor Trend, Major Trend, Major To Up, Major To Down, Price for Major Change, Major Trend Just Changed, ATR, Contract Rollover Day");
                        // Write the initial values to the Metrics Log
                        _metricsLog.WriteLine(prevDate.ToString("yyyyMMdd") + ", " +
                                              prevOpen.ToString() + ", " +
                                              prevHigh.ToString() + ", " +
                                              prevLow.ToString() + ",  " +
                                              prevClose.ToString() + ", " +
                                              _minorTrend.ToString() + ", " +
                                              _majorTrend.ToString() + ", " +
                                              _majorTrendChangeToUp.ToString() + ", " +
                                              _majorTrendChangeToDown.ToString() + ", " +
                                              _trendChangePrice.ToString() + ", " +
                                              _majorTrendJustchanged.ToString() + ", " +
                                              _atr.ToString() + ", " +
                                              _ohlc[_ohlc.Count - 1].ContractRollover.ToString());
                        // Write headers for the equity log
                        _equityLog.WriteLine("Date, Closing Balance, Open Profit, Current Equity, Projected Equity Curve, ICAGR");
                        // Write first entry for the equity log
                        _equityLog.WriteLine(prevDate.ToString("yyyyMMdd") + ", " + 
                                             _closingBalance.ToString() + ", " +
                                             _openProfit.ToString() + ", " + 
                                             _currentEquity.ToString() + ", " +
                                             _projectedEquity.ToString() + ", " + 
                                             _icagr.ToString());
                        // Write headers to the trade log
                        _tradeLog.WriteLine("Identifier, Top/Bottom Identifier, Entry Date, Entry Price, Entry Reason, Entry Additional Reason, Long/Short, No Contracts, Exit Stop, Exit Date, Exit Price, Exit Reason, Profit/Loss");
                    }
                    // Initialise today's data
                    DateTime date = barDataReader.OHLC[1].Date;
                    decimal open = barDataReader.OHLC[1].Open;
                    decimal high = barDataReader.OHLC[1].High;
                    decimal low = barDataReader.OHLC[1].Low;
                    decimal close = barDataReader.OHLC[1].Close;
                    _ohlc.Add(barDataReader.OHLC[1]);
                    //
                    // Calculate the true range
                    // which is the maximum of 
                    // High-Low, High-Yesterday Close, Yesterday Close-Low
                    //
                    _prevAtr = _atr;
                    _trueRange = Math.Max(high - low, high - prevClose);
                    _trueRange = Math.Max(_trueRange, prevClose - low);
                    //
                    // Calculate the atr
                    //
                    _atr = _prevAtr + (_trueRange - _prevAtr) / _atrLagNormalised;
                    // Declare variables for next day data
                    DateTime nextDate = date;
                    decimal nextOpen = open;
                    decimal nextHigh = high;
                    decimal nextLow = low;
                    decimal nextClose = close;
                    for (int i = 2; i < barDataReader.OHLC.Count; i++)
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
                            _metricsLog.WriteLine(date.ToString("yyyyMMdd") + ", " + 
                                                  open.ToString() + ", " + 
                                                  high.ToString() + ", " +
                                                  low.ToString() + ",  " + 
                                                  close.ToString() + ", " +
                                                  _minorTrend.ToString() + ", " +
                                                  _majorTrend.ToString() + ", " +
                                                  _majorTrendChangeToUp.ToString() + ", " +
                                                  _majorTrendChangeToDown.ToString() + ", " +
                                                  _trendChangePrice.ToString() + ", " +
                                                  _majorTrendJustchanged.ToString() + ", " +
                                                  _atr.ToString() + ", " +
                                                  _ohlc[_ohlc.Count - 1].ContractRollover.ToString());
                            _equityLog.WriteLine(date.ToString("yyyyMMdd") + ", " + 
                                                 _closingBalance.ToString() + ", " +
                                                 _openProfit.ToString() + ", " + 
                                                 _currentEquity.ToString() + ", " +
                                                 _projectedEquity.ToString() + ", " + 
                                                 _icagr.ToString());
                        }
                        // Get the next buffer data
                        nextDate = barDataReader.OHLC[i].Date;
                        nextOpen = barDataReader.OHLC[i].Open;
                        nextHigh = barDataReader.OHLC[i].High;
                        nextLow = barDataReader.OHLC[i].Low;
                        nextClose = barDataReader.OHLC[i].Close;
                        // Major trend change to Up on previous day so we close out shorts and go long
                        if (_majorTrendInitialised &&
                            (_majorTrendChangeToUp > -1) && 
                            !TopBottomAlreadyInTradeList(_majorTrendChangeToUp, _currentTrades, TradingEnumerations.GannTradeReason.DefinateMainTrendChangeToUp))
                        {
                            // Enter at open plus skid points
                            tradePrice = nextOpen + SkidPoints;
                            if (tradePrice > nextHigh)
                            {
                                tradePrice = nextHigh;
                            }
                            if (TradeTypes(_currentTrades) == TradingEnumerations.GannTradeTypesInList.ShortOnly)
                            {
                                // Close out the existing short trades
                                decimal profit = CalculateProfit(_currentTrades, tradePrice, DollarsPerPoint);
                                _profitLoss += profit;
                                _equity += profit;
                                _closingBalance = _equity;
                                _openProfit = 0.0M;
                                foreach (Gann2DayTradeData trade in _currentTrades)
                                {
                                    trade.ExitDate = nextDate;
                                    trade.ExitPrice = tradePrice;
                                    trade.ExitReason = TradingEnumerations.GannTradeReason.DefinateMainTrendChangeToUp;
                                    if (_writeLogFile)
                                    {
                                        _tradeLog.WriteLine(trade.Identifier.ToString() + ", " +
                                                            trade.TopBottomIdentifier.ToString() + ", " +
                                                            trade.EntryDate.ToString("yyyyMMdd") + ", " +
                                                            trade.EntryPrice.ToString() + ", " +
                                                            trade.EntryReason.ToString() + ", " +
                                                            trade.EntryAdditionalReason.ToString() + ", " +
                                                            trade.EntryType.ToString() + ", " +
                                                            trade.Contracts.ToString() + ", " +
                                                            trade.ExitStop.ToString() + ", " +
                                                            nextDate.ToString("yyyyMMdd") + ", " +
                                                            trade.ExitPrice.ToString() + " ," +
                                                            trade.ExitReason.ToString() + ", " +
                                                            (trade.CalculateProfitPoints() * DollarsPerPoint).ToString());
                                    }
                                }
                                _currentTrades.Clear();
                            }
                            else if (TradeTypes(_currentTrades) != TradingEnumerations.GannTradeTypesInList.None)
                            {
                                if (_writeLogFile)
                                {
                                    _metricsLog.WriteLine("Trade list was not short or empty" + nextDate.ToString("yyyyMMdd"));
                                }
                            }
                            // All trades in list should be cleared out now if we were short
                            if (TradeTypes(_currentTrades) == TradingEnumerations.GannTradeTypesInList.None)
                            {
                                // A bottom must have been created and it must be the last
                                // value in the _gann2DaySwingList
                                decimal lowValue = _gann2DaySwingList[_gann2DaySwingList.Count - 1].Price;
                                decimal exitStop;
                                if (ExitStopATR)
                                {
                                    exitStop = lowValue - ExitStop * _atr;
                                }
                                else
                                {
                                     exitStop = lowValue - ExitStop;
                                }
                                // Open the new trade
                                decimal riskPerLot = (tradePrice - exitStop) * DollarsPerPoint;
                                uint positionSize;
                                if (UseCurrentEquity)
                                {
                                    positionSize = (uint)(_currentEquity * _percentRisk / riskPerLot);
                                }
                                else
                                {
                                    positionSize = (uint)(_closingBalance * _percentRisk / riskPerLot);
                                }
                                Gann2DayTradeData newTrade = new Gann2DayTradeData(_identifier,
                                                                                   _majorTrendChangeToUp,
                                                                                   nextDate,
                                                                                   tradePrice,
                                                                                   TradingEnumerations.GannTradeReason.DefinateMainTrendChangeToUp,
                                                                                   TradingEnumerations.GannTradeAdditionalEntryReason.None,
                                                                                   positionSize,
                                                                                   TradingEnumerations.TradeStatus.Long,
                                                                                   exitStop,
                                                                                   null,
                                                                                   null,
                                                                                   TradingEnumerations.GannTradeReason.NotExitedYet);
                                _currentTrades.Add(newTrade);
                                _identifier++;
                                _majorUpBuyTrades.Add(new LineData(nextDate, tradePrice));
                            }
                            else
                            {
                                if (_writeLogFile)
                                {
                                    _metricsLog.WriteLine("More than one trade found in list on " + nextDate.ToString("yyyyMMdd"));
                                }
                            }
                        }
                        // Major trend change to Down on previous day so we close out longs and go short
                        else if (_majorTrendInitialised &&
                                 (_majorTrendChangeToDown > -1) &&
                                 !TopBottomAlreadyInTradeList(_majorTrendChangeToDown, _currentTrades, TradingEnumerations.GannTradeReason.DefinateMainTrendChangeToDown))
                        {
                            tradePrice = nextOpen - SkidPoints;
                            if (tradePrice < nextLow)
                            {
                                tradePrice = nextLow;
                            }
                            if (TradeTypes(_currentTrades) == TradingEnumerations.GannTradeTypesInList.LongOnly)
                            {
                                // Close out the existing long trades
                                decimal profit = CalculateProfit(_currentTrades, tradePrice, DollarsPerPoint);
                                _profitLoss += profit;
                                _equity += profit;
                                _closingBalance = _equity;
                                _openProfit = 0.0M;
                                foreach (Gann2DayTradeData trade in _currentTrades)
                                {
                                    trade.ExitDate = nextDate;
                                    trade.ExitPrice = tradePrice;
                                    trade.ExitReason = TradingEnumerations.GannTradeReason.DefinateMainTrendChangeToDown;
                                    if (_writeLogFile)
                                    {
                                        _tradeLog.WriteLine(trade.Identifier.ToString() + ", " +
                                                            trade.TopBottomIdentifier.ToString() + ", " +
                                                            trade.EntryDate.ToString("yyyyMMdd") + ", " +
                                                            trade.EntryPrice.ToString() + ", " +
                                                            trade.EntryReason.ToString() + ", " +
                                                            trade.EntryAdditionalReason.ToString() + ", " +
                                                            trade.EntryType.ToString() + ", " +
                                                            trade.Contracts.ToString() + ", " +
                                                            trade.ExitStop.ToString() + ", " +
                                                            nextDate.ToString("yyyyMMdd") + ", " +
                                                            trade.ExitPrice.ToString() + " ," +
                                                            trade.ExitReason.ToString() + ", " +
                                                            (trade.CalculateProfitPoints() * DollarsPerPoint).ToString());
                                    }
                                }
                                _currentTrades.Clear();
                            }
                            else if (TradeTypes(_currentTrades) != TradingEnumerations.GannTradeTypesInList.None)
                            {
                                if (_writeLogFile)
                                {
                                    _metricsLog.WriteLine("Trade list was not long or empty" + nextDate.ToString("yyyyMMdd"));
                                }
                            }
                            // All trades in list should be cleared out now if we were long
                            if (TradeTypes(_currentTrades) == TradingEnumerations.GannTradeTypesInList.None)
                            {
                                // A top must have been created and it must be the last
                                // value in the _gann2DaySwingList
                                decimal highValue = _gann2DaySwingList[_gann2DaySwingList.Count - 1].Price;
                                decimal exitStop;
                                if (ExitStopATR)
                                {
                                    exitStop = highValue + ExitStop * _atr;
                                }
                                else
                                {
                                    exitStop = highValue + ExitStop;
                                }
                                // Open the new trade
                                decimal riskPerLot = (exitStop - tradePrice) * DollarsPerPoint;
                                uint positionSize;
                                if (UseCurrentEquity)
                                {
                                    positionSize = (uint)(_currentEquity * _percentRisk / riskPerLot);
                                }
                                else
                                {
                                    positionSize = (uint)(_closingBalance * _percentRisk / riskPerLot);
                                }
                                Gann2DayTradeData newTrade = new Gann2DayTradeData(_identifier,
                                                                                   _majorTrendChangeToDown,
                                                                                   nextDate,
                                                                                   tradePrice,
                                                                                   TradingEnumerations.GannTradeReason.DefinateMainTrendChangeToDown,
                                                                                   TradingEnumerations.GannTradeAdditionalEntryReason.None,
                                                                                   positionSize,
                                                                                   TradingEnumerations.TradeStatus.Short,
                                                                                   exitStop,
                                                                                   null,
                                                                                   null,
                                                                                   TradingEnumerations.GannTradeReason.NotExitedYet);
                                _currentTrades.Add(newTrade);
                                _identifier++;
                                _majorDownSellTrades.Add(new LineData(nextDate, tradePrice));
                            }
                            else
                            {
                                if (_writeLogFile)
                                {
                                    _metricsLog.WriteLine("More than one trade found in list on " + nextDate.ToString("yyyyMMdd"));
                                }
                            }
                        }
                        else
                        {
                            if (_majorTrendInitialised)
                            {
                                if (_majorTrend == TradingEnumerations.Trend.Up)
                                {
                                    bool retracementPyramidPossible = false;
                                    decimal retracementPyramidEntryStop = 0.0M;
                                    decimal retracementPyramidTradePrice = 0.0M;
                                    decimal retracementPyramidExitStop = 0.0M;
                                    int retracementPyramidIndex = 0;
                                    bool trendUpPyramidPossible = false;
                                    decimal trendUpPyramidEntryStop = 0.0M;
                                    decimal trendUpPyramidTradePrice = 0.0M;
                                    decimal trendUpPyramidExitStop = 0.0M;
                                    int trendUpPyramidIndex = 0;
                                    bool exceedHiPyramidPossible = false;
                                    decimal exceedHiPyramidEntryStop = 0.0M;
                                    decimal exceedHiPyramidTradePrice = 0.0M;
                                    decimal exceedHiPyramidExitStop = 0.0M;
                                    int exceedHiPyramidIndex = 0;
                                    // Retracement Pyramid Trade
                                    if ((_minorTrend ==  TradingEnumerations.Trend.Down) && (high >= prevHigh) && (low >= prevLow) &&
                                         // Note: -1 for last Top here!
                                         !TopBottomAlreadyInTradeList(_gann2DaySwingList.Count - 1, _currentTrades, TradingEnumerations.GannTradeReason.MainTrendUpPyramid))
                                    {
                                        // The minor trend could change to up if nextHigh >= high
                                        if (EntryStopRetracementATR)
                                        {
                                            retracementPyramidEntryStop = high + EntryStopRetracement * _atr;
                                        }
                                        else
                                        {
                                            retracementPyramidEntryStop = high + EntryStopRetracement;
                                        }
                                        if ((nextHigh >= high) && (nextHigh > retracementPyramidEntryStop))
                                        {
                                            retracementPyramidPossible = true;
                                            retracementPyramidTradePrice = retracementPyramidEntryStop + SkidPoints;
                                            if (!((retracementPyramidTradePrice >= nextLow) && (retracementPyramidTradePrice <= nextHigh)))
                                            {
                                                retracementPyramidTradePrice = nextHigh;
                                            }
                                            if (ExitStopATR)
                                            {
                                                retracementPyramidExitStop = lowestLowSinceMinorTrendChange - ExitStop * _atr;
                                            }
                                            else
                                            {
                                                retracementPyramidExitStop = lowestLowSinceMinorTrendChange - ExitStop;
                                            }
                                            retracementPyramidIndex = _gann2DaySwingList.Count - 1;
                                        }
                                    }
                                    else if ((_minorTrend == TradingEnumerations.Trend.Up) &&
                                             (!_majorTrendJustchanged) &&
                                             // Note: -2 for last Top here! Because an extra bottom has been added as we have had a minor trend change to up.
                                             !TopBottomAlreadyInTradeList(_gann2DaySwingList.Count - 2, _currentTrades, TradingEnumerations.GannTradeReason.MainTrendUpPyramid))
                                    {
                                        // The minor trend has already changed to up and we have no trade for previous top
                                        if (EntryStopRetracementATR)
                                        {
                                            trendUpPyramidEntryStop = highAtMinorTrendChangeToUp + EntryStopRetracement * _atr;
                                        }
                                        else
                                        {
                                            trendUpPyramidEntryStop = highAtMinorTrendChangeToUp + EntryStopRetracement;
                                        }
                                        if (nextHigh > trendUpPyramidEntryStop)
                                        {
                                            trendUpPyramidPossible = true;
                                            trendUpPyramidTradePrice = trendUpPyramidEntryStop + SkidPoints;
                                            if (!((trendUpPyramidTradePrice >= nextLow) && (trendUpPyramidTradePrice <= nextHigh)))
                                            {
                                                trendUpPyramidTradePrice = nextHigh;
                                            }
                                            if (ExitStopATR)
                                            {
                                                trendUpPyramidExitStop = _gann2DaySwingList[_gann2DaySwingList.Count - 1].Price - ExitStop * _atr;
                                            }
                                            else
                                            {
                                                trendUpPyramidExitStop = _gann2DaySwingList[_gann2DaySwingList.Count - 1].Price - ExitStop;
                                            }
                                            trendUpPyramidIndex = _gann2DaySwingList.Count - 2;
                                        }
                                    }
                                    // Top taken out pyramid trade
                                    if (_minorTrend == TradingEnumerations.Trend.Down &&
                                        // Note: -1 for last Top here!
                                        !TopBottomAlreadyInTradeList(_gann2DaySwingList.Count - 1, _currentTrades, TradingEnumerations.GannTradeReason.MainTrendUpPyramid))
                                    {
                                        // We could enter a trade if the previous high is taken out
                                        if (EntryStopExceedHiLoATR)
                                        {
                                            exceedHiPyramidEntryStop = _gann2DaySwingList[_gann2DaySwingList.Count - 1].Price + EntryStopExceedHiLo * _atr;
                                        }
                                        else
                                        {
                                            exceedHiPyramidEntryStop = _gann2DaySwingList[_gann2DaySwingList.Count - 1].Price + EntryStopExceedHiLo;
                                        }
                                        if (nextHigh > exceedHiPyramidEntryStop)
                                        {
                                            exceedHiPyramidPossible = true;
                                            exceedHiPyramidTradePrice = exceedHiPyramidEntryStop + SkidPoints;
                                            if (!((exceedHiPyramidTradePrice >= nextLow) && (exceedHiPyramidTradePrice <= nextHigh)))
                                            {
                                                exceedHiPyramidTradePrice = nextHigh;
                                            }
                                            if (ExitStopATR)
                                            {
                                                exceedHiPyramidExitStop = lowestLowSinceMinorTrendChange - ExitStop * _atr;
                                            }
                                            else
                                            {
                                                exceedHiPyramidExitStop = lowestLowSinceMinorTrendChange - ExitStop;
                                            }
                                            exceedHiPyramidIndex = _gann2DaySwingList.Count - 1;
                                        }
                                    }
                                    else if ((_minorTrend == TradingEnumerations.Trend.Up) &&
                                             (!_majorTrendJustchanged) &&
                                             // Note: -2 for last Top here! Because an extra bottom has been added as we have had a minor trend change to up.
                                             !TopBottomAlreadyInTradeList(_gann2DaySwingList.Count - 2, _currentTrades, TradingEnumerations.GannTradeReason.MainTrendUpPyramid))
                                    {
                                        // The minor trend has already changed to up and we have no trade for previous top
                                        if (EntryStopExceedHiLoATR)
                                        {
                                            exceedHiPyramidEntryStop = _gann2DaySwingList[_gann2DaySwingList.Count - 2].Price + EntryStopExceedHiLo * _atr;
                                        }
                                        else
                                        {
                                            exceedHiPyramidEntryStop = _gann2DaySwingList[_gann2DaySwingList.Count - 2].Price + EntryStopExceedHiLo;
                                        }
                                        if (nextHigh > exceedHiPyramidEntryStop)
                                        {
                                            exceedHiPyramidPossible = true;
                                            exceedHiPyramidTradePrice = exceedHiPyramidEntryStop + SkidPoints;
                                            if (!((exceedHiPyramidTradePrice >= nextLow) && (exceedHiPyramidTradePrice <= nextHigh)))
                                            {
                                                exceedHiPyramidTradePrice = nextHigh;
                                            }
                                            if (ExitStopATR)
                                            {
                                                exceedHiPyramidExitStop = _gann2DaySwingList[_gann2DaySwingList.Count - 1].Price - ExitStop * _atr;
                                            }
                                            else
                                            {
                                                exceedHiPyramidExitStop = _gann2DaySwingList[_gann2DaySwingList.Count - 1].Price - ExitStop;
                                            }
                                            exceedHiPyramidIndex = _gann2DaySwingList.Count - 2;
                                        }
                                    }
                                    decimal lowestEntryStop = decimal.MaxValue;
                                    PyramidTradeType trade = PyramidTradeType.None;
                                    int index = 0;
                                    TradingEnumerations.GannTradeReason reason = TradingEnumerations.GannTradeReason.NoTrade;
                                    TradingEnumerations.GannTradeAdditionalEntryReason additionalReason = TradingEnumerations.GannTradeAdditionalEntryReason.None;
                                    decimal exitStop = 0.0M;
                                    if (retracementPyramidPossible)
                                    {
                                        trade = PyramidTradeType.Retracement;
                                        lowestEntryStop = retracementPyramidEntryStop;
                                        index = retracementPyramidIndex;
                                        reason = TradingEnumerations.GannTradeReason.MainTrendUpPyramid;
                                        additionalReason = TradingEnumerations.GannTradeAdditionalEntryReason.PossibleMinorChangeToUp;
                                        tradePrice = retracementPyramidTradePrice;
                                        exitStop = retracementPyramidExitStop;
                                    }
                                    if (trendUpPyramidPossible)
                                    {
                                        if (trendUpPyramidEntryStop < lowestEntryStop)
                                        {
                                            lowestEntryStop = trendUpPyramidEntryStop;
                                            trade = PyramidTradeType.Trend;
                                            index = trendUpPyramidIndex;
                                            reason = TradingEnumerations.GannTradeReason.MainTrendUpPyramid;
                                            additionalReason = TradingEnumerations.GannTradeAdditionalEntryReason.DefinateMinorChangeToUp;
                                            tradePrice = trendUpPyramidTradePrice;
                                            exitStop = trendUpPyramidExitStop;
                                        }
                                    }
                                    if (exceedHiPyramidPossible)
                                    {
                                        if (exceedHiPyramidEntryStop < lowestEntryStop)
                                        {
                                            lowestEntryStop = exceedHiPyramidEntryStop;
                                            trade = PyramidTradeType.HiloTakenOut;
                                            index = exceedHiPyramidIndex;
                                            reason = TradingEnumerations.GannTradeReason.MainTrendUpPyramid;
                                            additionalReason = TradingEnumerations.GannTradeAdditionalEntryReason.ExceedHi;
                                            tradePrice = exceedHiPyramidTradePrice;
                                            exitStop = exceedHiPyramidExitStop;
                                        }
                                    }
                                    if (trade != PyramidTradeType.None)
                                    {
                                        // Open the new trade
                                        decimal riskPerLot = (tradePrice - exitStop) * DollarsPerPoint;
                                        uint positionSize;
                                        if (UseCurrentEquity)
                                        {
                                            positionSize = (uint)(_currentEquity * _percentRisk / riskPerLot);
                                        }
                                        else
                                        {
                                            positionSize = (uint)(_closingBalance * _percentRisk / riskPerLot);
                                        }
                                        Gann2DayTradeData newTrade = new Gann2DayTradeData(_identifier,
                                                                                           index,
                                                                                           nextDate,
                                                                                           tradePrice,
                                                                                           reason,
                                                                                           additionalReason,
                                                                                           positionSize,
                                                                                           TradingEnumerations.TradeStatus.Long,
                                                                                           exitStop,
                                                                                           null,
                                                                                           null,
                                                                                           TradingEnumerations.GannTradeReason.NotExitedYet);
                                        _currentTrades.Add(newTrade);
                                        _identifier++;
                                        _pyramidBuyTrades.Add(new LineData(nextDate, tradePrice));
                                    }
                                }
                                else if (_majorTrend == TradingEnumerations.Trend.Down)
                                {
                                    bool retracementPyramidPossible = false;
                                    decimal retracementPyramidEntryStop = 0.0M;
                                    decimal retracementPyramidTradePrice = 0.0M;
                                    decimal retracementPyramidExitStop = 0.0M;
                                    int retracementPyramidIndex = 0;
                                    bool trendDownPyramidPossible = false;
                                    decimal trendDownPyramidEntryStop = 0.0M;
                                    decimal trendDownPyramidTradePrice = 0.0M;
                                    decimal trendDownPyramidExitStop = 0.0M;
                                    int trendDownPyramidIndex = 0;
                                    bool exceedLoPyramidPossible = false;
                                    decimal exceedLoPyramidEntryStop = 0.0M;
                                    decimal exceedLoPyramidTradePrice = 0.0M;
                                    decimal exceedLoPyramidExitStop = 0.0M;
                                    int exceedLoPyramidIndex = 0;
                                    if ((_minorTrend == TradingEnumerations.Trend.Up) && (low < prevLow) &&
                                        // Note: -1 for last Bottom here!
                                        !TopBottomAlreadyInTradeList(_gann2DaySwingList.Count - 1, _currentTrades, TradingEnumerations.GannTradeReason.MainTrendDownPyramid))
                                    {
                                        // The minor trend could change to down if nextLow < low
                                        if (EntryStopRetracementATR)
                                        {
                                            retracementPyramidEntryStop = low - EntryStopRetracement * _atr;
                                        }
                                        else
                                        {
                                            retracementPyramidEntryStop = low - EntryStopRetracement;
                                        }
                                        if ((nextLow < low) && (nextLow < retracementPyramidEntryStop))
                                        {
                                            retracementPyramidPossible = true;
                                            retracementPyramidTradePrice = retracementPyramidEntryStop - SkidPoints;
                                            if (!((retracementPyramidTradePrice >= nextLow) && (retracementPyramidTradePrice <= nextHigh)))
                                            {
                                                retracementPyramidTradePrice = nextLow;
                                            }
                                            if (ExitStopATR)
                                            {
                                                retracementPyramidExitStop = highestHighSinceMinorTrendChange + ExitStop * _atr;
                                            }
                                            else
                                            {
                                                retracementPyramidExitStop = highestHighSinceMinorTrendChange + ExitStop;
                                            }
                                            retracementPyramidIndex = _gann2DaySwingList.Count - 1;
                                        }
                                    }
                                    else if ((_minorTrend == TradingEnumerations.Trend.Down) &&
                                             (!_majorTrendJustchanged) &&
                                             // Note: -2 for last Bottom here! Because an extra top has been added as we have had a minor trend change to down.
                                             !TopBottomAlreadyInTradeList(_gann2DaySwingList.Count - 2, _currentTrades, TradingEnumerations.GannTradeReason.MainTrendDownPyramid))
                                    {
                                        // The minor trend has already changed to down and we have no trade for previous bottom
                                        if (EntryStopRetracementATR)
                                        {
                                            trendDownPyramidEntryStop = lowAtMinorTrendChangeToDown - EntryStopRetracement * _atr;
                                        }
                                        else
                                        {
                                            trendDownPyramidEntryStop = lowAtMinorTrendChangeToDown - EntryStopRetracement;
                                        }
                                        if (nextLow < trendDownPyramidEntryStop)
                                        {
                                            trendDownPyramidPossible = true;
                                            trendDownPyramidTradePrice = trendDownPyramidEntryStop - SkidPoints;
                                            if (!((trendDownPyramidTradePrice >= nextLow) && (trendDownPyramidTradePrice <= nextHigh)))
                                            {
                                                trendDownPyramidTradePrice = nextLow;
                                            }
                                            if (ExitStopATR)
                                            {
                                                trendDownPyramidExitStop = _gann2DaySwingList[_gann2DaySwingList.Count - 1].Price + ExitStop * _atr;
                                            }
                                            else
                                            {
                                                trendDownPyramidExitStop = _gann2DaySwingList[_gann2DaySwingList.Count - 1].Price + ExitStop;
                                            }
                                            trendDownPyramidIndex = _gann2DaySwingList.Count - 2;
                                        }
                                    }
                                    // Bottom taken out pyramid trade
                                    if (_minorTrend == TradingEnumerations.Trend.Up &&
                                        // Note: -1 for last Bottom here!
                                        !TopBottomAlreadyInTradeList(_gann2DaySwingList.Count - 1, _currentTrades, TradingEnumerations.GannTradeReason.MainTrendDownPyramid))
                                    {
                                        // We could enter a trade if the previous low is taken out
                                        if (EntryStopExceedHiLoATR)
                                        {
                                            exceedLoPyramidEntryStop = _gann2DaySwingList[_gann2DaySwingList.Count - 1].Price - EntryStopExceedHiLo * _atr;
                                        }
                                        else
                                        {
                                            exceedLoPyramidEntryStop = _gann2DaySwingList[_gann2DaySwingList.Count - 1].Price - EntryStopExceedHiLo;
                                        }
                                        if (nextLow < exceedLoPyramidEntryStop)
                                        {
                                            exceedLoPyramidPossible = true;
                                            exceedLoPyramidTradePrice = exceedLoPyramidEntryStop - SkidPoints;
                                            if (!((exceedLoPyramidTradePrice >= nextLow) && (exceedLoPyramidTradePrice <= nextHigh)))
                                            {
                                                exceedLoPyramidTradePrice = nextHigh;
                                            }
                                            if (ExitStopATR)
                                            {
                                                exceedLoPyramidExitStop = highestHighSinceMinorTrendChange + ExitStop * _atr;
                                            }
                                            else
                                            {
                                                exceedLoPyramidExitStop = highestHighSinceMinorTrendChange + ExitStop;
                                            }
                                            exceedLoPyramidIndex = _gann2DaySwingList.Count - 1;
                                        }
                                    }
                                    else if ((_minorTrend == TradingEnumerations.Trend.Down) &&
                                             (!_majorTrendJustchanged) &&
                                             // Note: -2 for last Bottom here! Because an extra top has been added as we have had a minor trend change to down.
                                             !TopBottomAlreadyInTradeList(_gann2DaySwingList.Count - 2, _currentTrades, TradingEnumerations.GannTradeReason.MainTrendDownPyramid))
                                    {
                                        // The minor trend has already changed to down and we have no trade for previous bottom
                                        if (EntryStopExceedHiLoATR)
                                        {
                                            exceedLoPyramidEntryStop = _gann2DaySwingList[_gann2DaySwingList.Count - 2].Price - EntryStopExceedHiLo * _atr;
                                        }
                                        else
                                        {
                                            exceedLoPyramidEntryStop = _gann2DaySwingList[_gann2DaySwingList.Count - 2].Price - EntryStopExceedHiLo;
                                        }
                                        if (nextLow < exceedLoPyramidEntryStop)
                                        {
                                            exceedLoPyramidPossible = true;
                                            exceedLoPyramidTradePrice = exceedLoPyramidEntryStop - SkidPoints;
                                            if (!((exceedLoPyramidTradePrice >= nextLow) && (exceedLoPyramidTradePrice <= nextHigh)))
                                            {
                                                exceedLoPyramidTradePrice = nextLow;
                                            }
                                            if (ExitStopATR)
                                            {
                                                exceedLoPyramidExitStop = _gann2DaySwingList[_gann2DaySwingList.Count - 1].Price + ExitStop * _atr;
                                            }
                                            else
                                            {
                                                exceedLoPyramidExitStop = _gann2DaySwingList[_gann2DaySwingList.Count - 1].Price + ExitStop;
                                            }
                                            exceedLoPyramidIndex = _gann2DaySwingList.Count - 2;
                                        }
                                    }
                                    decimal highestEntryStop = decimal.MinValue;
                                    PyramidTradeType trade = PyramidTradeType.None;
                                    int index = 0;
                                    TradingEnumerations.GannTradeReason reason = TradingEnumerations.GannTradeReason.NoTrade;
                                    TradingEnumerations.GannTradeAdditionalEntryReason additionalReason = TradingEnumerations.GannTradeAdditionalEntryReason.None;
                                    decimal exitStop = 0.0M;
                                    if (retracementPyramidPossible)
                                    {
                                        trade = PyramidTradeType.Retracement;
                                        highestEntryStop = retracementPyramidEntryStop;
                                        index = retracementPyramidIndex;
                                        reason = TradingEnumerations.GannTradeReason.MainTrendDownPyramid;
                                        additionalReason = TradingEnumerations.GannTradeAdditionalEntryReason.PossibleMinorChangeToDown;
                                        tradePrice = retracementPyramidTradePrice;
                                        exitStop = retracementPyramidExitStop;
                                    }
                                    if (trendDownPyramidPossible)
                                    {
                                        if (trendDownPyramidEntryStop > highestEntryStop)
                                        {
                                            highestEntryStop = trendDownPyramidEntryStop;
                                            trade = PyramidTradeType.Trend;
                                            index = trendDownPyramidIndex;
                                            reason = TradingEnumerations.GannTradeReason.MainTrendDownPyramid;
                                            additionalReason = TradingEnumerations.GannTradeAdditionalEntryReason.DefinateMinorChangeToDown;
                                            tradePrice = trendDownPyramidTradePrice;
                                            exitStop = trendDownPyramidExitStop;
                                        }
                                    }
                                    if (exceedLoPyramidPossible)
                                    {
                                        if (exceedLoPyramidEntryStop > highestEntryStop)
                                        {
                                            highestEntryStop = exceedLoPyramidEntryStop;
                                            trade = PyramidTradeType.HiloTakenOut;
                                            index = exceedLoPyramidIndex;
                                            reason = TradingEnumerations.GannTradeReason.MainTrendDownPyramid;
                                            additionalReason = TradingEnumerations.GannTradeAdditionalEntryReason.ExceedLo;
                                            tradePrice = exceedLoPyramidTradePrice;
                                            exitStop = exceedLoPyramidExitStop;
                                        }
                                    }
                                    if (trade != PyramidTradeType.None)
                                    {
                                        // Open the new trade
                                        decimal riskPerLot = (exitStop - tradePrice) * DollarsPerPoint;
                                        uint positionSize;
                                        if (UseCurrentEquity)
                                        {
                                            positionSize = (uint)(_currentEquity * _percentRisk / riskPerLot);
                                        }
                                        else
                                        {
                                            positionSize = (uint)(_closingBalance * _percentRisk / riskPerLot);
                                        }
                                        Gann2DayTradeData newTrade = new Gann2DayTradeData(_identifier,
                                                                                           index,
                                                                                           nextDate,
                                                                                           tradePrice,
                                                                                           reason,
                                                                                           additionalReason,
                                                                                           positionSize,
                                                                                           TradingEnumerations.TradeStatus.Short,
                                                                                           exitStop,
                                                                                           null,
                                                                                           null,
                                                                                           TradingEnumerations.GannTradeReason.NotExitedYet);
                                        _currentTrades.Add(newTrade);
                                        _identifier++;
                                        _pyramidSellTrades.Add(new LineData(nextDate, tradePrice));
                                    }
                                }
                            }
                        }
                        // Check if we are stopped out or not
                        TradingEnumerations.GannTradeTypesInList tradeTypes = TradeTypes(_currentTrades);
                        if (tradeTypes != TradingEnumerations.GannTradeTypesInList.None)
                        {
                            if (tradeTypes != TradingEnumerations.GannTradeTypesInList.Mixture)
                            {
                                if (tradeTypes == TradingEnumerations.GannTradeTypesInList.ShortOnly)
                                {
                                    // Each trade can have a different stop. So we must loop through to see if any are stopped out.
                                    List<Gann2DayTradeData> stoppedOutList = new List<Gann2DayTradeData>();
                                    List<Gann2DayTradeData> stillCurrentList = new List<Gann2DayTradeData>();
                                    foreach (Gann2DayTradeData trade in _currentTrades)
                                    {
                                        if (trade.ExitStop <= nextHigh)
                                        {
                                            // We are stopped out
                                            tradePrice = trade.ExitStop + SkidPoints;
                                            if (!((tradePrice >= nextLow) && (tradePrice <= nextHigh)))
                                            {
                                                tradePrice = nextHigh;
                                            }
                                            trade.ExitDate = nextDate;
                                            trade.ExitPrice = tradePrice;
                                            trade.ExitReason = TradingEnumerations.GannTradeReason.ExitStopHit;
                                            stoppedOutList.Add(trade);
                                            if (_writeLogFile)
                                            {
                                                _tradeLog.WriteLine(trade.Identifier.ToString() + ", " +
                                                                    trade.TopBottomIdentifier.ToString() + ", " +
                                                                    trade.EntryDate.ToString("yyyyMMdd") + ", " +
                                                                    trade.EntryPrice.ToString() + ", " +
                                                                    trade.EntryReason.ToString() + ", " +
                                                                    trade.EntryAdditionalReason.ToString() + ", " +
                                                                    trade.EntryType.ToString() + ", " +
                                                                    trade.Contracts.ToString() + ", " +
                                                                    trade.ExitStop.ToString() + ", " +
                                                                    ((DateTime)trade.ExitDate).ToString("yyyyMMdd") + ", " +
                                                                    trade.ExitPrice.ToString() + " ," +
                                                                    trade.ExitReason.ToString() + ", " +
                                                                    (trade.CalculateProfitPoints() * DollarsPerPoint).ToString());
                                            }
                                            _stoppedOutTrades.Add(new LineData((DateTime)trade.ExitDate, (decimal)trade.ExitPrice));
                                        }
                                        else
                                        {
                                            stillCurrentList.Add(trade);
                                        }
                                    }
                                    _currentTrades = stillCurrentList;
                                    // Calculate the profit etc.
                                    decimal profit = CalculateProfit(stoppedOutList, tradePrice, DollarsPerPoint);
                                    _profitLoss += profit;
                                    _equity += profit;
                                    _closingBalance = _equity;
                                }
                                else if (tradeTypes == TradingEnumerations.GannTradeTypesInList.LongOnly)
                                {
                                    // Each trade can have a different stop. So we must loop through to see if any are stopped out.
                                    List<Gann2DayTradeData> stoppedOutList = new List<Gann2DayTradeData>();
                                    List<Gann2DayTradeData> stillCurrentList = new List<Gann2DayTradeData>();
                                    foreach (Gann2DayTradeData trade in _currentTrades)
                                    {
                                        if (trade.ExitStop >= nextLow)
                                        {
                                            // We are stopped out
                                            tradePrice = trade.ExitStop - SkidPoints;
                                            if (!((tradePrice >= nextLow) && (tradePrice <= nextHigh)))
                                            {
                                                tradePrice = nextLow;
                                            }
                                            trade.ExitDate = nextDate;
                                            trade.ExitPrice = tradePrice;
                                            trade.ExitReason = TradingEnumerations.GannTradeReason.ExitStopHit;
                                            stoppedOutList.Add(trade);
                                            if (_writeLogFile)
                                            {
                                                _tradeLog.WriteLine(trade.Identifier.ToString() + ", " +
                                                                    trade.TopBottomIdentifier.ToString() + ", " +
                                                                    trade.EntryDate.ToString("yyyyMMdd") + ", " +
                                                                    trade.EntryPrice.ToString() + ", " +
                                                                    trade.EntryReason.ToString() + ", " +
                                                                    trade.EntryAdditionalReason.ToString() + ", " +
                                                                    trade.EntryType.ToString() + ", " +
                                                                    trade.Contracts.ToString() + ", " +
                                                                    trade.ExitStop.ToString() + ", " +
                                                                    ((DateTime)trade.ExitDate).ToString("yyyyMMdd") + ", " +
                                                                    trade.ExitPrice.ToString() + " ," +
                                                                    trade.ExitReason.ToString() + ", " +
                                                                    (trade.CalculateProfitPoints() * DollarsPerPoint).ToString());
                                            }
                                            _stoppedOutTrades.Add(new LineData((DateTime)trade.ExitDate, (decimal)trade.ExitPrice));
                                        }
                                        else
                                        {
                                            stillCurrentList.Add(trade);
                                        }
                                    }
                                    _currentTrades = stillCurrentList;
                                    // Calculate the profit etc.
                                    decimal profit = CalculateProfit(stoppedOutList, tradePrice, DollarsPerPoint);
                                    _profitLoss += profit;
                                    _equity += profit;
                                    _closingBalance = _equity;
                                }
                            }
                            else
                            {
                                _metricsLog.WriteLine("Checking for stop out. There was a mixture of trade types in the list.");
                            }
                        }
                        // The following should happen after all trade information has been finalised
                        if (_currentTrades.Count > 0)
                        {
                            // If we are in a trade
                            // Update the current equity
                            _openProfit = CalculateProfit(_currentTrades, nextClose, DollarsPerPoint);
                            _currentEquity = _closingBalance + _openProfit;
                        }
                        else
                        {
                            // We are not in a trade
                            _openProfit = 0.0M;
                            _currentEquity = _closingBalance;
                        }
                        // Recalculate the trend
                        // The minor trend
                        if (_minorTrend == TradingEnumerations.Trend.Up)
                        {
                            if (nextHigh > high)
                            {
                                // Mod. to Ganns rule. If we have higher highs and trend is Up, keep going Up
                                _minorTrend = TradingEnumerations.Trend.Up;
                            }
                            else if ((nextLow < low) && (low < prevLow))
                            {
                                // We need two consecutive lower days to change the trend to Down
                                _minorTrend = TradingEnumerations.Trend.Down;
                                // So we have had a top recently
                                // Go from i to the bottom (which must be the previous point in the _gann2DaySwingList)
                                // and find the point and add it to the list.
                                // We only look at the current date (actually nextDate) and dates for which the trend is UP
                                DateTime highDate = nextDate;
                                decimal highValue = nextHigh;
                                int highIndex = i;
                                for (int j = i; j >= _gann2DaySwingList[_gann2DaySwingList.Count-1].Index; j--)
                                {
                                    if ((barDataReader.OHLC[j].MinorTrend == TradingEnumerations.Trend.Up) && (barDataReader.OHLC[j].High > highValue))
                                    {
                                        highValue = barDataReader.OHLC[j].High;
                                        highDate = barDataReader.OHLC[j].Date;
                                        highIndex = j;
                                    }
                                }
                                _gann2DaySwingList.Add(new LineData(highDate, highValue, highIndex, TradingEnumerations.TopBottom.Top));
                                lowestLowSinceMinorTrendChange = nextLow;
                                highestHighSinceMinorTrendChange = nextHigh;
                                lowAtMinorTrendChangeToDown = nextLow;
                                if (tradeTypes != TradingEnumerations.GannTradeTypesInList.None)
                                {
                                    if (tradeTypes != TradingEnumerations.GannTradeTypesInList.Mixture)
                                    {
                                        if (tradeTypes == TradingEnumerations.GannTradeTypesInList.ShortOnly)
                                        {
                                            // Update the exit stop for all trades in the list
                                            foreach (Gann2DayTradeData trade in _currentTrades)
                                            {
                                                if (ExitStopATR)
                                                {
                                                    trade.ExitStop = highValue + ExitStop * _atr;
                                                }
                                                else
                                                {
                                                    trade.ExitStop = highValue + ExitStop;
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        _metricsLog.WriteLine("Updating exit stop. There was a mixture of trade types in the list.");
                                    }
                                }
                                // If we have had our first minor trend change since the major trend changed,
                                // reset the major trend just changed flag
                                if (_majorTrendJustchanged)
                                {
                                    _majorTrendJustchanged = false;
                                }
                            }
                            else
                            {
                                _minorTrend = TradingEnumerations.Trend.Up;  // The minor Trend remains UP
                            }
                        }
                        else if (_minorTrend == TradingEnumerations.Trend.Down)
                        {
                            if (nextLow < low)
                            {
                                // Mod. to Ganns rule. If we have lower lows and trend is Down, keep going Down
                                _minorTrend = TradingEnumerations.Trend.Down;
                            }
                            if ((nextHigh >= high) && (high >= prevHigh) && (nextLow >= low) && (low >= prevLow))
                            {
                                // We need two consecutive higher high and higher low days to change the trend to Up
                                _minorTrend = TradingEnumerations.Trend.Up;
                                // So we have had a bottom recently
                                // Go from i to the top (which must be the previous point in the _gann2DaySwingList)
                                // and find the point and add it to the list.
                                // We only look at the currrent date (actually nextDate) and dates for which the trend is DOWN
                                DateTime lowDate = nextDate;
                                decimal lowValue = nextLow;
                                int lowIndex = i;
                                for (int j = i; j >= _gann2DaySwingList[_gann2DaySwingList.Count - 1].Index; j--)
                                {
                                    if ((barDataReader.OHLC[j].MinorTrend == TradingEnumerations.Trend.Down) && (barDataReader.OHLC[j].Low < lowValue))
                                    {
                                        lowValue = barDataReader.OHLC[j].Low;
                                        lowDate = barDataReader.OHLC[j].Date;
                                        lowIndex = j;
                                    }
                                }
                                _gann2DaySwingList.Add(new LineData(lowDate, lowValue, lowIndex, TradingEnumerations.TopBottom.Bottom));
                                lowestLowSinceMinorTrendChange = nextLow;
                                highestHighSinceMinorTrendChange = nextHigh;
                                highAtMinorTrendChangeToUp = nextHigh;
                                if (tradeTypes != TradingEnumerations.GannTradeTypesInList.None)
                                {
                                    if (tradeTypes != TradingEnumerations.GannTradeTypesInList.Mixture)
                                    {
                                        if (tradeTypes == TradingEnumerations.GannTradeTypesInList.LongOnly)
                                        {
                                            // Update the exit stop for all trades in the list
                                            foreach (Gann2DayTradeData trade in _currentTrades)
                                            {
                                                if (ExitStopATR)
                                                {
                                                    trade.ExitStop = lowValue - ExitStop * _atr;
                                                }
                                                else
                                                {
                                                    trade.ExitStop = lowValue - ExitStop;
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        _metricsLog.WriteLine("Updating exit stop. There was a mixture of trade types in the list.");
                                    }
                                }
                                // If we have had our first minor trend change since the major trend changed,
                                // reset the major trend just changed flag
                                if (_majorTrendJustchanged)
                                {
                                    _majorTrendJustchanged = false;
                                }
                            }
                            else
                            {
                                _minorTrend = TradingEnumerations.Trend.Down;  // The minor trend remains down
                            }
                        }
                        else if (_minorTrend == TradingEnumerations.Trend.Unknown)
                        {
                            if ((nextHigh >= high) && (high >= prevHigh) && (nextLow >= low) && (low >= prevLow))
                            {
                                _minorTrend = TradingEnumerations.Trend.Up;  // The minor trend has changed to UP
                                // Go from i to the start of the list and find the lowest 
                                // point and add it to the list. This is the first low
                                // Note: we should start from a top or bottom
                                DateTime lowDate = DateTime.MaxValue;
                                decimal lowValue = decimal.MaxValue;
                                int lowIndex = -1;
                                for (int j = i; j >= 0; j--)
                                {
                                    if (barDataReader.OHLC[j].Low < lowValue)
                                    {
                                        lowValue = barDataReader.OHLC[j].Low;
                                        lowDate = barDataReader.OHLC[j].Date;
                                        lowIndex = j;
                                    }
                                }
                                _gann2DaySwingList.Add(new LineData(lowDate, lowValue, lowIndex, TradingEnumerations.TopBottom.Bottom));
                                lowestLowSinceMinorTrendChange = nextLow;
                                highestHighSinceMinorTrendChange = nextHigh;
                                highAtMinorTrendChangeToUp = nextHigh;
                            }
                            else if ((nextLow < low) && (low < prevLow))
                            {
                                _minorTrend = TradingEnumerations.Trend.Down;  // The minor trend has changed to DOWN
                                // Go from i to the start of the list and find the highest 
                                // point and add it to the list. This is the first high
                                // Note: we should start from a top or bottom
                                DateTime highDate = DateTime.MinValue;
                                decimal highValue = decimal.MinValue;
                                int highIndex = -1;
                                for (int j = i; j >= 0; j--)
                                {
                                    if (barDataReader.OHLC[j].High > highValue)
                                    {
                                        highValue = barDataReader.OHLC[j].High;
                                        highDate = barDataReader.OHLC[j].Date;
                                        highIndex = j;
                                    }
                                }
                                _gann2DaySwingList.Add(new LineData(highDate, highValue, highIndex, TradingEnumerations.TopBottom.Top));
                                lowestLowSinceMinorTrendChange = nextLow;
                                highestHighSinceMinorTrendChange = nextHigh;
                                lowAtMinorTrendChangeToDown = nextLow;
                            }                                 
                        }
                        if (nextLow < lowestLowSinceMinorTrendChange)
                        {
                            lowestLowSinceMinorTrendChange = nextLow;
                        }
                        if (nextHigh > highestHighSinceMinorTrendChange)
                        {
                            highestHighSinceMinorTrendChange = nextHigh;
                        }
                        // Initialise the major trend, if possible
                        if (_gann2DaySwingList.Count == 3)
                        {
                            if ((_gann2DaySwingList[_gann2DaySwingList.Count - 3].TopBottom == TradingEnumerations.TopBottom.Bottom) && (_gann2DaySwingList[_gann2DaySwingList.Count - 1].TopBottom == TradingEnumerations.TopBottom.Bottom))
                            {
                                if (_gann2DaySwingList[_gann2DaySwingList.Count - 1].Price >= _gann2DaySwingList[_gann2DaySwingList.Count - 3].Price)
                                {
                                    _majorTrend = TradingEnumerations.Trend.Up;
                                }
                                else
                                {
                                    _majorTrend = TradingEnumerations.Trend.Down;
                                }
                                _majorTrendInitialised = true;
                            }
                            if ((_gann2DaySwingList[_gann2DaySwingList.Count - 3].TopBottom == TradingEnumerations.TopBottom.Top) && (_gann2DaySwingList[_gann2DaySwingList.Count - 1].TopBottom == TradingEnumerations.TopBottom.Top))
                            {
                                if (_gann2DaySwingList[_gann2DaySwingList.Count - 1].Price < _gann2DaySwingList[_gann2DaySwingList.Count - 3].Price)
                                {
                                    _majorTrend = TradingEnumerations.Trend.Down;
                                }
                                else
                                {
                                    _majorTrend = TradingEnumerations.Trend.Up;
                                }
                                _majorTrendInitialised = true;
                            }
                        }
                        _majorTrendChangeToUp = -1;
                        _majorTrendChangeToDown = -1;
                        if (_majorTrendInitialised)
                        {
                            if (_majorTrend == TradingEnumerations.Trend.Up)
                            {
                                if (_gann2DaySwingList[_gann2DaySwingList.Count - 2].TopBottom == TradingEnumerations.TopBottom.Bottom)
                                {
                                    // Note: Minor Trend must be Down anyway or we wouldn't have a Bottom at position -2 
                                    if ((_minorTrend == TradingEnumerations.Trend.Down) &&
                                        (nextLow < _gann2DaySwingList[_gann2DaySwingList.Count - 2].Price))
                                    {
                                        _majorTrend = TradingEnumerations.Trend.Down;
                                        _majorTrendChangeToDown = _gann2DaySwingList.Count - 1;
                                        _trendChangePrice = _gann2DaySwingList[_gann2DaySwingList.Count - 2].Price;
                                        _majorTrendJustchanged = true;
                                    }
                                }
                            }
                            else if (_majorTrend == TradingEnumerations.Trend.Down)
                            {
                                if (_gann2DaySwingList[_gann2DaySwingList.Count - 2].TopBottom == TradingEnumerations.TopBottom.Top)
                                {
                                    // Note: Minor Trend must be down Up anyway or we wouldn't have a Top at position -2 
                                    if ((_minorTrend == TradingEnumerations.Trend.Up) &&
                                        (nextHigh > _gann2DaySwingList[_gann2DaySwingList.Count - 2].Price))
                                    {
                                        _majorTrend = TradingEnumerations.Trend.Up;
                                        _majorTrendChangeToUp = _gann2DaySwingList.Count - 1;
                                        _trendChangePrice = _gann2DaySwingList[_gann2DaySwingList.Count - 2].Price;
                                        _majorTrendJustchanged = true;
                                    }
                                }
                            }
                            else
                            {
                                if (_writeLogFile)
                                {
                                    _metricsLog.WriteLine("Error: Major Trend is initialised but NEITHER Up NOR Down, date: " + nextDate.ToString("yyyyMMdd"));
                                }
                            }
                        }
                        // Add the data to the list and Re-calculate ATR
                        _ohlc.Add(barDataReader.OHLC[i]);
                        //
                        // Calculate the true range
                        // which is the maximum of 
                        // High-Low, High-Yesterday Close, Yesterday Close-Low
                        //
                        _prevAtr = _atr;
                        _trueRange = Math.Max(nextHigh - nextLow, nextHigh - close);
                        _trueRange = Math.Max(_trueRange, close - nextLow);
                        //
                        // Calculate the atr
                        //
                        _atr = _prevAtr + (_trueRange - _prevAtr) / _atrLagNormalised;
                        _ohlc[_ohlc.Count - 1].MajorTrend = _majorTrend;
                        _ohlc[_ohlc.Count - 1].MinorTrend = _minorTrend;
                        // Update the values for the next pass through
                        prevDate = date;
                        prevOpen = open;
                        prevHigh = high;
                        prevLow = low;
                        prevClose = close;
                        date = nextDate;
                        open = nextOpen;
                        high = nextHigh;
                        low = nextLow;
                        close = nextClose;
                    }
                    // We have reached the end of the loop as there is no more data.
                    // If we are in a trade we need to close it.
                    if (_currentTrades.Count > 0)
                    {
                        // The exit is different.
                        // The system exits the final trade at the final closing price
                        decimal profit = CalculateProfit(_currentTrades, nextClose, DollarsPerPoint);
                        _profitLoss += profit;
                        _equity += profit;
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
                        // We have closed out the trade so there is no open profit
                        _openProfit = 0.0M;
                        foreach (Gann2DayTradeData trade in _currentTrades)
                        {
                            trade.ExitDate = nextDate;
                            trade.ExitPrice = nextClose;
                            if (_writeLogFile)
                            {
                                _tradeLog.WriteLine(trade.Identifier.ToString() + ", " +
                                                    trade.TopBottomIdentifier.ToString() + ", " +
                                                    trade.EntryDate.ToString("yyyyMMdd") + ", " +
                                                    trade.EntryPrice.ToString() + ", " +
                                                    trade.EntryReason.ToString() + ", " +
                                                    trade.EntryAdditionalReason.ToString() + ", " +
                                                    trade.EntryType.ToString() + ", " +
                                                    trade.Contracts.ToString() + ", " +
                                                    trade.ExitStop.ToString() + ", " +
                                                    nextDate.ToString("yyyyMMdd") + ", " +
                                                    trade.ExitPrice.ToString() + " ," +
                                                    TradingEnumerations.GannTradeReason.DefinateMainTrendChangeToUp.ToString() + ", " +
                                                    (trade.CalculateProfitPoints() * DollarsPerPoint).ToString());
                            }
                        }
                        if (_writeLogFile)
                        {
                            _tradeLog.WriteLine("Total Profit and Loss = " + _profitLoss.ToString());
                            _metricsLog.WriteLine(date.ToString("yyyyMMdd") + ", " +
                                                  open.ToString() + ", " +
                                                  high.ToString() + ", " +
                                                  low.ToString() + ",  " +
                                                  close.ToString() + ", " +
                                                  _minorTrend.ToString() + ", " +
                                                  _majorTrend.ToString() + ", " +
                                                  _majorTrendChangeToUp.ToString() + ", " +
                                                  _majorTrendChangeToDown.ToString() + ", " +
                                                  _trendChangePrice.ToString() + ", " +
                                                  _majorTrendJustchanged.ToString() + ", " +
                                                  _ohlc[_ohlc.Count - 1].ContractRollover.ToString());
                            _equityLog.WriteLine(date.ToString("yyyyMMdd") + ", " +
                                                 _closingBalance.ToString() + ", " +
                                                 _openProfit.ToString() + ", " +
                                                 _currentEquity.ToString() + ", " +
                                                 _projectedEquity.ToString() + ", " +
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
                            _tradeLog.WriteLine("Total Profit and Loss = " + _profitLoss.ToString());
                            _equityLog.WriteLine(date.ToShortDateString() + ", " +
                                                 _closingBalance.ToString() + ", " +
                                                 _openProfit.ToString() + ", " +
                                                 _currentEquity.ToString() + ", " +
                                                 _projectedEquity.ToString() + ", " +
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
        // Calculate the profit and loss or the open profit
        private decimal CalculateProfit(List<Gann2DayTradeData> tradeList, decimal price, decimal dollarsPerPoint)
        {
            decimal profit = 0.0M;
            if (tradeList.Count > 0)
            {
                foreach (Gann2DayTradeData trade in tradeList)
                {
                    if (trade.EntryType == TradingEnumerations.TradeStatus.Long)
                    {
                        profit += (price - trade.EntryPrice) * (decimal)trade.Contracts;
                    }
                    else if (trade.EntryType == TradingEnumerations.TradeStatus.Short)
                    {
                        profit += (trade.EntryPrice - price) * (decimal)trade.Contracts;
                    }
                }
            }
            return profit * dollarsPerPoint;
        }
        // Determine the trade types in the list
        private TradingEnumerations.GannTradeTypesInList TradeTypes(List<Gann2DayTradeData> tradeList)
        {
            TradingEnumerations.GannTradeTypesInList tradeTypes = TradingEnumerations.GannTradeTypesInList.None;
            int shortCount = 0;
            int longCount = 0;
            if ((tradeList != null) && (tradeList.Count > 0))
            {
                foreach (Gann2DayTradeData trade in tradeList)
                {
                    if (trade.EntryType == TradingEnumerations.TradeStatus.Long)
                    {
                        longCount++;
                    }
                    else if (trade.EntryType == TradingEnumerations.TradeStatus.Short)
                    {
                        shortCount++;
                    }
                }
                if ((longCount > 0) && (shortCount > 0))
                {
                    tradeTypes = TradingEnumerations.GannTradeTypesInList.Mixture;
                }
                else if ((longCount > 0) && (shortCount == 0))
                {
                    tradeTypes = TradingEnumerations.GannTradeTypesInList.LongOnly;
                }
                else if ((shortCount > 0) && (longCount == 0))
                {
                    tradeTypes = TradingEnumerations.GannTradeTypesInList.ShortOnly;
                }
            }
            return tradeTypes;
        }
        // Determine if this top/bottom is already in the trade list
        private bool TopBottomAlreadyInTradeList(int topBottomIdentifer, List<Gann2DayTradeData> tradeList, TradingEnumerations.GannTradeReason entryReason)
        {
            bool alreadyInList = false;
            if (tradeList.Count > 0)
            {
                foreach (Gann2DayTradeData trade in tradeList)
                {
                    if ((trade.TopBottomIdentifier == topBottomIdentifer) && (trade.EntryReason == entryReason))
                    {
                        alreadyInList = true;
                        break;
                    }
                }
            }
            return alreadyInList;
        }
        #endregion
    }
}
