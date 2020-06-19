namespace ProtoR.Web.Validators
{
    using FluentValidation;
    using ProtoR.Domain.SchemaGroupAggregate;

    public class GroupRestrictionValidator : AbstractValidator<string>
    {
        public GroupRestrictionValidator()
        {
            this.When(u => u != null, () =>
            {
                this.RuleFor(u => u)
                    .Matches(GroupRestriction.PatternValidator);
            });
        }
    }
}
