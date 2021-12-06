using System;
using System.Globalization;

namespace Pepino.Retrievers.Additional
{
    public class DateTimeOffsetRetriever : IRetriever
    {
        private readonly IDateTimeProvider dateTimeProvider;

        public DateTimeOffsetRetriever(IDateTimeProvider dateTimeProvider)
        {
            this.dateTimeProvider = dateTimeProvider ?? throw new ArgumentNullException(nameof(dateTimeProvider));
        }

        public bool TryRetrieve(
            string stringRepresentation,
            CultureInfo cultureInfo,
            out object? result,
            out string? errorDesciption)
        {
            if (!DateTimeSpanLogic.TryGetDateTimeOffset(cultureInfo, stringRepresentation,
                dateTimeProvider,
                out DateTimeOffset rv,
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
