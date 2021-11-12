// Copyright (c) 2012, Adam Duval Kangaroo Point South 4169, Australia.
// All Rights Reserved.
using System;
using System.Collections.Generic;
using System.IO;

namespace Adam.Trading.Mechanical.Model
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
                    string date;
                    string open;
                    string high;
                    string low;
                    string close;
                    string volume;
                    string contract = String.Empty;
                    bool contractRollover = false;
                    int count = 0;
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
                        if (words.Length >= 7)
                        {
                            contract = words[6];
                            if (count > 0)
                            {
                                if (contract != prevContract)
                                {
                                    contractRollover = true;
                                }
                                else
                                {
                                    contractRollover = false;
                                }
                            }
                        }
                        try
                        {
                            BarData barData = new BarData(date, open, high, low, close, volume, contractRollover);
                            _ohlc.Add(barData);
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                        prevContract = contract;
                        count++;
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
