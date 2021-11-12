// Free to use, modify, distribute
// No warranty
using System;
using System.Collections.Generic;

namespace Aligner
{
    /// <summary>
    /// A class to keep track of the data for an Open, High, Low, Close bar
    /// </summary>
    public class BarData
    {
        private DateTime _date;      // The date
        private decimal? _planet;    // The planet position in degrees
        private string   _contract;  // The contract
        private decimal? _open;      // The opening price
        private decimal? _high;      // The high price
        private decimal? _low;       // The low price
        private decimal? _close;     // The closing price
        private decimal? _volume;    // The volume
        private decimal? _oi;        // The open interest
                        
        #region Accessor Methods
        /// <summary>
        /// Get the Date
        /// </summary>
        public DateTime Date
        {
            get { return _date; }
        }

        /// <summary>
        /// Get the Planet Position
        /// </summary>
        public decimal? Planet
        {
            get { return _planet; }
        }

        /// <summary>
        /// Get the Contract
        /// </summary>
        public string Contract
        {
            get { return _contract; }
        }

        /// <summary>
        /// Get the Open Price
        /// </summary>
        public decimal? Open
        {
            get { return _open; }
        }

        /// <summary>
        /// Get the High Price
        /// </summary>
        public decimal? High
        {
            get { return _high; }
        }

        /// <summary>
        /// Get the Low Price
        /// </summary>
        public decimal? Low
        {
            get { return _low;}
        }

        /// <summary>
        /// Get the Close High Price
        /// </summary>
        public decimal? Close
        {
            get { return _close;}
        }

        /// <summary>
        /// Get the Volume
        /// </summary>
        public decimal? Volume
        {
            get { return _volume; }
        }

        /// <summary>
        /// Get the Open Interest
        /// </summary>
        public decimal? OI
        {
            get { return _oi; }
        }

        /// <summary>
        /// Get/Set whether we have a match here
        /// </summary>
        public decimal? Match
        {
            get; 
            set; 
        }
        #endregion

        #region The Constructor
        /// <summary>
        /// A constructor that accepts volume and all inputs are strings
        /// </summary>
        /// <param name="stringDate">The date</param>
        /// <param name="stringPlanet">The planet position</param>
        /// <param name="stringContract">The contract</param>
        /// <param name="stringOpen">The opening price</param>
        /// <param name="stringHigh">The high price</param>
        /// <param name="stringLow">The low price</param>
        /// <param name="stringClose">The closing price</param>
        /// <param name="stringVolume">The volume</param>
        /// <param name="stringOI">The open interest</param>
        public BarData(string stringDate, string stringPlanet, string stringContract, string stringOpen, string stringHigh, string stringLow, string stringClose, string stringVolume, string stringOI)
        {
            bool validDate = false;
            DateTime date;
            if (DateTime.TryParse(stringDate, out date))
            {
                validDate = true;
            }
            else if (stringDate.Length >= 8)
            {
                // Dates in the format YYYYMMDD
                char[] dateArray = stringDate.ToCharArray();
                string yearString = dateArray[0].ToString() + dateArray[1].ToString() + dateArray[2].ToString() + dateArray[3].ToString();
                string monthString = dateArray[4].ToString() + dateArray[5].ToString();
                string dayString = dateArray[6].ToString() + dateArray[7].ToString();
                int year;
                int month;
                int day;
                if ((int.TryParse(yearString, out year)) && (int.TryParse(monthString, out month)) && (int.TryParse(dayString, out day)))
                {
                    try
                    {
                        date = new DateTime(year, month, day);
                        validDate = true;
                    }
                    catch
                    {
                        validDate = false;
                    }
                }
            }
            if (validDate)
            {
                decimal? planet=null;
                string contract = string.Empty;
                decimal? open=null;
                decimal? high=null;
                decimal? low=null;
                decimal? close=null;
                decimal? volume=null;
                decimal? oi=null;
                decimal dummy;
                if (decimal.TryParse(stringPlanet, out dummy))
                {
                    planet = dummy;
                }
                else
                {
                    planet = null;
                }
                if (decimal.TryParse(stringOpen, out dummy))
                {
                    open = dummy;
                }
                else
                {
                    open = null;
                }
                if (decimal.TryParse(stringHigh, out dummy))
                {
                    high = dummy;
                }
                else
                {
                    high = null;
                }
                if (decimal.TryParse(stringLow, out dummy))
                {
                    low = dummy;
                }
                else
                {
                    low = null;
                }
                if (decimal.TryParse(stringClose, out dummy))
                {
                    close = dummy;
                }
                else
                {
                    close = null;
                }
                if (decimal.TryParse(stringVolume, out dummy))
                {
                    volume = dummy;
                }
                else
                {
                    volume = null;
                }
                if (decimal.TryParse(stringOI, out dummy))
                {
                    oi = dummy;
                }
                else
                {
                    oi = null;
                }
                _planet = planet;
                _contract = contract;
                _date = date;
                _open = open;
                _high = high;
                _low = low;
                _close = close;
                _volume = volume;
                _oi = oi;
                Match = null;
            }
            else
            {
                throw new ArgumentException("Invalid input date.");
            }
        }
        #endregion

        public override string ToString()
        {
            string date = _date.ToShortDateString();
            string planet;
            if (_planet.HasValue)
            {
                planet = _planet.ToString();
            }
            else
            {
                planet = string.Empty;
            }
            string contract = _contract;
            string open;
            if (_open.HasValue)
            {
                open = _open.ToString();
            }
            else
            {
                open = string.Empty;
            }
            string high;
            if (_high.HasValue)
            {
                high = _high.ToString();
            }
            else
            {
                high = string.Empty;
            }
            string low;
            if (_low.HasValue)
            {
                low = _low.ToString();
            }
            else
            {
                low = string.Empty;
            }
            string close;
            if (_close.HasValue)
            {
                close = _close.ToString();
            }
            else
            {
                close = string.Empty;
            }
            string volume;
            if (_volume.HasValue)
            {
                volume = _volume.ToString();
            }
            else
            {
                volume = string.Empty;
            }
            string oi;
            if (_oi.HasValue)
            {
                oi = _oi.ToString();
            }
            else
            {
                oi = string.Empty;
            }
            string match;
            if (Match.HasValue)
            {
                match = Match.ToString();
            }
            else
            {
                match = string.Empty;
            }
            return date + "," + planet + "," + contract + "," + open + "," + high + "," + low + "," + close + "," + volume + "," + oi + "," + match;
        }
    }
}