// Free to use, modify, distribute
// No warranty
using System;
using System.Collections.Generic;
using System.IO;

namespace Aligner
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
                _ohlc = new List<BarData>();
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
                        // Ignore the first line as we assume it contains the headers (i.e. Date, Planet, Contract, Open, High, Low, Close, Volume, OI)
                        string date;
                        string planet;
                        string contract;
                        string open;
                        string high;
                        string low;
                        string close;
                        string volume;
                        string oi;
                        CompareByPlanetPos comp = new CompareByPlanetPos();
                        // Read the input file line by line
                        while ((buffer = myFile.ReadLine()) != null)
                        {
                            // Read the csv delimited data
                            string[] words = buffer.Split(delimiterChar);
                            date = words[0];
                            planet = words[1];
                            contract = words[2];
                            open = words[3];
                            high = words[4];
                            low = words[5];
                            close = words[6];
                            volume = words[7];
                            oi = words[8];
                            try
                            {
                                BarData barData = new BarData(date, planet, contract, open, high, low, close, volume, oi);
                                int position = _ohlc.BinarySearch(barData, comp);
                                if (position < 0)
                                {
                                    position = ~position;
                                }
                                _ohlc.Insert(position, barData);
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
