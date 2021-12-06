using System;
using System.Globalization;

namespace Pepino.Retrievers.Default
{
    public class CharRetriever : IRetriever
    {
        private CharRetriever() { }

        private static Lazy<CharRetriever> lazy
            => new Lazy<CharRetriever>(() => new CharRetriever());


        public static CharRetriever Instance => lazy.Value;


        public bool TryRetrieve(
            string stringRepresentation,
            CultureInfo cultureInfo,
            out object? result,
            out string? errorDescription)
        {
            bool rv = true;
            errorDescription = null;

            if (char.TryParse(stringRepresentation, out char parsed))
            {
                result = parsed;
            }
            else
            {
                if (stringRepresentation != null
                    && (stringRepresentation.StartsWith(@"\u", StringComparison.Ordinal)
                    || stringRepresentation.StartsWith(@"\x", StringComparison.Ordinal)))
                {
                    var s = stringRepresentation[2..];
                    if (int.TryParse(s,
                        NumberStyles.AllowHexSpecifier,
                        cultureInfo,
                        out int i))
                    {
                        result = (char)i;
                    }
                    else
                    {
                        rv = false;
                        result = default(char);
                    }
                }
                else
                {
                    switch (stringRepresentation?.ToUpperInvariant())
                    {
                        case "MAXVALUE":
                            result = char.MaxValue;
                            break;
                        case "MINVALUE":
                            result = char.MinValue;
                            break;
                        default:
                            rv = false;
                            errorDescription = $"Value '{stringRepresentation}' is not supported.";
                            result = default(char);
                            break;
                    }
                }
            }

            return rv;
        }
    }
}
