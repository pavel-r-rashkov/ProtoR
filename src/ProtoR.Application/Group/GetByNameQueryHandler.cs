namespace ProtoR.Application.Group
{
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;

    public class GetByNameQueryHandler : IRequestHandler<GetByNameQuery, GroupDto>
    {
        public Task<GroupDto> Handle(GetByNameQuery request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new GroupDto());
        }
    }
}
