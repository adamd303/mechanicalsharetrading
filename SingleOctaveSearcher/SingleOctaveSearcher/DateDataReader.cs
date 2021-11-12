using System;
using System.Collections.Generic;
using System.IO;

namespace SingleOctaveSearcher
{
    public class DateDataReader
    {
        private string _inputFile;
        private List<DateData> _dates = new List<DateData>();

        #region Accessor methods
        public List<DateData> Dates
        {
            get { return _dates; }
        }
        #endregion

        public DateDataReader(string inputFile)
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
                CompareDates comp = new CompareDates();
                try
                {
                    string currentDirectory = System.IO.Directory.GetCurrentDirectory();
                    myFile = File.OpenText(_inputFile);
                    while ((buffer = myFile.ReadLine()) != null)
                    {
                        // Read the csv delimited data
                        string[] words = buffer.Split(delimiterChar);
                        string date = words[0];
                        try
                        {
                            DateData dateData = new DateData(date);
                            int position = _dates.BinarySearch(dateData, comp);
                            if (position < 0)
                            {
                                position = ~position;
                            }
                            _dates.Insert(position, dateData);
                        }
                        catch (Exception ex)
                        {
                            throw ex;
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

