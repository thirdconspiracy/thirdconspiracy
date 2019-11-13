using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace thirdconspiracy.Utilities.Extensions
{
    public static class AssemblyExtensions
    {
        public static Stream GetStream(this Assembly assembly, string name)
        {
            var embeddedResources = assembly.GetManifestResourceNames();
            var resourceName = GetExactMatch(name, embeddedResources)
                               ?? GetPartialMatch(name, embeddedResources);

            if (resourceName == null)
            {
                return null;
            }

            var assemblyStream = assembly.GetManifestResourceStream(resourceName);
            return resourceName.EndsWith(".gz", StringComparison.OrdinalIgnoreCase)
                ? new GZipStream(assemblyStream, CompressionMode.Decompress)
                : assemblyStream;
        }

        private static string GetExactMatch(string name, string[] embeddedResources)
        {
            return embeddedResources
                .FirstOrDefault(n => string.Equals(n, name, StringComparison.OrdinalIgnoreCase));
        }

        private static string GetPartialMatch(string desiredResourceName, string[] embeddedResources)
        {
            string resourceName = null;
            foreach (var embeddedResource in embeddedResources)
            {
                if (embeddedResource == null)
                {
                    continue;
                }

                // Match on *.{name}
                if (embeddedResource.EndsWith(desiredResourceName, StringComparison.OrdinalIgnoreCase))
                {
                    if (embeddedResource[embeddedResource.Length - desiredResourceName.Length - 1] == '.')
                    {
                        if (resourceName != null)
                        {
                            throw new NotSupportedException(
                                $"Multiple partial matches were found for the embedded resource name '{desiredResourceName}'.");
                        }

                        resourceName = embeddedResource;
                    }
                }
            }

            return resourceName;
        }

        public static IEnumerable<string> GetLines(this Assembly assembly, string resourceName)
        {
            using (var streamReader = new StreamReader(GetStream(assembly, resourceName)))
            {
                while (!streamReader.EndOfStream)
                {
                    yield return streamReader.ReadLine();
                }
            }
        }

        public static string GetString(this Assembly assembly, string resourceName)
        {
            var stream = assembly.GetStream(resourceName);
            if (stream == null)
            {
                throw new Exception("Could not find resource " + resourceName);
            }
            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

    }
}
