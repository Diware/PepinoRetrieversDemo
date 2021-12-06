using System;
using System.Globalization;

namespace Pepino.Retrievers.Default
{
    public class DecimalRetriever : IRetriever
    {
        private DecimalRetriever() { }

        private static Lazy<DecimalRetriever> lazy
            => new Lazy<DecimalRetriever>(() => new DecimalRetriever());


        public static DecimalRetriever Instance => lazy.Value;


        public bool TryRetrieve(
            string stringRepresentation,
            CultureInfo cultureInfo,
            out object? result,
            out string? errorDescription)
        {
            bool rv = true;
            errorDescription = null;

            if (decimal.TryParse(stringRepresentation,
                NumberStyles.Float,
                cultureInfo,
                out decimal parsed))
            {
                result = parsed;
            }
            else
            {
                switch (stringRepresentation?.ToUpperInvariant())
                {
                    case "MAXVALUE":
                        result = decimal.MaxValue;
                        break;
                    case "MINVALUE":
                        result = decimal.MinValue;
                        break;
                    case "MINUSONE":
                        result = decimal.MinusOne;
                        break;
                    case "ONE":
                        result = decimal.One;
                        break;
                    case "ZERO":
                        result = decimal.Zero;
                        break;
                    default:
                        result = default(decimal);
                        errorDescription = $"Value '{stringRepresentation}' is not supported.";
                        rv = false;
                        break;
                }
            }

            return rv;
        }
    }
}
