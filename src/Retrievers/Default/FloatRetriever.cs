using System;
using System.Globalization;

namespace Pepino.Retrievers.Default
{
    public class FloatRetriever : IRetriever
    {
        private FloatRetriever() { }

        private static Lazy<FloatRetriever> lazy
            => new Lazy<FloatRetriever>(() => new FloatRetriever());


        public static FloatRetriever Instance => lazy.Value;

        public bool TryRetrieve(
            string stringRepresentation,
            CultureInfo cultureInfo,
            out object? result,
            out string? errorDescription)
        {
            bool rv = true;
            errorDescription = null;

            if (float.TryParse(stringRepresentation,
                NumberStyles.Float,
                cultureInfo,
                out float parsed))
            {
                result = parsed;
            }
            else
            {
                switch (stringRepresentation?.ToUpperInvariant())
                {
                    case "MINVALUE":
                        result = float.MinValue;
                        break;
                    case "MAXVALUE":
                        result = float.MaxValue;
                        break;
                    case "EPSILON":
                        result = float.Epsilon;
                        break;
                    case "POSITIVEINFINITY":
                    case "INFINITY":
                    case "+INFINITY":
                    case "∞":
                    case "+∞":
                        result = float.PositiveInfinity;
                        break;
                    case "NEGATIVEINFINITY":
                    case "-INFINITY":
                    case "-∞":
                        result = float.NegativeInfinity;
                        break;
                    default:
                        rv = false;
                        errorDescription = $"Value '{stringRepresentation}' is not supported.";
                        result = default(float);
                        break;
                }
            }

            return rv;
        }
    }
}
