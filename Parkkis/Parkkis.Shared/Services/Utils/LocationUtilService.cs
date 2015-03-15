using System;
using System.Collections.Generic;
using System.Text;
using Windows.Devices.Geolocation;

namespace Parkkis.Services.Utils
{
    /// <summary>
    /// Location utils
    /// </summary>
    public class LocationUtilService
    {
        /// <summary>
        /// Get distance of locations
        /// </summary>
        /// <param name="pointA"></param>
        /// <param name="pointB"></param>
        /// <returns></returns>
        public double Distance(Geopoint pointA, Geopoint pointB)
        {
            return Distance(pointA.Position.Latitude, pointA.Position.Longitude, pointB.Position.Latitude, pointB.Position.Longitude);
        }


        /// <summary>
        /// Get distance of locations
        /// </summary>
        /// <param name="latitudeA"></param>
        /// <param name="longitudeA"></param>
        /// <param name="latitudeB"></param>
        /// <param name="longitudeB"></param>
        /// <returns></returns>
        public double Distance(double latitudeA, double longitudeA, double latitudeB, double longitudeB)
        {
            double R = 6367000.0; // earth radius in meters
            double dLat = this.toRadian(latitudeB) - this.toRadian(latitudeA);
            double dLon = this.toRadian(longitudeB) - this.toRadian(longitudeA);

            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) + Math.Cos(this.toRadian(latitudeA)) * Math.Cos(this.toRadian(latitudeB)) * Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            double distance = R * c;

            return distance;
        }


        /// <summary>
        /// Degrees to radian
        /// </summary>
        /// <param name="deg"></param>
        /// <returns></returns>
        private double toRadian(double deg)
        {
            return (Math.PI / 180) * deg;
        }
    }
}
