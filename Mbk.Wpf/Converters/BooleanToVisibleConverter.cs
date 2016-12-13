using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Mbk.Wpf.Converters
{
    public class BooleanToVisibleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool? isVisible = value as bool?;
            return isVisible.HasValue && isVisible.Value ?
                Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return false;
            }

            Visibility visibility;
            bool isValid = Enum.TryParse(value.ToString(), out visibility);
            return isValid && visibility == Visibility.Visible;
        }
    }
}
