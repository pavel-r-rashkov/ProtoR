namespace ProtoR.Application.Client
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using ProtoR.Domain.CategoryAggregate;
    using ProtoR.Domain.ClientAggregate;
    using ProtoR.Domain.RoleAggregate;
    using ProtoR.Domain.SeedWork;

    public class CreateClientCommandHandler : IRequestHandler<CreateClientCommand, long>
    {
        private readonly IClientRepository clientRepository;
        private readonly IUnitOfWork unitOfWork;

        public CreateClientCommandHandler(
            IClientRepository clientRepository,
            IUnitOfWork unitOfWork)
        {
            this.clientRepository = clientRepository;
            this.unitOfWork = unitOfWork;
        }

        public async Task<long> Handle(CreateClientCommand request, CancellationToken cancellationToken)
        {
            _ = request ?? throw new ArgumentNullException(nameof(request));

            var client = new Client(
                default,
                request.ClientId,
                request.ClientName,
                request.Secret?.ComputeSha256(),
                request.GrantTypes.ToList(),
                request.RedirectUris.Select(u => new Uri(u)).ToList(),
                request.PostLogoutRedirectUris.Select(u => new Uri(u)).ToList(),
                request.AllowedCorsOrigins.ToList(),
                request.RoleBindings.Select(rb => new RoleBinding(rb, null, 0)).ToList(),
                request.CategoryBindings.Select(cb => new CategoryBinding(cb, null, 0)).ToList());

            var id = await this.clientRepository.Add(client);
            await this.unitOfWork.SaveChanges();

            return id;
        }
    }
}
