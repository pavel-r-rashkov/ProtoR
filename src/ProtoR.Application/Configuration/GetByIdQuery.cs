namespace ProtoR.Application.Configuration
{
    using MediatR;

    public class GetByIdQuery : IRequest<ConfigurationDto>
    {
        public string ConfigurationId { get; set; }
    }
}
