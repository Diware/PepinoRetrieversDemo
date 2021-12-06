using Pepino.Retrievers.Default;
using System;

namespace Pepino.Retrievers
{
    public class DefaultRetrieversFactory
        : IRetrieversFactory
    {
        bool IRetrieversFactory.IsApplicable(Type type)
            => type.IsEnum
                || typeof(bool).Equals(type)
                || typeof(byte).Equals(type)
                || typeof(char).Equals(type)
                || typeof(decimal).Equals(type)
                || typeof(double).Equals(type)
                || typeof(float).Equals(type)
                || typeof(int).Equals(type)
                || typeof(long).Equals(type)
                || typeof(sbyte).Equals(type)
                || typeof(short).Equals(type)
                || typeof(uint).Equals(type)
                || typeof(ulong).Equals(type)
                || typeof(ushort).Equals(type);


        bool IRetrieversFactory.TryCreateInstance(
            OptionsRegistry optionsRegistry,
            Type type,
            out IRetriever? retriever,
            out string? errorDescription)
        {
            var rv = true;
            errorDescription = null;

            if (type.IsEnum)
            {
                retriever = new EnumRetriever(type);
            }
            else if (typeof(bool).Equals(type))
            {
                retriever = BoolRetriever.Instance;
            }
            else if (typeof(byte).Equals(type))
            {
                retriever = ByteRetriever.Instance;
            }
            else if (typeof(char).Equals(type))
            {
                retriever = CharRetriever.Instance;
            }
            else if (typeof(decimal).Equals(type))
            {
                retriever = DecimalRetriever.Instance;
            }
            else if (typeof(double).Equals(type))
            {
                retriever = DoubleRetriever.Instance;
            }
            else if (typeof(float).Equals(type))
            {
                retriever = FloatRetriever.Instance;
            }
            else if (typeof(int).Equals(type))
            {
                retriever = IntRetriever.Instance;
            }
            else if (typeof(long).Equals(type))
            {
                retriever = LongRetriever.Instance;
            }
            else if (typeof(sbyte).Equals(type))
            {
                retriever = SbyteRetriever.Instance;
            }
            else if (typeof(short).Equals(type))
            {
                retriever = ShortRetriever.Instance;
            }
            else if (typeof(uint).Equals(type))
            {
                retriever = UintRetriever.Instance;
            }
            else if (typeof(ulong).Equals(type))
            {
                retriever = UlongRetriever.Instance;
            }
            else if (typeof(ushort).Equals(type))
            {
                retriever = UshortRetriever.Instance;
            }
            else
            {
                retriever = null;
                errorDescription = $"Could not create a retriever of type {type.FullName}.";
                rv = false;
            }

            return rv;
        }
    }
}
