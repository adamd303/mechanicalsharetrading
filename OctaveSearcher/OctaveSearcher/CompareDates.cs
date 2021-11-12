using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctaveSearcher
{
    /// <summary>
    /// Compare IReal points by a real number
    /// </summary>
    public class CompareDates : IComparer<DateData>
    {
        public int Compare(DateData date1, DateData date2)
        {
            if (date1.Date > date2.Date)
            {
                //
                // date1 is greater 
                //
                return 1;
            }
            else if (date1.Date < date2.Date)
            {
                //
                // date2 is greater
                //
                return -1;
            }
            else
            {
                // The dates are equal
                return 0;
            }
        }
    }
}