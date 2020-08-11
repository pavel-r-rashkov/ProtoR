namespace ProtoR.Application.Configuration
{
    using MediatR;

    public class GetByGroupNameQuery : IRequest<ConfigurationDto>
    {
        public string Name { get; set; }
    }
}
