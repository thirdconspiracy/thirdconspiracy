using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace thirdconspiracy.Utilities.Extensions
{
    public static class DictionaryExtensions
    {
        public static bool TryGetValue<T>(this IDictionary<string, object> dictionary, string key, out T value)
        {
            value = default(T);
            if (!dictionary.TryGetValue(key, out var objValue))
            {
                return false;
            }

            if (objValue != null)
            {
                value = (T)objValue;
            }
            return true;
        }

        /// <summary>
        /// Get an Enum value with the given key out of the given dictionary.
        /// If the key does not exist in the dictionary or it's value is null/empty or invalid,
        /// return the default value for this Enum i.e. the first/zero value.
        /// </summary>
        /// <param name="dictionary">The dictionary to look up.</param>
        /// <param name="key">The key to look up by.</param>
        /// <returns>An Enum value - defaults to that Enum's default.</returns>
        public static TEnum GetSafeEnum<TEnum>(this IDictionary<string, string> dictionary, string key) where TEnum : struct
        {
            return dictionary.GetValueOrEmpty(key).ToEnumOrDefault(default(TEnum));
        }

        /// <summary>
        /// Get the string value with the given key out of the given dictionary.
        /// If the key does not exist in the dictionary - return an empty string.
        /// </summary>
        /// <param name="dictionary">The dictionary to look up.</param>
        /// <param name="key">The key to look up by.</param>
        /// <returns>A string.</returns>
        public static string GetValueOrEmpty(this IDictionary<string, string> dictionary, string key)
        {
            return dictionary.TryGetValue(key, out var value)
                ? value
                : string.Empty;
        }

        public static int AddOrIncrement<T>(this IDictionary<T, int> dictionary, T key, int value)
        {
            dictionary.TryGetValue(key, out var currentValue);
            var newValue = currentValue + value;
            dictionary[key] = newValue;
            return newValue;
        }

    }
}
