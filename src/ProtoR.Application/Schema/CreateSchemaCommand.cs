namespace ProtoR.Application.Schema
{
    using MediatR;

    public class CreateSchemaCommand : IRequest<CreateSchemaCommandResult>
    {
        public string GroupName { get; set; }

        public string Contents { get; set; }
    }
}
