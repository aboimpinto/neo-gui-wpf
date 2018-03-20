using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace Neo.Gui.Wpf.MarkupExtensions
{
    public class VisibleIfExtension : MarkupExtension
    {

        public string Path { get; set; }

        public VisibleIfExtension()
        {
            // NOP
        }

        public VisibleIfExtension(string path)
        {
            this.Path = path;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return new Binding
            {
                Path = new PropertyPath(this.Path),
                Converter = new InternalBoolToVisibilityConverter()
            }.ProvideValue(serviceProvider);
        }
    }

    internal class InternalBoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                if ((bool)value)
                {
                    return Visibility.Visible;
                }

                return Visibility.Collapsed;
            }

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
