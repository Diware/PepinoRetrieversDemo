using System;
using System.Globalization;

namespace Pepino.Retrievers.Additional
{
    public class DateTimeRetriever : IRetriever
    {
        private readonly IDateTimeProvider dateTimeProvider;

        public DateTimeRetriever(IDateTimeProvider dateTimeProvider)
        {
            this.dateTimeProvider = dateTimeProvider ?? throw new ArgumentNullException(nameof(dateTimeProvider));
        }

        public bool TryRetrieve(
            string stringRepresentation,
            CultureInfo cultureInfo,
            out object? result,
            out string? errorDesciption)
        {
            if (!DateTimeSpanLogic.TryGetDateTime(cultureInfo, stringRepresentation,
                dateTimeProvider,
                out DateTime rv,
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
