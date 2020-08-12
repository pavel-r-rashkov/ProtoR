namespace ProtoR.Web.Validators
{
    using FluentValidation;
    using ProtoR.Application.Common;

    public class PaginationValidator : AbstractValidator<Pagination>
    {
        private readonly int maxSize;

        public PaginationValidator()
        {
            this.maxSize = 100;
            this.ConfigureValidation();
        }

        public PaginationValidator(int maxSize)
        {
            this.maxSize = maxSize;
            this.ConfigureValidation();
        }

        private void ConfigureValidation()
        {
            this.RuleFor(p => p.Size)
                .GreaterThan(0)
                .LessThanOrEqualTo(this.maxSize);

            this.RuleFor(p => p.Page)
                .GreaterThan(0);
        }
    }
}
