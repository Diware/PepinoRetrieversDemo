using System;
using System.Globalization;

namespace Pepino.Retrievers.Default
{
    public class LongRetriever : IRetriever
    {
        private LongRetriever() { }

        private static Lazy<LongRetriever> lazy
            => new Lazy<LongRetriever>(() => new LongRetriever());


        public static LongRetriever Instance => lazy.Value;

        public bool TryRetrieve(
            string stringRepresentation,
            CultureInfo cultureInfo,
            out object? result,
            out string? errorDescription)
        {
            bool rv = true;
            errorDescription = null;

            if (long.TryParse(stringRepresentation,
                NumberStyles.Integer,
                cultureInfo,
                out long parsed))
            {
                result = parsed;
            }
            else
            {
                switch (stringRepresentation?.ToUpperInvariant())
                {
                    case "MINVALUE":
                        result = long.MinValue;
                        break;
                    case "MAXVALUE":
                        result = long.MaxValue;
                        break;
                    default:
                        result = default(long);
                        errorDescription = $"Value '{stringRepresentation}' is not supported.";
                        rv = false;
                        break;
                }
            }

            return rv;
        }
    }
}
