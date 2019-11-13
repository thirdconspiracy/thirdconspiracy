using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using ICSharpCode.SharpZipLib.Zip;
using LZ4;

namespace thirdconspiracy.Utilities.Utilities
{
    public static class Compression
    {

        #region Member Variables

        private const int BUFFER_SIZE_BYTES = 8192;

        #endregion Member Variables

        #region GZip

        /// <summary>
        /// Compresses a string value to byte array.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] Compress(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }

            var data = Encoding.UTF8.GetBytes(value);

            using (var msi = new MemoryStream(data, false))
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(mso, CompressionLevel.Fastest))
                {
                    msi.CopyTo(gs);
                }
                return mso.ToArray();
            }
        }

        /// <summary>
        /// Decompresses a byte array to original string value.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string Decompress(byte[] data)
        {
            if (data == null)
            {
                return null;
            }

            using (var msi = new MemoryStream(data, false))
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(msi, CompressionMode.Decompress))
                {
                    gs.CopyTo(mso);
                }
                return Encoding.UTF8.GetString(mso.ToArray());
            }
        }

        /// <summary>
        /// Compresses a string value to a base 64 encoded string value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string CompressToString(string value)
        {
            var data = Compress(value);
            if (data != null)
            {
                return Convert.ToBase64String(data);
            }
            return null;
        }

        /// <summary>
        /// Decompresses a string value to original string value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string DecompressFromString(string value)
        {
            if (value == null)
            {
                return null;
            }

            return Decompress(Convert.FromBase64String(value));
        }

        public static string GZipCompressString(string text)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(text);
            var memoryStream = new MemoryStream();
            using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Compress, true))
            {
                gZipStream.Write(buffer, 0, buffer.Length);
            }

            memoryStream.Position = 0;

            var compressedData = new byte[memoryStream.Length];
            memoryStream.Read(compressedData, 0, compressedData.Length);

            var gZipBuffer = new byte[compressedData.Length + 4];
            Buffer.BlockCopy(compressedData, 0, gZipBuffer, 4, compressedData.Length);
            Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, gZipBuffer, 0, 4);
            return Convert.ToBase64String(gZipBuffer);
        }

        public static string GZipDecompressString(string compressedText)
        {
            byte[] gZipBuffer = Convert.FromBase64String(compressedText);
            using (var memoryStream = new MemoryStream())
            {
                int dataLength = BitConverter.ToInt32(gZipBuffer, 0);
                memoryStream.Write(gZipBuffer, 4, gZipBuffer.Length - 4);

                var buffer = new byte[dataLength];

                memoryStream.Position = 0;
                using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
                {
                    gZipStream.Read(buffer, 0, buffer.Length);
                }

                return Encoding.UTF8.GetString(buffer);
            }
        }

        #endregion GZip

        #region LZ4

        public static byte[] CompressLz4(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }

            var data = Encoding.UTF8.GetBytes(value);

            using (var msi = new MemoryStream(data, false))
            using (var mso = new MemoryStream())
            {
                using (var gs = new LZ4Stream(mso, LZ4StreamMode.Compress))
                {
                    msi.CopyTo(gs);
                }
                return mso.ToArray();
            }
        }

        public static string DecompressLz4(byte[] data)
        {
            if (data == null)
            {
                return null;
            }

            using (var msi = new MemoryStream(data, false))
            using (var mso = new MemoryStream())
            {
                using (var gs = new LZ4Stream(msi, LZ4StreamMode.Decompress))
                {
                    gs.CopyTo(mso);
                }

                return Encoding.UTF8.GetString(mso.ToArray());
            }
        }

        public static string CompressLz4ToString(string value)
        {
            var data = CompressLz4(value);
            if (data != null)
            {
                return Convert.ToBase64String(data);
            }
            return null;
        }

        public static string DecompressLz4FromString(string value)
        {
            if (value == null)
            {
                return null;
            }

            return DecompressLz4(Convert.FromBase64String(value));
        }

        #endregion LZ4

        /// <summary>
        /// Unzip all the files in a ZIP folder to the same directory, preserving the directory tree
        /// in the ZIP file (if one exists)
        /// </summary>
        /// <returns>A collection of paths to the unzipped files</returns>
        public static IEnumerable<string> UnzipToSameDirectory(string zipFilePath)
        {
            var containingFolder = Path.GetDirectoryName(zipFilePath);

            using (var zipInputStream = new ZipInputStream(File.OpenRead(zipFilePath)))
            {
                ZipEntry zipEntry;

                // loop through every file in the ZIP
                while ((zipEntry = zipInputStream.GetNextEntry()) != null)
                {
                    var directoryName = Path.Combine(containingFolder, Path.GetDirectoryName(zipEntry.Name));
                    var fileName = Path.GetFileName(zipEntry.Name);
                    var outputFileNameWithPath = Path.Combine(directoryName, fileName);

                    // create directory
                    if (directoryName.Length > 0)
                    {
                        Directory.CreateDirectory(directoryName);
                    }

                    if (fileName == string.Empty)
                    {
                        continue;
                    }

                    using (var streamWriter = File.Create(outputFileNameWithPath))
                    {
                        var data = new byte[BUFFER_SIZE_BYTES];
                        while (true)
                        {
                            var size = zipInputStream.Read(data, 0, data.Length);
                            if (size > 0)
                            {
                                streamWriter.Write(data, 0, size);
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                    yield return outputFileNameWithPath;
                }
            }
        }

    }
}
