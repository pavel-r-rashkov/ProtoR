namespace ProtoR.Application.Schema
{
    using System;

    public class SchemaDto
    {
        public long Id { get; set; }

        public int Version { get; set; }

        public string Contents { get; set; }

        public DateTime CreatedOn { get; set; }

        public string CreatedBy { get; set; }
    }
}
