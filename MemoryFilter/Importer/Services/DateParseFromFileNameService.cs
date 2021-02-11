using System;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;

namespace MemoryFilter.Importer.Services {

    public class DateParseFromFileNameService {
        private static readonly Regex regex = new Regex("\\d{8}", RegexOptions.Compiled);

        public static bool TryGetFrom(string filePath, out DateTime dateTime) {
            var fileName = Path.GetFileName(filePath);
            dateTime = DateTime.MinValue;
            var match = regex.Match(fileName);
            if (match.Success && DateTime.TryParseExact(match.Groups[0].Value, "yyyyMMdd", CultureInfo.InvariantCulture,
                DateTimeStyles.None, out dateTime)) {
                return true;
            }

            return false;
        }
    }

}