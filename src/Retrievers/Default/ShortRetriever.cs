using System;
using System.Globalization;

namespace Pepino.Retrievers.Default
{
    public class ShortRetriever : IRetriever
    {
        private ShortRetriever() { }

        private static Lazy<ShortRetriever> lazy
            => new Lazy<ShortRetriever>(() => new ShortRetriever());


        public static ShortRetriever Instance => lazy.Value;


        public bool TryRetrieve(
            string stringRepresentation,
            CultureInfo cultureInfo,
            out object? result,
            out string? errorDescription)
        {
            bool rv = true;
            errorDescription = null;

            if (short.TryParse(stringRepresentation,
                NumberStyles.Integer,
                cultureInfo,
                out short parsed))
            {
                result = parsed;
            }
            else
            {
                switch (stringRepresentation?.ToUpperInvariant())
                {
                    case "MINVALUE":
                        result = short.MinValue;
                        break;
                    case "MAXVALUE":
                        result = short.MaxValue;
                        break;
                    default:
                        rv = false;
                        errorDescription = $"Value '{stringRepresentation}' is not supported.";
                        result = default(short);
                        break;
                }
            }

            return rv;
        }
    }
}
