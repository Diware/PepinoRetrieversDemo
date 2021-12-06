using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Pepino
{
    internal class DateTimeSpanLogic
    {
        private static readonly IEnumerable<string> _markers
            = new List<string> { "Y", "M", "D", "h", "m", "s", "ms", "t" };
        private static readonly char[] _splitters
            = new[] { ' ', '+', '-' };
        private static readonly Regex _rxIsOffset = new Regex(
            @"^(?:\+|\-)?(?<time>\d{1,2}(:\d{2})?){1}$",
            RegexOptions.Compiled);
        private static readonly Regex _rxIsMarkerPart = new Regex(
            @"^(?:\+|\-){1}(\d{1,11})(Y|M|D|h|m|s|ms|t){1}$",
            RegexOptions.Compiled);
        private static readonly Regex _rxStartAsDateOnly = new Regex(
            @"^(\d{4}-\d{2}-\d{2})(?:[ Z\+\-]{1}.*)?$",
            RegexOptions.Compiled);
        private static readonly Regex _rxStartAsDateTime = new Regex(
            @"^(\d{4}-\d{2}-\d{2}[T ]{1}\d{1,2}:\d{2}(?:\:\d{2})?){1}(?:[ Z\+\-]{1}.*)?$",
            RegexOptions.Compiled);


        public static bool TryGetDateTime(
            CultureInfo cultureInfo,
            string value,
            IDateTimeProvider dateTimeProvider,
            out DateTime result,
            out string? errorDescription)
        {
            if (value is null)
            {
                result = default;
                errorDescription = "Cannot parse null.";
                return false;
            }

            value = value.Trim();

            if (value.Length > 200)
            {
                //TODOH: this check must include length of translated words (NOW/TODAY)
                /* string is too long */
                errorDescription = "String is too long";
                result = default;
                return false;
            }

            var rv = TryGetDT(cultureInfo,
                value,
                false,
                dateTimeProvider,
                out DateTime dtoResult,
                out errorDescription,
                out string? _);

            result = dtoResult;
            return rv;
        }


        public static bool TryGetDateTimeOffset(
            CultureInfo cultureInfo,
            string value,
            IDateTimeProvider dateTimeProvider,
            out DateTimeOffset result,
            out string? errorDescription)
        {
            if (value is null)
            {
                result = default;
                errorDescription = "Cannot parse null.";
                return false;
            }

            value = value.Trim();

            if (value.Length > 200)
            {
                //TODOH: this check must include length of translated words (NOW/TODAY)
                /* string is too long */
                errorDescription = "String is too long";
                result = default;
                return false;
            }

            if (!TryGetDT(cultureInfo,
                value,
                true,
                dateTimeProvider,
                out DateTime dt,
                out errorDescription,
                out string? offsetText))
            {
                result = default;
                return false;
            }


            dt = DateTime.SpecifyKind(dt, DateTimeKind.Unspecified);

            /* check offset has a valid syntax */
            if (offsetText is null)
            {
                errorDescription = "String does not end with Z or time offset.";
                result = default;
                return false;
            }
            else
            {
                var mOffset = _rxIsOffset.Match(offsetText);
                if (offsetText == "Z")
                {
                    result = new DateTimeOffset(dt, TimeSpan.Zero);
                }
                else if (mOffset.Success)
                {
                    var str1 = mOffset.Groups["time"].Value;
                    if (offsetText![..1] == "-")
                    {
                        str1 = "-" + str1;
                    }

                    TimeSpan tsOffset = str1.Contains(':')
                        ? TimeSpan.Parse(str1)
                        : TimeSpan.FromHours(int.Parse(str1));
                    try
                    {
                        result = new DateTimeOffset(dt, tsOffset);
                    }
#pragma warning disable CA1031 // Do not catch general exception types
                    catch (ArgumentOutOfRangeException)
                    {
                        errorDescription = "Invalid offset.";
                        result = default;
                        return false;
                    }
#pragma warning restore CA1031 // Do not catch general exception types
                }
                else
                {
                    errorDescription = "String does not end with Z or time offset.";
                    result = default;
                    return false;
                }
            }

            errorDescription = null;
            return true;
        }


        private static bool TryGetDT(
            CultureInfo cultureInfo,
            string value,
            bool withOffset,
            IDateTimeProvider dateTimeProvider,
            out DateTime result,
            out string? errorDescription,
            out string? offsetText)
        {
            errorDescription = null;
            offsetText = null;
            result = default;

            if (!TryGetStartingDateTime(value, cultureInfo, dateTimeProvider,
                out int usedSymbolsCount,
                out result))
            {
                errorDescription = "Value has no starting date or starting date cannot be parsed.";
                return false;
            }

            value = value[usedSymbolsCount..].Trim();
            if (value.Length == 0)
            {
                return true;
            }

            var parts = SplitIntoParts(value);
            if (withOffset)
            {
                offsetText = parts.Last();
                parts = parts.SkipLast(1);
            }

            Match m;

            foreach (var part in parts)
            {
                m = _rxIsMarkerPart.Match(part);
                if (!m.Success)
                {
                    errorDescription = $"'{part}' is not a valid time span modifier.";
                    result = default;
                    return false;
                }

                if (!TryModifyDate(part, ref result, out errorDescription))
                {
                    return false;
                }
            }

            return true;
        }


        //TODOH: try supplying some very bad string - to get an exception thrown
        // Pepino must handle such exceptions
        public static bool TryGetTimeSpan(
            CultureInfo cultureInfo,
            string value,
            out TimeSpan result,
            out string? errorDescription)
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            value = value.Trim();

            if (value.Length > 200)
            {
                //TODOH: this check must include length of translated words (NOW/TODAY)
                /* string is too long */
                errorDescription = "String is too long";
                result = default;
                return false;
            }

            var parts = SplitIntoParts(value);
            result = TimeSpan.Zero;
            Match m;
            foreach (var part in parts)
            {
                m = _rxIsMarkerPart.Match(part);
                if (!m.Success)
                {
                    errorDescription = $"'{part}' is not a valid time span modifier.";
                    result = default;
                    return false;
                }


                if (!TryConvertPartToTimeSpan(part, out TimeSpan tsPart, out errorDescription))
                {
                    return false;
                }

                try
                {
                    result = result.Add(tsPart);
                }
#pragma warning disable CA1031 // Do not catch general exception types
                catch (OverflowException)
                {
                    errorDescription = "Value expression results in time span out of valid range.";
                    result = default;
                    return false;
                }
#pragma warning restore CA1031 // Do not catch general exception types
            }

            errorDescription = null;
            return true;
        }


        private static bool TryConvertPartToTimeSpan(
            string part,
            out TimeSpan result,
            out string? errorDescription)
        {
            part = part.Trim();

            var m = _rxIsMarkerPart.Match(part);
            if (!m.Success)
            {
                errorDescription = $"'{part}' is not a valid time span modifier.";
                result = default;
                return false;
            }

            /* try to extract integral value of part */
            long val;
            var str = (part.StartsWith('-') ? "-" : string.Empty)
                + m.Groups[1].Value;
            try
            {
                val = long.Parse(str);
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (FormatException)
            {
                errorDescription = $"Part '{part}' is not of valid syntax.";
                result = default;
                return false;
            }
            catch (OverflowException)
            {
                errorDescription = $"Value {str} is out of range for int64.";
                result = default;
                return false;
            }
#pragma warning restore CA1031 // Do not catch general exception types

            /* try to create a result */
            str = m.Groups[2].Value;
            try
            {

                switch (str)
                {
                    case "Y":
                        //TODOH: use option for days number in year
                        result = TimeSpan.FromDays(val * 365);
                        break;
                    case "M":
                        //TODOH: use option for days number in month
                        result = TimeSpan.FromDays(val * 30);
                        break;
                    case "D":   /* days */
                        result = TimeSpan.FromDays(val);
                        break;
                    case "h":   /* hours */
                        result = TimeSpan.FromHours(val);
                        break;
                    case "m":   /* minutes */
                        result = TimeSpan.FromMinutes(val);
                        break;
                    case "s":   /* seconds */
                        result = TimeSpan.FromSeconds(val);
                        break;
                    case "ms":
                        result = TimeSpan.FromMilliseconds(val);
                        break;
                    case "t":
                        result = TimeSpan.FromTicks(val);
                        break;
                    //hey, hey, continue here....
                    default:
                        errorDescription = $"Part '{part}' is not of valid syntax.";
                        result = default;
                        return false;
                }
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (OverflowException)
            {
                errorDescription = $"Value {str} is too large to be used as a time span.";
                result = default;
                return false;
            }
#pragma warning restore CA1031 // Do not catch general exception types

            errorDescription = null;
            return true;
        }


        private static bool TryModifyDate(
            string part,
            ref DateTime dto,
            out string? errorDescription)
        {
            part = part.Trim();

            var m = _rxIsMarkerPart.Match(part);
            if (!m.Success)
            {
                errorDescription = $"'{part}' is not a valid time span modifier.";
                return false;
            }

            var numPart = (part.StartsWith('-') ? "-" : string.Empty)
                + m.Groups[1].Value;
            var markerPart = m.Groups[2].Value;

            int valInt;
            long valLng;
            double valDbl;

            try
            {

                switch (markerPart)
                {
                    case "Y":
                        if (TryGetAsInt(numPart, ref part, out valInt, out errorDescription))
                        {
                            dto = dto.AddYears(valInt);
                        }
                        break;
                    case "M":
                        if (TryGetAsInt(numPart, ref part, out valInt, out errorDescription))
                        {
                            dto = dto.AddMonths(valInt);
                        }
                        break;
                    case "D":   /* days */
                        if (TryGetAsDouble(numPart, ref part, out valDbl, out errorDescription))
                        {
                            dto = dto.AddDays(valDbl);
                        }
                        break;
                    case "h":   /* hours */
                        if (TryGetAsDouble(numPart, ref part, out valDbl, out errorDescription))
                        {
                            dto = dto.AddHours(valDbl);
                        }
                        break;
                    case "m":   /* minutes */
                        if (TryGetAsDouble(numPart, ref part, out valDbl, out errorDescription))
                        {
                            dto = dto.AddMinutes(valDbl);
                        }
                        break;
                    case "s":   /* seconds */
                        if (TryGetAsDouble(numPart, ref part, out valDbl, out errorDescription))
                        {
                            dto = dto.AddSeconds(valDbl);
                        }
                        break;
                    case "ms":
                        if (TryGetAsDouble(numPart, ref part, out valDbl, out errorDescription))
                        {
                            dto = dto.AddMilliseconds(valDbl);
                        }
                        break;
                    case "t":
                        if (TryGetAsLong(numPart, ref part, out valLng, out errorDescription))
                        {
                            dto = dto.AddTicks(valLng);
                        }
                        break;
                    default:
                        errorDescription = $"Part '{part}' is not of valid syntax.";
                        return false;
                }
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (ArgumentOutOfRangeException)
            {
                errorDescription = $"Value {numPart} is too large to be used as a time span.";
                return false;
            }
#pragma warning restore CA1031 // Do not catch general exception types

            errorDescription = null;
            return true;
        }


        private static bool TryGetAsInt(
            string value,
            ref string part,
            out int result,
            out string? errorDescription)
        {
            try
            {
                result = int.Parse(value);
                errorDescription = null;
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (FormatException)
            {
                errorDescription = $"Part '{part}' is not of valid syntax.";
                result = default;
                return false;
            }
            catch (OverflowException)
            {
                errorDescription = $"Value {value} is out of range for int32.";
                result = default;
                return false;
            }
#pragma warning restore CA1031 // Do not catch general exception types

            return true;
        }


        private static bool TryGetAsLong(
            string value,
            ref string part,
            out long result,
            out string? errorDescription)
        {
            try
            {
                result = long.Parse(value);
                errorDescription = null;
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (FormatException)
            {
                errorDescription = $"Part '{part}' is not of valid syntax.";
                result = default;
                return false;
            }
            catch (OverflowException)
            {
                errorDescription = $"Value {value} is out of range for int64.";
                result = default;
                return false;
            }
#pragma warning restore CA1031 // Do not catch general exception types

            return true;
        }


        private static bool TryGetAsDouble(
            string value,
            ref string part,
            out double result,
            out string? errorDescription)
        {
            try
            {
                //TODOH: should ve support fractional parts here in parsing??
                result = double.Parse(value);
                errorDescription = null;
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (FormatException)
            {
                errorDescription = $"Part '{part}' is not of valid syntax.";
                result = default;
                return false;
            }
            catch (OverflowException)
            {
                errorDescription = $"Value {value} is out of range for double.";
                result = default;
                return false;
            }
#pragma warning restore CA1031 // Do not catch general exception types

            return true;
        }


        /// <summary>
        /// Splits a string into parts at "+", "-", or space characters.
        /// "+" and "-" characters are preserved in the resulting parts, while
        /// spaces are trimmed.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <returns></returns>
        internal static IEnumerable<string> SplitIntoParts(string s)
        {
            List<string> rv = new List<string>();

            int prevPos = 0;
            int nxtPos = 0;
            string s1, sToAdd = string.Empty;

            nxtPos = s.IndexOfAny(_splitters, nxtPos);
            bool lastShot = true;
            while (nxtPos > -1 || lastShot)
            {
                if (nxtPos > -1)
                {
                    s1 = s[prevPos..nxtPos].Trim();
                }
                else
                {
                    s1 = s[prevPos..].Trim();
                    lastShot = false;
                }
                if (s1.Length != 0)
                {
                    rv.Add(AddLeadingPlus(s1));
                }
                if (nxtPos > -1)
                {
                    prevPos = nxtPos;
                    nxtPos = s.IndexOfAny(_splitters, nxtPos + 1);
                }
            }
            if (sToAdd.Length > 0)
            {
                rv.Add(sToAdd);
            }

            return rv
                .Select(x => x.Trim())
                .AsEnumerable();


            static string AddLeadingPlus(string val)
            {
                if (val.Length == 0)
                {
                    return val;
                }

                if (val != "Z"
                    && val[..1] != "+" && val[..1] != "-")
                {
                    val = "+" + val;
                }
                return val;
            }
        }


        /// <summary>
        /// Gets the starting date time part or empty string.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <returns></returns>
        private static bool TryGetStartingDateTime(
            string s,
            CultureInfo cultureInfo,
            IDateTimeProvider dateTimeProvider,
            out int usedSymbolsCount,
            out DateTime result)
        {
            result = default;
            usedSymbolsCount = 0;

            if (s == "TODAY")
            {
                result = dateTimeProvider.Now.Date;
                usedSymbolsCount = 5;
                return true;
            }
            if (s == "NOW")
            {
                result = dateTimeProvider.Now.LocalDateTime;
                usedSymbolsCount = 3;
                return true;
            }

            foreach (var spl in _splitters)
            {
                if (s.StartsWith("TODAY" + spl))
                {
                    result = dateTimeProvider.Now.Date;
                    usedSymbolsCount = 5;
                    return true;
                }

                if (s.StartsWith("NOW" + spl))
                {
                    result = dateTimeProvider.Now.LocalDateTime;
                    usedSymbolsCount = 3;
                    return true;
                }
            }


            /* 
             * Neither TODAY nor NOW have been located.
             * Next we will try to use a (<DATETIME>) pattern,
             * if impossible - we will try to locate a date+time, 
             * if impossible - we will try to locate date only.
             */

            if (s.StartsWith('('))
            {
                var idx = s.IndexOf(')', 1);
                if (idx < 0)
                {
                    // no closing bracket found
                    return false;
                }

                var sDT = s[1..idx].Trim();
                if (DateTime.TryParse(sDT, cultureInfo,
                    DateTimeStyles.None,
                    out result))
                {
                    usedSymbolsCount = idx + 1;
                    return true;
                }
                else
                {
                    return false;
                }
            }

            var m = _rxStartAsDateTime.Match(s);
            if (m.Success)
            {
                var sVal = m.Groups[1].Value;
                var len = sVal.Length;
                var containsT = sVal.Contains('T');
                var containsSecs = len >= 18;
                var frmt = $"yyyy-MM-dd{(containsT ? "T" : " ")}H:mm{(containsSecs ? ":ss" : "")}";

                if (DateTime.TryParseExact(sVal, frmt,
                    cultureInfo, DateTimeStyles.None,
                    out result))
                {
                    usedSymbolsCount = len;
                    return true;
                }
                else
                {
                    return false;
                }
            }

            m = _rxStartAsDateOnly.Match(s);
            if (m.Success)
            {
                var sVal = m.Groups[1].Value;
                var len = sVal.Length;
                var frmt = "yyyy-MM-dd";

                if (DateTime.TryParseExact(sVal, frmt,
                    cultureInfo, DateTimeStyles.None,
                    out result))
                {
                    usedSymbolsCount = len;
                    return true;
                }
                else
                {
                    return false;
                }
            }

            return false;
        }
    }
}
