namespace ProtoR.Application.Configuration
{
    using MediatR;

    public class GetByGroupNameQuery : IRequest<ConfigurationDto>
    {
        public string GroupName { get; set; }
    }
}
