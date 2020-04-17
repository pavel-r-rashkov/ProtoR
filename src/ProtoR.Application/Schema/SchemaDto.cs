namespace ProtoR.Application.Schema
{
    using System;

    public class SchemaDto
    {
        public int Id { get; set; }

        public string Version { get; set; }

        public string Contents { get; set; }

        public DateTime CreatedOn { get; set; }

        public string CreatedBy { get; set; }
    }
}
