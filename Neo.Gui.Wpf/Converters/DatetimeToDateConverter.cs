using System;
using System.Globalization;
using System.Windows.Data;

namespace Neo.Gui.Wpf.Converters
{
    public class DatetimeToDateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return string.Empty;
            }

            var dateTime = DateTime.Parse(value.ToString());

            return $"{dateTime.Year:00}.{dateTime.Month:00}.{dateTime.Day:00}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
