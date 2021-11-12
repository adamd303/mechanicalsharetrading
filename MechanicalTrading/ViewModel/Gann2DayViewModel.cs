// Copyright (c) 2012, Adam Duval Kangaroo Point South 4169, Australia.
// All Rights Reserved.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Adam.Trading.Mechanical.Model;
using Adam.Trading.Mechanical.ViewModel.Command;

namespace Adam.Trading.Mechanical.ViewModel
{
    public class Gann2DayViewModel : ViewModelBase, IDataErrorInfo
    {
        private Gann2DayCalculator _gann2DayCalculator;                    // The Gann 2 Day calculation object
        private StockChartViewModel _stockChartViewModel;                  // The Stock Chart view model
        private StockDataSeriesViewModel _stockDataSeriesViewModel;        // The Stock Chart Data Series (OHLC) view model
        private LineDataSeriesViewModel _gann2DaySwing;                    // The Stock Chart Line Gann Swing Series
        private SymbolsViewModel _changeInTrendToUpBuy;                    // The symbol for a change in trend to up buy
        private SymbolsViewModel _changeInTrendToDownSell;                 // The symbol for a change in trend to down sell
        private SymbolsViewModel _stoppedOut;                              // The symbol for a stooped out trade
        private SymbolsViewModel _pyramidBuy;                              // The main trend up pyramid buy trades
        private SymbolsViewModel _pyramidSell;                             // The main trend down pyramid sell trades
        private SymbolsViewModel _contractRollover;                        // A contract rollover day
        private string _gannEntryStopRetracement;                          // The Entry Stop for Retracement
        private string _gannEntryStopExceedHiLo;                           // The Entry Stop for ExceedHiLo
        private string _gannExitStop;                                      // The Exit Stop
        private bool _gannEntryStopRetracementATR = false;                 // The Entry Stop for Retracement is in number of ATRs (Average True Ranges)
        private bool _gannEntryStopExceedHiLoATR = false;                  // The Entry Stop for ExceedHiLo is in number of ATRs
        private bool _gannExitStopATR = false;                             // The Exit Stop is in number of ATRS
        private bool _useOpenProfit = false;                               // Use Open Profit rather than closing balance for no. of contracts calculation (risk)
        private string _inputFile;                                         // The Input Data File name
        private string _dollarsPerPoint;                                   // The number of dollars per point of price movement
        private string _skidPoints;                                        // The number of points of skid for buy/sell orders
        private string _startingEquity;                                    // The Starting Equity
        private string _percentRisk;                                       // The Percent of Capital Risked for the Trade
        private string _skidFraction;                                      // The Skid Fraction to determine Entry/Exit price
        private string _benchmarkReturn;                                   // The Benchmark Return to compare to (For Shape Ratio calculation)
        private IFilePathProvider _filePathFinder;                         // Used to get the input file
        private bool _formValid;                                           // Are the form inputs in valid state
        private string _errorString = string.Empty;                        // The error message
        private decimal _decimalEntryStopRetracement;                      // The Entry stop for Retracement converted to a decimal
        private decimal _decimalEntryStopExceedHiLo;                       // The Entry stop for Exceed Hi Lo converted to a decimal
        private decimal _decimalExitStop;                                  // The Exit stop converted to a decimal
        private decimal _decimalDollarsPerPoint;                           // The Dollars per Point of Price movement converted to decimal
        private decimal _decimalSkidPoints;                                // The Skid Points for Buy/Sell orders converted to decimal
        private decimal _decimalStartingEquity;                            // The Starting Equity converted to a decimal
        private decimal _decimalPercentRisk;                               // The Percent of Capital converted to a decimal
        private decimal _decimalBenchmarkReturn;                           // The Benchmark Return converted to a decimal
        private DateTime _dataStartDate = DateTime.MinValue;               // The minimum data date
        private DateTime _dataEndDate = DateTime.MaxValue;                 // The maximum data date
        private DateTime _chartStartDate = DateTime.MinValue;              // The chart start date
        private DateTime _chartEndDate = DateTime.MaxValue;                // The chart end date
        private ICommand calculateCommand;                                 // The calculate Support/Resistance Trading Model command

        public event EventHandler<Gann2DayViewModelEventArgs<Gann2DayViewModel>> OnGann2DayCalculated;

        #region Accessor Methods
        /// <summary>
        /// The Gann Entry Stop for Retracement
        /// </summary>
        public string GannEntryStopRetracement
        {
            get
            {
                return _gannEntryStopRetracement;
            }
            set
            {
                if (_gannEntryStopRetracement != value)
                {
                    _gannEntryStopRetracement = value;
                    FirePropertyChanged("GannEntryStopRetracement");
                }
            }
        }

        /// <summary>
        /// The Gann Entry Stop for Exceed HiLo
        /// </summary>
        public string GannEntryStopExceedHiLo
        {
            get
            {
                return _gannEntryStopExceedHiLo;
            }
            set
            {
                if (_gannEntryStopExceedHiLo != value)
                {
                    _gannEntryStopExceedHiLo = value;
                    FirePropertyChanged("GannEntryStopExceedHiLo");
                }
            }
        }

        /// <summary>
        /// The Gann Entry Stop for Retracement is in ATRs
        /// </summary>
        public bool GannEntryStopRetracementATR
        {
            get
            {
                return _gannEntryStopRetracementATR;
            }
            set
            {
                if (_gannEntryStopRetracementATR != value)
                {
                    _gannEntryStopRetracementATR = value;
                    FirePropertyChanged("GannEntryStopRetracementATR");
                }
            }
        }

        /// <summary>
        /// The Gann Entry Stop for Exceed Hi Lo is in ATRs
        /// </summary>
        public bool GannEntryStopExceedHiLoATR
        {
            get
            {
                return _gannEntryStopExceedHiLoATR;
            }
            set
            {
                if (_gannEntryStopExceedHiLoATR != value)
                {
                    _gannEntryStopExceedHiLoATR = value;
                    FirePropertyChanged("GannEntryStopExceedHiLoATR");
                }
            }
        }

        /// <summary>
        /// The Gann Exit Stop
        /// </summary>
        public string GannExitStop
        {
            get
            {
                return _gannExitStop;
            }
            set
            {
                if (_gannExitStop != value)
                {
                    _gannExitStop = value;
                    FirePropertyChanged("GannExitStop");
                }
            }
        }

        /// <summary>
        /// The Gann Exit Stop is in ATRs
        /// </summary>
        public bool GannExitStopPercent
        {
            get
            {
                return _gannExitStopATR;
            }
            set
            {
                if (_gannExitStopATR != value)
                {
                    _gannExitStopATR = value;
                    FirePropertyChanged("GannExitStopATR");
                }
            }
        }

        /// <summary>
        /// Use Open Profit for no. of contracts calc. rather than closing balance
        /// </summary>
        public bool UseOpenProfit
        {
            get
            {
                return _useOpenProfit;
            }
            set
            {
                if (_useOpenProfit != value)
                {
                    _useOpenProfit = value;
                    FirePropertyChanged("UseOpenProfit");
                }
            }
        }

        /// <summary>
        /// The Input Data File name
        /// </summary>
        public string InputFile
        {
            get
            {
                return _inputFile;
            }
            set
            {
                if (_inputFile != value)
                {
                    _inputFile = value;
                    FirePropertyChanged("InputFile");
                }
            }
        }

        /// <summary>
        /// The Number of Dollars Per Point of Price Movement
        /// </summary>
        public string DollarsPerPoint
        {
            get
            {
                return _dollarsPerPoint;
            }
            set
            {
                if (_dollarsPerPoint != value)
                {
                    _dollarsPerPoint = value;
                    FirePropertyChanged("DollarsPerPoint");
                }
            }
        }

        /// <summary>
        /// The skid points for buy/sell orders
        /// </summary>
        public string SkidPoints
        {
            get
            {
                return _skidPoints;
            }
            set
            {
                if (_skidPoints != value)
                {
                    _skidPoints = value;
                    FirePropertyChanged("SkidPoints");
                }
            }
        }

        /// <summary>
        /// The Starting Equity
        /// </summary>
        public string StartingEquity
        {
            get
            {
                return _startingEquity;
            }
            set
            {
                if (_startingEquity != value)
                {
                    _startingEquity = value;
                    FirePropertyChanged("StartingEquity");
                }
            }
        }

        /// <summary>
        /// The Percent of Capital Risked for the Trade
        /// </summary>
        public string PercentRisk
        {
            get
            {
                return _percentRisk;
            }
            set
            {
                if (_percentRisk != value)
                {
                    _percentRisk = value;
                    FirePropertyChanged("PercentRisk");
                }
            }
        }

        /// <summary>
        /// The Skid Fraction to determine Entry/Exit price
        /// </summary>
        public string SkidFraction
        {
            get
            {
                return _skidFraction;
            }
            set
            {
                if (_skidFraction != value)
                {
                    _skidFraction = value;
                    FirePropertyChanged("SkidFraction");
                }
            }
        }

        /// <summary>
        /// The Benchmark Return used for Sharpe Ratio Calculation.
        /// </summary>
        public string BenchmarkReturn
        {
            get
            {
                return _benchmarkReturn;
            }
            set
            {
                if (_benchmarkReturn != value)
                {
                    _benchmarkReturn = value;
                    FirePropertyChanged("BenchmarkReturn");
                }
            }
        }

        /// <summary>
        /// The Stock Chart View Model for display
        /// </summary>
        public StockChartViewModel ChartViewModel
        {
            get { return _stockChartViewModel; }
        }

        /// <summary>
        /// The Data Start Date
        /// </summary>
        public DateTime DataStartDate
        {
            get { return _dataStartDate; }
        }

        /// <summary>
        /// The Data End Date
        /// </summary>
        public DateTime DataEndDate
        {
            get { return _dataEndDate; }
        }

        /// <summary>
        /// The Chart Start Date
        /// </summary>
        public DateTime ChartStartDate
        {
            get { return _chartStartDate; }
        }

        /// <summary>
        /// The Chart End Date
        /// </summary>
        public DateTime ChartEndDate
        {
            get { return _chartEndDate; }
        }

        #endregion

        #region Calculate Command
        public ICommand CalculateCommand
        {
            get
            {
                if (calculateCommand == null)
                {
                    calculateCommand = new RelayCommand(param => Calculate(), param => CanCalculate );
        		}
                return calculateCommand;
            }
        }

        public void Calculate()
        {
            if (CanCalculate)
            {
                _gann2DayCalculator = new Gann2DayCalculator(_decimalEntryStopRetracement,
                                                             _gannEntryStopRetracementATR,
                                                             _decimalEntryStopExceedHiLo,
                                                             _gannEntryStopExceedHiLoATR,
                                                             _decimalExitStop,
                                                             _gannExitStopATR,
                                                             _useOpenProfit,
                                                             _inputFile,
                                                             _decimalDollarsPerPoint,
                                                             _decimalSkidPoints,
                                                             _decimalStartingEquity,
                                                             _decimalPercentRisk,
                                                             _decimalBenchmarkReturn,
                                                             true);
                _gann2DayCalculator.CalculateTradingSystem();
                _dataStartDate = DateTime.MinValue;
                _dataEndDate = DateTime.MaxValue;
                _chartStartDate = DateTime.MinValue;
                _chartEndDate = DateTime.MaxValue;
                if (_gann2DayCalculator.OHLC.Count > 0)
                {
                    _dataStartDate = _gann2DayCalculator.OHLC[0].Date;
                    _dataEndDate = _gann2DayCalculator.OHLC[_gann2DayCalculator.OHLC.Count - 1].Date;
                    _chartStartDate = _gann2DayCalculator.OHLC[0].Date;
                    _chartEndDate = _chartStartDate.AddDays(DefaultInputs.Gann2DayCALENDARDAYSTODISPLAY);
                    if (_chartEndDate > _dataEndDate)
                    {
                        _chartEndDate = _dataEndDate;
                    }
                    CalculateChart(_chartStartDate, _chartEndDate);
                }
                FireModelCalculated(this, OnGann2DayCalculated);
            }
        }

        public bool CanCalculate
        {
            get
            {
                if (FormValid)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        #endregion

        #region constructor
        public Gann2DayViewModel(IFilePathProvider filePathFinder)
        {
            _filePathFinder = filePathFinder;
            // Set the defaults
            GannEntryStopRetracement = DefaultInputs.Gann2DayENTRYSTOPRETRACEMENT.ToString();
            GannEntryStopExceedHiLo = DefaultInputs.Gann2DayENTRYSTOPEXCEEDHILO.ToString();
            GannExitStop = DefaultInputs.Gann2DayEXITSTOP.ToString();
            InputFile = DefaultInputs.Gann2DayINPUTFILEMESSAGE;
            DollarsPerPoint = DefaultInputs.Gann2DayDOLLARSPERPOINT.ToString();
            SkidPoints = DefaultInputs.Gann2DaySKIDPOINTS.ToString();
            StartingEquity = DefaultInputs.Gann2DaySTARTINGEQUITYDEFAULT.ToString();
            PercentRisk = DefaultInputs.Gann2DayPERCENTRISKDEFAULT.ToString();
            BenchmarkReturn = DefaultInputs.Gann2DayBENCHMARKDEFAULT.ToString();
            _stockChartViewModel = new StockChartViewModel();
            _stockDataSeriesViewModel = new StockDataSeriesViewModel();
            _stockDataSeriesViewModel.StockChartType = StockChartEnumerations.StockChartType.Candle;
            _stockChartViewModel.StockDataSeries = _stockDataSeriesViewModel;
            _stockChartViewModel.StockDataSeries.MajorTrendDownColor = System.Windows.Media.Brushes.Red;
            _stockChartViewModel.StockDataSeries.MajorTrendUpColor = System.Windows.Media.Brushes.Green;
            _gann2DaySwing = new LineDataSeriesViewModel();
            _gann2DaySwing.LineColor = System.Windows.Media.Brushes.Magenta;
            _stockChartViewModel.LineDataSeries.Add(_gann2DaySwing);
            _changeInTrendToUpBuy = new SymbolsViewModel();
            _changeInTrendToUpBuy.SymbolType = StockChartEnumerations.SymbolTypeEnum.Triangle;
            _changeInTrendToUpBuy.FillColor = System.Windows.Media.Brushes.White;
            _changeInTrendToUpBuy.SymbolSize = 12.0;
            _changeInTrendToDownSell = new SymbolsViewModel();
            _changeInTrendToDownSell.SymbolType = StockChartEnumerations.SymbolTypeEnum.InvertedTriangle;
            _changeInTrendToDownSell.FillColor = System.Windows.Media.Brushes.Black;
            _changeInTrendToDownSell.SymbolSize = 12.0;
            _stoppedOut = new SymbolsViewModel();
            _stoppedOut.SymbolType = StockChartEnumerations.SymbolTypeEnum.Square;
            _stoppedOut.FillColor = System.Windows.Media.Brushes.Red;
            _stockChartViewModel.Symbols.Add(_changeInTrendToUpBuy);
            _stockChartViewModel.Symbols.Add(_changeInTrendToDownSell);
            _stockChartViewModel.Symbols.Add(_stoppedOut);
            _pyramidBuy = new SymbolsViewModel();
            _pyramidBuy.SymbolType = StockChartEnumerations.SymbolTypeEnum.Triangle;
            _pyramidBuy.FillColor = System.Windows.Media.Brushes.White;
            _pyramidSell = new SymbolsViewModel();
            _pyramidSell.SymbolType = StockChartEnumerations.SymbolTypeEnum.InvertedTriangle;
            _pyramidSell.FillColor = System.Windows.Media.Brushes.Black;
            _contractRollover = new SymbolsViewModel();
            _contractRollover.SymbolType = StockChartEnumerations.SymbolTypeEnum.Diamond;
            _contractRollover.FillColor = System.Windows.Media.Brushes.Goldenrod;
            _stockChartViewModel.Symbols.Add(_changeInTrendToUpBuy);
            _stockChartViewModel.Symbols.Add(_changeInTrendToDownSell);
            _stockChartViewModel.Symbols.Add(_stoppedOut);
            _stockChartViewModel.Symbols.Add(_pyramidBuy);
            _stockChartViewModel.Symbols.Add(_pyramidSell);
            _stockChartViewModel.Symbols.Add(_contractRollover);
        }
        #endregion

        #region
        public void CalculateChart(DateTime startDate, DateTime endDate)
        {
             decimal min = decimal.MaxValue;
             decimal max = decimal.MinValue;
            // Clear out all the chart data series
            _stockDataSeriesViewModel.OHLC.Clear();
            _gann2DaySwing.Line.Clear();
            _changeInTrendToUpBuy.Symbols.Clear();
            _changeInTrendToDownSell.Symbols.Clear();
            _stoppedOut.Symbols.Clear();
            _pyramidBuy.Symbols.Clear();
            _pyramidSell.Symbols.Clear();
            _contractRollover.Symbols.Clear();
            // Add the OHLC Bars to the chart
            foreach (BarData dataPoint in _gann2DayCalculator.OHLC)
            {
                if ((dataPoint.Date >= startDate) && (dataPoint.Date <= endDate))
                {
                    _stockDataSeriesViewModel.OHLC.Add(dataPoint);
                    if (dataPoint.High > max)
                    {
                        max = dataPoint.High;
                    }
                    if (dataPoint.Low < min)
                    {
                        min = dataPoint.Low;
                    }
                    if (dataPoint.ContractRollover)
                    {
                        _contractRollover.Symbols.Add(new LineData(dataPoint.Date, (dataPoint.High + dataPoint.Low) * 0.5M));
                    }
                }
            }
            // Add the gann 2 day swing line points
            foreach (LineData linePoint in _gann2DayCalculator.Gann2DaySwingList)
            {
                if ((linePoint.Date >= startDate) && (linePoint.Date <= endDate))
                {
                    _gann2DaySwing.Line.Add(linePoint);
                }
            }
            _stockChartViewModel.Xmin = -1.0;
            _stockChartViewModel.Xmax = _stockChartViewModel.DaysDiff(_stockChartViewModel.StockDataSeries.OHLC[0].Date,
                                                                      _stockChartViewModel.StockDataSeries.OHLC[_stockChartViewModel.StockDataSeries.OHLC.Count - 1].Date) + 1.0;
            _stockChartViewModel.Ymax = double.Parse(max.ToString());
            _stockChartViewModel.Ymin = double.Parse(min.ToString());
            // Add the Major Trend to Up/Down Buy/Sell trades
            foreach (LineData symbolPoint in _gann2DayCalculator.MajorUpBuyTrades)
            {
                if ((symbolPoint.Date >= startDate) && (symbolPoint.Date <= endDate))
                {
                    _changeInTrendToUpBuy.Symbols.Add(symbolPoint);
                }
            }
            foreach (LineData symbolPoint in _gann2DayCalculator.MajorDownSellTrades)
            {
                if ((symbolPoint.Date >= startDate) && (symbolPoint.Date <= endDate))
                {
                    _changeInTrendToDownSell.Symbols.Add(symbolPoint);
                }
            }
            foreach (LineData symbolPoint in _gann2DayCalculator.StoppedOutTrades)
            {
                if ((symbolPoint.Date >= startDate) && (symbolPoint.Date <= endDate))
                {
                    _stoppedOut.Symbols.Add(symbolPoint);
                }
            }
            foreach (LineData symbolPoint in _gann2DayCalculator.PyramidBuyTrades)
            {
                if ((symbolPoint.Date >= startDate) && (symbolPoint.Date <= endDate))
                {
                    _pyramidBuy.Symbols.Add(symbolPoint);
                }
            }
            foreach (LineData symbolPoint in _gann2DayCalculator.PyramidSellTrades)
            {
                if ((symbolPoint.Date >= startDate) && (symbolPoint.Date <= endDate))
                {
                    _pyramidSell.Symbols.Add(symbolPoint);
                }
            }
        }
        #endregion

        #region dependency injection
        public void Load()
        {
            string loadFilePath = _filePathFinder.GetLoadPath();
            if (loadFilePath != null)
            {
                InputFile = loadFilePath;
            }
        }
        #endregion

        #region error validation
        public bool FormValid
        {
            get
            {
                _formValid = false;
                ErrorString = this["GannEntryStopRetracement"];
                if (ErrorString.Length == 0)
                {
                    ErrorString = this["GannEntryStopPyramid"];
                    if (ErrorString.Length == 0)
                    {
                        ErrorString = this["GannExitStop"];
                        if (ErrorString.Length == 0)
                        {
                            ErrorString = this["InputFile"];
                            if (ErrorString.Length == 0)
                            {
                                ErrorString = this["DollarsPerPoint"];
                                if (ErrorString.Length == 0)
                                {
                                    ErrorString = this["SkidPoints"];
                                    if (ErrorString.Length == 0)
                                    {
                                        ErrorString = this["StartingEquity"];
                                        if (ErrorString.Length == 0)
                                        {
                                            ErrorString = this["PercentRisk"];
                                            if (ErrorString.Length == 0)
                                            {
                                                ErrorString = this["SkidFraction"];
                                                if (ErrorString.Length == 0)
                                                {
                                                    ErrorString = this["BenchmarkReturn"];
                                                    if (ErrorString.Length == 0)
                                                    {
                                                        _formValid = true;
                                                    }
                                                    return _formValid;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                return _formValid;
            }
        }

        // Note This Property is not used by WPF
        public string Error
        {
            get
            {
                throw new NotImplementedException("This value is not used by WPF");
            }
        }

        public string ErrorString
        {
            get
            {
                return _errorString;
            }
            set
            {
                if (_errorString != value)
                {
                    _errorString = value;
                    FirePropertyChanged("ErrorString");
                }
            }
        }

        public string this[string columnname]
		{
            get 
            {
                string error = string.Empty;
                switch (columnname)
                {
                    case "GannEntryStopRetracement":
                        {
                            _gannEntryStopRetracement = _gannEntryStopRetracement.Trim(' ');
                            if (_gannEntryStopRetracement == null)
                            {
                                error = Properties.Resources.NoEntryStopRetracementEntered;
                            }
                            else if (_gannEntryStopRetracement.Length < 1)
                            {
                                // I don't see how this could happen as we have trimmed and
                                // done the null check above, but put it in just in case
                                error = Properties.Resources.NoEntryStopRetracementEntered;
                            }
                            else if (!decimal.TryParse(_gannEntryStopRetracement, out _decimalEntryStopRetracement))
                            {
                                error = Properties.Resources.EntryStopRetracementNotNumeric;
                            }
                            else if (_decimalEntryStopRetracement < 0)
                            {
                                error = Properties.Resources.EntryStopRetracementLessZero;
                            }
                            break;
                        }
                    case "GannEntryStopExceedHiLo":
                        {
                            _gannEntryStopExceedHiLo = _gannEntryStopExceedHiLo.Trim(' ');
                            if (_gannEntryStopExceedHiLo == null)
                            {
                                error = Properties.Resources.NoEntryStopPyramidEntered;
                            }
                            else if (_gannEntryStopExceedHiLo.Length < 1)
                            {
                                // I don't see how this could happen as we have trimmed and
                                // done the null check above, but put it in just in case
                                error = Properties.Resources.NoEntryStopPyramidEntered;
                            }
                            else if (!decimal.TryParse(_gannEntryStopExceedHiLo, out _decimalEntryStopExceedHiLo))
                            {
                                error = Properties.Resources.EntryStopExceedHiLoNotNumeric;
                            }
                            else if (_decimalEntryStopExceedHiLo < 0)
                            {
                                error = Properties.Resources.EntryStopExceedHiLoLessZero;
                            }
                            break;
                        }
                    case "GannExitStop":
                        {
                            _gannExitStop = _gannExitStop.Trim(' ');
                            if (_gannExitStop == null)
                            {
                                error = Properties.Resources.NoExitStopEntered;
                            }
                            else if (_gannExitStop.Length < 1)
                            {
                                // I don't see how this could happen as we have trimmed and
                                // done the null check above, but put it in just in case
                                error = Properties.Resources.NoExitStopEntered;
                            }
                            else if (!decimal.TryParse(_gannExitStop, out _decimalExitStop))
                            {
                                error = Properties.Resources.ExitStopNotNumeric;
                            }
                            else if (_decimalExitStop < 0)
                            {
                                error = Properties.Resources.ExitStopLessZero;
                            }
                            break;
                        }
                    case "InputFile":
                        {
                            if (!System.IO.File.Exists(_inputFile))
                            {
                                error = Properties.Resources.InputFileMissing;
                            }
                            break;
                        }
                    case "DollarsPerPoint":
                        {
                            _dollarsPerPoint = _dollarsPerPoint.Trim(' ');
                            if (_dollarsPerPoint == null)
                            {
                                error = Properties.Resources.NoDollarsPerPointEntered;
                            }
                            else if (_dollarsPerPoint.Length < 1)
                            {
                                // I don't see how this could happen as we have trimmed and
                                // done the null check above, but put it in just in case
                                error = Properties.Resources.NoDollarsPerPointEntered;
                            }
                            else if (!decimal.TryParse(_dollarsPerPoint, out _decimalDollarsPerPoint))
                            {
                                error = Properties.Resources.DollarsPerPointNotNumeric;
                            }
                            else if (_decimalDollarsPerPoint <= 0)
                            {
                                error = Properties.Resources.DollarsPerPointLessZero;
                            }
                            break;
                        }
                    case "SkidPoints":
                        {
                            _skidPoints = _skidPoints.Trim(' ');
                            if (_skidPoints == null)
                            {
                                error = Properties.Resources.NoSkidPointsEntered;
                            }
                            else if (_skidPoints.Length < 1)
                            {
                                // I don't see how this could happen as we have trimmed and
                                // done the null check above, but put it in just in case
                                error = Properties.Resources.NoSkidPointsEntered;
                            }
                            else if (!decimal.TryParse(_skidPoints, out _decimalSkidPoints))
                            {
                                error = Properties.Resources.SkidPointsNotNumeric;
                            }
                            else if (_decimalSkidPoints < 0)
                            {
                                error = Properties.Resources.SkidPointsLessZero;
                            }
                            break;
                        }
                    case "StartingEquity":
                        {
                            _startingEquity = _startingEquity.Trim(' ');
                            if (_startingEquity == null)
                            {
                                error = Properties.Resources.NoStartingEquityEntered;
                            }
                            else if (_startingEquity.Length < 1)
                            {
                                // I don't see how this could happen as we have trimmed and
                                // done the null check above, but put it in just in case
                                error = Properties.Resources.NoStartingEquityEntered;
                            }
                            else if (!decimal.TryParse(_startingEquity, out _decimalStartingEquity))
                            {
                                error = Properties.Resources.StartingEquityNotNumeric;
                            }
                            else if (_decimalStartingEquity <= 0)
                            {
                                error = Properties.Resources.StartingEquityLessZero;
                            }
                            break;
                        }
                    case "PercentRisk":
                        {
                            _percentRisk = _percentRisk.Trim(' ');
                            if (_percentRisk == null)
                            {
                                error = Properties.Resources.NoPercentRiskEntered;
                            }
                            else if (_percentRisk.Length < 1)
                            {
                                // I don't see how this could happen as we have trimmed and
                                // done the null check above, but put it in just in case
                                error = Properties.Resources.NoPercentRiskEntered;
                            }
                            else if (!decimal.TryParse(_percentRisk, out _decimalPercentRisk))
                            {
                                error = Properties.Resources.PercentRiskNotNumeric;
                            }
                            else if (_decimalPercentRisk <= 0)
                            {
                                error = Properties.Resources.PercentRiskLessZero;
                            }
                            break;
                        }
                    case "BenchmarkReturn":
                        {
                            _benchmarkReturn = _benchmarkReturn.Trim(' ');
                            if (_benchmarkReturn == null)
                            {
                                error = Properties.Resources.NoBenchMarkReturnEntered;
                            }
                            else if (_benchmarkReturn.Length < 1)
                            {
                                // I don't see how this could happen as we have trimmed and
                                // done the null check above, but put it in just in case
                                error = Properties.Resources.NoBenchMarkReturnEntered;
                            }
                            else if (!decimal.TryParse(_benchmarkReturn, out _decimalBenchmarkReturn))
                            {
                                error = Properties.Resources.BenchMarkReturnNotNumeric;
                            }
                            else if (_decimalBenchmarkReturn < 0)
                            {
                                // Note this is a less than rather than
                                // a less than or equal to
                                error = Properties.Resources.SkidFractionLessZero;
                            }
                            break;
                        }
                }
        		return error;
            }
        }
        #endregion

        #region notify mainviewmodel
        private void FireModelCalculated<Gann2DayViewModel>(Gann2DayViewModel item, EventHandler<Gann2DayViewModelEventArgs<Gann2DayViewModel>> handler)
        {
            if (handler != null)
            {
                handler(this, new Gann2DayViewModelEventArgs<Gann2DayViewModel>(item));
            }
        }
        #endregion
    }

    public class Gann2DayViewModelEventArgs<Gann2DayViewModel> : EventArgs
    {
        public Gann2DayViewModel Item
        {
            get;
            private set;
        }

        public Gann2DayViewModelEventArgs(Gann2DayViewModel item)
        {
            Item = item;
        }
    }
}
