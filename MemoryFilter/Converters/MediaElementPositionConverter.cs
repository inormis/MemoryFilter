using System;
using System.Globalization;
using System.Windows.Data;

namespace MemoryFilter.Converters {

    public class MediaElementPositionConverter : IMultiValueConverter {
        public static readonly MediaElementPositionConverter Instance = new MediaElementPositionConverter();

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
            var currentTime = (TimeSpan) values[0];
            var totalTime = (TimeSpan) values[1];
            return currentTime.TotalSeconds / totalTime.TotalSeconds;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
            /* var newPosition = (double) value;
            var totalTime = (TimeSpan) parameter;
            return TimeSpan.FromSeconds(totalTime.TotalSeconds*newPosition);*/
            throw new NotImplementedException();
        }
    }

}