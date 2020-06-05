namespace ProtoR.Web.Validators
{
    using System.Text.RegularExpressions;
    using FluentValidation;
    using ProtoR.Application.Category;

    public class GetCategoryByIdQueryValidator : AbstractValidator<GetCategoryByIdQuery>
    {
        public GetCategoryByIdQueryValidator()
        {
            this.RuleFor(m => m.CategoryId)
                .Matches(@"^(\d+|default)$", RegexOptions.IgnoreCase)
                .WithMessage("Category must be a number or \"default\"");
        }
    }
}
