namespace ProtoR.Application.Client
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using ProtoR.Domain.ClientAggregate;
    using ProtoR.Domain.SeedWork;

    public class UpdateClientCommandHandler : AsyncRequestHandler<UpdateClientCommand>
    {
        private readonly IClientRepository clientRepository;
        private readonly IUnitOfWork unitOfWork;

        public UpdateClientCommandHandler(
            IClientRepository clientRepository,
            IUnitOfWork unitOfWork)
        {
            this.clientRepository = clientRepository;
            this.unitOfWork = unitOfWork;
        }

        protected override async Task Handle(UpdateClientCommand request, CancellationToken cancellationToken)
        {
            var client = await this.clientRepository.GetById(request.Id);

            client.ClientId = request.ClientId;
            client.ClientName = request.ClientName;

            if (!string.IsNullOrEmpty(request.Secret))
            {
                client.Secret = request.Secret.ComputeSha256();
            }

            client.GrantTypes = request.GrantTypes.ToList();
            client.RedirectUris = request.RedirectUris.Select(u => new Uri(u)).ToList();
            client.PostLogoutRedirectUris = request.PostLogoutRedirectUris.Select(u => new Uri(u)).ToList();
            client.AllowedCorsOrigins = request.AllowedCorsOrigins.ToList();
            client.SetRoles(request.RoleBindings);
            client.SetCategories(request.CategoryBindings);

            await this.clientRepository.Update(client);
            await this.unitOfWork.SaveChanges();
        }
    }
}
