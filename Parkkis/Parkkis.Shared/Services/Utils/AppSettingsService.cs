using System;
using System.Collections.Generic;
using System.Text;
using Windows.Storage;

namespace Parkkis.Services.Utils
{
    /// <summary>
    /// Application settings service 
    /// </summary>
    public class AppSettingsService
    {
        private ApplicationDataContainer roamingSettings;

        // The key names of settings
        private const string allowLocationUsageKeyName = "AllowLocationUsage";

        // The default value of our settings
        private const bool allowLocationUsageDefault = false;


        public AppSettingsService()
        {
            this.roamingSettings = ApplicationData.Current.RoamingSettings;
        }


        /// <summary>
        /// Update setting value, if it is not exists add
        /// </summary>
        /// <param name="Key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool AddOrUpdateValue(string Key, Object value)
        {
            bool valueChanged = false;
            
            if (this.roamingSettings.Values.ContainsKey(Key))
            {
                if (this.roamingSettings.Values[Key] != value)
                {
                    // Store the new value
                    this.roamingSettings.Values[Key] = value;
                    valueChanged = true;
                }
            }

            // Otherwise create the key
            else
            {
                this.roamingSettings.Values.Add(Key, value);
                valueChanged = true;
            }

            return valueChanged;
        }


        /// <summary>
        /// Get stting value or default
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public T GetValueOrDefault<T>(string Key, T defaultValue)
        {
            T value;

            if (this.roamingSettings.Values.ContainsKey(Key))
            {
                value = (T)this.roamingSettings.Values[Key];
            }

            else
            {
                value = defaultValue;
            }
            return value;
        }


        /// <summary>
        /// Allow location (gps) usage
        /// </summary>
        public bool AllowLocationUsage
        {
            get
            {
                return GetValueOrDefault<bool>(allowLocationUsageKeyName, allowLocationUsageDefault);
            }

            set
            {
                AddOrUpdateValue(allowLocationUsageKeyName, value);

            }
        }


    }
}
