using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml.Data;

namespace Parkkis.Converters
{
    /// <summary>
    /// Meters to distance string
    /// </summary>
    public class MetersToDistanceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
                return String.Empty;

            double meters = (double)value;

            if(meters <= 500)
            {
                return System.Convert.ToInt32(meters) + " m";
            }

            else
            {
                return (meters / 1000).ToString("F") + " km";
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
