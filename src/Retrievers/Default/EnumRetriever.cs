using System;
using System.Globalization;
using System.Linq;

namespace Pepino.Retrievers.Default
{
    public class EnumRetriever : IRetriever
    {
        public EnumRetriever(Type typeToRetrieve)
        {
            if (!typeToRetrieve.IsEnum)
            {
                throw new ArgumentException("Type is not enum type.", nameof(typeToRetrieve));
            }

            this.typeToRetrieve = typeToRetrieve;
        }


        private static readonly char[] enumSeparators = new char[] { ',', '+' };
        private readonly Type typeToRetrieve;

        bool IRetriever.TryRetrieve(
            string stringRepresentation,
            CultureInfo cultureInfo,
            out object? result,
            out string? errorDescription)
        {
            errorDescription = null;

            if (stringRepresentation is null)
            {
                result = default;
                errorDescription = "String representation is empty.";
                return false;
            }

            var targetType = typeToRetrieve;

            var parts = stringRepresentation
                .Split(enumSeparators, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim());
            var s = string.Join(", ", parts);
            result = Enum.Parse(targetType, s);
            return true;
        }
    }
}
