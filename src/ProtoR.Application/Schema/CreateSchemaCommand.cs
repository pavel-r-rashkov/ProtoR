namespace ProtoR.Application.Schema
{
    using MediatR;

    public class CreateSchemaCommand : IRequest<SchemaValidationResultDto>
    {
        public string GroupName { get; set; }

        public string Contents { get; set; }
    }
}