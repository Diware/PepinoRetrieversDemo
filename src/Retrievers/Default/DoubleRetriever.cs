using System;
using System.Globalization;

namespace Pepino.Retrievers.Default
{
    public class DoubleRetriever : IRetriever
    {
        private DoubleRetriever() { }

        private static Lazy<DoubleRetriever> lazy
            => new Lazy<DoubleRetriever>(() => new DoubleRetriever());


        public static DoubleRetriever Instance => lazy.Value;


        public bool TryRetrieve(
            string stringRepresentation,
            CultureInfo cultureInfo,
            out object? result,
            out string? errorDescription)
        {
            bool rv = true;
            errorDescription = null;

            if (double.TryParse(stringRepresentation,
                NumberStyles.Float,
                cultureInfo,
                out double parsed))
            {
                result = parsed;
            }
            else
            {
                switch (stringRepresentation?.ToUpperInvariant())
                {
                    case "MAXVALUE":
                        result = double.MaxValue;
                        break;
                    case "MINVALUE":
                        result = double.MinValue;
                        break;
                    case "EPSILON":
                        result = double.Epsilon;
                        break;
                    case "POSITIVEINFINITY":
                    case "INFINITY":
                    case "+INFINITY":
                    case "∞":
                    case "+∞":
                        result = double.PositiveInfinity;
                        break;
                    case "NEGATIVEINFINITY":
                    case "-INFINITY":
                    case "-∞":
                        result = double.NegativeInfinity;
                        break;
                    default:
                        result = default(double);
                        errorDescription = $"Value '{stringRepresentation}' is not supported.";
                        rv = false;
                        break;
                }
            }

            return rv;
        }
    }
}
