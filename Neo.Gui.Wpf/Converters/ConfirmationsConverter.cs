using System;
using System.Globalization;
using System.Windows.Data;
using Neo.UI.Core.Globalization.Resources;

namespace Neo.Gui.Wpf.Converters
{
    public class ConfirmationsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return Strings.Unconfirmed;
            }

            var confirmations = long.Parse(value.ToString());

            if (confirmations == 0)
            {
                return Strings.Unconfirmed;
            }

            if (confirmations > 10)
            {
                return "+10";
            }

            return confirmations.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
