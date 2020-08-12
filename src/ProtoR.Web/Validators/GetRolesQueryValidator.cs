namespace ProtoR.Web.Validators
{
    using FluentValidation;
    using ProtoR.Application.Role;
    using ProtoR.Web.Resources.RoleResource;

    public class GetRolesQueryValidator : AbstractValidator<GetRolesQuery>
    {
        public GetRolesQueryValidator()
        {
            this.RuleFor(q => q.Pagination)
                .SetValidator(new PaginationValidator());

            var allowedProperties = new string[]
            {
                nameof(RoleReadModel.Id),
                nameof(RoleReadModel.Name),
                nameof(RoleReadModel.CreatedOn),
                nameof(RoleReadModel.CreatedBy),
            };

            this.RuleForEach(q => q.OrderBy)
                .SetValidator(new SortOrderValidator(allowedProperties));

            this.RuleForEach(q => q.Filter)
                .SetValidator(new FilterValidator(allowedProperties));
        }
    }
}
