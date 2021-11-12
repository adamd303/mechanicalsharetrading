// Free to use, modify, distribute
// No warranty
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace OctaveSearcher
{
    public class OctaveSearcher
    {
        #region Constants
        private const string DEFAULTOUTPUTFILENAME = "Output.csv";
        private const uint ORB = 8;             // The orb in days
        private const bool DEBUG = true;
        #endregion
        #region Class private variables
        private List<DateData> _dates;          // The Date data for the turns from the input file
        private uint _length;                   // The Octave Length
        private string _outputFileName;         // The output file name
        private StreamWriter _outputFile;       // The output file streamwriter
        #endregion

        #region Accessor Methods
        /// <summary>
        /// The Data File name
        /// </summary>
        public string DataFile { get; private set; }

        /// <summary>
        /// The list of Date data from the input file
        /// </summary>
        public List<DateData> Dates
        {
            get { return _dates; }
        }
        #endregion

        #region The Contructor
        /// <summary>
        /// The constructor
        /// </summary>
        /// <param name="length">The Octave Length.</param>
        /// <param name="dataFile">The Data File name.</param>
        /// <param name="outputFileName">The Output File name.</param>
        public OctaveSearcher(uint length, string dataFile, string outputFileName)
        {
            _length = length;
            DataFile = dataFile;
            _outputFileName = outputFileName;
        }
        #endregion

        #region The Main Calculation Routine
        public void CalculateData()
        {
            _dates = new List<DateData>();
            // Open the output file
            if ((_outputFileName != null) && (_outputFileName != String.Empty))
            {
                _outputFile = File.CreateText(_outputFileName);
            }
            else
            {
                _outputFile = File.CreateText(DEFAULTOUTPUTFILENAME);
            }
            try
            {
                DateDataReader dateDataReader = new DateDataReader(DataFile);
                if (dateDataReader.Dates.Count >= 1)
                {
                    _dates = dateDataReader.Dates;
                    DateTime startDate = _dates[0].Date.AddDays(-_length);    // Initialise the start date
                    // Loop through all the possible Start Dates
                    bool keep_looping = true;
                    uint count = 0;
                    List<OutputData> outputData = new List<OutputData>();
                    DateTime endDate = _dates[_dates.Count - 1].Date.AddDays((double)ORB);
                    while (keep_looping)
                    {
                        DateTime currentStartDate = startDate.AddDays((double)count);
                        if (currentStartDate <= endDate)
                        {
                            List<DateTime> octaves = CreateAllOctaves(currentStartDate, endDate, _length);
                            if ((octaves != null) && (octaves.Count > 0))
                            {
                                uint noOfHits = 0;    // The number of hits for the input file with a given start date and octave length
                                double errorSum = 0.0;  // The sum of the differences between actual turns and octaves
                                foreach (DateData date in _dates)
                                {
                                    errorSum = 0.0;  // The sum of the differences between actual turns and octaves
                                    foreach (DateTime octave in octaves)
                                    {
                                        double error = Math.Abs(octave.Subtract(date.Date).TotalDays);
                                        if (error <= (double)ORB)
                                        {
                                            errorSum += error;
                                            noOfHits++;
                                        }
                                    }
                                }
                                double errorPerHit = -1.0;
                                if (noOfHits > 0)
                                {
                                    errorPerHit = errorSum / (double)noOfHits;
                                }
                                _outputFile.WriteLine("Start Date = " + currentStartDate + ", length = " + _length + ", No. of Hits = " + noOfHits + ", Error/Hit = " + errorPerHit + ", No. of Octave notes = " + octaves.Count);
                                outputData.Add(new OutputData(currentStartDate, noOfHits));
                            }
                            else
                            {
                                keep_looping = false;
                            }
                        }
                        else
                        {
                            keep_looping = false;
                        }
                        count++;
                    }
                    if (outputData.Count > 0)
                    {
                        DateTime searchStartDate = outputData[0].StartDate;
                        uint maxHits = outputData[0].NoHits;
                        foreach (OutputData output in outputData)
                        {
                            if (output.NoHits > maxHits)
                            {
                                maxHits = output.NoHits;
                                searchStartDate = output.StartDate;
                            }
                        }
                        _outputFile.WriteLine("Max Hits = " + maxHits + ", on Start Date " + searchStartDate + ", length = " + _length);
                        _outputFile.WriteLine();
                        _outputFile.WriteLine("Detail for start date with most hits:");
                        // Do a recalculation to get the detail for the data with the most no of hits
                        List<DateTime> octaves = CreateAllOctaves(searchStartDate, _dates[_dates.Count-1].Date.AddDays(ORB), _length);
                        if ((octaves != null) && (octaves.Count > 0))
                        {
                            foreach (DateData date in _dates)
                            {
                                DateTime? octaveMatch = null;
                                foreach (DateTime octave in octaves)
                                {
                                    if (Math.Abs(octave.Subtract(date.Date).TotalDays) <= ORB)
                                    {
                                        octaveMatch = octave;       
                                    }  
                                }
                                if (octaveMatch.HasValue)
                                {
                                    _outputFile.WriteLine("date: " + date.Date.ToString() + ", octave date = " + octaveMatch.ToString());
                                }
                                else
                                {
                                    _outputFile.WriteLine("date: " + date.Date.ToString() + " has no matching octave");
                                }
                            }
                            if (DEBUG)
                            {
                                _outputFile.WriteLine();
                                _outputFile.WriteLine("Date listing:");
                                foreach (DateData date in _dates)
                                {
                                    _outputFile.WriteLine("date: " + date.Date.ToString());
                                }
                                _outputFile.WriteLine();
                                _outputFile.WriteLine("Octave listing:");
                                foreach (DateTime octave in octaves)
                                {
                                    _outputFile.WriteLine("octave date: " + octave.ToString());
                                }
                            }
                        }
                        else
                        {
                            // Some strange error has occurred
                            _outputFile.WriteLine("Error finding detail for start date with most hits.");
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            _outputFile.Flush();
            _outputFile.Close();
            _outputFile.Dispose();
        }
        #endregion

        #region Helper Methods
        private List<DateTime> CreateOneOctave(DateTime startDate, double length)
        {
            DateTime DO = startDate;
            DateTime RE = startDate.AddDays(length * 1.0 / 8.0);
            DateTime MI = startDate.AddDays(length * 1.0 / 4.0);
            DateTime FA = startDate.AddDays(length * 1.0 / 3.0);
            DateTime SO = startDate.AddDays(length * 1.0 / 2.0);
            DateTime LA = startDate.AddDays(length * 2.0 / 3.0);
            DateTime TI = startDate.AddDays(length * 7.0 / 8.0);
            List<DateTime> output = new List<DateTime>();
            output.Add(DO);
            output.Add(RE);
            output.Add(MI);
            output.Add(FA);
            output.Add(SO);
            output.Add(LA);
            output.Add(TI);
            return output;
        }

        // startDate must be < endDate
        private List<DateTime> CreateAllOctaves(DateTime startDate, DateTime endDate, uint initialLength)
        {
            List<DateTime> output = new List<DateTime>();
            double totalDays = endDate.Subtract(startDate).TotalDays;
            bool keep_looping = true;
            double length = (double)initialLength;
            uint count = 0;
            DateTime currentStartDate = startDate;
            while (keep_looping)
            {
                List<DateTime> currentOctave = CreateOneOctave(currentStartDate, length);
                foreach(DateTime octaveDate in currentOctave)
                {
                    if (octaveDate > endDate)
                    {
                        keep_looping = false;
                        break;
                    }
                    else
                    {
                        output.Add(octaveDate);
                    }
                }
                count++;
                currentStartDate = currentStartDate.AddDays(length);
                length *= 2.0;
            }
            return output;
        }
        #endregion
    }
}
