// Free to use, modify, distribute
// No warranty
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Aligner
{
    public class Aligner
    {
        #region Constants
        private const string DEFAULTOUTPUTFILENAME = "Output.csv";
        #endregion
        #region Class private variables
        private string _outputFileName;               // The output file name
        private decimal _startPoint;                  // The startpoint
        private decimal _increment;                   // The increment
        private StreamWriter _outputFile;             // The output file streamwriter
        #endregion

        #region Accessor Methods
        /// <summary>
        /// The Data File name
        /// </summary>
        public string DataFile { get; private set; }
        #endregion

        #region The Contructor
        /// <summary>
        /// The constructor
        /// </summary>
        public Aligner(string dataFile, decimal startPoint, decimal increment, string outputFileName)
        {
            DataFile = dataFile;
            _outputFileName = outputFileName;
            _startPoint = startPoint;
            _increment = increment;
        }
        #endregion

        #region The Main Calculation Routine
        public void CalculateData()
        {
            // Open the output file
            if ((_outputFileName != null) && (_outputFileName != string.Empty))
            {
                _outputFile = File.CreateText(_outputFileName);
            }
            else
            {
                _outputFile = File.CreateText(DEFAULTOUTPUTFILENAME);
            }
            try
            {
                BarDataReader barDataReader = new BarDataReader(DataFile);
                if (barDataReader.OHLC.Count >= 1)
                {
                    System.Console.WriteLine("Before for loop, _startPoint = " + _startPoint.ToString());
                    System.Console.WriteLine("Before for loop, barDataReader.OHLC[barDataReader.OHLC.Count - 1].Planet = " + barDataReader.OHLC[barDataReader.OHLC.Count - 1].Planet.ToString());
                    CompareByPlanetPos comp = new CompareByPlanetPos();
                    for (decimal pt = _startPoint; pt <= barDataReader.OHLC[barDataReader.OHLC.Count - 1].Planet; pt += _increment)
                    {
                        System.Console.WriteLine("For loop, pt = " + pt.ToString());
                        BarData searchVal = new BarData(DateTime.Now.ToShortDateString(), pt.ToString(), string.Empty, "0.0", "0.0", "0.0", "0.0", "0.0", "0.0");
                        int position = barDataReader.OHLC.BinarySearch(searchVal, comp);
                        if (position < 0)
                        {
                            position = ~position;
                        }
                        System.Console.WriteLine("pt = " + pt.ToString() + ", position = " + position);
                        if ((position == 0) || (position == barDataReader.OHLC.Count - 1))
                        {
                            decimal planet = decimal.Parse(barDataReader.OHLC[position].Planet.ToString());
                            if (Math.Abs(planet - pt) < _increment)
                            {
                                barDataReader.OHLC[position].Match = pt;
                            }
                            else
                            {
                                barDataReader.OHLC[position].Match = 0.0M;
                            }
                        }
                        else
                        {
                            // Find the closest point either side
                            double point = double.Parse(pt.ToString());
                            double prev = double.Parse(barDataReader.OHLC[position - 1].Planet.ToString());
                            double next = double.Parse(barDataReader.OHLC[position].Planet.ToString());
                            double distPrev = Math.Sqrt((prev - point) * (prev - point));
                            double distNext = Math.Sqrt((next - point) * (next - point));
                            if (distPrev < distNext)
                            {
                                // Previous point is match
                                barDataReader.OHLC[position - 1].Match = pt;
                                barDataReader.OHLC[position].Match = 0.0M;
                            }
                            else
                            {
                                // Next point is match
                                barDataReader.OHLC[position].Match = pt;
                                barDataReader.OHLC[position-1].Match = 0.0M;
                            }
                        }
                    }
                    foreach (BarData barData in barDataReader.OHLC)
                    {
                        // Only output the points we want
                        if (barData.Match > 0.0M)
                        {
                            _outputFile.WriteLine(barData.ToString());
                        }
                    }
                }
                else
                {
                    System.Console.WriteLine("Not enough input data!");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            _outputFile.Flush();
            _outputFile.Close();
            _outputFile.Dispose();
        }
        #endregion
    }
}
