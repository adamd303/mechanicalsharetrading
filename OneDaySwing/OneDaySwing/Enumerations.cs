// Free to use, modify, distribute
// No warranty
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OneDaySwing
{
    /// <summary>
    /// A class containing enumerations relevant to trading
    /// </summary>
    public class Enumerations
    {
        /// <summary>
        /// The Trend Enumeration
        /// </summary>
        public enum Trend
        {
            Up,
            Down,
            Unknown
        }

        /// <summary>
        /// An enumeration which stores the type of Top/Bottom
        /// </summary>
        public enum TopBottom
        {
            Top,
            Bottom,
            Unknown,
        }
    }
}
