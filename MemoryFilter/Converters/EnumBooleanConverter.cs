using System;
using System.Globalization;
using System.Windows.Data;

namespace MemoryFilter.Converters {

    public class EnumBooleanConverter : IValueConverter {
        public static EnumBooleanConverter Instance = new EnumBooleanConverter();

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            return parameter?.Equals(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            return parameter;
        }

        #endregion
    }

}