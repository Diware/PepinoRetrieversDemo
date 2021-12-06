using Pepino.Retrievers.Additional;
using System;
using System.Drawing;

namespace Pepino.Retrievers
{
    public class AdditionalRetrieversFactory
        : IRetrieversFactory
    {
        bool IRetrieversFactory.IsApplicable(Type type)
            => typeof(Color).Equals(type)
            || typeof(DateTimeOffset).Equals(type)
            || typeof(DateTime).Equals(type)
            || typeof(Guid).Equals(type)
            || typeof(TimeSpan).Equals(type);

        bool IRetrieversFactory.TryCreateInstance(
            OptionsRegistry optionsRegistry,
            Type type,
            out IRetriever? retriever,
            out string? errorDescription)
        {
            var rv = true;
            errorDescription = null;

            if (typeof(Color).Equals(type))
            {
                retriever = ColorRetriever.Instance;
            }
            else if (typeof(Guid).Equals(type))
            {
                retriever = GuidRetriever.Instance;
            }
            else if (typeof(TimeSpan).Equals(type))
            {
                retriever = TimeSpanRetriever.Instance;
            }
            else if (typeof(DateTimeOffset).Equals(type)
                || typeof(DateTime).Equals(type))
            {
                var opts = optionsRegistry.GetOptions<AdditionalRetriversOptions>();
                if (opts is null)
                {
                    errorDescription = $"Must contain options of type {typeof(AdditionalRetriversOptions).FullName}.";
                    retriever = null;
                    rv = false;
                }
                else
                {
                    var dtp = opts.GetDateTimeProvider();
                    if (typeof(DateTimeOffset).Equals(type))
                    {
                        retriever = new DateTimeOffsetRetriever(dtp);
                    }
                    else
                    {
                        retriever = new DateTimeRetriever(dtp);
                    }
                }
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
