namespace ProtoR.Application.Client
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using ProtoR.Domain.ClientAggregate;
    using ProtoR.Domain.Exceptions;

    public class GetClientByIdQueryHandler : IRequestHandler<GetClientByIdQuery, ClientDto>
    {
        private readonly IClientDataProvider clientData;

        public GetClientByIdQueryHandler(IClientDataProvider clientData)
        {
            this.clientData = clientData;
        }

        public async Task<ClientDto> Handle(GetClientByIdQuery request, CancellationToken cancellationToken)
        {
            _ = request ?? throw new ArgumentNullException(nameof(request));
            var client = await this.clientData.GetById(request.ClientId);

            if (client == null)
            {
                throw new EntityNotFoundException<Client>(request.ClientId);
            }

            return client;
        }
    }
}
