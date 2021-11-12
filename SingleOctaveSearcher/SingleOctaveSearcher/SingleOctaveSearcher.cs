using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SingleOctaveSearcher
{
    public class OctaveSearcher
    {
        #region Constants
        private const string DEFAULTOUTPUTFILENAME = "Output.csv";
        #endregion
        #region Class private variables
        private List<DateData> _dates;          // The Date data for the turns from the input file
        private double _length;                   // The Octave Length
        private uint _repeats;                  // The Additions to Octave Length
        private double _orb;                      // The Orb
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
        /// <param name="repeats">Additions to Octave Length.</param>
        /// <param name="orb">The Orb.</param>
        /// <param name="dataFile">The Data File name.</param>
        /// </summary>
        public OctaveSearcher(double length, uint repeats, double orb, string dataFile)
        {
            _length = length;
            _repeats = repeats;
            _orb = orb;
            DataFile = dataFile;
            _outputFileName = DEFAULTOUTPUTFILENAME;
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
                    double length = _length;
                    for (uint j = 0; j <= _repeats; j++)
                    {
                        length = _length + (double)j;
                        _dates = dateDataReader.Dates;
                        DateTime startDate = _dates[0].Date.AddDays(-length);    // Initialise the start date
                        // Loop through all the possible Start Dates
                        bool keep_looping = true;
                        uint count = 0;
                        List<OutputData> outputData = new List<OutputData>();
                        DateTime endDate = _dates[_dates.Count - 1].Date.AddDays(_orb);
                        while (keep_looping)
                        {
                            DateTime currentStartDate = startDate.AddDays((double)count);
                            if (currentStartDate <= endDate)
                            {
                                List<OctaveElement> octaves = CreateOneOctave(currentStartDate, length);
                                List<OctaveElement> swap = new List<OctaveElement>();
                                foreach (OctaveElement octave in octaves)
                                {
                                    if (octave.Octave <= endDate)
                                    {
                                        swap.Add(octave);
                                    }
                                }
                                octaves = swap;
                                if ((octaves != null) && (octaves.Count > 0))
                                {
                                    foreach (DateData date in _dates)
                                    {
                                        foreach (OctaveElement octave in octaves)
                                        {
                                            double error = Math.Abs(octave.Octave.Subtract(date.Date).TotalDays);
                                            if (error <= _orb)
                                            {
                                                octave.HitDates.Add(date.Date);
                                                octave.Error += error;
                                            }
                                        }
                                    }
                                    outputData.Add(new OutputData(currentStartDate, octaves));
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
                            if (_repeats == 0)
                            {
                                foreach (OutputData output in outputData)
                                {
                                    if (output.EachOctaveHasAHit())
                                    {
                                        _outputFile.WriteLine("Each Octave has a hit for Start Date " + output.StartDate + ", length = " + length);
                                    }
                                }
                                _outputFile.WriteLine();
                            }
                            double minError = double.MaxValue;
                            int minErrorIndex = 0;
                            int i = 0;
                            DateTime minErrorDate = DateTime.MinValue;
                            foreach (OutputData output in outputData)
                            {
                                if (output.EachOctaveHasAUniqueHit())
                                {
                                    if (_repeats == 0)
                                    {
                                        _outputFile.WriteLine("Each Octave has a unique hit for Start Date " + output.StartDate + ", length = " + length + ", error = " + output.Error() / (double)(output.Octaves.Count));
                                    }
                                    if (output.Error() < minError)
                                    {
                                        minError = output.Error();
                                        minErrorIndex = i;
                                        minErrorDate = output.StartDate;
                                    }
                                }
                                i++;
                            }
                            if (minError != double.MaxValue)
                            {
                                if (_repeats == 0)
                                {
                                    _outputFile.WriteLine();
                                }
                                _outputFile.WriteLine("Hit with minimum error, start date = " + minErrorDate.ToShortDateString() + ", length = " + length + ", mean error = " + minError / ((double)outputData[minErrorIndex].Octaves.Count));
                                if (_repeats == 0)
                                {
                                    foreach (OctaveElement octave in outputData[minErrorIndex].Octaves)
                                    {
                                        _outputFile.WriteLine("Octave Date = " + octave.Octave.ToShortDateString() + ", Hit Date = " + octave.HitDates[0].ToShortDateString() + ", days diff. = " + Math.Abs(octave.Octave.Subtract(octave.HitDates[0]).TotalDays));
                                    }
                                }
                            }
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
        private List<OctaveElement> CreateOneOctave(DateTime startDate, double length)
        {
            OctaveElement DO = new OctaveElement(0, startDate);
            OctaveElement RE = new OctaveElement(0, startDate.AddDays(length * 1.0 / 8.0));
            OctaveElement MI = new OctaveElement(0, startDate.AddDays(length * 1.0 / 4.0));
            OctaveElement FA = new OctaveElement(0, startDate.AddDays(length * 1.0 / 3.0));
            OctaveElement SO = new OctaveElement(0, startDate.AddDays(length * 1.0 / 2.0));
            OctaveElement LA = new OctaveElement(0, startDate.AddDays(length * 2.0 / 3.0));
            OctaveElement TI = new OctaveElement(0, startDate.AddDays(length * 7.0 / 8.0));
            OctaveElement secondDO = new OctaveElement(0, startDate.AddDays(length));
            List<OctaveElement> output = new List<OctaveElement>();
            output.Add(DO);
            output.Add(RE);
            output.Add(MI);
            output.Add(FA);
            output.Add(SO);
            output.Add(LA);
            output.Add(TI);
            output.Add(secondDO);
            return output;
        }
        #endregion
    }
}

