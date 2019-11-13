using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using Microsoft.AspNetCore.Http;

namespace thirdconspiracy.Utilities.Utilities
{
    public static class FileUtilities
    {
        public static IEnumerable<string> ReadLines(IFormFile file)
        {
            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                foreach (var p in ReadLines(reader))
                {
                    yield return p;
                }
            }
        }

        public static IEnumerable<string> ReadLines(string compressedFilePath)
        {
            using (var zipReader = new StreamReader(File.OpenRead(compressedFilePath)))
            {
                using (var zip = new ZipArchive(zipReader.BaseStream))
                {
                    var stream = zip.Entries[0].Open();

                    if (stream == null)
                    {
                        yield break;
                    }

                    using (StreamReader reader = new StreamReader(stream))
                    {
                        foreach (var line in ReadLines(reader))
                        {
                            yield return line;
                        }
                    }
                }
            }
        }

        public static IEnumerable<string> ReadLines(StreamReader streamReader)
        {
            while (!streamReader.EndOfStream)
            {
                var line = streamReader.ReadLine();
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                yield return line;
            }
        }

    }
}
