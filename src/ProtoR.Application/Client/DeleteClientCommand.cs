namespace ProtoR.Application.Client
{
    using MediatR;

    public class DeleteClientCommand : IRequest
    {
        public long ClientId { get; set; }
    }
}
