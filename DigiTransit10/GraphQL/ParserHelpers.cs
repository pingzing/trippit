using System;
using System.Globalization;

namespace DigiTransit10.GraphQL
{
    public static class ParserHelpers
    {
        /// <summary>
        /// Converts the given value into a string, and performs some cleanup logic for easier server-side consumption.
        /// Wraps strings in backslashes and quotes, and ensures that numerics get ToString()ed into the Invariant forms.
        /// </summary>
        /// <param name="value">The Value to convert into a string.</param>
        /// <returns>The string form of the given value.</returns>
        public static string ParseValue(object value, DateParsingStrategy dateHandling = DateParsingStrategy.HyphenSeparators)
        {
            if (value is string) //because the server needs strings escaped... ◔_◔
            {
                return $"\\\"{value}\\\"";
            }
            if (value is int)
            {
                int intVal = (int)value;
                return $"{intVal.ToString(NumberFormatInfo.InvariantInfo)}";
            }
            if(value is bool)
            {
                bool boolVal = (bool)value;
                return boolVal.ToString().ToLowerInvariant();
            }
            if (value is float)
            {
                float floatVal = (float)value;
                return $"{floatVal.ToString(NumberFormatInfo.InvariantInfo)}";
            }
            if (value is double)
            {
                double doubleVal = (double)value;
                return $"{doubleVal.ToString(NumberFormatInfo.InvariantInfo)}";
            }
            //server expects Dates as strings (with the associated double quotes and backslashes) in ISO 8601 format: YYYY-MM-DD. Apparently YYYY-M-D is fine too.
            if(value is DateTime)
            {
                DateTime dateTimeVal = (DateTime)value;
                switch (dateHandling)
                {
                    case DateParsingStrategy.HyphenSeparators:
                        return $"\\\"{dateTimeVal.Date.Year.ToString(NumberFormatInfo.InvariantInfo)}-{dateTimeVal.Date.Month.ToString("00")}-{dateTimeVal.Date.Day.ToString("00")}\\\"";
                    case DateParsingStrategy.NoSeparators:
                        return $"\\\"{dateTimeVal.Date.Year.ToString(NumberFormatInfo.InvariantInfo)}{dateTimeVal.Date.Month.ToString("00")}{dateTimeVal.Date.Day.ToString("00")}\\\"";
                }                                
            }
            //server expects Times as strings (with associated double quotes and backslashes) in 24-hr HH:MM:SS format. H:M:S (24-hr) seems to be okay as well.
            if(value is TimeSpan)
            {
                TimeSpan timeSpanVal = (TimeSpan)value;
                return $"\\\"{timeSpanVal.Hours.ToString(NumberFormatInfo.InvariantInfo)}:{timeSpanVal.Minutes.ToString(NumberFormatInfo.InvariantInfo)}:{timeSpanVal.Seconds.ToString(NumberFormatInfo.InvariantInfo)}\\\"";
            }
            else
            {
                return $"{value}";
            }
        }
    }
}
