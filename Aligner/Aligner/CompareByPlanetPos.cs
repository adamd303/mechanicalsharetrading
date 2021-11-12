using System.Collections.Generic;

namespace Aligner
{
    /// <summary>
    /// Compare BarData points by planet position
    /// </summary>
    public class CompareByPlanetPos : IComparer<BarData>
    {
        public int Compare( BarData point1, BarData point2 )
        {
            if( point1 == null )
            {
                if( point2 == null )
                {
                    //
                    // If point1 is null and point2 is null, they're equal. 
                    //
                    return 0;
                }
                else
                {
                    //
                    // If point1 is null and point2 is not null, point2 is greater. 
                    //
                    return -1;
                }
            }
            else
            {
                //
                // If point1 is not null...
                //
                if( point2 == null )
                {
                    //
                    // ...and point2 is null, point1 is greater.
                    //
                    return 1;
                }
                else
                {
                    //
                    // ...and point2 is not null, compare the real values of the two points.
                    //
                    if( point1.Planet > point2.Planet )
                    {
                        //
                        // point1 is greater 
                        //
                        return 1;
                    }
                    else if( point1.Planet < point2.Planet )
                    {
                        //
                        // point2 is greater
                        //
                        return -1;
                    }
                    else
                    {
                        //
                        // The points are equal
                        //
                        return 0;
                    }
                }
            }
        }
    }
}

