using Parkkis.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Windows.ApplicationModel.Resources;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace Parkkis.Converters
{

    /// <summary>
    /// Parking Facility Status to Brush
    /// </summary>
    public class ParkingFacilityStatusToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value != null)
            {
                ParkingFacilityStatus type = (ParkingFacilityStatus)value;
                double opacity = 1;

                string stringParam = parameter as string;
                if (!String.IsNullOrEmpty(stringParam))
                {
                    double doubleParam = 0;
                    if (Double.TryParse(stringParam, out doubleParam))
                    {
                        opacity = doubleParam;
                    }
                }

                switch (type)
                {
                    case ParkingFacilityStatus.Full:
                        {
                            Color color = ((Color)App.Current.Resources["FacilityStatusFullColor"]);
                            SolidColorBrush brush = new SolidColorBrush(color);
                            brush.Opacity = opacity;
                            return brush;
                        }

                    case ParkingFacilityStatus.AlmostFull:
                        {
                            Color color = ((Color)App.Current.Resources["FacilityStatusAlmostFullColor"]);
                            SolidColorBrush brush = new SolidColorBrush(color);
                            brush.Opacity = opacity;
                            return brush;
                        }

                    case ParkingFacilityStatus.SpacesAvailable:
                        {
                            Color color = ((Color)App.Current.Resources["FacilityStatusSpacesAvailableColor"]);
                            SolidColorBrush brush = new SolidColorBrush(color);
                            brush.Opacity = opacity;
                            return brush;
                        }


                    case ParkingFacilityStatus.Closed:
                        {
                            Color color = ((Color)App.Current.Resources["FacilityStatusClosedColor"]);
                            SolidColorBrush brush = new SolidColorBrush(color);
                            brush.Opacity = opacity;
                            return brush;
                        }


                    case ParkingFacilityStatus.Open:
                        {
                            Color color = ((Color)App.Current.Resources["FacilityStatusOpenColor"]);
                            SolidColorBrush brush = new SolidColorBrush(color);
                            brush.Opacity = opacity;
                            return brush;
                        }

                    case ParkingFacilityStatus.Unknown:
                        {
                            Color color = ((Color)App.Current.Resources["FacilityStatusUnknownColor"]);
                            SolidColorBrush brush = new SolidColorBrush(color);
                            brush.Opacity = opacity;
                            return brush;
                        }

                    default:
                        {
                            Color color = ((Color)App.Current.Resources["FacilityStatusUnknownColor"]);
                            SolidColorBrush brush = new SolidColorBrush(color);
                            brush.Opacity = opacity;
                            return brush;
                        }
                }
            }

            else
            {
                return ((SolidColorBrush)App.Current.Resources["FacilityStatusUnknownBrush"]);
            }
        }


        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
