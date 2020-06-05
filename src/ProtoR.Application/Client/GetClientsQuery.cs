namespace ProtoR.Application.Client
{
    using System.Collections.Generic;
    using MediatR;

    public class GetClientsQuery : IRequest<IEnumerable<ClientDto>>
    {
    }
}
