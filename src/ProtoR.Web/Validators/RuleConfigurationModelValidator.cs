namespace ProtoR.Web.Validators
{
    using FluentValidation;
    using ProtoR.Web.Resources.ConfigurationResource;

    public class RuleConfigurationModelValidator : AbstractValidator<RuleConfigurationModel>
    {
        public RuleConfigurationModelValidator()
        {
            this.RuleFor(r => r.Code).Matches("(PB)[\\d]{4}");
            this.RuleFor(r => r.Severity)
                .GreaterThan(0)
                .LessThan(5);
        }
    }
}
