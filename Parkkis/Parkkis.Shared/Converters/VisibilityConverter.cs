using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Parkkis.Converters
{
    /// <summary>
    /// Visibility converter
    /// </summary>
    public class VisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool isVisible = true;

            if (value is Visibility)
            {
                return value;
            }

            else if (value is bool)
            {
                isVisible = (bool)value;
            }

            else if (value is int || value is short || value is long)
            {
                isVisible = 0 != (int)value;
            }

            else if (value is float || value is double)
            {
                isVisible = 0.0 != (double)value;
            }

            else if (value is string && string.IsNullOrEmpty((string)value))
            {
                isVisible = false;
            }

            else if (value is IEnumerable<object>)
            {
                isVisible = ((IEnumerable<object>)value).Any();
            }

            else if (value is IEnumerable)
            {
                isVisible = ((IEnumerable)value).GetEnumerator().MoveNext();
            }

            else if (value == null)
            {
                isVisible = false;
            }

            return isVisible ? Visibility.Visible : Visibility.Collapsed;
        }


        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }

    }
}
