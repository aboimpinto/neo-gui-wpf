using System;
using System.Globalization;
using System.Windows.Data;

namespace Neo.Gui.Wpf.Converters
{
    public class GasAmountFormatterConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return "";
            }

            return string.Format(
                CultureInfo.InvariantCulture,
                "{0:0,0.0}",
                value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
