namespace ProtoR.Web.Validators
{
    using FluentValidation;
    using ProtoR.Application.Group;
    using ProtoR.Web.Resources.GroupResource;

    public class GetGroupsQueryValidator : AbstractValidator<GetGroupsQuery>
    {
        public GetGroupsQueryValidator()
        {
            this.RuleFor(q => q.Pagination)
                .SetValidator(new PaginationValidator());

            var allowedProperties = new string[]
            {
                nameof(GroupReadModel.Id),
                nameof(GroupReadModel.Name),
                nameof(GroupReadModel.CreatedOn),
                nameof(GroupReadModel.CreatedBy),
            };

            this.RuleForEach(q => q.OrderBy)
                .SetValidator(new SortOrderValidator(allowedProperties));

            this.RuleForEach(q => q.Filter)
                .SetValidator(new FilterValidator(allowedProperties));
        }
    }
}
