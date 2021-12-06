using System;
using System.Globalization;

namespace Pepino.Retrievers.Default
{
    public class UshortRetriever : IRetriever
    {
        private UshortRetriever() { }

        private static Lazy<UshortRetriever> lazy
            => new Lazy<UshortRetriever>(() => new UshortRetriever());


        public static UshortRetriever Instance => lazy.Value;

        public bool TryRetrieve(
            string stringRepresentation,
            CultureInfo cultureInfo,
            out object? result,
            out string? errorDescription)
        {
            bool rv = true;
            errorDescription = null;

            if (ushort.TryParse(stringRepresentation,
                NumberStyles.Integer,
                cultureInfo,
                out ushort parsed))
            {
                result = parsed;
            }
            else
            {
                switch (stringRepresentation?.ToUpperInvariant())
                {
                    case "MAXVALUE":
                        result = ushort.MaxValue;
                        break;
                    case "MINVALUE":
                        result = ushort.MinValue;
                        break;
                    default:
                        rv = false;
                        errorDescription = $"Value '{stringRepresentation}' is not supported.";
                        result = default(ushort);
                        break;
                }
            }

            return rv;
        }
    }
}
