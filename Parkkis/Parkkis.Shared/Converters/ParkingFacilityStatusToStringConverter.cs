using Parkkis.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml.Data;

namespace Parkkis.Converters
{

    /// <summary>
    /// Parking Facility Status to string
    /// </summary>
    public class ParkingFacilityStatusToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value != null)
            {
                ResourceLoader loader = new Windows.ApplicationModel.Resources.ResourceLoader();

                ParkingFacilityStatus type = (ParkingFacilityStatus)value;

                switch (type)
                {
                    case ParkingFacilityStatus.AlmostFull:
                        return loader.GetString("FacilityStatusAlmostFull");

                    case ParkingFacilityStatus.Closed:
                        return loader.GetString("FacilityStatusClosed");

                    case ParkingFacilityStatus.Full:
                        return loader.GetString("FacilityStatusFull");

                    case ParkingFacilityStatus.Open:
                        return loader.GetString("FacilityStatusOpen");

                    case ParkingFacilityStatus.SpacesAvailable:
                        return loader.GetString("FacilityStatusSpacesAvailable");

                    case ParkingFacilityStatus.Unknown:
                        return loader.GetString("FacilityStatusUnknown");

                    default:
                        return String.Empty;
                }
            }

            else
            {
                return String.Empty;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
