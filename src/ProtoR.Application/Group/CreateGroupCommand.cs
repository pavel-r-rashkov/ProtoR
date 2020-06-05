namespace ProtoR.Application.Group
{
    using MediatR;

    public class CreateGroupCommand : IRequest<bool>
    {
        public string GroupName { get; set; }

        public long? CategoryId { get; set; }
    }
}
