using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using thirdconspiracy.Utilities.Utilities;

namespace thirdconspiracy.Utilities.Extensions
{
    public static class StringExtensions
    {

        public static readonly Regex NonAlphaNumericRegex = new Regex("[^a-zA-Z0-9]", RegexOptions.Compiled);
        public static readonly Regex WhiteSpaceRegex = new Regex(@"\s", RegexOptions.Compiled);


        public static Stream ToStream(this string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        public static string ToTitleCase(this string text)
        {
            if (text == null)
            {
                return string.Empty;
            }
            var fromSnakeCase = text.Replace("_", " ");
            // When lower to upper found, add space between
            var lowerToUpper = Regex.Replace(fromSnakeCase, @"(\p{Ll})(\p{Lu})", "$1 $2");
            // When 2+ upper case found, add space before last upper
            var sentenceCase = Regex.Replace(lowerToUpper, @"(\p{Lu}+)(\p{Lu}\p{Ll})", "$1 $2");
            // Set start of each work to upper
            return new CultureInfo("en-US", false).TextInfo.ToTitleCase(sentenceCase);
        }

        public static bool TryParseBool(this string value, out bool parsedValue)
        {
	        return BooleanUtilities.TryFuzzyParse(value, out parsedValue);
        }

        public static string TakeRight(this string text, int maxLength)
        {
	        if (text == null)
	        {
		        return string.Empty;
	        }

	        var actualLength = Math.Min(text.Length, maxLength);
	        var startIndex = text.Length - actualLength;

	        text = text.Substring(startIndex, actualLength);
	        return text;
        }

        public static string Truncate(this string s, int maxLength)
        {
            s = s?.Substring(0, Math.Min(s.Length, maxLength));
            return s ?? string.Empty;
        }

        public static string ToCData(this string text)
        {
            return string.IsNullOrWhiteSpace(text)
                ? string.Empty
                : $"<![CDATA[{text}]]>";
        }

        /// <summary>
        /// Get byte count. If text is null, returns 0. 
        /// Uses UTF8 encoding by default.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static int GetSafeByteCount(this string text)
        {
            return GetSafeByteCount(text, Encoding.UTF8);
        }

        /// <summary>
        /// Get byte count. If text is null, returns 0.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static int GetSafeByteCount(this string text, Encoding encoding)
        {
            return text == null
                ? 0
                : encoding.GetByteCount(text);
        }

        public static bool Contains(this string s, string value, StringComparison comparisonType)
        {
            if (s != null)
            {
                return s.IndexOf(value, comparisonType) > -1;
            }
            return value == null;
        }

        public static IEnumerable<string> SplitOptimized(this string input, string[] splitStrs, StringSplitOptions options = StringSplitOptions.None)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                if (options == StringSplitOptions.None)
                {
                    yield return string.Empty;
                }
                yield break;
            }

            if (splitStrs.Length == 0)
            {
                yield return input;
                yield break;
            }

            var startIndex = 0;
            for (var currentChar = 0; currentChar < input.Length; currentChar++)
            {
                var currentSubstring = input.Substring(startIndex, input.Length - startIndex);
                var foundStr = splitStrs.FirstOrDefault(s => currentSubstring.StartsWith(s));

                if (string.IsNullOrWhiteSpace(foundStr))
                {
                    continue;
                }

                //Match Found
                var substrLength = currentChar - startIndex;
                if (substrLength > 0)
                {
                    yield return input.Substring(startIndex, substrLength);
                }
                else if (options == StringSplitOptions.None)
                {
                    //split found back to back
                    yield return string.Empty;
                }

                if (currentChar == input.Length - 1 && options == StringSplitOptions.None)
                {
                    //split found at end
                    yield return string.Empty;
                }

                startIndex = currentChar + foundStr.Length;
            }

            //No Match
            if (startIndex == 0)
            {
                yield return input;
                yield break;
            }

            //Last token
            if (startIndex < input.Length)
            {
                yield return input.Substring(startIndex, input.Length - startIndex);
            }
        }

        public static IEnumerable<string> SplitOptimized(this string input, char[] splitChars, StringSplitOptions options = StringSplitOptions.None)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                if (options == StringSplitOptions.None)
                {
                    yield return string.Empty;
                }
                yield break;
            }

            if (splitChars.Length == 0)
            {
                yield return input;
                yield break;
            }

            var startIndex = 0;
            for (var currentChar = 0; currentChar < input.Length; currentChar++)
            {
                //No Match
                if (splitChars.All(c => input[currentChar] != c))
                {
                    continue;
                }

                //Match Found
                var substrLength = currentChar - startIndex;
                if (substrLength > 0)
                {
                    yield return input.Substring(startIndex, substrLength);
                }
                else if (options == StringSplitOptions.None)
                {
                    //split found back to back
                    yield return string.Empty;
                }

                if (currentChar == input.Length - 1 && options == StringSplitOptions.None)
                {
                    //split found at end
                    yield return string.Empty;
                }

                startIndex = currentChar + 1;
            }

            //No Match
            if (startIndex == 0)
            {
                yield return input;
                yield break;
            }

            //Last token
            if (startIndex < input.Length)
            {
                yield return input.Substring(startIndex, input.Length - startIndex);
            }
        }
    }
}
