// Copyright (c) 2012, Adam Duval Kangaroo Point South 4169, Australia.
// All Rights Reserved.
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Adam.Trading.Mechanical.Model
{
    public class KrauseGannCalculator
    {
        #region Constants
        private const int ATRDAYS = 20;  // The average true range (atr) is calculated over the last 20 trading days
        #endregion
        #region Class private variables
        private KrauseTradeData _currentTrade;                                 // The current trade (we only support one)
        private List<LineData> _majorUpBuyTrades = new List<LineData>();       // The list of all Major trend change to Up buy trades
        private List<LineData> _majorDownSellTrades = new List<LineData>();    // The list of all Major trend change to Down sell trades
        private int _majorTrendChangeToUp;                                     // -1 or Top/Bottom Index if major trend has changed to Up
        private int _majorTrendChangeToDown;                                   // -1 or Top/Bottom Index if major trend has changed to Down
        private bool _majorTrendJustchanged;                                   // True if we have had a major trend change but no minor trend change yet
        private decimal _trendChangePrice;                                     // The price at which the major trend (above) occurred.
        private List<LineData> _profitP1Trades = new List<LineData>();         // The list of all profit protection #1 trades
        private List<LineData> _profitP2Trades = new List<LineData>();         // The list of all profit protection #2 trades
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
        private decimal _benchmarkReturn;
        private TradingEnumerations.Trend _majorTrend = TradingEnumerations.Trend.Unknown;          // The Major Trend based on the Krause 2 Day Swing Chart
        private TradingEnumerations.Trend _minorTrend = TradingEnumerations.Trend.Unknown;          // The Minor Trend based on the Krause 2 Day Swing chart
        private StringBuilder _tradeLogMessage;                                                     // The message to write to the trade log file
        private StreamWriter _tradeLog;                                                             // The trade log file
        private StreamWriter _equityLog;                                                            // The equity log file
        private StreamWriter _metricsLog;                                                           // The metrics log file
        private List<LineData> _krauseSwingList = new List<LineData>();                             // The list of Krause 2 Day swing points
        private bool _writeLogFile = true;                                                          // Write to a log file or not
        private List<BarData> _ohlc = new List<BarData>();                                          // The Open, High, Low, Close data from the input file
        private bool _majorTrendInitialised = false;                                                // The major trend has been initialised
        private uint _identifier;                                                                   // The unique trade identifier. Incremented when a trade is opened
        private enum LocalTradeType
        {
            None,
            Retracement,
            Trend,
            HiloTakenOut,
            TrendChange
        }
        private decimal _trueRange;           // The current true range
        private decimal _atr;                 // The average true range (ATR)
        private decimal _prevAtr;             // The previous avreage true range
        private decimal _atrLagNormalised;    // The lag for ATR calculation = (System.Convert.ToInt32(atrLag + 1)) * 0.5M;
        private uint _noPositions;            // The number of contracts to trade
        private decimal _nextSellStop;        // The Krause Sell Stop for the next day
        private decimal _nextBuyStop;         // The Krause Buy Stop for the next day
        private decimal _sellStop;            // The sell stop for the previous day
        private decimal _buyStop;             // The buy stop for the previous day
        private bool _nextSellStopActive = true;      // The sell stop is active for the next day
        private List<LineData> _sellStops = new List<LineData>();  // The list of all sell stops
        private List<LineData> _buyStops = new List<LineData>();   // The list of all buy stops
        #endregion

        #region Accessor Methods
        /// <summary>
        /// The Entry Stop for Retracement
        /// </summary>
        public decimal EntryStopRetracement { get; private set; }

        /// <summary>
        /// The Entry Stop for Exceed Hi Lo
        /// </summary>
        public decimal EntryStopExceedHiLo { get; private set; }

        /// <summary>
        /// The Retracement Fraction for Profit Protection Rule #2
        /// </summary>
        public decimal RetracementFraction { get; private set; }

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
            get { return _ohlc; }
        }

        public List<LineData> KrauseSwingList
        {
            get { return _krauseSwingList; }
        }

        public List<LineData> MajorUpBuyTrades
        {
            get { return _majorUpBuyTrades; }
        }

        public List<LineData> MajorDownSellTrades
        {
            get { return _majorDownSellTrades; }
        }

        public List<LineData> ProfitP1Trades
        {
            get { return _profitP1Trades; }
        }

        public List<LineData> ProfitP2Trades
        {
            get { return _profitP2Trades; }
        }

        public List<LineData> PyramidBuyTrades
        {
            get { return _pyramidBuyTrades; }
        }

        public List<LineData> PyramidSellTrades
        {
            get { return _pyramidSellTrades; }
        }

        public List<LineData> BuyStops
        {
            get { return _buyStops; }
        }

        public List<LineData> SellStops
        {
            get { return _sellStops; }
        }
        #endregion

        #region The Contructor
        /// <summary>
        /// The constructor
        /// </summary>
        /// <param name="entryStopRetracement">The Entry Stop for Retracement.</param>
        /// <param name="entryStopExceedHiLo">The Entry Stop for Exceed Hi Lo.</param>
        /// <param name="retracementFraction">The Retracement Fraction for Profit Protection Rule #2</param>
        /// <param name="dataFile">The Data File name.</param>
        /// <param name="dollarsPerPoint">The Dollars Per Point.</param>
        /// <param name="skidPoints">The Skid Points for Buy/Sell orders.</param>
        /// <param name="startingEquity">The Starting Equity.</param>
        /// <param name="benchmarkReturn">The Benchmark return to calculate Sharpe ratio</param>
        /// <param name="writeLogFile">True to output to the metrics log.</param>
        public KrauseGannCalculator(decimal entryStopRetracement,
                                    decimal entryStopExceedHiLo,
                                    decimal retracementFraction,
                                    string dataFile,
                                    decimal dollarsPerPoint,
                                    decimal skidPoints,
                                    decimal startingEquity,
                                    decimal benchmarkReturn,
                                    bool writeLogFile)
        {
            EntryStopRetracement = entryStopRetracement;
            EntryStopExceedHiLo = entryStopExceedHiLo;
            RetracementFraction = retracementFraction;
            DataFile = dataFile;
            DollarsPerPoint = dollarsPerPoint;
            SkidPoints = skidPoints;
            _profitLoss = 0.0M;
            _closingBalance = startingEquity;
            _openProfit = 0.0M;
            _equity = startingEquity;
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
            _noPositions = (uint)(_initialEquity / 10000.0M);
            _nextSellStopActive = true;
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
            _krauseSwingList = new List<LineData>();
            _majorUpBuyTrades = new List<LineData>();
            _majorDownSellTrades = new List<LineData>();
            _profitP1Trades = new List<LineData>();
            _profitP2Trades = new List<LineData>();
            _pyramidBuyTrades = new List<LineData>();
            _pyramidSellTrades = new List<LineData>();
            _sellStops = new List<LineData>();
            _buyStops = new List<LineData>();
            _identifier = 0;
            _majorTrendInitialised = false;
            _majorTrendChangeToUp = -1;
            _majorTrendChangeToDown = -1;
            _majorTrendJustchanged = false;
            _nextSellStopActive = true;
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
                    _currentTrade = new KrauseTradeData();
                    // Initialise the trade price
                    decimal tradePrice = 0.0M;
                    // The highest high since a minor trend change and lowest low since a minor trend change
                    decimal lowestLowSinceMinorTrendChange = 0.0M;
                    decimal highestHighSinceMinorTrendChange = 0.0M;
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
                        _metricsLog.WriteLine("Date, Open, High, Low, Close, Minor Trend, Major Trend, Major To Up, Major To Down, Price for Major Change, Major Trend Just Changed, ATR, Contract Rollover Day, Sell Stop, Buy Stop");
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
                        _tradeLog.WriteLine("Identifier, Top/Bottom Identifier, Entry Date, Entry Price, Entry Reason, Entry Additional Reason, Long/Short, No Contracts, Exit Date, Exit Price, Exit Reason, Profit/Loss");
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
                                                  _ohlc[_ohlc.Count - 1].ContractRollover.ToString() + ", " +
                                                  _nextSellStop.ToString() + ", " +
                                                  _nextBuyStop.ToString());
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
                        if (i > 2)
                        {
                            if (nextClose < (_nextSellStop - EntryStopRetracement))
                            {
                                _nextSellStopActive = false;
                            }
                            else if (nextClose > (_nextBuyStop + EntryStopRetracement))
                            {
                                _nextSellStopActive = true;
                            }
                            TimeSpan dateDiffBefore;
                            DateTime averageBeforeDate;
                            TimeSpan dateDiffAfter;
                            DateTime averageAfterDate;
                            dateDiffBefore = nextDate.Subtract(date);
                            averageBeforeDate = date.AddDays(dateDiffBefore.TotalDays * 0.5);
                            if ((i + 1) < barDataReader.OHLC.Count)
                            {
                                dateDiffAfter = barDataReader.OHLC[i + 1].Date.Subtract(nextDate);
                                averageAfterDate = nextDate.AddDays(dateDiffAfter.TotalDays * 0.5);
                            }
                            else
                            {
                                averageAfterDate = nextDate;
                                dateDiffAfter = nextDate.Subtract(nextDate);
                            }
                            bool buyLineSegmentActive = true;
                            bool sellLineSegmentActive = true;
                            bool prevBuyLineSegmentActive = true;
                            bool prevSellLineSegmentActive = true;
                            if (!_nextSellStopActive)
                            {
                                // The buy stop is active so highlight it
                                buyLineSegmentActive = true;
                                sellLineSegmentActive = false;
                                prevBuyLineSegmentActive = false;
                                prevSellLineSegmentActive = false;
                            }
                            else if (_nextSellStopActive)
                            {
                                // The sell stop is active so highlight it
                                buyLineSegmentActive = false;
                                sellLineSegmentActive = true;
                                prevBuyLineSegmentActive = false;
                                prevSellLineSegmentActive = false;
                            }
                            LineData beforeSellStop = new LineData(averageBeforeDate, _nextSellStop);
                            LineData beforeBuyStop = new LineData(averageBeforeDate, _nextBuyStop);
                            beforeSellStop.LastLineSegmentActive = prevSellLineSegmentActive;
                            beforeBuyStop.LastLineSegmentActive = prevBuyLineSegmentActive;
                            LineData afterSellStop = new LineData(averageAfterDate, _nextSellStop);
                            LineData afterBuyStop = new LineData(averageAfterDate, _nextBuyStop);
                            afterSellStop.LastLineSegmentActive = sellLineSegmentActive;
                            afterBuyStop.LastLineSegmentActive = buyLineSegmentActive;
                            _sellStops.Add(beforeSellStop);
                            _buyStops.Add(beforeBuyStop);
                            _sellStops.Add(afterSellStop);
                            _buyStops.Add(afterBuyStop);
                        }
                        TradingEnumerations.KrauseTradeType tradeType = TradeType(_currentTrade);
                        bool newTradeToday = false;
                        if (_majorTrendInitialised)
                        {
                            bool trendChangeUpPossible = false;
                            decimal trendChangeUpEntryStop = 0.0M;
                            decimal trendChangeUpTradePrice = 0.0M;
                            int trendChangeUpIndex = 0;
                            bool trendChangeDownPossible = false;
                            decimal trendChangeDownEntryStop = 0.0M;
                            decimal trendChangeDownTradePrice = 0.0M;
                            int trendChangeDownIndex = 0;
                            bool retracementPyramidPossible = false;
                            decimal retracementPyramidEntryStop = 0.0M;
                            decimal retracementPyramidTradePrice = 0.0M;
                            int retracementPyramidIndex = 0;
                            bool exceedHiPyramidPossible = false;
                            decimal exceedHiPyramidEntryStop = 0.0M;
                            decimal exceedHiPyramidTradePrice = 0.0M;
                            int exceedHiPyramidIndex = 0;
                            bool exceedLoPyramidPossible = false;
                            decimal exceedLoPyramidEntryStop = 0.0M;
                            decimal exceedLoPyramidTradePrice = 0.0M;
                            int exceedLoPyramidIndex = 0;
                            uint option = 0;
                            if (_majorTrend == TradingEnumerations.Trend.Down)
                            {
                                // Main trend change to up trade
                                if ((_minorTrend == TradingEnumerations.Trend.Down) &&
                                    (nextHigh > high) &&
                                    (high > prevHigh) &&
                                    (nextHigh > _krauseSwingList[_krauseSwingList.Count - 1].Price) &&
                                    (i > 2) &&
                                    (_nextSellStopActive) &&
                                    (!TopBottomAlreadyInTradeList(_krauseSwingList.Count - 1, _currentTrade, TradingEnumerations.KrauseTradeReason.DefinateMainTrendChangeToUp)))
                                {
                                    option = 1;
                                }
                                else if ((_minorTrend == TradingEnumerations.Trend.Down) &&
                                         (nextHigh > high) &&
                                         (nextHigh > _krauseSwingList[_krauseSwingList.Count - 1].Price) &&
                                         (i > 2) &&
                                         (_nextSellStopActive) &&
                                         (!TopBottomAlreadyInTradeList(_krauseSwingList.Count - 1, _currentTrade, TradingEnumerations.KrauseTradeReason.DefinateMainTrendChangeToUp)))
                                {
                                    option = 2;
                                }
                                else if ((_minorTrend == TradingEnumerations.Trend.Up) &&
                                         (nextHigh > high) &&
                                         (nextHigh > _krauseSwingList[_krauseSwingList.Count - 2].Price) &&
                                         (i > 2) &&
                                         (_nextSellStopActive) &&
                                         (!TopBottomAlreadyInTradeList(_krauseSwingList.Count - 2, _currentTrade, TradingEnumerations.KrauseTradeReason.DefinateMainTrendChangeToUp)))
                                {
                                    option = 3;
                                }
                                else if ((_majorTrendChangeToUp > -1) &&
                                         _majorTrendJustchanged &&
                                         (i > 2) &&
                                         (_nextSellStopActive) &&
                                         (!TopBottomAlreadyInTradeList(_majorTrendChangeToUp, _currentTrade, TradingEnumerations.KrauseTradeReason.DefinateMainTrendChangeToUp)))
                                {
                                    option = 4;
                                }
                                if (option > 0)
                                {
                                    trendChangeUpPossible = true;
                                    if (option == 3)
                                    {
                                        // The minor trend is up so _krauseSwingList.Count - 2 is the previous top
                                        trendChangeUpEntryStop = _krauseSwingList[_krauseSwingList.Count - 2].Price;
                                        trendChangeUpTradePrice = _krauseSwingList[_krauseSwingList.Count - 2].Price + SkidPoints;
                                        trendChangeUpIndex = _krauseSwingList.Count - 2;
                                    }
                                    else if ((option == 1) || (option == 2))
                                    {
                                        // The minor trend is down so _krauseSwingList.Count - 1 is the previous top
                                        trendChangeUpEntryStop = _krauseSwingList[_krauseSwingList.Count - 1].Price;
                                        trendChangeUpTradePrice = _krauseSwingList[_krauseSwingList.Count - 1].Price + SkidPoints;
                                        trendChangeUpIndex = _krauseSwingList.Count - 1;
                                    }
                                    else if (option == 4)
                                    {
                                        // _majorTrendChangeToUp is the previous top
                                        trendChangeUpEntryStop = _krauseSwingList[_majorTrendChangeToUp].Price;
                                        trendChangeUpTradePrice = _krauseSwingList[_majorTrendChangeToUp].Price + SkidPoints;
                                        trendChangeUpIndex = _majorTrendChangeToUp;
                                    }
                                    if (trendChangeUpTradePrice > nextHigh)
                                    {
                                        trendChangeUpTradePrice = nextHigh;
                                    }
                                }
                                // Retracement Pyramid Trade
                                if ((_minorTrend == TradingEnumerations.Trend.Up) &&
                                    // Note: -1 for last Bottom here!
                                     !TopBottomAlreadyInTradeList(_krauseSwingList.Count - 1, _currentTrade, TradingEnumerations.KrauseTradeReason.MainTrendDownPyramid))
                                {
                                    retracementPyramidEntryStop = _nextSellStop - EntryStopRetracement;
                                    if (nextClose < retracementPyramidEntryStop)
                                    {
                                        retracementPyramidPossible = true;
                                        retracementPyramidTradePrice = retracementPyramidEntryStop - SkidPoints;
                                        if (!((retracementPyramidTradePrice >= nextLow) && (retracementPyramidTradePrice <= nextHigh)))
                                        {
                                            retracementPyramidTradePrice = nextLow;
                                        }
                                        retracementPyramidIndex = _krauseSwingList.Count - 1;
                                    }
                                }
                                else if ((_minorTrend == TradingEnumerations.Trend.Down) &&
                                         (!_majorTrendJustchanged) &&
                                         // Note: -2 for last Bottom here! Because an extra top has been added as we have had a minor trend change to down.
                                         !TopBottomAlreadyInTradeList(_krauseSwingList.Count - 2, _currentTrade, TradingEnumerations.KrauseTradeReason.MainTrendDownPyramid))
                                {
                                    retracementPyramidEntryStop = _nextSellStop - EntryStopRetracement;
                                    if (nextClose < retracementPyramidEntryStop)
                                    {
                                        retracementPyramidPossible = true;
                                        retracementPyramidTradePrice = retracementPyramidEntryStop - SkidPoints;
                                        if (!((retracementPyramidTradePrice >= nextLow) && (retracementPyramidTradePrice <= nextHigh)))
                                        {
                                            retracementPyramidTradePrice = nextLow;
                                        }
                                        retracementPyramidIndex = _krauseSwingList.Count - 2;
                                    }
                                }
                                // Bottom taken out pyramid trade
                                if (_minorTrend == TradingEnumerations.Trend.Up &&
                                    // Note: -1 for last Bottom here!
                                    !TopBottomAlreadyInTradeList(_krauseSwingList.Count - 1, _currentTrade, TradingEnumerations.KrauseTradeReason.MainTrendDownPyramid))
                                {
                                    // We could enter a trade if the previous low is taken out
                                    exceedLoPyramidEntryStop = _krauseSwingList[_krauseSwingList.Count - 1].Price - EntryStopExceedHiLo;
                                    if ((nextLow < exceedLoPyramidEntryStop) && !_nextSellStopActive)
                                    {
                                        exceedLoPyramidPossible = true;
                                        exceedLoPyramidTradePrice = exceedLoPyramidEntryStop - SkidPoints;
                                        if (!((exceedLoPyramidTradePrice >= nextLow) && (exceedLoPyramidTradePrice <= nextHigh)))
                                        {
                                            exceedLoPyramidTradePrice = nextHigh;
                                        }
                                        exceedLoPyramidIndex = _krauseSwingList.Count - 1;
                                    }
                                }
                                else if ((_minorTrend == TradingEnumerations.Trend.Down) &&
                                         (!_majorTrendJustchanged) &&
                                         // Note: -2 for last Bottom here! Because an extra top has been added as we have had a minor trend change to down.
                                         !TopBottomAlreadyInTradeList(_krauseSwingList.Count - 2, _currentTrade, TradingEnumerations.KrauseTradeReason.MainTrendDownPyramid))
                                {
                                    // The minor trend has already changed to down and we have no trade for previous bottom
                                    exceedLoPyramidEntryStop = _krauseSwingList[_krauseSwingList.Count - 2].Price - EntryStopExceedHiLo;
                                    if ((nextLow < exceedLoPyramidEntryStop) && !_nextSellStopActive)
                                    {
                                        exceedLoPyramidPossible = true;
                                        exceedLoPyramidTradePrice = exceedLoPyramidEntryStop - SkidPoints;
                                        if (!((exceedLoPyramidTradePrice >= nextLow) && (exceedLoPyramidTradePrice <= nextHigh)))
                                        {
                                            exceedLoPyramidTradePrice = nextLow;
                                        }
                                        exceedLoPyramidIndex = _krauseSwingList.Count - 2;
                                    }
                                }
                            }
                            else if (_majorTrend == TradingEnumerations.Trend.Up)
                            {
                                // Main trend change to down trade
                                if ((_minorTrend == TradingEnumerations.Trend.Up) &&
                                    (nextLow < low) &&
                                    (low < prevLow) &&
                                    (nextLow < _krauseSwingList[_krauseSwingList.Count - 1].Price) &&
                                    (i > 2) &&
                                    (!_nextSellStopActive) &&
                                    (!TopBottomAlreadyInTradeList(_krauseSwingList.Count - 1, _currentTrade, TradingEnumerations.KrauseTradeReason.DefinateMainTrendChangeToDown)))
                                {
                                    option = 1;
                                }
                                else if ((_minorTrend == TradingEnumerations.Trend.Up) &&
                                         (nextLow < low) &&
                                         (nextLow < _krauseSwingList[_krauseSwingList.Count - 1].Price) &&
                                         (i > 2) &&
                                         (!_nextSellStopActive) &&
                                         (!TopBottomAlreadyInTradeList(_krauseSwingList.Count - 1, _currentTrade, TradingEnumerations.KrauseTradeReason.DefinateMainTrendChangeToDown)))
                                {
                                    option = 2;
                                }
                                else if ((_minorTrend == TradingEnumerations.Trend.Down) &&
                                         (nextLow < low) &&
                                         (nextLow < _krauseSwingList[_krauseSwingList.Count - 2].Price) &&
                                         (i > 2) &&
                                         (!_nextSellStopActive) &&
                                         (!TopBottomAlreadyInTradeList(_krauseSwingList.Count - 2, _currentTrade, TradingEnumerations.KrauseTradeReason.DefinateMainTrendChangeToDown)))
                                {
                                    option = 3;
                                }
                                else if ((_majorTrendChangeToDown > -1) && 
                                         _majorTrendJustchanged &&
                                         (i > 2) &&
                                         (!_nextSellStopActive) &&
                                         (!TopBottomAlreadyInTradeList(_majorTrendChangeToDown, _currentTrade, TradingEnumerations.KrauseTradeReason.DefinateMainTrendChangeToDown)))
                                {
                                    option = 4;
                                }
                                if (option > 0)
                                {
                                    trendChangeDownPossible = true;
                                    if (option == 3)
                                    {
                                        // The minor trend is down so _krauseSwingList.Count - 2 is the previous bottom
                                        trendChangeDownEntryStop = _krauseSwingList[_krauseSwingList.Count - 2].Price;
                                        trendChangeDownTradePrice = _krauseSwingList[_krauseSwingList.Count - 2].Price - SkidPoints;
                                        trendChangeDownIndex = _krauseSwingList.Count - 2;                                        
                                    }
                                    else if ((option == 1) || (option == 2))
                                    {
                                        // The minor trend is up so _krauseSwingList.Count - 1 is the previous bottom
                                        trendChangeDownEntryStop = _krauseSwingList[_krauseSwingList.Count - 1].Price;
                                        trendChangeDownTradePrice = _krauseSwingList[_krauseSwingList.Count - 1].Price - SkidPoints;
                                        trendChangeDownIndex = _krauseSwingList.Count - 1;                                        
                                    }
                                    else if (option == 4)
                                    {
                                        // _majorTrendChangeToDown is the previous bottom
                                        trendChangeDownEntryStop = _krauseSwingList[_majorTrendChangeToDown].Price;
                                        trendChangeDownTradePrice = _krauseSwingList[_majorTrendChangeToDown].Price - SkidPoints;
                                        trendChangeDownIndex = _majorTrendChangeToDown;
                                    }
                                    if (trendChangeDownTradePrice < nextLow)
                                    {
                                        trendChangeDownTradePrice = nextLow;
                                    }
                                }
                                // Retracement Pyramid Trade
                                if ((_minorTrend == TradingEnumerations.Trend.Down) &&
                                    // Note: -1 for last Top here!
                                    !TopBottomAlreadyInTradeList(_krauseSwingList.Count - 1, _currentTrade, TradingEnumerations.KrauseTradeReason.MainTrendUpPyramid))
                                {
                                    retracementPyramidEntryStop = _nextBuyStop + EntryStopRetracement;
                                    if (nextClose > retracementPyramidEntryStop)
                                    {
                                        retracementPyramidPossible = true;
                                        retracementPyramidTradePrice = retracementPyramidEntryStop + SkidPoints;
                                        if (!((retracementPyramidTradePrice >= nextLow) && (retracementPyramidTradePrice <= nextHigh)))
                                        {
                                            retracementPyramidTradePrice = nextHigh;
                                        }
                                        retracementPyramidIndex = _krauseSwingList.Count - 1;
                                    }
                                }
                                else if ((_minorTrend == TradingEnumerations.Trend.Up) &&
                                         (!_majorTrendJustchanged) &&
                                         // Note: -2 for last Top here! Because an extra bottom has been added as we have had a minor trend change to up.
                                         !TopBottomAlreadyInTradeList(_krauseSwingList.Count - 2, _currentTrade, TradingEnumerations.KrauseTradeReason.MainTrendUpPyramid))
                                {
                                    retracementPyramidEntryStop = _nextBuyStop + EntryStopRetracement;
                                    if (nextClose > retracementPyramidEntryStop)
                                    {
                                        retracementPyramidPossible = true;
                                        retracementPyramidTradePrice = retracementPyramidEntryStop + SkidPoints;
                                        if (!((retracementPyramidTradePrice >= nextLow) && (retracementPyramidTradePrice <= nextHigh)))
                                        {
                                            retracementPyramidTradePrice = nextHigh;
                                        }
                                        retracementPyramidIndex = _krauseSwingList.Count - 2;
                                    }
                                }
                                // Top taken out pyramid trade
                                if (_minorTrend == TradingEnumerations.Trend.Down &&
                                    // Note: -1 for last Top here!
                                    !TopBottomAlreadyInTradeList(_krauseSwingList.Count - 1, _currentTrade, TradingEnumerations.KrauseTradeReason.MainTrendUpPyramid))
                                {
                                    // We could enter a trade if the previous high is taken out
                                    exceedHiPyramidEntryStop = _krauseSwingList[_krauseSwingList.Count - 1].Price + EntryStopExceedHiLo;
                                    if ((nextHigh > exceedHiPyramidEntryStop) && (_nextSellStopActive))
                                    {
                                        exceedHiPyramidPossible = true;
                                        exceedHiPyramidTradePrice = exceedHiPyramidEntryStop + SkidPoints;
                                        if (!((exceedHiPyramidTradePrice >= nextLow) && (exceedHiPyramidTradePrice <= nextHigh)))
                                        {
                                            exceedHiPyramidTradePrice = nextHigh;
                                        }
                                        exceedHiPyramidIndex = _krauseSwingList.Count - 1;
                                    }
                                }
                                else if ((_minorTrend == TradingEnumerations.Trend.Up) &&
                                         (!_majorTrendJustchanged) &&
                                         // Note: -2 for last Top here! Because an extra bottom has been added as we have had a minor trend change to up.
                                         !TopBottomAlreadyInTradeList(_krauseSwingList.Count - 2, _currentTrade, TradingEnumerations.KrauseTradeReason.MainTrendUpPyramid))
                                {
                                    // The minor trend has already changed to up and we have no trade for previous top
                                    exceedHiPyramidEntryStop = _krauseSwingList[_krauseSwingList.Count - 2].Price + EntryStopExceedHiLo;
                                    if ((nextHigh > exceedHiPyramidEntryStop) && (_nextSellStopActive))
                                    {
                                        exceedHiPyramidPossible = true;
                                        exceedHiPyramidTradePrice = exceedHiPyramidEntryStop + SkidPoints;
                                        if (!((exceedHiPyramidTradePrice >= nextLow) && (exceedHiPyramidTradePrice <= nextHigh)))
                                        {
                                            exceedHiPyramidTradePrice = nextHigh;
                                        }
                                        exceedHiPyramidIndex = _krauseSwingList.Count - 2;
                                    }
                                }
                            }
                            TradingEnumerations.TradeStatus tradeStatus = TradingEnumerations.TradeStatus.NoTrade;
                            decimal highestEntryStop = decimal.MinValue;
                            decimal lowestEntryStop = decimal.MaxValue;
                            LocalTradeType localTradeType = LocalTradeType.None;
                            int index = 0;
                            TradingEnumerations.KrauseTradeReason reason = TradingEnumerations.KrauseTradeReason.NoTrade;
                            TradingEnumerations.KrauseTradeAdditionalEntryReason additionalReason = TradingEnumerations.KrauseTradeAdditionalEntryReason.None;
                            tradeType = TradeType(_currentTrade);
                            if (trendChangeUpPossible || trendChangeDownPossible)
                            {
                                if (trendChangeDownPossible && (tradeType != TradingEnumerations.KrauseTradeType.Short))
                                {
                                    localTradeType = LocalTradeType.TrendChange;
                                    index = trendChangeDownIndex;
                                    reason = TradingEnumerations.KrauseTradeReason.DefinateMainTrendChangeToDown;
                                    additionalReason = TradingEnumerations.KrauseTradeAdditionalEntryReason.None;
                                    tradePrice = trendChangeDownTradePrice;
                                    tradeStatus = TradingEnumerations.TradeStatus.Short;
                                }
                                else if (trendChangeUpPossible && (tradeType != TradingEnumerations.KrauseTradeType.Long))
                                {
                                    localTradeType = LocalTradeType.TrendChange;
                                    index = trendChangeUpIndex;
                                    reason = TradingEnumerations.KrauseTradeReason.DefinateMainTrendChangeToUp;
                                    additionalReason = TradingEnumerations.KrauseTradeAdditionalEntryReason.None;
                                    tradePrice = trendChangeUpTradePrice;
                                    tradeStatus = TradingEnumerations.TradeStatus.Long;
                                }
                            }
                            else
                            {
                                if ((_majorTrend == TradingEnumerations.Trend.Down) &&
                                    (tradeType != TradingEnumerations.KrauseTradeType.Short))
                                {
                                    if (retracementPyramidPossible)
                                    {
                                        localTradeType = LocalTradeType.Retracement;
                                        highestEntryStop = retracementPyramidEntryStop;
                                        index = retracementPyramidIndex;
                                        reason = TradingEnumerations.KrauseTradeReason.MainTrendDownPyramid;
                                        additionalReason = TradingEnumerations.KrauseTradeAdditionalEntryReason.PossibleMinorChangeToDown;
                                        tradePrice = retracementPyramidTradePrice;
                                        tradeStatus = TradingEnumerations.TradeStatus.Short;
                                    }
                                    if (exceedLoPyramidPossible)
                                    {
                                        if (exceedLoPyramidEntryStop > highestEntryStop)
                                        {
                                            highestEntryStop = exceedLoPyramidEntryStop;
                                            localTradeType = LocalTradeType.HiloTakenOut;
                                            index = exceedLoPyramidIndex;
                                            reason = TradingEnumerations.KrauseTradeReason.MainTrendDownPyramid;
                                            additionalReason = TradingEnumerations.KrauseTradeAdditionalEntryReason.ExceedLo;
                                            tradePrice = exceedLoPyramidTradePrice;
                                            tradeStatus = TradingEnumerations.TradeStatus.Short;
                                        }
                                    }
                                }
                                else if ((_majorTrend == TradingEnumerations.Trend.Up) &&
                                         (tradeType != TradingEnumerations.KrauseTradeType.Long))
                                {
                                    if (retracementPyramidPossible)
                                    {
                                        localTradeType = LocalTradeType.Retracement;
                                        lowestEntryStop = retracementPyramidEntryStop;
                                        index = retracementPyramidIndex;
                                        reason = TradingEnumerations.KrauseTradeReason.MainTrendUpPyramid;
                                        additionalReason = TradingEnumerations.KrauseTradeAdditionalEntryReason.PossibleMinorChangeToUp;
                                        tradePrice = retracementPyramidTradePrice;
                                        tradeStatus = TradingEnumerations.TradeStatus.Long;
                                    }
                                    if (exceedHiPyramidPossible)
                                    {
                                        if (exceedHiPyramidEntryStop < lowestEntryStop)
                                        {
                                            lowestEntryStop = exceedHiPyramidEntryStop;
                                            localTradeType = LocalTradeType.HiloTakenOut;
                                            index = exceedHiPyramidIndex;
                                            reason = TradingEnumerations.KrauseTradeReason.MainTrendUpPyramid;
                                            additionalReason = TradingEnumerations.KrauseTradeAdditionalEntryReason.ExceedHi;
                                            tradePrice = exceedHiPyramidTradePrice;
                                            tradeStatus = TradingEnumerations.TradeStatus.Long;
                                        }
                                    }
                                }
                            }
                            if (localTradeType != LocalTradeType.None)
                            {
                                newTradeToday = true;
                                // Close out any existing trades
                                decimal profit = 0.0M;
                                if (tradeType != TradingEnumerations.KrauseTradeType.None)
                                {
                                    if (tradeType == TradingEnumerations.KrauseTradeType.Short)
                                    {
                                        if (tradeStatus != TradingEnumerations.TradeStatus.Short)
                                        {
                                            _currentTrade.ExitDate = nextDate;
                                            _currentTrade.ExitPrice = tradePrice;
                                            _currentTrade.ExitReason = reason;
                                            if (_writeLogFile)
                                            {
                                                _tradeLog.WriteLine(_currentTrade.Identifier.ToString() + ", " +
                                                                    _currentTrade.TopBottomIdentifier.ToString() + ", " +
                                                                    _currentTrade.EntryDate.ToString("yyyyMMdd") + ", " +
                                                                    _currentTrade.EntryPrice.ToString() + ", " +
                                                                    _currentTrade.EntryReason.ToString() + ", " +
                                                                    _currentTrade.EntryAdditionalReason.ToString() + ", " +
                                                                    _currentTrade.EntryType.ToString() + ", " +
                                                                    _currentTrade.Contracts.ToString() + ", " +
                                                                    ((DateTime)_currentTrade.ExitDate).ToString("yyyyMMdd") + ", " +
                                                                    _currentTrade.ExitPrice.ToString() + " ," +
                                                                    _currentTrade.ExitReason.ToString() + ", " +
                                                                    (_currentTrade.CalculateProfitPoints() * DollarsPerPoint).ToString());
                                            }
                                            // Calculate the profit etc.
                                            profit = CalculateProfit(_currentTrade, tradePrice, DollarsPerPoint);
                                            _profitLoss += profit;
                                            _equity += profit;
                                            _closingBalance = _equity;
                                            _currentTrade = new KrauseTradeData();
                                        }
                                        else
                                        {
                                            _metricsLog.WriteLine("Trade was already short and we are attempting another short trade.");
                                        }
                                    }
                                    else if (tradeType == TradingEnumerations.KrauseTradeType.Long)
                                    {
                                        if (tradeStatus != TradingEnumerations.TradeStatus.Long)
                                        {
                                            // We are stopped out
                                            _currentTrade.ExitDate = nextDate;
                                            _currentTrade.ExitPrice = tradePrice;
                                            _currentTrade.ExitReason = reason;
                                            if (_writeLogFile)
                                            {
                                                _tradeLog.WriteLine(_currentTrade.Identifier.ToString() + ", " +
                                                                    _currentTrade.TopBottomIdentifier.ToString() + ", " +
                                                                    _currentTrade.EntryDate.ToString("yyyyMMdd") + ", " +
                                                                    _currentTrade.EntryPrice.ToString() + ", " +
                                                                    _currentTrade.EntryReason.ToString() + ", " +
                                                                    _currentTrade.EntryAdditionalReason.ToString() + ", " +
                                                                    _currentTrade.EntryType.ToString() + ", " +
                                                                    _currentTrade.Contracts.ToString() + ", " +
                                                                    ((DateTime)_currentTrade.ExitDate).ToString("yyyyMMdd") + ", " +
                                                                    _currentTrade.ExitPrice.ToString() + " ," +
                                                                    _currentTrade.ExitReason.ToString() + ", " +
                                                                    (_currentTrade.CalculateProfitPoints() * DollarsPerPoint).ToString());
                                            }
                                            // Calculate the profit etc.
                                            profit = CalculateProfit(_currentTrade, tradePrice, DollarsPerPoint);
                                            _profitLoss += profit;
                                            _equity += profit;
                                            _closingBalance = _equity;
                                            _currentTrade = new KrauseTradeData();
                                        }
                                        else
                                        {
                                            _metricsLog.WriteLine("Trade was already long and we are attempting another long trade.");
                                        }
                                    }
                                }
                                if (tradeStatus == TradingEnumerations.TradeStatus.Short)
                                {
                                    // Open the new trade
                                    uint positionSize = _noPositions;
                                    KrauseTradeData newTrade = new KrauseTradeData(_identifier,
                                                                                   index,
                                                                                   nextDate,
                                                                                   tradePrice,
                                                                                   reason,
                                                                                   additionalReason,
                                                                                   positionSize,
                                                                                   TradingEnumerations.TradeStatus.Short,
                                                                                   null,
                                                                                   null,
                                                                                   TradingEnumerations.KrauseTradeReason.NotExitedYet);
                                    _currentTrade = newTrade;
                                    _identifier++;
                                    if (localTradeType == LocalTradeType.TrendChange)
                                    {
                                        _majorDownSellTrades.Add(new LineData(nextDate, tradePrice));
                                    }
                                    else
                                    {
                                        _pyramidSellTrades.Add(new LineData(nextDate, tradePrice));
                                    }
                                }
                                else if (tradeStatus == TradingEnumerations.TradeStatus.Long)
                                {
                                    // Open the new trade
                                    uint positionSize = _noPositions;
                                    KrauseTradeData newTrade = new KrauseTradeData(_identifier,
                                                                                   index,
                                                                                   nextDate,
                                                                                   tradePrice,
                                                                                   reason,
                                                                                   additionalReason,
                                                                                   positionSize,
                                                                                   TradingEnumerations.TradeStatus.Long,
                                                                                   null,
                                                                                   null,
                                                                                   TradingEnumerations.KrauseTradeReason.NotExitedYet);
                                    _currentTrade = newTrade;
                                    _identifier++;
                                    if (localTradeType == LocalTradeType.TrendChange)
                                    {
                                        _majorUpBuyTrades.Add(new LineData(nextDate, tradePrice));
                                    }
                                    else
                                    {
                                        _pyramidBuyTrades.Add(new LineData(nextDate, tradePrice));
                                    }
                                }
                            }
                        }
                        // Check Profit Protection Rules 
                        if (!newTradeToday)
                        {
                            if (tradeType != TradingEnumerations.KrauseTradeType.None)
                            {
                                if (tradeType == TradingEnumerations.KrauseTradeType.Short)
                                {
                                    // Try Profit Protection Rule #2 first as it happens intra-day
                                    // Profit Protection #1 happens at the close
                                    bool profitProtection2Possible = false;
                                    decimal lowestLow = 0.0M;
                                    decimal highestHigh = 0.0M; 
                                    if (_minorTrend == TradingEnumerations.Trend.Up)
                                    {
                                        // The minor trend has already turned up
                                        profitProtection2Possible = true;
                                        lowestLow = _krauseSwingList[_krauseSwingList.Count - 1].Price;
                                        highestHigh = _krauseSwingList[_krauseSwingList.Count - 2].Price;
                                    }
                                    else if ((_minorTrend == TradingEnumerations.Trend.Down) && 
                                             (high > prevHigh) && (nextHigh > high))
                                    {
                                        // The minor trend turns to up on the current bar
                                        profitProtection2Possible = true;
                                        // Note: the lowest low cannot include the current low unless
                                        // we watch the market intra-day
                                        lowestLow = lowestLowSinceMinorTrendChange;
                                        highestHigh = _krauseSwingList[_krauseSwingList.Count - 1].Price;
                                    }
                                    if (profitProtection2Possible)
                                    {
                                        decimal retracement = nextHigh - lowestLow;
                                        decimal previousSwing = highestHigh - lowestLow;
                                        decimal retracementFraction = retracement / previousSwing;
                                        if ((retracementFraction > RetracementFraction) &&
                                            (nextHigh > _nextBuyStop + EntryStopRetracement))
                                        {
                                            tradePrice = _nextBuyStop + EntryStopRetracement + SkidPoints;
                                            if (tradePrice > nextHigh)
                                            {
                                                tradePrice = nextHigh;
                                            }
                                            _currentTrade.ExitDate = nextDate;
                                            _currentTrade.ExitPrice = tradePrice;
                                            _currentTrade.ExitReason = TradingEnumerations.KrauseTradeReason.ProfitProtection2;
                                            if (_writeLogFile)
                                            {
                                                _tradeLog.WriteLine(_currentTrade.Identifier.ToString() + ", " +
                                                                    _currentTrade.TopBottomIdentifier.ToString() + ", " +
                                                                    _currentTrade.EntryDate.ToString("yyyyMMdd") + ", " +
                                                                    _currentTrade.EntryPrice.ToString() + ", " +
                                                                    _currentTrade.EntryReason.ToString() + ", " +
                                                                    _currentTrade.EntryAdditionalReason.ToString() + ", " +
                                                                    _currentTrade.EntryType.ToString() + ", " +
                                                                    _currentTrade.Contracts.ToString() + ", " +
                                                                    ((DateTime)_currentTrade.ExitDate).ToString("yyyyMMdd") + ", " +
                                                                    _currentTrade.ExitPrice.ToString() + " ," +
                                                                    _currentTrade.ExitReason.ToString() + ", " +
                                                                    (_currentTrade.CalculateProfitPoints() * DollarsPerPoint).ToString());
                                            }
                                            _profitP2Trades.Add(new LineData(nextDate, (decimal)_currentTrade.ExitPrice));
                                            // Calculate the profit etc.
                                            decimal profit = CalculateProfit(_currentTrade, tradePrice, DollarsPerPoint);
                                            _profitLoss += profit;
                                            _equity += profit;
                                            _closingBalance = _equity;
                                            _currentTrade = new KrauseTradeData();
                                        }
                                    }
                                    tradeType = TradeType(_currentTrade);
                                    // If we are still in a trade, attempt to close it out using Profit Protection Rule #1
                                    if (tradeType != TradingEnumerations.KrauseTradeType.None)
                                    {
                                        if (nextClose > (_nextBuyStop + EntryStopRetracement))
                                        {
                                            tradePrice = nextClose;
                                            _currentTrade.ExitDate = nextDate;
                                            _currentTrade.ExitPrice = tradePrice;
                                            _currentTrade.ExitReason = TradingEnumerations.KrauseTradeReason.ProfitProtection1;
                                            if (_writeLogFile)
                                            {
                                                _tradeLog.WriteLine(_currentTrade.Identifier.ToString() + ", " +
                                                                    _currentTrade.TopBottomIdentifier.ToString() + ", " +
                                                                    _currentTrade.EntryDate.ToString("yyyyMMdd") + ", " +
                                                                    _currentTrade.EntryPrice.ToString() + ", " +
                                                                    _currentTrade.EntryReason.ToString() + ", " +
                                                                    _currentTrade.EntryAdditionalReason.ToString() + ", " +
                                                                    _currentTrade.EntryType.ToString() + ", " +
                                                                    _currentTrade.Contracts.ToString() + ", " +
                                                                    ((DateTime)_currentTrade.ExitDate).ToString("yyyyMMdd") + ", " +
                                                                    _currentTrade.ExitPrice.ToString() + " ," +
                                                                    _currentTrade.ExitReason.ToString() + ", " +
                                                                    (_currentTrade.CalculateProfitPoints() * DollarsPerPoint).ToString());
                                            }
                                            _profitP1Trades.Add(new LineData(nextDate, (decimal)_currentTrade.ExitPrice));
                                            // Calculate the profit etc.
                                            decimal profit = CalculateProfit(_currentTrade, tradePrice, DollarsPerPoint);
                                            _profitLoss += profit;
                                            _equity += profit;
                                            _closingBalance = _equity;
                                            _currentTrade = new KrauseTradeData();
                                        }
                                    }
                                }
                                else if (tradeType == TradingEnumerations.KrauseTradeType.Long)
                                {
                                    // Try Profit Protection Rule #2 first as it happens intra-day
                                    // Profit Protection #1 happens at the close
                                    bool profitProtection2Possible = false;
                                    decimal lowestLow = 0.0M;
                                    decimal highestHigh = 0.0M;
                                    if (_minorTrend == TradingEnumerations.Trend.Down)
                                    {
                                        // The minor trend has already turned down
                                        profitProtection2Possible = true;
                                        lowestLow = _krauseSwingList[_krauseSwingList.Count - 2].Price;
                                        highestHigh = _krauseSwingList[_krauseSwingList.Count - 1].Price;
                                    }
                                    else if ((_minorTrend == TradingEnumerations.Trend.Up) &&
                                             (low < prevLow) && (nextLow < low))
                                    {
                                        // The minor trend turns to down on the current bar
                                        profitProtection2Possible = true;
                                        // Note: the highest high cannot include the current high unless
                                        // we watch the market intra-day
                                        lowestLow = _krauseSwingList[_krauseSwingList.Count - 1].Price;
                                        highestHigh = highestHighSinceMinorTrendChange;
                                    }
                                    if (profitProtection2Possible)
                                    {
                                        decimal retracement = highestHigh - nextLow;
                                        decimal previousSwing = highestHigh - lowestLow;
                                        decimal retracementFraction = retracement / previousSwing;
                                        if (retracementFraction > RetracementFraction &&
                                            (nextLow < _nextSellStop - EntryStopRetracement))
                                        {
                                            tradePrice = _nextSellStop - EntryStopRetracement - SkidPoints;
                                            if (tradePrice < nextLow)
                                            {
                                                tradePrice = nextLow;
                                            }
                                            _currentTrade.ExitDate = nextDate;
                                            _currentTrade.ExitPrice = tradePrice;
                                            _currentTrade.ExitReason = TradingEnumerations.KrauseTradeReason.ProfitProtection1;
                                            if (_writeLogFile)
                                            {
                                                _tradeLog.WriteLine(_currentTrade.Identifier.ToString() + ", " +
                                                                    _currentTrade.TopBottomIdentifier.ToString() + ", " +
                                                                    _currentTrade.EntryDate.ToString("yyyyMMdd") + ", " +
                                                                    _currentTrade.EntryPrice.ToString() + ", " +
                                                                    _currentTrade.EntryReason.ToString() + ", " +
                                                                    _currentTrade.EntryAdditionalReason.ToString() + ", " +
                                                                    _currentTrade.EntryType.ToString() + ", " +
                                                                    _currentTrade.Contracts.ToString() + ", " +
                                                                    ((DateTime)_currentTrade.ExitDate).ToString("yyyyMMdd") + ", " +
                                                                    _currentTrade.ExitPrice.ToString() + " ," +
                                                                    _currentTrade.ExitReason.ToString() + ", " +
                                                                    (_currentTrade.CalculateProfitPoints() * DollarsPerPoint).ToString());
                                            }
                                            _profitP2Trades.Add(new LineData(nextDate, (decimal)_currentTrade.ExitPrice));
                                            // Calculate the profit etc.
                                            decimal profit = CalculateProfit(_currentTrade, tradePrice, DollarsPerPoint);
                                            _profitLoss += profit;
                                            _equity += profit;
                                            _closingBalance = _equity;
                                            _currentTrade = new KrauseTradeData();
                                        }
                                    }
                                    tradeType = TradeType(_currentTrade);
                                    // If we are still in a trade, attempt to close it out using Profit Protection Rule #1
                                    if (tradeType != TradingEnumerations.KrauseTradeType.None)
                                    {
                                        if (nextClose < (_nextSellStop - EntryStopRetracement))
                                        {
                                            tradePrice = nextClose;
                                            _currentTrade.ExitDate = nextDate;
                                            _currentTrade.ExitPrice = tradePrice;
                                            _currentTrade.ExitReason = TradingEnumerations.KrauseTradeReason.ProfitProtection1;
                                            if (_writeLogFile)
                                            {
                                                _tradeLog.WriteLine(_currentTrade.Identifier.ToString() + ", " +
                                                                    _currentTrade.TopBottomIdentifier.ToString() + ", " +
                                                                    _currentTrade.EntryDate.ToString("yyyyMMdd") + ", " +
                                                                    _currentTrade.EntryPrice.ToString() + ", " +
                                                                    _currentTrade.EntryReason.ToString() + ", " +
                                                                    _currentTrade.EntryAdditionalReason.ToString() + ", " +
                                                                    _currentTrade.EntryType.ToString() + ", " +
                                                                    _currentTrade.Contracts.ToString() + ", " +
                                                                    ((DateTime)_currentTrade.ExitDate).ToString("yyyyMMdd") + ", " +
                                                                    _currentTrade.ExitPrice.ToString() + " ," +
                                                                    _currentTrade.ExitReason.ToString() + ", " +
                                                                    (_currentTrade.CalculateProfitPoints() * DollarsPerPoint).ToString());
                                            }
                                            _profitP1Trades.Add(new LineData(nextDate, (decimal)_currentTrade.ExitPrice));
                                            // Calculate the profit etc.
                                            decimal profit = CalculateProfit(_currentTrade, tradePrice, DollarsPerPoint);
                                            _profitLoss += profit;
                                            _equity += profit;
                                            _closingBalance = _equity;
                                            _currentTrade = new KrauseTradeData();
                                        }
                                    }
                                }
                            }
                        }
                        tradeType = TradeType(_currentTrade);
                        // The following should happen after all trade information has been finalised
                        if (tradeType != TradingEnumerations.KrauseTradeType.None)
                        {
                            // If we are in a trade
                            // Update the current equity
                            _openProfit = CalculateProfit(_currentTrade, nextClose, DollarsPerPoint);
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
                            if ((nextLow < low) && (low < prevLow))
                            {
                                // We need two consecutive lower days to change the trend to Down
                                _minorTrend = TradingEnumerations.Trend.Down;
                                // So we have had a top recently
                                // Go from i - 1 to the bottom (which must be the previous point in the _krauseSwingList)
                                // and find the point and add it to the list.
                                // We only look at the current date (actually nextDate) and dates for which the trend is UP
                                DateTime highDate = date;
                                decimal highValue = high;
                                int highIndex = i - 1;
                                for (int j = highIndex; j >= _krauseSwingList[_krauseSwingList.Count - 1].Index; j--)
                                {
                                    if ((barDataReader.OHLC[j].MinorTrend == TradingEnumerations.Trend.Up) && (barDataReader.OHLC[j].High >= highValue))
                                    {
                                        highValue = barDataReader.OHLC[j].High;
                                        highDate = barDataReader.OHLC[j].Date;
                                        highIndex = j;
                                    }
                                }
                                _krauseSwingList.Add(new LineData(highDate, highValue, highIndex, TradingEnumerations.TopBottom.Top));
                                lowestLowSinceMinorTrendChange = nextLow;
                                highestHighSinceMinorTrendChange = nextHigh;
                                // If we have had our first minor trend change since the major trend changed,
                                // reset the major trend just changed flag
                                if (_majorTrendJustchanged)
                                {
                                    _majorTrendJustchanged = false;
                                    _majorTrendChangeToUp = -1;
                                    _majorTrendChangeToDown = -1;
                                }
                            }
                            // If the major trend has been initialised and it is up and the minor trend is up
                            // and we have taken out the previous bottom
                            else if (_majorTrendInitialised && 
                                     (_majorTrend == TradingEnumerations.Trend.Up) && 
                                     (_minorTrend == TradingEnumerations.Trend.Up) &&
                                     (nextLow < _krauseSwingList[_krauseSwingList.Count - 1].Price))
                            {
                                // We have taken out the previous bottom so the minor and major
                                // trends must change to down
                                _minorTrend = TradingEnumerations.Trend.Down;
                                // So we have had a top recently
                                // Go from i - 1 to the bottom (which must be the previous point in the _krauseSwingList)
                                // and find the point and add it to the list.
                                // We only look at the current date (actually nextDate) and dates for which the trend is UP
                                DateTime highDate = date;
                                decimal highValue = high;
                                int highIndex = i - 1;
                                for (int j = highIndex; j >= _krauseSwingList[_krauseSwingList.Count - 1].Index; j--)
                                {
                                    if ((barDataReader.OHLC[j].MinorTrend == TradingEnumerations.Trend.Up) && (barDataReader.OHLC[j].High >= highValue))
                                    {
                                        highValue = barDataReader.OHLC[j].High;
                                        highDate = barDataReader.OHLC[j].Date;
                                        highIndex = j;
                                    }
                                }
                                _krauseSwingList.Add(new LineData(highDate, highValue, highIndex, TradingEnumerations.TopBottom.Top));
                                lowestLowSinceMinorTrendChange = nextLow;
                                highestHighSinceMinorTrendChange = nextHigh;
                            }
                        }
                        else if (_minorTrend == TradingEnumerations.Trend.Down)
                        {
                            if ((nextHigh > high) && (high > prevHigh))
                            {
                                // We need two consecutive higher high days to change the trend to Up
                                _minorTrend = TradingEnumerations.Trend.Up;
                                // So we have had a bottom recently
                                // Go from i - 1 to the top (which must be the previous point in the _krauseSwingList)
                                // and find the point and add it to the list.
                                // We only look at the currrent date (actually nextDate) and dates for which the trend is DOWN
                                DateTime lowDate = date;
                                decimal lowValue = low;
                                int lowIndex = i - 1;
                                for (int j = lowIndex; j >= _krauseSwingList[_krauseSwingList.Count - 1].Index; j--)
                                {
                                    if ((barDataReader.OHLC[j].MinorTrend == TradingEnumerations.Trend.Down) && (barDataReader.OHLC[j].Low <= lowValue))
                                    {
                                        lowValue = barDataReader.OHLC[j].Low;
                                        lowDate = barDataReader.OHLC[j].Date;
                                        lowIndex = j;
                                    }
                                }
                                _krauseSwingList.Add(new LineData(lowDate, lowValue, lowIndex, TradingEnumerations.TopBottom.Bottom));
                                lowestLowSinceMinorTrendChange = nextLow;
                                highestHighSinceMinorTrendChange = nextHigh;
                                // If we have had our first minor trend change since the major trend changed,
                                // reset the major trend just changed flag
                                if (_majorTrendJustchanged)
                                {
                                    _majorTrendJustchanged = false;
                                    _majorTrendChangeToUp = -1;
                                    _majorTrendChangeToDown = -1;
                                }
                            }
                            // If the major trend has been initialised and it is down and the minor trend is down
                            // and we have taken out the previous top
                            else if (_majorTrendInitialised && 
                                     (_majorTrend == TradingEnumerations.Trend.Down) && 
                                     (_minorTrend == TradingEnumerations.Trend.Down) &&
                                     (nextHigh > _krauseSwingList[_krauseSwingList.Count - 1].Price))
                            {
                                // We have taken out the previous top so the minor and major
                                // trends must change to down
                                _minorTrend = TradingEnumerations.Trend.Up;
                                // So we have had a bottom recently
                                // Go from i - 1 to the top (which must be the previous point in the _krauseSwingList)
                                // and find the point and add it to the list.
                                // We only look at the currrent date (actually nextDate) and dates for which the trend is DOWN
                                DateTime lowDate = date;
                                decimal lowValue = low;
                                int lowIndex = i - 1;
                                for (int j = lowIndex; j >= _krauseSwingList[_krauseSwingList.Count - 1].Index; j--)
                                {
                                    if ((barDataReader.OHLC[j].MinorTrend == TradingEnumerations.Trend.Down) && (barDataReader.OHLC[j].Low <= lowValue))
                                    {
                                        lowValue = barDataReader.OHLC[j].Low;
                                        lowDate = barDataReader.OHLC[j].Date;
                                        lowIndex = j;
                                    }
                                }
                                _krauseSwingList.Add(new LineData(lowDate, lowValue, lowIndex, TradingEnumerations.TopBottom.Bottom));
                                lowestLowSinceMinorTrendChange = nextLow;
                                highestHighSinceMinorTrendChange = nextHigh;
                            }
                        }
                        else if (_minorTrend == TradingEnumerations.Trend.Unknown)
                        {
                            if ((nextHigh > high) && (high > prevHigh))
                            {
                                _minorTrend = TradingEnumerations.Trend.Up;  // The minor trend has changed to UP
                                // Go from i - 1 to the start of the list and find the lowest 
                                // point and add it to the list. This is the first low
                                // Note: we should start from a top or bottom
                                DateTime lowDate = DateTime.MaxValue;
                                decimal lowValue = decimal.MaxValue;
                                int lowIndex = -1;
                                for (int j = i - 1; j >= 0; j--)
                                {
                                    if (barDataReader.OHLC[j].Low <= lowValue)
                                    {
                                        lowValue = barDataReader.OHLC[j].Low;
                                        lowDate = barDataReader.OHLC[j].Date;
                                        lowIndex = j;
                                    }
                                }
                                _krauseSwingList.Add(new LineData(lowDate, lowValue, lowIndex, TradingEnumerations.TopBottom.Bottom));
                                lowestLowSinceMinorTrendChange = nextLow;
                                highestHighSinceMinorTrendChange = nextHigh;
                            }
                            else if ((nextLow < low) && (low < prevLow))
                            {
                                _minorTrend = TradingEnumerations.Trend.Down;  // The minor trend has changed to DOWN
                                // Go from i - 1 to the start of the list and find the highest 
                                // point and add it to the list. This is the first high
                                // Note: we should start from a top or bottom
                                DateTime highDate = DateTime.MinValue;
                                decimal highValue = decimal.MinValue;
                                int highIndex = -1;
                                for (int j = i - 1; j >= 0; j--)
                                {
                                    if (barDataReader.OHLC[j].High >= highValue)
                                    {
                                        highValue = barDataReader.OHLC[j].High;
                                        highDate = barDataReader.OHLC[j].Date;
                                        highIndex = j;
                                    }
                                }
                                _krauseSwingList.Add(new LineData(highDate, highValue, highIndex, TradingEnumerations.TopBottom.Top));
                                lowestLowSinceMinorTrendChange = nextLow;
                                highestHighSinceMinorTrendChange = nextHigh;
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
                        if (_krauseSwingList.Count == 3)
                        {
                            if ((_krauseSwingList[_krauseSwingList.Count - 3].TopBottom == TradingEnumerations.TopBottom.Bottom) && (_krauseSwingList[_krauseSwingList.Count - 1].TopBottom == TradingEnumerations.TopBottom.Bottom))
                            {
                                if (_krauseSwingList[_krauseSwingList.Count - 1].Price >= _krauseSwingList[_krauseSwingList.Count - 3].Price)
                                {
                                    _majorTrend = TradingEnumerations.Trend.Up;
                                }
                                else
                                {
                                    _majorTrend = TradingEnumerations.Trend.Down;
                                }
                                _majorTrendInitialised = true;
                            }
                            if ((_krauseSwingList[_krauseSwingList.Count - 3].TopBottom == TradingEnumerations.TopBottom.Top) && (_krauseSwingList[_krauseSwingList.Count - 1].TopBottom == TradingEnumerations.TopBottom.Top))
                            {
                                if (_krauseSwingList[_krauseSwingList.Count - 1].Price < _krauseSwingList[_krauseSwingList.Count - 3].Price)
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
                        if (_majorTrendInitialised)
                        {
                            if (_majorTrend == TradingEnumerations.Trend.Up)
                            {
                                if (_krauseSwingList[_krauseSwingList.Count - 2].TopBottom == TradingEnumerations.TopBottom.Bottom)
                                {
                                    // Note: Minor Trend must be Down anyway or we wouldn't have a Bottom at position -2 
                                    if ((_minorTrend == TradingEnumerations.Trend.Down) &&
                                        (nextLow < _krauseSwingList[_krauseSwingList.Count - 2].Price))
                                    {
                                        _majorTrend = TradingEnumerations.Trend.Down;
                                        _majorTrendChangeToDown = _krauseSwingList.Count - 1;
                                        _trendChangePrice = _krauseSwingList[_krauseSwingList.Count - 2].Price;
                                        _majorTrendJustchanged = true;
                                    }
                                }
                            }
                            else if (_majorTrend == TradingEnumerations.Trend.Down)
                            {
                                if (_krauseSwingList[_krauseSwingList.Count - 2].TopBottom == TradingEnumerations.TopBottom.Top)
                                {
                                    // Note: Minor Trend must be Up anyway or we wouldn't have a Top at position -2 
                                    if ((_minorTrend == TradingEnumerations.Trend.Up) &&
                                        (nextHigh > _krauseSwingList[_krauseSwingList.Count - 2].Price))
                                    {
                                        _majorTrend = TradingEnumerations.Trend.Up;
                                        _majorTrendChangeToUp = _krauseSwingList.Count - 1;
                                        _trendChangePrice = _krauseSwingList[_krauseSwingList.Count - 2].Price;
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
                        // Calculate the buy and sell stops
                        _sellStop = _nextSellStop;
                        _buyStop = _nextBuyStop;
                        _nextSellStop = (prevLow + low + nextLow) / 3.0M;
                        _nextBuyStop = (prevHigh + high + nextHigh) / 3.0M;
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
                    if (TradeType(_currentTrade) != TradingEnumerations.KrauseTradeType.None)
                    {
                        // The exit is different.
                        // The system exits the final trade at the final closing price
                        decimal profit = CalculateProfit(_currentTrade, nextClose, DollarsPerPoint);
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
                        _currentTrade.ExitDate = nextDate;
                        _currentTrade.ExitPrice = nextClose;
                        if (_writeLogFile)
                        {
                            _tradeLog.WriteLine(_currentTrade.Identifier.ToString() + ", " +
                                                _currentTrade.TopBottomIdentifier.ToString() + ", " +
                                                _currentTrade.EntryDate.ToString("yyyyMMdd") + ", " +
                                                _currentTrade.EntryPrice.ToString() + ", " +
                                                _currentTrade.EntryReason.ToString() + ", " +
                                                _currentTrade.EntryAdditionalReason.ToString() + ", " +
                                                _currentTrade.EntryType.ToString() + ", " +
                                                _currentTrade.Contracts.ToString() + ", " +
                                                nextDate.ToString("yyyyMMdd") + ", " +
                                                _currentTrade.ExitPrice.ToString() + " ," +
                                                TradingEnumerations.KrauseTradeReason.DefinateMainTrendChangeToUp.ToString() + ", " +
                                                (_currentTrade.CalculateProfitPoints() * DollarsPerPoint).ToString());
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
        private decimal CalculateProfit(KrauseTradeData trade, decimal price, decimal dollarsPerPoint)
        {
            decimal profit = 0.0M;
            if (TradeType(trade) != TradingEnumerations.KrauseTradeType.None)
            {
                if (trade.EntryType == TradingEnumerations.TradeStatus.Long)
                {
                    profit = (price - trade.EntryPrice) * (decimal)trade.Contracts;
                }
                else if (trade.EntryType == TradingEnumerations.TradeStatus.Short)
                {
                    profit = (trade.EntryPrice - price) * (decimal)trade.Contracts;
                }
            }
            return profit * dollarsPerPoint;
        }

        // Determine the trade type in the list
        private TradingEnumerations.KrauseTradeType TradeType(KrauseTradeData trade)
        {
            TradingEnumerations.KrauseTradeType tradeType = TradingEnumerations.KrauseTradeType.None;
            if (trade.EntryReason != TradingEnumerations.KrauseTradeReason.NoTrade)
            {
                if (trade.EntryType == TradingEnumerations.TradeStatus.Long)
                {
                    tradeType = TradingEnumerations.KrauseTradeType.Long;
                }
                else if (trade.EntryType == TradingEnumerations.TradeStatus.Short)
                {
                    tradeType = TradingEnumerations.KrauseTradeType.Short;
                }
                else
                {
                    // I don't see how this could happen but put it here anyway 
                    tradeType = TradingEnumerations.KrauseTradeType.None;
                }
            }
            else
            {
                tradeType = TradingEnumerations.KrauseTradeType.None;
            }
            return tradeType;
        }

        // Determine if this top/bottom already corresponds to the current trade
        private bool TopBottomAlreadyInTradeList(int topBottomIdentifer, KrauseTradeData trade, TradingEnumerations.KrauseTradeReason entryReason)
        {
            bool alreadyInList = false;
            if (TradeType(trade) != TradingEnumerations.KrauseTradeType.None)
            {
                if ((trade.TopBottomIdentifier == topBottomIdentifer) && (trade.EntryReason == entryReason))
                {
                    alreadyInList = true;
                }
            }
            return alreadyInList;
        }
        #endregion
    }
}

