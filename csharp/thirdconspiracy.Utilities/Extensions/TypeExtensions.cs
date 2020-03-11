using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace thirdconspiracy.Utilities.Extensions
{
    public static class TypeExtensions
    {
        //TODO: Move to const class
        private static readonly string POBOX_REGEX_PATTERN =
            @"^\s*((#\d+)|((box|bin)[-\.\s\/\\]*\d+)|(.*p[-\.\s\/\\]*\s*(o|0)[-\.\s\/\\]*((box|bin)|b|(#|num|number)*\d+))|(p(ost)?\s*(o(ff(ice)?)?)?\s*((box|bin)|b)?(([-\.\s\/\\]+)|$)\d*)|(p\s*-?\/?(o)?\*-?box)|((box|bin)|b)\s*(number|num|#)?\s*\d+|(num|number|#)\s*\d+)";

        public static bool IsPoBox(string addressLine)
        {
            var re = new Regex(POBOX_REGEX_PATTERN, RegexOptions.IgnoreCase);
            return re.IsMatch(addressLine);
        }

        public static bool IsNumericType(this object o)
        {
            return IsNumericType(o.GetType());
        }

        public static bool IsNumericType(this Type t)
        {
            switch (Type.GetTypeCode(t))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsImeiType(this string imei)
        {
	        try
	        {
		        if (string.IsNullOrWhiteSpace(imei) || imei.Length > 15)
		        {
			        return false;
		        }

		        if (imei.Length < 15)
		        {
			        imei = imei.PadLeft(15, '0');
		        }

		        var digits = imei.ToCharArray()
			        .Select(c => int.Parse(c.ToString()))
			        .ToArray();

		        var sum =
			        digits[0] +
			        ImeiDoubled(digits[1]) +
			        digits[2] +
			        ImeiDoubled(digits[3]) +
			        digits[4] +
			        ImeiDoubled(digits[5]) +
			        digits[6] +
			        ImeiDoubled(digits[7]) +
			        digits[8] +
			        ImeiDoubled(digits[9]) +
			        digits[10] +
			        ImeiDoubled(digits[11]) +
			        digits[12] +
			        ImeiDoubled(digits[13]);

		        var checkDigit = 10 - (sum % 10);
		        return digits[14] == checkDigit;
	        }
	        catch (Exception)
	        {
		        return false;
	        }
        }

        private static int ImeiDoubled(int digit)
        {
	        var total = digit * 2;
	        if (total < 10)
	        {
		        return total;
	        }
	        return total - 9;
        }


        public static bool IsGtinType(this string gtin)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(gtin) || gtin.Length < 5 || gtin.Length > 14)
                {
                    return false;
                }

                var digits = gtin.ToCharArray()
                    .Select(c => int.Parse(c.ToString()))
                    .ToArray();

                //Calcluate Checksum
                var currentMultiplier = 3;
                var sum = 0;
                for (var index = digits.Length - 2; index >= 0; index--)
                {
                    sum += digits[index] * currentMultiplier;
                    currentMultiplier = currentMultiplier == 3 ? 1 : 3;
                }

                var calculatedCheckDigit = 10 - (sum % 10);
                if (calculatedCheckDigit == 10)
                {
                    calculatedCheckDigit = 0;
                }

                return digits.Last() == calculatedCheckDigit;
            }
            catch (Exception)
            {
                return false;
            }

        }

        public static bool IsIssnType(this string s)
        {
            if (string.IsNullOrWhiteSpace(s))
            {
                return false;
            }

            var issn = s.Replace("-", "");
            try
            {
                for (var i = 0; i < issn.Length - 1; i++)
                {
                    if (!char.IsDigit(issn[i]))
                    {
                        return false;
                    }
                }

                var checkDigit = issn.Last();
                var weightedSum =
                    issn[0] * 8 +//int.parse?
                    issn[1] * 7 +
                    issn[2] * 6 +
                    issn[3] * 5 +
                    issn[4] * 4 +
                    issn[5] * 3 +
                    issn[6] * 2;
                var checksumValue = 11 - (weightedSum % 11);
                var checksum = checksumValue == 10
                    ? 'X'
                    : Convert.ToChar(checksumValue);

                return char.ToUpperInvariant(checkDigit) == char.ToUpperInvariant(checksum);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool IsIsbnType(this string s)
        {
            if (string.IsNullOrWhiteSpace(s))
            {
                return false;
            }


            var isbn = s.Replace("-", "");
            try
            {
                switch (isbn.Length)
                {
                    case 10:
                        return ValidateIsbn10(isbn);
                    case 13:
                        return ValidateIsbn13(isbn);
                    default:
                        return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static bool ValidateIsbn10(string isbn)
        {
            var actualCheckDigit = isbn.Last();
            if (!char.IsDigit(actualCheckDigit) && actualCheckDigit != 'X')
            {
                return false;
            }

            for (int i = 0; i < isbn.Length - 1; i++)
            {
                if (!char.IsDigit(isbn[i]))
                {
                    return false;
                }
            }
            // formula (11 - (d1 x 10 + d2 x 9 + d3 x 8 ... d9 x 2) % 11) % 11
            var sum = 0;
            var multiplier = 10;
            for (var i = 0; i < isbn.Length - 1; i++)
            {
                sum += int.Parse(isbn.Substring(i, 1)) * multiplier;
                multiplier--;
            }

            var checkDigit = (11 - (sum % 11)) % 11;
            var computedCheckDigit = checkDigit == 10
                ? "X"
                : checkDigit.ToString();

            return string.Equals(computedCheckDigit, isbn.Substring(isbn.Length - 1, 1), StringComparison.OrdinalIgnoreCase);
        }

        private static bool ValidateIsbn13(string isbn)
        {
            if (isbn.Any(c => !Char.IsDigit(c)))
            {
                return false;
            }

            // formula 10 - (d1 x 1 + d2 x 3 + d3 x 1 ... d12 x 3) % 10)
            int sum = 0;
            int multiplier = 1;
            for (int i = 0; i < isbn.Length - 1; i++)
            {
                sum += int.Parse(isbn.Substring(i, 1)) * multiplier;
                multiplier = multiplier == 1 ? 3 : 1;
            }

            var checkDigit = 10 - (sum % 10);
            // replace 10 with 0
            if (checkDigit == 10)
            {
                checkDigit = 0;
            }
            return string.Equals(checkDigit.ToString(), isbn.Substring(isbn.Length - 1, 1), StringComparison.OrdinalIgnoreCase);
        }

    }
}
