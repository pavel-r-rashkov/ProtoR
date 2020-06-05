namespace ProtoR.Web.Validators
{
    using FluentValidation;
    using ProtoR.Web.Resources.RoleResource;

    public class RoleWriteModelValidator : AbstractValidator<RoleWriteModel>
    {
        public RoleWriteModelValidator()
        {
            this.RuleFor(r => r.Name)
                .NotEmpty()
                .NotNull()
                .MaximumLength(500);
        }
    }
}
