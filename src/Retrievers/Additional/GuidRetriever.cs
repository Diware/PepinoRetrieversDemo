using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Pepino.Retrievers.Additional
{
    /// <summary>
    /// Retrieves a <see cref="Guid"/> from a string representation.
    /// </summary>
    /// <seealso cref="Retrievers.RetrieverBase" />
    /// <remarks>
    /// String must be represented as such:
    /// * GUID-[up to 36 hexadecimal symbols, optionally mixed with hyphens 
    ///     (up to 7 hyphens)]
    /// </remarks>
    public class GuidRetriever : IRetriever
    {
        private readonly Regex rxCompactGuid = new Regex(
            @"^(GUID-|UUID-){1}([0-9a-f\-]{1,39})$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private GuidRetriever() { }


        private static Lazy<GuidRetriever> lazy
            => new Lazy<GuidRetriever>(() => new GuidRetriever());


        public static GuidRetriever Instance => lazy.Value;


        public bool TryRetrieve(
            string stringRepresentation,
            CultureInfo cultureInfo,
            out object? result,
            out string? errorDescription)
        {
            if (stringRepresentation is null)
            {
                result = default;
                errorDescription = "Cannot retrieve a GUID value from a NULL string.";
                return false;
            }

            stringRepresentation = stringRepresentation.Trim();

            if (stringRepresentation.Length > 44)
            {
                result = default(Guid);
                errorDescription = "String is too long.";
                return false;
            }

            bool rv = true;

            var m = rxCompactGuid.Match(stringRepresentation);
            if (m.Success)
            {
                var v = m.Groups[2].Value
                    .Replace("-", "", StringComparison.Ordinal);
                v = new string('0', 32 - v.Length) + v;
                errorDescription = null;
                result = new Guid(v);
            }
            //TODOH: support general GUID notations ("N", "D", "X")
            //TODOH: support for TechTalk's GUIDs (first chars are followed by zeros)
            else
            {
                result = default(Guid);
                errorDescription = "Invalid syntax.";
                rv = false;
            }

            return rv;
        }
    }
}
