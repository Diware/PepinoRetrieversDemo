using System;
using System.Globalization;

namespace Pepino.Retrievers.Additional
{
    public class TimeSpanRetriever : IRetriever
    {
        private TimeSpanRetriever() { }


        private static Lazy<TimeSpanRetriever> lazy
            => new Lazy<TimeSpanRetriever>(() => new TimeSpanRetriever());

        public static TimeSpanRetriever Instance => lazy.Value;



        public bool TryRetrieve(
            string stringRepresentation,
            CultureInfo cultureInfo,
            out object? result,
            out string? errorDesciption)
        {
            if (stringRepresentation is null)
            {
                errorDesciption = "Cannot retrieve a TimeSpan value from a NULL string.";
                result = default;
                return false;
            }

            if (!DateTimeSpanLogic.TryGetTimeSpan(
                cultureInfo,
                stringRepresentation,
                out TimeSpan rv,
                out errorDesciption))
            {
                result = default;
                return false;
            }

            result = rv;
            return true;
        }
    }
}
