namespace ProtoR.Web.Validators
{
    using FluentValidation;
    using ProtoR.Application.User;
    using ProtoR.Web.Resources.UserResource;

    public class GetUsersQueryValidator : AbstractValidator<GetUsersQuery>
    {
        public GetUsersQueryValidator()
        {
            this.RuleFor(q => q.Pagination)
                .SetValidator(new PaginationValidator());

            var allowedProperties = new string[]
            {
                nameof(UserReadModel.Id),
                nameof(UserReadModel.UserName),
                nameof(UserReadModel.CreatedOn),
                nameof(UserReadModel.CreatedBy),
            };

            this.RuleForEach(q => q.OrderBy)
                .SetValidator(new SortOrderValidator(allowedProperties));

            this.RuleForEach(q => q.Filter)
                .SetValidator(new FilterValidator(allowedProperties));
        }
    }
}
