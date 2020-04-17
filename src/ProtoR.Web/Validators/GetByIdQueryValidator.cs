namespace ProtoR.Web.Validators
{
    using FluentValidation;
    using ProtoR.Application.Configuration;

    public class GetByIdQueryValidator : AbstractValidator<GetByIdQuery>
    {
        public GetByIdQueryValidator()
        {
            this.RuleFor(m => m.ConfigurationId).Matches(@"^(\d+|global)$");
        }
    }
}
