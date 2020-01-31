namespace Web.Features.Test
{
    using FluentValidation;

    public class TestRequestValidator : AbstractValidator<TestRequest>
    {
        public TestRequestValidator()
        {
            this.RuleFor(r => r.FirstName).MaximumLength(3);
        }
    }
}
