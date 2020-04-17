namespace ProtoR.Application.Group
{
    using System.Collections.Generic;
    using MediatR;

    public class GetGroupsQuery : IRequest<IEnumerable<GroupDto>>
    {
    }
}
