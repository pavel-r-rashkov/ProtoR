namespace ProtoR.Web.Validators
{
    using FluentValidation;

    public class PasswordValidator : AbstractValidator<string>
    {
        public PasswordValidator()
        {
            this.When(u => u != null, () =>
            {
                this.RuleFor(u => u)
                    .MinimumLength(14)
                    .MaximumLength(500)
                    .Matches("[A-Z]+")
                    .Matches("[a-z]+")
                    .Matches("[0-9]+")
                    .Matches("[^A-Za-z0-9]+")
                    .WithMessage("Password must be at least 14 characters long and contain at least one upper case character, one lower case, a number and a special character.");
            });
        }
    }
}
