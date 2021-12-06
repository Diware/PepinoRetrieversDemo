using System;
using System.Globalization;

namespace Pepino.Retrievers.Default
{
    public class SbyteRetriever : IRetriever
    {
        private SbyteRetriever() { }

        private static Lazy<SbyteRetriever> lazy
            => new Lazy<SbyteRetriever>(() => new SbyteRetriever());


        public static SbyteRetriever Instance => lazy.Value;


        public bool TryRetrieve(
            string stringRepresentation,
            CultureInfo cultureInfo,
            out object? result,
            out string? errorDescription)
        {
            bool rv = true;
            errorDescription = null;

            //TODO: add HEX support??
            if (sbyte.TryParse(stringRepresentation,
                NumberStyles.Integer,
                cultureInfo,
                out sbyte parsed))
            {
                result = parsed;
            }
            else
            {
                switch (stringRepresentation?.ToUpperInvariant())
                {
                    case "MAXVALUE":
                        result = sbyte.MaxValue;
                        break;
                    case "MINVALUE":
                        result = sbyte.MinValue;
                        break;
                    default:
                        rv = false;
                        errorDescription = $"Value '{stringRepresentation}' is not supported.";
                        result = default(sbyte);
                        break;
                }
            }

            return rv;
        }
    }
}
