namespace ProtoR.Application.Client
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;

    public class GetClientsQueryHandler : IRequestHandler<GetClientsQuery, IEnumerable<ClientDto>>
    {
        private readonly IClientDataProvider clientData;

        public GetClientsQueryHandler(IClientDataProvider clientData)
        {
            this.clientData = clientData;
        }

        public async Task<IEnumerable<ClientDto>> Handle(GetClientsQuery request, CancellationToken cancellationToken)
        {
            return await this.clientData.GetClients();
        }
    }
}
