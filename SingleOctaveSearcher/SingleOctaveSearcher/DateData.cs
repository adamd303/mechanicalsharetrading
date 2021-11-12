using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SingleOctaveSearcher
{
    /// <summary>
    /// A class to keep track of date data
    /// </summary>
    public class DateData
    {
        private DateTime _date;     // The date
        
        #region Accessor Methods
        /// <summary>
        /// Get the Date
        /// </summary>
        public DateTime Date
        {
            get { return _date; }
        }
        #endregion

        #region The Constructor
        /// <summary>
        /// A constructor accepts the date as a string
        /// </summary>
        /// <param name="date">The date</param>
        public DateData(string stringDate)
        {
            DateTime date;
            if (DateTime.TryParse(stringDate, out date))
            {
                _date = date.AddDays(0.5);  // Make date at midday
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
                    date = new DateTime(year, month, day);
                    _date = date.AddDays(0.5);  // Make date at midday
                }
            }
            else
            {
                throw new ArgumentException("Unsupported input data format.");
            }
        }
        #endregion
    }
}
