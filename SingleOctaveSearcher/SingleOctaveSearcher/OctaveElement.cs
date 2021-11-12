using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SingleOctaveSearcher
{
    public class OctaveElement
    {
        public List<DateTime> HitDates { get; set; }
        public DateTime Octave { get; set; }
        public double Error { get; set; }

        public OctaveElement (uint noHits, DateTime octave)
        {
            HitDates = new List<DateTime>();
            Octave = octave;
            Error = 0.0;
        }
    }
}
