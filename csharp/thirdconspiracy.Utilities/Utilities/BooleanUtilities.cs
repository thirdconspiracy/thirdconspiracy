using System;
using System.Collections.Generic;

namespace thirdconspiracy.Utilities.Utilities
{
    public static class BooleanUtilities
    {
        private static readonly HashSet<string> _yesStrings
            = new HashSet<string> { "yes", "y", "true", "t", "1", "enabled", "on" };
        private static readonly HashSet<string> _noStrings
            = new HashSet<string>{ "no", "n", "false", "f", "0", "disabled", "off" };

        public static bool TryFuzzyParse(string input, out bool value)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                value = false;
                return false;
            }

            var cleanInput = input.Trim().ToLower();

            if (_yesStrings.Contains(cleanInput))
            {
                value = true;
                return true;
            }

            if (_noStrings.Contains(cleanInput))
            {
                value = false;
                return true;
            }

            value = false;
            return false;
        }

        public static bool FuzzyParse(string input)
        {
            if (TryFuzzyParse(input, out var value))
            {
                return value;
            }

            throw new FormatException("String was not recognized as a valid Boolean.");
        }
    }
}
