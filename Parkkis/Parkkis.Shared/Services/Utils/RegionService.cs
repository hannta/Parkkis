using Caliburn.Micro;
using Parkkis.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Windows.Devices.Geolocation;
using Windows.Storage;

namespace Parkkis.Services.Utils
{
    public class RegionService
    {
        private readonly ObservableCollection<Region> regions;
        private readonly ApplicationDataContainer roamingSettings;
        private readonly string defaultRegionKey;
        private readonly string regionStorageKey;


        public RegionService()
        {

            this.regions = new ObservableCollection<Region>();
            this.roamingSettings = ApplicationData.Current.RoamingSettings;
            this.regionStorageKey = "currentRegion";
            this.defaultRegionKey = "tampere";

            // Tampere
            this.regions.Add(new Region
            {
                Key = "tampere",
                DisplayName = "Tampere",
                MapCenterDefault = new Geopoint(new BasicGeoposition() { Latitude = 61.497593, Longitude = 23.759222 }),
                MapZoomLevelDefault = 13.5
            });


            // Oulu
            this.regions.Add(new Region
            {
                Key = "oulu",
                DisplayName = "Oulu",
                MapCenterDefault = new Geopoint(new BasicGeoposition() { Latitude = 65.009370697662234, Longitude = 25.472881179302931 }),
                MapZoomLevelDefault = 13.5
            });
        }


        /// <summary>
        /// Get collection of available regions
        /// </summary>
        public ObservableCollection<Region> Regions
        {
            get
            {
                return regions;
            }
        }


        /// <summary>
        /// Set & Get current region
        /// </summary>
        public Region CurrentRegion
        {
            set
            {
                if (this.roamingSettings.Values.ContainsKey(this.regionStorageKey))
                {
                    // If the region has changed
                    if (this.roamingSettings.Values[this.regionStorageKey] != value.Key)
                    {
                        this.roamingSettings.Values[this.regionStorageKey] = value.Key;
                    }
                }

                else
                {
                    this.roamingSettings.Values.Add(this.regionStorageKey, value.Key);
                }
            }

            get
            {
                // If the key exists, retrieve the region key and region.
                if (this.roamingSettings.Values.ContainsKey(this.regionStorageKey))
                {
                    string regionKey = (String)this.roamingSettings.Values[this.regionStorageKey];
                    Region selectedRegion = this.Regions.FirstOrDefault(r => r.Key == regionKey);

                    if (selectedRegion != null)
                    {
                        return selectedRegion;
                    }

                    // Get default if not found
                    else
                    {
                        return this.Regions.FirstOrDefault(r => r.Key == this.defaultRegionKey);
                    }

                }

                // Otherwise, use the default value.
                else
                {
                    return this.Regions.FirstOrDefault(r => r.Key == this.defaultRegionKey);
                }
            }
        }


        /// <summary>
        /// Get ParkingFacilityService
        /// </summary>
        /// <param name="regKey"></param>
        /// <returns></returns>
        public IParkingFacilityService GetParkingFacilityService(string regKey = null)
        {
            string regionKey = regKey;

            if (String.IsNullOrWhiteSpace(regionKey))
            {
                regionKey = this.CurrentRegion.Key;
            }

            return IoC.Get<IParkingFacilityService>(regionKey);
        }

    }
}
