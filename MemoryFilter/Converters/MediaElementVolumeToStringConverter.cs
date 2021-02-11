using System;
using System.Globalization;
using System.Windows.Data;

namespace MemoryFilter.Converters {

    public class MediaElementVolumeToStringConverter : IValueConverter {
        private static readonly double BytesInMegabyte = 1024 * 1024;
        public static readonly MediaElementVolumeToStringConverter Instance = new MediaElementVolumeToStringConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            return ((double) value * 100).ToString("N0");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }

}