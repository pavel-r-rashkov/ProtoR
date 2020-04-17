namespace ProtoR.Application.Group
{
    using MediatR;

    public class CreateGroupCommand : IRequest
    {
        public string Name { get; set; }
    }
}
