// Free to use, modify, distribute
// No warranty
using System;
using System.Collections.Generic;
using System.IO;

namespace TwoDaySwing
{
    public class BarDataReader
    {
        private string _inputFile;
        private List<BarData> _ohlc = new List<BarData>();

        #region Accessor methods
        public List<BarData> OHLC
        {
            get { return _ohlc; }
        }
        #endregion

        public BarDataReader(string inputFile)
        {
            if (inputFile != null)
            {
                _inputFile = inputFile;
                // Only support csv files with no 
                // header at the moment
                char delimiterChar = ',';
                string buffer;
                // Open the data file
                StreamReader myFile;
                string prevContract = String.Empty;
                try
                {
                    string currentDirectory = System.IO.Directory.GetCurrentDirectory();
                    myFile = File.OpenText(_inputFile);
                    // Read the first line
                    if ((buffer = myFile.ReadLine()) != null)
                    {
                        // Ignore the first line as we assume it contains the headers (i.e. Date, Open, High, Low, Close, Volume)
                        string date;
                        string open;
                        string high;
                        string low;
                        string close;
                        string volume;
                        // Read the input file line by line
                        while ((buffer = myFile.ReadLine()) != null)
                        {
                            // Read the csv delimited data
                            string[] words = buffer.Split(delimiterChar);
                            date = words[0];
                            open = words[1];
                            high = words[2];
                            low = words[3];
                            close = words[4];
                            volume = words[5];
                            try
                            {
                                BarData barData = new BarData(date, open, high, low, close, volume);
                                _ohlc.Add(barData);
                            }
                            catch (Exception ex)
                            {
                                throw ex;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error reading Input Data File, " + _inputFile.ToString() + ", error: " + ex.ToString());
                }
                myFile.Close();
            }
            else
            {
                throw new ArgumentException("Invalid input data file.");
            }
        }
    }
}

