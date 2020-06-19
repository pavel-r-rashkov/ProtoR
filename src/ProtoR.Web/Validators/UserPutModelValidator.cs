namespace ProtoR.Web.Validators
{
    using FluentValidation;
    using ProtoR.Web.Resources.UserResource;

    public class UserPutModelValidator : AbstractValidator<UserPutModel>
    {
        public UserPutModelValidator()
        {
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
