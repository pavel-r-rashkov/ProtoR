namespace ProtoR.Web.Resources.SchemaResource
{
    using ProtoR.Web.Swagger;

    public class SchemaWriteModel
    {
        [SwaggerExclude]
        public string GroupName { get; set; }

        public string Contents { get; set; }
    }
}
