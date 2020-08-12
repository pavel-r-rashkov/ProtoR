namespace ProtoR.Web.Validators
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FluentValidation;
    using ProtoR.Application.Common;

    public class FilterValidator : AbstractValidator<Filter>
    {
        private readonly IEnumerable<string> allowedProperties;

        public FilterValidator(IEnumerable<string> allowedProperties)
        {
            this.allowedProperties = allowedProperties;

            this.RuleFor(f => f.PropertyName)
                .Must(p => this.allowedProperties
                    .Any(ap => ap.Equals(p, StringComparison.InvariantCultureIgnoreCase)))
                .WithMessage($"Allowed properties for filtering: {string.Join(", ", this.allowedProperties)}");
        }
    }
}
