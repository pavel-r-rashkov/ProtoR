namespace ProtoR.Application.Schema
{
    using MediatR;

    public class ValidateSchemaCommand : IRequest<SchemaValidationResultDto>
    {
        public string GroupName { get; set; }

        public string Contents { get; set; }
    }
}
