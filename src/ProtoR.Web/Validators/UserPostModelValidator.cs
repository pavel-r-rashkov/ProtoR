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
                .MaximumLength(500);

            this.RuleFor(u => u.Password)
                .NotNull()
                .NotEmpty()
                .MinimumLength(12)
                .MaximumLength(500);
        }
    }
}
