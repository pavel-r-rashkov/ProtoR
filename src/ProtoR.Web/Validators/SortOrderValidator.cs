namespace ProtoR.Web.Validators
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FluentValidation;
    using ProtoR.Application.Common;

    public class SortOrderValidator : AbstractValidator<SortOrder>
    {
        private readonly IEnumerable<string> allowedProperties;

        public SortOrderValidator(IEnumerable<string> allowedProperties)
        {
            this.allowedProperties = allowedProperties;

            this.RuleFor(f => f.PropertyName)
                .Must(p => this.allowedProperties
                    .Any(ap => ap.Equals(p, StringComparison.InvariantCultureIgnoreCase)))
                .WithMessage($"Allowed properties for sorting: {string.Join(", ", this.allowedProperties)}");
        }
    }
}
