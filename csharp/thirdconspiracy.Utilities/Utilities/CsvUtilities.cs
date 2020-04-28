using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using CsvHelper;
using CsvHelper.Configuration;
using thirdconspiracy.Utilities.Extensions;

namespace thirdconspiracy.Utilities.Utilities
{
    public static class CsvUtilities
    {
        public static IEnumerable<string[]> ParseZipFile(string reportPath)
        {
            using (var zipReader = new StreamReader(File.OpenRead(reportPath)))
            {
                using (var zip = new ZipArchive(zipReader.BaseStream))
                {
                    var stream = zip.Entries[0].Open();
                    return stream == null
                        ? Enumerable.Empty<string[]>()
                        : Parse(stream);
                }
            }
        }

        public static IEnumerable<T> Parse<T>(IEnumerable<string> lines)
        {
            using (var ieStream = new IEnumerableStringReader(lines))
            using (var csvReader = new CsvReader(ieStream, CultureInfo.InvariantCulture))
            {
                return csvReader.GetRecords<T>();
            }
        }

        public static IEnumerable<string[]> Parse(string csvText)
        {
            return Parse<string[]>(csvText.ToStream());
        }

        public static IEnumerable<string[]> Parse(Stream csvStream)
        {
            return Parse<string[]>(csvStream);
        }

        public static IEnumerable<T> Parse<T>(Stream csvStream)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = ",", TrimOptions = TrimOptions.Trim };
            using (var sr = new StreamReader(csvStream))
            using (var csvReader = new CsvReader(sr, config))
            {
                return csvReader.GetRecords<T>();
            }
        }

    }
}
