using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace thirdconspiracy.Utilities.Extensions
{
    public static class EnumExtensions
    {

        public static TEnum ToEnumOrDefault<TEnum>(this string value, TEnum defaultValue) where TEnum : struct
        {
            return Enum.TryParse(value ?? string.Empty, true, out TEnum parsedValue)
                ? parsedValue
                : defaultValue;
        }

    }
}
