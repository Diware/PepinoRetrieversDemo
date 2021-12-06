using Pepino.Retrievers;
using System;

namespace Pepino
{
    public static class PepinoExtensions
    {
        /// <summary>
        /// Adds the default Pepino retrievers for value types.
        /// </summary>
        public static PepinoFacade AddDefaultRetrievers(
            this PepinoFacade @this)
        {
            @this.RegisterRetrieversFactory(
                new DefaultRetrieversFactory());
            return @this;
        }


        /// <summary>
        /// Adds additional Pepino retrievers:
        /// - Color;
        /// - GUID;
        /// - DateTime;
        /// - DateTimeOffset;
        /// - TimeSpan.
        /// </summary>
        public static PepinoFacade AddAdditionalRetrievers(
            this PepinoFacade @this,
            Action<AdditionalRetriversOptions> options)
        {
            var opts = new AdditionalRetriversOptions();
            options.Invoke(opts);
            _ = @this.RegisterOptions(opts);

            @this.RegisterRetrieversFactory(
                new AdditionalRetrieversFactory());

            return @this;
        }
    }
}
