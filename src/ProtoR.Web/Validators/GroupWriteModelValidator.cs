namespace ProtoR.Web.Validators
{
    using FluentValidation;
    using ProtoR.Web.Resources.GroupResource;

    public class GroupWriteModelValidator : AbstractValidator<GroupWriteModel>
    {
        public GroupWriteModelValidator()
        {
            this.RuleFor(group => group.GroupName)
                .NotNull()
                .NotEmpty()
                .MaximumLength(500);
        }
    }
}