using System;
using System.Globalization;

namespace Pepino.Retrievers.Default
{
    public class UlongRetriever : IRetriever
    {
        private UlongRetriever() { }

        private static Lazy<UlongRetriever> lazy
            => new Lazy<UlongRetriever>(() => new UlongRetriever());


        public static UlongRetriever Instance => lazy.Value;


        public bool TryRetrieve(
            string stringRepresentation,
            CultureInfo cultureInfo,
            out object? result,
            out string? errorDescription)
        {
            bool rv = true;
            errorDescription = null;

            if (ulong.TryParse(stringRepresentation,
                NumberStyles.Integer,
                cultureInfo,
                out ulong parsed))
            {
                result = parsed;
            }
            else
            {
                switch (stringRepresentation?.ToUpperInvariant())
                {
                    case "MAXVALUE":
                        result = ulong.MaxValue;
                        break;
                    case "MINVALUE":
                        result = ulong.MinValue;
                        break;
                    default:
                        rv = false;
                        errorDescription = $"Value '{stringRepresentation}' is not supported.";
                        result = default(ulong);
                        break;
                }
            }

            return rv;
        }
    }
}
