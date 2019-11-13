using System;
using System.Collections.Generic;
using System.Linq;

namespace tc.Base64
{
    public static class Encode
    {
        private const string Base64Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";

        public static string HexToBase64(string hexString)
        {
            if (hexString.Length % 2 != 0)
            {
                throw new ArgumentException("It's odd");
            }
            var base64String = HexToBinary(hexString)
                .Process(BinaryTo64Binary)
                .Process(Binary64ToString);

            return base64String;
        }

        private static string HexToBinary(string hexString)
        {
            var splitStrs = hexString
                .Select(c => Convert
                    .ToString(Convert.ToInt32(c.ToString(), 16), 2)
                    .PadLeft(4, '0'));
            var binary = string.Concat(splitStrs);
            return binary;
        }
        private static string Binary64ToString(IEnumerable<char[]> binary64CharArrays)
        {
            var chars = binary64CharArrays
                .Select(ca => Convert.ToInt32(string.Concat(ca), 2))
                .Select(i => Base64Chars[i]).ToList();

            var paddingCharsRequired = (4 - chars.Count % 4) % 4;
            for (var i = 0; i < paddingCharsRequired; i++)
            {
                chars.Add('=');
            }

            var result = string.Concat(chars);
            return result;
        }

        private static IEnumerable<char[]> BinaryTo64Binary(string binaryString)
        {
            var batchedCharArrays = binaryString
                .Select(c => c)
                .Batch(6)
                .Select(batchedCharArray =>
                {
                    var padded = Enumerable.Repeat('0', 8).ToArray();
                    for (var i = 0; i < batchedCharArray.Count; i++)
                    {
                        padded[2 + i] = batchedCharArray[i];
                    }

                    return padded;
                });
            return batchedCharArrays;
        }

    }
}
