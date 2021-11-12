// Copyright (c) 2011, Adam Duval Kangaroo Point South 4169, Australia.
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
    public class SupportResistanceViewModel : ViewModelBase, IDataErrorInfo
    {
        private SupportResistanceCalculator _supportResistanceCalculator;  // The Support and Resistance calculation object
        private StockChartViewModel _stockChartViewModel;                  // The Stock Chart view model
        private StockDataSeriesViewModel _stockDataSeriesViewModel;        // The Stock Chart Data Series (OHLC) view model
        private LineDataSeriesViewModel _longSupport;                      // The Stock Chart Line Series for Long Support
        private LineDataSeriesViewModel _shortSupport;                     // The Stock Chart Line Series for Short Support
        private LineDataSeriesViewModel _longResistance;                   // The Stock chart Line Series for Long Resistance
        private LineDataSeriesViewModel _shortResistance;                  // The Stock Chart Line Series for Short Resistance
        private string _longTermSupportResistanceDays;                     // The Long Term Support/Resistance Days
        private string _shortTermSupportResistanceDays;                    // The Short Term Support/Resistance Days   
        private string _inputFile;                                         // The Input Data File name
        private string _startingEquity;                                    // The Starting Equity
        private string _percentRisk;                                       // The Percent of Capital Risked for the Trade
        private string _skidFraction;                                      // The Skid Fraction to determine Entry/Exit price
        private string _benchmarkReturn;                                   // The Benchmark Return to compare to (For Shape Ratio calculation)
        private IFilePathProvider _filePathFinder;                         // Used to get the input file
        private bool _formValid;                                           // Are the form inputs in valid state
        private string _errorString = string.Empty;                        // The error message
        private int _intLongTermSupportResistanceDays;                     // The Long Term Support/Resistance Days converted to an int
        private int _intShortTermSupportResistanceDays;                    // The Short Term Support/Resistance Days converted to an int
        private decimal _decimalStartingEquity;                            // The Starting Equity converted to a decimal
        private decimal _decimalPercentRisk;                               // The Percent of Capital converted to a decimal
        private decimal _decimalSkidFraction;                              // The Skid Fraction converted to a decimal
        private decimal _decimalBenchmarkReturn;                           // The Benchmark Return converted to a decimal
        private DateTime _dataStartDate = DateTime.MinValue;               // The minimum data date
        private DateTime _dataEndDate = DateTime.MaxValue;                 // The maximum data date
        private DateTime _chartStartDate = DateTime.MinValue;              // The chart start date
        private DateTime _chartEndDate = DateTime.MaxValue;                // The chart end date
        private ICommand calculateCommand;                                 // The calculate Support/Resistance Trading Model command

        public event EventHandler<SupportResistanceViewModelEventArgs<SupportResistanceViewModel>> OnSupportResistanceCalculated;

        #region Accessor Methods
        /// <summary>
        /// The Long Term Support/Resistance Days
        /// </summary>
        public string LongTermSupportResistanceDays
        {
            get
            {
                return _longTermSupportResistanceDays;
            }
            set
            {
                if (_longTermSupportResistanceDays != value)
                {
                    _longTermSupportResistanceDays = value;
                    FirePropertyChanged("LongTermSupportResistanceDays");
                }
            }
        }

        /// <summary>
        /// The Short Term Support/Resistance Days
        /// </summary>
        public string ShortTermSupportResistanceDays
        {
            get
            {
                return _shortTermSupportResistanceDays;
            }
            set
            {
                if (_shortTermSupportResistanceDays != value)
                {
                    _shortTermSupportResistanceDays = value;
                    FirePropertyChanged("ShortTermSupportResistanceDays");
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
                _supportResistanceCalculator = new SupportResistanceCalculator(_intLongTermSupportResistanceDays,
                                                               _intShortTermSupportResistanceDays,
                                                               _inputFile,
                                                               _decimalStartingEquity,
                                                               _decimalPercentRisk,
                                                               _decimalSkidFraction,
                                                               _decimalBenchmarkReturn,
                                                               true);
                _supportResistanceCalculator.CalculateTradingSystem();
                _dataStartDate = DateTime.MinValue;
                _dataEndDate = DateTime.MaxValue;
                _chartStartDate = DateTime.MinValue;
                _chartEndDate = DateTime.MaxValue;
                if (_supportResistanceCalculator.OHLC.Count > 0)
                {
                    _dataStartDate = _supportResistanceCalculator.OHLC[0].Date;
                    _dataEndDate = _supportResistanceCalculator.OHLC[_supportResistanceCalculator.OHLC.Count - 1].Date;
                    _chartStartDate = _supportResistanceCalculator.OHLC[0].Date;
                    _chartEndDate = _chartStartDate.AddDays(DefaultInputs.SupportResistanceCALENDARDAYSTODISPLAY);
                    if (_chartEndDate > _dataEndDate)
                    {
                        _chartEndDate = _dataEndDate;
                    }
                    CalculateChart(_chartStartDate, _chartEndDate);
                }
                FireModelCalculated(this, OnSupportResistanceCalculated);
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
        public SupportResistanceViewModel(IFilePathProvider filePathFinder)
        {
            _filePathFinder = filePathFinder;
            // Set the defaults
            LongTermSupportResistanceDays = DefaultInputs.SupportResistanceLONGTERMDAYSDEFAULT.ToString();
            ShortTermSupportResistanceDays = DefaultInputs.SupportResistanceSHORTTERMDAYSDEFAULT.ToString();
            InputFile = DefaultInputs.SupportResistanceINPUTFILEMESSAGE;
            StartingEquity = DefaultInputs.SupportResistanceSTARTINGEQUITYDEFAULT.ToString();
            PercentRisk = DefaultInputs.SupportResistancePERCENTRISKDEFAULT.ToString();
            SkidFraction = DefaultInputs.SupportResistanceSKIDFRACTIONDEFAULT.ToString();
            BenchmarkReturn = DefaultInputs.SupportResistanceBENCHMARKDEFAULT.ToString();
            _stockChartViewModel = new StockChartViewModel();
            _stockDataSeriesViewModel = new StockDataSeriesViewModel();
            _stockDataSeriesViewModel.StockChartType = StockChartEnumerations.StockChartType.Candle;
            _stockChartViewModel.StockDataSeries = _stockDataSeriesViewModel;
            _longSupport = new LineDataSeriesViewModel();
            _longSupport.LineColor = System.Windows.Media.Brushes.DarkRed;
            _stockChartViewModel.LineDataSeries.Add(_longSupport);
            _shortSupport = new LineDataSeriesViewModel();
            _stockChartViewModel.LineDataSeries.Add(_shortSupport);
            _shortSupport.LineColor = System.Windows.Media.Brushes.Magenta;
            _longResistance = new LineDataSeriesViewModel();
            _longResistance.LineColor = System.Windows.Media.Brushes.Blue;
            _stockChartViewModel.LineDataSeries.Add(_longResistance);
            _shortResistance = new LineDataSeriesViewModel();
            _shortResistance.LineColor = System.Windows.Media.Brushes.Green;
            _stockChartViewModel.LineDataSeries.Add(_shortResistance);
        }
        #endregion

        #region
        public void CalculateChart(DateTime startDate, DateTime endDate)
        {
             decimal min = decimal.MaxValue;
             decimal max = decimal.MinValue;
            // Clear out all the chart data series
            _stockDataSeriesViewModel.OHLC.Clear();
            _longSupport.Line.Clear();
            _shortSupport.Line.Clear();
            _longResistance.Line.Clear();
            _shortResistance.Line.Clear();
            // Add the OHLC Bars to the chart
            foreach (BarData dataPoint in _supportResistanceCalculator.OHLC)
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
                }
            }
            // Add the long support line points
            foreach (LineData linePoint in _supportResistanceCalculator.LongSupportList)
            {
                if ((linePoint.Date >= startDate) && (linePoint.Date <= endDate))
                {
                    _longSupport.Line.Add(linePoint);
                    if (linePoint.Price > max)
                    {
                        max = linePoint.Price;
                    }
                    if (linePoint.Price < min)
                    {
                        min = linePoint.Price;
                    }
                }
            }
            // Add the short support line points
            foreach (LineData linePoint in _supportResistanceCalculator.ShortSupportList)
            {
                if ((linePoint.Date >= startDate) && (linePoint.Date <= endDate))
                {
                    _shortSupport.Line.Add(linePoint);
                    if (linePoint.Price > max)
                    {
                        max = linePoint.Price;
                    }
                    if (linePoint.Price < min)
                    {
                        min = linePoint.Price;
                    }
                }
            }
            // Add the long resistance line points
            foreach (LineData linePoint in _supportResistanceCalculator.LongResistanceList)
            {
                if ((linePoint.Date >= startDate) && (linePoint.Date <= endDate))
                {
                    _longResistance.Line.Add(linePoint);
                    if (linePoint.Price > max)
                    {
                        max = linePoint.Price;
                    }
                    if (linePoint.Price < min)
                    {
                        min = linePoint.Price;
                    }
                }
            }
            // Add the short resistance line points
            foreach (LineData linePoint in _supportResistanceCalculator.ShortResistanceList)
            {
                if ((linePoint.Date >= startDate) && (linePoint.Date <= endDate))
                {
                    _shortResistance.Line.Add(linePoint);
                    if (linePoint.Price > max)
                    {
                        max = linePoint.Price;
                    }
                    if (linePoint.Price < min)
                    {
                        min = linePoint.Price;
                    }
                }
            }
            _stockChartViewModel.Xmin = -1.0;
            _stockChartViewModel.Xmax = _stockChartViewModel.DaysDiff(_stockChartViewModel.StockDataSeries.OHLC[0].Date,
                                                                      _stockChartViewModel.StockDataSeries.OHLC[_stockChartViewModel.StockDataSeries.OHLC.Count - 1].Date) + 1.0;
            _stockChartViewModel.Ymax = double.Parse(max.ToString());
            _stockChartViewModel.Ymin = double.Parse(min.ToString());
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
                ErrorString = this["LongTermSupportResistanceDays"];
                if (ErrorString.Length == 0)
                {
                    ErrorString = this["ShortTermSupportResistanceDays"];
                    if (ErrorString.Length == 0)
                    {
                        ErrorString = this["InputFile"];
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
                                            // No errors in any individual field
                                            if (_intLongTermSupportResistanceDays <= _intShortTermSupportResistanceDays)
                                            {
                                                ErrorString = Properties.Resources.LongTermLessShortTerm;
                                            }
                                            else
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
                    case "LongTermSupportResistanceDays":
                        {
                            _longTermSupportResistanceDays = _longTermSupportResistanceDays.Trim(' ');
                            if (_longTermSupportResistanceDays == null)
                            {
                                error = Properties.Resources.NoLongTermEntered;
                            }
                            else if (_longTermSupportResistanceDays.Length < 1)
                            {
                                // I don't see how this could happen as we have trimmed and
                                // done the null check above, but put it in just in case
                                error = Properties.Resources.NoLongTermEntered;
                            }
                            else if (!int.TryParse(_longTermSupportResistanceDays, out _intLongTermSupportResistanceDays))
                            {
                                error = Properties.Resources.LongTermNotNumeric;
                            }
                            else if (_intLongTermSupportResistanceDays <= 0)
                            {
                                error = Properties.Resources.LongTermLessZero;
                            }
                            break;
                        }
                    case "ShortTermSupportResistanceDays":
                        {
                            _shortTermSupportResistanceDays = _shortTermSupportResistanceDays.Trim(' ');
                            if (_shortTermSupportResistanceDays == null)
                            {
                                error = Properties.Resources.NoShortTermEntered;
                            }
                            else if (_shortTermSupportResistanceDays.Length < 1)
                            {
                                // I don't see how this could happen as we have trimmed and
                                // done the null check above, but put it in just in case
                                error = Properties.Resources.NoShortTermEntered;
                            }
                            else if (!int.TryParse(_shortTermSupportResistanceDays, out _intShortTermSupportResistanceDays))
                            {
                                error = Properties.Resources.ShortTermNotNumeric;
                            }
                            else if (_intShortTermSupportResistanceDays <= 0)
                            {
                                error = Properties.Resources.ShortTermLessZero;
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
                    case "StartingEquity":
                        {
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
                    case "SkidFraction":
                        {
                            if (_skidFraction == null)
                            {
                                error = Properties.Resources.NoSkidFractionEntered;
                            }
                            else if (_skidFraction.Length < 1)
                            {
                                // I don't see how this could happen as we have trimmed and
                                // done the null check above, but put it in just in case
                                error = Properties.Resources.NoSkidFractionEntered;
                            }
                            else if (!decimal.TryParse(_skidFraction, out _decimalSkidFraction))
                            {
                                error = Properties.Resources.SkidFractionNotNumeric;
                            }
                            else if (_decimalSkidFraction < 0)
                            {
                                // Note this is a less than rather than
                                // a less than or equal to
                                error = Properties.Resources.SkidFractionLessZero;
                            }
                            break;
                        }
                    case "BenchmarkReturn":
                        {
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
        private void FireModelCalculated<SupportResistanceViewModel>(SupportResistanceViewModel item, EventHandler<SupportResistanceViewModelEventArgs<SupportResistanceViewModel>> handler)
        {
            if (handler != null)
            {
                handler(this, new SupportResistanceViewModelEventArgs<SupportResistanceViewModel>(item));
            }
        }
        #endregion
    }

    public class SupportResistanceViewModelEventArgs<SupportResistanceViewModel> : EventArgs
    {
        public SupportResistanceViewModel Item
        {
            get;
            private set;
        }

        public SupportResistanceViewModelEventArgs(SupportResistanceViewModel item)
        {
            Item = item;
        }
    }
}

