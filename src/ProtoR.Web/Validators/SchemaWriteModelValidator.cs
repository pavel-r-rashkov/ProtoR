namespace ProtoR.Web.Validators
{
    using FluentValidation;
    using ProtoR.Web.Resources.SchemaResource;

    public class SchemaWriteModelValidator : AbstractValidator<SchemaWriteModel>
    {
        public SchemaWriteModelValidator()
        {
            this.RuleFor(s => s.Contents)
                .NotNull()
                .NotEmpty();

            this.RuleFor(s => s.GroupName)
                .NotNull()
                .NotEmpty();
        }
    }
}
