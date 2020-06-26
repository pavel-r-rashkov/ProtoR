namespace ProtoR.Web.Validators
{
    using FluentValidation;
    using ProtoR.Web.Resources.UserResource;

    public class UserPostModelValidator : AbstractValidator<UserPostModel>
    {
        public UserPostModelValidator()
        {
            this.RuleFor(u => u.UserName)
                .NotNull()
                .NotEmpty()
                .Matches("^[A-Za-z0-9]*$")
                .MaximumLength(500);

            this.RuleFor(u => u.Password)
                .NotNull()
                .NotEmpty()
                .SetValidator(new PasswordValidator());

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
