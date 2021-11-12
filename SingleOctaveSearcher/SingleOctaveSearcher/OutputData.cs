using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SingleOctaveSearcher
{
    /// <summary>
    /// A class to keep track of the output data
    /// </summary>
    public class OutputData
    {
        #region Accessor Methods
        /// <summary>
        /// Get the Start Date
        /// </summary>
        public DateTime StartDate
        {
            get;
            set;
        }

        /// <summary>
        /// Get the Octaves
        /// </summary>
        public List<OctaveElement> Octaves
        {
            get;
            set; 
        }
        #endregion

        #region The Constructor
        /// <summary>
        /// The constructor
        /// </summary>
        /// <param name="startDate">The start date</param>
        /// <param name="octaves">The octaves</param>
        public OutputData(DateTime startDate, List<OctaveElement> octaves)
        {
            StartDate = startDate;
            Octaves = octaves;
        }

        public bool EachOctaveHasAHit()
        {
            if (Octaves == null)
            {
                return false;
            }
            else if (Octaves.Count < 8)
            {
                return false;
            }
            else
            {
                bool output = true;
                foreach(OctaveElement octave in Octaves)
                {
                    if (octave.HitDates.Count <= 0)
                    {
                        output = false;
                        break;
                    }
                }
                return output;
            }
        }

        public bool EachOctaveHasAUniqueHit()
        {
            if (Octaves == null)
            {
                return false;
            }
            else if (Octaves.Count < 8)
            {
                return false;
            }
            else
            {
                bool output = true;
                foreach (OctaveElement octave in Octaves)
                {
                    if (octave.HitDates.Count != 1)
                    {
                        output = false;
                        break;
                    }
                }
                return output;
            }
        }

        public double Error()
        {
            double output = 0.0;
            foreach (OctaveElement octave in Octaves)
            {
                output += octave.Error;
            }
            return output;
        }
        #endregion
    }
}

