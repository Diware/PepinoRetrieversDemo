using System;
using System.Globalization;

namespace Pepino.Retrievers.Default
{
    public class IntRetriever : IRetriever
    {
        private IntRetriever() { }

        private static Lazy<IntRetriever> lazy
            => new Lazy<IntRetriever>(() => new IntRetriever());


        public static IntRetriever Instance => lazy.Value;


        public bool TryRetrieve(
            string stringRepresentation,
            CultureInfo cultureInfo,
            out object? result,
            out string? errorDescription)
        {
            bool rv = true;
            errorDescription = null;

            if (int.TryParse(stringRepresentation,
                NumberStyles.Integer,
                cultureInfo,
                out int parsed))
            {
                result = parsed;
            }
            else
            {
                switch (stringRepresentation?.ToUpperInvariant())
                {
                    case "MAXVALUE":
                        result = int.MaxValue;
                        break;
                    case "MINVALUE":
                        result = int.MinValue;
                        break;
                    default:
                        rv = false;
                        errorDescription = $"Value '{stringRepresentation}' is not supported.";
                        result = default(int);
                        break;
                }
            }

            return rv;
        }
    }
}
