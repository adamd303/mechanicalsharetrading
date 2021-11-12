// Free to use, modify, distribute
// No warranty
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace OneDaySwing
{
    public class OneDaySwingData
    {
        #region Constants
        private const string DEFAULTOUTPUTFILENAME = "Output.csv";
        private const string ALLTURNSFILENAME = "AllTurns.csv";
        #endregion
        #region Class private variables
        private Enumerations.Trend _majorTrend;       // The Major Trend based on the Gann 2 Day Swing Chart
        private Enumerations.Trend _minorTrend;       // The Minor Trend based on the Gann 2 Day Swing chart
        private List<LineData> _gann1DaySwingList;    // The list of Gann 1Day swing points
        private List<DateTime> _outsideDayList;       // The list of outside day dates
        private List<DateTime> _allTurnDates;         // The list of dates for all turns
        private List<BarData> _ohlc;                  // The Open, High, Low, Close data from the input file
        private List<BarData> _outputOHLC;            // The OHLC etc. data to output
        private string _outputFileName;               // The output file name
        private StreamWriter _outputFile;             // The output file streamwriter
        private StreamWriter _allTurnsFile;           // The output all turns file streamwriter
        #endregion

        #region Accessor Methods
        /// <summary>
        /// The Data File name
        /// </summary>
        public string DataFile { get; private set; }

        /// <summary>
        /// The list of Open, High, Low close data from the input data file
        /// </summary>
        public List<BarData> OHLC
        {
            get { return _ohlc;  }
        }

        /// <summary>
        /// The list of two day swing data, calculated
        /// </summary>
        public List<LineData> Gann1DaySwingList
        {
            get { return _gann1DaySwingList; }
        }
        #endregion

        #region The Contructor
        /// <summary>
        /// The constructor
        /// </summary>
        /// <param name="dataFile">The Data File name.</param>
        /// <param name="outputFileName">The Output File name.</param>
        public OneDaySwingData(string dataFile, string outputFileName)
        {
            DataFile = dataFile;
            _outputFileName = outputFileName;
        }
        #endregion

        #region The Main Calculation Routine
        public void CalculateData()
        {       
            _ohlc = new List<BarData>();
            _outputOHLC = new List<BarData>();
            _gann1DaySwingList = new List<LineData>();
            _outsideDayList = new List<DateTime>();
            _allTurnDates = new List<DateTime>();
            _majorTrend = Enumerations.Trend.Unknown;
            _minorTrend = Enumerations.Trend.Unknown;
            // Open the output file
            if ((_outputFileName != null) && (_outputFileName != String.Empty))
            {
                _outputFile = File.CreateText(_outputFileName);
            }
            else
            {   
                _outputFile = File.CreateText(DEFAULTOUTPUTFILENAME);
            }
            _allTurnsFile = File.CreateText(ALLTURNSFILENAME);
            try
            {
                BarDataReader barDataReader = new BarDataReader(DataFile);
                if (barDataReader.OHLC.Count >= 2)
                {
                    // Initialise the previous day's data
                    DateTime prevDate = barDataReader.OHLC[0].Date;
                    decimal prevOpen = (decimal)barDataReader.OHLC[0].Open;
                    decimal prevHigh = (decimal)barDataReader.OHLC[0].High;
                    decimal prevLow = (decimal)barDataReader.OHLC[0].Low;
                    decimal prevClose = (decimal)barDataReader.OHLC[0].Close;
                    _ohlc.Add(barDataReader.OHLC[0]);
                    // The highest high since a minor trend change and lowest low since a minor trend change
                    decimal lowestLowSinceMinorTrendChange = 0.0M;
                    decimal highestHighSinceMinorTrendChange = 0.0M;
                    decimal highAtMinorTrendChangeToUp = 0.0M;
                    decimal lowAtMinorTrendChangeToDown = 0.0M;
                    for (int i = 2; i < barDataReader.OHLC.Count; i++)
                    {
                        // Get the next buffer data
                        DateTime date = barDataReader.OHLC[i].Date;
                        decimal open = (decimal)barDataReader.OHLC[i].Open;
                        decimal high = (decimal)barDataReader.OHLC[i].High;
                        decimal low = (decimal)barDataReader.OHLC[i].Low;
                        decimal close = (decimal)barDataReader.OHLC[i].Close;
                        // Recalculate the trend
                        // The minor trend
                        if (_minorTrend == Enumerations.Trend.Up)
                        {
                            if (high > prevHigh)
                            {
                                // Mod. to Ganns rule. If we have higher highs and trend is Up, keep going Up
                                _minorTrend = Enumerations.Trend.Up;
                                // If low is less than previous low, this is an outside day and we need to add it to the list
                                if (low < prevLow)
                                {
                                    _outsideDayList.Add(date);
                                }
                            }
                            else if (low < prevLow)
                            {
                                _minorTrend = Enumerations.Trend.Down;
                                // So we have had a top recently
                                // Go from i to the bottom (which must be the previous point in the _gann2DaySwingList)
                                // and find the point and add it to the list.
                                // We only look at the current date (actually nextDate) and dates for which the trend is UP
                                DateTime highDate = date;
                                decimal highValue = high;
                                int highIndex = i;
                                for (int j = i; j >= _gann1DaySwingList[_gann1DaySwingList.Count-1].Index; j--)
                                {
                                    if ((barDataReader.OHLC[j].MinorTrend == Enumerations.Trend.Up) && (barDataReader.OHLC[j].High > highValue))
                                    {
                                        highValue = (decimal)barDataReader.OHLC[j].High;
                                        highDate = barDataReader.OHLC[j].Date;
                                        highIndex = j;
                                    }
                                }
                                _gann1DaySwingList.Add(new LineData(highDate, highValue, highIndex, Enumerations.TopBottom.Top));
                                lowestLowSinceMinorTrendChange = low;
                                highestHighSinceMinorTrendChange = high;
                                lowAtMinorTrendChangeToDown = low;
                            }
                            else
                            {
                                _minorTrend = Enumerations.Trend.Up;  // The minor Trend remains UP
                            }
                        }
                        else if (_minorTrend == Enumerations.Trend.Down)
                        {
                            if (low < prevLow)
                            {
                                // Mod. to Ganns rule. If we have lower lows and trend is Down, keep going Down
                                _minorTrend = Enumerations.Trend.Down;
                                // If high is greater than previous low, this is an outside day and we need to add it to the list
                                if (high > prevHigh)
                                {
                                    _outsideDayList.Add(date);
                                }
                            }
                            if ((high >= prevHigh) && (low >= prevLow))
                            {
                                _minorTrend = Enumerations.Trend.Up;
                                // So we have had a bottom recently
                                // Go from i to the top (which must be the previous point in the _gann2DaySwingList)
                                // and find the point and add it to the list.
                                // We only look at the currrent date (actually nextDate) and dates for which the trend is DOWN
                                DateTime lowDate = date;
                                decimal lowValue = low;
                                int lowIndex = i;
                                for (int j = i; j >= _gann1DaySwingList[_gann1DaySwingList.Count - 1].Index; j--)
                                {
                                    if ((barDataReader.OHLC[j].MinorTrend == Enumerations.Trend.Down) && (barDataReader.OHLC[j].Low < lowValue))
                                    {
                                        lowValue = (decimal)barDataReader.OHLC[j].Low;
                                        lowDate = barDataReader.OHLC[j].Date;
                                        lowIndex = j;
                                    }
                                }
                                _gann1DaySwingList.Add(new LineData(lowDate, lowValue, lowIndex, Enumerations.TopBottom.Bottom));
                                lowestLowSinceMinorTrendChange = low;
                                highestHighSinceMinorTrendChange = high;
                                highAtMinorTrendChangeToUp = high;
                            }
                            else
                            {
                                _minorTrend = Enumerations.Trend.Down;  // The minor trend remains down
                            }
                        }
                        else if (_minorTrend == Enumerations.Trend.Unknown)
                        {
                            if ((high >= prevHigh) && (low >= prevLow))
                            {
                                _minorTrend = Enumerations.Trend.Up;  // The minor trend has changed to UP
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
                                        lowValue = (decimal)barDataReader.OHLC[j].Low;
                                        lowDate = barDataReader.OHLC[j].Date;
                                        lowIndex = j;
                                    }
                                }
                                _gann1DaySwingList.Add(new LineData(lowDate, lowValue, lowIndex, Enumerations.TopBottom.Bottom));
                                lowestLowSinceMinorTrendChange = low;
                                highestHighSinceMinorTrendChange = high;
                                highAtMinorTrendChangeToUp = high;
                            }
                            else if (low < prevLow)
                            {
                                _minorTrend = Enumerations.Trend.Down;  // The minor trend has changed to DOWN
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
                                        highValue = (decimal)barDataReader.OHLC[j].High;
                                        highDate = barDataReader.OHLC[j].Date;
                                        highIndex = j;
                                    }
                                }
                                _gann1DaySwingList.Add(new LineData(highDate, highValue, highIndex, Enumerations.TopBottom.Top));
                                lowestLowSinceMinorTrendChange = low;
                                highestHighSinceMinorTrendChange = high;
                                lowAtMinorTrendChangeToDown = low;
                            }                                 
                        }
                        if (low < lowestLowSinceMinorTrendChange)
                        {
                            lowestLowSinceMinorTrendChange = low;
                        }
                        if (high > highestHighSinceMinorTrendChange)
                        {
                            highestHighSinceMinorTrendChange = high;
                        }
                        // Add the data to the list 
                        _ohlc.Add(barDataReader.OHLC[i]);
                        _ohlc[_ohlc.Count - 1].MajorTrend = _majorTrend;
                        _ohlc[_ohlc.Count - 1].MinorTrend = _minorTrend;
                        // Update the values for the next pass through
                        prevDate = date;
                        prevOpen = open;
                        prevHigh = high;
                        prevLow = low;
                        prevClose = close;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            // There is probably a more efficient way of doing this than looping through at the end
            // but this will do for now
            if (_gann1DaySwingList.Count >= 2)
            {
                // Add all the no trading days and weekends to the output list
                for (int i = 0; i < _ohlc.Count - 1; i++)
                {
                    // Add the current day to the output list
                    _outputOHLC.Add(new BarData(_ohlc[i].Date, _ohlc[i].Open, _ohlc[i].High, _ohlc[i].Low, _ohlc[i].Close, _ohlc[i].Volume));
                    _outputOHLC[_outputOHLC.Count - 1].MajorTrend = _ohlc[i].MajorTrend;
                    _outputOHLC[_outputOHLC.Count - 1].MinorTrend = _ohlc[i].MinorTrend;
                    DateTime currentDay = _ohlc[i].Date;
                    DateTime nextDay = _ohlc[i + 1].Date;
                    int daysDiff = (int)(nextDay.Subtract(currentDay)).TotalDays;
                    if (daysDiff > 1)
                    {
                        // There has been a weekend or non-trading day(s)
                        // between the last two dates
                        for (int j = 1; j < daysDiff; j++)
                        {
                            // Add the missing date(s) with no ohlc data to the list
                            _outputOHLC.Add(new BarData(_ohlc[i].Date.AddDays(j), null, null, null, null, null));
                            _outputOHLC[_outputOHLC.Count - 1].MajorTrend = _ohlc[i].MajorTrend;
                            _outputOHLC[_outputOHLC.Count - 1].MinorTrend = _ohlc[i].MinorTrend;
                        }
                    }
                }
                // Add the last day to the output list
                _outputOHLC.Add(new BarData(_ohlc[_ohlc.Count - 1].Date, _ohlc[_ohlc.Count - 1].Open, _ohlc[_ohlc.Count - 1].High, _ohlc[_ohlc.Count - 1].Low, _ohlc[_ohlc.Count - 1].Close, _ohlc[_ohlc.Count - 1].Volume));
                _outputOHLC[_outputOHLC.Count - 1].MajorTrend = _ohlc[_ohlc.Count - 1].MajorTrend;
                _outputOHLC[_outputOHLC.Count - 1].MinorTrend = _ohlc[_ohlc.Count - 1].MinorTrend;
                for (int i = 0; i < _gann1DaySwingList.Count - 1; i++)
                {
                    int lowerLimit = -1;
                    int upperLimit = -1;
                    for (int j = 0; j < _outputOHLC.Count; j++)
                    {
                        if (_outputOHLC[j].Date == _gann1DaySwingList[i].Date)
                        {
                            _outputOHLC[j].RealPivotPrice = _gann1DaySwingList[i].Price;
                            _outputOHLC[j].InterpolatedPivotPrice = _gann1DaySwingList[i].Price;
                            lowerLimit = j;
                        }
                        if (lowerLimit > -1)
                        {
                            if (_outputOHLC[j].Date == _gann1DaySwingList[i + 1].Date)
                            {
                                _outputOHLC[j].RealPivotPrice = _gann1DaySwingList[i + 1].Price;
                                _outputOHLC[j].InterpolatedPivotPrice = _gann1DaySwingList[i + 1].Price;
                                upperLimit = j;
                            }
                        }
                    }
                    if ((lowerLimit > -1) && (upperLimit > -1))
                    {
                        // Find the date difference in days between the upper and lower limits
                        decimal totalDaysDiff = (decimal)(_outputOHLC[upperLimit].Date.Subtract(_outputOHLC[lowerLimit].Date)).TotalDays;
                        _outputOHLC[upperLimit].DaysUpDown = (uint)totalDaysDiff;
                        for (int j = lowerLimit + 1; j < upperLimit; j++)
                        {
                            // Find the current day diff to interpolate
                            decimal currentDaysDiff = (decimal)(_outputOHLC[j].Date.Subtract(_outputOHLC[lowerLimit].Date)).TotalDays;
                            // Interpolate for the values between the pivots
                            _outputOHLC[j].InterpolatedPivotPrice = _outputOHLC[lowerLimit].InterpolatedPivotPrice +
                                                                    (_outputOHLC[upperLimit].InterpolatedPivotPrice - _outputOHLC[lowerLimit].InterpolatedPivotPrice) /
                                                                    totalDaysDiff * currentDaysDiff;
                        }
                    }
                    else
                    {
                        System.Console.WriteLine("Error performing pivot interpolation, date = " + _gann1DaySwingList[i].Date + ", upper limit = " + upperLimit.ToString() + ", lower limit = " + lowerLimit.ToString());
                    }
                    if (!_allTurnDates.Contains(_gann1DaySwingList[i].Date))
                    {
                        _allTurnDates.Add(_gann1DaySwingList[i].Date);
                    }
                }
                // Write the headers to the output file
                _outputFile.WriteLine("Date, Minor Trend, Pivot Price, Interpolated Pivot Price");
                foreach (BarData ohlc in _outputOHLC)
                {
                    // Write the data to the output file
                    _outputFile.WriteLine(ohlc.Date.ToString("dd/MM/yyyy") + ", " +
                                          ohlc.MinorTrend.ToString() + ", " +
                                          ohlc.RealPivotPrice.ToString() + ", " +
                                          ohlc.InterpolatedPivotPrice.ToString() + ", " +
                                          ohlc.DaysUpDown.ToString());
                }
                foreach (DateTime outsideDay in _outsideDayList)
                {
                    if (!_allTurnDates.Contains(outsideDay))
                    {
                        _allTurnDates.Add(outsideDay);
                    }
                }
                _allTurnDates.Sort();
                foreach (DateTime date in _allTurnDates)
                {
                    _allTurnsFile.WriteLine(date.Date);
                }
            }
            else
            {
                _outputFile.WriteLine("Not enough data i.e. less than two data points.");
            }
            _outputFile.Flush();
            _outputFile.Close();
            _outputFile.Dispose();
            _allTurnsFile.Flush();
            _allTurnsFile.Close();
            _allTurnsFile.Dispose();
        }
        #endregion
    }
}

