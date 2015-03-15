using Parkkis.Models.Base;
using System;
using System.Collections.Generic;
using System.Text;
using Windows.Devices.Geolocation;
using Windows.Foundation;

namespace Parkkis.Models.Oulu
{
    public class OuluParkingFacility : BaseParkingFacility
    {

        /// <summary>
        /// Parkign facility status
        /// </summary>
        public override ParkingFacilityStatus Status
        {
            get
            {
                if(FreeSpace == null || TotalSpace == null)
                    return ParkingFacilityStatus.Unknown;

                if (FreeSpace == 0)
                    return ParkingFacilityStatus.Full;

                // Calculate fill rate
                double fillRate = (double)FreeSpace.Value / TotalSpace.Value;

                if (fillRate < 0.2)
                    return ParkingFacilityStatus.AlmostFull;

                else
                    return ParkingFacilityStatus.SpacesAvailable;

            }
        }

    }
}
