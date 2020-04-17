namespace ProtoR.Web.Validators
{
    using System.Text.RegularExpressions;
    using FluentValidation;
    using ProtoR.Application.Schema;

    public class GetByVersionQueryValidator : AbstractValidator<GetByVersionQuery>
    {
        public GetByVersionQueryValidator()
        {
            this.RuleFor(m => m.Version)
                .Matches(@"^(\d+|latest)$", RegexOptions.IgnoreCase)
                .WithMessage("Version must be a number or \"latest\"");
        }
    }
}
