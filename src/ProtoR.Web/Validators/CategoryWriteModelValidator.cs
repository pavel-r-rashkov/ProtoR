namespace ProtoR.Web.Validators
{
    using FluentValidation;
    using ProtoR.Web.Resources.CategoryResource;

    public class CategoryWriteModelValidator : AbstractValidator<CategoryWriteModel>
    {
        public CategoryWriteModelValidator()
        {
            this.RuleFor(config => config.Name)
                .NotNull()
                .NotEmpty()
                .MaximumLength(500);
        }
    }
}
