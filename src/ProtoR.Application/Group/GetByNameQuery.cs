namespace ProtoR.Application.Group
{
    using MediatR;

    public class GetByNameQuery : IRequest<GroupDto>
    {
        public string GroupName { get; set; }
    }
}
