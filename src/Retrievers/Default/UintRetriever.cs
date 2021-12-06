using System;
using System.Globalization;

namespace Pepino.Retrievers.Default
{
    public class UintRetriever : IRetriever
    {
        public UintRetriever() { }

        private static Lazy<UintRetriever> _lazy
            => new Lazy<UintRetriever>(() => new UintRetriever());


        public static UintRetriever Instance => _lazy.Value;

        public bool TryRetrieve(
            string stringRepresentation,
            CultureInfo cultureInfo,
            out object? result,
            out string? errorDescription)
        {
            bool rv = true;
            errorDescription = null;

            if (uint.TryParse(stringRepresentation,
                NumberStyles.Integer,
                cultureInfo,
                out uint parsed))
            {
                result = parsed;
            }
            else
            {
                switch (stringRepresentation?.ToUpperInvariant())
                {
                    case "MAXVALUE":
                        result = uint.MaxValue;
                        break;
                    case "MINVALUE":
                        result = uint.MinValue;
                        break;
                    default:
                        rv = false;
                        errorDescription = $"Value '{stringRepresentation}' is not supported.";
                        result = default(uint);
                        break;
                }
            }

            return rv;
        }
    }
}
