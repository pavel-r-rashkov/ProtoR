namespace ProtoR.Application.Schema
{
    using System.Collections.Generic;

    public class CreateSchemaCommandResult
    {
        public IEnumerable<RuleViolationDto> RuleViolations { get; set; }

        public string NewVersion { get; set; }

        public string SchemaParseErrors { get; set; }
    }
}
