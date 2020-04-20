namespace ProtoR.Web.Resources.SchemaResource
{
    using System;

    public class SchemaReadModel
    {
        public int Id { get; set; }

        public int Version { get; set; }

        public DateTime CreatedOn { get; set; }

        public string CreatedBy { get; set; }
    }
}
