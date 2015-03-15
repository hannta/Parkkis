using System;
using System.Collections.Generic;
using System.Text;

namespace Parkkis.Models
{
    public enum ParkingFacilityStatus
    {
        SpacesAvailable,
        AlmostFull,
        Full,
        Closed,
        Open, // We do not actually know the free space status
        Unknown,
    }
}
