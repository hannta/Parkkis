using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Text;
using Windows.Devices.Geolocation;
using Windows.Foundation;

namespace Parkkis.Models
{
    public interface IParkingFacility
    {

        /// <summary>
        /// Facility id
        /// </summary>
        string Id { get;  }


        /// <summary>
        /// Parking facility name
        /// </summary>
        string Name { get; }


        /// <summary>
        /// Parking facility address
        /// </summary>
        string Address { get; }


        /// <summary>
        /// Parking facility location
        /// </summary>
        Geopoint Location { get; }


        /// <summary>
        /// Parking facility status
        /// </summary>
        ParkingFacilityStatus Status { get; }


        /// <summary>
        /// Free space count, if info available
        /// </summary>
        int? FreeSpace { get; }


        /// <summary>
        /// Total space, if info available
        /// </summary>
        int? TotalSpace { get; }


        /// <summary>
        /// Updated time
        /// </summary>
        DateTime Updated { get; }


        /// <summary>
        /// Distance to my location, meters
        /// </summary>
        double? Distance { get; set; }


        /// <summary>
        /// Is facility selected
        /// </summary>
        bool IsSelected { get; set; }


        /// <summary>
        /// Map market NormalizedAnchorPoint, workaround for mapcontrol issue (we need to bind the value)
        /// </summary>
        Point NormalizedAnchorPoint { get; }


    }
}
