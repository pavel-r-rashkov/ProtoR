namespace ProtoR.Application.Client
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using ProtoR.Application.Common;

    public class GetClientsQueryHandler : IRequestHandler<GetClientsQuery, PagedResult<ClientDto>>
    {
        private readonly IClientDataProvider clientData;

        public GetClientsQueryHandler(IClientDataProvider clientData)
        {
            this.clientData = clientData;
        }

        public async Task<PagedResult<ClientDto>> Handle(GetClientsQuery request, CancellationToken cancellationToken)
        {
            return await this.clientData.GetClients(
                request.Filter,
                request.OrderBy,
                request.Pagination);
        }
    }
}
