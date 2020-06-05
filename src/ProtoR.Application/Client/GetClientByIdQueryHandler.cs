namespace ProtoR.Application.Client
{
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;

    public class GetClientByIdQueryHandler : IRequestHandler<GetClientByIdQuery, ClientDto>
    {
        private readonly IClientDataProvider clientData;

        public GetClientByIdQueryHandler(IClientDataProvider clientData)
        {
            this.clientData = clientData;
        }

        public async Task<ClientDto> Handle(GetClientByIdQuery request, CancellationToken cancellationToken)
        {
            return await this.clientData.GetById(request.ClientId);
        }
    }
}
