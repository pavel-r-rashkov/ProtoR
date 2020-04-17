namespace ProtoR.Web.Validators
{
    using FluentValidation;
    using ProtoR.Web.Resources.ConfigurationResource;

    public class ConfigurationWriteModelValidator : AbstractValidator<ConfigurationWriteModel>
    {
        public ConfigurationWriteModelValidator()
        {
            this.When(config => !config.BackwardCompatible, () =>
            {
                this.RuleFor(config => config.ForwardCompatible)
                    .Equal(true)
                    .WithMessage("Either backward or forward compatibility or both need to be selected");
            });

            this.RuleForEach(config => config.RuleConfigurations)
                .NotNull()
                .SetValidator(new RuleConfigurationModelValidator());

            this.RuleFor(config => config.ConfigurationId)
                .Matches(@"^(\d+|global)$")
                .WithMessage("Configuration ID must be a number or \"global\"");
        }
    }
}
