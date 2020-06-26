namespace ProtoR.Web.Validators
{
    using FluentValidation;
    using ProtoR.Web.Resources.UserResource;

    public class UserPutModelValidator : AbstractValidator<UserPutModel>
    {
        public UserPutModelValidator()
        {
            this.RuleFor(u => u.NewPassword)
                .SetValidator(new PasswordValidator());

            this.When(u => u.NewPassword != null, () =>
            {
                this.RuleFor(u => u.OldPassword)
                    .NotEmpty()
                    .NotNull();
            });

            this.RuleFor(u => u.GroupRestrictions)
                .NotNull()
                .NotEmpty();

            this.RuleForEach(u => u.GroupRestrictions)
                .NotEmpty()
                .NotNull()
                .SetValidator(new GroupRestrictionValidator());
        }
    }
}
