namespace ProtoR.Application.Client
{
    using MediatR;

    public class GetClientByIdQuery : IRequest<ClientDto>
    {
        public long ClientId { get; set; }
    }
}
