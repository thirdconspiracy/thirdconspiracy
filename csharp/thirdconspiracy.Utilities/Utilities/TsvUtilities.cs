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
    public class TsvUtilities
    {

        public static IEnumerable<string[]> ParseZipFile(string reportPath)
        {
            using (var zipReader = new StreamReader(File.OpenRead(reportPath)))
            {
                using (var zip = new ZipArchive(zipReader.BaseStream))
                {
                    var stream = zip.Entries[0].Open();

                    if (stream == null)
                    {
                        return Enumerable.Empty<string[]>();
                    }

                    using (var reader = new StreamReader(stream))
                    {
                        return Parse(reader);
                    }
                }
            }
        }

        public static IEnumerable<string[]> Parse(string csvText)
        {
            using (var csvStream = new StreamReader(csvText.ToStream()))
            {
                return Parse(csvStream);
            }
        }

        public static IEnumerable<string[]> Parse(StreamReader csvStream)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = "\t", TrimOptions = TrimOptions.Trim };
            using (var parser = new CsvParser(csvStream, config))
            {
                string[] line;
                while ((line = parser.Read()) != null)
                {
                    yield return line;
                }
            }
        }

    }
}
