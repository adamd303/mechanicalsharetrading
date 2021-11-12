// Copyright (c) 2011, Adam Duval Kangaroo Point South 4169, Australia.
// All Rights Reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam.Trading.Mechanical.Model
{
    /// <summary>
    /// A class to keep track of the current equity
    /// </summary>
    public class Equity
    {
        #region Accessor Methods
        /// <summary>
        /// Get and Set the Date
        /// </summary>
        public DateTime Date
        {
            get;
            set;
        }
        /// <summary>
        /// Get and Set the Current Equity
        /// </summary>
        public decimal CurrentEquity
        {
            get;
            set;
        }
        #endregion

        #region The Constructors
        /// <summary>
        /// The standard simple constructor for when we already have a DateTime to input
        /// </summary>
        /// <param name="date">The date as a DateTime.</param>
        /// <param name="equity">The equity.</param>
        public Equity(DateTime date, decimal equity)
        {
            Date = date;
            CurrentEquity = equity;
        }
        /// <summary>
        /// The more complicated constructor where the date and time is inpuyt as a 
        /// string in the format yyyymmdd
        /// </summary>
        /// <param name="date">The date as a string with various formats.</param>
        /// <param name="equity">The equity.</param> 
        public Equity(string date, decimal equity)
        {
            if (date.Contains("-"))
            {
                // Dates in the format dd-Mmm-YY
                string[] dateParts = date.Split('-');
                int day = System.Convert.ToInt32(dateParts[0]);
                int year = System.Convert.ToInt32(dateParts[2]) + 2000;
                string monthString = dateParts[1];
                int month = 0;
                if (monthString == "Jan")
                {
                    month = 1;
                }
                else if (monthString == "Feb")
                {
                    month = 2;
                }
                else if (monthString == "Mar")
                {
                    month = 3;
                }
                else if (monthString == "Apr")
                {
                    month = 4;
                }
                else if (monthString == "May")
                {
                    month = 5;
                }
                else if (monthString == "Jun")
                {
                    month = 6;
                }
                else if (monthString == "Jul")
                {
                    month = 7;
                }
                else if (monthString == "Aug")
                {
                    month = 8;
                }
                else if (monthString == "Sep")
                {
                    month = 9;
                }
                else if (monthString == "Oct")
                {
                    month = 10;
                }
                else if (monthString == "Nov")
                {
                    month = 11;
                }
                else if (monthString == "Dec")
                {
                    month = 12;
                }
                Date = new DateTime(year, month, day);
                CurrentEquity = equity;
            }
            else if (date.Length >= 8)
            {
                // Dates in the format YYYYMMDD
                char[] dateArray = date.ToCharArray();
                string yearString = dateArray[0].ToString() + dateArray[1].ToString() + dateArray[2].ToString() + dateArray[3].ToString();
                int year = System.Convert.ToInt32(yearString);
                string monthString = dateArray[4].ToString() + dateArray[5].ToString();
                int month = System.Convert.ToInt32(monthString);
                string dayString = dateArray[6].ToString() + dateArray[7].ToString();
                int day = System.Convert.ToInt32(dayString);
                Date = new DateTime(year, month, day);
                CurrentEquity = equity;
            }
            else
            {
                throw new ArgumentException("Unsupported input data format.");
            }
        }
        #endregion
    }
}
