using System;
using System.Globalization;
using System.Windows.Data;

namespace MemoryFilter.Converters {

    public class ProgressToStringConverter : IValueConverter {
        public static ProgressToStringConverter Instance = new ProgressToStringConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (Equals(value, null)) {
                return "";
            }

            var progress = (decimal) value;

            if (Equals(0m, progress)) {
                return "";
            }

            return progress.ToString("0.00%");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }

}