using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace thirdconspiracy.Utilities.Models
{
    public class JsonObjectSerializationOptions
    {
        #region Properties

        /// <summary>
        /// Determines whether or not to indent the ouput of the serialized JSON. Default is true
        /// </summary>
        public bool IndentOutput { get; set; }

        /// <summary>
        /// Determines whether or not to serialize properties that have null values. Default is true
        /// </summary>
        public bool SerializeNullProperties { get; set; }

        /// <summary>
        /// Determines whether to serialize enum values as their string representations
        /// (instead of representing them as their underlying int values). Default is false
        /// </summary>
        public bool SerializeEnumsAsStrings { get; set; }

        /// <summary>
        /// Determines whether to use the ISO 8601 date time formatting standard 
        /// for DateTime objects. Defaults to false.
        /// </summary>
        public bool UseIso8601DateFormatting { get; set; }

        #endregion

        // Ctor
        public JsonObjectSerializationOptions()
        {
            // Set defaults
            IndentOutput = true;
            SerializeNullProperties = true;
            SerializeEnumsAsStrings = false;
            UseIso8601DateFormatting = false;
        }
    }
}
