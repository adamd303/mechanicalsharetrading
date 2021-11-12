using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctaveSearcher
{
    /// <summary>
    /// A class to keep track of the output data
    /// </summary>
    public class OutputData
    {
        private DateTime _startDate;     // The start date for the search
        private uint _noHits;            // The number of hits for the start date/length combination


        #region Accessor Methods
        /// <summary>
        /// Get the Start Date
        /// </summary>
        public DateTime StartDate
        {
            get { return _startDate; }
        }

        /// <summary>
        /// Get the Start Date
        /// </summary>
        public uint NoHits
        {
            get { return _noHits; }
        }
        #endregion

        #region The Constructor
        /// <summary>
        /// The constructor
        /// </summary>
        /// <param name="noHits">The number of hits</param>
        /// <param name="startDate">The start date</param>
        public OutputData(DateTime startDate, uint noHits)
        {
            _startDate = startDate;
            _noHits = noHits;
        }
        #endregion
    }
}
