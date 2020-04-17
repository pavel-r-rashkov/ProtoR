namespace ProtoR.Web.Validators
{
    using FluentValidation;
    using ProtoR.Web.Resources.GroupResource;

    public class GroupWriteModelValidator : AbstractValidator<GroupWriteModel>
    {
        public GroupWriteModelValidator()
        {
            this.RuleFor(group => group.Name)
                .NotNull()
                .MinimumLength(1)
                .MaximumLength(500);
        }
    }
}
