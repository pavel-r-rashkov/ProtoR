namespace ProtoR.Application.Group
{
    using MediatR;

    public class UpdateGroupCommand : IRequest
    {
        public string GroupName { get; set; }

        public long CategoryId { get; set; }
    }
}
