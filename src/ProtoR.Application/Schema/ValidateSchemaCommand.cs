namespace ProtoR.Application.Schema
{
    using MediatR;

    public class ValidateSchemaCommand : IRequest<SchemaValidationResultDto>
    {
        public string Name { get; set; }

        public string Contents { get; set; }
    }
}
