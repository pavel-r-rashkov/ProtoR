namespace ProtoR.Web.Validators
{
    using FluentValidation;
    using ProtoR.Application.Schema;
    using ProtoR.Web.Resources.SchemaResource;

    public class GetGroupSchemasQueryValidator : AbstractValidator<GetGroupSchemasQuery>
    {
        public GetGroupSchemasQueryValidator()
        {
            this.RuleFor(q => q.Pagination)
                .SetValidator(new PaginationValidator());

            var allowedProperties = new string[]
            {
                nameof(SchemaReadModel.Id),
                nameof(SchemaReadModel.Version),
                nameof(SchemaReadModel.Contents),
                nameof(SchemaReadModel.CreatedOn),
                nameof(SchemaReadModel.CreatedBy),
            };

            this.RuleForEach(q => q.OrderBy)
                .SetValidator(new SortOrderValidator(allowedProperties));

            this.RuleForEach(q => q.Filter)
                .SetValidator(new FilterValidator(allowedProperties));
        }
    }
}
