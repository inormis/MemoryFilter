using System;
using System.Globalization;
using System.Windows.Data;

namespace MemoryFilter.Converters {

    public class BytesToMegabytesTextConverter : IValueConverter {
        public static readonly BytesToMegabytesTextConverter Instance = new BytesToMegabytesTextConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            return $"{value:N1} mb";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }

}