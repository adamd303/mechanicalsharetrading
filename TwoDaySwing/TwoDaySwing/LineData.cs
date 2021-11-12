// Free to use, modify, distribute
// No warranty
using System;
using System.Collections.Generic;

namespace TwoDaySwing
{
    /// <summary>
    /// A class to keep track of the data for a Line Series
    /// </summary>
    public class LineData
    {
        private DateTime _date;        // The date or x-coordinate
        private decimal _price;        // The price or y-coordinate
        private int _index = -1;       // The index of the point in the original list (optional)
                                       // Set it to -1 to indicate it hasn't been set yet
        private Enumerations.TopBottom _topBottom = Enumerations.TopBottom.Unknown;
        private bool _lastLineSegmentActive = false;  // The last line segment is active

        #region Accessor Methods
        /// <summary>
        /// Get the Date
        /// </summary>
        public DateTime Date
        {
            get { return _date; }
        }

        /// <summary>
        /// Get the Price
        /// </summary>
        public decimal Price
        {
            get { return _price; }
        }

        /// <summary>
        /// Get the Index
        /// </summary>
        public int Index
        {
            get { return _index; }
        }

        /// <summary>
        /// Get the Top/Bottom Value
        /// </summary>
        public Enumerations.TopBottom TopBottom
        {
            get { return _topBottom; }
        }

        /// <summary>
        /// Get/Set the _lastLineSegmentActive Value
        /// Strictly this should probably be in the ViewModel but leave it here for now.
        /// </summary>
        public bool LastLineSegmentActive
        {
            get { return _lastLineSegmentActive; }
            set { _lastLineSegmentActive = value; }
        }
        #endregion

        #region The Constructor
        /// <summary>
        /// The constructor which just takes a date and price
        /// </summary>
        /// <param name="date">The date</param>
        /// <param name="open">The price</param>
        public LineData(DateTime date, decimal price)
        {
            _date = date;
            _price = price;
        }

        /// <summary>
        /// The constructor which takes a date, price and index
        /// </summary>
        /// <param name="date"></param>
        /// <param name="price"></param>
        public LineData(DateTime date, decimal price, int index)
        {
            _date = date;
            _price = price;
            _index = index;
        }

        /// <summary>
        /// The constructor which takes a date, price, index and top/bottom
        /// </summary>
        /// <param name="date"></param>
        /// <param name="price"></param>
        public LineData(DateTime date, decimal price, int index, Enumerations.TopBottom topBottom)
        {
            _date = date;
            _price = price;
            _index = index;
            _topBottom = topBottom;
        }
        #endregion
    }
}