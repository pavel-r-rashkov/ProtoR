namespace ProtoR.Web.Validators
{
    using FluentValidation;
    using ProtoR.Application.Client;
    using ProtoR.Web.Resources.ClientResource;

    public class GetClientsQueryValidator : AbstractValidator<GetClientsQuery>
    {
        public GetClientsQueryValidator()
        {
            this.RuleFor(q => q.Pagination)
                .SetValidator(new PaginationValidator());

            var allowedProperties = new string[]
            {
                nameof(ClientReadModel.Id),
                nameof(ClientReadModel.ClientId),
                nameof(ClientReadModel.ClientName),
                nameof(ClientReadModel.CreatedOn),
                nameof(ClientReadModel.CreatedBy),
            };

            this.RuleForEach(q => q.OrderBy)
                .SetValidator(new SortOrderValidator(allowedProperties));

            this.RuleForEach(q => q.Filter)
                .SetValidator(new FilterValidator(allowedProperties));
        }
    }
}
