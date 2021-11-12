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
    public class KrauseGannViewModel : ViewModelBase, IDataErrorInfo
    {
        private KrauseGannCalculator _krauseGannCalculator;                // The Krause Basic System calculation object
        private StockChartViewModel _stockChartViewModel;                  // The Stock Chart view model
        private StockDataSeriesViewModel _stockDataSeriesViewModel;        // The Stock Chart Data Series (OHLC) view model
        private LineDataSeriesViewModel _krauseSwing;                      // The Stock Chart Line Krause Swing Series
        private LineDataSeriesViewModel _krauseSellStops;                  // The Stock Chart Line Krause Sell Stop Series
        private LineDataSeriesViewModel _krauseBuyStops;                   // The Stock Chart Line Krause Buy Stop Series
        private SymbolsViewModel _changeInTrendToUpBuy;                    // The symbol for a change in trend to up buy
        private SymbolsViewModel _changeInTrendToDownSell;                 // The symbol for a change in trend to down sell
        private SymbolsViewModel _profitP1;                                // The symbol for a profit protection #1 trade
        private SymbolsViewModel _profitP2;                                // The symbol for a profit protection #2 trade
        private SymbolsViewModel _pyramidBuy;                              // The main trend up pyramid buy trades
        private SymbolsViewModel _pyramidSell;                             // The main trend down pyramid sell trades
        private SymbolsViewModel _contractRollover;                        // A contract rollover day
        private string _krauseEntryStopRetracement;                        // The Entry Stop for Retracement
        private string _krauseEntryStopExceedHiLo;                         // The Entry Stop for ExceedHiLo
        private string _krauseRetracementFraction;                         // The Krause Retracement Stop Retracement Fraction
        private bool _krauseUseRetracementStop = true;                     // Use the Retracement Stop
        private bool _krauseFixContracts = true;                           // Fix the number of contracts for it stays constant
        private string _inputFile;                                         // The Input Data File name
        private string _dollarsPerPoint;                                   // The number of dollars per point of price movement
        private string _skidPoints;                                        // The number of points of skid for buy/sell orders
        private string _startingEquity;                                    // The Starting Equity
        private string _krauseNoContracts;                                 // The number if contracts/$10000 equity for Krause Gann
        private string _skidFraction;                                      // The Skid Fraction to determine Entry/Exit price
        private string _tickSize;                                          // The Tick Size for the contract being traded
        private string _benchmarkReturn;                                   // The Benchmark Return to compare to (For Shape Ratio calculation)
        private IFilePathProvider _filePathFinder;                         // Used to get the input file
        private bool _formValid;                                           // Are the form inputs in valid state
        private string _errorString = string.Empty;                        // The error message
        private decimal _decimalEntryStopRetracement;                      // The Entry stop for Retracement converted to a decimal
        private decimal _decimalEntryStopExceedHiLo;                       // The Entry stop for Exceed Hi Lo converted to a decimal
        private decimal _decimalRetracementFraction;                       // The Retracement Fraction converted to a decimal
        private decimal _decimalDollarsPerPoint;                           // The Dollars per Point of Price movement converted to decimal
        private decimal _decimalSkidPoints;                                // The Skid Points for Buy/Sell orders converted to decimal
        private decimal _decimalStartingEquity;                            // The Starting Equity converted to a decimal
        private decimal _decimalNoContracts;                               // The No. of Contracts/$10000 equity converted to a decimal
        private decimal _decimalTickSize;                                  // The tick size for the contract being traded converted to decimal
        private decimal _decimalBenchmarkReturn;                           // The Benchmark Return converted to a decimal
        private DateTime _dataStartDate = DateTime.MinValue;               // The minimum data date
        private DateTime _dataEndDate = DateTime.MaxValue;                 // The maximum data date
        private DateTime _chartStartDate = DateTime.MinValue;              // The chart start date
        private DateTime _chartEndDate = DateTime.MaxValue;                // The chart end date
        private ICommand calculateCommand;                                 // The calculate Support/Resistance Trading Model command

        public event EventHandler<KrauseGannViewModelEventArgs<KrauseGannViewModel>> OnKrauseGannCalculated;

        #region Accessor Methods
        /// <summary>
        /// The Krause Entry Stop for Retracement
        /// </summary>
        public string KrauseEntryStopRetracement
        {
            get
            {
                return _krauseEntryStopRetracement;
            }
            set
            {
                if (_krauseEntryStopRetracement != value)
                {
                    _krauseEntryStopRetracement = value;
                    FirePropertyChanged("KrauseEntryStopRetracement");
                }
            }
        }

        /// <summary>
        /// The Krause Entry Stop for Exceed HiLo
        /// </summary>
        public string KrauseEntryStopExceedHiLo
        {
            get
            {
                return _krauseEntryStopExceedHiLo;
            }
            set
            {
                if (_krauseEntryStopExceedHiLo != value)
                {
                    _krauseEntryStopExceedHiLo = value;
                    FirePropertyChanged("KrauseEntryStopExceedHiLo");
                }
            }
        }

        /// <summary>
        /// The Krause Retracement Exit Stop Retracement Fraction
        /// </summary>
        public string KrauseRetracementFraction
        {
            get
            {
                return _krauseRetracementFraction;
            }
            set
            {
                if (_krauseRetracementFraction != value)
                {
                    _krauseRetracementFraction = value;
                    FirePropertyChanged("KrauseRetracementFraction");
                }
            }
        }

        /// <summary>
        /// Use the Krause Retracement Stop or no
        /// </summary>
        public bool KrauseUseRetracementStop
        {
            get
            {
                return _krauseUseRetracementStop;
            }
            set
            {
                if (_krauseUseRetracementStop != value)
                {
                    _krauseUseRetracementStop = value;
                    FirePropertyChanged("KrauseUseRetracementStop");
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
        /// The Number of Contarcts per $10000 of account equity
        /// </summary>
        public string KrauseNoContracts
        {
            get
            {
                return _krauseNoContracts;
            }
            set
            {
                if (_krauseNoContracts != value)
                {
                    _krauseNoContracts = value;
                    FirePropertyChanged("KrauseNoContracts");
                }
            }
        }

        /// <summary>
        /// Fix the number of contracts to be constant
        /// </summary>
        public bool KrauseFixContracts
        {
            get
            {
                return _krauseFixContracts;
            }
            set
            {
                if (_krauseFixContracts != value)
                {
                    _krauseFixContracts = value;
                    FirePropertyChanged("KrauseFixContracts");
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
        /// The Tick Size for the contract being traded
        /// </summary>
        public string TickSize
        {
            get
            {
                return _tickSize;
            }
            set
            {
                if (_tickSize != value)
                {
                    _tickSize = value;
                    FirePropertyChanged("TickSize");
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
                    calculateCommand = new RelayCommand(param => Calculate(), param => CanCalculate);
                }
                return calculateCommand;
            }
        }

        public void Calculate()
        {
            if (CanCalculate)
            {
                _krauseGannCalculator = new KrauseGannCalculator(_decimalEntryStopRetracement,
                                                                 _decimalEntryStopExceedHiLo,
                                                                 _decimalRetracementFraction,
                                                                 _inputFile,
                                                                 _decimalDollarsPerPoint,
                                                                 _decimalSkidPoints,
                                                                 _decimalStartingEquity,
                                                                 _decimalBenchmarkReturn,
                                                                 true);
                _krauseGannCalculator.CalculateTradingSystem();
                _dataStartDate = DateTime.MinValue;
                _dataEndDate = DateTime.MaxValue;
                _chartStartDate = DateTime.MinValue;
                _chartEndDate = DateTime.MaxValue;
                if (_krauseGannCalculator.OHLC.Count > 0)
                {
                    _dataStartDate =_krauseGannCalculator.OHLC[0].Date;
                    _dataEndDate = _krauseGannCalculator.OHLC[_krauseGannCalculator.OHLC.Count - 1].Date;
                    _chartStartDate = _krauseGannCalculator.OHLC[0].Date;
                    _chartEndDate = _chartStartDate.AddDays(DefaultInputs.KrauseCALENDARDAYSTODISPLAY);
                    if (_chartEndDate > _dataEndDate)
                    {
                        _chartEndDate = _dataEndDate;
                    }
                    CalculateChart(_chartStartDate, _chartEndDate);
                }
                FireModelCalculated(this, OnKrauseGannCalculated);
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
        public KrauseGannViewModel(IFilePathProvider filePathFinder)
        {
            _filePathFinder = filePathFinder;
            // Set the defaults
            KrauseEntryStopRetracement = DefaultInputs.KrauseENTRYSTOPRETRACEMENT.ToString();
            KrauseEntryStopExceedHiLo = DefaultInputs.KrauseENTRYSTOPEXCEEDHILO.ToString();
            KrauseRetracementFraction = DefaultInputs.KrauseRETRACEMENTFRACTION.ToString();
            InputFile = DefaultInputs.KrauseINPUTFILEMESSAGE;
            DollarsPerPoint = DefaultInputs.KrauseDOLLARSPERPOINT.ToString();
            SkidPoints = DefaultInputs.KrauseSKIDPOINTS.ToString();
            StartingEquity = DefaultInputs.KrauseSTARTINGEQUITYDEFAULT.ToString();
            KrauseNoContracts = DefaultInputs.KrauseNOOFCONTRACTS.ToString();
            TickSize = DefaultInputs.KrauseTICKSIZE.ToString();
            BenchmarkReturn = DefaultInputs.KrauseBENCHMARKDEFAULT.ToString();
            _stockChartViewModel = new StockChartViewModel();
            _stockDataSeriesViewModel = new StockDataSeriesViewModel();
            _stockDataSeriesViewModel.StockChartType = StockChartEnumerations.StockChartType.HiLoOpenClose;
            _stockChartViewModel.StockDataSeries = _stockDataSeriesViewModel;
            _stockChartViewModel.StockDataSeries.MajorTrendDownColor = System.Windows.Media.Brushes.Red;
            _stockChartViewModel.StockDataSeries.MajorTrendUpColor = System.Windows.Media.Brushes.Green;
            _krauseSwing = new LineDataSeriesViewModel();
            _krauseSwing.LineColor = System.Windows.Media.Brushes.Magenta;
            _krauseSellStops = new LineDataSeriesViewModel();
            _krauseSellStops.LineColor = System.Windows.Media.Brushes.OrangeRed;
            _krauseBuyStops = new LineDataSeriesViewModel();
            _krauseBuyStops.LineColor = System.Windows.Media.Brushes.LightSeaGreen;
            _stockChartViewModel.LineDataSeries.Add(_krauseSwing);
            _stockChartViewModel.LineDataSeries.Add(_krauseSellStops);
            _stockChartViewModel.LineDataSeries.Add(_krauseBuyStops);
            _changeInTrendToUpBuy = new SymbolsViewModel();
            _changeInTrendToUpBuy.SymbolType = StockChartEnumerations.SymbolTypeEnum.Triangle;
            _changeInTrendToUpBuy.FillColor = System.Windows.Media.Brushes.White;
            _changeInTrendToUpBuy.SymbolSize = 12.0;
            _changeInTrendToDownSell = new SymbolsViewModel();
            _changeInTrendToDownSell.SymbolType = StockChartEnumerations.SymbolTypeEnum.InvertedTriangle;
            _changeInTrendToDownSell.FillColor = System.Windows.Media.Brushes.Black;
            _changeInTrendToDownSell.SymbolSize = 12.0;
            _profitP1 = new SymbolsViewModel();
            _profitP1.SymbolType = StockChartEnumerations.SymbolTypeEnum.Square;
            _profitP1.FillColor = System.Windows.Media.Brushes.Red;
            _profitP2 = new SymbolsViewModel();
            _profitP2.SymbolType = StockChartEnumerations.SymbolTypeEnum.Square;
            _profitP2.FillColor = System.Windows.Media.Brushes.Orange;
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
            _stockChartViewModel.Symbols.Add(_profitP1);
            _stockChartViewModel.Symbols.Add(_profitP2);
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
            _krauseSwing.Line.Clear();
            _krauseSellStops.Line.Clear();
            _krauseBuyStops.Line.Clear();
            _changeInTrendToUpBuy.Symbols.Clear();
            _changeInTrendToDownSell.Symbols.Clear();
            _profitP1.Symbols.Clear();
            _profitP2.Symbols.Clear();
            _pyramidBuy.Symbols.Clear();
            _pyramidSell.Symbols.Clear();
            _contractRollover.Symbols.Clear();
            // Add the OHLC Bars to the chart
            foreach (BarData dataPoint in _krauseGannCalculator.OHLC)
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
            foreach (LineData linePoint in _krauseGannCalculator.KrauseSwingList)
            {
                if ((linePoint.Date >= startDate) && (linePoint.Date <= endDate))
                {
                    _krauseSwing.Line.Add(linePoint);
                }
            }
            // Add the sell stops
            foreach (LineData linePoint in _krauseGannCalculator.SellStops)
            {
                if ((linePoint.Date >= startDate) && (linePoint.Date <= endDate))
                {
                    _krauseSellStops.Line.Add(linePoint);
                }
            }
            // Add the buy stops
            foreach (LineData linePoint in _krauseGannCalculator.BuyStops)
            {
                if ((linePoint.Date >= startDate) && (linePoint.Date <= endDate))
                {
                    _krauseBuyStops.Line.Add(linePoint);
                }
            }
            _stockChartViewModel.Xmin = -1.0;
            _stockChartViewModel.Xmax = _stockChartViewModel.DaysDiff(_stockChartViewModel.StockDataSeries.OHLC[0].Date,
                                                                      _stockChartViewModel.StockDataSeries.OHLC[_stockChartViewModel.StockDataSeries.OHLC.Count - 1].Date) + 1.0;
            _stockChartViewModel.Ymax = double.Parse(max.ToString());
            _stockChartViewModel.Ymin = double.Parse(min.ToString());
            // Add the Major Trend to Up/Down Buy/Sell trades
            foreach (LineData symbolPoint in _krauseGannCalculator.MajorUpBuyTrades)
            {
                if ((symbolPoint.Date >= startDate) && (symbolPoint.Date <= endDate))
                {
                    _changeInTrendToUpBuy.Symbols.Add(symbolPoint);
                }
            }
            foreach (LineData symbolPoint in _krauseGannCalculator.MajorDownSellTrades)
            {
                if ((symbolPoint.Date >= startDate) && (symbolPoint.Date <= endDate))
                {
                    _changeInTrendToDownSell.Symbols.Add(symbolPoint);
                }
            }
            foreach (LineData symbolPoint in _krauseGannCalculator.ProfitP1Trades)
            {
                if ((symbolPoint.Date >= startDate) && (symbolPoint.Date <= endDate))
                {
                    _profitP1.Symbols.Add(symbolPoint);
                }
            }
            foreach (LineData symbolPoint in _krauseGannCalculator.ProfitP2Trades)
            {
                if ((symbolPoint.Date >= startDate) && (symbolPoint.Date <= endDate))
                {
                    _profitP2.Symbols.Add(symbolPoint);
                }
            }
            foreach (LineData symbolPoint in _krauseGannCalculator.PyramidBuyTrades)
            {
                if ((symbolPoint.Date >= startDate) && (symbolPoint.Date <= endDate))
                {
                    _pyramidBuy.Symbols.Add(symbolPoint);
                }
            }
            foreach (LineData symbolPoint in _krauseGannCalculator.PyramidSellTrades)
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
                ErrorString = this["KrauseEntryStopRetracement"];
                if (ErrorString.Length == 0)
                {
                    ErrorString = this["KrauseEntryStopPyramid"];
                    if (ErrorString.Length == 0)
                    {
                        ErrorString = this["KrauseRetracementFraction"];
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
                                            ErrorString = this["KrauseNoContracts"];
                                            if (ErrorString.Length == 0)
                                            {
                                                ErrorString = this["TickSize"];
                                                if (ErrorString.Length == 0)
                                                {
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
                    case "KrauseEntryStopRetracement":
                        {
                            _krauseEntryStopRetracement = _krauseEntryStopRetracement.Trim(' ');
                            if (_krauseEntryStopRetracement == null)
                            {
                                error = Properties.Resources.NoEntryStopRetracementEntered;
                            }
                            else if (_krauseEntryStopRetracement.Length < 1)
                            {
                                // I don't see how this could happen as we have trimmed and
                                // done the null check above, but put it in just in case
                                error = Properties.Resources.NoEntryStopRetracementEntered;
                            }
                            else if (!decimal.TryParse(_krauseEntryStopRetracement, out _decimalEntryStopRetracement))
                            {
                                error = Properties.Resources.EntryStopRetracementNotNumeric;
                            }
                            else if (_decimalEntryStopRetracement < 0)
                            {
                                error = Properties.Resources.EntryStopRetracementLessZero;
                            }
                            break;
                        }
                    case "KrauseEntryStopExceedHiLo":
                        {
                            _krauseEntryStopExceedHiLo = _krauseEntryStopExceedHiLo.Trim(' ');
                            if (_krauseEntryStopExceedHiLo == null)
                            {
                                error = Properties.Resources.NoEntryStopPyramidEntered;
                            }
                            else if (_krauseEntryStopExceedHiLo.Length < 1)
                            {
                                // I don't see how this could happen as we have trimmed and
                                // done the null check above, but put it in just in case
                                error = Properties.Resources.NoEntryStopPyramidEntered;
                            }
                            else if (!decimal.TryParse(_krauseEntryStopExceedHiLo, out _decimalEntryStopExceedHiLo))
                            {
                                error = Properties.Resources.EntryStopExceedHiLoNotNumeric;
                            }
                            else if (_decimalEntryStopExceedHiLo < 0)
                            {
                                error = Properties.Resources.EntryStopExceedHiLoLessZero;
                            }
                            break;
                        }
                    case "KrauseRetracementFraction":
                        {
                            error = String.Empty;
                            if (_krauseUseRetracementStop)
                            {
                                _krauseRetracementFraction = _krauseRetracementFraction.Trim(' ');
                                if (_krauseRetracementFraction == null)
                                {
                                    error = Properties.Resources.NoRetracementFractionEntered;
                                }
                                else if (_krauseRetracementFraction.Length < 1)
                                {
                                    // I don't see how this could happen as we have trimmed and
                                    // done the null check above, but put it in just in case
                                    error = Properties.Resources.NoRetracementFractionEntered;
                                }
                                else if (!decimal.TryParse(_krauseRetracementFraction, out _decimalRetracementFraction))
                                {
                                    error = Properties.Resources.RetracementFractionNotNumeric;
                                }
                                else if (_decimalRetracementFraction < 0)
                                {
                                    error = Properties.Resources.RetracementFractionLessZero;
                                }
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
                    case "KrauseNoContracts":
                        {
                            _krauseNoContracts = _krauseNoContracts.Trim(' ');
                            if (_krauseNoContracts == null)
                            {
                                error = Properties.Resources.NoContractsEntered;
                            }
                            else if (_krauseNoContracts.Length < 1)
                            {
                                // I don't see how this could happen as we have trimmed and
                                // done the null check above, but put it in just in case
                                error = Properties.Resources.NoContractsEntered;
                            }
                            else if (!decimal.TryParse(_krauseNoContracts, out _decimalNoContracts))
                            {
                                error = Properties.Resources.NoContractsNotNumeric;
                            }
                            else if (_decimalNoContracts <= 0)
                            {
                                error = Properties.Resources.NoContractsLessZero;
                            }
                            break;
                        }
                    case "TickSize":
                        {
                            _tickSize = _tickSize.Trim(' ');
                            if (_tickSize == null)
                            {
                                error = Properties.Resources.NoTickSizeEntered;
                            }
                            else if (_tickSize.Length < 1)
                            {
                                // I don't see how this could happen as we have trimmed and
                                // done the null check above, but put it in just in case
                                error = Properties.Resources.NoTickSizeEntered;
                            }
                            else if (!decimal.TryParse(_tickSize, out _decimalTickSize))
                            {
                                error = Properties.Resources.TickSizeNotNumeric;
                            }
                            else if (_decimalTickSize <= 0)
                            {
                                error = Properties.Resources.TickSizeLessZero;
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
        private void FireModelCalculated<KrauseGannViewModel>(KrauseGannViewModel item, EventHandler<KrauseGannViewModelEventArgs<KrauseGannViewModel>> handler)
        {
            if (handler != null)
            {
                handler(this, new KrauseGannViewModelEventArgs<KrauseGannViewModel>(item));
            }
        }
        #endregion
    }

    public class KrauseGannViewModelEventArgs<KrauseGannViewModel> : EventArgs
    {
        public KrauseGannViewModel Item
        {
            get;
            private set;
        }

        public KrauseGannViewModelEventArgs(KrauseGannViewModel item)
        {
            Item = item;
        }
    }
}


