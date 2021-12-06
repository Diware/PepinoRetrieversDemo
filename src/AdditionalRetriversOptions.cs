using Diware.SL;
using Diware.SL.Exceptions;
using System;

namespace Pepino
{
    public class AdditionalRetriversOptions : IOptions
    {
        public void Validate()
        {
            if (GetDateTimeProvider is null)
            {
                throw new OptionsException($"{nameof(GetDateTimeProvider)} is not set.");
            }
        }


        public Func<IDateTimeProvider> GetDateTimeProvider { get; set; }
    }
}
