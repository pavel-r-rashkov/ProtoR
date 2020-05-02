namespace ProtoR.Application.Group
{
    using MediatR;

    public class CreateGroupCommand : IRequest<bool>
    {
        public string Name { get; set; }
    }
}
