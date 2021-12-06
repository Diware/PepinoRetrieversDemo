using System;
using System.Globalization;

namespace Pepino.Retrievers.Default
{
    public class BoolRetriever : IRetriever
    {
        private BoolRetriever() { }

        private static readonly Lazy<BoolRetriever> _lazy
            = new Lazy<BoolRetriever>(() => new BoolRetriever());

        public static BoolRetriever Instance => _lazy.Value;

        public bool TryRetrieve(
            string stringRepresentation,
            CultureInfo cultureInfo,
            out object? result,
            out string? errorDescription)
        {
            bool rv = true;
            errorDescription = null;

            if (bool.TryParse(stringRepresentation, out bool parsed))
            {
                result = parsed;
            }
            else
            {
                switch (stringRepresentation?.ToUpperInvariant())
                {
                    case "T":
                        result = true;
                        break;
                    case "F":
                        result = false;
                        break;
                    default:
                        rv = false;
                        errorDescription = $"Value '{stringRepresentation}' is not supported.";
                        result = default(bool);
                        break;
                };
            }

            return rv;
        }
    }
}
