using System;
using System.Collections.Generic;
using System.Text;
using Windows.Devices.Geolocation;

namespace Parkkis.Models
{

    /// <summary>
    /// Region model
    /// </summary>
    public class Region
    {

        /// <summary>
        /// Unique region key
        /// </summary>
        public string Key
        {
            get;
            set;
        }


        /// <summary>
        /// Region display name, shown to user
        /// </summary>
        public string DisplayName
        {
            get;
            set;
        }


        /// <summary>
        /// Region default map center location
        /// </summary>
        public Geopoint MapCenterDefault
        {
            get;
            set;
        }


        /// <summary>
        /// Region default map zoom level
        /// </summary>
        public double MapZoomLevelDefault
        {
            get;
            set;
        }

    }
}
