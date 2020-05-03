namespace ProtoR.Application.Group
{
    using MediatR;

    public class DeleteGroupCommand : IRequest
    {
        public string GroupName { get; set; }
    }
}
