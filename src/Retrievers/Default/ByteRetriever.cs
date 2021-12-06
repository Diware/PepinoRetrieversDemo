using System;
using System.Globalization;

namespace Pepino.Retrievers.Default
{
    public class ByteRetriever : IRetriever
    {
        private ByteRetriever() { }

        private static Lazy<ByteRetriever> lazy
            => new Lazy<ByteRetriever>(() => new ByteRetriever());


        public static ByteRetriever Instance => lazy.Value;


        public bool TryRetrieve(
            string stringRepresentation,
            CultureInfo cultureInfo,
            out object? result,
            out string? errorDescription)
        {
            bool rv = true;
            errorDescription = null;

            if (byte.TryParse(stringRepresentation,
                NumberStyles.Integer,
                cultureInfo,
                out byte parsed))
            {
                result = parsed;
            }
            else
            {
                switch (stringRepresentation?.ToUpperInvariant())
                {
                    case "MAXVALUE":
                        result = byte.MaxValue;
                        break;
                    case "MINVALUE":
                        result = byte.MinValue;
                        break;
                    default:
                        rv = false;
                        errorDescription = $"Value '{stringRepresentation}' is not supported.";
                        result = default(byte);
                        break;
                }
            }

            return rv;
        }
    }
}
